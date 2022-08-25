using Data.DB;
using Data.Model;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class StatisticBusiness : GenericBusiness
    {
        public StatisticBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        OrderServiceBusiness orderBus = new OrderServiceBusiness();
        public double Revenue(int? agentId, string FromDate, string Todate)
        {
            DateTime? fd = Util.ConvertDate(FromDate);
            DateTime? td = Util.ConvertDate(Todate);

            var query = cnn.OrderServices.Where(x => x.Status == Constant.ORDER_STATUS_COMPLETE && x.IsActive == SystemParam.ACTIVE).ToList();
            if (agentId != null)
            {
                query = query.Where(x => x.AgentID == agentId).ToList();
            }
            if (FromDate != null && FromDate != "")
            {
                query = query.Where(x => x.CreatedDate >= fd.Value).ToList();
            }
            if (Todate != null && Todate != "")
            {
                query = query.Where(x => x.CreatedDate <= td.Value.AddDays(1)).ToList();
            }
            if (query != null)
            {
                return query.Sum(x => x.TotalPrice);
            }
            else
            {
                return 0;
            }


        }

        public int RevenuePoint(int? agentID, string FromDate, string Todate)
        {
            DateTime? fd = Util.ConvertDate(FromDate);
            DateTime? td = Util.ConvertDate(Todate);
            var query = cnn.OrderServices.Where(x => x.Status == 2 && x.IsActive == SystemParam.ACTIVE).ToList();
            //var sum = cnn.Orders.Where(x => x.Status == 2 && x.IsActive == SystemParam.ACTIVE).Select(x=>x.TotalPrice).Sum();
            if (agentID != null)
            {
                query = query.Where(x => x.AgentID == agentID).ToList();
            }
            if (FromDate != null && FromDate != "")
            {
                query = query.Where(x => x.CreatedDate >= fd.Value).ToList();
            }
            if (Todate != null && Todate != "")
            {
                query = query.Where(x => x.CreatedDate <= td.Value.AddDays(1)).ToList();
            }
            if (query != null)
            {
                return query.Sum(x => x.TotalPrice);
            }
            else
            {
                return 0;
            }
        }

        public List<StatisticRevenueOutputModel> Search(int Page, int? AgentID, string FromDate, string ToDate)
        {
            try
            {
                List<StatisticRevenueOutputModel> list = new List<StatisticRevenueOutputModel>();
                var Customer = cnn.Customers;
                var query = from oi in cnn.OrderServiceDetails
                            where oi.OrderService.IsActive == SystemParam.ACTIVE && oi.OrderService.Status == Constant.ORDER_STATUS_COMPLETE
                            select new StatisticRevenueOutputModel
                            {
                                customer = oi.OrderService.Customer,
                                orderItem = oi.OrderService,
                                orderItemDetail = oi,
                                AgentName = oi.OrderService.Agent.Name,
                                CustomerName = oi.OrderService.Customer.Name,
                                Revenue = oi.OrderService.TotalPrice
                            };
                if (AgentID != null)
                {
                    query = query.Where(x => x.orderItem.AgentID == AgentID);
                }
                if (FromDate != "" && FromDate != null)
                {
                    DateTime? fd = Util.ConvertDate(FromDate); ;
                    query = query.Where(x => x.orderItem.CreatedDate >= fd);
                }
                if (ToDate != "" && ToDate != null)
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(x => x.orderItem.CreatedDate <= td);
                }
                if (query != null && query.Count() > 0)
                {
                    return query.OrderByDescending(x => x.orderItem.CreatedDate).ToList();
                }
                else
                {
                    return new List<StatisticRevenueOutputModel>();
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<StatisticRevenueOutputModel>();
            }
        }

        public List<Agent> getListAgent()
        {
            try
            {
                var query = from c in cnn.Agents
                            where c.IsActive.Equals(SystemParam.ACTIVE)
                            select c;
                return query.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<Agent>();
            }
        }

        public SystemResult GetOverView()
        {
            OverViewModel query = new OverViewModel();
            var member = cnn.Members.Where(u => !u.IsActive.Equals(SystemParam.INACTIVE) && (u.CustomerID.HasValue ? u.IsLogin.HasValue : true)).ToList();
            query.Customer = member.Where(u => u.CustomerID.HasValue).ToList().Count;
            query.Washer = member.Where(u => u.AgentID.HasValue && !u.Agent.IsActive.Equals(SystemParam.INACTIVE)).ToList().Count;
            query.Request = cnn.MembersTransactionHistories.Where(u => u.Type.Equals(Constant.TYPE_TRANSACTION_WITHDRAW) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList().Count;
            query.Transaction = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList().Count;
            return resultBus.SucessResult(query);
        }

        public SystemResult ChartByCreateDate(string fDate, string tDate, int type)
        {
            ChartModel query = new ChartModel();
            DateTime? fromDate = Util.ConvertDate(fDate);
            DateTime? toDate = Util.ConvertDate(tDate);
            int ditanceDate = Distance(fromDate, toDate);
            if (!fromDate.HasValue)
                fromDate = DateTime.Now;
            if (ditanceDate == 1)
                query = ChartInDay(fromDate.Value, type);
            else
                query = ChartIntDate(fromDate.Value, toDate.Value, type, ditanceDate);
            return resultBus.SucessResult(query);
        }
        private int Distance(DateTime? fromDate, DateTime? toDate)
        {
            if (toDate.HasValue)
                toDate = toDate.Value.AddDays(1);
            int ditanceDate = 1;
            if (fromDate.HasValue && toDate.HasValue)
                ditanceDate = (toDate.Value - fromDate.Value).Days;
            if (ditanceDate <= 0)
                ditanceDate = 1;
            return ditanceDate;
        }
        public ChartModel ChartInDay(DateTime date, int type)
        {
            var ListOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            ListOrder = ListOrder.Where(u => type.Equals(1) ? u.CreatedDate.Date.Equals(date.Date) : u.BookingDate.Value.Date.Equals(date.Date)).ToList();
            ChartModel query = new ChartModel
            {
                Max_H_axis = 23,
                Max_V_axis = ListOrder.Count(),
                List_H_axis = new List<string>(),
                List_V_axis = new List<int>(),
            };
            for (int index = 0; index <= 23; index++)
            {
                query.List_H_axis.Add(
                  index.ToString() + ":00"
                );
                int count = 0;
                switch (type)
                {
                    case 1:
                        count = ListOrder.Where(u => u.CreatedDate >= date.AddHours(index) && u.CreatedDate < date.AddHours(index + 1)).Count();
                        break;
                    case 2:
                        count = ListOrder.Where(u => u.BookingDate.Value >= date.AddHours(index) && u.BookingDate.Value < date.AddHours(index + 1)).Count();
                        break;
                    case 3:
                        count = ListOrder.Where(u => u.Status.Equals(3) && u.CompletedDate.HasValue && u.CompletedDate.Value >= date.AddHours(index) && u.CompletedDate.Value < date.AddHours(index + 1)).Count();
                        break;
                }
                query.List_V_axis.Add(count);
            }
            return query;
        }
        public ChartModel ChartIntDate(DateTime fromDate, DateTime toDate, int type, int distance)
        {
            var ListOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            ChartModel query = new ChartModel
            {
                Max_H_axis = distance,
                Max_V_axis = 0,
                List_H_axis = new List<string>(),
                List_V_axis = new List<int>(),
            };

            for (int index = 0; index < distance; index++)
            {
                string date = fromDate.AddDays(index).ToString("dd/MM");
                query.List_H_axis.Add(date);
                int count = 0;
                switch (type)
                {
                    case 1:
                        count = ListOrder.Where(u => u.CreatedDate >= fromDate.AddDays(index) && u.CreatedDate < fromDate.AddDays(index + 1)).Count();
                        break;
                    case 2:
                        count = ListOrder.Where(u => u.BookingDate.Value >= fromDate.AddDays(index) && u.BookingDate.Value < fromDate.AddDays(index + 1)).Count();
                        break;
                    case 3:
                        count = ListOrder.Where(u => u.Status.Equals(3) && u.CompletedDate.HasValue && u.CompletedDate.Value >= fromDate.AddDays(index) && u.CompletedDate.Value < fromDate.AddDays(index + 1)).Count();
                        break;
                }
                query.List_V_axis.Add(count);
            }
            query.Max_V_axis = query.List_V_axis.Max(u => u);
            return query;
        }

        public SystemResult ChartServiceByCreateDate(string fDate, string tDate)
        {
            ChartServiceModel query = new ChartServiceModel();
            DateTime? fromDate = Util.ConvertDate(fDate);
            DateTime? toDate = Util.ConvertDate(tDate);
            int ditanceDate = Distance(fromDate, toDate);
            if (!fromDate.HasValue)
                fromDate = DateTime.Now;
            if (ditanceDate == 1)
                query = ChartServiceDate(fromDate.Value);
            else
                query = ChartServiceDays(fromDate.Value, toDate.Value, ditanceDate);
            return resultBus.SucessResult(query);
        }
        public ChartServiceModel ChartServiceDays(DateTime fromDate, DateTime toDate, int distance)
        {
            var ListOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            ChartServiceModel query = new ChartServiceModel
            {
                Max_H_axis = distance,
                Max_V_axis = 0,
                List_V_axis_combo = new List<int>(),
                List_V_axis_diamon = new List<int>(),
                List_V_axis_gold = new List<int>(),
                List_V_axis_sliver = new List<int>(),
                List_H_axis = new List<string>(),
            };

            for (int index = 0; index < distance; index++)
            {
                string date = fromDate.AddDays(index).ToString("dd/MM");
                var listService = ListOrder.Where(u => u.CreatedDate >= fromDate.AddDays(index) && u.CreatedDate < fromDate.AddDays(index + 1)).ToList();
                query.List_H_axis.Add(date);
                int count_diamon = listService.Where(u => !u.ServiceComboSegmentID.HasValue && u.OrderServiceDetails.Where(s => s.ServicePrice.ServiceID.Value.Equals(1)).Count() > 0).Count();
                int count_gold = listService.Where(u => !u.ServiceComboSegmentID.HasValue && u.OrderServiceDetails.Where(s => s.ServicePrice.ServiceID.Value.Equals(2)).Count() > 0).Count();
                int count_sliver = listService.Where(u => !u.ServiceComboSegmentID.HasValue && u.OrderServiceDetails.Where(s => s.ServicePrice.ServiceID.Value.Equals(3)).Count() > 0).Count();
                int count_combo = listService.Where(u => u.ServiceComboSegmentID.HasValue).GroupBy(u => u.CodeCombo).Count();
                query.List_V_axis_gold.Add(count_gold);
                query.List_V_axis_diamon.Add(count_diamon);
                query.List_V_axis_sliver.Add(count_sliver);
                query.List_V_axis_combo.Add(count_combo);
            }
            query.Max_V_axis = query.List_V_axis_gold.Max(u => u);
            return query;
        }
        public ChartServiceModel ChartServiceDate(DateTime date)
        {
            var ListOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            ChartServiceModel query = new ChartServiceModel
            {
                Max_H_axis = 23,
                Max_V_axis = 0,
                List_V_axis_combo = new List<int>(),
                List_V_axis_diamon = new List<int>(),
                List_V_axis_gold = new List<int>(),
                List_V_axis_sliver = new List<int>(),
                List_H_axis = new List<string>(),
            };
            for (int index = 0; index <= 23; index++)
            {
                query.List_H_axis.Add(
                  index.ToString() + ":00"
                );
                var listService = ListOrder.Where(u => u.CreatedDate >= date.AddHours(index) && u.CreatedDate < date.AddHours(index + 1)).ToList();
                int count_diamon = listService.Where(u => !u.ServiceComboSegmentID.HasValue && u.OrderServiceDetails.Where(s => s.ServicePrice.ServiceID.Value.Equals(1)).Count() > 0).Count();
                int count_gold = listService.Where(u => !u.ServiceComboSegmentID.HasValue && u.OrderServiceDetails.Where(s => s.ServicePrice.ServiceID.Value.Equals(2)).Count() > 0).Count();
                int count_sliver = listService.Where(u => !u.ServiceComboSegmentID.HasValue && u.OrderServiceDetails.Where(s => s.ServicePrice.ServiceID.Value.Equals(3)).Count() > 0).Count();
                int count_combo = listService.Where(u => u.ServiceComboSegmentID.HasValue).GroupBy(u => u.CodeCombo).Count();
                query.List_V_axis_gold.Add(count_gold);
                query.List_V_axis_diamon.Add(count_diamon);
                query.List_V_axis_sliver.Add(count_sliver);
                query.List_V_axis_combo.Add(count_combo);
            }
            return query;
        }

        public ChartComboServer getChartCombo(int month, int year)
        {
            string monthStr = month < 10 ? "0" + month.ToString() : month.ToString();
            string daystr = "01/" + monthStr + "/" + year.ToString();
            DateTime daychart = Util.ConvertDate(daystr).Value;
            DateTime todate = daychart.AddMonths(1);
            var ListOrder = cnn.OrderServices.Where(u => !u.Status.Equals(0) && u.IsActive.Equals(SystemParam.ACTIVE) && u.BookingDate.Value >= daychart && u.BookingDate.Value < todate && u.ServiceComboSegmentID.HasValue).ToList();
            var query = new ChartComboServer();
            var listdiamond = new List<int>();
            var listsliver = new List<int>();
            for (int i = 1; i <= 4; i++)
            {
                listdiamond.Add(ListOrder.Where(u => u.OrderServiceDetails.Where(d => d.ServicePrice.ServiceID.Equals(1)).Count() > 0 && u.BookingDate.Value >= daychart.AddDays((7 * (i - 1)) + 1) && u.BookingDate.Value < daychart.AddDays((7 * i) + 1)).Count());
                listsliver.Add(ListOrder.Where(u => u.OrderServiceDetails.Where(d => d.ServicePrice.ServiceID.Equals(3)).Count() > 0 && u.BookingDate.Value >= daychart.AddDays((7 * (i - 1)) + 1) && u.BookingDate.Value < daychart.AddDays((7 * i) + 1)).Count());
            }
            query.diamon = listdiamond;
            query.sliver = listsliver;
            return query;
        }
        public List<TaskDetail> getListServerTimeLine()
        {

            var listService = cnn.Services.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.TYPE_MAIN_SERVICE)).ToList();
            return listService.Select(u => new TaskDetail
            {
                serviceName = u.NameEN,
                color = String.IsNullOrEmpty(u.Color) ? "#009e0f" : u.Color
            }).ToList();
        }
        public List<TimeLineOutputModel> getTimeLine(string DateTime, int? provinceCode = null)
        {
            DateTime time = Util.ConvertDate(DateTime).Value;
            DateTime fromDate = time.AddDays(1);
            List<DateTime> listTime = new List<DateTime>();
            foreach (string timestr in Constant.ListTime)
            {
                listTime.Add(ConvertDateTime(timestr).Value);
            }
            List<Day> lsday = cnn.Days.ToList();
            List<TimeLineOutputModel> query = new List<TimeLineOutputModel>();
            var lsWasher = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.AgentID.HasValue).ToList();
            var lsOrder = cnn.OrderServices.Where(u => !u.Status.Equals(0) && u.IsActive.Equals(SystemParam.ACTIVE) && u.AgentID.HasValue && u.BookingDate.Value >= time && u.BookingDate.Value <= fromDate).ToList();
            List<int> listAreaID = cnn.Areas.Where(u => provinceCode.HasValue ? u.District.ProvinceCode.Equals(provinceCode.Value) : false).Select(u => u.ID).ToList();
            query = lsWasher.Select(u => new TimeLineOutputModel
            {
                Name = u.Agent.Name,
                Phone = u.User,
                Avatar = u.Agent.AvatarUrl,
                Code = u.Agent.Code,
                listTask = listTime.Select(t => new task
                {
                    taskDetail = lsOrder.Where(s => 
                    s.AgentID.Value.Equals(u.AgentID.Value) 
                    && s.BookingDate.Value.TimeOfDay < t.AddHours(1).TimeOfDay 
                    && Util.ConvertDateEST(s.EstBookingDate.Value, lsday).TimeOfDay >= t.TimeOfDay 
                    && (provinceCode.HasValue ? listAreaID.Contains(s.AreaID.Value) : true)).Select(s => new TaskDetail
                    {
                        BookingDate = s.BookingDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR),
                        ESTBookingDate = s.EstBookingDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR),
                        serviceID = s.OrderServiceDetails.Where(i => i.ServicePrice.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(i => i.ServicePrice.ServiceID.Value).FirstOrDefault(),
                        serviceName = s.OrderServiceDetails.Where(i => i.ServicePrice.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(i => i.ServicePrice.Service.NameEN).FirstOrDefault(),
                        color = s.OrderServiceDetails.Where(i => i.ServicePrice.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(i => String.IsNullOrEmpty(i.ServicePrice.Service.Color) || i.ServicePrice.Service.Color.Length == 0 ? "#009e0f" : i.ServicePrice.Service.Color).FirstOrDefault(),
                    }).FirstOrDefault(),
                    time = t.ToString("HH:mm")
                }).ToList()
            }).ToList();
            return query.ToList();
            //return query.Where(u => u.listTask.Where(t => t.taskDetail != null).Count() > 0).ToList();
        }

        public static DateTime? ConvertDateTime(string date)
        {
            return DateTime.ParseExact(date, "HH:mm", null);
        }

        public List<OrderTaskDetail> GetListOrder(int? status = null, int? provinceCode = null)
        {
            DateTime time = DateTime.Today;
            DateTime fromDate = time.AddDays(1);
            List<int> listAreaID = cnn.Areas.Where(u => provinceCode.HasValue ? u.District.ProvinceCode.Equals(provinceCode.Value) : false).Select(u => u.ID).ToList();
            List<OrderTaskDetail> query = new List<OrderTaskDetail>();
            query = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.BookingDate.Value >= time && u.BookingDate.Value <= fromDate && (provinceCode.HasValue ? listAreaID.Contains(u.AreaID.Value) : true)).Select(u => new OrderTaskDetail
            {
                ID = u.ID,
                Address = u.Address,
                AgentName = u.AgentID.HasValue ? u.Agent.Name : "",
                AgentID = u.AgentID,
                BookingDate = u.BookingDate.Value,
                CustomerName = u.Customer.Name,
                PhoneCustomer = u.Customer.Phone,
                ServiceName = u.OrderServiceDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.ServicePrice.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(s => s.ServicePrice.Service.NameEN).FirstOrDefault(),
                isMonthlyCombo = u.ServiceComboSegmentID.HasValue ? 1 : 0,
                listAdditionService = u.OrderServiceDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.ServicePrice.Service.Type.Equals(Constant.TYPE_ADDITION_SERVICE)).Select(s => s.ServicePrice.Service.NameEN).ToList(),
                Status = u.Status,
                create_date = u.CreatedDate

            }).ToList();
            if (status.HasValue)
            {
                int[] waiting = { Constant.ORDER_STATUS_WAITING, Constant.ORDER_STATUS_FIND_ORTHER_WASHER };
                int[] washing = { Constant.ORDER_STATUS_WASHING, Constant.ORDER_STATUS_CONFIRM_WASHING };
                switch (status.Value)
                {
                    case Constant.ORDER_STATUS_WAITING:
                        {
                            query = query.Where(u => waiting.ToList().Contains(u.Status)).ToList();
                            break;
                        }
                    case Constant.ORDER_STATUS_CONFIRM:
                        {
                            query = query.Where(u => u.Status.Equals(status.Value)).ToList();
                            break;
                        }
                    case Constant.ORDER_STATUS_WASHING:
                        {
                            query = query.Where(u => washing.ToList().Contains(u.Status)).ToList();
                            break;
                        }
                    case Constant.ORDER_STATUS_COMPLETE:
                        {
                            query = query.Where(u => u.Status.Equals(status.Value)).ToList();
                            break;
                        }
                    case Constant.ORDER_STATUS_CANCEL:
                        {
                            query = query.Where(u => u.Status.Equals(status.Value)).ToList();
                            break;
                        }
                    case Constant.ORDER_STATUS_NO_CONFIRM:
                        {
                            query = query.Where(u => u.Status.Equals(status.Value)).ToList();
                            break;
                        }
                }
            }
            return query.OrderByDescending(u => u.create_date).ToList();
        }

        public SystemResult UpdateOrder(int orderID, int agentID, string time)
        {
            DateTime bookingDate = DateTime.Now;
            try
            {
                bookingDate = DateTime.ParseExact(time, SystemParam.CONVERT_DATETIME_HAVE_HOUR, null);
            }
            catch
            {
                return resultBus.ErrorResult(MessEN.FALSE_CONVERT_DATETIME);
            }
            if (bookingDate < DateTime.Now)
            {
                return resultBus.ErrorResult("Wrong select booking date");
            }
            int[] washing = { Constant.ORDER_STATUS_CONFIRM, Constant.ORDER_STATUS_CONFIRM_WASHING, Constant.ORDER_STATUS_WASHING };
            List<DB.Day> lsDay = cnn.Days.Select(u => u).ToList();
            string date = bookingDate.DayOfWeek.ToString();
            var Days = lsDay.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Day1.Equals(date)).Select(u => u).FirstOrDefault();
            var shift = Days.ClassShift.Shifts.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ClassShiftID.Value.Equals(Days.ClassShiftID)).OrderBy(u => u.ID).Select(u => u.shift1).ToList();
            if (shift.Count == 0)
                return resultBus.ErrorResult(MessEN.SHIFT_ERROR);
            OrderService order = cnn.OrderServices.Find(orderID);
            DateTime estTime = bookingDate.AddMinutes(order.TotalEstTime.Value);
            var lsItemAwaiting = cnn.OrderServices.Where(u =>
            !u.ID.Equals(order.ID)
            && (!u.Status.Equals(Constant.ORDER_STATUS_COMPLETE)
            && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
            && u.IsActive.Equals(SystemParam.ACTIVE)
            && u.AgentID.Value.Equals(agentID)
            && ((u.BookingDate.Value <= bookingDate && u.EstBookingDate.Value > bookingDate) || (u.BookingDate.Value < estTime && u.EstBookingDate.Value >= estTime))).ToList();
            if (lsItemAwaiting.Count > 0)
            {
                return resultBus.ErrorResult(MessEN.HAVE_SHCEDULE);
            }
            order.BookingDate = bookingDate;
            order.EstBookingDate = estTime;
            SystemResult checkChangeWasher = orderBus.ChangeWasher(orderID, agentID, bookingDate, estTime);
            if (checkChangeWasher.Status.Equals(1))
            {
                cnn.SaveChanges();
            }
            return checkChangeWasher;
        }
    }
}
