using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Business
{
    public class OneSignalBusiness : GenericBusiness
    {
        public OneSignalBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }
        //public string StartPushNoti(object obj, List<string> deviceID, string contents, int type, string headerStr, int? sound = null)
        //{
        //    OneSignalInputs input = new OneSignalInputs();
        //    TextInput header = new TextInput();
        //    header.en = headerStr;
        //    TextInput content = new TextInput();
        //    content.en = contents;
        //    input.app_id = type.Equals(Constant.CUSTOMER_ROLE) ? CustomerOnesignal.APP_ID : WasherOnesignal.APP_ID;
        //    input.data = obj;
        //    input.headings = header;
        //    input.ios_sound = ((DataOnesignal)obj).type == Constant.NOTI_ORDER_STATUS_CONFIRM ? WasherOnesignal.IOS_SOUND_ORDER : WasherOnesignal.IOS_SOUND_DEFAULT;
        //    input.contents = content;
        //    input.android_channel_id = type.Equals(Constant.CUSTOMER_ROLE) ? CustomerOnesignal.ANDROID_CHANNEL_ID : (sound.HasValue ? WasherOnesignal.ANDROID_CHANNEL_ID : WasherOnesignal.NO_SOUND);
        //    input.include_player_ids = deviceID;
        //    input.ttl = 180;
        //    return JsonConvert.SerializeObject(input);
        //}

        //public OneSignalOutputModel PushOneSignals(string value, string deviceID = "")
        //{

        //    string url = SystemParam.URL_ONESIGNAL;
        //    var req = HttpWebRequest.Create(string.Format(url));
        //    req.Headers["Authorization"] = SystemParam.Authorization;
        //    req.Headers["https"] = SystemParam.URL_BASE_https;
        //    var byteData = Encoding.UTF8.GetBytes(value);
        //    req.ContentType = "application/json";
        //    req.Method = "POST";
        //    try
        //    {
        //        using (var stream = req.GetRequestStream())
        //        {
        //            stream.Write(byteData, 0, byteData.Length);
        //        }
        //        var response = (HttpWebResponse)req.GetResponse();
        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //        OneSignalOutputModel data = JsonConvert.DeserializeObject<OneSignalOutputModel>(responseString.ToString());
        //        SaveLog(responseString, value);
        //        return data;

        //    }
        //    catch (WebException e)
        //    {
        //        SaveLog(e.ToString(), deviceID);
        //        OneSignalOutputModel data = new OneSignalOutputModel { errors = e.ToString() };
        //        return data;
        //    }
        //}
        //public void SaveLog(string content, string body)
        //{
        //    var reportDirectory = string.Format("~/text/{0}/", DateTime.Now.ToString("yyyy-MM-dd"));
        //    reportDirectory = System.Web.Hosting.HostingEnvironment.MapPath(reportDirectory);
        //    if (!Directory.Exists(reportDirectory))
        //    {
        //        Directory.CreateDirectory(reportDirectory);
        //    }
        //    var dailyReportFullPath = string.Format("{0}text_{1}.log", reportDirectory, DateTime.Now.Day);
        //    var logContent = string.Format("{0}-{1}-{2}", DateTime.Now, "noti: " + content + " / " + body, Environment.NewLine);
        //    File.AppendAllText(dailyReportFullPath, logContent);
        //}

        //public string PushNotify(int? orderID, string content, int type, int memberID, int? newID, int? productID, string code, string header, int? sound = null, string comboCode = "")
        //{
        //    try
        //    {
        //        Member member = cnn.Members.Find(memberID);
        //        int role = member.CustomerID.HasValue ? Constant.CUSTOMER_ROLE : Constant.AGENT_ROLE;
        //        List<string> deviceID = new List<string>();
        //        if (!String.IsNullOrEmpty(member.DeviceID) && member.DeviceID.Length > 0)
        //        {
        //            deviceID.Add(member.DeviceID);
        //            DataOnesignal data = new DataOnesignal();
        //            data.timeWait = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.TIME_DELAY_PUSH_NOTI)).FirstOrDefault().ValueConstant) * SystemParam.MINI_SECOND;
        //            data.orderServiceID = orderID;
        //            data.newsID = newID;
        //            data.type = type;
        //            data.productID = productID;
        //            data.AgentCode = code;
        //            data.deviceID = deviceID.FirstOrDefault();
        //            data.comboCode = comboCode;
        //            string value = StartPushNoti(data, deviceID, content, role, header, sound);
        //            RetryPushNoty(value, member.DeviceID);
        //        }
        //        return Constant.STATUS_SUCCESS;
        //    }
        //    catch (Exception ex)
        //    {
        //        return MessVN.ERROR_STR;
        //    }
        //}
        //public void SendNotifyForWasher(int? orderID, string content, int type, List<string> lsDeviceID, int? newID, int? productID, string code, string header, int? sound = null, string comboCode = "", int role = Constant.AGENT_ROLE)
        //{
        //    try
        //    {
        //        DataOnesignal data = new DataOnesignal();
        //        data.timeWait = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.TIME_DELAY_PUSH_NOTI)).FirstOrDefault().ValueConstant) * SystemParam.MINI_SECOND;
        //        data.orderServiceID = orderID;
        //        data.newsID = newID;
        //        data.type = type;
        //        data.productID = productID;
        //        data.AgentCode = code;
        //        data.deviceID = lsDeviceID.FirstOrDefault();
        //        data.comboCode = comboCode;
        //        string value = StartPushNoti(data, lsDeviceID, content, role, header, sound);
        //        PushOneSignals(value);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        //public void MultiplePushNoti(List<string> deviceID, string title, string contents, int role, string lang, int? newID = null, int typenoti = Constant.NOTI_FROM_ADMIN)
        //{
        //    OneSignalInputs input = new OneSignalInputs();
        //    TextInput header = new TextInput();
        //    header.en = newID.HasValue ? "Blog CarRect" : lang.Equals(SystemParam.EN) && role.Equals(Constant.AGENT_ROLE) ? "Notification" : "Thông báo";
        //    DataOnesignal data = new DataOnesignal();
        //    data.timeWait = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.TIME_DELAY_PUSH_NOTI)).FirstOrDefault().ValueConstant) * SystemParam.MINI_SECOND;
        //    data.type = typenoti;
        //    data.deviceID = deviceID.FirstOrDefault();
        //    data.newsID = newID;
        //    TextInput content = new TextInput();
        //    content.en = title;
        //    input.app_id = role.Equals(Constant.CUSTOMER_ROLE) ? CustomerOnesignal.APP_ID : WasherOnesignal.APP_ID;
        //    input.data = data;
        //    input.headings = header;
        //    input.ios_sound = typenoti == Constant.NOTI_ORDER_STATUS_CONFIRM ? WasherOnesignal.IOS_SOUND_ORDER : WasherOnesignal.IOS_SOUND_DEFAULT;
        //    input.contents = content;
        //    input.android_channel_id = role.Equals(Constant.CUSTOMER_ROLE) ? CustomerOnesignal.ANDROID_CHANNEL_ID : WasherOnesignal.NO_SOUND;
        //    input.ttl = 180;
        //    if (deviceID.Count <= Constant.MAX_DEVICEID_ONESIGNAL)
        //    {
        //        input.include_player_ids = deviceID.Where(u => u.Length > 10).ToList();
        //        string value = JsonConvert.SerializeObject(input);
        //        OneSignalOutputModel ouput = PushOneSignals(value);
        //    }
        //    else
        //    {
        //        int count = deviceID.Count / Constant.MAX_DEVICEID_ONESIGNAL;
        //        int remain = deviceID.Count % Constant.MAX_DEVICEID_ONESIGNAL;
        //        for (int index = 0; index <= count; index++)
        //        {
        //            List<string> listDeviceID = deviceID.GetRange(index * Constant.MAX_DEVICEID_ONESIGNAL, Constant.MAX_DEVICEID_ONESIGNAL);
        //            input.include_player_ids = listDeviceID;
        //            string values = JsonConvert.SerializeObject(input);
        //            OneSignalOutputModel ouputs = PushOneSignals(values);
        //        }
        //        List<string> lsDeviceID = deviceID.GetRange(count * Constant.MAX_DEVICEID_ONESIGNAL, Constant.MAX_DEVICEID_ONESIGNAL);
        //        input.include_player_ids = lsDeviceID;
        //        string value = JsonConvert.SerializeObject(input);
        //        OneSignalOutputModel ouput = PushOneSignals(value);
        //    }
        //}
        //public void MultiplePushNotiWeb(List<string> deviceID, string title, string contents, int role, string lang, int? newID, int typenoti = Constant.NOTI_FROM_ADMIN)
        //{
        //    OneSignalInputs input = new OneSignalInputs();
        //    TextInput header = new TextInput();
        //    header.en = title;
        //    DataOnesignal data = new DataOnesignal();
        //    data.timeWait = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.TIME_DELAY_PUSH_NOTI)).FirstOrDefault().ValueConstant) * SystemParam.MINI_SECOND;
        //    data.type = typenoti;
        //    data.deviceID = deviceID.FirstOrDefault();
        //    data.newsID = newID;
        //    TextInput content = new TextInput();
        //    content.en = contents;
        //    input.app_id = role.Equals(Constant.CUSTOMER_ROLE) ? CustomerOnesignal.APP_ID : WasherOnesignal.APP_ID;
        //    input.data = data;
        //    input.headings = header;
        //    input.ios_sound = typenoti == Constant.NOTI_ORDER_STATUS_CONFIRM ? WasherOnesignal.IOS_SOUND_ORDER : WasherOnesignal.IOS_SOUND_DEFAULT;
        //    input.contents = content;
        //    input.android_channel_id = role.Equals(Constant.CUSTOMER_ROLE) ? CustomerOnesignal.ANDROID_CHANNEL_ID : WasherOnesignal.NO_SOUND;
        //    input.ttl = 180;
        //    if (deviceID.Count <= Constant.MAX_DEVICEID_ONESIGNAL)
        //    {
        //        input.include_player_ids = deviceID.Where(u => u.Length > 10).ToList();
        //        string value = JsonConvert.SerializeObject(input);
        //        OneSignalOutputModel ouput = PushOneSignals(value);
        //    }
        //    else
        //    {
        //        int count = deviceID.Count / Constant.MAX_DEVICEID_ONESIGNAL;
        //        int remain = deviceID.Count % Constant.MAX_DEVICEID_ONESIGNAL;
        //        for (int index = 0; index <= count - 1; index++)
        //        {
        //            List<string> listDeviceID = deviceID.GetRange(index * Constant.MAX_DEVICEID_ONESIGNAL, Constant.MAX_DEVICEID_ONESIGNAL);
        //            input.include_player_ids = listDeviceID;
        //            string values = JsonConvert.SerializeObject(input);
        //            OneSignalOutputModel ouputs = PushOneSignals(values);
        //        }
        //        List<string> lsDeviceID = deviceID.GetRange(count * Constant.MAX_DEVICEID_ONESIGNAL, remain);
        //        input.include_player_ids = lsDeviceID;
        //        string value = JsonConvert.SerializeObject(input);
        //        OneSignalOutputModel ouput = PushOneSignals(value);
        //    }
        //}
        //public void RetryPushNoty(string value, string deviceID, int i = 1)
        //{
        //    OneSignalOutputModel ouput = PushOneSignals(value, deviceID);
        //    if (i < SystemParam.MAX_RETRY_NOTI && ouput.errors != null)
        //    {
        //        i++;
        //        Task.Delay(10000).ContinueWith(t => RetryPushNoty(value, deviceID));
        //    }
        //}
    }
}
