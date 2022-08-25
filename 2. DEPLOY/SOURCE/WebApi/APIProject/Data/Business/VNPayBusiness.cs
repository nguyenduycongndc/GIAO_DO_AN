using Data.DB;
using Data.Model;
using Data.Utils;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.UI;
using static Data.Business.VNPAY_CS_ASPX;

namespace Data.Business
{
    public class VNPayBusiness : GenericBusiness
    {
        public VNPayBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        VnPayLibrary vnpay = new VnPayLibrary();
        PushNotifyBusiness oneSignalBus = new PushNotifyBusiness();
        BookingBusiness bookBus = new BookingBusiness();
        //OneSignalBusiness oneSignalBus = new OneSignalBusiness();
        //TransactionHistoryBusiness transactionBus = new TransactionHistoryBusiness();
        public string GetUrl(int TransactionID)
        {
            //Get Config Info
            //Get payment input
            //Build URL for VNPAY
            MembersTransactionHistory transaction = cnn.MembersTransactionHistories.Find(TransactionID);

            vnpay.AddRequestData("vnp_Version", "2.0.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", SystemParam.vnp_TmnCode_Real);
            string locale = "vn";//"en"
            if (!string.IsNullOrEmpty(locale))
            {
                vnpay.AddRequestData("vnp_Locale", locale);
            }
            else
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", transaction.ID.ToString());
            vnpay.AddRequestData("vnp_OrderInfo", transaction.Content);
            vnpay.AddRequestData("vnp_OrderType", "insurance"); //default value: other
            vnpay.AddRequestData("vnp_Amount", (transaction.Point * 100).ToString());
            vnpay.AddRequestData("vnp_ReturnUrl", SystemParam.vnp_Return_url_Real);
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            //if (bank.SelectedItem != null && !string.IsNullOrEmpty(bank.SelectedItem.Value))
            //{
            //    vnpay.AddRequestData("vnp_BankCode", bank.SelectedItem.Value);
            //}
            string paymentUrl = vnpay.CreateRequestUrl(SystemParam.vnp_Url_Real, SystemParam.vnp_HashSecret_Real);
            oneSignalBus.SaveLog("url", paymentUrl);
            return paymentUrl;
        }

        public static string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown"))
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }

        public VnpViewModel GetVnpReturn(VnpOutputModel vnp)
        {
            string json = JsonConvert.SerializeObject(vnp);
            oneSignalBus.SaveLog("url_return", json);
            VnpViewModel vnpOutput = new VnpViewModel();
            string lang = "vi";
            string Transaction_False = lang.Equals(SystemParam.VN) ? MessVN.Transaction_False_view : MessEN.Transaction_False_view;
            string Transaction_Succes = lang.Equals(SystemParam.VN) ? MessVN.Transaction_Succes : MessVN.Transaction_Succes;
            try
            {
                MembersTransactionHistory transaction = cnn.MembersTransactionHistories.Find(int.Parse(vnp.vnp_TxnRef));
                int? orderID = null;
                if (transaction != null)
                    orderID = transaction.OrderServiceID;
                int orderID1 = transaction.OrderServiceID.GetValueOrDefault();
                int money;
                var orderType = cnn.OrderServices.Where(x => x.ID == orderID1).Select(x => x.TypeBooking).FirstOrDefault();
                var orderStatus = cnn.OrderServices.Where(x => x.ID == orderID1).Select(x => x.Status).FirstOrDefault();
                try
                {
                    money = int.Parse(vnp.vnp_Amount) / 100;
                    if (money != transaction.Point)
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", money), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, orderID.HasValue ? SystemParam.customer_failed : SystemParam.driver_failed);
                        return vnpOutput;
                    }
                }
                catch (Exception ex)
                {
                    string jsonEx = JsonConvert.SerializeObject(ex);
                    oneSignalBus.SaveLog("Exepcion", jsonEx);
                    vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, orderID.HasValue ? SystemParam.customer_failed : SystemParam.driver_failed);
                    return vnpOutput;
                }

