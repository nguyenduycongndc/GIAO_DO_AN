using APIProject.Models;
using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using N.EntityFramework.Extensions;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace Data.Business
{
    public class NotifyBusiness : GenericBusiness
    {
        OneSignalBusiness oneSignalBus;
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);

        public NotifyBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
            oneSignalBus = new OneSignalBusiness(this.cnn);
        }

        public int SendNotification(int Type, int TypeSend, string Title, string Content)
        {
            try
            {
                PushNotifyBusiness pushnoti = new PushNotifyBusiness();
                //if (Content.Length > 300)
                //    return resultBus.ErrorResult("Nội dung quá dài");
                //if (title.Length > 100)
                //    return resultBus.ErrorResult("The title is too long");
                var listMember = (TypeSend == Constant.CUSTOMER_ROLE ? cnn.Members.Where(c => c.IsActive == SystemParam.ACTIVE && c.CustomerID.HasValue).ToList() : (TypeSend == Constant.SHIPER_ROLE ? cnn.Members.Where(c => c.ShiperID.HasValue && c.IsActive == SystemParam.ACTIVE).ToList() : cnn.Members.Where(c => c.ShopID.HasValue && c.IsActive == SystemParam.ACTIVE).ToList()));
                string code = "";

                DateTime date = DateTime.Now;
                if (code.Length == 0)
                    code = Util.CreateMD5(date.ToString()).Substring(0, 9);
                List<string> listDeviceCustomer = new List<string>();
                List<string> listDeviceIDAgent = new List<string>();
                foreach (var member in listMember)
                {
                    Notification noti = new Notification();
                    noti.MemberID = member.ID;
                    noti.IsRead = false;
                    noti.CreateDate = date;
                    noti.IsActive = (Type == SystemParam.SAVE_NOTIFICATION ? 2 : (Type == SystemParam.SEND_NOTIFICATION ? SystemParam.ACTIVE : 0));
                    noti.Title = Title;
                    noti.Type = Constant.NOTI_FROM_ADMIN;
                    noti.Content = Content;
                    noti.Code = code;
                    cnn.Notifications.Add(noti);
                    if (!String.IsNullOrEmpty(member.DeviceID) && member.DeviceID.Length > 0)
                    {
                        listDeviceCustomer.Add(member.DeviceID);
                    }
                }
                cnn.SaveChanges();
                if (Type == SystemParam.SEND_NOTIFICATION)
                {
                    var res = pushnoti.PushNotify(null, listDeviceCustomer, Title, Content, TypeSend == Constant.CUSTOMER_ROLE ? 1 : 2);
                    pushnoti.PushOneSignal(res, TypeSend == Constant.CUSTOMER_ROLE ? 1 : 2);
                }
                else
                {
                    return SystemParam.SUCCESS;
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return SystemParam.FAIL;
            }
        }
        public string GetJson(string url)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(url));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            Console.WriteLine(WebResp.StatusCode);
            Console.WriteLine(WebResp.Server);

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            return jsonString;
        }

        //Tìm kiếm thông báo phía admin
        public IPagedList<NotiAdminModel> Search(int page, string searchKey, int? type, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.Notifications.Where(n => n.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(searchKey) ? n.Title.Contains(searchKey) : true)
                && n.Type.Equals(SystemParam.NOTY_BY_ADMIN)
                && (type.HasValue && type == SystemParam.TYPE_PUSH_TO_CUSTOMERS ? n.Member.CustomerID.HasValue : true)
                && (type.HasValue && type == SystemParam.TYPE_PUSH_TO_SHIPERS ? n.Member.ShiperID.HasValue : true)
                && (type.HasValue && type == SystemParam.TYPE_PUSH_TO_SHOPS ? n.Member.ShopID.HasValue : true)
                && (fd.HasValue ? n.CreateDate >= fd.Value : true) && (td.HasValue ? n.CreateDate <= td.Value : true))
                    .GroupBy(n => n.Code).OrderByDescending(n => n.FirstOrDefault().ID)
                    .Select(n => new NotiAdminModel
                    {
                        Code = n.FirstOrDefault().Code,
                        Content = n.FirstOrDefault().Content,
                        Title = n.FirstOrDefault().Title,
                        CreateDate = n.FirstOrDefault().CreateDate,
                        Type = n.FirstOrDefault().Description,
                        Count = n.Count()
                    }).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<NotiAdminModel>().ToPagedList(1, 1);
            }
        }

        public JsonResultModel GetListNotify(int CusID, int? type, int page, int limit)
        {
            try
            {
                List<NotifiedByCustomerIDOutputModel> data = new List<NotifiedByCustomerIDOutputModel>();
                NotifiedByCustomerOutput ouput = new NotifiedByCustomerOutput();
                data = cnn.Notifications.Where(n => n.MemberID.Equals(CusID) && n.IsActive.Equals(SystemParam.ACTIVE) && (type.HasValue ? n.Type == type : true))

                                 .Select(n => new NotifiedByCustomerIDOutputModel
                                 {
                                     NotifyID = n.ID,
                                     Content = n.Content,
                                     CreatedDate = n.CreateDate,
                                     Viewed = n.IsRead.Equals(true) ? 1 : 0,
                                     Title = n.Title,
                                     Type = n.Type,
                                     OrderServiceID = n.OrderServiceID,
                                     newsID = n.NewsID
                                 }).OrderByDescending(n => n.NotifyID).ToList();
                int count = data.Count();
                double totalPage = (double)count / (double)limit;
                ouput.viewed = data.Any(x => x.Viewed == 0) ? 0 : 1;
                ouput.limit = limit;
                ouput.totalCount = count;
                ouput.page = page;
                ouput.lastPage = (int)Math.Ceiling(totalPage);
                ouput.listNoti = data.ToPagedList(page, limit);
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, ouput);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public JsonResultModel CheckViewdNoti(int MemberID)
        {
            try
            {
                var check = cnn.Notifications.Where(x => x.MemberID == MemberID).Any(x => x.IsRead == false);
                if (check == true)
                {
                    return rpBus.SuccessResult(MessVN.SUCCESS_STR, 0);
                }
                else
                {
                    return rpBus.SuccessResult(MessVN.SUCCESS_STR, 1);
                }
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public JsonResultModel ViewNotify(int ID, int readAll, int memberID)
        {
            try
            {
                if (ID > 0)
                {
                    var notify = cnn.Notifications.Find(ID);
                    notify.IsRead = true;
                }

                if (readAll.Equals(1))
                {
                    var lstNoti = cnn.Notifications.Where(n => n.IsActive.Equals(SystemParam.ACTIVE) && n.MemberID.Equals(memberID) && n.IsRead.Equals(false))
                 .ToList().All(n => n.IsRead = true);
                }
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }

        }

        public JsonResultModel PushNotiToMember(NotifyInputModel input)
        {
            if (String.IsNullOrEmpty(input.title) || String.IsNullOrEmpty(input.content))
                return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
            try
            {
                PushNotifyBusiness nt = new PushNotifyBusiness();
                List<Member> lstMember = new List<Member>();

                switch (input.type)
                {
                    case SystemParam.TYPE_PUSH_ALL:
                        lstMember = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                        break;
                    case SystemParam.TYPE_PUSH_TO_CUSTOMERS:
                        lstMember = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && m.CustomerID.HasValue).ToList();
                        break;
                    case SystemParam.TYPE_PUSH_TO_SHIPERS:
                        lstMember = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && m.ShiperID.HasValue).ToList();
                        break;
                    case SystemParam.TYPE_PUSH_TO_SHOPS:
                        lstMember = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && m.ShopID.HasValue).ToList();
                        break;
                    default:
                        break;
                }
                //Lưu lại thông báo
                if (lstMember.Count() > 0)
                {
                    string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10);
                    List<Notification> lstNoti = lstMember.Select(n => new Notification
                    {
                        Code = code,
                        MemberID = n.ID,
                        Content = input.content,
                        Title = input.title,
                        IsRead = false,
                        Type = SystemParam.NOTY_BY_ADMIN,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        Description = input.type.ToString()
                    }).ToList();
                    cnn.BulkInsert(lstNoti);
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.id = 0;
                    notifyData.type = SystemParam.NOTY_BY_ADMIN;
                    notifyData.code = "";
                    notifyData.content = input.content;
                    List<string> lstDeviceIDAppCus = lstMember.Where(m => !String.IsNullOrEmpty(m.DeviceID) && m.DeviceID.Length > 15 && m.CustomerID.HasValue).Select(m => m.DeviceID).ToList();
                    List<string> lstDeviceIDAppDriver = lstMember.Where(m => !String.IsNullOrEmpty(m.DeviceID) && m.DeviceID.Length > 15 && !m.CustomerID.HasValue).Select(m => m.DeviceID).ToList();
                    var notiCus =nt.PushNotify(notifyData, lstDeviceIDAppCus, input.title, input.content, 1);
                    var notiDriver = nt.PushNotify(notifyData, lstDeviceIDAppDriver, input.title, input.content, 2);
                    nt.PushOneSignal(notiCus, 1);
                    nt.PushOneSignal(notiDriver, 2);
                }
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
    }
}
