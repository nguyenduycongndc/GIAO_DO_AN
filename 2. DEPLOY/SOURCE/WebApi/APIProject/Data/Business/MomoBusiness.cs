using Data.DB;
using Data.Model;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Data.Business
{
    public class MomoBusiness : GenericBusiness
    {
        public MomoBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        OneSignalBusiness oneSignalBus = new OneSignalBusiness();
        TransactionHistoryBusiness transactionBus = new TransactionHistoryBusiness();
        NotifyBusiness noti = new NotifyBusiness();
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        public void paymentMomo(int transactionID)
        {
            MembersTransactionHistory transaction = cnn.MembersTransactionHistories.Find(transactionID);
            if (transaction != null && transaction.Status.Equals(Constant.STATUS_TRANSACTION_WAITING))
            {
                string lang = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(transaction.MemberID)).Select(u => u.Lang).FirstOrDefault();
                if (String.IsNullOrEmpty(lang) || lang.Length == 0)
                    lang = SystemParam.VN;
                if (transaction.Type.Equals(Constant.TYPE_TRANSACTION_RECHARGE))
                {
                    Wallet walet = cnn.Wallets.Find(transaction.WalletID.Value);
                    walet.Balance += transaction.Point;
                }
                transaction.Status = Constant.STATUS_PAYMENT_COMPLETE;
                transaction.ConfirmDate = DateTime.Now;
                if (!String.IsNullOrEmpty(transaction.ComboCode) && transaction.ComboCode.Length > 0)
                {
                    List<OrderService> list = cnn.OrderServices.Where(u => u.ServiceComboSegmentID.HasValue && u.CodeCombo.Equals(transaction.ComboCode)).ToList();
                    foreach (OrderService order in list)
                    {
                        order.Status = Constant.ORDER_STATUS_WAITING;
                        order.CreatedDate = DateTime.Now;
                    }
                    int? usePoint = list.Select(u => u.UsePoint).FirstOrDefault();
                    if (usePoint.HasValue && usePoint.Value > 0)
                        transactionBus.CreateTransaction(transaction.MemberID, usePoint.Value, Constant.TYPE_TRANSACTION_USE_POINT_CUSTOMER, Util.CreateMD5(Util.RandomNumber(0000, 9999).ToString()).Substring(0, 6), null, null, Constant.WALLET_NO_WITHDRAW, lang, Constant.STATUS_TRANSACTION_SUCCESS, transaction.ComboCode);
                    cnn.SaveChanges();
                    List<OrderService> listCheck = list.Where(u => u.FirstAgentID.HasValue).ToList();
                    if (listCheck.Count == list.Count)
                    {
                        OrderService order = listCheck.FirstOrDefault();
                        Member Washer = cnn.Members.Where(u => u.AgentID.Value.Equals(order.FirstAgentID.Value)).FirstOrDefault();
                        string contentTransaction = MessVN.NOTI_ORDER_STATUS_WAITING;
                        oneSignalBus.PushNotify(0, contentTransaction, Constant.NOTI_ORDER_STATUS_CONFIRM, Washer.ID, null, null, "", MessVN.NOTI_HEADER, 1, transaction.ComboCode);
                    }
                }
                else if (transaction.OrderServiceID.HasValue)
                {
                    OrderService order = cnn.OrderServices.Find(transaction.OrderServiceID.Value);
                    order.Status = Constant.ORDER_STATUS_WAITING;
                    order.CreatedDate = DateTime.Now;
                    if (order.FirstAgentID.HasValue)
                    {
                        Member Washer = cnn.Members.Where(u => u.AgentID.Value.Equals(order.FirstAgentID.Value)).FirstOrDefault();
                        string contentTransaction = MessVN.NOTI_ORDER_STATUS_WAITING;
                        oneSignalBus.PushNotify(order.ID, contentTransaction, Constant.NOTI_ORDER_STATUS_CONFIRM, Washer.ID, null, null, "", MessVN.NOTI_HEADER, 1);
                    }
                    if (order.UsePoint.HasValue && order.UsePoint.Value > 0)
                    {
                        transactionBus.CreateTransaction(transaction.MemberID, order.UsePoint.Value, Constant.TYPE_TRANSACTION_USE_POINT_CUSTOMER, Util.CreateMD5(Util.RandomNumber(0000, 9999).ToString()).Substring(0, 6), null, null, Constant.WALLET_NO_WITHDRAW, lang, Constant.STATUS_TRANSACTION_SUCCESS, transaction.ComboCode);
                    }
                }
                cnn.SaveChanges();
                string contentVN = "Giao dịch thành công: \n + Mã đơn hàng qua MoMo : " + transaction.TransactionID + "\n + Số tiền: " + string.Format("{0:#,0}", transaction.Point) + " đ\n + Thời gian: " + DateTime.Now.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                string contentEN = "Transaction complete: \n + Order code: " + transaction.TransactionID + "\n + Amount: " + string.Format("{0:#,0}", transaction.Point) + " đ\n + Time: " + DateTime.Now.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                string content = lang.Equals(SystemParam.VN) ? contentVN : contentEN;
                noti.CreateNoti(transaction.MemberID, Constant.NOTI_MOMO_SUCCESS, transaction.OrderServiceID, null, null, lang, content, true, transaction.ComboCode);
            }
        }
        public string signSHA256(string message, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-", "").ToLower();
                return hex;
            }
        }

        public string buildQueryHash(object input, string publicKey, int bite)
        {
            string json = JsonConvert.SerializeObject(input);
            byte[] data = Encoding.UTF8.GetBytes(json);
            string result = null;
            using (var rsa = new RSACryptoServiceProvider(bite))
            {
                try
                {
                    rsa.FromXmlString(publicKey);
                    var encryptedData = rsa.Encrypt(data, false);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    result = base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }

            }
            return result;

        }
        public object payment(string token, int transactionID, string customerNumber, int amount)
        {
            var listConfig = cnn.Configs.ToList();

            string payAppUrl = "https://test-payment.momo.vn/pay/app";
            string partnerCode = listConfig.Where(u => u.NameConstant.Equals("PARTNER_CODE")).Select(u => u.ValueConstant).FirstOrDefault();
            string publicKey = listConfig.Where(u => u.NameConstant.Equals("PUB_KEY")).Select(u => u.ValueConstant).FirstOrDefault();
            string xmlpublicKey = "<RSAKeyValue><Modulus>oApztkpQCpiQtBHpQWJAd0sc+nC1c0mBS1fmSsAjSA5uB7qxNRAVX1ZAJ0zjMIUnIiKfVB5xJqV4KJ9YDOqcioBB8R0jv70Q1EOYG8bksWy0Wc4eeGBjSIZjonsYwWOrtXD1YBSgCnJiCM0NAMqG3a3a/AYI9xiKgPwX+mB+1RFRtJlZxfGyfIXjzAx7qSoPJVWUVGsHdD+A0xDtJ/yOeBCIxRoE6ENsb/IHKOflnxIalwqAeXeRHPXRYzPBp9S2zfst0JB8tFxLgcfxxhjnZu0GnGKVnJR4bLOkmK5temBJPxwVNIplaGwtawrV4i+TFWP4wASCFgFwaPQkfLUjKoCD+aBHcsq5TxHuda4IRItIxfOwHyp0KQ2p3c9w0eW7FIQk2cCUxNGvWIsnuV+En7r5JvGUMQXGpBpSQ6tZ0JavKJyr4a85Uv/pSHKb637vIyW2UGbaCCoMsMKALPRpSBn+xhVgoVMGcxjW3LQJUK3nnNbwHEn252BlmiIUgfgInaP8YjmX3cMeEcCyUUptfxGKZfxeKXWyKrW9pl+FXHJwNVG/hBNEzePdPWPtPjtDDOEILWhqliV9Sui7L0oqmhL0m1PfGGax+xcYh+EixcBUKCyTBHjj3nDfuz2Bwjro2xRHd5LTt4gmaxQoPbEsXKQJ+196AOq+r2xf4smb6VU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            object dataHash = new { partnerCode = partnerCode, partnerRefId = transactionID.ToString(), amount = amount };
            string hash = buildQueryHash(dataHash, xmlpublicKey, 2048);
            PayAppMoMo inputPayApp = new PayAppMoMo
            {
                partnerCode = partnerCode,
                partnerRefId = transactionID.ToString(),
                customerNumber = customerNumber,
                appData = token,
                hash = hash,
            };
            string valuePayApp = JsonConvert.SerializeObject(inputPayApp);
            oneSignalBus.SaveLog("intput_payment", valuePayApp);
            string momoResponPayAppSTR = apiBus.Post(payAppUrl, valuePayApp);
            PaymentRespone momoResponPayApp = JsonConvert.DeserializeObject<PaymentRespone>(momoResponPayAppSTR);
            string json = JsonConvert.SerializeObject(momoResponPayApp);
            oneSignalBus.SaveLog("payment", json);
            if (momoResponPayApp.status == 0)
            {
                return confirm(transactionID, momoResponPayApp, listConfig);
            }
            else
                return momoResponPayApp;
        }
        public object confirm(int transactionID, PaymentRespone momoResponPayApp, List<Config> listConfig)
        {
            string comfirmUrl = "https://test-payment.momo.vn/pay/confirm";
            string partnerCode = listConfig.Where(u => u.NameConstant.Equals("PARTNER_CODE")).Select(u => u.ValueConstant).FirstOrDefault();
            string secret_key = listConfig.Where(u => u.NameConstant.Equals("SECRET_KEY")).Select(u => u.ValueConstant).FirstOrDefault();
            string requestID = Util.CreateMD5(momoResponPayApp.transid).ToLower();
            string dataConfirm = "partnerCode=" + partnerCode + "&partnerRefId=" + transactionID.ToString() + "&requestType=capture&requestId=" + requestID + "&momoTransId=" + momoResponPayApp.transid;
            string signalture = signSHA256(dataConfirm, "Xh68GuWMmNvrQgTIDHr9ir8hR9aXrY8e");
            ConfirmMomo inputConfirm = new ConfirmMomo
            {
                partnerCode = partnerCode,
                momoTransId = momoResponPayApp.transid,
                partnerRefId = transactionID.ToString(),
                requestId = requestID,
                signature = signalture,
                requestType = "capture",
            };
            string valueConfirm = JsonConvert.SerializeObject(inputConfirm);
            oneSignalBus.SaveLog("intput_confirm", valueConfirm);
            string momoResponConfirmStr = apiBus.Post(comfirmUrl, valueConfirm);
            PaymentRespone momoResponConfirm = JsonConvert.DeserializeObject<PaymentRespone>(momoResponConfirmStr);
            string json = JsonConvert.SerializeObject(momoResponConfirm);
            oneSignalBus.SaveLog("confirm", json);
            if (momoResponConfirm.status == 0)
            {
                paymentMomo(transactionID);
                MembersTransactionHistory histrory = cnn.MembersTransactionHistories.Find(transactionID);
                if (histrory != null)
                {
                    if (histrory.OrderServiceID.HasValue)
                    {
                        return histrory.OrderServiceID.Value;
                    }
                    else
                    {
                        return histrory.ComboCode;
                    }
                }
            }
            return null;
        }
    }
}
