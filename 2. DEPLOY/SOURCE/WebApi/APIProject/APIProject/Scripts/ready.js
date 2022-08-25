
$(document).ready(function () {

    GetSessionLogin();

    FocusTabMenu();

    //$('.date').datepicker({
    //    dateFormat: "dd/mm/yy"
    //});
    $('.date').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: 'c-70:c'
    });

    $(document).on("wheel", "input[type=number]", function (e) {
        $(this).blur();
    });


}); //end document.ready


//
function FocusTabMenu() {

    var url = window.location.pathname;

    switch (url) {
        case "/Home/Index":
            $('#tabHome').addClass('active');
            break;
        case "/Customer/Index":
            $('#tabWasher').addClass('active');
            break;
        case "/Shipper/Index":
            $('#tabShipper').addClass('active');
            break;
        case "/Shop/Index":
            $('#tabProduct').addClass('active');
            break;
        case "/ServiceCategory/Index":
            $('#tabServiceCategory').addClass('active');
            break;
        case "/Transaction/Index":
            $("#ulTransaction").attr("aria-expanded", "true");
            $("#ulTransaction").addClass("collapse in");
            $('#tabTransaction').addClass('active');
            $('#tabBookCar').addClass('active');
            break;
        case "/TransactionFood/Index":
            $("#ulTransaction").attr("aria-expanded", "true");
            $("#ulTransaction").addClass("collapse in");
            $('#tabTransaction').addClass('active');
            $('#tabFood').addClass('active');
            break;
        case "/TransactionDelivery/Index":
            $("#ulTransaction").attr("aria-expanded", "true");
            $("#ulTransaction").addClass("collapse in");
            $('#tabTransaction').addClass('active');
            $('#tabDelivery').addClass('active');
            break;
        case "/TransactionDeliveryNation/Index":
            $("#ulTransaction").attr("aria-expanded", "true");
            $("#ulTransaction").addClass("collapse in");
            $('#tabTransaction').addClass('active');
            $('#tabDeliveryNation').addClass('active');
            break;
        case "/Promocode/Index":
            $('#tabPromocode').addClass('active');
            break;
        case "/WithdrawalRequest/Index":
            $('#tabWithdrawalRequest').addClass('active');
            break;
        case "/Wallet/Index":
            $('#tabGrade').addClass('active');
            break;
        case "/Vehicle/Chat":
            $('#tabVehicle').addClass('active');
            break;
        case "/Notification/Index":
            $('#tabNotify').addClass('active');
            break;
        case "/StatisticCustomer/Index":
            $("#ulStatistic").attr("aria-expanded", "true");
            $("#ulStatistic").addClass("collapse in");
            $('#tabStatistic').addClass('active');
            $('#tabStatisticCustomer').addClass('active');
            break;
        case "/StatisticWasher/Index":
            $("#ulStatistic").attr("aria-expanded", "true");
            $("#ulStatistic").addClass("collapse in");
            $('#tabStatistic').addClass('active');
            $('#tabStatisticWasher').addClass('active');
            break;
        case "/StatisticPayment/Index":
            $("#ulStatistic").attr("aria-expanded", "true");
            $("#ulStatistic").addClass("collapse in");
            $('#tabStatistic').addClass('active');
            $('#tabStatisticPayment').addClass('active');
            break;
        //case "/StatisticGift/Index":
        //    $("#ulStatistic").attr("aria-expanded", "true");
        //    $("#ulStatistic").addClass("collapse in");
        //    $('#tabStatistic').addClass('active');
        //    $('#tabStatisticGift').addClass('active');
        //    break;
        //case "/StatisticPoit/Index":
        //    $("#ulStatistic").attr("aria-expanded", "true");
        //    $("#ulStatistic").addClass("collapse in");
        //    $('#tabStatistic').addClass('active');
        //    $('#tabStatisticPoit').addClass('active');
        //    break;
        //case "/StatisticRevenue/Index":
        //    $("#ulStatistic").attr("aria-expanded", "true");
        //    $("#ulStatistic").addClass("collapse in");
        //    $('#tabStatistic').addClass('active');
        //    $('#tabStatisticRevenue').addClass('active');
        //    break;
        case "/News/Index":
            $('#tabNews').addClass('active');
            break;
        case "/Complain/Index":
            $('#tabRank').addClass('active');
            break;
        case "/Config/Index":
            $('#tabConfig').addClass('active');
            break;
        case "/User/Index":
            $('#tabUser').addClass('active');
            break;
        default:
            break;
    }

}


