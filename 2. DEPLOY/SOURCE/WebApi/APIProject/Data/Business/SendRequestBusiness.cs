using Data.DB;
using Data.Model.APIApp;
using Data.Utils;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class SendRequestBusiness : GenericBusiness
    {
        public SendRequestBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }
        OneSignalBusiness oneSignalBus = new OneSignalBusiness();
        TransactionHistoryBusiness transactionBus = new TransactionHistoryBusiness();
        NotifyBusiness notiBus = new NotifyBusiness();
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        AgentBusiness agentBus = new AgentBusiness();
        public List<WasherRequestOutputModel> SearchWasherRequest(int areaID, double log, double lat, DateTime BookingDate, DateTime? EstBookingDate, int PaymentType, int point, bool IsConfirm = false, string code = "", int? agentID = null)
        {
            var ListAgentID = cnn.AgentAreas.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.AreaID.Equals(areaID)).Select(u => u.AgentID).ToList();
            int[] washing = { Constant.ORDER_STATUS_CONFIRM, Constant.ORDER_STATUS_CONFIRM_WASHING, Constant.ORDER_STATUS_WASHING };
            var listConfig = cnn.Configs;
            int GPS = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.GPSNotValid)).FirstOrDefault().ValueConstant);
            int MinBalanceSendRequest = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.MIN_BALANCE_SEND_MESSAGE)).FirstOrDefault().ValueConstant);
            int MinBalanceSendRequestFirst = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.MIN_BALANCE_SEND_REQUEST_FIRST)).FirstOrDefault().ValueConstant);
            //int MaxDistanceSendRequest = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.MAX_DISTANCE_SEND_REQUEST)).FirstOrDefault().ValueConstant);
            List<Member> listAgent = cnn.Members.Where(u => u.AgentID.HasValue
            && ((IsConfirm ? true : (u.Agent.AcceptService.HasValue
            && u.Agent.AcceptService.Value.Equals(SystemParam.ACTIVE)))
            && u.IsActive.Equals(SystemParam.ACTIVE)
            && u.IsLogin.Value.Equals(SystemParam.ACTIVE)
            && ListAgentID.Contains(u.AgentID.Value)
            && ((PaymentType == Constant.PAYMENT_TYPE_CASH) ? u.Wallets.Where(c => c.TYPE.Equals(Constant.PAYMENT_TYPE_CASH) && c.Balance >= point).Count() > 0 : true)
            && u.Agent.ModifyDate.HasValue)
            || (agentID.HasValue ? u.AgentID.Value.Equals(agentID.Value) : false)).ToList();
            //System.Diagnostics.Debug.WriteLine("hihi");
            var query = listAgent.Select(u => new WasherRequestOutputModel
            {
                MemberID = u.ID,
                Code = u.Agent.Code,
                WasherID = u.AgentID.Value,
                DeviceID = u.DeviceID,
                Lang = u.Lang,
                Name = u.Agent.Name,
                Phone = u.Agent.Phone,
                InHouse = u.Agent.IsInHouse.HasValue ? u.Agent.IsInHouse.Value : 1,
                Distance = Util.Distance(lat, log, u.Agent.lati.HasValue ? u.Agent.lati.Value : 0, u.Agent.longi.HasValue ? u.Agent.longi.Value : 0),
                ModifyDate = u.Agent.ModifyDate,
                Balance = u.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).FirstOrDefault().Balance,
                CountOrderService = u.Agent.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && !o.Status.Equals(Constant.ORDER_STATUS_CANCEL)).Count(),
                CountHaveOrder = u.Agent.OrderServices.Where(o => ((o.EstBookingDate.Value >= BookingDate && o.BookingDate.Value <= BookingDate) || (EstBookingDate.HasValue ? (o.EstBookingDate.Value >= EstBookingDate.Value && o.BookingDate.Value <= EstBookingDate.Value) : false)) && o.IsActive.Equals(SystemParam.ACTIVE) && washing.ToList().Contains(o.Status)).ToList().Count
            }).ToList();
            var num = cnn.OrderServices.Count();
            query = query.Where(u => u.CountHaveOrder == 0 || (agentID.HasValue ? u.WasherID.Equals(agentID.Value) : false)).OrderBy(u => u.Distance).ThenByDescending(u => u.ModifyDate.Value).ToList();
            query = query.Where(u => (u.CountOrderService > 0 ? (u.Balance >= MinBalanceSendRequest) : (u.Balance >= MinBalanceSendRequestFirst)) || (agentID.HasValue ? u.WasherID.Equals(agentID.Value) : false)).ToList();
            query = CheckGPS(query, GPS);
            if (code.Length > 0)
            {
                string body = JsonConvert.SerializeObject(query);
                var reportDirectory = string.Format("~/logWasher/{0}/", DateTime.Now.ToString("yyyy-MM-dd"));
                reportDirectory = System.Web.Hosting.HostingEnvironment.MapPath(reportDirectory);
                if (!Directory.Exists(reportDirectory))
                {
                    Directory.CreateDirectory(reportDirectory);
                }
                var dailyReportFullPath = string.Format("{0}text_{1}.log", reportDirectory, DateTime.Now.Day);
                var logContent = string.Format("{0}-{1}-{2}", DateTime.Now, "noti: " + code + " / " + body, Environment.NewLine);
                File.AppendAllText(dailyReportFullPath, logContent);
            }
            return query;
        }
        public List<WasherRequestOutputModel> CheckGPS(List<WasherRequestOutputModel> listWasher, int GPS)
        {
            List<WasherRequestOutputModel> query = new List<WasherRequestOutputModel>();
            query = listWasher.Where(u => u.ModifyDate.Value.AddMinutes(GPS) >= DateTime.Now).OrderByDescending(u => u.InHouse).ThenBy(u => u.Distance).ToList();
            var listAgentNOTGPS = listWasher.Where(u => u.ModifyDate.Value.AddMinutes(GPS) < DateTime.Now).OrderByDescending(u => u.InHouse).ThenBy(u => u.Distance).ToList();
            query = query.Concat(listAgentNOTGPS).ToList();
            return query;
        }
        public List<WasherRequestOutputModel> SearchWasherRequestCombo(int areaID, double log, double lat, DateTime BookingDate, DateTime EstBookingDate, int PaymentType, int point, int count, int index, bool IsConfirm = false)
        {
            var ListAgentID = cnn.AgentAreas.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.AreaID.Equals(areaID)).Select(u => u.AgentID).ToList();
            var listConfig = cnn.Configs;
            int[] washing = { Constant.ORDER_STATUS_CONFIRM, Constant.ORDER_STATUS_CONFIRM_WASHING, Constant.ORDER_STATUS_WASHING };
            int GPS = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.GPSNotValid)).FirstOrDefault().ValueConstant);
            int MinBalanceSendRequest = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.MIN_BALANCE_SEND_MESSAGE)).FirstOrDefault().ValueConstant);
            int MinBalanceSendRequestFirst = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.MIN_BALANCE_SEND_REQUEST_FIRST)).FirstOrDefault().ValueConstant);
            //int MaxDistanceSendRequest = int.Parse(listConfig.Where(u => u.NameConstant.Equals(Constant.MAX_DISTANCE_SEND_REQUEST)).FirstOrDefault().ValueConstant);
            List<Member> listAgent = cnn.Members.Where(u => u.AgentID.HasValue
            && (IsConfirm ? true : (u.Agent.AcceptService.HasValue
            && u.Agent.AcceptService.Value.Equals(SystemParam.ACTIVE)))
            && u.IsActive.Equals(SystemParam.ACTIVE)
            && ListAgentID.Contains(u.AgentID.Value)
            && u.IsLogin.Value.Equals(SystemParam.ACTIVE)
            //&& ((PaymentType == Constant.PAYMENT_TYPE_NO_VNP) ? u.Wallets.Where(c => c.TYPE.Equals(Constant.PAYMENT_TYPE_NO_VNP) && c.Balance >= point).Count() > 0 : true)
            && u.Agent.ModifyDate.HasValue).ToList();
            for (int i = 0; i < count; i++)
            {
                int abc = (i - index) * 7;
                listAgent = listAgent.Where(u => u.Agent.OrderServices
                .Where(o => (
                ((o.EstBookingDate.Value >= BookingDate.AddDays(abc) && o.BookingDate.Value <= BookingDate.AddDays(abc))
                || (o.EstBookingDate.Value >= EstBookingDate.AddDays(abc) && o.BookingDate.Value <= EstBookingDate.AddDays(abc)))
                && o.IsActive.Equals(SystemParam.ACTIVE)
                && washing.ToList().Contains(o.Status))).ToList().Count == 0).ToList();
            }
            List<WasherRequestOutputModel> query = new List<WasherRequestOutputModel>();
            query = listAgent.Select(u => new WasherRequestOutputModel
            {
                MemberID = u.ID,
                Code = u.Agent.Code,
                WasherID = u.AgentID.Value,
                Lang = u.Lang,
                Distance = Util.Distance(lat, log, u.Agent.lati.HasValue ? u.Agent.lati.Value : 0, u.Agent.longi.HasValue ? u.Agent.longi.Value : 0),
                ModifyDate = u.Agent.ModifyDate,
                Balance = u.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).FirstOrDefault().Balance,
                CountOrderService = u.Agent.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && !o.Status.Equals(Constant.ORDER_STATUS_CANCEL)).Count(),
                DeviceID = u.DeviceID
            }).ToList();
            query = query.Where(u => (u.CountOrderService > 0 ? (u.Balance >= MinBalanceSendRequest) : (u.Balance >= MinBalanceSendRequestFirst))).OrderBy(u => u.Distance).ThenByDescending(u => u.ModifyDate.Value).ToList();
            query = CheckGPS(query, GPS);
            return query;
        }

        public WasherRequestOutputModel FindFirstWasherCombo(int areaID, double log, double lat, DateTime BookingDate, DateTime EstBookingDate, int paymentType, int point, int count, int index)
        {
            var output = SearchWasherRequestCombo(areaID, log, lat, BookingDate, EstBookingDate, paymentType, point, count, index, false).FirstOrDefault();
            if (output != null)
            {
                return output;
            }
            else
                return null;
        }
        public WasherRequestOutputModel FindFirstWasher(int areaID, double log, double lat, DateTime BookingDate, DateTime EstBookingDate, int paymentType, int point)
        {
            var output = SearchWasherRequest(areaID, log, lat, BookingDate, EstBookingDate, paymentType, point).FirstOrDefault();
            if (output != null)
            {
                return output;
            }
            else
                return null;
        }
        public List<WasherRequestOutputModel> SendAllWasher(int orderServiceID, bool isPush, string code = "")
        {
            OrderService order = cnn.OrderServices.Find(orderServiceID);
            if (order == null)
                return null;
            var output = SearchWasherRequest(order.AreaID.Value, order.Longitude, order.Latitude, order.BookingDate.Value, order.EstBookingDate.Value, order.PaymentType.Value, order.BasePrice, false, code);
            if (isPush && order.Status.Equals(Constant.ORDER_STATUS_WAITING))
            {
                oneSignalBus.SaveLog("đã vào đây 2", "");
                List<string> lsDeviceID = new List<string>();
                if (order.FirstAgentID.HasValue)
                    output = output.Where(u => u.WasherID != order.FirstAgentID.Value).ToList();
                foreach (WasherRequestOutputModel input in output)
                {
                    if (!String.IsNullOrEmpty(input.DeviceID) && input.DeviceID.Length > 0)
                        lsDeviceID.Add(input.DeviceID);
                }
                string content = MessVN.NOTI_ORDER_STATUS_WAITING;
                if (lsDeviceID.Count > 0)
                    oneSignalBus.SendNotifyForWasher(orderServiceID, content, Constant.NOTI_ORDER_STATUS_CONFIRM, lsDeviceID, null, null, "", MessVN.NOTI_HEADER, 1);
            }
            return output;
        }
        public List<WasherRequestOutputModel> SendAllWasherCombo(string code, bool isPush)
        {
            try
            {
                OrderService order = cnn.OrderServices.Where(u => u.CodeCombo.Equals(code)).OrderBy(u => u.ID).FirstOrDefault();
                if (order == null)
                    return null;
                int count = cnn.ServiceComboDetails.Where(u => u.ServiceComboID.Equals(order.ServiceComboSegment.ServiceComboID) && u.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Count();
                var output = SearchWasherRequestCombo(order.AreaID.Value, order.Longitude, order.Latitude, order.BookingDate.Value, order.EstBookingDate.Value, order.PaymentType.Value, 0, count, 0);
                if (isPush && order.Status.Equals(Constant.ORDER_STATUS_WAITING))
                {
                    List<string> lsDeviceID = new List<string>();
                    if (order.FirstAgentID.HasValue)
                        output = output.Where(u => u.WasherID != order.FirstAgentID.Value).ToList();
                    foreach (WasherRequestOutputModel input in output)
                    {
                        if (!String.IsNullOrEmpty(input.DeviceID) && input.DeviceID.Length > 0)
                            lsDeviceID.Add(input.DeviceID);
                    }
                    string content = MessVN.NOTI_ORDER_STATUS_WAITING;

                    oneSignalBus.SendNotifyForWasher(null, content, Constant.NOTI_ORDER_STATUS_CONFIRM, lsDeviceID, null, null, "", MessVN.NOTI_HEADER, 1, code);
                }
                oneSignalBus.SaveLog("Push all combo", code);
                return output;
            }
            catch (Exception ex)
            {
                oneSignalBus.SaveLog("Push all combo", ex.ToString());
                return null;
            }
        }

        public void PushAllAgent()
        {
            int timeWait = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.TIME_DELAY_PUSH_NOTI)).FirstOrDefault().ValueConstant);
            var listOrderActive = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            var listOrder = listOrderActive.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(Constant.ORDER_STATUS_WAITING) && !u.ServiceComboSegmentID.HasValue).ToList();
            listOrder = listOrder.Where(u => u.CreatedDate.AddMinutes(timeWait) <= DateTime.Now).ToList();
            var LsMember = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.HasValue).ToList();
            // gói thường
            foreach (OrderService order in listOrder)
            {
                Member member = LsMember.Where(U => U.CustomerID.Value.Equals(order.CustomerID)).FirstOrDefault();
                if (order.BookingNow.Equals(1))
                {
                    oneSignalBus.SaveLog("đã vào đây booking now", "");
                    if (member != null)
                    {
                        string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 8);
                        if (!order.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH))
                            transactionBus.CreateTransaction(member.ID, order.BasePrice, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, member.Lang, Constant.STATUS_TRANSACTION_SUCCESS);
                        if (order.UsePoint.HasValue && order.UsePoint.Value > 0 && order.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH))
                            transactionBus.CreateTransaction(member.ID, order.UsePoint.Value, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, member.Lang, Constant.STATUS_TRANSACTION_SUCCESS);
                        notiBus.CreateNoti(member.ID, Constant.NOTI_ORDER_STATUS_CANCEL, order.ID, null, null, member.Lang, member.Lang.Equals(SystemParam.VN) ? "Chuyên gia không nhận đơn" : "Master do not accept applications", true);
                    }
                    order.Status = Constant.ORDER_STATUS_CANCEL;
                }
                else
                {
                    SendAllWasher(order.ID, true);
                    order.Status = Constant.ORDER_STATUS_FIND_ORTHER_WASHER;
                }
            }
            cnn.SaveChanges();
            // gói combo
            var listcombo = listOrderActive.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(Constant.ORDER_STATUS_WAITING) && u.ServiceComboSegmentID.HasValue).ToList();
            List<string> lscomboCode = listcombo.Where(u => u.CreatedDate.AddMinutes(timeWait) <= DateTime.Now).GroupBy(u => u.CodeCombo).Select(u => u.FirstOrDefault().CodeCombo).ToList();
            foreach (string code in lscomboCode)
            {
                SendAllWasherCombo(code, true);
            }
            foreach (string code in lscomboCode)
            {
                var lsCombo = listcombo.Where(u => u.CodeCombo.Equals(code)).ToList();
                foreach (var combo in lsCombo)
                {
                    combo.Status = Constant.ORDER_STATUS_FIND_ORTHER_WASHER;
                }

            }
            cnn.SaveChanges();
            listOrder = listOrderActive.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(Constant.ORDER_STATUS_FIND_ORTHER_WASHER)).ToList();
            listOrder = listOrder.Where(u => u.CreatedDate.AddMinutes(timeWait * 2) <= DateTime.Now).ToList();
            // còn lại
            foreach (OrderService order in listOrder)
            {
                Member member = LsMember.Where(U => U.CustomerID.Value.Equals(order.CustomerID)).FirstOrDefault();
                if (order.BookingNow.Equals(1))
                {
                    if (member != null)
                    {
                        string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 8);
                        if (!order.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH))
                            transactionBus.CreateTransaction(member.ID, order.BasePrice, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, member.Lang, Constant.STATUS_TRANSACTION_SUCCESS);
                        if (order.UsePoint.HasValue && order.UsePoint.Value > 0 && order.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH))
                            transactionBus.CreateTransaction(member.ID, order.UsePoint.Value, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, member.Lang, Constant.STATUS_TRANSACTION_SUCCESS);
                    }
                    order.Status = Constant.ORDER_STATUS_CANCEL;
                }
                else
                {
                    order.Status = Constant.ORDER_STATUS_NO_CONFIRM;
                }
            }
            cnn.SaveChanges();

        }
        public void FindJob() {
            try
            {
                List<int> listStatus = new List<int> { 0, 1, 9, 3 };
                var listJob = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH) && !listStatus.Contains(u.Status) && u.AgentID.HasValue &&
                u.MembersTransactionHistories.Where(t => t.MemberID.Equals(u.Agent.Members.FirstOrDefault().ID) && t.Type.Equals(Constant.TYPE_TRANSACTION_SUBTRACT_WASHER_SUBMIT_ORDER)).Count() == 0).ToList();
                string a = "";
                foreach (var job in listJob)
                {
                    string code = Util.CreateMD5(Util.RandomNumber(10000, 99999).ToString()).Substring(5, 8);
                    transactionBus.CreateTransaction(job.Agent.Members.FirstOrDefault().ID, job.BasePrice, Constant.TYPE_TRANSACTION_SUBTRACT_WASHER_SUBMIT_ORDER, code, job.ID, null, Constant.WALLET_NO_WITHDRAW, job.Agent.Members.FirstOrDefault().Lang, Constant.STATUS_PAYMENT_COMPLETE);
                }
            }
            catch {
                string a = "";
            }
        }
        public void UpdateCustomer() {
            List<Customer> lsCustomer = cnn.Customers.Where(u => !u.CustomerRankID.HasValue).ToList();
            foreach (var cus in lsCustomer) {
                cus.ProvinceCode = 1;
                cus.DistrictCode = 1;
                cus.CustomerRankID = 1;
                cus.RankingPoint = 0;
                cus.RankDate = DateTime.Now;
            }
            cnn.SaveChanges();
        }
        public void PushAfterWashing()
        {
            var config = cnn.Configs;
            // before30minute
            var message_vn = config.Where(x => x.NameConstant == SystemParam.MESSAGE_TEXT_VN).Select(x => x.ValueConstant).FirstOrDefault();
            var message_en = config.Where(x => x.NameConstant == SystemParam.MESSAGE_TEXT_EN).Select(x => x.ValueConstant).FirstOrDefault();
            DateTime time30min = DateTime.Now.AddMinutes(30);
            DateTime time10min = DateTime.Now.AddMinutes(10);
            List<OrderService> lsOrderService = cnn.OrderServices.Where(u =>
            u.BookingNow == 0
            && u.IsActive.Equals(SystemParam.ACTIVE)
            && u.Status.Equals(Constant.ORDER_STATUS_CONFIRM)
            && u.BookingDate.Value <= time30min
            ).ToList();
            lsOrderService = lsOrderService.Where(u => (u.IsPushFirst.HasValue ? u.IsPushFirst.Value.Equals((int)TypeSentOrder.FirstWasher) : u.ServiceComboSegmentID.HasValue)).ToList();
            if (lsOrderService.Count > 0)
            {
                foreach (OrderService order in lsOrderService)
                {
                    order.IsPushFirst = (int)TypeSentOrder.Before30Min;
                }
                cnn.SaveChanges();
                foreach (OrderService order in lsOrderService)
                {
                    notiBus.CreateNoti(order.Agent.Members.FirstOrDefault().ID, Constant.NOTI_READY_FOR_WASHING, order.ID, null, null, SystemParam.VN, "30 phút nữa là đến giờ cung cấp dịch vụ cho khách hàng, hãy chuẩn bị xuất phát thôi !", true);
                    //oneSignalBus.SendNotifyForWasher(order.ID, "30 phút nữa là đến giờ cung cấp dịch vụ cho khách hàng, hãy chuẩn bị xuất phát thôi !", Constant.NOTI_READY_FOR_WASHING, order.Agent.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => u.DeviceID).ToList(), null, null, "", MessVN.NOTI_HEADER);
                    string lang = order.Customer.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => u.Lang).FirstOrDefault();
                    if (String.IsNullOrEmpty(lang))
                        lang = SystemParam.VN;
                    string mess = lang.Equals(SystemParam.VN) ? message_vn == null ? "" : message_vn : message_en == null ? "" : message_en;
                    notiBus.CreateNoti(order.Customer.Members.FirstOrDefault().ID, Constant.NOTI_READY_FOR_WASHING, order.ID, null, null, order.Customer.Members.FirstOrDefault().Lang, mess, true);
                    string header = lang.Equals(SystemParam.VN) ? MessVN.NOTI_HEADER : MessEN.NOTI_HEADER;
                    //oneSignalBus.SendNotifyForWasher(order.ID, mess, Constant.NOTI_READY_FOR_WASHING, order.Customer.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => u.DeviceID).ToList(), null, null, "", header, null, "", 1);
                }
            }
            ThreadCancel();
            PushAllAgent();
            SendRequestForFirstWasher();
            // before10minute
            //List<OrderService> lsOrderServices = cnn.OrderServices.Where(u => u.BookingNow == 0 && u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(Constant.ORDER_STATUS_CONFIRM) && u.BookingDate.Value <= time10min && u.IsPushFirst.Value.Equals((int)TypeSentOrder.Before30Min)).ToList();
            //foreach (OrderService order in lsOrderServices)
            //{
            //    order.IsPushFirst = (int)TypeSentOrder.Before10Min;
            //    oneSignalBus.SendNotifyForWasher(order.ID, "Bạn đã có thể bắt đầu rửa đơn " + order.Code, Constant.NOTI_READY_FOR_WASHING, order.Agent.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => u.DeviceID).ToList(), null, null, "", MessVN.NOTI_HEADER);
            //}
        }
        public void SendRequestForFirstWasher()
        {
            var ListOrderService = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(1) && !u.IsPushFirst.HasValue && !u.ServiceComboSegmentID.HasValue && u.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH)).ToList();
            List<ThreadSendNotiFirst> lsOrderID = new List<ThreadSendNotiFirst>();
            List<Member> lsagent = cnn.Members.Where(u => u.AgentID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            if (ListOrderService.Count > 1)
            {
                foreach (OrderService order in ListOrderService)
                {
                    int index = ListOrderService.IndexOf(order);
                    if (index == 0)
                        lsOrderID.Add(new ThreadSendNotiFirst
                        {
                            OrderID = order.ID,
                            FirstWasher = order.FirstAgentID.Value
                        });
                    else
                    {
                        List<int> ListWasher = GetListAgentConfirm(order);
                        if (ListWasher.Count > 1)
                        {
                            if (lsOrderID.Where(u => u.FirstWasher.Equals(order.FirstAgentID.Value)).Count() > 0)
                            {
                                List<int> ListWasherCanConfirm = ListWasher.Where(u => !lsOrderID.Select(s => s.FirstWasher).ToList().Contains(u)).ToList();
                                if (ListWasherCanConfirm.Count == 0)
                                    ListWasherCanConfirm = ListWasher;
                                Random random = new Random();
                                int i = random.Next(0, ListWasherCanConfirm.Count);
                                order.FirstAgentID = ListWasherCanConfirm[i];
                            }
                        }

                        lsOrderID.Add(new ThreadSendNotiFirst
                        {
                            OrderID = order.ID,
                            FirstWasher = order.FirstAgentID.Value
                        });

                    }
                    order.IsPushFirst = 1;
                    Member agent = lsagent.Where(u => u.AgentID.Value.Equals(order.FirstAgentID.Value)).FirstOrDefault();
                    string content = MessVN.NOTI_ORDER_STATUS_WAITING;
                    string a = oneSignalBus.PushNotify(order.ID, content, Constant.NOTI_ORDER_STATUS_CONFIRM, agent.ID, null, null, "", MessVN.NOTI_HEADER, 1);
                }
                cnn.SaveChanges();
            }
            else if (ListOrderService.Count == 1)
            {
                OrderService order = ListOrderService.FirstOrDefault();
                order.IsPushFirst = 1;
                cnn.SaveChanges();
                Member agent = lsagent.Where(u => u.AgentID.Value.Equals(order.FirstAgentID.Value)).FirstOrDefault();
                string content = MessVN.NOTI_ORDER_STATUS_WAITING;
                string a = oneSignalBus.PushNotify(order.ID, content, Constant.NOTI_ORDER_STATUS_CONFIRM, agent.ID, null, null, "", MessVN.NOTI_HEADER, 1);
            }
        }
        public List<int> GetListAgentConfirm(OrderService order)
        {
            var listWaher = SearchWasherRequest(order.AreaID.Value, order.Longitude, order.Latitude, order.BookingDate.Value, order.EstBookingDate, order.PaymentType.Value, order.BasePrice);
            return listWaher.Select(u => u.WasherID).ToList();
        }
        public bool CreateRequestAddCar(string note, int memberID)
        {
            RequestForAdmin rq = new RequestForAdmin();
            rq.IsActive = SystemParam.ACTIVE;
            rq.CreateDate = DateTime.Now;
            rq.MemberID = memberID;
            rq.Note = note;
            rq.status = Constant.STATUS_REQUEST_PENDING;
            cnn.RequestForAdmins.Add(rq);
            cnn.SaveChanges();
            string url = Constant.HTTP + HttpContext.Current.Request.Url.Host;
            int type = 0;
            if (ServerUrl.MAIN_SERVER.ToList().Contains(url))
                type = ServerUrl.TYPE_MAIN_SERVER;
            if (ServerUrl.SERVER_TEST_WINDSOFT.ToList().Contains(url))
                type = ServerUrl.TYPE_SERVER_TEST_WINDSOFT;
            if (ServerUrl.SERVER_TEST_CARRECT.ToList().Contains(url))
                type = ServerUrl.TYPE_SERVER_TEST_CARRECT;
            apiBus.GetJson("http://118.27.192.110:3001/socketio?addCar=" + 1 + "&type=" + type + "&content=" + note);
            return true;
        }
        public void ThreadCancel()
        {
            int[] listWait = { Constant.ORDER_STATUS_WAITING, Constant.ORDER_STATUS_NO_CONFIRM, Constant.ORDER_STATUS_FIND_ORTHER_WASHER, Constant.ORDER_STATUS_CONFIRM };
            List<OrderService> lsOrder = cnn.OrderServices.Where(u => listWait.ToList().Contains(u.Status) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            if (lsOrder.Count > 0) {
                List<int> lsCustomerID = lsOrder.Select(s => s.CustomerID).ToList();
                List<Member> lsMember = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.HasValue && lsCustomerID.Contains(u.CustomerID.Value)).ToList();
                List<ThreadCancelModel> lsOrderCancel = new List<ThreadCancelModel>();
                foreach (OrderService order in lsOrder)
                {
                    Member member = lsMember.Where(u => u.CustomerID.Value.Equals(order.CustomerID)).FirstOrDefault();
                    List<WasherRequestOutputModel> listWaher = SearchWasherRequest(order.AreaID.Value, order.Longitude, order.Latitude, order.BookingDate.Value, order.EstBookingDate, order.PaymentType.Value, order.BasePrice);
                    if (order.ServiceComboSegmentID.HasValue)
                    {
                        List<OrderService> listOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CodeCombo.Equals(order.CodeCombo)).ToList();
                        listWaher = SearchWasherRequestCombo(order.AreaID.Value, order.Longitude, order.Latitude, order.BookingDate.Value, order.EstBookingDate.Value, order.PaymentType.Value, order.ComboPrice.Value, listOrder.Count, 0);
                    }
                    if (!String.IsNullOrEmpty(order.AgentCancel) && order.AgentCancel.Length > 0)
                        listWaher = listWaher.Where(u => !u.Code.Equals(order.AgentCancel)).ToList();
                    if (((listWaher.Count == 0 || (order.Status.Equals(Constant.ORDER_STATUS_NO_CONFIRM) && order.BookingDate.Value <= DateTime.Now)) && member != null && order.Status != Constant.ORDER_STATUS_CONFIRM) || (order.Status == Constant.ORDER_STATUS_CONFIRM && order.BookingDate.Value.AddHours(12) < DateTime.Now))
                    {
                        ThreadCancelModel query = new ThreadCancelModel
                        {
                            MemberID = member.ID,
                            OrderID = order.ID,
                            Lang = String.IsNullOrEmpty(member.Lang) ? SystemParam.VN : member.Lang,
                            DeviceID = member.DeviceID,
                        };
                        lsOrderCancel.Add(query);
                        order.Status = Constant.ORDER_STATUS_CANCEL;
                        order.ReasonCancel = !String.IsNullOrEmpty(member.Lang) && member.Lang.Equals(SystemParam.VN) ? MessVN.WASHER_NO_WORKING : MessEN.WASHER_NO_WORKING;
                        CancelTransaction(order, member.Lang, member.ID);
                    }
                }
                cnn.SaveChanges();
                if (lsOrderCancel.Count > 0)
                    notiBus.CreateMultiCancelOrder(lsOrderCancel);
            }
        }
        public void CancelTransaction(OrderService order, string lang, int MemberID)
        {
            if (String.IsNullOrEmpty(lang))
                lang = SystemParam.VN;
            string code = Util.CreateMD5(DateTime.Now.ToString() + order.ID.ToString()).Substring(0, 6);
            if (!order.ServiceComboSegmentID.HasValue)
            {
                if (!order.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH))
                    transactionBus.CreateTransaction(MemberID, order.CouponPoint.HasValue ? order.BasePrice - order.CouponPoint.Value : order.BasePrice, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, lang, Constant.STATUS_TRANSACTION_SUCCESS);
                if (order.UsePoint.HasValue && order.UsePoint.Value > 0 && order.PaymentType.Value.Equals(Constant.PAYMENT_TYPE_CASH))
                    transactionBus.CreateTransaction(MemberID, order.UsePoint.Value, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, lang, Constant.STATUS_TRANSACTION_SUCCESS);
            }
            else
            {
                List<OrderService> listOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CodeCombo.Equals(order.CodeCombo)).ToList();
                transactionBus.CreateTransaction(MemberID, order.UsePoint.Value + order.ComboPrice.Value, Constant.TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN, code, order.ID, null, Constant.WALLET_NO_WITHDRAW, lang, Constant.STATUS_TRANSACTION_SUCCESS);
                foreach (OrderService od in listOrder)
                {
                    od.Status = Constant.ORDER_STATUS_CANCEL;
                    order.ReasonCancel = String.IsNullOrEmpty(lang) ? MessVN.WASHER_NO_WORKING : MessEN.WASHER_NO_WORKING;
                }
                cnn.SaveChanges();
            }
        }
    }
}
