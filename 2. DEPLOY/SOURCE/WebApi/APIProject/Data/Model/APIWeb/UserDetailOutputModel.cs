using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class UserDetailOutputModel
    {
        public int UserID { get; set; }
        public int Role { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? CreateDate { set; get; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public DateTime? Dob { get; set; }
        public string DobSTR
        {
            get
            {
                return Dob.HasValue ? Dob.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string Identity { get; set; }
        public int IsActive { get; set; }
        public int? sex { get; set; }
    }
}
