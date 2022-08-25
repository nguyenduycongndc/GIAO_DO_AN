using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CreateAgentInputModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int sex { get; set; }
        public int IsInHouse { get; set; }
        public string Dob { get; set; }
        public DateTime? DateOb { get {
                try
                {
                    return DateTime.ParseExact(Dob, SystemParam.CONVERT_DATETIME, null);
                }
                catch {
                    return null;
                }
            } }
        public string identification { get; set; }
        public int? CommissionID { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> IdentityImage { get; set; }
        public List<int> listArea { get; set; }
        public int? Status { get; set; }
        public string Code { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? AcceptService { get; set; }
        public string Address { get; set; }
        public double? Rating { get; set; }
        public string CommisstitonName { get; set; }
        public string MastersBenefit { get; set; }
        public int Cash { get; set; }
        public  int Deposit { get; set; }
        public List<AreaOutputModel> ListAreaPriority { get; set; }
        public List<BankAgentViewModel> ListBankAgent { get; set; }
        public DateTime? DobDetail { get; set; }
        public int MemberId { get; set; }
        public string Lang { get; set; }
        public string AcceptServiceStr
        {
            get
            {
                if (AcceptService == 1)
                {
                    return "Accept";
                }
                else
                {
                    return "Ignore";
                }
            }
        }
        public string StatusStr
        {
            get
            {
                if (Status == 1)
                {
                    return "Active";
                }
                else if (Status == 2)
                {
                    return "Deactive";
                }
                else if (Status == 0)
                {
                    return "Inactive";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
