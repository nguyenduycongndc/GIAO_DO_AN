using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class WasherRequestOutputModel
    {
        public int WasherID { get; set; }
        public int MemberID { get; set; }
        public double Distance { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifyDateStr
        {
            get
            {
                return ModifyDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
        }
        public int Balance { get; set; }
        public int InHouse { get; set; }
        public int CountOrderService { get; set; }
        public int CountHaveOrder { get; set; }
        public string Lang { get; set; }
        public string DeviceID { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
