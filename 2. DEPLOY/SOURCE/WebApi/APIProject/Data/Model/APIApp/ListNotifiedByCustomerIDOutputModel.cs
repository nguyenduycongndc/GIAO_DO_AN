using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIApp
{
    public class NotifiedByCustomerOutput
    {
        public int limit { get; set; }
        public int totalCount { get; set; }
        public int page { get; set; }
        public int lastPage { get; set; }
        public int viewed { get; set; }
        public IPagedList<NotifiedByCustomerIDOutputModel> listNoti { get; set; }
    }
    public class NotifiedByCustomerIDOutputModel
    {
        public int NotifyID { set; get; }
        public string Content { set; get; }
        public int Viewed { set; get; }
        public int Type { get; set; }
        public DateTime? CreatedDate { set; get; }
        public string CreatedDateStr
        {
            set { }
            get
            {
                return CreatedDate.HasValue ? CreatedDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
        public string Title { get; set; }
        public string Uri { get; set; }
        public int? OrderServiceID { get; set; }
        public int? newsID { get; set; }
        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_DRIVER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_DRIVER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_PACKAGE:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_PACKAGE_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_FOOD:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_FOOD_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_DRIVER_VNPAY:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_DRIVER_VNPAY_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_PACKAGE_VNPAY:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_PACKAGE_VNPAY_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_FOOD_VNPAY:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_FOOD_VNPAY_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOK_DRIVER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOK_DRIVER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_INTERNAL:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_INTERNAL_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_BOOK_FOOD:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_BOOK_FOOD_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_DRIVER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_DRIVER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_PACKAGE:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_PACKAGE_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_FOOD:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_FOOD_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER_SHIPER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER_SHIPER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE_SHIPER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE_SHIPER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD_SHIPER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD_SHIPER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_DRIVER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_DRIVER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_PACKAGE:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_PACKAGE_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_FOOD:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_FOOD_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_MINUS_MONEY_DRIVER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_MINUS_MONEY_DRIVER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_UP_RANK:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_UP_RANK_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_MONEY_SHIPPER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_MONEY_SHIPPER_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_WALLET_NO_WITHDRAW_BY_ADMIN:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_RECHARGE_WALLET_NO_WITHDRAW_BY_ADMIN_ICON;

                    case SystemParam.NOTI_TYPE_REJECT_WITHDRAW:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_REJECT_WITHDRAW_ICON;

                    case SystemParam.NOTI_TYPE_WITH_DRAW_REQUEST:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_WITH_DRAW_REQUEST_ICON;

                    case SystemParam.NOTI_TYPE_NAVIGATE_EARN_POINT_CUSTOMER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_NAVIGATE_EARN_POINT_CUSTOMER_ICON;

                    case SystemParam.NOTI_TYPE_CHANGE_INTERNAL_STATUS_SHHIPER:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_CHANGE_INTERNAL_STATUS_SHHIPER_ICON;

                    case SystemParam.NOTI_TYPE_WITH_EARN_MONEY:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_WITH_EARN_MONEY_ICON;

                    case SystemParam.NOTI_TYPE_WITH_REFUND_MONEY:
                        return Util.getFullUrl() + SystemParam.NOTI_TYPE_WITH_REFUND_MONEY_ICON;
                    default:
                        return Util.getFullUrl() + "/Content/images/logo-weship.png";
                }
            }
        }
    }
}