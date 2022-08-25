using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class RequestBusiness : GenericBusiness
    {
        public RequestBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        NotifyBusiness notiBus = new NotifyBusiness();
        TransactionHistoryBusiness transBus = new TransactionHistoryBusiness();
        public SystemResult SearchTransaction(int? status = null, string searchKey = "", string fromDate = "", string toDate = "")
        {
            List<RequestTransactionModel> list = new List<RequestTransactionModel>();
            DateTime? fDate = Util.ConvertDate(fromDate);
            DateTime? tDate = Util.ConvertDate(toDate);
            if (tDate.HasValue)
                tDate = tDate.Value.AddDays(1);
            list = cnn.MembersTransactionHistories
                .Where(u =>
                u.IsActive.Equals(SystemParam.ACTIVE)
                && u.Member.IsActive.Equals(SystemParam.ACTIVE)
                && u.Member.AgentID.HasValue
                && u.Type.Equals(Constant.TYPE_TRANSACTION_WITHDRAW)
                //&& u.Type.Equals(Constant.TYPE_TRANSACTION_VNPAY)
                && (String.IsNullOrEmpty(searchKey) ? true : (u.Member.Agent.Name.Contains(searchKey) || u.Member.Agent.Phone.Contains(searchKey)))
                && (fDate.HasValue ? u.CreateDate >= fDate.Value : true)
                && (tDate.HasValue ? u.CreateDate <= tDate.Value : true)
                && (status.HasValue ? u.Status.Equals(status.Value) : true)
                )
                .Select(u => new RequestTransactionModel
                {
                    ID = u.ID,
                    Amount = u.Point,
                    BrankName = u.BankMemberID.HasValue ? u.BankMember.Bank.Name : "",
                    Acount = u.BankMemberID.HasValue ? u.BankMember.Acount : "",
                    Owner = u.BankMemberID.HasValue ? u.BankMember.AcountOwner : "",
                    CreateDate = u.CreateDate,
                    Status = u.Status,
                    WasherName = u.Member.Agent.Name,
                    WasherPhone = u.Member.User
                }).OrderByDescending(u => u.ID).ToList();
            return resultBus.SucessResult(list, "");
        }

        public SystemResult ChangeStatusTransaction(List<int> listID, int status, string content = "")
        {

            var list = cnn.MembersTransactionHistories.Where(u => listID.Contains(u.ID)).ToList();
            var listDone = list.Where(u => u.Status.Equals(Constant.STATUS_TRANSACTION_SUCCESS)).ToList();
            var listRefund = list.Where(u => u.Status.Equals(Constant.STATUS_TRANSACTION_FLASE)).ToList();
            var listWaiting = list.Where(u => u.Status.Equals(Constant.STATUS_TRANSACTION_WAITING)).ToList();
            var listApprove = list.Where(u => u.Status.Equals(Constant.STATUS_TRANSACTION_APPROVE)).ToList();
            switch (status)
            {
                case Constant.STATUS_TRANSACTION_SUCCESS:
                    {
                        if (listDone.Count > 0 || listRefund.Count > 0 || listWaiting.Count > 0)
                            return resultBus.ErrorResult("Error selecting wrong data");
                        notiBus.CreateNotiAllCustomer("Yêu cầu rút tiền của bạn đã được hoàn thành", "Yêu cầu rút tiền", Constant.AGENT_ROLE, SystemParam.ACTIVE, list.Select(u => u.MemberID).ToList(), Util.CreateMD5(DateTime.Now.ToString()), 10, null, "Yêu cầu rút tiền của bạn đã được hoàn thành");
                        break;
                    }
                case Constant.STATUS_TRANSACTION_FLASE:
                    {
                        if (listDone.Count > 0 || listRefund.Count > 0)
                            return resultBus.ErrorResult("Error selecting wrong data");
                        break;
                    }
                case Constant.STATUS_TRANSACTION_APPROVE:
                    {
                        if (listDone.Count > 0 || listRefund.Count > 0)
                            return resultBus.ErrorResult("Error selecting wrong data");
                        break;
                    }
            }
            foreach (var trans in list)
            {
                trans.Status = status;
                trans.ConfirmDate = DateTime.Now;
            }
            int i = 0;
            if (status.Equals(Constant.STATUS_TRANSACTION_FLASE))
                foreach (var transaction in list)
                {
                    i++;
                    string code = Util.CreateMD5(DateTime.Now.ToString() + i.ToString()).Substring(0, 8);
                    transBus.CreateTransaction(transaction.MemberID, transaction.Point, Constant.TYPE_TRANSACTION_REFUND_TRANSACTION, code, null, null, Constant.WALLET_WITHDRAW, transaction.Member.Lang, Constant.STATUS_TRANSACTION_SUCCESS, "", null, content);
                }
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult SearchRequestAddCar(int? status = null, string searchKey = "", string fromDate = "", string toDate = "")
        {

            List<RequestAddCarModel> list = new List<RequestAddCarModel>();
            DateTime? fDate = Util.ConvertDate(fromDate);
            DateTime? tDate = Util.ConvertDate(toDate);
            if (tDate.HasValue)
                tDate = tDate.Value.AddDays(1);
            list = cnn.RequestForAdmins
               .Where(u =>
               u.IsActive.Equals(SystemParam.ACTIVE) && u.Member.CustomerID.HasValue
               && (u.Member.Customer.Name.Contains(searchKey) || u.Member.Customer.Phone.Contains(searchKey))
               && (fDate.HasValue ? u.CreateDate >= fDate.Value : true)
               && (tDate.HasValue ? u.CreateDate <= tDate.Value : true)
               && (status.HasValue ? u.status.Equals(status.Value) : true)
               )
               .Select(u => new RequestAddCarModel
               {
                   ID = u.ID,
                   CreateDate = u.CreateDate,
                   Status = u.status,
                   CustomerName = u.Member.Customer.Name,
                   CustomerPhone = u.Member.Customer.Phone,
                   Content = u.Note,
                   CustomerID = u.Member.CustomerID.Value
               }).OrderByDescending(u => u.ID).ToList();
            return resultBus.SucessResult(list);
        }
        public SystemResult ChangeStatusRequestAddCar(int ID, int status, string note = "")
        {
            var request = cnn.RequestForAdmins.Find(ID);
            if (request == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            Member mb = cnn.Members.Find(request.MemberID);
            if (mb == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            request.status = status;
            if (status.Equals(Constant.STATUS_TRANSACTION_APPROVE))
                request.ConfirmDate = DateTime.Now;
            else if (status.Equals(Constant.STATUS_TRANSACTION_FLASE))
            {
                note = (mb.Lang.Equals(SystemParam.VN) ? "Yêu cầu của bạn bị huỷ với lý do: " : "Your request was canceled with the reason : ") + note;
                notiBus.CreateNoti(request.MemberID.Value, Constant.NOTI_FROM_ADMIN, null, null, null, mb.Lang, note, true);
            }
            else if (status.Equals(Constant.STATUS_REQUEST_SUCCESS))
            {

                note = (mb.Lang.Equals(SystemParam.VN) ? "Yêu cầu của bạn đã được hoàn thành" : "Your request was complete");
                notiBus.CreateNoti(request.MemberID.Value, Constant.NOTI_FROM_ADMIN, null, null, null, mb.Lang, note, true);
            }
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }

        public ExcelPackage ExportListRequest(List<ListRequestChecked> Data)
        {
            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/listRequest.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 4;
                int no = 1;
                if (Data.Count() > 0 && Data != null)
                {
                    foreach (var dt in Data)
                    {
                        sheet.Cells[row, 1].Value = no;
                        sheet.Cells[row, 2].Value = dt.Name;
                        sheet.Cells[row, 3].Value = dt.Phone;
                        sheet.Cells[row, 4].Value = dt.Amout;
                        sheet.Cells[row, 5].Value = dt.Bank;
                        sheet.Cells[row, 6].Value = dt.Acount;
                        sheet.Cells[row, 7].Value = dt.Owner;
                        sheet.Cells[row, 8].Value = dt.Date;
                        no++;
                        row++;
                    }
                }

                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ExcelPackage();
            }

        }
    }
}
