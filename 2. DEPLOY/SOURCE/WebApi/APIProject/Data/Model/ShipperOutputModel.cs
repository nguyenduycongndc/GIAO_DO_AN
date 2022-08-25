using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DB;
using Data.Utils;

namespace Data.Model
{
    public class ShipperOutputModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AvartarUrl { get; set; }
        public double Rating { get; set; }
        public int IsVip { get; set; }
        public bool IsAcceptService { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public double RatingAdmin { get; set; }
        public int CommissionID { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public double Longi { get; set; }
        public double Lati { get; set; }
        public string Identification { get; set; }
        public string GetStringCreateDate
        {
            get
            {
                return CreatedDate.ToString(SystemParam.CONVERT_DATETIME) ;
            }
        }
    }
}