                if (vnp.vnp_ResponseCode == SystemParam.vnp_CodeSucces)
                {
                    if (transaction != null)
                    {
                        if(orderStatus == SystemParam.ORDER_STATUS_DENY)
                        {
                            vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, orderID.HasValue ? (SystemParam.customer_failed + orderID.Value + '/' + orderType) : SystemParam.driver_failed);
                        }
                        else
                        {
                            vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_Succes, orderID.HasValue ? (SystemParam.customer_success + orderID.Value + '/' + orderType) : SystemParam.driver_success);
                        }
                    }
                    else
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, orderID.HasValue ? (SystemParam.customer_failed + orderID.Value + '/' + orderType) : SystemParam.driver_failed);
                    }
                }
                else
                {
                    if (transaction != null)
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, orderID.HasValue ? (SystemParam.customer_failed + orderID.Value + '/' + orderType) : SystemParam.driver_failed);

                    }
                    else
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, orderID.HasValue ? (SystemParam.customer_failed + orderID.Value + '/' + orderType) : SystemParam.driver_failed);
                }
            }
            catch (Exception ex)
            {
                string jsonEx = JsonConvert.SerializeObject(ex);
                oneSignalBus.SaveLog("Exepcion", jsonEx);



                vnpOutput.getVnpModel(vnp.vnp_TxnRef, "", DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
            }
            return vnpOutput;
        }

        public VNPayOutputModel GetVnpIpn(VnpOutputModel vnp)
        {

            VNPayOutputModel output = new VNPayOutputModel();
            string lang = SystemParam.VN;
            try
            {
                string json = JsonConvert.SerializeObject(vnp);
                oneSignalBus.SaveLog("url_ipn", json);
                PushNotifyBusiness noti = new PushNotifyBusiness();
                int appID = 0;
                appID = int.Parse(vnp.vnp_TxnRef);
                MembersTransactionHistory transaction = cnn.MembersTransactionHistories.Find(int.Parse(vnp.vnp_TxnRef));
                if (transaction == null)
                {
                    output = output.GetPayOutputModel("Order not found", "01");
                    return output;
                }
                if (transaction != null)
                {
                    int money = 0;
                    try
                    {
                        if (transaction.OrderServiceID.HasValue)
                        {
                            var orderID = transaction.OrderServiceID.GetValueOrDefault();
                            var order = cnn.OrderServices.FirstOrDefault(x => x.ID == orderID);
                            if (order == null)
                            {
                                output = output.GetPayOutputModel("Order not found", "01");
                                return output;
                            }
                        }
                        money = int.Parse(vnp.vnp_Amount) / 100;
                        if (money != transaction.Point)
                        {
                            output = output.GetPayOutputModel("Invalid amount", "04");
                            string Transaction_False = (lang.Equals(SystemParam.VN) ? MessVN.Transaction_False : MessEN.Transaction_False) + output.RspCode;
                            return output;
                        }

                    }
                    catch
                    {
                        output = output.GetPayOutputModel("Invalid amount", "04");
                        string Transaction_False = (lang.Equals(SystemParam.VN) ? MessVN.Transaction_False : MessEN.Transaction_False) + output.RspCode;
                        return output;
                    }
                    bool checkSignature = vnpay.ValidateSignature(vnp.vnp_SecureHash, SystemParam.vnp_HashSecret_Real, vnp);
                    var orderIDCheck = transaction.OrderServiceID.GetValueOrDefault();
                    var orderCheck = cnn.OrderServices.FirstOrDefault(x => x.ID == orderIDCheck);
                    if (checkSignature)
                    {
                        try
                        {
                            if (vnp.vnp_ResponseCode == SystemParam.vnp_CodeSucces)
                            {

                                if (transaction.Status == Constant.STATUS_TRANSACTION_WAITING && (orderCheck != null ? orderCheck.Status == SystemParam.ORDER_STATUS_WAITING : true))
                                {
                                    if (transaction.OrderServiceID.HasValue)
                                    {
                                        var order = cnn.OrderServices.FirstOrDefault(x => x.ID == transaction.OrderServiceID.Value);
                                        order.BookingDate = DateTime.Now;
                                        order.StatusPayment = SystemParam.ORDER_PAYMENT;
                                        bookBus.FindDriver(order.ID);
                                        bookBus.PushCustomerRequestVnPay(order.ID, order.CustomerID, order.TypeBooking);
                                        cnn.SaveChanges();
                                    }
                                    else
                                    {
                                        var walletShipper = cnn.Wallets.FirstOrDefault(x => x.ID == transaction.WalletID.Value);
                                        walletShipper.Balance += transaction.Point.GetValueOrDefault();
                                        var title = "Bạn vừa nạp thành công " + Util.ConvertCurrency(transaction.Point.GetValueOrDefault()) + "đ vào ví cọc";
                                        var type = SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_MONEY_SHIPPER;
                                        oneSignalBus.PushNotifyapp(title, type, null, transaction.MemberID, 2);
                                        cnn.SaveChanges();
                                    }
                                    transaction.Status = Constant.STATUS_PAYMENT_COMPLETE;
                                    transaction.ConfirmDate = DateTime.Now;
                                    cnn.SaveChanges();
                                    string contentVN = "Giao dịch thành công: \n + Mã đơn hàng qua VNPAY: " + vnp.vnp_TxnRef + "\n + Số tiền: " + string.Format("{0:#,0}", transaction.Point) + " đ\n + Thời gian: " + DateTime.Now.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                                    string contentEN = "Transaction complete: \n + VNPAY order code: " + vnp.vnp_TxnRef + "\n + Amount: " + string.Format("{0:#,0}", transaction.Point) + " đ\n + Time: " + DateTime.Now.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                                    string content = lang.Equals(SystemParam.VN) ? contentVN : contentEN;
                                    output = output.GetPayOutputModel("Confirm Success", vnp.vnp_ResponseCode);
                                    return output;
                                }
                                else
                                {
                                    output = output.GetPayOutputModel("Order already confirmed", "02");
                                }

                            }
                            else
                            {
                                if (transaction.Status == Constant.STATUS_TRANSACTION_WAITING)
                                {
                                    transaction.Status = Constant.STATUS_TRANSACTION_FLASE;
                                    cnn.SaveChanges();
                                    if (transaction.OrderServiceID.HasValue)
                                    {
                                        var order = cnn.OrderServices.FirstOrDefault(x => x.ID == transaction.OrderServiceID.Value);
                                        order.Status = SystemParam.ORDER_STATUS_DENY;
                                        order.CancleDate = DateTime.Now;
                                        cnn.SaveChanges();
                                    }
                                    output = output.GetPayOutputModel("Confirm Success", "00");
                                    string Transaction_False = (lang.Equals(SystemParam.VN) ? MessVN.Transaction_False : MessEN.Transaction_False) + output.RspCode;
                                    return output;
                                }
                                else if (transaction.Status == Constant.STATUS_TRANSACTION_FLASE)
                                {
                                    output = output.GetPayOutputModel("Confirm Success", "00");
                                }
                                else
                                {
                                    output = output.GetPayOutputModel("Order already confirmed", "02");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            output = output.GetPayOutputModel("Unknow error", "99");
                            oneSignalBus.SaveLog("Exception", ex.ToString());
                            string Transaction_False = (lang.Equals(SystemParam.VN) ? MessVN.Transaction_False : MessEN.Transaction_False) + output.RspCode;
                        }
                    }
                    else
                    {
                        output = output.GetPayOutputModel("Invalid signature", "97");
                        string Transaction_False = (lang.Equals(SystemParam.VN) ? MessVN.Transaction_False : MessEN.Transaction_False) + output.RspCode;
                    }
                }
                else
                    output = output.GetPayOutputModel("Order not found", "01");
            }
            catch (Exception ex)
            {
                output = output.GetPayOutputModel("Unknow error", "99");
                oneSignalBus.SaveLog("Exception", ex.ToString());
                string Transaction_False = (lang.Equals(SystemParam.VN) ? MessVN.Transaction_False : MessEN.Transaction_False) + output.RspCode;
            }
            oneSignalBus.SaveLog(output.RspCode, output.Message);
            return output;
        }
    }
}
