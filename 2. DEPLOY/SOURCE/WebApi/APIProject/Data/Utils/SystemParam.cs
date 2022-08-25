using Data.DB;
using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Utils
{
    public class SystemParam
    {
        //WE_SHIPEntities cnns = new WE_SHIPEntities();
        //public const string vnp_Return_url = "http://api.carrect.vn/VnPay/Index";
        //public const string vnpay_api_url = "http://api.carrect.vn/api/Service/vnp_ipn";
        //public const string vnp_Return_url = "http://winds.hopto.org/VnPay/Index";
        //public const string vnpay_api_url = "http://winds.hopto.org/api/Service/vnp_ipn";
        public const string vnp_Return_url = "http://weship.winds.vn/VnPay/Index";
        public const string vnp_Return_url_Real = "http://weshipvn.com/VnPay/Index";
        public const string vnpay_api_url = "http://weship.winds.vn/api/Payment/vnp_ipn";
        public const string vnpay_api_url_Real = "http://weshipvn.com/api/Payment/vnp_ipn";
        //public const string vnp_Return_url = "http://localhost:11111/VnPay/Index";
        //public const string vnpay_api_url = "http://localhost:11111/api/Service/vnp_ipn";
        public const string DSN_SENTRY = "https://26b64b238a274d22911b8ae3d43837bd@o507139.ingest.sentry.io/5597821";
        public const string URL_WEB_SOCKET = "http://54.251.116.128:8100/socketio";
        public const int MAX_RETRY_NOTI = 3;
        public const string driver_success = "weshipdriver://success/";
        public const string customer_success = "weshipcustomer://success/";
        public const string driver_failed = "weshipdriver://failed/";
        public const string customer_failed = "weshipcustomer://failed/";


        public const string vnp_Return_Rawurl = "/api/Service/vnp_return";
        public const string vnpay_api_Rawurl = "/api/Service/vnp_ipn";
        public const string GOOGLE_MAP_API = "https://maps.googleapis.com/maps/api/directions/json";
        public const string GOOGLE_MAP_DETAIL_API = "https://maps.googleapis.com/maps/api/place/details/json";
        //public const string GOOGLE_MAP_Key = "AIzaSyCmeRtaN1l1mhfL6l7ExCjmi1eVzdh58m4";
        public const string GOOGLE_MAP_Key = "AIzaSyAWWFzb1uMxLnm_DaD0ZVfZXrBzeEfrJTM";
        public const string GOOGLE_MAP_Mode = "driving";

        public const string vnp_Url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string vnp_TmnCode = "WESHIP01";
        public const string vnp_HashSecret = "LRKPTTMKJNEKREDJVXCKGGRXYTJBPVPA";

        public const string vnp_Url_Real = "http://pay.vnpay.vn/vpcpay.html";
        public const string vnp_TmnCode_Real = "WESHIP01";
        public const string vnp_HashSecret_Real = "LCMJGDESOPUTQKWSHOQAEVSQSEAWNOFA";

        //public const string vnp_Url_Real = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        //public const string vnp_TmnCode_Real = "WESHIP02";
        //public const string vnp_HashSecret_Real = "EM2PZWOPTPUNO88ZLV2U11M75BGBAC0W";
        //public const string vnp_Url = "https://pay.vnpay.vn/vpcpay.html";
        //public const string vnp_TmnCode = "EGLOWW01";
        //public const string vnp_HashSecret = "QHJOBDZLODAUJLBDDPILYQODHFERPKUQ";

        //Firebase Config
        public const string FIREBASE_SECRET = "U0TQQrocdxzY5kl2Clj11os9SrLGBcV67hXVzVLF";
        public const string FIREBASE_BASEPATH = "https://weship-e93cc-default-rtdb.firebaseio.com/";
        public const string FIREBASE_SECRET_TEST = "j0PfGsGEkkgGEDDJpRJeZ3YdYF1vykkfwzuWFUtC";
        public const string FIREBASE_BASEPATH_TEST = "https://weship-test-9c0fa-default-rtdb.firebaseio.com/";

        public const string vnp_CodeSucces = "00";

        public const double LONG_DEFAULT = 105.7930512;
        public const double LAT_DEFAULT = 20.9977337;
        public const string PASS_DEFAUL = "windsoft123456a@";
        public const string HOST_DEFAUL = "smtp.gmail.com";
        public const string EMAIL_CONFIG = "sport.weship@gmail.com";
        public const string PASS_CONFIG = "jbgceknqdnugwnjp";
        public const string PASS_EMAIL = "widnsoftemailtest@gmail.com";
        public const string EMAIL_HEADER = "[HỆ THỐNG WE SHIP]";
        public const string SESSION_LOGIN = "Login";
        public const string ERROR_MESSAGE_LOGIN_FAIL = "Tài khoản hoặc mật khẩu không đúng";
        public const string ERROR_MESSAGE_CHECK_PASS_FAIL = "Mật khẩu cũ không chính xác";
        public const int FEMALE = 0;//Nữ
        public const int MALE = 1;//Nam
        //Config Token 
        public const string TOKEN_INVALID = "Token invalid";
        public const string TOKEN_NOT_FOUND = "Token not found";

        // Config One Signal   
        // Thông báo
        public const string WE_SHIP_NOTI = "WE_SHIP thông báo";

        // phân biệt 2 app nên cần 2 push onsinal
        public const string URL_ONESIGNAL = "https://onesignal.com/api/v1/notifications";
        public const string URL_BASE_https = "Basic ://onesignal.com/api/v1/notifications";
        // app khách hàng
        public const string APP_ID = "aedac8f4-5983-4797-9488-36e0a87e7510";
        public const string Authorization = "Basic :NDUwMjhlMmUtMjAyMy00YjZjLWI5M2EtZGM0ZjZkNDExOTMz";
        public const string ANDROID_CHANNEL_ID = "9186a90d-ee1f-48c9-abf1-8bba4add8458";
        // app Tài Xế
        public const string APP_DRIVER_ID = "bd112c67-f9fd-49a6-af1e-4cb78a2ed7fe";
        public const string Authorization_driver = "Basic :ZWJiODY3OGUtYjM4NS00NGU2LTkwMzEtNjIwMDgyNTg4ZmMz";
        public const string ANDROID_CHANNEL_ID_DRIVER = "3b054be1-7110-4ce3-9d29-63f013fb5fd8";

        // app Tài xế có tiếng
        public const string ANDROID_CHANNEL_ID_DRIVER_SOUND = "3b054be1-7110-4ce3-9d29-63f013fb5fd8";
        public const string COUNTDOWN_DRIVER_SOUND = "countdown.wav";


        // Đơn vị quãng đường 
        public const int Km = 1000;
        // Đơn vị thời gian
        public const int Minute = 60;
        // Config 
        public const string CONFIG_COUNTDOWN = "CountDown";
        public const string CONFIG_TIMECANCLEORDER = "TimeCancleOrder";
        public const string CONFIG_CANCLETIME = "CancleTime";
        public const string CONFIG_MAXCODFEE = "MaxCODFee";
        public const int COUNTDOWN = 60; // Thời gian đếm ngược (s)
        public const int TIME_CANCLE_ORDER = 900; // Tối đa số giây tìm kiếm
        public const int MAX_COD_FEE = 10000000; // Phí thu hộ tối đa
        public const int MAX_TRANSACTION_MONEY = 10000000; // Số tiền giao dịch tối đa
        public const int MIN_TRANSACTION_MONEY = 50000; // Số tiền giao dịch tối thiểu
        //Type push noti on web admin
        public const int TYPE_PUSH_ALL = 0;//gửi tất cả
        public const int TYPE_PUSH_TO_CUSTOMERS = 1;//gửi tới tất cả khách hàng
        public const int TYPE_PUSH_TO_SHOPS = 2;//gửi tới tất cả các shop
        public const int TYPE_PUSH_TO_SHIPERS = 3;//gửi tới tất cả shiper
        // config type navigate table noti

        public const int NOTI_TYPE_NAVIGATE_BOOK_DRIVER = 10; // APP Tài xế - Lấy thông tin đặt xe
        public const string NOTI_TYPE_NAVIGATE_BOOK_DRIVER_ICON = "/Uploads/icons/driver-finding.png"; 
        public const int NOTI_TYPE_NAVIGATE_BOOK_PACKAGE = 20; // APP Tài xế - Lấy thông tin giao hàng
        public const string NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_ICON = "/Uploads/icons/package-finding.png";
        public const int NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_INTERNAL = 27; // APP Tài xế - Lấy thông tin giao hàng
        public const string NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_INTERNAL_ICON = "/Uploads/icons/package-finding.png";
        public const int NOTI_TYPE_NAVIGATE_BOOK_FOOD = 30; // APP Tài xế - Lấy thông tin giao đồ ăn
        public const string NOTI_TYPE_NAVIGATE_BOOK_FOOD_ICON = "/Uploads/icons/food-finding.png";
        public const int NOTI_TYPE_NAVIGATE_ACCEPT_DRIVER = 11; // APP Khách hàng - Lấy thông tin đặt xe khi tiếp nhận cho khách hàng
        public const string NOTI_TYPE_NAVIGATE_ACCEPT_DRIVER_ICON = "/Uploads/icons/driver-finding.png";
        public const int NOTI_TYPE_NAVIGATE_ACCEPT_PACKAGE = 21; // APP Khách hàng - Lấy thông tin giao hàng khi tiếp nhận cho khách hàng
        public const string NOTI_TYPE_NAVIGATE_ACCEPT_PACKAGE_ICON = "/Uploads/icons/package-finding.png";
        public const int NOTI_TYPE_NAVIGATE_ACCEPT_FOOD = 31; // APP Khách hàng - Lấy thông tin giao đồ ăn khi tiếp nhận cho khách hàng
        public const string NOTI_TYPE_NAVIGATE_ACCEPT_FOOD_ICON = "/Uploads/icons/food-finding.png";
        public const int NOTI_TYPE_NAVIGATE_DENY_DRIVER = 12; // APP Khách hàng - chuyển qua màn đặt xe
        public const string NOTI_TYPE_NAVIGATE_DENY_DRIVER_ICON = "/Uploads/icons/driver-decline.png";
        public const int NOTI_TYPE_NAVIGATE_DENY_PACKAGE = 22; // APP Khách hàng - chuyển qua màn giao hàng
        public const string NOTI_TYPE_NAVIGATE_DENY_PACKAGE_ICON = "/Uploads/icons/package-decline.png";
        public const int NOTI_TYPE_NAVIGATE_DENY_FOOD = 32; // APP Khách hàng - chuyển qua màn giao đồ ăn
        public const string NOTI_TYPE_NAVIGATE_DENY_FOOD_ICON = "/Uploads/icons/food-decline.png";
        public const int NOTI_TYPE_NAVIGATE_DENY_DRIVER_SHIPER = 13; // APP Tài xế - chuyển qua màn hủy đơn đặt xe
        public const string NOTI_TYPE_NAVIGATE_DENY_DRIVER_SHIPER_ICON = "/Uploads/icons/driver-decline.png";
        public const int NOTI_TYPE_NAVIGATE_DENY_PACKAGE_SHIPER = 23; // APP Tài xế - chuyển qua màn hủy đơn giao hàng
        public const string NOTI_TYPE_NAVIGATE_DENY_PACKAGE_SHIPER_ICON = "/Uploads/icons/package-decline.png";
        public const int NOTI_TYPE_NAVIGATE_DENY_FOOD_SHIPER = 33; // APP Tài xế - chuyển qua màn hủy đơn giao đồ ăn
        public const string NOTI_TYPE_NAVIGATE_DENY_FOOD_SHIPER_ICON = "/Uploads/icons/food-decline.png";
        public const int NOTI_TYPE_NAVIGATE_FINISH_BOOKING_DRIVER = 14; // APP Khách hàng - chuyển qua màn hoàn thành đặt xe
        public const string NOTI_TYPE_NAVIGATE_FINISH_BOOKING_DRIVER_ICON = "/Uploads/icons/driver-complete.png";
        public const int NOTI_TYPE_NAVIGATE_FINISH_BOOKING_PACKAGE = 24; // APP Khách hàng - chuyển qua màn hoàn thành giao hàng
        public const string NOTI_TYPE_NAVIGATE_FINISH_BOOKING_PACKAGE_ICON = "/Uploads/icons/package-success.png";
        public const int NOTI_TYPE_NAVIGATE_FINISH_BOOKING_FOOD = 34; //APP Khách hàng - chuyển qua màn hoàn thành giao đồ ăn
        public const string NOTI_TYPE_NAVIGATE_FINISH_BOOKING_FOOD_ICON = "/Uploads/icons/food-success.png";
        public const int NOTI_TYPE_NAVIGATE_BOOKING_DRIVER_VNPAY = 15; // APP Khách hàng - chuyển qua màn tìm kiếm đặt xe khi thanh toán VNPAY
        public const string NOTI_TYPE_NAVIGATE_BOOKING_DRIVER_VNPAY_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_BOOKING_PACKAGE_VNPAY = 25; // APP Khách hàng - chuyển qua màn tìm kiếm giao hàng khi thanh toán VNPAY
        public const string NOTI_TYPE_NAVIGATE_BOOKING_PACKAGE_VNPAY_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_BOOKING_FOOD_VNPAY = 35; //APP Khách hàng - chuyển qua màn tìm kiếm giao đồ ăn khi thanh toán VNPAY
        public const string NOTI_TYPE_NAVIGATE_BOOKING_FOOD_VNPAY_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_CONFIRM_DRIVER = 16; // APP Khách hàng - Lấy thông tin đặt xe khi xác nhận cho khách hàng
        public const string NOTI_TYPE_NAVIGATE_CONFIRM_DRIVER_ICON = "/Uploads/icons/driver-confirm.png";
        public const int NOTI_TYPE_NAVIGATE_CONFIRM_PACKAGE = 26; // APP Khách hàng - Lấy thông tin giao hàng khi xác nhận cho khách hàng
        public const string NOTI_TYPE_NAVIGATE_CONFIRM_PACKAGE_ICON = "/Uploads/icons/package-confirm.png";
        public const int NOTI_TYPE_NAVIGATE_CONFIRM_FOOD = 36; //APP Khách hàng - Lấy thông tin giao đồ ăn khi xác nhận cho khách hàng
        public const string NOTI_TYPE_NAVIGATE_CONFIRM_FOOD_ICON = "/Uploads/icons/food-confirm.png";
        public const int NOTI_TYPE_NAVIGATE_MINUS_MONEY_DRIVER = 50; // APP Tài xế - Thu phí dịch vụ
        public const string NOTI_TYPE_NAVIGATE_MINUS_MONEY_DRIVER_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_UP_RANK = 51; // APP Khách hàng - Lên hạng
        public const string NOTI_TYPE_NAVIGATE_UP_RANK_ICON = "/Uploads/icons/up-rank.png";
        public const int NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER = 52; // APP Khách hàng - Hoàn tiền
        public const string NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_RECHARGE_MONEY_SHIPPER = 53; // APP tài xế - Nạp tiền ví cọc
        public const string NOTI_TYPE_NAVIGATE_RECHARGE_MONEY_SHIPPER_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_REJECT_WITHDRAW = 54;//Từ chối yêu cầu rút tiền
        public const string NOTI_TYPE_REJECT_WITHDRAW_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_RECHARGE_WALLET_NO_WITHDRAW_BY_ADMIN = 55;//Được nạp tiền từ admin
        public const string NOTI_TYPE_NAVIGATE_RECHARGE_WALLET_NO_WITHDRAW_BY_ADMIN_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_NAVIGATE_EARN_POINT_CUSTOMER= 56;// APP Khách hàng - điểm thưởng
        public const string NOTI_TYPE_NAVIGATE_EARN_POINT_CUSTOMER_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_WITH_DRAW_REQUEST = 57;//App tài xế - yêu cầu rút tiền
        public const string NOTI_TYPE_WITH_DRAW_REQUEST_ICON = "/Uploads/icons/money-fee.png";
        public const int NOTI_TYPE_CHANGE_INTERNAL_STATUS_SHHIPER = 58;
        public const string NOTI_TYPE_CHANGE_INTERNAL_STATUS_SHHIPER_ICON = "/Uploads/icons/change-driver.png";
        public const int NOTI_TYPE_WITH_EARN_MONEY = 59;//App tài xế - Thưởng tiền ví thu nhập
        public const string NOTI_TYPE_WITH_EARN_MONEY_ICON = "/Uploads/icons/money-fee.png";       
        public const int NOTI_TYPE_WITH_REFUND_MONEY = 60;//App tài xế - Hoàn tiền ví cọc
        public const string NOTI_TYPE_WITH_REFUND_MONEY_ICON = "/Uploads/icons/money-fee.png";


        public const int NOTY_BY_ADMIN = 40;//Type thông báo từ admin

        //Param configtransport area
        public const int IN_PROVINCE = 1;//Nội tỉnh
        public const int OUT_PROVINCE = 0;//Ngoại tỉnh
        public const int TRANSPORT_TYPE_STANDARD = 1;//Tiêu chuẩn
        public const int TRANSPORT_TYPE_FAST = 2;//Nhanh
        public const int TRANSPORT_TYPE_WESEN = 3;//Wesen
        public const int TRANSPORT_TYPE_VIP_IN_PROVINCE = 4;// Siêu tốc nội thành
        public const int TRANSPORT_TYPE_VIP_STANDARD = 5;//Giao hàng toàn quốc - gói tiêu chuẩn
        public const int TRANSPORT_TYPE_VIP_AIRLINE = 6;//Giao hàng toàn quốc - gói đi bay

        public const int ORDER_COD_FEE = 1; //Đơn có tính phí COD
        public const int ORDER_NOT_COD_FEE = 0; //Đơn không tính phí COD

        

        //Param Shipper Type
        public const int SHIPPER_PARTNER = 0; // Shipper đối tác
        public const int SHIPPER_INTERNAL = 1; // Shipper nội bộ
        public const int SHIPPER_ALL = 2; // Tất cả shipper
        public const int SHIPPER_VIP_COUNTDOWN = 3; // Shipper vip có countdown
        public const int SHIPPER_VIP_NO_COUNTDOWN = 4; // Shipper vip ko có countdown

        public const int SHIPPER_NORMAL = 0;
        public const int SHIPPER_VIP = 1;

        public const int CUSTOMER_NORMAL = 0;
        public const int CUSTOMER_VIP = 1;

        // Config Type Booking
        public const int SHIP_DRIVER = 1;
        public const string SHIP_DRIVER_STR = "Đặt xe";
        public const int SHIP_PACKAGE = 2;
        public const string SHIP_PACKAGE_STR = "Giao hàng";
        public const int SHIP_FOOD = 3;
        public const string SHIP_FOOD_STR = "Giao đồ ăn";
        public const int SHIP_PACKAGE_FAST = 1;
        public const string SHIP_PACKAGE_FAST_STR = "Giao Nhanh";
        public const int SHIP_PACKAGE_NORMAL = 2;
        public const string SHIP_PACKAGE_NORMAL_STR = "Giao thường";
        // Config Bảng Coupon
        public const int COUPON_TYPE_COUPON_PERCENT = 1; // Giảm giá theo %
        public const int COUPON_TYPE_COUPON_DISCOUNT = 2; // Giảm giá theo tiền
        public const int COUPON_TYPE_DRIVER = 1;
        public const string COUPON_TYPE_DRIVER_STR = "Đặt xe";
        public const int COUPON_TYPE_PACKAGE = 2;
        public const string COUPON_TYPE_PACKAGE_STR = "Giao hàng";
        public const int COUPON_TYPE_FOOD = 3;
        public const string COUPON_TYPE_FOOD_STR = "Giao đồ ăn";
        public const int COUPON_TYPE_TIME_LIMIT = 1; // Có giới hạn thời gian
        public const int COUPON_TYPE_TIME_NOT_LIMIT = 2; // Vô thời hạn
        public const int COUPON_RANK_MEMBER = 1;
        public const string COUPON_RANK_MEMBER_STR = "Thành viên";
        public const int COUPON_RANK_SILVER = 2;
        public const string COUPON_RANK_SILVER_STR = "Bạc";
        public const int COUPON_RANK_GOLD = 3;
        public const string COUPON_RANK_GOLD_STR = "Vàng";
        public const int COUPON_RANK_DIAMOND = 4;
        public const string COUPON_RANK_DIAMOND_STR = "Kim cương";
        public const string COUPON_EMPTY_LIST = "Danh sách rỗng";
        public const int COUPON_NOT_FOUND = -1;
        public const string COUPON_NOT_FOUND_STR = "Mã giảm giá không tồn tại";
        public const string COUPON_APPLY_ERROR_STR = "Có lỗi trong quá trình áp dụng mã giảm giá";
        public const int COUPON_NOT_USED = 0; // Mã giảm giá chưa được sử dụng
        public const int COUPON_USED = 1; // Mã giảm giá đã qua sử dụng

        // config Bảng OrderService
        public const int PAYMENT_TYPE_ON_DELIVERY = 1; // Thanh toán = tiền mặt
        public const string PAYMENT_TYPE_ON_DELIVERY_STR = "Trả tiền mặt";
        public const int PAYMENT_TYPE_VN_PAY = 2; // Thanh toán VN PAY
        public const string PAYMENT_TYPE_VN_PAY_STR = "Thanh toán VN PAY";
        public const int ORDER_PAYMENT = 1; // Đã thanh toán
        public const int ORDER_NOT_PAYMENT = 0; // Chưa thanh toán
        public const int ORDER_RECEIVER_PAYMENT = 1; // Người nhận thanh toán
        public const int ORDER_RECEIVER_NOT_PAYMENT = 0; // Người nhận không thanh toán
        public const int ORDER_SHIPER_CANCLE = 0; // Shiper hủy đơn 
        public const int ORDER_USER_CANCLE = 1; // Khách hàng hủy đơn

        public const int MIN_PRICE_PAYMENT_VNPAY = 10000;

        public const int RATE_SHIPPER = 1; // Đánh giá tài xế
        public const int RATE_SHOP = 2; // Đánh giá shop
        public const int ORDER_RATE = 1; // Đã đánh giá
        public const int ORDER_NOT_RATE = 0; // Chưa đánh giá
        public const int ORDER_PUSH_FIRST = 1; // Đơn shipper đã được tiếp nhận
        public const int ORDER_NOT_PUSH_FIRST = 0; // Đơn shipper chưa được tiếp nhận
        public const int ORDER_IS_RATE_TRUE = 1; // Đơn shipper đã được đánh giá
        public const int ORDER_IS_RATE_FALSE = 0; // Đơn shipper chưa được đánh giá
        public const int ORDER_STATUS_DENY = -1; // Đơn Shipper bị từ chối
        public const int ORDER_STATUS_PENDING = 0; // Đơn Shipper chờ tiếp nhận
        public const int ORDER_STATUS_DELIVERY = 1; // Đơn Shipper đang tiếp nhận
        public const int ORDER_STATUS_PICK_UP = 2; // Đơn Shipper đã đón khách , đã lấy hàng , đồ ăn
        public const int ORDER_STATUS_FINISH = 3; // Đơn Shipper hoàn thành

        // Config API Thêm vào giỏ hàng
        public const int ADD_CART_INVALID_QUANTITY = -1;
        public const string ADD_CART_INVALID_QUANTITY_STR = "Số lượng không hợp lệ";
        public const int CART_NOT_FOUND = -2;
        public const string CART_NOT_FOUND_STR = "Không tìm thấy giỏ hàng";       

        public const int ADD_CART_FAIL = 0;
        public const string ADD_CART_FAIL_STR = "Thêm vào giỏ hàng thất bại";
        public const int UPDATE_CART_SUCCESS = 2;
        public const string UPDATE_CART_SUCCESS_STR = "Cập nhật giỏ hàng thành công";

        // Config bảng ServicePrice
        public const int SERVICE_TYPE_MAIN = 1; // Món chính
        public const int SERVICE_TYPE_TOPING = 2; // Món phụ

        // Config API Đặt Xe
        public const int BOOK_DRIVER_FAIL = 0;
        public const string BOOK_DRIVER_FAIL_STR = "Có lỗi trong quá trình đặt xe";
        public const int BOOK_DRIVER_SUCCESS = 1;
        public const string BOOK_DRIVER_SUCCESS_STR = "Đặt xe thành công";
        public const string BOOK_PACKAGE_SUCCESS_STR = "Đặt đơn giao hàng thành công";
        public const string BOOK_FOOD_SUCCESS_STR = "Đặt đơn giao đồ ăn thành công";
        public const int SHIPPER_NOT_FOUND = -1;
        public const string SHIPPER_NOT_FOUND_STR = "Hiện tại không có tài xế hoạt động trong khu vực";
        public const int AREA_NOT_FOUND = -2;
        public const string AREA_NOT_FOUND_STR = "Khu vực hoạt động không tồn tại";
        public const int COUPON_NOT_VALID = -3;
        public const string COUPON_NOT_VALID_STR = "Mã giảm giá không hợp lệ";
        public const int POINT_NOT_VALID = -4;
        public const string POINT_NOT_VALID_STR = "Điểm sử dụng không hợp lệ";
        public const int CUSTOMER_NOT_FOUND = -5;
        public const string CUSTOMER_NOT_FOUND_STR = "Không tìm thấy khách hàng";
        public const int ORDER_NOT_FOUND = -6;
        public const string ORDER_NOT_FOUND_STR = "Không tìm thấy đơn hàng";
        public const int WALLET_NOT_FOUND = -7;
        public const string WALLET_NOT_FOUND_STR = "Không tìm thấy ví shipper";
        public const int SHIPER_ACCEPT_SERVICE = -8;
        public const string SHIPER_ACCEPT_SERVICE_STR = "Tài xế đã tiếp nhận khách hàng";
        public const int WALLET_NOT_ENOUGH_MONEY = -9;
        public const string WALLET_NOT_ENOUGH_MONEY_STR = "Ví cọc tài xế không đủ tiền";
        public const int BOOK_DRIVER_ALREADY_PICK_UP = -10;
        public const string BOOK_DRIVER_ALREADY_PICK_UP_STR = "Khách hàng đã được tiếp nhận ";
        public const int BOOK_DRIVER_DECLINE = -11;
        public const string BOOK_DRIVER_DECLINE_STR = "Đơn hàng đã bị hủy";
        public const int BOOK_DRIVER_NOT_PICK_UP = -12;
        public const string BOOK_DRIVER_NOT_PICK_UP_STR = "Chưa có tài xế nào tiếp nhận đơn hàng";
        public const int PICK_UP_CUSTOMER_FAIL = -13;
        public const string PICK_UP_CUSTOMER_FAIL_STR = "Có lỗi trong quá trình đón khách";
        public const int DECLINE_BOOK_DRIVER_FAIL = -14;
        public const string DECLINE_BOOK_DRIVER_FAIL_STR = "Có lỗi trong quá trình hủy chuyến";
        public const int BOOK_DRIVER_ALREADY_DECLINE = -15;
        public const string BOOK_DRIVER_ALREADY_DECLINE_STR = "Tài xế đã từ chối đơn hàng";
        public const int ORDER_CUSTOMER_NO_PERMISSION = -16;
        public const string ORDER_CUSTOMER_NO_PERMISSION_STR = "Đây không phải đơn của khách hàng này";
        public const int ORDER_DRIVER_NO_PERMISSION = -17;
        public const string ORDER_DRIVER_NO_PERMISSION_STR = "Đây không phải đơn của tài xế này";
        public const int LOCATION_NOT_VALID = -18;
        public const string LOCATION_NOT_VALID_STR = "Vị trí không hợp lệ";
        public const int ORDER_NOT_DECLINE = -19;
        public const string ORDER_NOT_DECLINE_STR = "Đơn hàng này không thể hủy";
        public const int SHOP_NOT_FOUND = -20;
        public const string SHOP_NOT_FOUND_STR = "Không tìm thấy cửa hàng";
        public const int CART_EMPTY = -21;
        public const string CART_EMPTY_STR = "Giỏ hàng trống";
        public const int TOPING_NOT_FOUND = -22;
        public const string TOPING_NOT_FOUND_STR = "Món phụ không tồn tại";
        public const int WALLET_CUSTOMER_NOT_FOUND = -23;
        public const string WALLET_CUSTOMER_NOT_FOUND_STR = "Không tìm thấy ví khách hàng";
        public const int CUSTOMER_CAN_NOT_CANCLE_ORDER = -24;
        public const string CUSTOMER_CAN_NOT_CANCLE_ORDER_STR = "Khách hàng đã vượt quá số lần hủy chuyến";
        public const int SHOP_LOCATION_NOT_VALID = -25;
        public const string SHOP_LOCATION_NOT_VALID_STR = "Vị trí cửa hàng không tồn tại";        
        public const int COD_FEE_NOT_VALID = -26;
        public const string COD_FEE_NOT_VALID_STR = "Phí thu hộ của bạn vượt quá mức quy định";        
        public const int LOCATION_EMPTY = -27;
        public const string LOCATION_EMPTY_STR = "Xin vui lòng nhập địa chỉ";        
        public const int COUPON_NOT_REMAIN = -28;
        public const string COUPON_NOT_REMAIN_STR = "Mã giảm giá đã hết số lượt sử dụng";
        public const int COUPON_EXCEED_VALUE = -29;
        public const string COUPON_EXCEED_VALUE_STR = "Mã giảm giá vượt quá giá trị đơn hàng";
        public const int CART_CHANGED = -30;
        public const string CART_CHANGED_STR = "Giỏ hàng đã bị thay đổi ";        
        public const int ITEM_CHANGED = -31;
        public const string ITEM_CHANGED_STR = "Sản phẩm đã bị thay đổi ";
        public const int ORDER_ALREADY_DECLINE = -32;
        public const string ORDER_ALREADY_DECLINE_STR = "Đơn hàng đã bị hủy";        
        public const int ORDER_ALREADY_COMPLETE = -33;
        public const string ORDER_ALREADY_COMPLETE_STR = "Đơn hàng đã được hoàn thành";       
        public const int ORDER_VNPAY_MINPRICE_INVALID = -34;
        public const string ORDER_VNPAY_MINPRICE_INVALID_STR = "Thanh toán VNPAY chỉ áp dụng cho đơn hàng từ 10000 đồng trở lên";
        public const int ORDER_TRANSPORT_DRIVER_NOT_VALID = -35;
        public const string ORDER_TRANSPORT_DRIVER_NOT_VALID_STR = "Phương tiện đặt xe này không được hỗ trợ";
        public const int ORDER_TRANSPORT_PACKAGE_NOT_VALID = -36;
        public const string ORDER_TRANSPORT_PACKAGE_NOT_VALID_STR = "Khoảng cách giao hàng không được hỗ trợ";
        public const int ORDER_TRANSPORT_FOOD_NOT_VALID = -37;
        public const string ORDER_TRANSPORT_FOOD_NOT_VALID_STR = "Hình thức giao đồ ăn không được hỗ trợ";

        public const string DECLINE_BOOK_DRIVER_SUCCESS = "Hủy chuyến thành công";
        public const int CUS_MAX_CANCLE_TIME = 100;

        // Config Bảng CustomerRank
        public const int RANK_DIAMOND_LEVEL = 4; 

        // check box status
        public const int CHECKBOX = 1;
        public const int UNCHECKBOX = 0;
        public const int CHECKALL = 2;
        public const int UNCHECKALL = 3;
        //public const int TIME_DELAY = cnn.Configs.Where(u =>u.Key.Equals("")).Select(u =>u.Value).FirstOrDefault();
        public const int MIN_MONEY_SENNOTI = 1000;
        //public const int DISTANCE_DEFAULT = 5;
        public const int CUSTOMER_DEFAULT = 3;

        public const double SHOP_MAX_DISTANCE = 100;

        public const int CONFIG_TIME = 5;
        public const int ROLL_ADMIN = 1;
        public const int ROLL_ACCOUNTANT = 2;
        public const int ROLL_MARKETING = 3;
        public const string ROLL_ACCOUNTANT_STRING = "Accountant";
        public const string ROLL_MARKETING_STRING = "Marketing";
        public const int ROLL_CUSTOMER = 0;
        public const int POINT_START = 9;
        //public const int ROLL_ADMIN = 1;
        public const int TYPE_LOGIN_PHONE = 3;
        public const int TYPE_LOGIN_FACE = 1;
        public const int TYPE_LOGIN_GOOGLE = 2;

        public const int NO_NEED_UPDATE = 0;
        public const int NEED_UPDATE = 1;

        public const string CONVERT_DATETIME = "dd/MM/yyyy";
        public const string CONVERT_DATETIME_HAVE_HOUR = "HH:mm dd/MM/yyyy";
        public const string CONVERT_DATETIME_HOUR = "HH:mm";
        public const int MAX_ROW_IN_LIST = 30;
        public const int ACTIVE = 1;
        public const int INACTIVE = 0;
        public const int DEACTIVE = 2;

        public const int RETURN_TRUE = 1;
        public const int RETURN_FALSE = 0;
        public const int ACTIVE_FALSE = 0;
        public const int COUNT_NULL = 0;
        public const int DELETE_REQUEST_FAIL = 2;
        public const int CATEGORY_PRODUCT = 11;

        public const int TYPE_IMAGE = 1;
        public const int TYPE_VIDEO = 2;
        // thanh cong
        public const int SUCCESS_CODE = 200;
        // thanh cong
        public const int STATUS_CHANGED = 7749;
        // sai mk
        public const int ERROR_TOKEN_NOTFOUND = 403;
        // loi quy trinh
        public const int PROCESS_ERROR = 500;
        public const int PERMISSION_DENIED = 101;
        public const int FAIL = 501;
        public const int ERROR = 0;
        public const int SUCCESS = 1;

        //Type language Vietamese
        public const int LANGUAGE_VIETNAMESE = 1;
        //Type language english
        public const int LANGUAGE_ENGLISH = 2;

        // khong duoc phep
        public const int NOT_FOUND = 404;
        // khong thay du lieu
        public const int DATA_NOT_FOUND = 400;
        // khong duoc phep
        public const int UNAUTHORIZED = 401;

        public const int STATUS_RUNNING = 1;
        //Trạng thái yêu cầu rút tiền
        public const int STATUS_REQUEST_WAITING = 0;//Đang chờ duyệt
        public const int STATUS_REQUEST_SUCCESS = 1;//Xác nhận
        public const int STATUS_REQUEST_CANCEl = -1;//Hủy
        public const int STATUS_REQUEST_COMPLETE = 2;//Hoàn thành
        // Type đổi quà
        public const int TYPE_POINT_SAVE = 1;
        public const int TYPE_RECEIVE_ORDER = 2;
        //public const int TYPE_POINT_RECEIVE = 3;
        //public const int TYPE_POINT_RECEIVE_GIFT =4;
        //public const int TYPE_ADD_POINT =5;
        //public const int TYPE_CARD =6;

        public const int SIZE_CODE = 8;
        public const int MIN_NUMBER = 100000;
        public const int MAX_NUMBER = 999999;

        //Type shop Image
        public const int TYPE_SHOP_IMAGE = 1;// type ảnh đại diện
        public const int TYPE_SHOP_LICENSE = 2;//type ảnh giấy phép kinh doanh
        // Status warranty 
        public const int W_STATUS_ACTIVE = 1;
        public const int W_STATUS_NO_ACTIVE = 2;
        public const int W_STATUS_ERROR = 3;
        //Type service price
        public const int MAIN_DISHES = 1;//Món chính
        public const int SIDE_DISHES = 2;//món phụ
        // cách kiểu tích điểm
        public const int WARRANTY = 2;
        public const int PRODUCT = 1;

        //
        public const int MESS_BY_CUS = 1;
        public const int MESS_BY_ADMIN = 2;
        //
        public const int NEWS_TYPE_ADVERTISEMENT = 1;
        public const int NEWS_TYPE_NEWS = 2;
        public const int NEWS_TYPE_BANER_HOME = 3;
        public const int NEWS_TYPE_BANER_FOOD = 4;
        public const int NEWS_TYPE_PROMOTION = 5;
        public const int NEWS_TYPE_QA = 6;

        //Các trạng thái xử lý khiếu nại
        public const int STATUS_WAITING = 1;//Chờ xử lý
        public const int STATUS_COMPLETE = 2;//Đã xử lý


        //
        public const string COMMENT_HISTORY_ADD_POINT = "Tích điểm";
        // link check access Token
        public const string LINK_URL_FACEBOOK = "https://graph.facebook.com/me?fields=name,picture.height(960).width(960)&access_token=";
        public const string LINK_URL_GOOGLE_MAIL = "https://www.googleapis.com/plus/v1/people/me?access_token=";
        // Telecom
        public const int MAX_TELECOM = 4;
        public const string URL_VIETTEL = "https://upload.wikimedia.org/wikipedia/vi/thumb/e/e8/Logo_Viettel.svg/800px-Logo_Viettel.svg.png";
        public const string URL_MOBIPHONE = "https://upload.wikimedia.org/wikipedia/commons/d/de/Mobifone.png";
        public const string URL_VINAPHONE = "https://lozimom.com/wp-content/uploads/2017/04/vinaphone-logo.png";
        public const string URL_VIETNAMMOBILE = "https://upload.wikimedia.org/wikipedia/vi/thumb/a/a8/Vietnamobile_Logo.svg/1280px-Vietnamobile_Logo.svg.png";
        public const int TELECOM_TYPE_GIFT = 0;
        public const int TYPE_VIETTEL = 1;
        public const int TYPE_MOBIPHONE = 2;
        public const int TYPE_VINAPHONE = 3;
        public const int TYPE_VIETNAMMOBILE = 4;
        public const string TYPE_VIETTEL_STRING = "Viettel";
        public const string TYPE_MOBIPHONE_STRING = "Mobiphone";
        public const string TYPE_VINAPHONE_STRING = "Vinaphone";
        public const string TYPE_VIETNAMMOBILE_STRING = "VietnamMobile";
        public const string URL_FIRST = "https://graph.facebook.com/";
        public const string URL_LAST = "/picture?type=large&redirect=true&width=250&height=250";
        public const int STATUS_PRODUCT_ACTIVE = 1;
        public const int STATUS_PRODUCT_NO_ACTIVE = 2;
        public const string STATUS_PRODUCT_ACTIVE_STRING = "Đã sử dụng";
        public const string STATUS_PRODUCT_NO_ACTIVE_STRING = "Chưa sử dụng";

        public const int STATUS_REQUEST_PENDING = 0;
        public const int STATUS_REQUEST_ACCEPTED = 1;
        public const int STATUS_REQUEST_CANCEL = 2;
        public const string STATUS_REQUEST_PENDING_STRING = "Chờ xác nhận";
        public const string STATUS_REQUEST_ACCEPTED_STRING = "Đã xác nhận";
        public const string STATUS_REQUEST_CANCEL_STRING = "Hủy";

        public const int TYPE_REQUEST_GIFT = 1;
        public const int TYPE_REQUEST_VOUCHER = 2;
        public const int TYPE_REQUEST_CARD = 3;

        public const string TYPE_REQUEST_GIFT_STRING = "Quà tặng";
        public const string TYPE_REQUEST_VOUCHER_STRING = "Voucher";
        public const string TYPE_REQUEST_CARD_STRING = "Thẻ cào";

        public const int TYPE_GIFT_GIFT = 1;
        public const int TYPE_GIFT_VOUCHER = 2;
        public const int TYPE_GIFT_CARD = 3;

        public const int STATUS_GIFT_ACTIVE = 1;
        public const int STATUS_GIFT_PAUSE = 0;
        public const int STATUS_GIFT_CANCEL_AND_ADD = 2;
        public const int STATUS_GIFT_CANCEL = 3;

        public const int NO_ACTIVE_DELETE = 0;
        public const int MAX_ROW_IN_LIST_WEB = 20;
        public const bool BOOLEAN_TRUE = true;
        public const bool BOOLEAN_FALSE = false;
        public const int DUPLICATE_NAME = 2;

        public const int QRCODE_TYPE_PRODUCT = 1;
        public const int QRCODE_TYPE_WARRANTY = 2;

        public const int STATUS_CARD_ACTIVE = 1;
        public const int STATUS_CARD_NO_ACTIVE = 2;
        public const int ERROR_DATE = 3;



        public const float KeyA = 11;
        public const float KeyB = 87;
        public const float KeyC = 48;
        public const string ICON_DEFAULT = "https://img.icons8.com/color/240/000000/google-alerts.png";
        public const string ICON_SUB_POINT = "https://img.icons8.com/color/240/000000/google-alerts.png";


        public const int TYPE_NOTI_NEW_ORDER = 0;
        public const int TYPE_NOTI_CONFIRM_ORDER = 1;
        public const int TYPE_NOTI_ORDER_CUSSCESS = 2;
        public const int TYPE_NOTI_ORDER_CANCEL = 3;
        public const int TYPE_NOTI_ORDER_ADMIN = 4;

        public const int HAVE_A_NEW_ORDER = 1;
        public const int AGENT_DEFAULT_TYPE = -1;
        public const int HAVE_A_NEW_NOTI = 2;
        public const int HAVE_A_NEW_NEWS = 3;

        public const int TYPE_ADS = 1;
        public const int TYPE_EVENT = 2;
        public const int TYPE_NEWS = 3;
        public const int TYPE_PRODUCT = 4;
        public const int TYPE_PROMOTION = 5;
        public const int TYPE_PRICE_QUOTE = 6;

        public const string TYPE_ADS_STRING = "Advertisement";
        public const string TYPE_EVENT_STRING = "Event";
        public const string TYPE_NEWS_STRING = "News";
        public const string TYPE_PRODUCT_STRING = "Products";
        public const string TYPE_PROMOTION_STRING = "Promotion";
        public const string TYPE_PRICE_QUOTE_STRING = "Price quote";


        public const int PARENT_NEWS_PRODUCT = 11;

        public const int TYPE_SEND_CUSTOMER = 2;
        public const int TYPE_SEND_AGENCY = 1;
        public const int TYPE_SEND_ALL = 0;

        public const int STATUS_COUPONS_TYPE_EXPIRED = 1;
        public const int STATUS_COUPONS_TYPE_UNEXPIRED = 0;

        public const int STATUS_COUPONS_PERCENT = 1;
        public const int STATUS_COUPONS_DISCOUNT = 2;

        public const int STATUS_COUPONS_ACTIVE = 1;
        public const int STATUS_COUPONS_INACTIVE = 0;
        public const int STATUS_COUPONS_EXPIRED = 2;

        public const int STATUS_NEWS_ACTIVE = 1;
        public const int STATUS_NEWS_DRAFT = 0;
        public const int UPDATE_NEWS_DEFAULT = 1;
        public const int UPDATE_NEWS_POST = 0;
        public const int LENGTH_QR_HASH = 15;
        public const int EXISTING = 2;
        public const int CAN_NOT_DELETE = 2;
        public const int ROLE_USER_ORDER = 3;
        public const int ROLE_USER = 2;
        public const int ROLE_ADMIN = 1;
        public const int NOT_ADMIN = 3;
        public const int WRONG_PASSWORD = 2;
        public const int FAIL_LOGIN = 2;
        public const int TYPE_REQUEST_NOTIFY = 4;
        public const int TYPE_ORDER_NOTIFY = 7;

        //ROLE
        public const int TYPE_ROLE_ADMIN = 1;
        public const int TYPE_ROLE_MANAGEMENT = 2;//QUẢN LÝ ĐIỂM GD
        public const int TYPE_ROLE_TELLERS = 3;//GIAO DỊCH VIÊN
        public const int TYPE_ROLE_BUSINESS = 4;//Kinh Doanh
        public const int TYPE_ROLE_MARKETING = 5;//Marketing
        public const int TYPE_ROLE_ACCOUNTANT = 6;//KẾ TOÁN
        public const int TYPE_ROLE_BUSINESS_MANAGEMENT = 7;//Quản lý Kinh doanh

        public const string MAX_POINT_PER_DAY = "MaxPointPerDay";
        public const string MIN_POINT = "MinPoin";

        //Card
        public const string IMPORT_CARD_VIETTEL = "viettel";
        public const string IMPORT_CARD_MOBIPHONE = "mobi";
        public const string IMPORT_CARD_VINAPHONE = "vina";
        public const string IMPORT_CARD_VIETNAMOBILE = "vnmobile";
        public const int TELECOMTYPE_VIETTEL = 1;
        public const int TELECOMTYPE_MOBIPHONE = 2;
        public const int TELECOMTYPE_VINAPHONE = 3;
        public const int TELECOMTYPE_VIETNAMOBILE = 4;
        public const int ERROR_IMPORT_DUPLICATE = 0;
        //public const int NO_ACTIVE_CARD = 2;
        // status Order
        public const int ORDER_STATUS_WAITING = 0;
        public const int ORDER_STATUS_PROCESS = 1;
        public const int ORDER_STATUS_REFUSE = 2;
        // Status for all
        public const int STATUS_ACTIVE = 1;
        public const int STATUS_NO_ACTIVE = 0;

        // Not Found
        public const int PHONE_NOT_FOUND = -1;

        // ExcelFile Error
        public const int FILE_NOT_FOUND = -1;
        public const int FILE_DATA_DUPLICATE = 0;
        public const int FILE_IMPORT_SUCCESS = 1;
        public const int FILE_FORMAT_ERROR = -2;
        public const int FILE_EMPTY = -3;
        public const int IMPORT_ERROR = -4;
        public const int MIN_LENGTH_VALIDATE = -5;
        public const int DATA_ERROR = -6;

        // check Length Card
        public const int MAX_LENGTH_CODE = -7;
        public const int MAX_LENGTH_SERI = -8;
        public const int CODE_EQUALS_SERI = -9;

        // type MemberPointHistory
        public const int HISPOINT_TICH_DIEM = 1;
        public const int HISPOINT_TANG_DIEM = 2;
        public const int HISPOINT_DUOC_TANG_DIEM = 3;
        public const int HISPOINT_DOI_QUA = 4;
        public const int HISPOINT_HE_THONG_CONG_DIEM = 5;
        public const int HISPOINT_DOI_THE = 6;
        public const int HAVE_A_NEWS = 6;
        public const int CHANGE_MINUS_POINT_ITEM = 4;

        //
        public const int ACTIVE_AGENT = 1;

        public const int HISTORY_POINT_CANCEL_REQUEST = 7;


        public const string EXPO_NOTI = "https://exp.host/--/api/v2/push/send";

        public const string SUCCES_STR = "Thành công";
        public const string LANG = "lang";
        public const string VN = "vi";
        public const string EN = "en";

        public const int MINI_SECOND = 60;

        //type return result login/acount
        public const int USER_NOT_FOUND = 0;
        public const int PHONE_EXISTING = -1;
        public const int EMAIL_EXISTING = -2;
        public const int PASSWORD_INCORRECT = -3;
        //type notifivation
        public const int SAVE_NOTIFICATION = 2;
        public const int SEND_NOTIFICATION = 1;
    }

    public class Constant
    {
        public static int[] washerConstant = { 10, 13, 17, 19, 21, 37 };
        public static int[] customerConstant = { 20, 24, 48, 49, 70, 121 };
        public const int NOTI_FROM_ADMIN = 41;
        //type image requite
        public const int IMAGE_BEFORE = 1;
        public const int IMAGE_AFTER = 2;
        // TYPE IMAGE Service
        public const int MAIN_IMAGE = 1;
        public const int ORTHER_IMAGE = 2;
        public const int THUMBNAIL_IMAGE = 3;

        //otp
        public const int OTP_TYPE_REGISTER = 1;
        public const int OTP_TYPE_FORGOT_PASS = 2;

        // wallet
        public const int WALLET_WITHDRAW = 2;//Ví thu nhập
        public const int WALLET_NO_WITHDRAW = 1;//Ví cọc
        // role
        public const int ROLE_ALL = 0;
        public const int CUSTOMER_ROLE = 1;//Khách hàng
        public const int SHOP_ROLE = 2;//Shop
        public const int SHIPER_ROLE = 3;//Shiper

        // config
        public const string FIRST_LOGIN_ADD_POINT = "FirstLogin";
        public const string PROFIT = "Profit";
        public const string TIME_DELAY = "TimeWaiting";
        public const string LIMIT_IMAGE = "LimitCatImage";
        public const string SHIFT_TIME = "Shift";
        public const string MIN_BALANCE_SEND_REQUEST = "MinBalanceSendRequest";
        public const string MIN_BALANCE_SEND_REQUEST_FIRST = "MinBalanceSendRequestFirst";
        public const string MIN_BALANCE_SEND_MESSAGE = "MinBalanceSendMessage";
        public const string MAX_DISTANCE_SEND_REQUEST = "MaxDistanceSendRequest";
        public const string TIME_WITHDRAW = "TimeWithdraw";//Số lần rút tiền
        public const string MIN_VALUE_WALLET_WITHDRAW = "MinValueWalletWithdraw";//Số tiền tối thiểu trong ví thu nhập
        public const string MIN_VALUE_WALLET_NO_WITHDRAW = "MinValueWalletNoWithDraw";//Số tiền tối thiểu trong ví cọc
        public const string COUNT_DOWN = "CountDown";//Thời gian nhận count down
        public const string CANCLE_TIME = "CancleTime";//Số lần hủy trong ngày
        public const string TIME_CANCLE_ORDER = "TimeCancleOrder";//Thời gian tự động hủy đơn khi không có tài xế tiếp nhận đơn
        public const string MAX_AREA_NUMBER = "MaxAreaNumber";//Khu vực hoạt đọng tối đa
        public const string START_TIME = "StartTime";//Thời gian bắt đầu làm việc
        public const string END_TIME = "EndTime";//Thời gian kết thúc
        public const string MAX_COD_FEE = "MaxCODFee";//Phí thu hộ tối đa


        public const int MAX_DEVICEID_ONESIGNAL = 30;

        // transaction Type ( bảng MemberTransactionHistory)

        // rút tiền
        public const int TYPE_TRANSACTION_WITHDRAW = 1;
        public const string TYPE_TRANSACTION_WITHDRAW_ICON = "/Uploads/icons/DepositMoney.png";
        public const int TYPE_TRANSACTION_REFUND_WITHDRAW = 2;//hoàn tiền khi yêu cầu rút tiền bị từ chối
        public const string TYPE_TRANSACTION_REFUND_WITHDRAW_ICON = "/Uploads/icons/DepositMoney.png";
        // chuyển tiền
        public const int TYPE_TRANSACTION_TRANSFER_WALLET = 3;//Chuyển tiền sang ví cọc
        public const string TYPE_TRANSACTION_TRANSFER_WALLET_ICON = "/Uploads/icons/ExchangeMoney.png";

        public const int TYPE_TRANSACTION_TRANSFER_NO_WALLET = 4;//Nhận tiền khi hoàn thành đơn
        public const string TYPE_TRANSACTION_TRANSFER_NO_WALLET_ICON = "/Uploads/icons/OrderSuccess.png";

        public const int TYPE_TRANSACTION_RECHARGE = 5;//Nạp tiền từ hệ thống
        public const string TYPE_TRANSACTION_RECHARGE_ICON = "/Uploads/icons/DepositMoney.png";
        public const int TYPE_TRANSACTION_ACCEPT_ORDER = 6;//Trừ tiền ví cọc khi nhận đơn
        public const string TYPE_TRANSACTION_ACCEPT_ORDER_ICON = "/Uploads/icons/MinusServiceFee.png";
        public const int TYPE_TRANSACTION_REFUND_ORDER_CANCLE = 7;//Hoàn tiền ví cọc khi đơn bị hủy
        public const string TYPE_TRANSACTION_REFUND_ORDER_CANCLE_ICON = "/Uploads/icons/OrderSuccess.png";
        // Thanh toán VnPay
        public const int TYPE_TRANSACTION_VNPAY = 8;// Thanh toán VNPAY
        public const int TYPE_TRANSACTION_VNPAY_REFUND = 9;// Hoàn tiền VNPAY khi đơn bị hủy

        // Đặt đơn 
        public const int TYPE_TRANSACTION_USE_POINT = 10; // Dùng điểm thưởng
        public const int TYPE_TRANSACTION_REFUND_USE_POINT = 11; // Hoàn điểm thưởng
        public const int TYPE_TRANSACTION_EARN_POINT = 12; // Cộng điểm thưởng
        public const int TYPE_TRANSACTION_EARN_POINT_LEVEL_UP = 13; // Cộng điểm tăng hạng

        public const int TYPE_TRANSACTION_TRANSFER_NO_WALLET_EXCHANGE = 14;//Nhận tiền từ ví thu nhập;
        public const string TYPE_TRANSACTION_TRANSFER_NO_WALLET_EXCHANGE_ICON = "/Uploads/icons/ExchangeMoney.png";
        public const int TYPE_TRANSACTION_RECHARGE_ADMIN = 15;// Cộng tiền từ Admin
        public const string TYPE_TRANSACTION_RECHARGE_ADMIN_ICON = "/Uploads/icons/DepositMoney.png";

        // Type ( bảng MemberTransactionHistory)
        public const int RECHAGE = 1;//Type cộng
        public const int SUBTRACT_POINT = 0;//Type trừ
        public const int TRANSACTION_ADD_POINT = 1;//Type cộng
        public const int TRANSACTION_SUBTRACT_POINT = 0;//Type trừ


        //public const string TYPE_TRANSACTION_WITHDRAW_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        //public const string TYPE_TRANSACTION_RECHARGE_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_SUBTRACT_POINT_BUY_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_ADD_POINT_BUY_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_ADD_POINT_BY_ADMIN_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_SUBTRACT_WASHER_SUBMIT_ORDER_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_REWARD_WASHER_COMPLETE_ORDER_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_USE_POINT_CUSTOMER_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";
        public const string TYPE_TRANSACTION_FIRST_LOGIN_ICON = "https://cdn4.iconfinder.com/data/icons/finance-and-banking-free/64/Finance_loan_money-512.png";

        public const int STATUS_TRANSACTION_SUCCESS = 1;//Giao dịch thành công
        public const int STATUS_TRANSACTION_WAITING = 2;//Chờ xác nhận
        public const int STATUS_TRANSACTION_FLASE = 0;//Từ chối
        public const int STATUS_TRANSACTION_APPROVE = 3;//Xác nhận

        /// <summary>
        /// req for Admin
        /// </summary>
        public const int STATUS_REQUEST_SUCCESS = 1;
        public const int STATUS_REQUEST_REJECT = 0;
        public const int STATUS_REQUEST_PENDING = 2;
        public const int STATUS_REQUEST_APPROVE = 3;
        public const string PASS_DEFAUL_CUSTOMER = "123456";

        public const int TRANSACTION_TYPE_RECHARGE_WALLET_WITHDRAW = 1;
        public const int TRANSACTION_TYPE_RECHARGE_WALLET_NO_WITHDRAW = 2;
        // orderService
        public const int ORDER_STATUS_WAITING = 1;
        public const int ORDER_STATUS_CONFIRM = 2;
        public const int ORDER_STATUS_COMPLETE = 3;
        public const int ORDER_STATUS_WASHING = 5;
        public const int ORDER_STATUS_CONFIRM_WASHING = 6;
        public const int ORDER_STATUS_CANCEL = 0;
        public const int ORDER_STATUS_NO_CONFIRM = 4;
        public const int ORDER_STATUS_FIND_ORTHER_WASHER = 9;
        public const int ORDER_STATUS_CANT_SEE = -1;

        public const int TYPE_MAIN_SERVICE = 1;
        public const int TYPE_ADDITION_SERVICE = 2;

        public const int TYPE_ORDER_SERVICE_IMAGE_CAR = 1;
        public const int TYPE_ORDER_SERVICE_IMAGE_PARK = 2;
        // orther
        public const int TIME_EXPRICE_TOKEN = 3;
        public const int PER_PAGE = 16;
        public const string HTTP = "http://";
        // DeleteImageType
        public const int DELETE_IMAGE_CAR = 1;
        public const int DELETE_IMAGE_ORDER_STEP = 2;

        // coupon
        public const int COUPON_PRODUCT = 1;
        public const int COUPON_ITEM = 2;

        public const int COUPON_TYPE_PERCENT = 1;
        public const int COUPON_TYPE_DISCOUNT = 2;


        public const int COUPON_NOT_USED = 1;
        public const int COUPON_USED = 2;

        //
        public const string URL_CAR_BRAND = "https://private-anon-32ae3f7662-carsapi1.apiary-mock.com/manufacturers";
        public const string URL_CAR_MODEL = "https://private-anon-32ae3f7662-carsapi1.apiary-mock.com/cars";
        // KeyGoogle
        public const string KEY_GOOGLE_MAP = "AIzaSyCmeRtaN1l1mhfL6l7ExCjmi1eVzdh58m4";
        public const string KEY_PROVINCE = "administrative_area_level_1";
        public const string KEY_DISTRICT = "administrative_area_level_2";
        public const string STATUS_SUCCESS = "OK";
        // Washer
        public const string KEY_CODE_WASHER = "CM";
        public const int MAX_LENGT_ID = 2;




        // Payment
        public const int PAYMENT_TYPE_NO_VNP = 1;
        public const int PAYMENT_TYPE_VNP = 2;

        public const int STATUS_PAYMENT_WAITING = 0;
        public const int STATUS_PAYMENT_COMPLETE = 1;
        public const int STATUS_PAYMENT_CANCEL = 2;

        // new
        public const int NEWS_PUSH_NOTI = 1;
        public const int NEWS_NO_PUSH_NOTI = 0;
    }

    public class StatusOrder
    {
        public static int[] STATUS_WAITING = { 1, 4, 9 };
        public static int[] STATUS_CONFIRMED = { 2 };
        public static int[] STATUS_WASHING = { 5, 6 };
        public static int[] STATUS_COMPLETE = { 3 };
        public const string STATUS_WAITING_STR = "Waiting";
        public const string STATUS_CONFIRMED_STR = "Confirmed";
        public const string STATUS_WASHING_STR = "Washing";
        public const string STATUS_COMPLETE_STR = "Completed";
        public const string STATUS_CANCEL_STR = "Canceled";
    }

    public class MessVN
    {
        // Return output ChangeStatus
        public const string CHANGE_STATUS_SUCCES = "Chuyển trạng thái thành công";
        public const string CHANGE_STATUS_PERMISSION_DINE = "Không có quyền đổi trạng thái đơn hàng";
        public const string CHANGE_STATUS_CONFIRM_ERROR = "Đơn hàng đã có người khác nhận";
        public const string CHANGE_STATUS_HAVE_ORDER = "Vui lòng hoàn thành đơn để bắt đầu đơn mới";
        public const string CHANGE_STATUS_FINISH_ERROR = "Vui lòng tải đủ ảnh trước khi hoàn thành đơn";
        public const string CHANGE_STATUS_CHANGED = "Đơn hàng đã chuyển trạng thái";
        public const string CHANGE_STATUS_CANCEL = "Đơn hàng đã bị hủy";
        public const string CHANGE_STATUS_FALSE = "Bạn chưa thể thực hiện thao tác này";
        public const string CHANGE_STATUS_IMAGE_FLASE = "Vui lòng tải đủ ảnh trước khi thực hiện tác vụ này";
        public const string CHANGE_STATUS_NOT_ENOUGH_AMOUNT = "Tài khoản của bạn không đủ để nhận đơn này";
        public const string NOT_ENOUGH_MONEY = "Tài khoản của bạn không đủ để thực hiện giao dịch này";
        public const string MONEY_INVALID = "Số tiền giao dịch không được quá 10,000,000";
        public const string ERROR_ROLE_INFO = "Lỗi thông tin tài khoản";
        /// <summary>
        /// Mess type Transacsion
        /// </summary>
        /// 

        public const string TYPE_TRANSACTION_WITHDRAW_STR = " từ yêu cầu rút điểm ";
        public const string TYPE_TRANSACTION_RECHARGE_STR = " điểm từ việc nạp tiền";
        public const string TYPE_TRANSACTION_SUBTRACT_POINT_BUY_PRODUCT_STR = " điểm từ việc rút tiền";
        public const string TYPE_TRANSACTION_ADD_POINT_WHEN_COMPELETE_CUSTOMER_STR = " điểm thưởng sau khi hoàn tất đơn hàng ";
        public const string TYPE_TRANSACTION_ADD_POINT_BY_ADMIN_STR = " điểm từ hệ thống";
        public const string TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN_STR = " điểm từ hệ thống do huỷ đơn hàng ";
        public const string TYPE_TRANSACTION_SUBTRACT_WASHER_SUBMIT_ORDER_STR = " điểm khi nhận đơn hàng ";
        public const string TYPE_TRANSACTION_REWARD_WASHER_COMPLETE_ORDER_STR = " điểm thưởng khi hoàn thành đơn hàng ";
        public const string TYPE_TRANSACTION_USE_POINT_CUSTOMER_STR = " điểm cho đơn hàng ";
        public const string TYPE_TRANSACTION_FIRST_LOGIN_STR = " điểm cho lần đầu đăng nhập";
        public const string MESS_ADD = "Nhận ";
        public const string MESS_SUBTRACT = "Trừ ";
        /// <summary>
        ///  mess Result
        /// </summary>
        public const string REQUIRE_FIELD = "Vui lòng không để trống!";
        public const string INVALID_NUMBER = "Chỉ được phép nhập số!";
        public const string CONFIRM_FAIL = "Hệ thống đã hết thẻ vui lòng chọn sản phẩm khác";
        public const string SUCCESS_STR = "Thành công";
        public const string ERROR_STR = "Hệ thống bảo trì";
        public const string TOKEN_ERROR = "Tài khoản của bạn đã được đăng nhập ở nơi khác";
        public const string NOT_FOUND_MESS = "Không tồn tại";
        public const string PERMISSION_DENIED_MES = "Không có quyền truy cập";
        public const string ERROR_CODE = "Mã đã tồn tại";
        /// <summary>
        /// orther
        /// </summary>
        public const string LIMIT_IMAGE_ERROR = "Vượt quá số ảnh quy định";
        public const string FALSE_CONVERT_DATETIME = "Sai định dạng ngày";
        public const string PAGE_ERROR = "Trang không tồn tại";
        public const string LICENSEPLATES_ERROR = "Biển số xe đã tồn tại";
        /// <summary>
        /// TranSaction
        /// </summary>
        public const string Transaction_Succes = "Giao dịch thành công";
        public const string Transaction_False = "Giao dịch thất bại mã lỗi : ";
        public const string Transaction_False_view = "Giao dịch thất bại";

        public const string NOTI_HEADER = "Thông báo";
        /// <summary>
        /// Order
        /// </summary>
        public const string WASHER = "Chuyên gia ";
        public const string LIMIT_CANCEL = "Bạn đã huỷ quá nhiều lần , vui lòng thanh toán bằng VNPAY";
        public const string LIST_ITEM_NULL = "Vui lòng chọn gói dịch vụ";
        public const string WALLET_ERROR = "Ví của bạn không có điểm";
        public const string ITEM_MAIN_ERROR = "Chỉ được chọn một gói dịch vụ chính";
        public const string CAR_ERROR = "Vui lòng chọn xe";
        public const string LOCATION_ERROR = "Vui lòng thử lại";
        public const string AREA_ERROR = "Khu vực chưa được hỗ trợ";
        public const string NO_WASHER = "Rất tiếc tất cả các chuyên gia đều bận vui lòng thử lại sau";
        public const string WASHER_NO_WORKING = " không thể thực hiện yêu cầu của bạn lúc này";
        public const string PLACE_ERROR = "Vui lòng thử lại";
        public const string HAVE_SHCEDULE = "Đã có lịch rửa xe trong thời gian này";
        public const string BOOK_DATE_ERROR = "Thời gian đặt lịch không hợp lệ";
        public const string COUPON_ERROR = "Mã giảm giá không tồn tại";
        public const string AGENT_ERROR = "Mã chuyên gia rửa xe không đúng";
        public const string REVIEW_ERROR = "Bạn chưa thể đánh giá chuyên gia rửa xe";
        public const string SHIFT_ERROR = "Hiện tại hệ thống không làm việc ở khung giờ này vui lòng chọn khung giờ khác";
        public const string PERMISSION_DENIED_CHANGE = "Bạn không có quyển thao tác với đơn này";
        public const string REASON_ERROR = "Vui lòng điền lý do bạn huỷ đơn hàng này";
        public const string LIMIT_CANCEL_ERROR = "Bạn đã huỷ quá nhiều lần , vui lòng thanh toán bằng VNPAY";
        public const string MAIN_SERVICE_NOT_FOUND = "Gói dịch vụ đang tạm dừng hoạt động";
        public const string ADDITION_SERVICE_ERROR = "Gói dịch vụ phụ đang tạm dừng hoạt động";


        /// <summary>
        /// Wallet
        /// </summary>
        public const string WALLET_NOT_FOUND = "Không tìm thấy ví ";
        public const string WALLET_DAY_WITHDRAW_FALSE_BEFORE = "Bạn chỉ có thẻ rút tiền từ ngày ";
        public const string WALLET_POINT_WITHDRAW_FALSE = "Số điểm của bạn không đủ để thực hiện giao dịch này";
        public const string WALLET_MIN_POINT_WITHDRAW_FALSE = "Số điểm rút của bạn ít nhất là ";
        public const string WALLET_MIN_BALANCE_WITHDRAW_FALSE_BEFORE = "Bạn phải có ít nhất ";
        public const string WALLET_MIN_BALANCE_WITHDRAW_FALSE_AFTER = " điểm trong ví để thực hiện giao dịch này";

        /// <summary>
        /// content NOTI
        /// </summary>
        public const string NOTI_ORDER_STATUS_WAITING = "Bạn có đơn hàng mới cần xác nhận";
        public const string NOTI_ORDER_STATUS_CONFIRM = "Đơn hàng bạn đặt đã được xác nhận";
        public const string NOTI_ORDER_STATUS_COMPLETE = "Xe của bạn đã được rửa xong ";
        public const string NOTI_ORDER_STATUS_WASHING = "Xe của bạn đang được Chuyên gia rửa";
        public const string NOTI_ORDER_STATUS_CANCEL = "Đơn hàng của bạn đã bị hủy";
        public const string NOTI_ORDER_STATUS_NO_CONFIRM = "Đơn hàng chưa có người xác nhận";
        public const string NOTI_WALLET_NEED_RECHARGE_BEFORE = "Ví điểm của bạn dưới ";
        public const string NOTI_WALLET_NEED_RECHARGE_AFTER = " vui lòng nạp thêm điểm để tiếp tục sử dụng dịch vụ";
        public const string NOTI_WALLET_NEED_RECHARGE_AFTER2 = " vui lòng nạp thêm điểm để nhận đơn mới";
        public const string NOTI_TRANSACTION_AMOUNT = "Số điểm: ";
        public const string NOTI_TRANSACTION_BALANCE = "/nSố dư: ";
        public const string NOTI_TRANSACTION_SUCCESS = "Giao dịch hoàn thành";
        public const string NOTI_UPLOAD_IMAGE = "Thợ rửa xe vừa tải lên ảnh ";
        public const string NOTI_FROM_ADMIN = "Thông báo từ hệ thống";
        public const string NOTI_REVIEW = "Bạn vừa được đánh giá ";

        /// <summary>
        /// Login
        /// </summary>
        public const string LOGIN_ERROR = "Sai tài khoản hoặc mật khẩu";
        public const string PASS_ERROR = "Mật khẩu không đúng";
        public const string PHONE_ERROR = "Số điện thoại chưa được đăng ký";
        public const string EMAIL_ERROR = "Email không hợp lệ";
        public const string INVALID_PHONE = "Số điện thoại không hợp lệ";
        public const string EMAIL_USED = "Email đã được sử dụng";
        public const string PHONE_USED = "Số điện thoại đã được sử dụng";
        public const string PARAM_ERROR = "Vui lòng nhập đầy đủ thông tin";
        public const string OTP_ERROR = "Vui lòng nhập lại mã OTP";
        public const string OTP_SUCCES = "Xác nhận tài khoản thành công";
        public const string ACCOUNT_EXISTS = "Tài khoản đã tồn tại";
        public const string INCORRECT_OLD_PASS = "Mật khẩu cũ không đúng";
        public const string INVALID_NEW_PASS = "Mật khẩu mới không được trùng với mật khẩu mới";
        public const string MAX_CAR_IMAGE_ERROR = "Bạn chỉ có thể tải lên tối đa ";
        public const string ADD_CAR_SUCCESS = "Chúng tôi đã gửi yêu cầu của bạn lên hệ thống vui long đợi phản hồi của quản lý";
    }

    public class MessEN
    {
        // Return output ChangeStatus
        public const string CHANGE_STATUS_SUCCES = "Order's status is completed";
        public const string CHANGE_STATUS_PERMISSION_DINE = "Permission dinied";
        public const string CHANGE_STATUS_CONFIRM_ERROR = "This application already has recipients, you cannot accept it";
        public const string CHANGE_STATUS_HAVE_ORDER = "You have an incomplete application. Please complete the application to start a new application";
        public const string CHANGE_STATUS_FINISH_ERROR = "Please upload enough photos before completing the application";
        public const string CHANGE_STATUS_CHANGED = "The Status of order had changed";
        public const string CHANGE_STATUS_FALSE = "You cannot do this yet";
        public const string CHANGE_STATUS_IMAGE_FLASE = "Please upload all images before taking this action";
        public const string CHANGE_STATUS_NOT_ENOUGH_AMOUNT = "Your Account Balance is not sufficient to receive this application";
        public const string CHANGE_STATUS_CANCEL = "The order has been canceled";
        /// <summary>
        /// Mess type Transacsion
        /// </summary>
        public const string TYPE_TRANSACTION_WITHDRAW_STR = "Withdraw";
        public const string TYPE_TRANSACTION_RECHARGE_STR = "Top up";
        public const string TYPE_TRANSACTION_SUBTRACT_POINT_BUY_PRODUCT_STR = " points when buying products ";
        public const string TYPE_TRANSACTION_ADD_POINT_WHEN_COMPELETE_CUSTOMER_STR = " reward point after completing the service ";
        public const string TYPE_TRANSACTION_ADD_POINT_BY_ADMIN_STR = " points from the system ";
        public const string TYPE_TRANSACTION_REFUND_POINT_BY_ADMIN_STR = " points from the system";
        public const string TYPE_TRANSACTION_SUBTRACT_WASHER_SUBMIT_ORDER_STR = " points when confirming applications ";
        public const string TYPE_TRANSACTION_REWARD_WASHER_COMPLETE_ORDER_STR = " points for completing orders ";
        public const string TYPE_TRANSACTION_USE_POINT_CUSTOMER_STR = " points for order ";
        public const string TYPE_TRANSACTION_FIRST_LOGIN_STR = " points for the first login";
        public const string MESS_ADD = "Added ";
        public const string MESS_SUBTRACT = "Subtracted ";
        /// <summary>
        ///  mess Result
        /// </summary>
        //public const string REQUIRE_FIELD = "Vui lòng không để trống!";
        public const string INVALID_NUMBER = "You may only enter numbers";
        public const string SUCCESS_STR = "Success req";
        public const string ERROR_STR = "The system is maintenance";
        public const string TOKEN_ERROR = "Your account has been signed in elsewhere";
        public const string NOT_FOUND_MESS = "Data not found";
        public const string PERMISSION_DENIED_MES = "Permission deined";
        /// <summary>
        /// orther
        /// </summary>
        public const string LIMIT_IMAGE_ERROR = "The number of images has exceeded the number of images specified";
        public const string FALSE_CONVERT_DATETIME = "Wrong date format";
        public const string PAGE_ERROR = "Wrong param page";
        public const string LICENSEPLATES_ERROR = "License plate already exists";

        /// <summary>
        /// TranSaction
        /// </summary>
        public const string Transaction_Succes = "Transaction completed";
        public const string Transaction_False = "Transaction failed code: ";
        public const string Transaction_False_view = "Giao dịch thất bại";

        public const string NOTI_HEADER = "Notification";

        /// <summary>
        /// Order
        /// </summary>
        public const string WASHER = "Mater ";
        public const string LIMIT_CANCEL = "You have canceled too many times, please pay with VNPAY";
        public const string LIST_ITEM_NULL = "Please select one main service";
        public const string WALLET_ERROR = "Wallet balance is not enough";
        public const string ITEM_MAIN_ERROR = "Only one main service can be selected";
        public const string CAR_ERROR = "Please select your car before selecting service";
        public const string LOCATION_ERROR = "Please try again";
        public const string PLACE_ERROR = "Please try again";
        public const string AREA_ERROR = "Area is inactive";
        public const string NO_WASHER = "Sorry all the Mater are busy please try again later";
        public const string WASHER_NO_WORKING = " your req could not be processed at this time";
        public const string HAVE_SHCEDULE = "You had schedule";
        public const string BOOK_DATE_ERROR = "The booking time is not valid";
        public const string COUPON_ERROR = "The coupon failed";
        public const string AGENT_ERROR = "Wrong agent's code";
        public const string REVIEW_ERROR = "You cannot rate Washer yet";
        public const string SHIFT_ERROR = "Currently the system does not work at this time please select another time";
        public const string PERMISSION_DENIED_CHANGE = "You do not have an operation manual for this application";
        public const string REASON_ERROR = "Please enter your reason for canceling this service";
        public const string LIMIT_CANCEL_ERROR = "You have canceled too many times, please pay with VNPAY";
        public const string MAIN_SERVICE_NOT_FOUND = "The service is not working";
        public const string ADDITION_SERVICE_ERROR = "Have a addition service is not working";
        /// <summary>
        /// Wallet
        /// </summary>
        public const string WALLET_NOT_FOUND = "Wallet is not found";
        public const string WALLET_DAY_WITHDRAW_FALSE_BEFORE = "You can withdraw from ";
        public const string WALLET_POINT_WITHDRAW_FALSE = "wallet balance is not enough";
        public const string WALLET_MIN_POINT_WITHDRAW_FALSE = "Your withdrawal amount is at least ";
        public const string WALLET_MIN_BALANCE_WITHDRAW_FALSE_BEFORE = "You must have at least ";
        public const string WALLET_MIN_BALANCE_WITHDRAW_FALSE_AFTER = " points in you'r wallet to make this transaction";

        /// <summary>
        /// content NOTI
        /// </summary>
        public const string NOTI_ORDER_STATUS_WAITING = "You have new orders to confirm";
        public const string NOTI_ORDER_STATUS_CONFIRM = "Your order has been confirmed";
        public const string NOTI_ORDER_STATUS_COMPLETE = "Your car has finished washing ";
        public const string NOTI_ORDER_STATUS_WASHING = "Your car is being washed by car washers";
        public const string NOTI_ORDER_STATUS_CANCEL = "Your order has been cancel";
        public const string NOTI_ORDER_STATUS_NO_CONFIRM = "Order has not been confirmed";
        public const string NOTI_WALLET_NEED_RECHARGE_BEFORE = "Your wallet is under ";
        public const string NOTI_WALLET_NEED_RECHARGE_AFTER = " points, please top up to continue using the service";
        public const string NOTI_WALLET_NEED_RECHARGE_AFTER2 = "points, please submit additional points to receive a new application";
        public const string NOTI_TRANSACTION_AMOUNT = "Amount: ";
        public const string NOTI_TRANSACTION_BALANCE = "/n Balance: ";
        public const string NOTI_TRANSACTION_SUCCESS = "Transaction complete";
        public const string NOTI_UPLOAD_IMAGE = "The car washer has just uploaded the photo ";
        public const string NOTI_FROM_ADMIN = "Notification from the System";
        public const string NOTI_REVIEW = "You have just been evaluated ";
        /// <summary>
        /// Login
        /// </summary>
        public const string LOGIN_ERROR = "Wrong account or password";
        public const string EMAIL_ERROR = "Wrong email";
        public const string PASS_ERROR = "Wrong password";
        public const string EMAIL_USED = "Email already exists";
        public const string PARAM_ERROR = "Please enter full information";
        public const string OTP_ERROR = "Please enter the OTP code again";
        public const string OTP_SUCCES = "Account verification successful";
        public const string ACCOUNT_EXISTS = "Account already exists";
        public const string PHONE_ERROR = "The Phone number is not register";

        public const string MAX_CAR_IMAGE_ERROR = "You can only upload at most ";
        public const string ADD_CAR_SUCCESS = "We have submitted your req to the system, please wait for the management's response";



    }

    public class CustomerOnesignal
    {
        public const string APP_ID = "5a36e238-e0b9-481b-82a6-f496aa9b3682";
        public const string ANDROID_CHANNEL_ID = "c8a999be-9cbe-4a17-9839-a1c1c7acc553";
    }

    public class WasherOnesignal
    {
        public const string APP_ID = "ac4b681b-0885-400d-8fe4-ec83a8f67547";
        public const string ANDROID_CHANNEL_ID = "257fe7ea-90a6-4772-a16b-25d0c061f01c";
        public const string NO_SOUND = "ea8f7e6d-106f-47af-a537-c3d5837a5435";
    }

    public class OTPTEST
    {
        public const string SEND_TIME = "null";
        public const string USER = "vmgtest1";
        public const string PASS = "vmG@123b";
        public const string ALIAS = "VMGtest";
        public const string LINK_MESS = "http://brandsms.vn:8018/VMGAPI.asmx/BulkSendSms?";
        public const string CONTENT_MESS = "Ma xac nhan cua quy khach la ";
    }
    public class OTPRelease
    {
        public const string ALIAS = "Carrect.vn";
        public const string LINK_MESS = "http://api.brandsms.vn/api/SMSBrandname/SendSMS";
        public const string CONTENT_MESS = " la ma xac nhan cua ban";
        public const string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c24iOiJldmVyZ2xvd3ciLCJzaWQiOiI1MTRhODNkNS00M2Y1LTQ2YWMtOWQxYi03YTYwZmQwYzg2OTMiLCJvYnQiOiIiLCJvYmoiOiIiLCJuYmYiOjE1OTA0NjA0NDMsImV4cCI6MTU5MDQ2NDA0MywiaWF0IjoxNTkwNDYwNDQzfQ.DhRjrL1x0HlhtaUnlDWkZyNTZYXXopnbe1dgB35d2Ms";
    }
    public class ServerUrl
    {
        public static string[] MAIN_SERVER = { "http://118.27.192.110", "http://carrect.winds.vn", "http://api.carrect.vn" };
        public static string[] SERVER_TEST_WINDSOFT = { "http://winds.hopto.org" };
        public static string[] SERVER_TEST_CARRECT = { "http://winds.hopto.org:8517" };
        public const int TYPE_MAIN_SERVER = 1;
        public const int TYPE_SERVER_TEST_WINDSOFT = 2;
        public const int TYPE_SERVER_TEST_CARRECT = 3;
        public const string CONTENT = "";
    }
}
