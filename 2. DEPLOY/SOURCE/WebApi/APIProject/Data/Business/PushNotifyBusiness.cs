using Data.DB;
using Data.Model;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class PushNotifyBusiness:GenericBusiness
    {
        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int length = 6;
        public PushNotifyBusiness(WE_SHIPEntities context = null) : base()
        {
        }
        // push noti cho app
        public void PushNotifyapp(string Title,int Type,int? OrderServiceId,int MemberId, int AppType)
        {
            Notification notify = new Notification
            {
                Title = Title,
                Content = Title,
                Type = Type,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now,
                IsRead = false,
                MemberID = MemberId,
                Code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                OrderServiceID = OrderServiceId
            };
            cnn.Notifications.Add(notify);
            cnn.SaveChanges();
            NotifyDataModel notifyData = new NotifyDataModel();
            notifyData.id = OrderServiceId.GetValueOrDefault();
            notifyData.type = Type;
            notifyData.code = "";
            notifyData.content = Title;
            string memberDevice = cnn.Members.Where(x => x.IsActive == SystemParam.ACTIVE && x.ID == MemberId).Select(x => x.DeviceID).FirstOrDefault();
            List<string> listDevice = new List<string>();
            if (memberDevice != null && memberDevice.Length > 10)
            {
                listDevice.Add(memberDevice);
            }
            string value = PushNotify(notifyData, listDevice, SystemParam.WE_SHIP_NOTI, notifyData.content, AppType);
            PushOneSignal(value, AppType);
        }

        public string PushNotify(object obj, List<string> deviceID, string title, string contents, int check = 1)
        {
            // check 1 : app khachs hang --- check 2 : app tài xế
            var appid = SystemParam.APP_ID;
            var channelid = SystemParam.ANDROID_CHANNEL_ID;
            if (check == 2)
            {
                appid = SystemParam.APP_DRIVER_ID;
                channelid = SystemParam.ANDROID_CHANNEL_ID_DRIVER;
            }
            OneSignalInputs input = new OneSignalInputs();
            if (check == 3)
            {
                appid = SystemParam.APP_DRIVER_ID;
                channelid = SystemParam.ANDROID_CHANNEL_ID_DRIVER_SOUND;
                input.ios_sound = SystemParam.COUNTDOWN_DRIVER_SOUND;
            }
            TextInput header = new TextInput();
            header.en = contents.Length > 0 ? title : SystemParam.WE_SHIP_NOTI;
            TextInput content = new TextInput();
            content.en = contents.Length > 0 ? contents : title;
            input.app_id = appid;
            input.data = obj;
            input.headings = header;
            input.contents = content;
            input.android_channel_id = channelid;
            input.include_player_ids = deviceID;
            return JsonConvert.SerializeObject(input);
        }


        public string PushOneSignal(string value, int check = 1)
        {
            string url = SystemParam.URL_ONESIGNAL;

            var req = HttpWebRequest.Create(string.Format(url));
            var author = SystemParam.Authorization;
            if (check == 2 || check == 3)
            {
                author = SystemParam.Authorization_driver;
            }

            req.Headers["Authorization"] = author;
            req.Headers["https"] = SystemParam.URL_BASE_https;
            var byteData = Encoding.UTF8.GetBytes(value);
            req.ContentType = "application/json";
            req.Method = "POST";
            try
            {
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)req.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }
        public void SaveLog(string content, string body)
        {
            var reportDirectory = string.Format("~/text/{0}/", DateTime.Now.ToString("yyyy-MM-dd"));
            reportDirectory = System.Web.Hosting.HostingEnvironment.MapPath(reportDirectory);
            if (!Directory.Exists(reportDirectory))
            {
                Directory.CreateDirectory(reportDirectory);
            }
            var dailyReportFullPath = string.Format("{0}text_{1}.log", reportDirectory, DateTime.Now.Day);
            var logContent = string.Format("{0}-{1}-{2}", DateTime.Now, "noti: " + content + " / " + body, Environment.NewLine);
            File.AppendAllText(dailyReportFullPath, logContent);
        }
    }
}
