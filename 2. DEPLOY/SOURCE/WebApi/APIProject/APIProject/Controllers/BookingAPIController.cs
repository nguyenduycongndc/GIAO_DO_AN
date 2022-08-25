using APIProject.Models;
using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace APIProject.Controllers
{
    public class BookingAPIController : BaseAPIController
    {
        // GET: Booking
        // Lấy danh sách phương tiện
        [HttpGet]
        public JsonModel GetListVehicle(string Origin, string Destination)
        {
            try
            {
                var model = bookBus.GetListVehicle(Origin, Destination);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        [HttpGet]
        public JsonModel GetNearShippers(double Longtitude, double Latitude, double Radius)
        {
            try
            {
                var model = bookBus.GetNearShipers(Longtitude, Latitude, Radius);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        [HttpGet]
        public JsonModel GetCustomerLocation(int Type = 1)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetCustomerLocation(cusID.Value, Type);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Lấy phí ship giao hàng
        [HttpGet]
        public JsonModel GetPackageDeliveryPrice(string Origin, string Destination, double Weight)
        {
            try
            {
                var model = bookBus.GetPackageDeliveryPrice(Origin, Destination, Weight);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy phí ship giao hàng
        [HttpGet]
        public JsonModel GetPackageDeliveryPriceVIP(string Origin, string Destination, double Weight,int Type)
        {
            try
            {
                var model = bookBus.GetPackageDeliveryPriceVIP(Origin, Destination, Weight,Type);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy phí ship giao đồ ăn
        [HttpGet]
        public JsonModel GetFoodDeliveryPrice(int ShopID, string Destination)
        {
            try
            {
                var model = bookBus.GetFoodDeliveryPrice(ShopID, Destination);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy danh sách mã giảm giá
        [HttpGet]
        public JsonModel GetListCoupon(int? Type = null, int? Used = null)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetListCoupon(cusID.Value, Type, Used);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy số điểm tích lũy của khách hàng
        [HttpGet]
        public JsonModel GetCustomerPoint()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetCustomerPoint(cusID.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy đơn đặt xe hiện tại của tài xế
        public JsonModel GetDriverOrderInfo(int type = 0, int page = 1, int limit = 15,int? typeBooking = null ,string fromDate = "",string toDate = "")
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetDriverOrderInfo(shipperID.Value, type, page, limit,typeBooking,fromDate,toDate);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }

        }

        // Tìm tài xế
        [HttpPost]
        public JsonModel FindDriver([FromBody] FindDriverInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.CreateOrderDriver(cusID.Value, input.LatiFrom, input.LongiFrom, input.AddressFrom, input.LatiTo, input.LongiTo
                    , input.AddressTo, input.VehicleID, input.CouponID.GetValueOrDefault(), input.Point.GetValueOrDefault(), input.PaymentMethod);
                if (model == SystemParam.BOOK_DRIVER_FAIL)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.AREA_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.AREA_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.POINT_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.POINT_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.WALLET_CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.LOCATION_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_EMPTY_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_REMAIN)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_REMAIN_STR, "");
                }
                else if (model == SystemParam.COUPON_EXCEED_VALUE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_EXCEED_VALUE_STR, "");
                }
                else if (model == SystemParam.ORDER_TRANSPORT_DRIVER_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_TRANSPORT_DRIVER_NOT_VALID_STR, "");
                }
                if (input.PaymentMethod != SystemParam.PAYMENT_TYPE_VN_PAY)
                {
                    bookBus.FindDriver(model);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_DRIVER_SUCCESS_STR, model);
                }
                else
                {
                    var TransactionID = bookBus.CreateTransactionVnPay(model);
                    if (TransactionID == SystemParam.ORDER_NOT_FOUND)
                    {
                        return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                    }
                    var vnPayModel = new OrderVNPayOutputModel
                    {
                        ID = model,
                        url = vnPayBus.GetUrl(TransactionID)
                    };

                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_DRIVER_SUCCESS_STR, vnPayModel);
                }

            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Tìm tài xế giao hàng
        [HttpPost]
        public JsonModel FindDriverPackage()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var httpRequest = HttpContext.Current.Request;
                var data = httpRequest.Form;
                FindDriverPackageInputModel input = new FindDriverPackageInputModel
                {
                    LatiFrom = data.GetValues("LatiFrom").FirstOrDefault(),
                    LongiFrom = data.GetValues("LongiFrom").FirstOrDefault(),
                    AddressFrom = data.GetValues("AddressFrom").FirstOrDefault(),
                    LatiTo = data.GetValues("LatiTo").FirstOrDefault(),
                    LongiTo = data.GetValues("LongiTo").FirstOrDefault(),
                    AddressTo = data.GetValues("AddressTo").FirstOrDefault(),
                    AddressToDetail = data.GetValues("AddressToDetail").FirstOrDefault(),
                    SenderName = data.GetValues("SenderName").FirstOrDefault(),
                    SenderPhone = data.GetValues("SenderPhone").FirstOrDefault(),
                    ReceiverName = data.GetValues("ReceiverName").FirstOrDefault(),
                    ReceiverPhone = data.GetValues("ReceiverPhone").FirstOrDefault(),
                    CouponID = Util.ParseInt(data.GetValues("CouponID").FirstOrDefault(), 0).GetValueOrDefault(),
                    Point = Util.ParseInt(data.GetValues("Point").FirstOrDefault(), 0).GetValueOrDefault(),
                    PaymentMethod = Util.ParseInt(data.GetValues("PaymentMethod").FirstOrDefault(), 1).GetValueOrDefault(),
                    TransportType = Util.ParseInt(data.GetValues("TransportType").FirstOrDefault(), 1).GetValueOrDefault(),
                    CODFee = Util.ParseInt(data.GetValues("CODFee").FirstOrDefault(), 0).GetValueOrDefault(),
                    PackageType = data.GetValues("PackageType").FirstOrDefault(),
                    Weight = Util.ParseDouble(data.GetValues("Weight").FirstOrDefault(), 0).GetValueOrDefault(),
                    PackageFee = Util.ParseInt(data.GetValues("PackageFee").FirstOrDefault(), 0).GetValueOrDefault(),
                    Note = data.GetValues("Note").FirstOrDefault(),
                    IsPaymentReceiver = Util.ParseInt(data.GetValues("IsPaymentReceiver").FirstOrDefault()).GetValueOrDefault(),
                    Images = Util.UploadFile("images")
                };
                var model = bookBus.CreateOrderPackage(cusID.Value, input.LatiFrom, input.LongiFrom, input.AddressFrom, input.LatiTo, input.LongiTo,
                    input.AddressTo, input.AddressToDetail, input.SenderName, input.SenderPhone, input.ReceiverName, input.ReceiverPhone,
                    input.CouponID.GetValueOrDefault(), input.Point.GetValueOrDefault(), input.PaymentMethod, input.TransportType
                    , input.CODFee, input.PackageType, input.Weight, input.PackageFee, input.Note, input.IsPaymentReceiver, input.Images);
                if (model == SystemParam.BOOK_DRIVER_FAIL)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.AREA_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.AREA_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.POINT_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.POINT_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.WALLET_CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.COD_FEE_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COD_FEE_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.LOCATION_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_EMPTY_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_REMAIN)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_REMAIN_STR, "");
                }
                else if (model == SystemParam.COUPON_EXCEED_VALUE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_EXCEED_VALUE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_COMPLETE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_COMPLETE_STR, "");
                }
                else if (model == SystemParam.ORDER_TRANSPORT_PACKAGE_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_TRANSPORT_PACKAGE_NOT_VALID_STR, "");
                }
                if (input.PaymentMethod != SystemParam.PAYMENT_TYPE_VN_PAY)
                {

                    bookBus.FindDriver(model);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_PACKAGE_SUCCESS_STR, model);
                }
                else
                {
                    var TransactionID = bookBus.CreateTransactionVnPay(model);
                    if (TransactionID == SystemParam.ORDER_NOT_FOUND)
                    {
                        return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                    }
                    var vnPayModel = new OrderVNPayOutputModel
                    {
                        ID = model,
                        url = vnPayBus.GetUrl(TransactionID)
                    };
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_PACKAGE_SUCCESS_STR, vnPayModel);
                }
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Tìm tài xế giao hàng vip
        [HttpPost]
        public JsonModel FindDriverPackageVIP()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var httpRequest = HttpContext.Current.Request;
                var data = httpRequest.Form;
                FindDriverPackageInputModel input = new FindDriverPackageInputModel
                {
                    LatiFrom = data.GetValues("LatiFrom").FirstOrDefault(),
                    LongiFrom = data.GetValues("LongiFrom").FirstOrDefault(),
                    AddressFrom = data.GetValues("AddressFrom").FirstOrDefault(),
                    LatiTo = data.GetValues("LatiTo").FirstOrDefault(),
                    LongiTo = data.GetValues("LongiTo").FirstOrDefault(),
                    AddressTo = data.GetValues("AddressTo").FirstOrDefault(),
                    AddressToDetail = data.GetValues("AddressToDetail").FirstOrDefault(),
                    SenderName = data.GetValues("SenderName").FirstOrDefault(),
                    SenderPhone = data.GetValues("SenderPhone").FirstOrDefault(),
                    ReceiverName = data.GetValues("ReceiverName").FirstOrDefault(),
                    ReceiverPhone = data.GetValues("ReceiverPhone").FirstOrDefault(),
                    DistrictID = Util.ParseInt(data.GetValues("DistrictID").FirstOrDefault(), 0).GetValueOrDefault(),
                    ProvinceID = Util.ParseInt(data.GetValues("ProvinceID").FirstOrDefault(), 0).GetValueOrDefault(),
                    Point = Util.ParseInt(data.GetValues("Point").FirstOrDefault(), 0).GetValueOrDefault(),
                    PaymentMethod = Util.ParseInt(data.GetValues("PaymentMethod").FirstOrDefault(), 1).GetValueOrDefault(),
                    TransportType = Util.ParseInt(data.GetValues("TransportType").FirstOrDefault(), 1).GetValueOrDefault(),
                    CODFee = Util.ParseInt(data.GetValues("CODFee").FirstOrDefault(), 0).GetValueOrDefault(),
                    PackageType = data.GetValues("PackageType").FirstOrDefault(),
                    Weight = Util.ParseDouble(data.GetValues("Weight").FirstOrDefault(), 0).GetValueOrDefault(),
                    PackageFee = Util.ParseInt(data.GetValues("PackageFee").FirstOrDefault(), 0).GetValueOrDefault(),
                    Width = Util.ParseInt(data.GetValues("Width").FirstOrDefault(), 0).GetValueOrDefault(),
                    Length = Util.ParseInt(data.GetValues("Length").FirstOrDefault(), 0).GetValueOrDefault(),
                    Height = Util.ParseInt(data.GetValues("Height").FirstOrDefault(), 0).GetValueOrDefault(),
                    Note = data.GetValues("Note").FirstOrDefault(),
                    IsPaymentReceiver = Util.ParseInt(data.GetValues("IsPaymentReceiver").FirstOrDefault()).GetValueOrDefault(),
                    Images = Util.UploadFile("images")
                };
                var model = bookBus.CreateOrderPackageVIP(cusID.Value, input.LatiFrom, input.LongiFrom, input.AddressFrom, input.LatiTo, input.LongiTo,
    input.AddressTo, input.AddressToDetail, input.SenderName, input.SenderPhone, input.ReceiverName, input.ReceiverPhone, input.Point.GetValueOrDefault(), input.PaymentMethod, input.TransportType
    , input.CODFee, input.PackageType, input.Weight, input.PackageFee, input.Note, input.IsPaymentReceiver, input.Images,input.Width,input.Length,input.Height,input.ProvinceID,input.DistrictID);
                if (model == SystemParam.BOOK_DRIVER_FAIL)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.AREA_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.AREA_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.POINT_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.POINT_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.WALLET_CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.COD_FEE_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COD_FEE_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.LOCATION_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_EMPTY_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_REMAIN)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_REMAIN_STR, "");
                }
                else if (model == SystemParam.COUPON_EXCEED_VALUE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_EXCEED_VALUE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_COMPLETE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_COMPLETE_STR, "");
                }
                if (input.PaymentMethod != SystemParam.PAYMENT_TYPE_VN_PAY)
                {

                    bookBus.FindDriver(model);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_PACKAGE_SUCCESS_STR, model);
                }
                else
                {
                    var TransactionID = bookBus.CreateTransactionVnPay(model);
                    if (TransactionID == SystemParam.ORDER_NOT_FOUND)
                    {
                        return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                    }
                    var vnPayModel = new OrderVNPayOutputModel
                    {
                        ID = model,
                        url = vnPayBus.GetUrl(TransactionID)
                    };
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_PACKAGE_SUCCESS_STR, vnPayModel);
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Cập nhật đơn giao hàng vip
        [HttpPost]
        public JsonModel UpdateOrderPackageVIP()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var httpRequest = HttpContext.Current.Request;
                var data = httpRequest.Form;
                FindDriverPackageInputModel input = new FindDriverPackageInputModel
                {
                    OrderID = Util.ParseInt(data.GetValues("OrderID").FirstOrDefault(), 0).GetValueOrDefault(),
                    LatiFrom = data.GetValues("LatiFrom").FirstOrDefault(),
                    LongiFrom = data.GetValues("LongiFrom").FirstOrDefault(),
                    AddressFrom = data.GetValues("AddressFrom").FirstOrDefault(),
                    LatiTo = data.GetValues("LatiTo").FirstOrDefault(),
                    LongiTo = data.GetValues("LongiTo").FirstOrDefault(),
                    AddressTo = data.GetValues("AddressTo").FirstOrDefault(),
                    AddressToDetail = data.GetValues("AddressToDetail").FirstOrDefault(),
                    SenderName = data.GetValues("SenderName").FirstOrDefault(),
                    SenderPhone = data.GetValues("SenderPhone").FirstOrDefault(),
                    ReceiverName = data.GetValues("ReceiverName").FirstOrDefault(),
                    ReceiverPhone = data.GetValues("ReceiverPhone").FirstOrDefault(),
                    CouponID = Util.ParseInt(data.GetValues("CouponID").FirstOrDefault(), 0).GetValueOrDefault(),
                    DistrictID = Util.ParseInt(data.GetValues("DistrictID").FirstOrDefault(), 0).GetValueOrDefault(),
                    ProvinceID = Util.ParseInt(data.GetValues("ProvinceID").FirstOrDefault(), 0).GetValueOrDefault(),
                    Point = Util.ParseInt(data.GetValues("Point").FirstOrDefault(), 0).GetValueOrDefault(),
                    PaymentMethod = Util.ParseInt(data.GetValues("PaymentMethod").FirstOrDefault(), 1).GetValueOrDefault(),
                    TransportType = Util.ParseInt(data.GetValues("TransportType").FirstOrDefault(), 1).GetValueOrDefault(),
                    CODFee = Util.ParseInt(data.GetValues("CODFee").FirstOrDefault(), 0).GetValueOrDefault(),
                    PackageType = data.GetValues("PackageType").FirstOrDefault(),
                    Weight = Util.ParseDouble(data.GetValues("Weight").FirstOrDefault(), 0).GetValueOrDefault(),
                    PackageFee = Util.ParseInt(data.GetValues("PackageFee").FirstOrDefault(), 0).GetValueOrDefault(),
                    Width = Util.ParseInt(data.GetValues("Width").FirstOrDefault(), 0).GetValueOrDefault(),
                    Length = Util.ParseInt(data.GetValues("Length").FirstOrDefault(), 0).GetValueOrDefault(),
                    Height = Util.ParseInt(data.GetValues("Height").FirstOrDefault(), 0).GetValueOrDefault(),
                    Note = data.GetValues("Note").FirstOrDefault(),
                    IsPaymentReceiver = Util.ParseInt(data.GetValues("IsPaymentReceiver").FirstOrDefault()).GetValueOrDefault(),
                    Images = Util.UploadFile("images")
                };
                var model = bookBus.UpdateOrderPackageVIP(input.OrderID,input.LatiTo, input.LongiTo,input.AddressTo, input.AddressToDetail, input.ReceiverName, input.ReceiverPhone,input.TransportType
             , input.CODFee, input.PackageType, input.Weight, input.PackageFee, input.Note, input.IsPaymentReceiver, input.Images, input.Width, input.Length, input.Height, input.ProvinceID, input.DistrictID);
                if (model == SystemParam.BOOK_DRIVER_FAIL)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.AREA_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.AREA_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.COD_FEE_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COD_FEE_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.LOCATION_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_EMPTY_STR, "");
                }
                var order = bookBus.GetOrderServicePackageVIPDriver(model);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_PACKAGE_SUCCESS_STR, order);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Tìm tài xế giao đồ ăn
        [HttpPost]
        public JsonModel FindDriverFood([FromBody] FindDriverFoodInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.CreateOrderFood(cusID.Value, input.LatiTo, input.LongiTo, input.AddressTo, input.ShopID,
                input.CouponID.GetValueOrDefault(), input.Point.GetValueOrDefault(), input.PaymentMethod);
                if (model == SystemParam.BOOK_DRIVER_FAIL)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.AREA_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.AREA_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.POINT_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.POINT_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.SHOP_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHOP_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.CART_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CART_EMPTY_STR, "");
                }
                else if (model == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.WALLET_CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.SHOP_LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHOP_LOCATION_NOT_VALID_STR, "");
                }
                else if (model == SystemParam.LOCATION_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_EMPTY_STR, "");
                }
                else if (model == SystemParam.COUPON_NOT_REMAIN)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_REMAIN_STR, "");
                }
                else if (model == SystemParam.COUPON_EXCEED_VALUE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_EXCEED_VALUE_STR, "");
                }
                else if (model == SystemParam.ORDER_TRANSPORT_FOOD_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_TRANSPORT_FOOD_NOT_VALID_STR, "");
                }
                if (input.PaymentMethod != SystemParam.PAYMENT_TYPE_VN_PAY)
                {
                    bookBus.FindDriver(model);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_FOOD_SUCCESS_STR, model);
                }
                else
                {
                    var TransactionID = bookBus.CreateTransactionVnPay(model);
                    if (TransactionID == SystemParam.ORDER_NOT_FOUND)
                    {
                        return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                    }
                    var vnPayModel = new OrderVNPayOutputModel
                    {
                        ID = model,
                        url = vnPayBus.GetUrl(TransactionID)
                    };
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.BOOK_FOOD_SUCCESS_STR, vnPayModel);
                }
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        [HttpGet]
        public async Task<JsonModel> TestAPI()
        {
            try
            {
                await bookBus.UpdateLocationDriverProcedureTest();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, "");
            }
            catch(Exception e)
            {
                return serverError();
            }
        }
        // Áp dụng mã giảm giá
        [HttpPost]
        public JsonModel ApplyCoupon([FromBody] CouponInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.CheckApplyCoupon(input.CouponCode, cusID.Value);
                if (check == SystemParam.COUPON_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.COUPON_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_NOT_VALID_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.COUPON_APPLY_ERROR_STR, "");
                }
                var model = bookBus.GetCouponDetail(check);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Tiếp nhận khách hàng đặt xe
        [HttpPost]
        public JsonModel AcceptOrderService(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var Accept = bookBus.AcceptOrderService(OrderServiceID, shipperID.Value);
                if (Accept == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPER_ACCEPT_SERVICE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPER_ACCEPT_SERVICE_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_ENOUGH_MONEY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_ENOUGH_MONEY_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_PICK_UP_STR, "");
                }
                else if (Accept == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else
                {
                    var model = bookBus.GetOrderServiceDriver(OrderServiceID);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
                }

            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Tiếp nhận giao hàng
        [HttpPost]
        public JsonModel AcceptOrderServicePackage(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var Accept = bookBus.AcceptOrderService(OrderServiceID, shipperID.Value);
                if (Accept == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPER_ACCEPT_SERVICE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPER_ACCEPT_SERVICE_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_ENOUGH_MONEY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_ENOUGH_MONEY_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_PICK_UP_STR, "");
                }
                else if (Accept == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else
                {
                    var model = bookBus.GetOrderServicePackageDriver(OrderServiceID);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
                }

            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Tiếp nhận giao hàng
        [HttpPost]
        public JsonModel AcceptOrderServicePackageVIP(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var Accept = bookBus.AcceptOrderService(OrderServiceID, shipperID.Value);
                if (Accept == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPER_ACCEPT_SERVICE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPER_ACCEPT_SERVICE_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_ENOUGH_MONEY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_ENOUGH_MONEY_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_PICK_UP_STR, "");
                }
                else if (Accept == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else
                {
                    var model = bookBus.GetOrderServicePackageVIPDriver(OrderServiceID);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
                }

            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Tiếp nhận giao đồ ăn
        [HttpPost]
        public JsonModel AcceptOrderServiceFood(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var Accept = bookBus.AcceptOrderService(OrderServiceID, shipperID.Value);
                if (Accept == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.SHIPER_ACCEPT_SERVICE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPER_ACCEPT_SERVICE_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (Accept == SystemParam.WALLET_NOT_ENOUGH_MONEY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_ENOUGH_MONEY_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_DECLINE_STR, "");
                }
                else if (Accept == SystemParam.BOOK_DRIVER_ALREADY_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_PICK_UP_STR, "");
                }
                else if (Accept == SystemParam.LOCATION_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.LOCATION_NOT_VALID_STR, "");
                }
                else
                {
                    var model = bookBus.GetOrderServiceFoodDriver(OrderServiceID);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
                }

            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Từ chối tiếp nhận khách hàng đặt xe , giao hàng , giao đồ ăn
        [HttpPost]
        public JsonModel DeclineOrderService(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.DeclineOrderService(OrderServiceId, shipperID.Value);
                if (model == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.DECLINE_BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.BOOK_DRIVER_ALREADY_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_ALREADY_PICK_UP_STR, "");
                }
                else if (model == SystemParam.BOOK_DRIVER_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_DECLINE_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Khách hàng hủy chuyến lần 1
        [HttpPost]
        public JsonModel DeclineOrderServiceFirst(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.DeclineOrderServiceFirst(OrderServiceId, cusID.Value);
                if (model == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.DECLINE_BOOK_DRIVER_FAIL_STR, "");
                }
                else if (model == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.ORDER_CUSTOMER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_CUSTOMER_NO_PERMISSION_STR, "");
                }
                else if (model == SystemParam.ORDER_NOT_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_DECLINE_STR, "");
                }
                else if (model == SystemParam.CUSTOMER_CAN_NOT_CANCLE_ORDER)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_CAN_NOT_CANCLE_ORDER_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_DECLINE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_COMPLETE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_COMPLETE_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.DECLINE_BOOK_DRIVER_SUCCESS, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // khách hàng hủy chuyến lần 2
        [HttpPost]
        public JsonModel DeclineOrderServiceSecond([FromBody] DeclineOrderServiceInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.DeclineOrderServiceSecond(input.OrderServiceID, cusID.Value, input.Reason);
                if (model == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.BOOK_DRIVER_NOT_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_NOT_PICK_UP_STR, "");
                }
                else if (model == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.ORDER_CUSTOMER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_CUSTOMER_NO_PERMISSION_STR, "");
                }
                else if (model == SystemParam.ORDER_NOT_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_DECLINE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_DECLINE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_COMPLETE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_COMPLETE_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.DECLINE_BOOK_DRIVER_SUCCESS, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Hủy đơn đặt xe , giao hàng , giao đồ ăn
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonModel NotPickUpCustomer([FromBody] DeclineOrderServiceInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.NotPickUpCustomer(input.OrderServiceID, shipperID.Value, input.Reason);
                if (model == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.BOOK_DRIVER_NOT_PICK_UP)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_NOT_PICK_UP_STR, "");
                }
                else if (model == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                else if (model == SystemParam.ORDER_NOT_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_DECLINE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_DECLINE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_COMPLETE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_COMPLETE_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.DECLINE_BOOK_DRIVER_SUCCESS, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Đón khách
        [HttpPost]
        public JsonModel PickUpCustomer(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.PickUpCustomer(OrderServiceId, shipperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                var model = bookBus.GetOrderServiceDriver(OrderServiceId);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy hàng
        [HttpPost]
        public JsonModel PickUpPackage(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.PickUpCustomer(OrderServiceId, shipperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                var model = bookBus.GetOrderServicePackageDriver(OrderServiceId);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy hàng VIP
        [HttpPost]
        public JsonModel PickUpPackageVIP(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.PickUpCustomer(OrderServiceId, shipperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                var model = bookBus.GetOrderServicePackageVIPDriver(OrderServiceId);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy đồ ăn
        [HttpPost]
        public JsonModel PickUpFood(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.PickUpCustomer(OrderServiceId, shipperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.BOOK_DRIVER_FAIL_STR, "");
                }
                var model = bookBus.GetOrderServiceFoodDriver(OrderServiceId);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Hoàn thành đơn đặt xe
        [HttpPost]
        public JsonModel CompleteOrderService(int OrderServiceId)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.CompleteOrderService(OrderServiceId, shipperID.Value);
                if (model == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.WALLET_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.WALLET_CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.WALLET_CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.CUSTOMER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CUSTOMER_NOT_FOUND_STR, "");
                }
                else if (model == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_DECLINE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_DECLINE_STR, "");
                }
                else if (model == SystemParam.ORDER_ALREADY_COMPLETE)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_ALREADY_COMPLETE_STR, "");
                }
                else if (model == SystemParam.ERROR)
                {
                    return serverError();
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, OrderServiceId);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Lấy thông tin đơn đặt xe 
        [HttpGet]
        public JsonModel GetBookingDriverDetail(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetBookingDriverDetail(OrderServiceID, shipperID.Value);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn giao hàng
        [HttpGet]
        public JsonModel GetBookingPackageDetail(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetBookingPackageDetail(OrderServiceID, shipperID.Value);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn giao hàng
        [HttpGet]
        public JsonModel GetBookingPackageVIPDetail(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetBookingPackageVIPDetail(OrderServiceID, shipperID.Value);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn giao đồ ăn
        [HttpGet]
        public JsonModel GetBookingFoodDetail(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shipperID = Util.checkTokenApp(token);
                if (shipperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetBookingFoodDetail(OrderServiceID, shipperID.Value);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn đặt xe khi tiếp nhận cho tài xế
        [HttpGet]
        public JsonModel GetOrderServiceDriver(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shiperID = Util.checkTokenApp(token);
                if (shiperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.CheckGetOrderServiceDriver(OrderServiceID, shiperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                var model = bookBus.GetOrderServiceDriver(OrderServiceID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Lấy thông tin đơn giao hàng khi tiếp nhận cho tài xế
        [HttpGet]
        public JsonModel GetOrderServicePackageDriver(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shiperID = Util.checkTokenApp(token);
                if (shiperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.CheckGetOrderServiceDriver(OrderServiceID, shiperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                var model = bookBus.GetOrderServicePackageDriver(OrderServiceID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Lấy thông tin đơn giao hàng khi tiếp nhận cho tài xế
        [HttpGet]
        public JsonModel GetOrderServicePackageVIPDriver(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shiperID = Util.checkTokenApp(token);
                if (shiperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.CheckGetOrderServiceDriver(OrderServiceID, shiperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                var model = bookBus.GetOrderServicePackageVIPDriver(OrderServiceID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn giao đồ ăn khi tiếp nhận cho tài xế
        [HttpGet]
        public JsonModel GetOrderServiceFoodDriver(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shiperID = Util.checkTokenApp(token);
                if (shiperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.CheckGetOrderServiceDriver(OrderServiceID, shiperID.Value);
                if (check == SystemParam.ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ORDER_DRIVER_NO_PERMISSION)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_DRIVER_NO_PERMISSION_STR, "");
                }
                var model = bookBus.GetOrderServiceFoodDriver(OrderServiceID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn đặt xe khi tiếp nhận cho khách hàng
        [HttpGet]
        public JsonModel GetOrderServiceCustomer(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetOrderServiceCustomer(OrderServiceID);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Lấy thông tin đơn đặt giao hàng khi tiếp nhận cho khách hàng
        [HttpGet]
        public JsonModel GetOrderServicePackageCustomer(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetOrderServicePackageCustomer(OrderServiceID);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn đặt giao hàng khi tiếp nhận cho khách hàng
        [HttpGet]
        public JsonModel GetOrderServicePackageVIPCustomer(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetOrderServicePackageCustomer(OrderServiceID);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy thông tin đơn đặt giao đồ ăn khi tiếp nhận cho khách hàng
        [HttpGet]
        public JsonModel GetOrderServiceFoodCustomer(int OrderServiceID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = bookBus.GetOrderServiceFoodCustomer(OrderServiceID);
                if (model == null)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ORDER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // Cập nhật vị trí tài xế
        [HttpPost]
        public JsonModel UpdateShiperLocation([FromBody] ShiperLocationInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? shiperID = Util.checkTokenApp(token);
                if (shiperID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = bookBus.UpdateShiperLocation(shiperID.Value, input.lati, input.longi);
                if (check == SystemParam.SHIPPER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.SHIPPER_NOT_FOUND_STR, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Lấy danh sách giỏ hàng trong shop
        [HttpGet]
        public JsonModel GetShopCart(int ShopID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var model = cartBus.GetCartDetailOutputs(ShopID, cusID.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Thêm vào giỏ hàng
        [HttpPost]
        public JsonModel AddCart([FromBody] AddCartInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = cartBus.AddCart(cusID.Value, input.ServicePriceID, input.TopingID, input.Quantity);
                if (check == SystemParam.ADD_CART_INVALID_QUANTITY)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ADD_CART_INVALID_QUANTITY_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ADD_CART_FAIL_STR, "");
                }
                else if (check == SystemParam.TOPING_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.TOPING_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ITEM_CHANGED)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ITEM_CHANGED_STR, "");
                }
                var model = cartBus.GetCartByID(check);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Cập nhật số lượng giỏ hàng
        [HttpPost]
        public JsonModel UpdateCartQuantity([FromBody] UpdateCartQuantityInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = cartBus.UpdateCartQuantity(input.CartID, input.Quantity, cusID.Value);
                if (check == SystemParam.CART_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CART_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.CART_CHANGED)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CART_CHANGED_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ADD_CART_FAIL_STR, "");
                }
                var model = cartBus.GetCartByID(check);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Cập nhật ghi chú giỏ hàng
        [HttpPost]
        public JsonModel UpdateCartNote([FromBody] UpdateCartNoteInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = cartBus.UpdateCartNote(input.CartID, input.Note);
                if (check == SystemParam.CART_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.CART_NOT_FOUND_STR, "");
                }
                else if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.ADD_CART_FAIL_STR, "");
                }
                var model = cartBus.GetCartByID(check);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, model);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }
        // Xóa giỏ hàng
        [HttpPost]
        public JsonModel RemoveCart()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_TOKEN_NOTFOUND, SystemParam.TOKEN_INVALID, "");
                var check = cartBus.RemoveCart(cusID.Value);
                if (check == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, "Xóa giỏ hàng thất bại", "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }



    }
}