//lấy thông tin đối tượng vừa đăng nhập
function GetSessionLogin() {

    $.ajax({
        url: '/Home/GetUserLogin',
        type: 'GET',
        success: function (response) {
            var role = response.Role;
            if (role != 1) {
                $("#tabUser").hide();
            }
            //QLDGD
            if (role == 2) {
                //$("#tabHome").hide();
                $("#tabWasher").hide();
                $("#tabServiceCategory").hide();
                $("#tabPromocode").hide();
                $("#tabWithdrawalRequest").hide();
                $("#tabNotify").hide();
                $("#tabNews").hide();
                $("#tabConfig").hide();


                $("#tabStatisticCustomer").hide();
                $("#tabStatisticWasher").hide();
            }
            //Giao dịch viên
            if (role == 3) {
                $("#tabWasher").hide();//khách hàng
                $("#tabServiceCategory").hide();//danh mục sản phẩm

                $("#tabPromocode").hide();//khuyến mại
                $("#tabWithdrawalRequest").hide();//yêu cầu rút tiền
                $("#tabNotify").hide();//thông báo

                    $("#tabStatisticCustomer").hide();//khách hàng
                    $("#tabStatisticWasher").hide();//đối tác

                $("#tabNews").hide();//tin tức
                $("#tabConfig").hide();//cài đặt
            }
            //Kinh doanh
            if (role == 4) {
                $("#tabWasher").hide();//khách hàng
                $("#tabShipper").hide();//tài xế
                $("#tabServiceCategory").hide();//danh mục sản phẩm

                $("#tabPromocode").hide();//khuyến mại
                $("#tabWithdrawalRequest").hide();//yêu cầu rút tiền
                $("#tabGrade").hide();//quản lý ví
                $("#tabNotify").hide();//thông báo

                $("#tabStatistic").hide();//báo cáo
                    $("#tabStatisticCustomer").hide();//khách hàng
                    $("#tabStatisticWasher").hide();//đối tác
                    $("#tabStatisticPayment").hide();//thanh toán

                $("#tabNews").hide();//tin tức
                $("#tabConfig").hide();//cài đặt
            }
            //Marketing
            if (role == 5) {
                $("#tabWasher").hide();//khách hàng
                $("#tabShipper").hide();//tài xế
                $("#tabProduct").hide();//cửa hàng
                $("#tabServiceCategory").hide();//danh mục sản phẩm

                $("#tabTransaction").hide();//giao dịch
                    $("#tabBookCar").hide();//đặt xe
                    $("#tabFood").hide();//đồ ăn
                    $("#tabDelivery").hide();//siêu tốc nội thành
                    $("#tabDeliveryNation").hide();//giao hàng toàn quốc

                $("#tabWithdrawalRequest").hide();//yêu cầu rút tiền
                $("#tabGrade").hide();//quản lý ví

                $("#tabStatistic").hide();//báo cáo
                    $("#tabStatisticCustomer").hide();//khách hàng
                    $("#tabStatisticWasher").hide();//đối tác
                    $("#tabStatisticPayment").hide();//thanh toán

                $("#tabRank").hide();//khiếu nại
                $("#tabConfig").hide();//cài đặt
            }
            //Kế toán
            if (role == 6) {
                $("#tabWasher").hide();//khách hàng
                $("#tabProduct").hide();//cửa hàng
                $("#tabServiceCategory").hide();//danh mục sản phẩm

                $("#tabPromocode").hide();//khuyến mại
                $("#tabNotify").hide();//thông báo

                $("#tabNews").hide();//tin tức
                $("#tabRank").hide();//khiếu nại
                $("#tabConfig").hide();//cài đặt
            }
            //Quản lý kinh doanh
            if (role == 7) {
                $("#tabWasher").hide();//khách hàng
                $("#tabServiceCategory").hide();//danh mục sản phẩm

                $("#tabPromocode").hide();//khuyến mại
                $("#tabWithdrawalRequest").hide();//yêu cầu rút tiền
                $("#tabNotify").hide();//thông báo

                
                $("#tabNews").hide();//tin tức
                $("#tabConfig").hide();//cài đặt
            }
            $('#txtName').text(response.UserName);

            $('#txtPhone').text(response.Phone);
            $('#txtEmail').text(response.Email);
            $('#txtNameAdmin').text(response.UserName);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}
