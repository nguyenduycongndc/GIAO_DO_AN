using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Data.Model.APIApp;
using System.IO;
using System.Web;

namespace Data.Utils
{
    public class Util
    {
        public static bool validPhone(string phone)
        {
            return Regex.Match(phone, @"^0[1-9]{1}[0-9]{8,9}$").Success;
        }
        public static bool ValidateEmail(string Email)
        {
            return Regex.Match(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success;
        }

        public static bool validNumber(string number)
        {
            // \d bắt buộc là số, dấu + bắt buộc xuất hiện 1 lần
            return Regex.Match(number, @"^[\d]+$").Success;
        }
        public static string ConvertCurrency(long Price)
        {
            return Price.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("is"));
        }
        public static string ConvertCurrencyInt(int Price)
        {
            return Price.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("is"));
        }
        public static int? checkTokenApp(string token)
        {
            WE_SHIPEntities cnn = new WE_SHIPEntities();
            Member member = cnn.Members.Where(u => u.Token.Equals(token) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
            if (member != null)
            {
                if (member.CustomerID.HasValue)
                    return member.CustomerID;
                else if (member.ShiperID.HasValue)
                    return member.ShiperID;
                else if (member.ShopID.HasValue)
                    return member.ShopID;
                else
                    return null;
            }
            else
                return null;
        }
        public static string ConverStatusOrder(int Status)
        {
            if (StatusOrder.STATUS_WAITING.ToList().Contains(Status))
                return StatusOrder.STATUS_WAITING_STR;
            if (StatusOrder.STATUS_CONFIRMED.ToList().Contains(Status))
                return StatusOrder.STATUS_CONFIRMED_STR;
            if (StatusOrder.STATUS_WASHING.ToList().Contains(Status))
                return StatusOrder.STATUS_WASHING_STR;
            if (StatusOrder.STATUS_COMPLETE.ToList().Contains(Status))
                return StatusOrder.STATUS_COMPLETE_STR;
            else
                return StatusOrder.STATUS_CANCEL_STR;

        }
        public static void DeleteIamgeLocal(string url)
        {
            try
            {
                string rootFolder = HttpContext.Current.Server.MapPath(@"\Uploads\");
                string[] str = url.Split('/');
                string[] files = Directory.GetFiles(rootFolder);
                foreach (string file in files)
                {
                    string fileName = rootFolder + str[str.Length - 1];
                    if (file.Equals(fileName))
                    {
                        File.Delete(file);
                        Console.WriteLine($"{file} is deleted.");
                    }
                }
            }
            catch { }
        }
        public static double Distance(double la1, double lo1, double la2, double lo2)
        {
            double dLat = (la2 - la1) * (Math.PI / 180);
            double dLon = (lo2 - lo1) * (Math.PI / 180);
            double la1ToRad = la1 * (Math.PI / 180);
            double la2ToRad = la2 * (Math.PI / 180);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(la1ToRad) * Math.Cos(la2ToRad) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = 6371 * c;
            return Math.Round(d, 3);
        }

        public static string ConvertDateTimeActual(DateTime dateTime)
        {
            return "";
            //string date = dateTime.DayOfWeek.ToString();
            //var Days = lsday.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Day1.Equals(date)).Select(u => u).FirstOrDefault();
            //var listTime = Days.ClassShift.Shifts.Select(u => new
            //{
            //    Date = DateTime.ParseExact(u.shift1, SystemParam.CONVERT_DATETIME_HOUR, null)
            //}).Where(u => u.Date.TimeOfDay >= dateTime.TimeOfDay).FirstOrDefault();
            //if (listTime == null)
            //{
            //    listTime = Days.ClassShift.Shifts.Select(u => new
            //    {
            //        Date = DateTime.ParseExact(u.shift1, SystemParam.CONVERT_DATETIME_HOUR, null)
            //    }).FirstOrDefault();
            //}
            //DateTime time = dateTime.Date.AddHours(listTime.Date.Hour).AddMinutes(listTime.Date.Minute);
            //return time.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
        }
        public static string CreateMD5(string input)
        {
            //bam du lieu
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string CheckNullString(string input)
        {
            string output = "";
            try
            {
                output = input.ToString();
            }
            catch
            {

            }
            return output;
        }
        private static readonly string[] VietNamChar = new string[]
        {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
        };
        public static string Converts(string str)
        {
            try
            {
                if (!String.IsNullOrEmpty(str) && str.Length > 0)
                {
                    //Thay thế và lọc dấu từng char      
                    for (int i = 1; i < VietNamChar.Length; i++)
                    {
                        for (int j = 0; j < VietNamChar[i].Length; j++)
                            str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                    }
                    return str.ToLower();
                }
                return str;
            }
            catch
            {
                return "";
            }
        }
        public static int ConvertCurrencyToNumber(string str)
        {
            try
            {
                if (!String.IsNullOrEmpty(str))
                {
                    var number = str.Replace(",", "");
                    return Int32.Parse(number);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            else
                return builder.ToString().ToUpper();
        }
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public static string ConvertPhone(string phonenumber)
        {
            if (phonenumber.Contains("+84"))
            {
                int length = phonenumber.Length - 3;
                phonenumber = "0" + phonenumber.Substring(3, length);
            }
            return phonenumber;
        }

        /// <summary>
        /// Code SeriCard
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Code(string text)
        {
            char[] charArr = text.ToCharArray();
            byte[] input = Encoding.ASCII.GetBytes(charArr);
            List<string> lsoutput = new List<string>();
            foreach (byte by in input)
            {
                int value = (int)((by * SystemParam.KeyA) % SystemParam.KeyB + SystemParam.KeyC);
                string valuestr = Encoding.ASCII.GetString(new byte[] { (byte)value });
                lsoutput.Add(valuestr);
            }
            int balance1 = (int)((48 * SystemParam.KeyA) / SystemParam.KeyB);
            int balance2 = (int)((57 * SystemParam.KeyA) / SystemParam.KeyB);
            lsoutput.Add("!" + balance1.ToString());
            lsoutput.Add("!" + balance2.ToString());
            string output = DateTime.Now.ToString("HHyyyyssddmmMM!");
            foreach (string o in lsoutput)
            {
                output += o;
            }
            return output;
        }
        /// <summary>
        /// EnCode Card   
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EnCode(string text)
        {
            string[] lsString = text.Split('!');
            string inputstr = lsString[lsString.Length - 3];
            int balance1 = int.Parse(lsString[lsString.Length - 2]);
            int balance2 = int.Parse(lsString[lsString.Length - 1]);
            char[] charArr = inputstr.ToCharArray();
            byte[] input = Encoding.ASCII.GetBytes(charArr);
            string lsoutput = "";
            foreach (byte c in input)
            {
                float value = (c - SystemParam.KeyC + SystemParam.KeyB * balance1) / SystemParam.KeyA;
                if (value < 48 || value > 57 || value != (int)value)
                    value = (c - SystemParam.KeyC + SystemParam.KeyB * balance2) / SystemParam.KeyA;
                string output = Encoding.ASCII.GetString(new byte[] { (byte)value });
                lsoutput += output;
            }
            return lsoutput;
        }


        //Convert Datetime 
        public static Nullable<DateTime> ConvertDate(string date, string datepaser = SystemParam.CONVERT_DATETIME)
        {
            try
            {
                if (!String.IsNullOrEmpty(date))
                    return DateTime.ParseExact(date, datepaser, null);
                return null;
            }
            catch
            {
                return null;
            }

        } 
        //Convert Datetime 
        public static Nullable<DateTime> ConvertFromDate(string date, string datepaser = SystemParam.CONVERT_DATETIME_HAVE_HOUR)
        {
            try
            {
                if (!String.IsNullOrEmpty(date))
                {
                    var dateTime = "00:00 " + date ;
                    return DateTime.ParseExact(dateTime, datepaser, null);
                }
                    
                return null;
            }
            catch
            {
                return null;
            }

        } 
        //Convert Datetime 
        public static Nullable<DateTime> ConvertToDate(string date, string datepaser = SystemParam.CONVERT_DATETIME_HAVE_HOUR)
        {
            try
            {
                if (!String.IsNullOrEmpty(date))
                {
                    var dateTime = "23:59 " + date ;
                    return DateTime.ParseExact(dateTime, datepaser, null);
                }
                    
                return null;
            }
            catch
            {
                return null;
            }

        }

        //get name TYPE_ADD 
        public static string GetNameType(int ID)
        {
            string result = "";
            switch (ID)
            {
                case 1: result = "Tích điểm"; break;
                case 2: result = "Tặng điểm"; break;
                case 3: result = "Được tặng điểm"; break;
                case 4: result = "Đổi quà"; break;
                case 5: result = "Hệ thống cộng điểm"; break;
                case 6: result = "Đổi thẻ"; break;
                case 7: result = "Hủy yêu cầu đổi quà"; break;
            }
            return result;
        }

        public static string GetNameStatusWarranty(int Status)
        {
            string result = "";
            switch (Status)
            {
                case 1: result = "Đã tích điểm"; break;
                case 2: result = "Chưa tích điểm"; break;
            }
            return result;
        }

        public static string GenPass(string pass)
        {
            return BCrypt.Net.BCrypt.HashPassword(pass, 10);
        }

        public static bool CheckPass(string pass, string userPass)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(pass, userPass);
            }
            catch
            {
                return false;
            }
        }

        public static string getFullUrl()
        {
            if (HttpContext.Current == null)
                return "";
            string url = "http://" + HttpContext.Current.Request.Url.Authority;
            return url;
        }

        public static List<string> UploadFile(string fileName)
        {
            var httpRequest = HttpContext.Current.Request;
            var postedFile = httpRequest.Files.GetMultiple(fileName);
            //string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            List<string> lsImage = new List<string>();
            foreach (var file in postedFile)
            {
                string name = DateTime.Now.ToString("ssddMMyyyy") + file.FileName;                
                var filePath = HttpContext.Current.Server.MapPath(@"\Uploads\" + name);
                string urlFile = "/Uploads/" + name;
                lsImage.Add(urlFile);
                file.SaveAs(filePath);
            }
            return lsImage;
        }

        public static void RemoveFile(List<string> listUrl)
        {
            string rootFolder = HttpContext.Current.Server.MapPath(@"\Uploads\");
            foreach (string file in listUrl)
            {
                string fileName = rootFolder + file;
                if (file.Equals(fileName))
                {
                    File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }
            }
        }

        public static int? ParseInt(string input, int? value = null)
        {
            try
            {
                return int.Parse(input);
            }
            catch
            {

                return value;
            }
        }
        public static float? ParseFloat(string input, float? value = null)
        {
            try
            {
                return float.Parse(input);
            }
            catch
            {

                return value;
            }
        }
        public static double? ParseDouble(string input, double? value = null)
        {
            try
            {
                return double.Parse(input);
            }
            catch
            {

                return value;
            }
        }

    }

}
