using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class TimeLineOutputModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public string Avatar { get; set; }
        public List<task> listTask { get; set; }
    }

    public class task
    {
        public string time { get; set; }
        public int isTask { get { return taskDetail != null ? 1 : 0; } }
        public TaskDetail taskDetail { get; set; }
    }
    public class TaskDetail
    {
        public string BookingDate { get; set; }
        public string ESTBookingDate { get; set; }
        public int serviceID { get; set; }
        public string serviceName { get; set; }
        public string color { get; set; }
    }
    public class OrderTaskDetail
    {
        public int ID { get; set; }
        public int? AgentID { get; set; }
        public string AgentName { get; set; }
        public string ServiceName { get; set; }
        public string CustomerName { get; set; }
        public string PhoneCustomer { get; set; }
        public string Address { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingDateStr { get { return BookingDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public DateTime create_date { get; set; }
        public List<string> listAdditionService { get; set; }

        public int isMonthlyCombo { get; set; }
        public int Status { get; set; }
        public string StatusStr
        {
            get
            {
                if (Status == 0)
                {
                    return "Cancel";
                }
                else if (Status == 1 || Status == 9)
                {
                    return "Waiting";
                }
                else if (Status == 2)
                {
                    return "Confirmed";
                }
                else if (Status == 3)
                {
                    return "Complete";
                }
                else if (Status == 4)
                {
                    return "No confirm";
                }
                else if (Status == 5 || Status == 6)
                {
                    return "Washing";
                }
                else
                {
                    return "";
                }
            }
        }
    }


}
