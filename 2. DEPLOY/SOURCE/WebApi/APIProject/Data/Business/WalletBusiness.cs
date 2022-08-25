using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class WalletBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        TransactionHistoryBusiness transactionBus = new TransactionHistoryBusiness();
        VNPayBusiness vnPayBus = new VNPayBusiness();
        public WalletBusiness(WE_SHIPEntities context = null) : base()
        {
        }
        /// <summary>
        /// Lấy danh sách tài khoản ngân hàng của shiper
        /// </summary>
        /// <param name="shiperID"></param>
        /// <returns></returns>
        public JsonResultModel GetListBankOfShiper(int shiperID)
        {
            try
            {
                ListBankOutputModel data = new ListBankOutputModel();
                Member mb = cnn.Members.Find(shiperID);
                if (!mb.ShiperID.HasValue)
                    return rpBus.ErrorResult(MessVN.ERROR_ROLE_INFO, SystemParam.PROCESS_ERROR);
                data.listBankInfo = mb.BankMembers.Where(b => b.IsActive.Equals(SystemParam.ACTIVE) && b.MemberID.Equals(mb.ID))
                    .Select(b => new ListBank
                    {
                        ID = b.ID,
                        Account = b.Account,
                        AccountOwner = b.AccountOwner,
                        BankName = b.Bank.Name,
                        BankID = b.BankID
                    }).OrderByDescending(b => b.ID).ToList();
                data.balance = mb.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.Type.Equals(Constant.WALLET_WITHDRAW) && w.MemberID.Equals(mb.ID)).FirstOrDefault().Balance;
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel CreateRequestWithDraw(RequestWithDrawInputModel input)
        {
            if (input.amount < SystemParam.MIN_TRANSACTION_MONEY)
                return rpBus.ErrorResult("Số tiền mỗi lần rút phải từ 50.000 trở lên", SystemParam.PROCESS_ERROR);
            if (input.amount > SystemParam.MAX_TRANSACTION_MONEY)
                return rpBus.ErrorResult("Số tiền mỗi lần rút phải từ 10.000.000 trở xuống", SystemParam.PROCESS_ERROR);
            NotifyBusiness notify = new NotifyBusiness();
            PushNotifyBusiness push = new PushNotifyBusiness();
            var connect = cnn.Database.BeginTransaction();
            try
            {
                Member mb = cnn.Members.Find(input.shiperID);
                if (!mb.ShiperID.HasValue)
                    return rpBus.ErrorResult(MessVN.PERMISSION_DENIED_MES, SystemParam.PROCESS_ERROR);
                //Check có đúng tài khoản của shiper không
                BankMember bm = mb.BankMembers.Where(b => b.IsActive.Equals(SystemParam.ACTIVE) && b.AccountOwner.Equals(input.owner)).FirstOrDefault();
                if(bm == null)
                    return rpBus.ErrorResult("Số tài khoản không hợp lệ vui lòng kiểm tra lại!", SystemParam.PROCESS_ERROR);
                //Check khoảng cách thời gian gửi yêu cầu
                int timeConfig = Int32.Parse(cnn.Configs.Where(c => c.NameConstant.Equals(Constant.TIME_WITHDRAW)).FirstOrDefault().ValueConstant);
                DateTime? checkDate = cnn.RequestWithdraws.Where(r => r.IsActive.Equals(SystemParam.ACTIVE) && r.ShiperID.Equals(mb.ShiperID.Value)).OrderByDescending(r => r.CreaedDate).Select(r => r.CreaedDate).FirstOrDefault();

                if (checkDate.HasValue && DateTime.Now < checkDate.Value.AddDays(timeConfig))
                    return rpBus.ErrorResult("Mỗi lần yêu cầu rút tiền phải cách nhau " + timeConfig + " ngày!", SystemParam.PROCESS_ERROR);
                //Check số dư trong tài khoản shiper
                Wallet wallet = mb.Wallets.Where(b => b.IsActive.Equals(SystemParam.ACTIVE) && b.Type.Equals(Constant.WALLET_WITHDRAW)).FirstOrDefault();
                if (wallet.Balance < input.amount)
                    return rpBus.ErrorResult(MessVN.NOT_ENOUGH_MONEY, SystemParam.PROCESS_ERROR);

                int beforeBlance = wallet.Balance;
                int afterBalance = wallet.Balance - (int)input.amount;
                RequestWithdraw rq = new RequestWithdraw();
                rq.ShiperID = mb.ShiperID.Value;
                rq.BankMemberID = input.bankID;
                rq.Account = input.account;
                rq.Amount = (int)input.amount;
                rq.Owner = input.owner;
                rq.CreaedDate = DateTime.Now;
                rq.Status = SystemParam.STATUS_REQUEST_WAITING;
                rq.IsActive = SystemParam.ACTIVE;
                //Lưu lại lịch sử giao dịch
                MembersTransactionHistory mbt = new MembersTransactionHistory();
                mbt.MemberID = input.shiperID;
                mbt.Title = "Yêu cầu rút tiền";
                mbt.Content = !String.IsNullOrEmpty(input.content) ? input.content : "";
                mbt.BeforeBalance = beforeBlance;
                mbt.AfterBalance = afterBalance;
                mbt.Type = Constant.SUBTRACT_POINT;
                mbt.TransactionType = Constant.TYPE_TRANSACTION_WITHDRAW;
                mbt.Status = Constant.STATUS_TRANSACTION_SUCCESS;
                mbt.Icon = "";
                mbt.WalletID = wallet.ID;
                mbt.Point = (int)input.amount;
                MembersTransactionHistory history = transactionBus.CreateTransactionHistory(mbt);

                //Lưu lại thông báo
                string content = "Yêu cầu rút tiền của bạn đã được thực hiện và đang được phê duyệt";
                Notification nt = new Notification();
                nt.MemberID = mb.ID;
                nt.Content = content;
                nt.Title = content;
                nt.Type = SystemParam.NOTI_TYPE_WITH_DRAW_REQUEST;
                nt.IsRead = false;
                nt.IsActive = SystemParam.ACTIVE;
                nt.CreateDate = DateTime.Now;
                nt.Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10);
                cnn.Notifications.Add(nt);
                cnn.RequestWithdraws.Add(rq);
                cnn.MembersTransactionHistories.Add(history);
                wallet.Balance = afterBalance;
                mb.Shiper.WithDrawSum += input.amount;
                cnn.SaveChanges();
                connect.Commit();
                connect.Dispose();
                //Tiến hành gửi thông báo đến web admin
                var url = SystemParam.URL_WEB_SOCKET + "?content=Một yêu cầu rút tiền mới đang đợi được xác nhận";
                notify.GetJson(url);

                //Push noti to app
                if (!String.IsNullOrEmpty(mb.DeviceID) && mb.DeviceID.Length > 10)
                {
                    NotifyDataModel notifyData = new NotifyDataModel();
                    List<string> lstDeviceID = new List<string>();
                    notifyData.type = SystemParam.NOTI_TYPE_WITH_DRAW_REQUEST;
                    notifyData.code = nt.Code;
                    notifyData.content = nt.Content;
                    lstDeviceID.Add(mb.DeviceID);
                   
                    string value = push.PushNotify(notifyData, lstDeviceID, SystemParam.WE_SHIP_NOTI, notifyData.content, 2);
                    push.PushOneSignal(value, 2);
                }
              
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                connect.Rollback();
                connect.Dispose();
                return rpBus.serverError();
            }
        }
        public JsonResultModel RechargeMoneyToWallet(int memberId, long Amount)
        {
            try
            {
                //Ví cọc
                Wallet depositWallet = cnn.Wallets.FirstOrDefault(wl => wl.Type.Equals(Constant.WALLET_NO_WITHDRAW) &&
                wl.IsActive.Equals(SystemParam.ACTIVE) && wl.MemberID.Equals(memberId));
                //Lưu lại lịch sử giao dịch của ví cọc
                string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 8);
                MembersTransactionHistory mbt = new MembersTransactionHistory();
                mbt.MemberID = memberId;
                mbt.Title = "Nạp tiền ví cọc";
                mbt.Content = "Nạp " + Amount + "đ vào ví cọc tài xế";
                mbt.BeforeBalance = depositWallet.Balance;
                mbt.AfterBalance = depositWallet.Balance + (int)Amount;
                mbt.Type = Constant.RECHAGE;
                mbt.TransactionType = Constant.TYPE_TRANSACTION_RECHARGE;
                mbt.Status = Constant.STATUS_TRANSACTION_WAITING;
                mbt.Icon = "";
                mbt.Point = (int)Amount;
                mbt.WalletID = depositWallet.ID;
                mbt.TransactionID = code;
                mbt.IsExtra = true;
                mbt.CreateDate = DateTime.Now;
                mbt.IsActive = SystemParam.ACTIVE;
                cnn.MembersTransactionHistories.Add(mbt);
                cnn.SaveChanges();

                var VnPayUrl = vnPayBus.GetUrl(mbt.ID);
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, VnPayUrl);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return rpBus.serverError();
            }
        }
        public JsonResultModel RechargeMoneyToDepositWallet(RequestWithDrawInputModel input)
        {
            try
            {
                var connect = cnn.Database.BeginTransaction();
                List<Wallet> wallets = cnn.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.MemberID.Equals(input.shiperID)).ToList();
                //Ví cọc
                Wallet depositWallet = wallets.Where(wl => wl.Type.Equals(Constant.WALLET_NO_WITHDRAW)).FirstOrDefault();
                //Ví thu nhập
                Wallet inceomeWallet = wallets.Where(wl => wl.Type.Equals(Constant.WALLET_WITHDRAW)).FirstOrDefault();
                if (inceomeWallet.Balance < input.amount)
                    return rpBus.ErrorResult(MessVN.NOT_ENOUGH_MONEY, SystemParam.PROCESS_ERROR);
                if (input.amount > SystemParam.MAX_TRANSACTION_MONEY)
                    return rpBus.ErrorResult(MessVN.MONEY_INVALID, SystemParam.PROCESS_ERROR);
                List<MembersTransactionHistory> lstMembersTransactionsHistory = new List<MembersTransactionHistory>();
                //Lưu lại lịch sử giao dịch của ví cọc
                string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 8);
                MembersTransactionHistory mbt = new MembersTransactionHistory();
                mbt.MemberID = input.shiperID;
                mbt.Title = "Cộng tiền từ ví thu nhập";
                mbt.Content = String.IsNullOrEmpty(input.content) ? input.content : "";
                mbt.BeforeBalance = depositWallet.Balance;
                mbt.AfterBalance = depositWallet.Balance + (int)input.amount;
                mbt.Type = Constant.RECHAGE;
                mbt.TransactionType = Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET_EXCHANGE;
                mbt.Status = Constant.STATUS_TRANSACTION_SUCCESS;
                mbt.Icon = "";
                mbt.Point = (int)input.amount;
                mbt.WalletID = depositWallet.ID;
                mbt.TransactionID = code;
                lstMembersTransactionsHistory.Add(transactionBus.CreateTransactionHistory(mbt));
                //Lưu lại lịch sử giao dịch của thu nhập
                MembersTransactionHistory mb = new MembersTransactionHistory();
                mb.MemberID = input.shiperID;
                mb.Title = "Chuyển tiền đến ví cọc";
                mb.Content = String.IsNullOrEmpty(input.content) ? input.content : "";
                mb.BeforeBalance = inceomeWallet.Balance;
                mb.AfterBalance = inceomeWallet.Balance - (int)input.amount;
                mb.Type = Constant.SUBTRACT_POINT;
                mb.TransactionType = Constant.TYPE_TRANSACTION_TRANSFER_WALLET;
                mb.Status = Constant.STATUS_TRANSACTION_SUCCESS;
                mb.Icon = "";
                mb.Point = (int)input.amount;
                mb.WalletID = inceomeWallet.ID;
                mb.TransactionID = code;
                lstMembersTransactionsHistory.Add(transactionBus.CreateTransactionHistory(mb));
                //Tính lại các giá trị và tiến hành lưu lại
                depositWallet.Balance = depositWallet.Balance + (int)input.amount;
                inceomeWallet.Balance = inceomeWallet.Balance - (int)input.amount;
                cnn.MembersTransactionHistories.AddRange(lstMembersTransactionsHistory);
                cnn.SaveChanges();
                connect.Commit();
                connect.Dispose();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return rpBus.serverError();
            }
        }

        //Lấy danh sách lịch sử ví
        public IPagedList<ListWalletHistoryModel> GetListWalletHistory(int page, int limit, int memberID, int? type)
        {
            try
            {
                var data = cnn.MembersTransactionHistories.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && m.Status == Constant.STATUS_TRANSACTION_SUCCESS
                && m.WalletID.HasValue && (type.HasValue ? m.Wallet.Type.Equals(type.Value) : true) && m.MemberID.Equals(memberID))
                     .Select(m => new ListWalletHistoryModel()
                     {
                         id = m.ID,
                         amount = m.Point.Value,
                         createdDate = m.CreateDate,
                         content = m.Content,
                         title = m.Title,
                         type = type.HasValue ? type.Value : 1,
                         isPlus = m.Type,
                         code = m.TransactionID,
                         typeTransaction = m.TransactionType
                     }).OrderByDescending(m => m.id).ToPagedList(page, limit);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListWalletHistoryModel>().ToPagedList(1, 1);
            }
        }

        //tìm kiếm
        public IPagedList<TransactionHistoryWalletModel> SearchWallet(int page, string searchKey, int? walletType, int? transactionType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                DateTime? startDate = Util.ConvertDate(fromDate);
                DateTime? endDate = Util.ConvertDate(toDate);
                if (endDate.HasValue)
                    endDate = endDate.Value.AddDays(1);
                var data = cnn.MembersTransactionHistories.Where(m => m.IsActive.Equals(SystemParam.ACTIVE)
                && (!String.IsNullOrEmpty(searchKey) ? m.Member.Shiper.Name.Contains(searchKey) || m.Member.Shiper.Phone.Contains(searchKey) : true)
                 && (walletType.HasValue ? m.Wallet.Type.Equals(walletType.Value) : true)
                 &&(m.Member.Shiper != null) && (m.Status.Equals(Constant.STATUS_PAYMENT_COMPLETE))
                && (transactionType.HasValue ? m.TransactionType.Equals(transactionType.Value) : true)
                && (provinceID.HasValue ? m.Member.Shiper.ShiperAreas.Select(x => x.Area.District.ProvinceID).Contains(provinceID.Value) : true)
                && (districtID.HasValue ? m.Member.Shiper.ShiperAreas.Select(x => x.Area.DistrictID).Contains(districtID.Value) : true)
                && (startDate.HasValue ? m.CreateDate >= startDate.Value : true) && (endDate.HasValue ? m.CreateDate <= endDate.Value : true))
                    .Select(m => new TransactionHistoryWalletModel
                    {
                        Id = m.ID,
                        Name = m.Member.Shiper.Name,
                        Phone = m.Member.Shiper.Phone,
                        WalletType = m.Wallet.Type,
                        MemType = m.Type,
                        TransactionType = m.TransactionType,
                        Point = m.Point,
                        AfterBalance = m.AfterBalance,
                        CreateDate = m.CreateDate
                    }).OrderByDescending(m => m.CreateDate).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<TransactionHistoryWalletModel>().ToPagedList(1, 1);
            }
        }
        //Lấy dữ liệu thống kê ví
        public List<TransactionHistoryWalletModel> GetDataWallet(string searchKey, int? walletType, int? transactionType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                DateTime? startDate = Util.ConvertDate(fromDate);
                DateTime? endDate = Util.ConvertDate(toDate);
                if (endDate.HasValue)
                    endDate = endDate.Value.AddDays(1);
                var data = cnn.MembersTransactionHistories.Where(m => m.IsActive.Equals(SystemParam.ACTIVE)
                && (!String.IsNullOrEmpty(searchKey) ? m.Member.Shiper.Name.Contains(searchKey) || m.Member.Shiper.Phone.Contains(searchKey) : true)
                 && (walletType.HasValue ? m.Wallet.Type.Equals(walletType.Value) : true) && (m.Status.Equals(Constant.STATUS_PAYMENT_COMPLETE))
                && (transactionType.HasValue ? m.TransactionType.Equals(transactionType.Value) : true)
                && (provinceID.HasValue ? m.Member.Shiper.ShiperAreas.Select(x => x.Area.District.ProvinceID).Contains(provinceID.Value) : true)
                && (districtID.HasValue ? m.Member.Shiper.ShiperAreas.Select(x => x.Area.DistrictID).Contains(districtID.Value) : true)
                && (startDate.HasValue ? m.CreateDate >= startDate.Value : true) && (endDate.HasValue ? m.CreateDate <= endDate.Value : true))
                    .Select(m => new TransactionHistoryWalletModel
                    {
                        Id = m.ID,
                        Name = m.Member.Shiper.Name,
                        Phone = m.Member.Shiper.Phone,
                        WalletType = m.Wallet.Type,
                        MemType = m.Type,
                        TransactionType = m.TransactionType,
                        Point = m.Point,
                        AfterBalance = m.AfterBalance,
                        CreateDate = m.CreateDate
                    }).OrderByDescending(m => m.CreateDate).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<TransactionHistoryWalletModel>();
            }
        }
        //Xuất bản excel thống kê ví
        public ExcelPackage ExportWallet(string searchKey, int? walletType, int? transactionType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                List<TransactionHistoryWalletModel> data = GetDataWallet(searchKey, walletType, transactionType, provinceID, districtID, fromDate, toDate);
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/wallet_transaction.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;
                if (data != null && data.Count() > 0)
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = stt;
                        sheet.Cells[row, 2].Value = dt.Name;
                        sheet.Cells[row, 3].Value = dt.Phone;
                        switch (dt.WalletType)
                        {
                            case Constant.WALLET_NO_WITHDRAW:
                                sheet.Cells[row, 4].Value = "Ví cọc";
                                break;
                            case Constant.WALLET_WITHDRAW:
                                sheet.Cells[row, 4].Value = "Ví thu nhập";
                                break;
                        }
                        switch (dt.TransactionType)
                        {
                            case Constant.TYPE_TRANSACTION_WITHDRAW:
                                sheet.Cells[row, 5].Value = "Rút tiền";
                                break;
                            case Constant.TYPE_TRANSACTION_REFUND_WITHDRAW:
                                sheet.Cells[row, 5].Value = "Hoàn tiền khi yêu cầu rút tiền bị từ chối";
                                break;
                            case Constant.TYPE_TRANSACTION_TRANSFER_WALLET:
                                sheet.Cells[row, 5].Value = "Chuyển tiền sang ví cọc";
                                break;
                            case Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET:
                                sheet.Cells[row, 5].Value = "Nhận tiền sau khi hoàn thành đơn";
                                break;
                            case Constant.TYPE_TRANSACTION_RECHARGE:
                                sheet.Cells[row, 5].Value = "Nạp tiền từ hệ thống";
                                break;
                            case Constant.TYPE_TRANSACTION_RECHARGE_ADMIN:
                                sheet.Cells[row, 5].Value = "Cộng tiền từ Admin";
                                break;
                            case Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET_EXCHANGE:
                                sheet.Cells[row, 5].Value = "Nhận tiền từ ví thu nhập";
                                break;
                            case Constant.TYPE_TRANSACTION_ACCEPT_ORDER:
                                sheet.Cells[row, 5].Value = "Trừ tiền ví cọc khi nhận đơn";
                                break;
                            case Constant.TYPE_TRANSACTION_REFUND_ORDER_CANCLE:
                                sheet.Cells[row, 5].Value = "Hoàn tiền ví cọc khi đơn bị hủy";
                                break;
                        }
                        switch (dt.MemType)
                        {
                            case 1:
                                sheet.Cells[row, 6].Value = "+" + string.Format("{0:#,0}", Convert.ToDecimal(dt.Point));
                                break;
                            case 0:
                                sheet.Cells[row, 6].Value = "-" + string.Format("{0:#,0}", Convert.ToDecimal(dt.Point));
                                break;
                        }
                        sheet.Cells[row, 7].Value = string.Format("{0:#,0}", Convert.ToDecimal(dt.AfterBalance));
                        sheet.Cells[row, 8].Value = dt.CreateDate.ToString("dd/MM/yyyy");
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

    }
}

