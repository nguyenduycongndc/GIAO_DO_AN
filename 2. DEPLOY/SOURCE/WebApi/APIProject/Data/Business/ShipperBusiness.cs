using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using Data.Model.APIWeb;
using Data.Utils;
using Data.Model;
using SharpRaven;
using SharpRaven.Data;
using APIProject.Models;
using OfficeOpenXml;
using System.IO;
using System.Web;

namespace Data.Business
{
    public class ShipperBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        PushNotifyBusiness pushNotifyBusiness = new PushNotifyBusiness();
        public ShipperBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public IPagedList<ShipperOutputModel> Search(int Page, int? Status, string Key , int? provinceID, int? districtID,int? IsVip, string FromDate , string ToDate)
        {
            try
            {
                DateTime? fd = Util.ConvertFromDate(FromDate);
                DateTime? td = Util.ConvertToDate(ToDate);
                var query = (from s in cnn.Shipers
                             where s.Members.Any(m => m.IsActive > SystemParam.ACTIVE_FALSE)
                             && (provinceID.HasValue ? s.ShiperAreas.Select(x => x.Area.District.ProvinceID).Contains(provinceID.Value) : true)
                             && (districtID.HasValue ? s.ShiperAreas.Select(x => x.Area.DistrictID).Contains(districtID.Value) : true)
                             && (!String.IsNullOrEmpty(Key) ? s.Name.Contains(Key) || s.Code.Contains(Key) || s.Email.Contains(Key) || s.Phone.Contains(Key) : true)
                             && (fd.HasValue ? s.CreatedDate >= fd.Value : true)
                             && (td.HasValue ? s.CreatedDate <= td.Value : true)
                             && (Status.HasValue ? s.Members.Any(m => m.IsActive == Status) : true)
                             && (IsVip.HasValue ? (IsVip.Value.Equals(SystemParam.SHIPPER_VIP) ? s.IsVip == IsVip : (s.IsVip.Value.Equals(SystemParam.SHIPPER_NORMAL) || !s.IsVip.HasValue)) : true)
                             orderby s.CreatedDate descending
                             select new ShipperOutputModel
                             {
                                 ID = s.ID,
                                 Code = s.Code,
                                 Name = s.Name,
                                 Phone = s.Phone,
                                 Email = s.Email,
                                 IsVip = s.IsVip.HasValue ? s.IsVip.Value : SystemParam.SHIPPER_NORMAL,
                                 CreatedDate = s.CreatedDate,
                                 IsActive = s.Members.FirstOrDefault().IsActive,
                                 Rating = s.Rating
                             }).ToPagedList(Page, 20);
                var a = query.Count();
                return query;

            }
            catch (Exception e)
            {
                e.ToString();
                return new List<ShipperOutputModel>().ToPagedList(1, 1);
            }
        }
        public List<ShipperOutputModel> GetDataListShipper(int? Status, string Key , int? provinceID, int? districtID, int? IsVip, string FromDate , string ToDate)
        {
            try
            {
                DateTime? fd = Util.ConvertFromDate(FromDate);
                DateTime? td = Util.ConvertToDate(ToDate);
                var query = (from s in cnn.Shipers
                             where s.Members.Any(m => m.IsActive > SystemParam.ACTIVE_FALSE)
                             && (provinceID.HasValue ? s.ShiperAreas.Select(x => x.Area.District.ProvinceID).Contains(provinceID.Value) : true)
                             && (districtID.HasValue ? s.ShiperAreas.Select(x => x.Area.DistrictID).Contains(districtID.Value) : true)
                             && (!String.IsNullOrEmpty(Key) ? s.Name.Contains(Key) || s.Code.Contains(Key) || s.Email.Contains(Key) || s.Phone.Contains(Key) : true)
                             && (fd.HasValue ? s.CreatedDate >= fd.Value : true)
                             && (td.HasValue ? s.CreatedDate <= td.Value : true)
                             && (Status.HasValue ? s.Members.Any(m => m.IsActive == Status) : true)
                             && (IsVip.HasValue ? (IsVip.Value.Equals(SystemParam.SHIPPER_VIP) ? s.IsVip == IsVip : (s.IsVip.Value.Equals(SystemParam.SHIPPER_NORMAL) || !s.IsVip.HasValue)) : true)
                             orderby s.CreatedDate descending
                             select new ShipperOutputModel
                             {
                                 ID = s.ID,
                                 Code = s.Code,
                                 Name = s.Name,
                                 Phone = s.Phone,
                                 Email = s.Email,
                                 IsVip = s.IsVip.HasValue ? s.IsVip.Value : SystemParam.SHIPPER_NORMAL,
                                 CreatedDate = s.CreatedDate,
                                 IsActive = s.Members.FirstOrDefault().IsActive,
                                 Rating = s.Rating
                             }).ToList();
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<ShipperOutputModel>();
            }
        }
        public ExcelPackage ExportListShipper(int? Status, string Key, int? provinceID, int? districtID, int? IsVip, string FromDate, string ToDate)
        {
            try
            {
                List<ShipperOutputModel> data = GetDataListShipper(Status, Key, provinceID, districtID, IsVip, FromDate, ToDate);
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/listShipper.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;
                if (data != null && data.Count() > 0)
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = stt;
                        sheet.Cells[row, 2].Value = dt.Code;
                        sheet.Cells[row, 3].Value = dt.Name;
                        sheet.Cells[row, 4].Value = dt.Phone;
                        sheet.Cells[row, 5].Value = dt.Email;
                        switch (dt.IsVip)
                        {
                            case SystemParam.SHIPPER_VIP:
                                sheet.Cells[row, 6].Value = "VIP";
                                break;
                            case SystemParam.SHIPPER_NORMAL:
                                sheet.Cells[row, 6].Value = "Thường";
                                break;
                        }

                        sheet.Cells[row, 7].Value = dt.Rating;
                        sheet.Cells[row, 8].Value = dt.CreatedDate.ToString(SystemParam.CONVERT_DATETIME);
                        switch (dt.IsActive)
                        {
                            case SystemParam.ACTIVE:
                                sheet.Cells[row, 9].Value = "Hoạt động";
                                break;
                            case SystemParam.INACTIVE:
                                sheet.Cells[row, 9].Value = "Ngừng hoạt động";
                                break;
                        }

                        row++;
                        stt++;
                    }
                return pack;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ExcelPackage();
            }
        }
        //Count Shipper
        public async Task<int> CountShipper()
        {
            var count = cnn.Members.Where(x => x.ShiperID != null && x.IsActive.Equals(SystemParam.ACTIVE)).Count();

            return count;
        }
        public async Task<double> countNewShiperPercent()
        {
            try
            {
                var today = DateTime.Today;
                var first = new DateTime(today.Year, today.Month, 1);
                double ShiperLastMonth = cnn.Members.Where(x => x.ShiperID != null && x.CreatedDate <= first && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                if (ShiperLastMonth <= 0)
                {
                    return 0;
                }
                double ShiperNow = cnn.Members.Where(x => x.ShiperID != null && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                double ShiperNew = ShiperNow - ShiperLastMonth;
                return Math.Round((ShiperNew / ShiperLastMonth) * 100, 2);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public async Task<List<int>> getShiperEveryMonth()
        {
            try
            {
                var today = DateTime.Today;
                var listShiperCount = new List<int>();
                for (var i = 1; i <= today.Month; i++)
                {
                    var month = new DateTime(today.Year, i, 1);
                    var last = month.AddMonths(1);
                    var ShiperCount = cnn.Members.Where(x => x.ShiperID != null && x.CreatedDate <= last && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                    listShiperCount.Add(ShiperCount);
                }
                return listShiperCount;
            }
            catch (Exception e)
            {
                return new List<int>();
            }
        }
        public ShipperDetailModel GetDetailShipper(int ID)
        {
            try
            {
                var shipper = cnn.Shipers.Find(ID);
                var mem = cnn.Members.Where(m => m.ShiperID == ID).FirstOrDefault();
                ShipperDetailModel res = new ShipperDetailModel();
                res.ship = shipper;

                var lstOrder = (from c in cnn.OrderServices
                                where c.ShiperID == ID
                                orderby c.CreatedDate descending
                                select new OrderOutputModel()
                                {
                                    Code = c.Code,
                                    TotalPrice = c.OrderServiceDetails.GroupBy(od => od.ID).Select(od => od.Select(o => o.Price).Sum()).FirstOrDefault(),
                                    Status = c.Status,
                                    CompletedDate = c.CompletedDate,
                                    CreateDate = c.CreatedDate,
                                    NameService = c.CarTypeID == null ? "Đặt đồ ăn" : "Đặt xe",
                                }).ToPagedList(1, SystemParam.MAX_ROW_IN_LIST_WEB);
                var data = cnn.BankMembers.Where(b => b.IsActive.Equals(SystemParam.ACTIVE) && b.MemberID.Equals(mem.ID))
                    .Select(b => new ListBankOutputModelWeb
                    {
                        ID = b.ID,
                        Account = b.Account,
                        AccountOwner = b.AccountOwner,
                        BankName = b.Bank.Name,
                        BankID = b.BankID
                    }).OrderByDescending(b => b.ID).ToList();
                res.shipListBank = data;
                res.lstCustomerOrder = lstOrder;
                res.ID = ID;
                return res;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return null;
            }
        }
        public void CreateNoti(int id, int type, string Content, int Point)
        {
            Notification notify = new Notification
            {
                Title = "Hệ thống cộng " + Point + " điểm vì " + Content,
                Content = Content,
                Type = type,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now,
                IsRead = false,
                MemberID = id,
                Code = Util.CreateMD5(id + DateTime.Now.ToString()).Substring(0, 5),
                OrderServiceID = null

            };
            cnn.Notifications.Add(notify);
            cnn.SaveChanges();

            string memberDevice = cnn.Members.Where(x => x.IsActive == SystemParam.ACTIVE && x.ID == id).Select(x => x.DeviceID).FirstOrDefault();
            List<string> listDevice = new List<string>();
            if (memberDevice != null && memberDevice.Length > 10)
            {
                listDevice.Add(memberDevice);
            }

            cnn.SaveChanges();
        }
        public int AddPointShip(string ListIdPoint, string Point, string Content)
        {
            try
            {
                //UserDetailOutputModel userLogin = UserLogins;
                List<int> IDs = ListIdPoint.Split(',').Select(Int32.Parse).ToList();
                if (IDs.Count() == 0)
                    return SystemParam.ERROR;

                NotifyDataModel notifyData = new NotifyDataModel();
                List<Wallet> lstWallet = cnn.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.Type.Equals(Constant.WALLET_NO_WITHDRAW) && w.Member.ShiperID.HasValue && IDs.Contains(w.Member.ShiperID.Value)).ToList();

                //Tạo nội dung thông báo đến app
                notifyData.type = SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_WALLET_NO_WITHDRAW_BY_ADMIN;
                notifyData.code = "Nạp tiền từ hệ thống";
                notifyData.content = "Bạn được cộng " + Point + " điểm từ hệ thống với lý do : " + Content;
                //Tạo list notify
                List<Notification> lstNoti = lstWallet.Select(n => new Notification
                {
                    MemberID = n.MemberID,
                    Content = "Bạn được cộng " + Point + " điểm từ hệ thống với lý do : " + Content,
                    Title = "Nạp tiền từ hệ thống",
                    IsRead = false,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    Type = SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_WALLET_NO_WITHDRAW_BY_ADMIN,
                    Code = ""
                }).ToList();
                //Tạo lịch sử giao dịch
                List<MembersTransactionHistory> lstTrasactionHistory = lstWallet.Select(m => new MembersTransactionHistory
                {
                    MemberID = m.MemberID,
                    Title = "Nạp tiền từ hệ thống",
                    Content = "Bạn được cộng " + Point + " điểm từ hệ thống với lý do : " + Content,
                    Point = Int32.Parse(Point.Replace(",", "")),
                    TransactionType = Constant.TYPE_TRANSACTION_RECHARGE_ADMIN,
                    Type = Constant.RECHAGE,
                    Status = Constant.STATUS_TRANSACTION_SUCCESS,
                    BeforeBalance = m.Balance,
                    AfterBalance = m.Balance + Int32.Parse(Point.Replace(",", "")),
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    WalletID = m.ID,
                    Icon = Constant.TYPE_TRANSACTION_RECHARGE_ICON,
                    TransactionID = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10),
                    IsExtra = false
                }).ToList();

                List<string> listDevice = lstWallet.Where(m => !String.IsNullOrEmpty(m.Member.DeviceID) && m.Member.DeviceID.Length > 10).Select(m => m.Member.DeviceID).ToList();

                foreach (var w in lstWallet)
                {
                    w.Balance += Int32.Parse(Point.Replace(",", ""));
                }
                cnn.MembersTransactionHistories.AddRange(lstTrasactionHistory);
                cnn.Notifications.AddRange(lstNoti);
                cnn.SaveChanges();
                if (listDevice.Count() > 0)
                {
                    string value = pushNotifyBusiness.PushNotify(notifyData, listDevice, SystemParam.WE_SHIP_NOTI, notifyData.content, 2);
                    pushNotifyBusiness.PushOneSignal(value, 2);
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        // sửa Shipper
        public JsonResultModel EditShipper(ShiperInputModel input)
        {
            try
            {
                int checkPhone = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.ShiperID != input.ID && c.User.Equals(input.Phone) && (c.ShiperID.HasValue || c.ShopID.HasValue)).Count();
                int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.ShiperID != input.ID && (c.Customer.Email.Equals(input.Email) || c.Shop.Email.Equals(input.Email) || c.Shiper.Email.Equals(input.Email))
                                                && (c.ShiperID.HasValue || c.ShopID.HasValue)).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);

                bool checkPushNoti = false;
                List<int> lstI = input.ShipperProvinces.Substring(0, input.ShipperProvinces.Length - 1).Split(',').Select(Int32.Parse).ToList();

                DateTime? dob = !String.IsNullOrEmpty(input.DOB) ? Util.ConvertDate(input.DOB) : null;
                Shiper shiper = cnn.Shipers.Find(input.ID);

                //Check nếu thay đổi trạng thái shipper nội bộ thì sẽ gửi thông báo về app tài xế
                if (!shiper.IsInternal.Equals(input.IsInternal))
                    checkPushNoti = true;

                shiper.Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10);
                shiper.AvartarUrl = input.Avatar;
                shiper.Name = input.Name;
                shiper.DOB = dob;
                shiper.Identification = input.Identify;
                shiper.Phone = input.Phone;
                shiper.Email = input.Email;
                shiper.Address = input.Address;
                shiper.CommissionID = input.ComissionID;
                shiper.IsVip = input.IsVip;
                shiper.Address = input.Address;
                shiper.IsActive = SystemParam.ACTIVE;
                shiper.ModifyDate = DateTime.Now;
                shiper.IsInternal = input.IsInternal;
                shiper.Sex = input.Sex;
                //Sử thông tin xe shiper
                CarShiper cs = shiper.CarShipers.FirstOrDefault();

                cs.CarBrand = input.CarBrand;
                cs.CarModel = input.CarModel;
                cs.LicensePlates = input.LicensePlate;
                cs.VehicleTypeID = input.VehicleTypeID;
                cs.CarColor = "";
                Member m = shiper.Members.FirstOrDefault();
                m.User = input.Phone;
                //Add area for shipper
                List<ShiperArea> lstShiperArea = shiper.ShiperAreas.Where(a => a.ShiperID.Equals(shiper.ID)).ToList();
                cnn.ShiperAreas.RemoveRange(lstShiperArea);
                shiper.ShiperAreas = lstI.Select(u => new ShiperArea
                {
                    AreaID = u,
                    IsActive = SystemParam.ACTIVE,
                    CreatedDate = DateTime.Now
                }).ToList();

                //Xử lý ảnh của shiper
                List<string> lstImgCard = input.ImgIdentify.Substring(0, input.ImgIdentify.Length - 1).Split(',').ToList();
                List<ShiperImage> lstImgShiper = shiper.ShiperImages.ToList();
                cnn.ShiperImages.RemoveRange(lstImgShiper);
                List<ShiperImage> lishipImg = lstImgCard.Select(im => new ShiperImage
                {
                    ShiperID = shiper.ID,
                    IsActive = SystemParam.ACTIVE,
                    Path = im,
                    CreatedDate = DateTime.Now
                }).ToList();
                shiper.ShiperImages = lishipImg;
                cnn.SaveChanges();

                //Gửi thông báo về app tài xế khi checkPushNoti = true
                if (checkPushNoti.Equals(true) && shiper.Members.Any(mb => mb.IsActive > SystemParam.ACTIVE_FALSE && mb.ShiperID == shiper.ID && !String.IsNullOrEmpty(mb.DeviceID) && mb.DeviceID.Length > 10))
                {
                    var notifyData = new
                    {
                        type = SystemParam.NOTI_TYPE_CHANGE_INTERNAL_STATUS_SHHIPER,
                        isInterNal = input.IsInternal,
                        content = shiper.IsInternal.Equals(SystemParam.ACTIVE) ? "Bạn đã trở thành shipper nội bộ của hệ thống WE SHIP" : "Bạn đã trở thành shipper đối tác của hệ  thống WE SHIP"
                    };
                    List<string> listDevice = new List<string>(new string[] { shiper.Members.Where(mb => mb.IsActive > SystemParam.ACTIVE_FALSE && mb.ShiperID == shiper.ID).Select(mb => mb.DeviceID).FirstOrDefault() });

                    PushNotifyBusiness pushNotifyBusiness = new PushNotifyBusiness();
                    string value = pushNotifyBusiness.PushNotify(notifyData, listDevice, SystemParam.WE_SHIP_NOTI, notifyData.content, 2);
                    pushNotifyBusiness.PushOneSignal(value, 2);
                }

                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public ShipperDetailOutputModelWeb GetShiperDetail(int ID)
        {
            try
            {
                Shiper sp = cnn.Shipers.Find(ID);
                ShipperDetailOutputModelWeb data = new ShipperDetailOutputModelWeb();
                data.ID = sp.ID;
                data.Avatar = sp.AvartarUrl;
                data.Name = sp.Name;
                data.DOB = sp.DOB.HasValue ? sp.DOB.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
                data.Identify = sp.Identification;
                data.Sex = sp.Sex;
                data.Phone = sp.Phone;
                data.Email = sp.Email;
                data.Address = sp.Address;
                data.ComissionID = sp.CommissionID;
                data.IsInternal = sp.IsInternal;
                data.IsVip = sp.IsVip.GetValueOrDefault();
                data.VehicleTypeID = sp.CarShipers.Select(c => c.VehicleTypeID).FirstOrDefault();
                data.CarBrand = sp.CarShipers.Select(c => c.CarBrand).FirstOrDefault();
                data.CarModel = sp.CarShipers.Select(c => c.CarModel).FirstOrDefault();
                data.LicensePlate = sp.CarShipers.Select(c => c.LicensePlates).FirstOrDefault();
                data.LstAreaShiper = sp.ShiperAreas.Where(a => a.IsActive.Equals(SystemParam.ACTIVE))
                    .Select(a => new Area
                    {
                        ID = a.Area.ID,
                        Name = a.Area.District.Name
                    }).ToList();
                data.lstImgIdentify = sp.ShiperImages.Select(i => i.Path).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return null;
            }
        }


        public IPagedList<OrderOutputModel> GetListBusiness(int Page, int ID)
        {
            try
            {
                var lstOrder = (from c in cnn.OrderServices
                                where c.ShiperID == ID
                                orderby c.CreatedDate descending
                                select new OrderOutputModel()
                                {
                                    Code = c.Code,
                                    TotalPrice = c.OrderServiceDetails.GroupBy(od => od.ID).Select(od => od.Select(o => o.Price).Sum()).FirstOrDefault(),
                                    Status = c.Status,
                                    CompletedDate = c.CompletedDate,
                                    CreateDate = c.CreatedDate,
                                    NameService = c.CarTypeID == null ? "Đặt đồ ăn" : "Đặt xe",
                                }
                                ).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return lstOrder;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return null;
            }
        }

        public List<Area> GetArea()
        {
            return cnn.Areas.Where(a => a.IsActive.Equals(SystemParam.ACTIVE)).ToList();
        }
        public List<District> GetDistricByProvince(int provinceID)
        {
            return cnn.Districts.Where(d => d.ProvinceID.Equals(provinceID)).ToList();
        }
        // Thêm shipper
        public JsonResultModel CreateShipper(ShiperInputModel input)
        {
            try
            {
                int checkPhone = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.User.Equals(input.Phone) && (c.ShiperID.HasValue || c.ShopID.HasValue)).Count();
                int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && (c.ShiperID.HasValue || c.ShopID.HasValue) && (c.Customer.Email.Equals(input.Email) || c.Shop.Email.Equals(input.Email) || c.Shiper.Email.Equals(input.Email))).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);

                string[] lstShipperArea = input.ShipperProvinces.Substring(0, input.ShipperProvinces.Length - 1).Split(',');
                List<int> lstI = new List<int>();
                foreach (var i in lstShipperArea)
                {
                    int item = Int32.Parse(i);
                    lstI.Add(item);
                }
                DateTime? dob = !String.IsNullOrEmpty(input.DOB) ? Util.ConvertDate(input.DOB) : null;
                Shiper shiper = new Shiper();
                shiper.Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10);
                shiper.AvartarUrl = input.Avatar;
                shiper.Name = input.Name;
                shiper.DOB = dob;
                shiper.Identification = input.Identify;
                shiper.Phone = input.Phone;
                shiper.Email = input.Email;
                shiper.Address = input.Address;
                shiper.CommissionID = input.ComissionID;
                shiper.CreatedDate = DateTime.Now;
                shiper.Address = input.Address;
                shiper.RatingAdmin = 0;
                shiper.IsVip = input.IsVip;
                shiper.IsActive = SystemParam.ACTIVE;
                shiper.Lati = 20.9858584;
                shiper.Longi = 105.810133;
                shiper.IsAcceptService = true;
                shiper.ModifyDate = DateTime.Now;
                shiper.Rating = 0;
                shiper.Sex = input.Sex;
                shiper.IsInternal = input.IsInternal;
                //add car for shipper
                List<CarShiper> lst = new List<CarShiper>();
                CarShiper cs = new CarShiper();
                cs.CarBrand = input.CarBrand;
                cs.CarModel = input.CarModel;
                cs.LicensePlates = input.LicensePlate;
                cs.IsActive = SystemParam.ACTIVE;
                cs.CreatedDate = DateTime.Now;
                cs.VehicleTypeID = input.VehicleTypeID;
                cs.CarColor = "";
                lst.Add(cs);
                shiper.CarShipers = lst;
                List<Member> mb = new List<Member>();
                Member m = new Member();
                m.User = input.Phone;
                m.DeviceID = "";
                m.Password = Util.GenPass("123456");
                m.Token = "";
                m.IsActive = SystemParam.ACTIVE;
                m.CreatedDate = DateTime.Now;
                m.IsLogin = true;
                m.ExpriceDateOTP = DateTime.Now;
                m.Status = SystemParam.ACTIVE;
                m.QtyOTP = 0;
                m.OTPDateTime = DateTime.Now;
                //Create wallet for shiper
                List<Wallet> w = new List<Wallet>();
                w.Add(new Wallet() { Balance = 0, Type = Constant.WALLET_NO_WITHDRAW, CreatedDate = DateTime.Now, IsActive = SystemParam.ACTIVE });
                w.Add(new Wallet() { Balance = 0, Type = Constant.WALLET_WITHDRAW, CreatedDate = DateTime.Now, IsActive = SystemParam.ACTIVE });
                m.Wallets = w; //add wallet to member
                //Add area for shipper
                List<ShiperArea> lsa = new List<ShiperArea>();
                shiper.ShiperAreas = lstI.Select(u => new ShiperArea
                {
                    AreaID = u,
                    IsActive = 1,
                    CreatedDate = DateTime.Now
                }).ToList();
                m.KeyChat = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                m.RoomID = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                mb.Add(m);
                shiper.Members = mb;
                List<string> lstImgUrl = input.ImgIdentify.Substring(0, input.ImgIdentify.Length - 1).Split(',').ToList();
                List<ShiperImage> lisImgShip = lstImgUrl.Select(i => new ShiperImage
                {
                    IsActive = SystemParam.ACTIVE,
                    Path = i,
                    CreatedDate = DateTime.Now
                }).ToList();

                shiper.ShiperImages = lisImgShip;
                cnn.Shipers.Add(shiper);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public int CheckExistShipper(string Phone)
        {
            try
            {
                var shipper = cnn.Shipers.Where(s => s.IsActive == 1 && s.Phone == Phone).FirstOrDefault();
                if (shipper != null)
                {
                    return SystemParam.ACTIVE;
                }
                else
                {
                    return SystemParam.FAIL;
                }
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return SystemParam.FAIL;
            }
        }

        // Xóa Shipper
        public int DeleteShip(int ID)
        {
            try
            {
                Shiper s = cnn.Shipers.Find(ID);
                s.IsActive = SystemParam.INACTIVE;
                Member m = (from mem in cnn.Members where mem.ShiperID == ID select mem).FirstOrDefault();
                m.IsActive = SystemParam.INACTIVE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        //yêu cầu rút tiền shiper
        public IPagedList<RequestWithdraw> SearchWithDraw(int Page, string Name, int? Status, string FromDate, string ToDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(FromDate);
                DateTime? td = Util.ConvertDate(ToDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.RequestWithdraws.Where(r => r.IsActive.Equals(SystemParam.ACTIVE)
                && (!String.IsNullOrEmpty(Name) ? r.Shiper.Name.Contains(Name) || r.Shiper.Phone.Contains(Name) || r.Shiper.Code.Contains(Name) : true)
                && (fd.HasValue ? r.CreaedDate >= fd.Value : true) && (td.HasValue ? r.CreaedDate <= td.Value : true)
                && (Status.HasValue ? r.Status.Equals(Status.Value) : true)).OrderBy(r => r.Status >= SystemParam.STATUS_REQUEST_WAITING ? r.Status : 3).ThenByDescending(r => r.CreaedDate)
                .ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<RequestWithdraw>().ToPagedList(1, 1);
            }
        }
        /// <summary>
        /// Chấp thuận yêu cầu rút tiền: type = 1 , Hoàn thành yêu cầu rút tiền : type = 2
        /// </summary>
        /// <param name="lstRequestID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int AcceptWithDraw(string lstRequestID, int type)
        {
            try
            {
                List<int> lstRqID = lstRequestID.Split(',').Select(Int32.Parse).ToList();
                //Lấy danh sách yêu cầu
                var ListRequest = (from r in cnn.RequestWithdraws
                                   join sh in cnn.Shipers on r.ShiperID equals sh.ID
                                   join m in cnn.Members on sh.ID equals m.ShiperID
                                   where lstRqID.Contains(r.ID) && r.Status.Equals(type - 1) && m.IsActive.Equals(SystemParam.ACTIVE)
                                   select new
                                   {
                                       ID = r.ID,
                                       MemberID = m.ID,
                                       DeviceID = m.DeviceID
                                   }).ToList();
                if (ListRequest.Count() == 0)
                    return -1;
                //Lưu lại thông báo
                NotifyDataModel notifyData = new NotifyDataModel();
                notifyData.type = SystemParam.NOTI_TYPE_REJECT_WITHDRAW;
                notifyData.code = "";
                notifyData.content = type == SystemParam.STATUS_REQUEST_SUCCESS ? "Yêu cầu rút tiền của bạn đã được chấp nhận" : "Yêu cầu rút tiền của bạn đã được hoàn thành";
                List<Notification> lstNoti = ListRequest.Select(u => new Notification
                {
                    Content = notifyData.content,
                    Title = notifyData.content,
                    Type = notifyData.type,
                    IsRead = false,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    Code = "",
                    MemberID = u.MemberID
                }).ToList();
                List<string> lstDeviceID = ListRequest.Where(mb => !String.IsNullOrEmpty(mb.DeviceID) && mb.DeviceID.Length > 10).Select(u => u.DeviceID).ToList();
                foreach (var r in ListRequest)
                {
                    var request = cnn.RequestWithdraws.FirstOrDefault(x => x.ID.Equals(r.ID));
                    //Thay đổi trạng thái của request
                    request.Status = type;
                }
                cnn.Notifications.AddRange(lstNoti);
                cnn.SaveChanges();
                if (lstDeviceID.Count() > 0)
                {
                    string value = pushNotifyBusiness.PushNotify(notifyData, lstDeviceID, SystemParam.WE_SHIP_NOTI, notifyData.content, 2);
                    pushNotifyBusiness.PushOneSignal(value, 2);
                }
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int DenyWithDraw(string lstRequestID, string reason)
        {
            try
            {
                List<int> lstRqID = lstRequestID.Split(',').Select(Int32.Parse).ToList();
                //Lấy danh sách yêu cầu
                var ListRequest = (from r in cnn.RequestWithdraws
                                   join sh in cnn.Shipers on r.ShiperID equals sh.ID
                                   join m in cnn.Members on sh.ID equals m.ShiperID
                                   join w in cnn.Wallets on m.ID equals w.MemberID
                                   where lstRqID.Contains(r.ID) && r.Status.Equals(SystemParam.STATUS_REQUEST_WAITING) && m.IsActive.Equals(SystemParam.ACTIVE) 
                                   && w.Type.Equals(Constant.WALLET_WITHDRAW)
                                   select new
                                   {
                                       ID = r.ID,
                                       MemberID = m.ID,
                                       DeviceID = m.DeviceID,
                                       Amount = r.Amount,
                                       Balance = w.Balance,
                                       WalletID = w.ID
                                   }).ToList();
                if (ListRequest.Count() == 0)
                    return -1;
                NotifyDataModel notifyData = new NotifyDataModel();
                notifyData.type = SystemParam.NOTI_TYPE_REJECT_WITHDRAW;
                notifyData.code = "";
                notifyData.content = "Yêu cầu rút tiền của bạn đã bị từ chối do " + reason;
                //Lưu lại thông báo
                List<Notification> lstNoti = ListRequest.Select(u => new Notification
                {
                    Content = notifyData.content,
                    Title = notifyData.content,
                    Type = SystemParam.NOTI_TYPE_REJECT_WITHDRAW,
                    IsRead = false,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    Code = "",
                    MemberID = u.MemberID
                }).ToList();
                List<string> lstDeviceID = ListRequest.Where(mb => !String.IsNullOrEmpty(mb.DeviceID) && mb.DeviceID.Length > 10).Select(u => u.DeviceID).ToList();
                //Lưu lại lịch sử giao dịch
                List<MembersTransactionHistory> histories = ListRequest.Select(u => new MembersTransactionHistory
                {
                    MemberID = u.MemberID,
                    IsActive = SystemParam.ACTIVE,
                    Title = "Từ chối yêu cầu rút tiền",
                    Content = notifyData.content,
                    Type = Constant.TRANSACTION_ADD_POINT,
                    TransactionType = Constant.TYPE_TRANSACTION_REFUND_WITHDRAW,
                    TransactionID = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10),
                    BeforeBalance = u.Balance,
                    AfterBalance = u.Balance + u.Amount,
                    Status = Constant.STATUS_PAYMENT_COMPLETE,
                    IsExtra = false,
                    CreateDate = DateTime.Now,
                    Point = u.Amount,
                    WalletID = u.WalletID
                }).ToList();

                foreach (var i in ListRequest)
                {
                    //Thay đổi trạng thái của request
                    var request = cnn.RequestWithdraws.FirstOrDefault(x => x.ID.Equals(i.ID));
                    request.Status = SystemParam.STATUS_REQUEST_CANCEl;
                    //Hoàn lại tiền cho tài xế:
                    Wallet w = cnn.Wallets.FirstOrDefault(x => x.ID.Equals(i.WalletID));
                    w.Balance += i.Amount;
                }
                cnn.Notifications.AddRange(lstNoti);
                cnn.MembersTransactionHistories.AddRange(histories);
                cnn.SaveChanges();
                if (lstDeviceID.Count() > 0)
                {
                    string value = pushNotifyBusiness.PushNotify(notifyData, lstDeviceID, SystemParam.WE_SHIP_NOTI, notifyData.content, 2);
                    pushNotifyBusiness.PushOneSignal(value, 2);
                }
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        /// <summary>
        /// Ngưng hoạt động shipper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResultModel DeActiveShiper(int id)
        {
            try
            {
                Member mb = cnn.Members.Where(m => m.IsActive > SystemParam.ACTIVE_FALSE && m.ShiperID == id).FirstOrDefault();
                mb.IsActive = mb.IsActive.Equals(SystemParam.ACTIVE) ? SystemParam.DEACTIVE : SystemParam.ACTIVE;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        /// <summary>
        /// Tìm kiếm shipper theo keyword(Thuộc bộ chức năng cộng tiền cho shipper)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public JsonResultModel GetListShipperByKeyword(string keyword)
        {
            try
            {
                var data = cnn.Members.Where(m => m.IsActive > SystemParam.ACTIVE_FALSE
               && (!String.IsNullOrEmpty(keyword) ? m.Shiper.Name.Contains(keyword) || m.Shiper.Phone.Contains(keyword) : true) && m.ShiperID.HasValue)
                    .Select(m => new
                    {
                        id = m.ShiperID,
                        name = m.Shiper.Name
                    }).ToList();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
    }
}
