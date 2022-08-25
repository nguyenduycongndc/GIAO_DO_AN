$(document).ready(function () {
    //check số int

    //tự động chọn option có cùng giá trị
    var typeNews = $("#cbbType").attr("value");
    $("#cbbType option").each(function () {
        if (typeNews == $(this).val()) {
            $(this).attr('selected', 'selected');
        }
    });

    var typeSendNews = $("#cbbTypeSend").attr("value");
    $("#cbbTypeSend option").each(function () {
        if (typeSendNews == $(this).val()) {
            $(this).attr('selected', 'selected');
        }
    });

    //clear text when close modal
    $('.modal').on('hidden.bs.modal', function () {
        $(this).find("input,textarea").val('');
    });

    //change option in Combobox
    $('#status').on("change", function () {
        searchWarrantyCard();
    });

    $('#type').on('change', function () {
        searchPoint();
    });

    $('#itemStatus').on('change', function () {
        SearchItem();
    });

    //auto trim input text
    $('input[type="text"]').change(function () {
        this.value = $.trim(this.value);
    });

    $('#place').on('change', function () {
        LoadPlaceCreateShop();
    });

    //auto format number input
    $('.number').keyup(function () {
        $val = cms_decode_currency_format($(this).val());
        $(this).val(cms_encode_currency_format($val));
    });

    //Chặn dấu đặc biệt trong type number input
    $('.number-type').keydown(function (e) {
        if (e.keyCode == 69 || e.keyCode == 189 || e.keyCode == 188 || e.keyCode == 231 || e.keyCode == 190)
            return false;
    });
    $(window).keypress(function (e) {
        var key = e.which;
        if (key == 13) {
            $(".btn_search").click();
        }
    });
    $('.checkPhone').keyup(function (e) {
        var value = $(this).val();
        if (value.includes("-") || value.includes(".") || value.includes(",")) {
            $(this).val("");
        }
    })
}); //end document.ready


const SUCCESS = 1;
const ERROR = 0;
const DUPLICATE_NAME = 2;
const CAN_NOT_DELETE = 2;
const WRONG_PASSWORD = 2;
const NOT_ADMIN = 3;
const EXISTING = 2;
const FAIL_LOGIN = 2;
const URL_ADD_IMG_DEFAULT = "/Uploads/files/add_img.png";


//đăng nhập
function Login() {

    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var phone = $("#txtUsernameLogin").val();
    var password = $("#txtPasswordLogin").val();
    if (phone == "" || password == "") {
        swal({
            title: "Vui lòng điền đầy đủ thông tin",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Home/UserLogin',
        data: { phone: phone, password: password },
        type: 'POST',
        success: function (response) {
            if (response.Status == SUCCESS) {
                window.location.assign("/Home/Index");
            } else {
                swal({
                    title: response.Message,
                    text: "",
                    icon: "error"
                })
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function logout() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Home/Logout',
        data: {},
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                location.reload();
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

//đổi mật khẩu
function changePassword() {

    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var currentPassword = $.trim($("#txtCurrentPassword").val());
    var newPassword = $.trim($("#txtNewPassword").val());
    var confirmPassword = $.trim($("#txtConfirmPassword").val());

    if (currentPassword == "" || newPassword == "" || confirmPassword == "") {
        swal({
            title: "Vui lòng điền đầ đủ thông tin!",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (newPassword != confirmPassword) {
        $("#txtConfirmPassword").val("");
        swal({
            title: "Xác nhận mật khẩu không đúng",
            text: "",
            icon: "warning"
        })
        return;
    }
    var result = 0;
    $.ajax({
        url: '/User/ChangePassword',
        data: {
            CurrentPass: currentPassword,
            newPass: newPassword
        },
        beforeSend: function () {
            $("#modalLoad").modal('show');
        },
        type: 'POST',
        success: function (response) {
            result = response.Status;
            $("#modalLoad").modal('hide');
            swal({
                title: response.Message,
                icon: response.Status == SUCCESS ? "success" : "error"
                //}).then((sc) => {
                //    if (result == SUCCESS && sc) {
                //        window.location = "/User/Index";
                //    }
            })
        },
        error: function (result) {
            console.log(result.responseText);
            swal({
                title: "error",
                text: "",
                icon: "warning"
            })
        }
    });
}

// start xóa yêu cầu đổi quà
function deleteRequest(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Request/DeleteRequest',
                    data: { RequestID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {

                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/Request/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    RequestCode: $("#txtRequestCodeSearch").val(),
                                    Status: $("#cbbStatus").val(),
                                    Type: $("#cbbType").val(),
                                    FromDate: $("#txtRequestFromDate").val(),
                                    ToDate: $("#txtRequestToDate").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableRequest").html(response);
                                },
                                error: function (result) {
                                    console.log(result.responseText);
                                }
                            });

                        } else
                            if (response == CAN_NOT_DELETE) {
                                swal({
                                    title: "Can not delete!",
                                    text: "Yêu cầu này đã được xử lý.",
                                    icon: "warning"
                                })
                            } else {
                                swal({
                                    title: "Can not delete!",
                                    text: "An error occurred.",
                                    icon: "warning"
                                })
                            }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa yêu cầu đổi quà

// start chấp nhận yêu cầu đổi quà
function acceptRequest(id, customerID, statusRequest) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var note = $.trim($("#noteRequest").val());
    var requestGiftName = $("#requestGiftName").html();

    if (statusRequest == 2) {

        swal({
            title: "Bạn chắc chắn hủy yêu cầu chứ?",
            text: "",
            icon: "warning",
            buttons: ["Cancel", "OK"],
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        url: '/Request/AcceptRequest',
                        data: {
                            //status request: gửi trạng thái xem thực hiện việc xác nhận hay hủy yêu cầu
                            StatusRequest: statusRequest,
                            RequestID: id,
                            CustomerID: customerID,
                            RequestGiftName: requestGiftName,
                            Note: note
                        },
                        type: 'POST',
                        success: function (response) {
                            if (response == SUCCESS) {
                                $('#questDetail').modal('hide');
                                swal({
                                    title: "Đã hủy yêu cầu!",
                                    text: "",
                                    icon: "success"
                                })

                                $.ajax({
                                    url: '/Request/Search',
                                    data: {
                                        Page: $("#txtPageCurrent").val(),
                                        RequestCode: $("#txtRequestCodeSearch").val(),
                                        Status: $("#cbbStatus").val(),
                                        Type: $("#cbbType").val(),
                                        FromDate: $("#txtRequestFromDate").val(),
                                        ToDate: $("#txtRequestToDate").val()
                                    },
                                    type: 'POST',
                                    success: function (response) {
                                        $("#tableRequest").html(response);
                                    },
                                    error: function (result) {
                                        console.log(result.responseText);
                                    }
                                });

                            } else
                                swal({
                                    title: "An error occurred!",
                                    text: "",
                                    icon: "warning"
                                })
                        },
                        error: function (result) {
                            console.log(result.responseText);
                        }
                    });
                }
            })
    } else
        $.ajax({
            url: '/Request/AcceptRequest',
            data: {
                //status request: gửi trạng thái xem thực hiện việc xác nhận hay hủy yêu cầu
                StatusRequest: statusRequest,
                RequestID: id,
                CustomerID: customerID,
                RequestGiftName: requestGiftName,
                Note: note
            },
            type: 'POST',
            success: function (response) {
                if (response == SUCCESS) {
                    $('#questDetail').modal('hide');
                    swal({
                        title: "Successful!",
                        text: "The request has been confirmed",
                        icon: "success"
                    })

                    $.ajax({
                        url: '/Request/Search',
                        data: {
                            Page: $("#txtPageCurrent").val(),
                            RequestCode: $("#txtRequestCodeSearch").val(),
                            Status: $("#cbbStatus").val(),
                            Type: $("#cbbType").val(),
                            FromDate: $("#txtRequestFromDate").val(),
                            ToDate: $("#txtRequestToDate").val()
                        },
                        type: 'POST',
                        success: function (response) {
                            $("#tableRequest").html(response);
                        },
                        error: function (result) {
                            console.log(result.responseText);
                        }
                    });

                } else
                    swal({
                        title: "An error occurred!",
                        text: "",
                        icon: "warning"
                    })
            },
            error: function (result) {
                console.log(result.responseText);
            }
        });
}// end chấp nhận yêu cầu đổi quà

// start thông tin chi tiết 1 người dùng
function updateRole(id) {
    var phone = $('#txtPhoneUserEdit').val().trim();
    var userName = $('#txtNameUserEdit').val().trim();

    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/User/UpdateRole',
        data: { ID: id, Phone: phone, UserName: userName },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#modalRole').modal('hide');
                swal({
                    title: "Chỉnh sửa Successful!",
                    text: "",
                    icon: "success"
                })
                $.ajax({
                    url: '/User/Search',
                    data: {
                        Page: $("#txtPageCurrent").val(),
                        Phone: $("#txtPhoneUser").val(),
                        FromDate: $("#txtFromDateUser").val(),
                        ToDate: $("#txtToDateUser").val()
                    },
                    type: 'POST',
                    success: function (response) {
                        $("#tableUser").html(response);
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });

            } else {
                swal("error", "", "warning");
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 người dùng

function getUserDetail(id) {
    debugger;
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/User/GetUserDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divUserDetail").html(response);
            $('#EditUser').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}
//function getUserDetail(data) {
//    $('#val-id').val($(data).attr('data-id'));
//    $('#txt-name-detail').val($(data).attr('data-name'));
//    $('#txt-email-detail').val($(data).attr('data-email'));
//    $('#txt-phone-detail').val($(data).attr('data-phone'));
//    $('#user-detail').modal('show');
//}

// start thông tin chi tiết 1 yêu cầu đổi quà
function getRequestDetail(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Request/GetRequestDetail',
        data: { RequestID: id },
        type: 'POST',
        success: function (response) {
            $("#divRequestDetail").html(response);
            $('#questDetail').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 yêu cầu đổi quà

// start tìm kiếm yêu cầu đổi quà
function searchRequest() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var requestCode = $.trim($("#txtRequestCodeSearch").val());
    var status = $("#cbbStatus").val();
    var type = $("#cbbType").val();
    var fromDate = $.trim($("#txtRequestFromDate").val());
    var toDate = $.trim($("#txtRequestToDate").val());

    $.ajax({
        url: '/Request/Search',
        data: {
            Page: 1,
            RequestCode: requestCode,
            Status: status,
            Type: type,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            $("#tableRequest").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm yêu cầu đổi quà

// start tìm kiếm tin tức
function searchNews() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var title = $.trim($("#txtTitle").val());
    var createUser = $("#cbbCreateUser").val();
    var type = $("#cbbTypeNews").val();
    var status = $("#cbbStatusNews").val();
    var fromDate = $.trim($("#txtNewsFromDate").val());
    var toDate = $.trim($("#txtNewsToDate").val());
    $.ajax({
        url: '/News/Search',
        data: {
            Page: 1,
            Title: title,
            CreateUserID: createUser,
            Type: type,
            Status: status,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            $("#tableNews").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm tin tức

// start danh sách thiết lập quà hoặc voucher
function searchConfigGift() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: '/Config/SearchConfigGift',
        data: {
            Page: 1
        },
        type: 'POST',
        success: function (response) {
            $("#tableConfigGift").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end danh sách thiết lập quà hoặc voucher

// start tạo user
//function createUser() {
//    if (!navigator.onLine) {
//        swal({
//            title: "Check internet connection!",
//            text: "",
//            icon: "warning"
//        })
//        return;
//    }
//    var phone = $.trim($("#txtPhoneCreateUser").val());
//    var userName = $.trim($("#txtNameCreateUser").val());
//    var password = $('#txtPasswordCreateUser').val().trim();
//    var passwordConfirm = $('#txtPasswordConfirmCreateUser').val().trim();

//    if (phone.length == "" || userName.length == "") {
//        swal({
//            title: "",
//            text: "Please enter full information!",
//            icon: "warning"
//        })
//        return;
//    } else if (password != passwordConfirm) {
//        swal({
//            title: "",
//            text: "Password và Password nhập lại không trùng nhau",
//            icon: "warning"
//        })
//        return;
//    }
//    //} else
//    //    if (!isNumeric(phone)) {
//    //        swal({
//    //            title: "",
//    //            text: "Số điện thoại chỉ được phép nhập số!",
//    //            icon: "warning"
//    //        })
//    //        return;
//    //    }
//    $.ajax({
//        url: '/User/CreateUser',
//        data: {
//            Phone: phone,
//            UserName: userName,
//            Password: password
//        },
//        type: 'POST',
//        success: function (response) {
//            if (response == SUCCESS) {
//                $('#createUser').modal('hide');
//                swal({
//                    title: "Tạo tài khoản Successful!",
//                    text: "Mật khẩu mặc định là Tài khoản.",
//                    icon: "success"
//                })

//                $.ajax({
//                    url: '/User/Search',
//                    data: {
//                        Page: $("#txtPageCurrent").val(),
//                        Phone: $("#txtPhoneUser").val(),
//                        FromDate: $("#txtFromDateUser").val(),
//                        ToDate: $("#txtToDateUser").val()
//                    },
//                    type: 'POST',
//                    success: function (response) {
//                        $("#tableUser").html(response);
//                    },
//                    error: function (result) {
//                        console.log(result.responseText);
//                    }
//                });

//            } else
//                if (response == EXISTING) {
//                    swal({
//                        title: "Không thể tạo tài khoản!",
//                        text: "Tài khoản đã tồn tại. Vui lòng sử dụng Tài khoản khác.",
//                        icon: "warning"
//                    })
//                    $("#createUser #txtPhoneCreateUser").val("");
//                } else
//                    if (response == NOT_ADMIN) {
//                        $('#createUser').modal('hide');
//                        swal({
//                            title: "Bạn không có quyền tạo tài khoản.",
//                            text: "",
//                            icon: "warning"
//                        })
//                    } else {
//                        swal({
//                            title: "An error occurred!",
//                            text: "",
//                            icon: "warning"
//                        })
//                    }
//        },
//        error: function (result) {
//            console.log(result.responseText);
//        }
//    });
//}// end tạo user

//lưu config birthday 
function SaveConfigBirthday() {
    var Before = $("#beforeBirthday").val().trim().replace(/,/g, "");
    var After = $("#afterBirthday").val().trim().replace(/,/g, "");
    if (Before.length <= 0 || After.length <= 0) {
        swal({
            title: "Please complete the information ",
            icon: "warning"
        })
        return;
    }
    if (Before > 15 || Before < 0 || After > 15 || After < 0) {
        swal({
            title: "Before birthday and After birthday must be greater than 0 and less than 15 ",
            icon: "warning"
        })
        return;
    }

    var item = {
        Before: Before,
        After: After
    };
    $.ajax({
        url: "/Config/UpdateConfigBirthDay",
        type: "POST",
        data: {
            item: item
        }, beforeSend: function () {
            $("#modalLoad").modal("show");
        }, success: function (res) {
            $("#modalLoad").modal("hide");
            if (res.Status) {
                swal({
                    title: "Success!",
                    text: "Update config successfully !",
                    icon: "success"
                })
            }
            else {
                swal({
                    title: "Faild!",
                    text: "Update config faild !",
                    icon: "error"
                })
            }
        }

    })
}


//lưu config exprid
function SaveConfigRankUp() {
    var Expired = $("#txtRankUpExpired").val().trim().replace(/,/g, "");
    if (Expired < 0 || Expired > 15) {
        swal({
            title: "Expired time must be greater than 0 and less than 15 ",
            icon: "warning"
        })
        return;
    }
    if (Expired.length <= 0) {
        swal({
            title: "Please complete the information ",
            icon: "warning"
        })
        return;
    }
    var item = {
        Expired: Expired
    };
    $.ajax({
        url: "/Config/UpdateConfigRankup",
        type: "POST",
        data: {
            item: item
        }, beforeSend: function () {
            $("#modalLoad").modal("show");
        }, success: function (res) {
            $("#modalLoad").modal("hide");
            if (res.Status) {
                swal({
                    title: "Success!",
                    text: "Update config successfully !",
                    icon: "success"
                })
            }
            else {
                swal({
                    title: "Faild!",
                    text: "Update config faild !",
                    icon: "error"
                })
            }
        }

    })
}
function createUser() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }

    var phone = $.trim($("#txtPhoneCreateUser").val());
    var userName = $.trim($("#txtNameCreateUser").val());
    var password = $('#txtPasswordCreateUser').val().trim();
    var passwordConfirm = $('#txtPasswordConfirmCreateUser').val().trim();

    if (phone.length == "" || userName.length == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    } else if (password != passwordConfirm) {
        swal({
            title: "",
            text: "Password và Password nhập lại không trùng nhau",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: '/User/CreateUser',
        data: {
            Phone: phone,
            UserName: userName,
            Password: password
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                $('#createUser').modal('hide');
                swal({
                    title: "Tạo tài khoản Successful!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/User/Search',
                    data: {
                        Page: $("#txtPageCurrent").val(),
                        Phone: $("#txtPhoneUser").val(),
                        FromDate: $("#txtFromDateUser").val(),
                        ToDate: $("#txtToDateUser").val()
                    },
                    type: 'POST',
                    success: function (response) {
                        $("#tableUser").html(response);
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                })
            } else {
                if (response == EXISTING) {
                    swal({
                        title: "Không thể tạo tài khoản!",
                        text: "Tài khoản đã tồn tại. Vui lòng sử dụng Tài khoản khác.",
                        icon: "warning"
                    })
                    $("#createUser #txtPhoneCreateUser").val("");
                } else {
                    if (response == NOT_ADMIN) {
                        $('#createUser').modal('hide');
                        swal({
                            title: "Bạn không có quyền tạo tài khoản.",
                            text: "",
                            icon: "warning"
                        })
                    } else {
                        swal({
                            title: "An error occurred!",
                            text: "",
                            icon: "warning"
                        })
                    }
                }
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    })
}

// start tìm kiếm user
function searchUser() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var key = $("#txt-key-search").val().replace(/\s\s+/g, ' ');

    $.ajax({
        url: '/User/Search',
        data: {
            Page: 1,
            Key: key
        },
        type: 'POST',
        success: function (response) {
            $("#tbl-user").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm user

// start tìm kiếm lô hàng
function searchBatch() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }

    var batchCode = $.trim($("#txtBatchSearch").val());
    var fromDate = $.trim($("#txtBatchFromDate").val());
    var toDate = $.trim($("#txtBatchToDate").val());
    $.ajax({
        url: '/Batch/Search',
        data: {
            Page: 1,
            BatchCode: batchCode,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            $("#tableBatch").html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end tìm kiếm lô hàng

function isSpace(string) {
    if (/\s/.test(string)) {
        return true;
    }
}

function isNumeric(s) {
    var re = new RegExp("^[0-9,]+$");
    return re.test(s);
}

// start tạo lô hàng mới
function createBatch() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var code = $.trim($("#createBatch #code").val());
    var name = $.trim($("#createBatch #name").val());
    var price = $.trim($("#createBatch #price").val());
    var qty = cms_decode_currency_format($.trim($("#createBatch #qty").val()));
    var point = $.trim($("#createBatch #point").val());
    var note = $.trim($("#createBatch #note").val());

    if (code.length == "" || name.length == "" || price.length == "" || qty.length == "" || point.length == "") {
        swal({
            title: "",
            text: "Chỉ được phép bỏ trống phần ghi chú!",
            icon: "warning"
        })
        return;
    } else
        if (!isNumeric(price)) {
            swal({
                title: "",
                text: "Giá tiền chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        } else
            if (!isNumeric(qty)) {
                swal({
                    title: "",
                    text: "Số lượng chỉ được phép nhập số!",
                    icon: "warning"
                })
                return;
            } else
                if (!isNumeric(point)) {
                    swal({
                        title: "",
                        text: "Số điểm chỉ được phép nhập số!",
                        icon: "warning"
                    })
                    return;
                } else
                    if (qty <= 0 || qty > 50) {
                        swal({
                            title: "",
                            text: "Số lượng phải từ 1 đến 50",
                            icon: "warning"
                        })
                        return;
                    } else
                        if (isSpace(code)) {
                            swal({
                                title: "",
                                text: "Mã lô không được có khoảng trắng!",
                                icon: "warning"
                            })
                            return;
                        } else

                            $.ajax({
                                url: '/Batch/CreateBatch',
                                data: $("#form_create_batch").serialize(),
                                type: 'POST',
                                success: function (response) {
                                    if (response == SUCCESS) {
                                        $('#createBatch').modal('hide');
                                        swal({
                                            title: "Successful!",
                                            text: "",
                                            icon: "success"
                                        })

                                        //$.ajax({
                                        //    url: '/Batch/Search',
                                        //    data: {
                                        //        Page: $("#txtPageCurrent").val(),
                                        //        BatchCode: $("#txtBatchSearch").val(),
                                        //        FromDate: $("#txtBatchFromDate").val(),
                                        //        ToDate: $("#txtBatchToDate").val()
                                        //    },
                                        //    type: 'POST',
                                        //    success: function (response) {
                                        //        $("#tableBatch").html(response);
                                        //    },
                                        //    error: function (result, status, err) {
                                        //        console.log(result.responseText);
                                        //        console.log(status.responseText);
                                        //        console.log(err.Message);
                                        //    }
                                        //});
                                        searchBatch();

                                    } else
                                        if (response == DUPLICATE_NAME) {
                                            swal({
                                                title: "Không thể tạo lô hàng!",
                                                text: "Mã lô hàng đã tồn tại. Vui lòng sử dụng mã khác.",
                                                icon: "warning"
                                            })
                                            $("#createBatch #code").val("");
                                        } else {
                                            swal({
                                                title: "An error occurred!",
                                                text: "Không thể tạo lô hàng.",
                                                icon: "warning"
                                            })
                                        }
                                },
                                error: function (result) {
                                    console.log(result.responseText);
                                }
                            });
}// end tạo lô hàng mới


// start xóa user
function deleteUser(id, thiss) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    if ($(thiss).attr("data-id") == id) {
        swal({
            title: "Không thể xóa tài khoản đang đăng nhập !",
            text: "",
            icon: "error",
        })
        return;
    }
    swal({
        title: "Bạn chắc chắn muốn xóa ?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/User/DeleteUser',
                data: { id: id },
                type: "POST",
                beforeSend: function () {
                    $('#modalLoad').modal('show');
                },
                success: function (response) {
                    $('#modalLoad').modal('hide');

                    swal({
                        title: response.Message,
                        icon: response.Status == SUCCESS ? "success" : "error"
                    }).then((success) => {
                        if (success) {
                            searchUser()
                        }
                    })
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    })
}// end xóa user

// start xóa lô hàng
function deleteBatch(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Batch/DeleteBatch',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/Batch/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    BatchCode: $("#txtBatchSearch").val(),
                                    FromDate: $("#txtBatchFromDate").val(),
                                    ToDate: $("#txtBatchToDate").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableBatch").html(response);
                                },
                                error: function (result) {
                                    console.log(result.responseText);
                                }
                            });

                        } else
                            if (response == CAN_NOT_DELETE) {
                                swal({
                                    title: "Can not delete!",
                                    text: "Trong lô hàng đã có sản phẩm được dùng.",
                                    icon: "warning"
                                })
                            }
                            else
                                if (response == 96) {
                                    swal({
                                        title: "Mất mạng!",
                                        text: "Kiểm tra kết nối internet.",
                                        icon: "warning"
                                    })
                                }
                                else {
                                    swal({
                                        title: "An error occurred!",
                                        text: "",
                                        icon: "warning"
                                    })
                                }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa lô hàng

// start xóa thiết lập đổi điểm với quà, voucher
function deleteConfigCard(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Config/DeleteConfigCard',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/Config/SearchConfigCard',
                                data: {
                                    Page: $("#txtPageCurrentCard").val()
                                },
                                success: function (response) {
                                    $("#tableConfigCard").html(response);
                                }
                            });

                            //searchConfigGift();

                        } else {
                            swal({
                                title: "Can not delete!",
                                text: "error.",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}// end xóa thiết lập đổi điểm với thẻ cào


// start xóa thiết lập đổi điểm với quà, voucher
function deleteConfigGift(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Config/DeleteConfigGift',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })

                            //$.ajax({
                            //    url: '/Config/SearchConfigGift',
                            //    data: {
                            //        Page: $("#txtPageCurrent").val()
                            //    },
                            //    type: 'POST',
                            //    success: function (response) {
                            //        $("#tableConfigGift").html(response);
                            //    }
                            //});

                            searchConfigGift();

                        } else {
                            swal({
                                title: "Can not delete!",
                                text: "error.",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}// end xóa thiết lập đổi điểm với quà, voucher

// start xóa bài viết
function deleteNews(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/News/DeleteNews',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })

                            $.ajax({
                                url: '/News/Search',
                                data: {
                                    Page: $("#txtPageCurrent").val(),
                                    Title: $("#txtTitle").val(),
                                    CreateUserID: $("#cbbCreateUser").val(),
                                    Type: $("#cbbTypeNews").val(),
                                    Status: $("#cbbStatusNews").val(),
                                    FromDate: $("#txtNewsFromDate").val(),
                                    ToDate: $("#txtNewsToDate").val()
                                },
                                type: 'POST',
                                success: function (response) {
                                    $("#tableNews").html(response);
                                }
                            });

                        } else {
                            swal({
                                title: "Can not delete!",
                                text: "error.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}// end xóa bài iết



// start thông tin chi tiết bài viết
function getNewsDetail(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    window.location = "UpdateNews";
    //window.open('UpdateNews');
    $.ajax({
        url: '/News/GetNewsDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divBatchDetail").html(response);
            window.location.href = "_UpdateNews.html";
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết bài viết

// start thông tin chi tiết 1 lô hàng
function getBatchDetail(batchID) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $('#modalLoad').modal("show");
    $.ajax({
        url: '/Batch/GetBatchDetail',
        data: { BatchID: batchID },
        type: 'POST',
        success: function (response) {
            $("#divBatchDetail").html(response);
            $('#mdBatchDetail').modal('show');
            $('#modalLoad').modal("hide");
        },
        error: function (result) {
            $('#modalLoad').modal("hide");
            console.log(result.responseText);
        }
    });
}// end thông tin chi tiết 1 lô hàng

// start in mã QR của lô hàng
function printQrBatch() {
    swal({
        title: "",
        text: "Chức năng đang xây dựng!",
        icon: "warning"
    })
}// end in mã QR của lô hàng


//sửa thiết lập điểm vs thẻ cào
function updateConfigCard(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var point = cms_decode_currency_format($.trim($('#updateConfigCard #txtPoint').val()));
    var description = $.trim($('#updateConfigCard #txtDescription').val());

    if (point == "" || description == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "",
            text: "Số điểm chỉ được phép nhập số.",
            icon: "warning"
        })
        return;
    }
    if (point < 0) {
        swal({
            title: "",
            text: "Số điểm không được nhỏ hơn 0.",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/UpdateConfigCard",
        data: {
            ID: id,
            Point: point,
            Description: description
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#updateConfigCard').modal('hide');
                swal({
                    title: "Successful!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigCard',
                    data: {
                        Page: $("#txtPageCurrentCard").val()
                    },
                    success: function (response) {
                        $("#tableConfigCard").html(response);
                    }
                });

            } else {
                swal({
                    title: "Error",
                    text: "Không thể chỉnh sửa thiết lập.",
                    icon: "warning"
                })
            }
        },
        error: function () {
            swal({
                title: "Error hệ thống",
                text: "",
                icon: "warning"
            })
        }
    });

}
//sửa thiết lập điểm vs thẻ cào


//thiết lập điểm vs thẻ cào
function createConfigCard(type) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var price = $.trim($('#cbbPriceConfigCard').val());
    var point = cms_decode_currency_format($('#txtPointConfigCard').val().trim());
    var description = $.trim($('#txtDescriptionConfigCard').val());
    var telecomType = $.trim($('#cbbTelecomTypeConfigCard').val());

    if (price == "" || point == "" || description == "" || telecomType == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Số điểm chỉ được phép nhập số.",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/CreateConfigCard",
        data: {
            Price: price,
            Point: point,
            Description: description,
            Type: type,
            TelecomType: telecomType
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#createConfigCard').modal('hide');
                swal({
                    title: "Successful!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigCard',
                    data: {
                        Page: $("#txtPageCurrentCard").val()
                    },
                    success: function (response) {
                        $("#tableConfigCard").html(response);
                    }
                });

            } else
                if (response == EXISTING) {
                    $('#createConfigCard').modal('hide');
                    swal({
                        title: "",
                        text: "Thẻ cào đã được thiết lập trước đó.",
                        icon: "warning"
                    })
                } else {
                    swal({
                        title: "Error",
                        text: "Không thể tạo thiết lập.",
                        icon: "warning"
                    })
                }
        },
        error: function () {
            swal({
                title: "An error occurred.",
                text: "",
                icon: "warning"
            })
        }
    });

}
//thiết lập điểm vs thẻ cào

//sửa thiết lập điểm vs quà, voucher
function updateConfigGift(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var test = id;
    var type = $("#configGiftDetail #cbbType").val();
    var name = $.trim($('#configGiftDetail #txtName').val());
    var price = cms_decode_currency_format($.trim($('#configGiftDetail #txtPrice').val()));
    var point = cms_decode_currency_format($.trim($('#configGiftDetail #txtPoint').val()));
    var fromDate = $.trim($('#configGiftDetail #txtFromDateEdit').val());
    var toDate = $.trim($('#configGiftDetail #txtToDateEdit').val());
    var description = $.trim($('#configGiftDetail #txtDescription').val());
    var url = $("#configGiftDetail #tagImg2").attr("src");
    var status = $('#configGiftDetail #slStatus').val();

    if (name == "" || point == "" || price == "" || fromDate == "" || toDate == "" || description == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }
    if (url == URL_ADD_IMG_DEFAULT) {
        swal({
            title: "",
            text: "Please select a picture for setup!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(price)) {
        swal({
            title: "Price is only allowed to enter the number.",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Points are only allowed to enter numbers.",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/UpdateConfigGift",
        data: {
            ID: test,
            Name: name,
            Price: price,
            Point: point,
            UrlImage: url,
            Description: description,
            Type: type,
            FromDate: fromDate,
            ToDate: toDate,
            Status: status
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#configGiftDetail').modal('hide');
                swal({
                    title: "success!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigGift',
                    data: {
                        Page: $("#txtPageCurrent").val()
                    },
                    success: function (response) {
                        $("#tableConfigGift").html(response);
                    }
                });

            } else {
                swal({
                    title: "Cannot edit settings.",
                    text: "",
                    icon: "warning"
                })
            }
        },
        error: function () {
            swal({
                title: "An error occurred.",
                text: "",
                icon: "warning"
            })
        }

    });

}
//sửa thiết lập điểm vs quà, voucher


//thiết lập điểm vs quà, voucher
function createConfigGift() {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var type = $("#cbbType").val();
    var name = $.trim($('#txtName').val());
    var price = cms_decode_currency_format($.trim($('#txtPrice').val()));
    var point = cms_decode_currency_format($.trim($('#txtPoint').val()));
    var fromDate = $.trim($('#txtFromDate').val());
    var toDate = $.trim($('#txtToDate').val());
    var description = $.trim($('#txtDescription').val());
    var url = $("#tagImg").attr("src");

    if (name == "" || price == "" || point == "" || fromDate == "" || toDate == "" || description == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }
    if (url == URL_ADD_IMG_DEFAULT) {
        swal({
            title: "",
            text: "Please select a picture for setup!",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(price)) {
        swal({
            title: "Price is only allowed to enter the number.",
            text: "",
            icon: "warning"
        })
        return;
    }
    if (!isNumeric(point)) {
        swal({
            title: "Points are only allowed to enter numbers.",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/CreateConfigGift",
        data: {
            Name: name,
            Price: price,
            Point: point,
            UrlImage: url,
            Description: description,
            Type: type,
            FromDate: fromDate,
            ToDate: toDate
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {

                $('#createConfigGift').modal('hide');
                swal({
                    title: "Success!",
                    text: "",
                    icon: "success"
                })

                $.ajax({
                    url: '/Config/SearchConfigGift',
                    data: {
                        Page: $("#txtPageCurrent").val()
                    },
                    success: function (response) {
                        $("#tableConfigGift").html(response);
                    }
                });

            } else {
                swal({
                    title: "Unable to create config.",
                    text: "",
                    icon: "warning"
                })
            }
        },
        error: function (response) {
            console.log(response);
            swal({
                title: "An error occurred!",
                text: "",
                icon: "warning"
            })
        }
    });

}
//thiết lập điểm vs quà, voucher


//tạo bài viết mới
function createNews(status) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    //lấy nội dung html trong CKEDITOR
    var content = $.trim(CKEDITOR.instances['editor'].getData());
    var title = $.trim($('#txtTitle').val());
    var description = $.trim($('#txtDescription').val());
    var type = 3;
    var display = cms_decode_currency_format($('#txtDisplay').val());
    var typeSend = $('#cbbTypeSend').val();
    // var item = $('#cbbItemNews').val();
    var url = $('#AddImgLogoPlace').attr('src');

    if (display == 0) {
        swal('', 'The priority order must be greater than 0', 'warning');
        return;
    }

    if (content == "" || title == "" || description == "" || type == "" || display == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }

    if (url == URL_ADD_IMG_DEFAULT) {
        swal({
            title: "",
            text: "Please select a featured image for the article!",
            icon: "warning"
        })
        return;
    }
    $('#modalLoad').modal("show");
    $.ajax({
        url: '/News/CreateNewsDekko',
        data: {
            Content: content,
            Title: title,
            Description: description,
            Type: type,
            TypeSend: typeSend,
            UrlImage: url,
            Status: status,
            // Item: item,
            Display: display
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                swal({
                    title: "Successful",
                    text: "Add posts successfully",
                    icon: "success"
                })
                $('#modalLoad').modal("hide");
                setTimeout(
                    function () {
                        window.location.replace('/News/Index');
                    }, 1000);
            } else {
                swal({
                    title: "Error",
                    text: "Unable to create new article.",
                    icon: "warning"
                })
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}
//tạo bài viết mới

//chỉnh sửa bài viết
function updateNews(id, status) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    //lấy nội dung html trong CKEDITOR
    var content = $.trim(CKEDITOR.instances['editor'].getData());
    var title = $.trim($('#txtTitle').val());
    var description = $.trim($('#txtDescription').val());
    var type = $('#cbbType').val();
    var display = cms_decode_currency_format($('#txtDisplay').val());
    var item = $('#cbbItemNews').val();
    var typeSend = $('#cbbTypeSend').val();
    var url = $('#AddImgLogoPlace').attr('src');

    if (display == 0) {
        swal('', 'The priority order must be greater than 0', 'warning');
        return;
    }

    if (content == "" || title == "" || description == "" || type == "" || typeSend == "" || url == "" || display == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }

    if (item == null && type == 4) {
        swal({
            title: "",
            text: "Please choose a product!",
            icon: "warning"
        })
        return;
    }

    if (item == null && type != 4) {
        item = 0;
    }

    $.ajax({
        url: '/News/UpdateNewsDekko',
        data: {
            ID: id,
            Content: content,
            Title: title,
            Description: description,
            Type: type,
            TypeSend: typeSend,
            UrlImage: url,
            Status: status,
            Item: item,
            Display: display
        },
        type: 'POST',
        success: function (response) {
            if (response == SUCCESS) {
                swal({
                    title: "Successful",
                    text: "Edit the article successfully",
                    icon: "success"
                })
                setTimeout(
                    function () {
                        window.location.replace('/News/Index');
                    }, 1000);
            } else {
                swal({
                    title: "Error",
                    text: "Unable to create new article.",
                    icon: "warning"
                })
            }
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });

}
//Tìm kiếm PaymentReport

//search customer detail
//function SearchCustomerDetail(id) {
//    var fromDate = $.trim($('#txtFromDate').val());
//    var toDate = $.trim($('#txtToDate').val());
//    var checkData = {
//        fromDate: fromDate,
//        toDate: toDate,
//        id: id
//    };
//    console.log(checkData);
//    window.location.hash = "/_ListCusDetail";
//    window.location.hash = "/_ListCusDetail";
//    window.onhashchange = function () { window.location.hash = "/_ListCusDetail"; }
//    $.ajax({
//        url: "/StatisticCustomer/SearchCustomerDetail",
//        data: { id: id, toDate: toDate, fromDate: fromDate },
//        type: 'GET',
//        beforeSend: function () {
//            $("#modalLoad").modal("show");
//        },
//        success: function (result) {
//            $("#modalLoad").modal("hide");
//            $('#ViewCusDetail').html(result);
//        }
//    })
//}
//Tìm kiếm CustomerReport
//function searchCustomerReport() {
//    var code = $('#txtCodeOrName').val().trim();
//    var phone = $('#txtPhone').val().trim();
//    var fDate = $('#txtFromDate').val().trim();
//    var tDate = $('#txtToDate').val().trim();
//    $.ajax({
//        url: "/StatisticCustomer/SearchCustomer",
//        data: {
//            Page: 1,
//            Code: code,
//            Phone: phone,
//            fromDate: fDate,
//            toDate: tDate
//        },
//        beforeSend: function () {
//            $("#modalLoad").modal("show");
//        },
//        success: function (result) {
//            $("#tableCustomerReport").html(result);
//            $("#modalLoad").modal("hide");
//        },
//        error: function (result) {
//            console.log(result.responseText);
//            $("#modalLoad").modal("hide");
//        }
//    })
//}
//Tìm kiếm Coupon
function searchCoupons() {

    var code = $('#txtCode').val().trim();
    var type = $('#couponType').val();

    var fromDate = $('#txtCouponFromdate').val().trim();
    var toDate = $('#txtCouponTodate').val().trim();
    //if (status == "") {
    //    status = null;
    //}
    //if (type == "") {
    //    type = null;
    //}
    $.ajax({
        url: "/Promocode/Search",
        data: {
            Page: 1,
            Code: code,
            CouponType: type,

            CouponFromDate: fromDate,
            CouponToDate: toDate
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (result) {
            $("#modalLoad").modal("hide");
            $("#ListPromocode").html(result)
        },
        error: function (result) {
            console.log(result.responseText);
        }
    })
}
//thêm mới khuyến mại
function CreatePromocode() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }
    var code = $('#txtPromoCode').val();
    var name = $('#txtName').val();
    var content = $('#txtPaintedPromo').val();
    var promoval = $('#txtPromoVal').val().trim().replace(/,/g, "");
    var typecoupon = $('#slPromoVal').val();
    var type = $('#slType').val();
    var typetime = $('#cbTypePromo').prop("checked") ? 1 : 0;
    var fromdate = $('#dateFrom').val();
    var todade = $('#dateEnd').val();
    var cbsendall = $('#cbSendall').prop("checked") ? 1 : 0;
    if (cbsendall == 1) {
        var rank = "";
        var quantum = "0";
    } else {
        var rank = $('#slRank').val();
        var quantum = $('#ipQuantum').val();
    }
    if (code.length == 0 || content.length == 0 || name.length == 0 || promoval.length == 0 || type.length == 0 || typecoupon.length == 0) {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Promocode/CreateCoupon",
        data: {
            Name: name,
            Code: code,
            Content: content,
            TypeCoupon: typecoupon,
            type: type,
            TypeTime: typetime,
            Amount: promoval,
            CreateDate: fromdate,
            ExpriceDate: todade,
            allCustomer: cbsendall,
            QTY: quantum,
            rank: rank,
        },
        success: function (res) {
            if (res == 1) {
                swal({
                    title: "Thêm mã khuyến mại thành công",
                    text: "",
                    icon: "success"
                })
                $("#addPromocode").modal('hide');
                searchCoupons();
            } else {
                swal({
                    title: "Thêm mới thất bại",
                    text: "",
                    icon: "error"
                })
            }
        }
        //beforeSend: function () {
        //    $("#modalLoad").modal('show');
        //},
        //type: 'POST',
        //success: function (response) {
        //    result = response.Status;
        //    $("#modalLoad").modal('hide');
        //    swal({
        //        title: response.Message,
        //        icon: response.Status == SUCCESS ? "success" : "error"

        //    }).then((rp) => {
        //        if (rp) {

        //        }
        //    })

        //},
        //error: function (result) {
        //    console.log(result.responseText);
        //    swal({
        //        title: "error",
        //        text: "",
        //        icon: "warning"
        //    })
        //}
    });


}
//Add coupon
function saveCreateCoupon() {
    //alert($('#txt_promocode').val());
    var promocode = $.trim($('#txt_promocode').val());
    var content = $.trim($('#txt_content').val());
    var amount = $.trim($('#txt_amount').val().replace(/,/g, ""));
    var typeCoupon = $('#txtValue').val();
    //var typeCoupon = $('#txt_typeCoupon').val();
    var startDate = $.trim($('#txtFromDateCoupon').val());
    var endDate = $.trim($('#txtToDateCoupon').val());
    var quantity = $.trim($('#txt_quantity').val());
    var type = $("#txt_typeCouponCreate").val();
    var fileUpload = $("#input-promocode-url-image").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    var fileName = "";
    var rankId = $("#select-rank-promocode").val() == -1 ? null : $("#select-rank-promocode").val();
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
        fileName = files[i].name;
    }
    var urlImage = window.location.origin + "/Uploads/images/" + fileName;
    //console.log(promocode);
    if (promocode == "") {
        swal({
            title: "",
            text: "Please! Enter your promocode",
            icon: "warning"
        })
        return;
    }

    if (promocode.length <= 3) {
        swal({
            title: "",
            text: "Promocde must be over three characters!",
            icon: "warning"
        })
        return;
    }
    if (amount >= 100 && typeCoupon == 1) {
        swal({
            title: "",
            text: " Amount value must be less than 100 !",
            icon: "warning"
        })
        return;
    }
    if (amount < 1000 && typeCoupon == 2) {
        swal({
            title: "",
            text: " Amount value must be biger than 1000 !",
            icon: "warning"
        })
        return;
    }
    if (quantity > 2000) {
        swal({
            title: "",
            text: "Quantity must be less than 2000",
            icon: "warning"
        })
        return;
    }
    if ($('.dropify-wrapper').hasClass("has-preview")) {
        console.log("has");
    }
    else {
        swal({
            title: "",
            text: "please select image",
            icon: "warning"
        });
        return;
    }
    if (quantity == 0 && endDate.length == 0) {
        swal({
            title: "",
            text: "please select Quantity or Exprice Date",
            icon: "warning"
        });
        return;
    }
    //else if (url == null) {
    //    swal({
    //        title: "Thông báo",
    //        text: "Vui lòng chọn ít nhất 1 ảnh",
    //        icon: "warning"
    //    })
    //    return;
    //}
    if (fileName != "") {
        $.ajax({
            url: '/Promocode/UploadImage',
            type: 'POST',
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            cache: false,
            data: fileData,
            success: function (res) {
                if (res.Status == 1) {
                    $.ajax({
                        url: "/Promocode/CreateCoupon",
                        data: {
                            Code: promocode,
                            Content: content,
                            TypeCoupon: typeCoupon,
                            Amount: amount,
                            QTY: quantity,
                            CreateDate: startDate,
                            ExpriceDate: endDate,
                            Path: urlImage,
                            type: type,
                            rankId: rankId
                        },
                        success: function (response) {
                            if (response == 1) {
                                $("#add-image-promocode").empty();
                                $("#add-image-promocode").append('<input type="file" id="input-promocode-url-image" name="UrlImage" class="dropify" />');
                                $('#input-promocode-url-image').dropify({
                                    messages: {
                                        default: 'Click to select image',
                                        replace: 'Click to select another image',
                                        remove: 'Delete image'
                                    }
                                });
                                $("#expire").prop("checked", false);
                                $(".checkDisplay").addClass("display-none");
                                $("#addPromocode").modal('hide');
                                searchCoupons();
                                swal({
                                    title: "",
                                    text: "Add promocode successFully !",
                                    icon: "success"
                                })
                            }
                            else {
                                swal({
                                    title: "",
                                    text: "Promocode already exists !",
                                    icon: "error"
                                })
                            }
                        }
                    });
                }
                else {
                    swal({
                        title: "",
                        text: "Image can not empty!",
                        icon: "warning"
                    });
                }
            }

        })
    }
    else {
        swal({
            title: "",
            text: "please select image",
            icon: "warning"
        });
    }
}
// modal Sửa coupon
function Detail($id) {
    $.ajax({
        url: "/Promocode/ModalEditCoupon",
        data: { ID: $id },
        type: 'POST',
        beforeSend: function () {
            $('#modalLoad').modal('show');

        },
        success: function (response) {
            $('#modalLoad').modal('hide');
            $("#update_coupon").html(response);
            $('#modal_edit_coupon').modal('show');
        }
    });
}
function changeExpireEdit() {
    if ($("#expireEdit").prop("checked") == true) {
        $(".checkDisplay").removeClass("display-none");
    }
    else {
        $(".checkDisplay").addClass("display-none");
    }
    //console.log($("#expire").prop("checked"));
}
//function ClearFilterShiperReport() {
//    $('txtCodeName').val('');
//    $('txtShiperPhone').val('');
//    $('txtFromeDates').val('');
//    $('txtToDates').val('');
//    SearchShipersReport();
//}
////search bao cao doi tac
//function SearchShipersReport() {
//    var searchKeys = $('txtCodeName').val();  
//    var fromDates = $('txtFromeDates').val();
//    var toDates = $('txtToDates').val();
//    $.ajax({
//        url: "/StatisticShiper/SearchShiperReport",
//        data: { page: 1, searchKey: searchKeys, fromDate: fromDates, toDate: toDates },
//        success: function (rs) {
//            $("#modalLoad").modal('hide');
//            $('#tableWasherReport').html(rs);
//        }

//    });
//}
function ClearFilterPayReport() {
    $('#txtCodeOrName').val('');
    $('#txtPhone').val('');
    $('#txtFromDate').val('');
    $('#txtToDate').val('');
    searchPaymentReport();
}
function ClearFilterCusReport() {
    $('#txtCodeOrName').val('');
    $('#txtPhone').val('');
    $('#txtFromDate').val('');
    $('#txtToDate').val('');
    searchCustomerReport();
}
function ClearFilterCoupon() {
    $('#txtCode').val('');
    $('#couponType').val('');
    $('#couponStatus').val('');
    $('#txtCouponFromdate').val('');
    $('#txtCouponTodate').val('');
    searchCoupons();
}
//chỉnh sửa bài viết
//Delete Category
function DeleteCoupon($id) {

    swal({
        title: "Are you sure you want to delete?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Promocode/DeleteCoupon',
                    data: { ID: $id },
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Thông báo",
                                text: "Xóa thành công!",
                                icon: "success"
                            });
                            searchCoupons();
                        }
                        else {
                            swal({
                                title: "Thông báo",
                                text: "Lỗi!",
                                icon: "warning"
                            });
                        }
                    }

                });
            } searchCoupons();
        });
}
//Tìm kiếm Card
function searchCard() {
    var seri = $('#txtSeri').val().trim();
    var fromDate = $('#txtFromDate').val().trim();
    var toDate = $('#txtToDate').val().trim();
    var status = $('#cmbStatus').val().trim();
    $.ajax({
        url: "/Card/Search",
        data: {
            Page: 1,
            Seri: seri,
            FromDate: fromDate,
            ToDate: toDate,
            Status: status
        },
        success: function (result) {
            $("#tableCard").html(result)
        },
        error: function (result) {
            console.log(result.responseText);
        }
    })
}
// thêm mới card 
function createCard() {

    $("#frmCreateCard").validate({
        ignore: ".date",
        rules: {
            CardCode: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            },
            SeriNumber: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            }
        },
        messages: {
            CardCode: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Mã thẻ phải có ít nhất 10 chứ số",
                maxlength: "Mã thẻ tối đa 15 chữ số"
            },
            SeriNumber: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Seri phải có ít nhất 10 chứ số",
                maxlength: "Seri tối đa 15 chữ số"
            }
        },
        submitHandler: function () {
            if ($("#dtpStartDate").val() == '' || $("#dtpExprireDate").val() == '') {
                swal({
                    title: "",
                    text: "Please enter full information",
                    icon: "warning"
                });
                return;
            }
            $.ajax({
                url: "/Card/addCard",
                data: $('#frmCreateCard').serialize(),
                type: 'POST',
                success: function (result) {
                    if (result == 1) {
                        swal({
                            title: "",
                            text: "Tạo mới Successful",
                            icon: "success"
                        });
                        $('#createCard').modal("hide");
                        searchCard();
                    }
                    else if (result == DUPLICATE_NAME) {
                        swal({
                            title: "",
                            text: "Mã thẻ hoặc seri đã tồn tại, vui lòng kiểm tra lại !",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == -9) {
                        swal({
                            title: "",
                            text: "mã thẻ và seri không được trùng nhau",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == 3) {
                        swal({
                            title: "Kiểm Tra Lại Ngày Hết Hạn",
                            text: "Ngày hết hạn phải lớn hơn ngày bắt đầu",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "error trong quá trình thêm mới",
                            text: "Vui lòng liên hệ với bộ phận hỗ trợ khách hàng",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    });
}

// chỉnh sửa Card
function editCard(ID, Status) {
    if (Status == 1) {
        swal({
            title: "",
            text: "Card đã đổi, bạn không thể sửa",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Card/showEditCard",
        data: { CardID: ID },
        success: function (result) {
            $('#frmEdit').html(result);
            $('#showEdit').modal("show");
        }
    })
}
// lưu thay đổi chỉnh sửa
function SaveEdit(id) {
    $("#frmShowEdit").validate({
        ignore: ".date",
        rules: {
            CardCode: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            },
            SeriNumber: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 15
            }
        },
        messages: {
            CardCode: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Mã thẻ phải có ít nhất 10 chứ số",
                maxlength: "Mã thẻ tối đa 15 chứ số"
            },
            SeriNumber: {
                required: "Vui lòng không để trống",
                digits: "Vui lòng nhập số nguyên dương",
                minlength: "Seri phải có ít nhất 10 chứ số",
                maxlength: "Seri tối đa 15 chữ số"
            }
        },
        submitHandler: function () {
            if ($("#dtpStartDateEdit").val() == '' || $("#dtpExprireDateEdit").val() == '') {
                swal({
                    title: "",
                    text: "Please enter full information",
                    icon: "warning"
                });
                return;
            }
            $.ajax({
                url: "/Card/addCard",
                data: $('#frmShowEdit').serialize(),
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "",
                            text: "Cập nhật Successful",
                            icon: "success"
                        });
                        $('#showEdit').modal("hide");
                        searchCard();
                    }
                    else if (result == DUPLICATE_NAME) {
                        swal({
                            title: "",
                            text: "Mã thẻ hoặc seri đã tồn tại, vui lòng kiểm tra lại !",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == -9) {
                        swal({
                            title: "",
                            text: "mã thẻ và seri không được trùng nhau",
                            icon: "warning"
                        });
                        $('#txtCardCode').val("");
                        $('#txtSeriNumber').val("");
                    }
                    else if (result == 3) {
                        swal({
                            title: "Kiểm Tra Lại Ngày Hết Hạn",
                            text: "Ngày hết hạn phải lớn hơn ngày bắt đầu",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Error rồi đại vương ơi",
                            text: "Vui lòng liên hệ với bộ phận chăm sóc khách hàng",
                            icon: "warning"
                        })
                    }
                }
            });
        }
    });
}
// xóa card
function deleteCard(id) {
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Card/DeleteCard',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })
                            searchCard();
                        } else {
                            swal({
                                title: "Can not delete!",
                                text: "error.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
}
// load District

function loadListDistrictShop(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrictShop",
        data: { ProvinceID: prvID },
        success: function (result) {
            $('#ListDistrictShop').html(result);
        }
    });
}

function loadListDistrictShopCreate(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrictShopCreate",
        data: { ProvinceID: prvID },
        success: function (result) {
            $('#ListDistrictShopCreate').html(result);
        }
    });
}

function loadListDistrictShopUpdate(prvID, id) {
    $.ajax({
        url: "/Shop/LoadDistrictShopUpdate",
        data: {
            ProvinceID: prvID,
            ID: id
        },
        success: function (result) {
            $('#ListDistrictShopUpdate').html(result);
        }
    });
}

function loadListDistrict(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrict",
        data: { ProvinceID: prvID },
        success: function (result) {
            $("#slDistrict").empty();
            $("#slDistrict").append("<option selected disabled hidden '>Quận/Huyện</option>");
            $.each(result, function () {
                var select = "<option value='" + this.ID + "'>" + this.Name + "</option>";
                $("#slDistrict").append(select);
            });
        }
    });
}


function loadListDistrictModal(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrict",
        data: { ProvinceID: prvID },
        success: function (result) {
            $("#slDistrictModal").empty();
            $("#slDistrictModal").append("<option selected disabled hidden '>Quận/Huyện</option>");
            $.each(result, function () {
                var select = "<option value='" + this.ID + "'>" + this.Name + "</option>";
                $("#slDistrictModal").append(select);
            });
        }
    });
}
function loadListDistrictModals(prvID) {
    $.ajax({
        url: "/Shop/LoadDistrict",
        data: { ProvinceID: prvID },
        success: function (result) {
            $("#slDistrictModals").empty();
            $("#slDistrictModals").append("<option selected disabled hidden '>Quận/Huyện</option>");
            $.each(result, function () {
                var select = "<option value='" + this.ID + "'>" + this.Name + "</option>";
                $("#slDistrictModals").append(select);
            });
        }
    });
}


function LoadChecked() {
    var listcheckbox = $(".checkboxcus");
    var Local = localStorage.getItem("ListCus") == "" ? null : localStorage.getItem("ListCus");
    var ListLocal = Local != null ? Local.split(",") : [];
    if (ListLocal.length > 0) {
        for (i = 0; i < ListLocal.length; i++) {
            var PhoneName = ListLocal[i].split("-");
            $.each(listcheckbox, function () {
                if ($(this).attr("data-phone").trim() == PhoneName[1].trim()) {
                    $(this).prop("checked", true);
                }
            })

        }
    }

}
//Tìm kiếm khách hàng
function searchCustomer() {
    var fromDate = $('#dtFromdate').val().trim();
    var toDate = $('#dtTodateIndex').val().trim();
    var phone = $('#txtPhone').val().trim();
    var codeOrName = $('#txtName').val().trim();
    var email = $('#txtEmail').val().trim();
    var active = $('#txtActive').val().trim();
    //var province = $('#slProvince').val();
    //var district = $('#slDistrict').val();
    //var role = $('#cmbRoleCus').val();
    //var status = $('#cbbStatusCustomer').val();

    $.ajax({
        url: "/Customer/Search",
        data: {
            page: 1,
            fromDate: fromDate.length == 0 ? "" : fromDate,
            toDate: toDate.length == 0 ? "" : toDate,
            //City: province,
            //District: district,
            phone: phone.length == 0 ? "" : phone,
            active: active,
            email: email.length == 0 ? "" : email,
            codeOrName: codeOrName.length == 0 ? "" : codeOrName
        },
        success: function (result) {
            $('#ListCustomer').html(result);

        }
    });
}

function showAddPointWithChecked(inputCheck) {
    var row = $(inputCheck).parents('tr');
    var check = $(row).find('#txtchecked').prop('checked');
    var phone = $(row).find('#colPhone').html();
    if (check) {
        $('#mdAddPoint #txtPhoneNumber').val(phone);
    }
    else {
        $('#mdAddPoint #txtPhoneNumber').val('');
    }
}

function addPoint() {

    $("#frmAddPoint").validate({
        rules: {
            pointNum: {
                required: true,
            },
            phoneNum: {
                required: true,
                minlength: 10,
            }

        },
        messages: {
            pointNum: {
                required: "Vui lòng không để trống",
            },
            phoneNum: {
                required: "Vui lòng không để trống",
                minlength: "Số Điện thoại phải >= 10 ký tự",
            }
        },
        submitHandler: function () {
            var phone = $('#mdAddPoint #txtPhoneNumber').val();
            var point = $('#mdAddPoint #txtPoint').val();
            var note = $('#mdAddPoint #txtNote').val();
            $.ajax({
                url: "/Customer/AddPoint",
                data: {
                    Phone: phone,
                    Point: point,
                    Note: note
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "Thêm Điểm Successful",
                            text: "",
                            icon: "success"
                        })
                        searchCustomer();
                        $('#mdAddPoint').modal("hide");
                    }
                    else {
                        swal({
                            title: "",
                            text: "error.",
                            icon: "warning"
                        })
                    }
                }
            });
        }
    });
}

function GetCustomerDetail(id) {
    //window.location.hash = "/detail";
    //window.location.hash = "/detail";
    //window.onhashchange = function () { window.location.hash = "/update"; }
    $.ajax({
        url: "/Customer/CustomerDetail",
        //type: 'GET',
        data: { id: id },
        beforeSend: function () {
            $("#modalLoad").modal('show');
        },
        success: function (result) {
            $("#modalLoad").modal('hide');
            if (result == null) {
                swal({
                    title: "",
                    text: "System is under maintenance.",
                    icon: "warning"
                })
            }
            else {
                $('#View').html(result);
            }

        }
    });
}

function saveEditCustomer(id) {
    $("#EditCustomer #frmEdit_Customer").validate({
        ignore: ".date",
        rules: {
            cusName: {
                required: true,
            },
            cusPhone: {
                minlength: 10,
            },
            cusEmail: {
                email: true
            }

        },
        messages: {
            cusName: {
                required: "Vui lòng không để trống",
            },
            cusPhone: {
                minlength: "Số Điện thoại phải >= 10 ký tự",
            },
            cusEmail: {
                email: "Vui lòng nhập đúng Email"
            }
        },
        submitHandler: function () {
            var name = $('#txtCusName').val().trim();
            var phone = $('#txtCusPhone').val().trim();
            var email = $('#txtCusEmail').val().trim();
            var sex = $('#cmbSex').val();
            //var status = $('#cbbStatusUpdate').val();
            var birthday = $('#dtpBirthDay').val().trim();
            var address = $('#txtAddress').val().trim();
            var lati = $('#lati3').val().trim();
            var long = $('#long3').val().trim();

            $.ajax({
                url: "/Customer/SaveEditCustomer",
                data: {
                    Name: name,
                    Phone: phone,
                    Email: email,
                    Sex: sex,
                    //Status: status,
                    BirthDay: birthday,
                    Address: address,
                    Lati: lati,
                    Long: long,
                    ID: id
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        $('#EditCustomer').modal("hide");
                        $('.modal-backdrop').hide(); // xóa lớp mờ mờ đen khi đóng modal Error
                        swal({
                            title: "Cập nhật Successful",
                            text: "",
                            icon: "success"
                        });
                        //setTimeout(function () {
                        GetCustomerDetail(id);
                        //}, 1000);

                    }
                    else {
                        swal({
                            title: "Error",
                            text: "",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    });
}

function SearchHistoryPoint(id) {
    var fromdate = $('#addPoint #dtpFromDate').val();
    var todate = $('#addPoint #dtpTodate').val();

    $.ajax({
        url: "/Customer/SearchHistoryPoint",
        data: {
            Page: 1,
            cusID: id,
            FromDate: fromdate,
            ToDate: todate
        },
        success: function (result) {
            $('#ListHistoryPoint').html(result);
        }
    });
}

function SearchRequset(id) {
    var fromdate = $('#changePoint #dtpFromdateRQ').val();
    var todate = $('#changePoint #dtpToDateRQ').val();

    $.ajax({
        url: "/Customer/SearchReQuest",
        data: {
            Page: 1,
            cusID: id,
            FromDate: fromdate,
            ToDate: todate
        },
        success: function (result) {
            $('#ListRequest').html(result);
        }
    });
}

function searchOrderHistory(id) {
    $.ajax({
        url: "/Customer/searchOrderHistory",
        data: {
            Page: 1,
            cusID: id,
            fromDate: $("#dtpFromdateOH").val(),
            toDate: $("#dtpToDateOH").val()
        },
        success: function (result) {
            $("#ListOrderHistory").html(result);
        }
    });
}

// Xóa Cus
function DeleteCus(id) {
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: "/Customer/DeleteCustomer",
                    data: { ID: id },
                    success: function (result) {
                        if (result == SUCCESS) {
                            swal({
                                title: "Delete successful",
                                text: "",
                                icon: "success"
                            })
                            searchCustomer();
                        } else if (result == 2) {
                            swal({
                                title: "Can not delete khách hàng",
                                text: "Chỉ được xóa những khách hàng tạm dừng hoạt động.",
                                icon: "warning"
                            })
                            searchCustomer();
                        }
                        else {
                            swal({
                                title: "error",
                                text: "",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}

// Thêm Rank
function LoadRank() {
    $.ajax({
        url: "/Config/LoadRank",
        data: { Page: 1 },
        success: function (result) {
            $('#ListRank').html(result);
        }
    });
}

//edit Rank Details
function SaveEditRank(Id) {
    var checkQTY = true;
    var lsServiceBonus = [];
    var lsServiceBirthDay = [];
    var listTagBD = $("#lsServiceBirthDay").children("tr");
    var listTagBN = $("#lsServiceBonus").children("tr");
    var Name = $("#nameRank").val();
    var MaxPoint = $("#toPointRank").val().split(",").join("");
    var checkpoint = $("#toPointRank").attr("data-id");
    var DescriptionEN = $.trim(CKEDITOR.instances['txtDescriptionEN'].getData());
    var DescriptionVI = $.trim(CKEDITOR.instances['txtDescriptionVI'].getData())
    var profitCash = $("#profitCash").val().split(",").join("");
    var profitVnpay = $("#profitVnpay").val().split(",").join("");
    var PointBonus = $("#pointBonus").val().split(",").join("");
    var BirthDayProfit = $("#birthdayProfit").val().split(",").join("");
    var otherGiftRank = $("#otherGiftRank").val().split(",").join("");
    var PolicyEN = $.trim(CKEDITOR.instances['txtPolicyEN'].getData())
    var PolicyVN = $.trim(CKEDITOR.instances['txtPolicyVN'].getData())
    $.each(listTagBD, function () {
        if ($(this).find("input").val() <= 0) {
            checkQTY = false;
            return;
        }
        else {
            var obj = new Object();
            obj.ServiceID = $(this).attr("id");
            obj.Name = $(this).text();
            obj.Qty = $(this).find("input").val();
            lsServiceBirthDay.push(obj);
        }
    })
    $.each(listTagBN, function () {
        if ($(this).find("input").val() <= 0) {
            checkQTY = false;
            return;
        } else {
            var obj = new Object();
            obj.ServiceID = $(this).attr("id");
            obj.Name = $(this).text();
            obj.Qty = $(this).find("input").val();
            lsServiceBonus.push(obj);
        }

    })
    if (!checkQTY) {
        swal({
            title: "Quantity must be greater 0 ",
            icon: "warning"
        })
        return;
    }
    if (Name.length <= 0) {
        swal({
            title: "Name is not empty ",
            icon: "warning"
        })
        return;
    }
    if (MaxPoint <= 0 && checkpoint == 1) {
        swal({
            title: "Point must be greater 0  ",
            icon: "warning"
        })
        return;
    }
    if (DescriptionEN.length <= 0 || DescriptionVI.length <= 0) {
        swal({
            title: "Description is not empty  ",
            icon: "warning"
        })
        return;
    }
    if (profitCash < 0 || profitCash > 100 || profitVnpay < 0 || profitVnpay > 100) {
        swal({
            title: "Profit must be greater than 0 and less than 100 ",
            icon: "warning"
        })
        return;
    }
    if (PointBonus < 0) {
        swal({
            title: "Point must be greater than 0 and less than 100 ",
            icon: "warning"
        })
        return;
    }
    if (BirthDayProfit < 100) {
        swal({
            title: "Birthday Deal must be greater than 100 ",
            icon: "warning"
        })
        return;
    }
    if (PolicyEN.length <= 0 || PolicyVN.length <= 0) {
        swal({
            title: "Policy is not empty  ",
            icon: "warning"
        })
        return;
    }
    var item = {
        Name: Name,
        ID: Id,
        MaxPoint: MaxPoint,
        DescriptionEN: DescriptionEN,
        DescriptionVI: DescriptionVI,
        profitCash: profitCash,
        profitVnpay: profitVnpay,
        PointBonus: PointBonus,
        BirthDayProfit: BirthDayProfit,
        lsServiceBonus: lsServiceBonus,
        lsServiceBirthDay: lsServiceBirthDay,
        ortherGift: otherGiftRank,
        PolicyEN: PolicyEN,
        PolicyVN: PolicyVN
    }
    $.ajax({
        url: "/Rank/UpdateRanking",
        type: "POST",
        data: {
            item: item
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            console.log(res);
            if (res.Status == 1) {
                swal({
                    title: "Success",
                    text: "Edit rank successfully !",
                    icon: "success"
                })
            }
            else {
                swal({
                    title: "Faild",
                    text: "Edit rank Faild !",
                    icon: "error"
                })
            }
        }
    })
}

// ShowEdit RAnk
function showEditRank(id) {
    $('#editRank').remove();
    $.ajax({
        url: "/Config/ShowEditRank",
        data: { ID: id },
        success: function (result) {
            $('#editArea').html(result);
            $('#editRank').modal("show");
        }
    });
}

// Edit Rank
function saveEditRank(id) {
    var min = $('#txtMinPointEdit').val().trim();
    var max = $('#txtMaxPointEdit').val().trim();
    var des = $('#txtDesEdit').val().trim();

    swal({
        title: "Cảnh Báo !!",
        text: "Nếu bạn thay vùng giá trị điểm của mục này sẽ làm vùng giá trị khác thay đổi theo",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                if (min == "" || max == "" || des == "") {
                    swal({
                        title: "!",
                        text: "Please enter full information",
                        icon: "warning"
                    })
                    return;
                }
                if (min >= max) {
                    swal({
                        title: "Cảnh Báo !!",
                        text: "Giá Trị Bắt Đầu Phải Nhỏ Hơn Giá Trị Kết Thúc",
                        icon: "warning"
                    })
                    return;
                }
                $.ajax({
                    url: "/Config/EditRank",
                    data: {
                        ID: id,
                        Descripton: des,
                        MaxPoint: max,
                        MinPoint: min
                    },
                    success: function (result) {
                        if (result == SUCCESS) {
                            swal({
                                title: "Cập nhật Successful",
                                text: "",
                                icon: "success"
                            });

                            setTimeout(function () {
                                $(".swal-button--confirm").click();
                            }, 1000);
                            setTimeout(function () {
                                LoadRank();
                            }, 1500);
                            $("#editRank").modal("hide");

                        }
                        else {
                            swal({
                                title: "error",
                                text: "",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })

}

function getConfigGiftDetail(id) {
    //swal({
    //    title: "",
    //    text: "Chức năng đang xây dựng",
    //    icon: "warning"
    //})
    $.ajax({
        url: '/Config/GetConfigGiftDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divConfigGiftDetail").html(response);
            $('#configGiftDetail').modal('show');
        }
    });
}

function getConfigCardDetail(id) {
    $.ajax({
        url: '/Config/GetConfigCardDetail',
        data: { ID: id },
        type: 'POST',
        success: function (response) {
            $("#divConfigCardDetail").html(response);
            $('#updateConfigCard').modal('show');
        }
    });
}


function searchPoint(id) {
    fromDate = $.trim($('#fromDate').val());
    toDate = $.trim($('#toDate').val());
    name = $.trim($('#agentName').val());
    $.ajax({
        url: "/Point/Search",
        data: {
            Page: 1,
            Name: name,
            FromDate: fromDate,
            ToDate: toDate,
            CusID: id
        },
        success: function (response) {
            $('#tablePoint').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function getPointDetail($id) {
    $.ajax({
        url: "/Point/GetPointDetail",
        data: { ID: $id },
        type: 'POST',
        success: function (response) {
            $('#modalDetailPoint').html(response);
            $('#detailPoint').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function searchWarrantyCard() {
    $fromDate = $('#fromDate').val();
    $toDate = $('#toDate').val();
    $status = $('#status').val();
    $code = $.trim($('#warrantyCardCode').val());

    if ($code.length == 16) {
        var count = $code.length - 1;
        $warrantyCardCode = $code.substring(0, count);
    } else {
        $warrantyCardCode = $code;
    }

    $.ajax({
        url: "/Warranty/Search",
        data: { Page: 1, fromdate: $fromDate, ToDate: $toDate, Status: $status, WarrantyCardCode: $warrantyCardCode },
        success: function (response) {
            $('#TableWarranty').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function CreateWarranty() {
    var quantity = $.trim($('#quantity').val());
    var point = $.trim($('#point').val());
    var expireDate = $('#expireDate').val();
    if (quantity == "" || point == "" || expireDate == "") {
        swal({
            title: "",
            text: "Mời nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(quantity)) {
            swal({
                title: "",
                text: "Số lượng chỉ được phép nhập số!",
                icon: "warning"
            })
            return;
        }
        else {
            if (!isNumeric(point)) {
                swal({
                    title: "",
                    text: "Số điểm chỉ được phép nhập số!",
                    icon: "warning"
                })
                return;
            }
            else {
                $.ajax({
                    url: "/Warranty/CreateWarranty",
                    data: $('#form_create_warranty').serialize(),
                    type: "POST",
                    success: function (response) {
                        if (response != null) {
                            $('#QrCodeWarrantyCard').html(response);
                            $('#createWarranty').modal('hide');
                            swal({
                                title: "",
                                text: "Successful!",
                                icon: "success"
                            });

                            $('#printWarranty').modal('show');
                            searchWarrantyCard();
                            //resetInputCreteWrtCard();
                        } else {
                            //resetInputCreteWrtCard();
                            swal({
                                title: "",
                                text: "Error",
                                icon: "warning"
                            })
                            return;
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        }
    }
}

function searchRequestForGift() {
    var name = $('#txtCusName').val().trim();
    var type = $('#slGiftType').val();
    var fromdate = $('#txtFromDate').val().trim();
    var todate = $('#txtToDate').val().trim();

    $.ajax({
        url: "/StatisticGift/SearchRequestForGift",
        data: {
            Page: 1,
            CusName: name,
            GiftType: type,
            FromDate: fromdate,
            ToDate: todate
        },
        success: function (result) {
            $('#TableRQ').html(result);
        }
    });
}

function resetInputCreteWrtCard() {
    $('#quantity').val('');
    $('#point').val('');
}

function DeleteWarrantyCard($id) {
    swal({
        title: "Are you sure you want to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Warranty/DeleteWarrantyCard',
                    data: { ID: $id },
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "",
                                text: "Delete successful!",
                                icon: "success"
                            });
                            searchWarrantyCard();
                        }
                        else {
                            swal({
                                title: "",
                                text: "Error!",
                                icon: "warning"
                            });
                        }
                    }
                });
            }
        })
}

function getWarrantyDetail($ID, $WarrantyCardCode) {
    $.ajax({
        url: '/Warranty/getWarrantyDetail/',
        data: { ID: $ID, WarrantyCodeCard: $WarrantyCardCode },
        success: function (response) {
            $('#DetailWarrantyQRCode').html(response);
            $('#QrcodeDetail').modal('show');
        }
    });
}


// thống kê  doanh thu
function statisticRevenue() {
    var obj = $('#slObj').val();
    var fd = $('#txtFromDate').val();
    var td = $('#txtToDate').val();
    $.ajax({
        url: "/StatisticRevenue/Search",
        data: {
            Page: 1,
            AgentID: obj,
            FromDate: fd,
            ToDate: td
        },
        success: function (result) {
            $('#list').html(result);
        }
    });
}

// Tìm Kiếm Đơn Hàng
function searchOrder() {
    var agent = $('#slAgent').val();
    var customer = $('#slCustomer').val();
    var status = $('#slStatus').val();
    var fd = $('#txtFromDate').val().trim();
    var td = $('#txtToDate').val().trim();
    var code = $('#orderCode').val().trim();

    $.ajax({
        url: "/Order/Search",
        data: {
            Page: 1,
            Agent: agent,
            Customer: customer,
            Status: status,
            FromDate: fd,
            ToDate: td,
            Code: code
        },
        success: function (result) {
            $('#list').html(result);
        }
    });
}

//Tìm kiếm sản phẩm
function searchItem() {
    var itemName = $('#txtItemName').val().trim()
    var itemCode = $('#txtItemCode').val().trim()
    var frmDate = $('#frmDate').val().trim()
    var toDate = $('#toDate').val().trim()

    $.ajax({
        url: '/Item/Search',
        data: {
            Page: 1,
            FromDate: frmDate,
            ToDate: toDate,
            ItemName: itemName,
            ItemCode: itemCode
        },
        success: function (response) {
            $('#tableItem').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

function CreateItem() {
    var Code = $('#CodeCreate').val().trim();
    var Name = $('#NameCreate').val().trim();
    var Price = $('#PriceCreate').val().trim();
    var AgentPrice = $('#AgentPriceCreate').val().trim()
    var DiscountPrice = $('#DiscountPriceCreate').val().trim()
    var Image = $('#txt-url-img').val().trim();
    var Description = $('#DescriptionCreate').val().trim();
    var Brand = $('#Brand').val().trim();
    var MadeIn = $('#MadeIn').val().trim();
    var Warranty = $('#Warranty').val().trim();

    if (Code.length < 6) {
        swal({
            title: "",
            text: "Product code must be at least 6 characters long!",
            icon: "warning"
        })
        return;
    }
    if (isSpace(Code)) {
        swal({
            title: "",
            text: "Product code must not contain spaces!",
            icon: "warning"
        })
        return;
    }
    if (Code == "" || Name == "" || Price == "" || Image == "" || Description == "" || Brand == "" || MadeIn == "" || Warranty == "") {
        swal({
            title: "",
            text: "Please complete all information",
            icon: "warning"
        })
        return;
    } else {
        if (!isNumeric(Price)) {
            swal({
                title: "",
                text: "Price is only allowed to enter the number!",
                icon: "warning"
            })
            return;
        }
        else {
            $.ajax({
                url: '/Item/CreateItem',
                data: $('#form_create_item').serialize(),
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "Success!",
                            text: "",
                            icon: "success"
                        });
                        $('#createItem').modal('hide');
                        searchItem();
                    }
                    else if (response == EXISTING) {
                        swal({
                            title: "Cannot create product",
                            text: "Product code already exists. Please use another code.",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "An error occurred!",
                            text: "Cannot create product",
                            icon: "warning"
                        });
                    }
                },
                error: function (result) {
                    console.log(result.responseText);
                }
            });
        }
    }
}

//Load data to Popup UpdateItem
function LoadItem(id) {
    $.ajax({
        url: '/Item/LoadItem',
        data: { ID: id },
        success: function (response) {
            $('#UpdateItem').html(response);
            $('#EditItem').modal('show');
        }
    });
}

function DeleteItem($id) {
    swal({
        title: "Are you sure you want to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((isconfirm) => {
        if (isconfirm) {
            $.ajax({
                url: '/Item/DeleteItem',
                data: { ID: $id },
                success: function (response) {
                    if (response == SUCCESS) {
                        swal({
                            title: "",
                            text: "Delete successful!",
                            icon: "success"
                        });
                        searchItem();
                    }
                    else {
                        swal({
                            title: "",
                            text: "Error!",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    })
}

//Lưu lại cập nhập
function SaveEditItem() {
    var Code = $('#CodeEdit').val().trim();
    var Name = $('#NameEdit').val().trim();
    var Price = $('#PriceEdit').val().trim();
    var AgentPrice = $('#AgentPriceEdit').val().trim()
    var DiscountPrice = $('#DiscountPriceEdit').val().trim()
    var Image = $('#tagImage').attr('src');
    var Description = $('#NoteEdit').val().trim();
    if (Code == "" || Name == "" || Price == "" || Image == "" || Description == "") {
        swal({
            title: "",
            text: "Please enter full information!",
            icon: "warning"
        })
        return;
    }
    else {
        if (!isNumeric(Price) || !isNumeric(AgentPrice) || !isNumeric(DiscountPrice)) {
            swal({
                title: "",
                text: "Price is only allowed to enter the number!",
                icon: "warning"
            })
            return;
        }
        else {
            swal({
                title: "You definitely want to save changes?",
                text: "",
                icon: "warning",
                buttons: ["Cancel", "OK"],
                dangerMode: true
            }).then((reponse) => {
                if (reponse) {
                    $.ajax({
                        url: '/Item/SaveEditItem',
                        data: $('#form_update_item').serialize(),
                        success: function (response) {
                            if (response == SUCCESS) {
                                swal({
                                    title: "Success!",
                                    text: "",
                                    icon: "success"
                                });
                                $('#EditItem').modal('hide');
                                searchItem();
                            }
                            else {
                                swal({
                                    title: "An error occurred!",
                                    text: "Cannot edit product.",
                                    icon: "warning"
                                });
                            }
                        },
                        error: function (result) {
                            console.log(result.responseText);
                        }
                    });
                }
            })
        }
    }
}
// Show Edit Order
function showEditOrder(id) {
    $.ajax({
        url: "/Order/ShowEditOrder",
        data: { ID: id },
        success: function (result) {
            $('#fillModal').html(result);
            $('#mdEdit').modal("show");
        }
    });
}
// Save Edit Status Order
function SaveEditOrder(id) {

    $("#frmEditOrder").validate({
        rules: {
            CusName: {
                required: true
            },
            CusPhone: {
                //required: true,
                number: true,
                minlength: 10
            },
            CusAddress: {
                required: true
            },
            AddPoint: {
                required: true,
                number: true,
                min: 0
            },
            Discount: {
                required: true,
                number: true
            }
        },
        messages: {
            CusName: {
                required: "Vui lòng không để trống"
            },
            CusPhone: {
                //required: "Vui lòng không để trống",
                number: "Vui lòng nhập số",
                minlength: "Số ĐT phải có ít nhất 10 chứ số"
            },
            CusAddress: {
                required: "Vui lòng không để trống"
            },
            AddPoint: {
                required: "Vui lòng không để trống",
                number: "Vui lòng nhập số",
                min: "Không chấp nhận số âm"
            },
            Discount: {
                required: "Vui lòng không để trống",
                number: "Vui lòng nhập số"
            }
        },
        submitHandler: function () {

            var status = $('#mdEdit #slStatus').val();
            var addPoint = $("#mdEdit #txtAddPoint").val().trim();
            var buyerName = $("#mdEdit #txtCusName").val().trim();
            var buyerPhone = $("#mdEdit #txtPhone").val().trim();
            var buyerAddress = $("#mdEdit #txtAddress").val().trim();
            //var disCount = $("#mdEdit #txtDiscount").val().replace(/,/g, '');
            var totalPrice = cms_decode_currency_format($("#mdEdit #Pay").html());
            var discount = cms_decode_currency_format($("#mdEdit #textMoneyDiscount").html());
            $('#mdEdit').modal('hide');
            $("#modalLoad").modal("show");
            $.ajax({
                url: "/Order/SaveEditOrder",
                data: {
                    ID: id,
                    Status: status,
                    AddPoint: addPoint,
                    BuyerName: buyerName,
                    BuyerPhone: buyerPhone,
                    BuyerAddress: buyerAddress,
                    TotalPrice: totalPrice,
                    Discount: discount
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        //$('#mdEdit').modal('hide');
                        $("#modalLoad").modal("hide");
                        swal({
                            title: "",
                            text: "Cập nhật Successful",
                            icon: "success"
                        });
                        setTimeout(function () {
                            searchOrder();
                        }, 1000);

                    }
                    else {
                        //$('#mdEdit').modal('hide');
                        swal({
                            title: "",
                            text: "error !!",
                            icon: "warning"
                        });
                    }
                }
            });
        }
    });
}


// delete order
function deleteOrder(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    } else {
        swal({
            title: "Are you sure you want to delete?",
            text: "",
            icon: "warning",
            buttons: ["Cancel", "OK"],
            //exit: {text:"Exit"},
            dangerMode: true,
        }).then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Order/DeleteOrder',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            });
                            searchOrder();
                        } else {
                            swal({
                                title: "Can not delete!",
                                text: "Error.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }

        })
    }
}


// Search Agent
function searchAgent() {
    var phone = $('#txtPhone').val().trim();
    var name = $('#txtName').val().trim();
    var fd = $('#txtFromDate').val();
    var td = $('#txtToDate').val();

    $.ajax({
        url: "/Agent/Search",
        data: {
            Page: 1,
            Phone: phone,
            Name: name,
            FromDate: fd,
            ToDate: td
        },
        success: function (result) {
            $('#list').html(result);
        }
    });
}

//function createAgent() {
//    $("#frmCreate").validate({
//        rules: {
//            Name: { required: true },
//            Phone: { required: true },
//            Email: { required: true },
//            Password: { required: true },
//            ConfirmPassword: { required: true },
//            Address: { required: true }
//        },
//        messages: {
//            Name: { required: "Vui lòng không để trống" },
//            Phone: { required: "Vui lòng không để trống" },
//            Email: { required: "Vui lòng không để trống" },
//            Password: { required: "Vui lòng không để trống" },
//            ConfirmPassword: { required: "Vui lòng không để trống" },
//            Address: { required: "Vui lòng không để trống" }
//        },
//        submitHandler: function () {
//            $.ajax({
//                url: "/Agent/CreateAgent",
//                data: $('#frmCreate').serialize(),
//                success: function (result) {
//                    if (result == SUCCESS) {
//                        swal({
//                            title: "Thêm Successful ",
//                            text: "",
//                            icon: "success"
//                        });
//                        $('#createAgent').modal("hide");
//                        setTimeout(function () {
//                            searchAgent();
//                        }, 1000);
//                    }
//                    else
//                        if (result == EXISTING) {
//                            swal({
//                                title: "Không thể tạo đại lý.",
//                                text: "Mã đại lý đã tồn tại. Vui lòng dùng mã khác.",
//                                icon: "warning"
//                            })
//                        }
//                        else {
//                            swal({
//                                title: "Thất Bại, error ! ",
//                                text: "",
//                                icon: "warning"
//                            });
//                        }
//                }
//            })
//        }
//    });
//}

//function createAgent() {
//    var Phone = $.trim($('#PhoneCreate').val())
//    var Name = $.trim($('#NameCreate').val())
//    var Email = $.trim($('#EmailCreate').val())
//    var Password = $.trim($('#PasswordCreate').val())
//    var PasswordConfirm = $.trim($('#PasswordConfirmCreate').val())
//    var Address = $.trim($('#AddressCreate').val())

//    if (Phone == "" || Name == "" || Email == "" || Password == "" || PasswordConfirm == "" || Address == "") {
//        swal({
//            title: "",
//            text: "Please enter full information",
//            icon: "warning"
//        })
//        return
//    } else if (PasswordConfirm != Password) {
//        swal({
//            title: "",
//            text: "Mật khẩu không khớp, vui lòng nhập lại",
//            icon: "warning"
//        })
//        return
//    } else {
//        $.ajax({
//            url: "Agent/CreateAgent",
//            data: $('#frmCreate').serialize(),
//            success: function (response) {
//                if (response == SUCCESS) {
//                    swal({
//                        title: "Successful",
//                        text: "",
//                        icon: "success"
//                    })
//                    $('#createAgent').modal(hide)
//                    searchAgent()
//                } else if (reponse == EXISTING) {
//                    swal({
//                        title: "Không thể tạo sản phẩm",
//                        text: "Email hoặc số điện thoại đã được đăng ký.",
//                        icon: "warning"
//                    })
//                } else {
//                    swal({
//                        title: "An error occurred",
//                        text: "Không thể tạo Agent",
//                        icon: "warning"
//                    })
//                }
//            },
//            errorr: function (response) {
//                console.log(response.reponseText)
//            }
//        })
//    }
//}

function createAgent() {
    var Phone = $.trim($('#PhoneCreate').val())
    var Name = $.trim($('#NameCreate').val())
    var Email = $.trim($('#EmailCreate').val())
    var Password = $.trim($('#PasswordCreate').val())
    var PasswordConfirm = $.trim($('#PasswordConfirmCreate').val())
    var Address = $.trim($('#AddressCreate').val())
    //var Lati = $('#lati').val().trim();
    //var Long = $('#long').val().trim();
    //var PlusCode = $('#plusCode').val().trim();

    var reEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    var rePhone = /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/

    if (Phone == "" || Name == "" || Email == "" || Password == "" || PasswordConfirm == "" || Address == "") {
        swal({
            title: "",
            text: "Please enter full information",
            icon: "warning"
        })
        return
    } else if (Password != PasswordConfirm) {
        swal({
            title: "",
            text: "Confirmation password does not match",
            icon: "warning"
        })
        return
    } else if (reEmail.test(Email) == false) {
        swal({
            title: "",
            text: "Email invalidate",
            icon: "warning"
        })
    } else if (rePhone.test(Phone) == false) {
        swal({
            title: "",
            text: "The phone number is not in the correct format",
            icon: "warning"
        })
    } else {
        $.ajax({
            url: "/Agent/CreateAgent",
            data: $('#frmCreate').serialize(),
            success: function (response) {
                if (response == EXISTING) {
                    swal("Không thể tạo đại lý", "Số điện thoại hoặc Email đã được sử dụng", "warning");
                } else if (response == SUCCESS) {
                    swal("Successful!", "", "success");
                    $('#createAgent').modal('hide');
                    searchAgent();
                } else {
                    swal("An error occurred!", "Không thể tạo đại lý.", "warning");
                }
            }
        });
    }
}



// Show Edit Agent
function showEditAgent(id) {
    $.ajax({
        url: "/Agent/ShowEditForm",
        data: { ID: id },
        success: function (result) {
            $('#fill').html(result);
            $('#mdEdit').modal("show");
        }
    });
}

// Save Edit Agent

function saveEditAgent(id) {
    $("#frmUpdate").validate({
        rules: {
            Name: { required: true },
            Address: { required: true }
        },
        messages: {
            Name: { required: "Vui lòng không để trống" },
            Address: { required: "Vui lòng không để trống" }
        },
        submitHandler: function () {
            $.ajax({
                url: "/Agent/SaveEdit",
                data: {
                    ID: id,
                    Name: $('#txtNameEdit').val().trim(),
                    Address: $('#txtAddressEdit').val().trim()
                },
                success: function (result) {
                    if (result == SUCCESS) {
                        swal({
                            title: "Cập Nhật Successful ",
                            text: "",
                            icon: "success"
                        });
                        $('#mdEdit').modal("hide");
                        setTimeout(function () {
                            searchAgent();
                        }, 1500);
                    }
                    else if (result = -1) {
                        swal({
                            title: "Thất Bại, Không tìm thấy SĐT  !! ",
                            text: "",
                            icon: "warning"
                        });
                    }
                    else {
                        swal({
                            title: "Thất Bại, error !! ",
                            text: "",
                            icon: "warning"
                        });
                    }
                }
            })
        }
    });
}

// Delete Agent
function deleteAgent(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Are you sure to delete?",
            text: "",
            icon: "warning",
            buttons: ["Cancel", "OK"],
            dangerMode: true,
        }).then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Agent/DeleteAgent',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Delete successful!",
                                text: "",
                                icon: "success"
                            })
                            searchAgent();
                        } else {
                            swal({
                                title: "Can not delete!",
                                text: "error.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result) {
                        console.log(result.responseText);
                    }
                });
            }
        })
    }
}

// hủy kích hoạt đại lý

function cancelActiveAgent(id) {
    if (!navigator.onLine) {
        swal({
            title: "Check internet connection!",
            text: "",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Bạn chắc chắn muốn hủy kích hoạt đại lý này chứ!",
            text: "",
            icon: "warning",
            buttons: ["Cancel", "OK"],
            dangerMode: true,
        }).then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Agent/cancelActive',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "Hủy Kích Hoạt Successful",
                                text: "",
                                icon: "success"
                            });
                            $("#mdEdit").modal("hide");
                            setTimeout(function () {
                                searchAgent();
                            }, 1000);
                        } else {
                            swal({
                                title: "error!",
                                text: "Vui lòng phản hồi với bộ phận hỗ trợ.",
                                icon: "warning"
                            })
                        }
                    },
                    error: function (result, status, err) {
                        console.log(result.responseText);
                        console.log(status.responseText);
                        console.log(err.Message);
                    }
                });
            }
        })
    }
}





//Nút tìm kiếm ServiceCategory
function SearchServiceCategory() {
    var _name = $.trim($('#txtCateMa').val());
    var _isActive = $('#txtCateIsActive').val();
    var _formDate = $('#txtCateFromdate').val();
    var _toDate = $('#txtCateTodate').val();
    $.ajax({
        url: '/ServiceCategory/SearchServiceCategory',
        data: {
            Page: 1,
            name: _name,
            isActive: _isActive,
            FromDate: _formDate,
            ToDate: _toDate,
        },
        success: function (response) {
            $('#ListCategory').html(response);
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}


function saveCreateShop() {
    var name = $.trim($('#Name').val());
    var contactName = $.trim($('#ContactName').val());
    var contactPhone = $.trim($('#ContactPhone').val());
    var province = $('#createShop #ProvinceID').val();
    var district = $('#slDistrictShopCreate').val();
    var place = $('#place').val();
    var long = $('#Long').val();
    var lat = $('#Lati').val();
    var address = $.trim($('#Address').val());
    var url = $(".imgCreateShop").attr("src");

    //if (name == "") {
    //    swal({
    //        title: "",
    //        text: "Mời nhập tên cửa hàng!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else if (contactName == "") {
    //    swal({
    //        title: "",
    //        text: "Mời nhập tên người liên hệ!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else if (province < 1 || district < 1) {
    //    swal({
    //        title: "",
    //        text: "Vui lòng chọn Tỉnh/Thành Quận/Huyện!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else if (address == "") {
    //    swal({
    //        title: "",
    //        text: "Mời nhập tên địa chỉ!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else if (!isNumeric(contactPhone)) {
    //    swal({
    //        title: "",
    //        text: "Số điện thoại chỉ được nhập số!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else if (url == null) {
    //    swal({
    //        title: "",
    //        text: "Vui lòng chọn ít nhất 1 ảnh!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else if (place == "") {
    //    swal({
    //        title: "",
    //        text: "Mời nhập vào địa chỉ URL!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //if (long == "" || lat == "") {
    //    swal({
    //        title: "",
    //        text: "Vui lòng chọn đúng và xác nhận Url maps!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    //else {
    $.ajax({
        url: "/Shop/CreateShop",
        data: $('#form_create_shops').serialize(),
        success: function (response) {
            if (response == EXISTING) {
                swal("Không thể tạo cửa hàng", "Vị trí bạn chọn đã được sử dụng cho 1 cửa hàng khác.", "warning");
            }
            else if (response == SUCCESS) {
                swal("Successful!", "", "success");
                $('#createShop').modal('hide');
                SearchShop();
                resetInputShop();
            }
            else {
                swal("An error occurred!", "Không thể tạo cửa hàng.", "warning");
            }
        }
    });
    //}
}

// Cấu hình hạng thành viên
function updateCustomerRank(id) {
    debugger;
    var Name = $("#name").val().trim();
    var MinPoint = $("#min_point").val().trim().replace(/,/g, '');
    //var IsActive = $("#is_active").val().trim();
    var MaxPoint = $("#max_point").val().trim().replace(/,/g, '');
    var Description = $("#desc_ranks").val().trim();
    var ProfitCash = $("#profit_cash").val().trim();
    var ProfitVPN = $("#profit_vpn").val().trim();
    var PointBonus = $("#point_bonus").val().trim().replace(/,/g, '');
    var ProfitExtraBirthDay = $("#profit_birthday").val().trim().replace(/,/g, '');
    var Policy = $.trim(CKEDITOR.instances['policy_ranks'].getData());
    if (Name == "" || MinPoint == "" || MaxPoint == "" || Description == "" || ProfitCash == "" || ProfitVPN == "" || PointBonus == "" || ProfitExtraBirthDay == "" || Policy == "") {
        swal({
            title: "Vui lòng nhập đầy đủ thông tin!",
            icon: 'warning'
        })
        return;
    }
    $.ajax({
        url: '/Config/updateCustomerRank',
        type: 'POST',
        dataType: "json",
        contentType: 'application/json',
        data: JSON.stringify({
            ID: id,
            Name: Name,
            MinPoint: MinPoint,
            MaxPoint: MaxPoint,
            Description: Description,
            ProfitCash: ProfitCash,
            ProfitVPN: ProfitVPN,
            PointBonus: PointBonus,
            ProfitExtraBirthDay: ProfitExtraBirthDay,
            Policy: Policy
        }),
        success: function (e) {
            if (e == SUCCESS) {
                swal("Thông báo", "Cập nhật thành công", "success");
                window.location = '/Config/Index';

            }
            else {
                swal("Thông báo", "Cập nhật thất bại", "warning");
            }
        }
    })
}

function loadModalEditCustomerRank($id) {
    $.ajax({
        url: '/Config/ModalEditCustomerRank',
        beforeSend: function () {
            $('#modalLoad').modal("show");
        },
        data: { ID: $id },
        beforeSend: function () {
            $('#modalLoad').modal('show');
        },
        success: function (response) {
            $('#modalLoad').modal("hide");
            $('#modalEditCustomerRank').html(response);
            $('#EditCustomerRank').modal('show');
            //loadListDistrictShop($("#calc_shipping_provinces").val());
            //listUrlImage();
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}



// Chức năng tạo
function createTransportCost() {
    var Type = $("#slTypeService").val().trim();
    var TransportType = $("#TransportType").val().trim();
    var slTypeVehicle = $("#slTypeVehicle").val().trim();
    var FirstDistance = $("#FirstDistance").val().trim();
    var FirstPrice = $("#FirstPrice").val().trim().replace(/,/g, '');
    var PerKmPrice = $("#PerKmPrice").val().trim().replace(/,/g, '');
    var FirstWeight = $("#FirstWeight").val().trim();
    var PerKg = $("#PerKg").val().trim()
    var PerKgPrice = $("#PerKgPrice").val().trim().replace(/,/g, '');
    if (Type == 2) {
        if (Type == "" || TransportType == "" || slTypeVehicle == "" || FirstDistance == "" || FirstPrice == "" || PerKmPrice == "" || FirstWeight == "" || PerKg == "" || PerKgPrice == "") {
            swal({
                title: "Vui lòng nhập đầy đủ thông tin!",
                icon: 'warning'
            })
            return;
        }
    } else {
        if (Type == "" || TransportType == "" || slTypeVehicle == "" || FirstDistance == "" || FirstPrice == "" || PerKmPrice == "" ) {
            swal({
                title: "Vui lòng nhập đầy đủ thông tin!",
                icon: 'warning'
            })
            return;
        }
    }
    

    $.ajax({
        url: '/Config/CreateConfigTransportCost',
        data: {
            Type: Type,
            TransportType: TransportType,
            VehicleTypeID: slTypeVehicle,
            FirstDistance: FirstDistance,
            FirstPrice: FirstPrice,
            PerKmPrice: PerKmPrice,
            FirstWeight: FirstWeight,
            PerKg: PerKg,
            PerKgPrice: PerKgPrice,
        },
        success: function (e) {
            if (e == SUCCESS) {
                swal("Thông báo", "Thêm mới thành công", "success");
                window.location = '/Config/Index';

            }
            else {
                swal("Thông báo", "Thêm mới thất bại", "warning");
            }
        }
    })
}

function createTransportArea() {
    var result = 0;
    var addisProvince = 0;
    var perpricekm = $('#perpricekm').val().replace(',', '');
    var from_address = $('#from_address').val();
    var to_address = $('#to_address').val();
    var perkm = $('#perkm').val();
    var time_ship = $('#time_ship').val();
    var type_transport = $('#type_transport').val();
    var name_transport_area = $('#name_transport_area').val();

    if (perpricekm == "" || from_address == "" || to_address == "" || perkm == "" || time_ship == "" || type_transport == "" || name_transport_area == "") {
        swal({
            title: "Vui lòng nhập đầy đủ thông tin!",
            icon: 'warning'
        })
        return;
    }

    if ($('#val-isprovince').prop('checked') == true) {
        addisProvince = 1;
    }
    if (from_address >= to_address) {
        swal("Thông báo", "Điểm bắt đầu phải nhỏ hơn điểm kết thúc", "warning");
    } else {
        $.ajax({
            url: "/Config/CreateDataConfigTransportArea",
            type: "POST",
            data: $('#frmTransportArea').serialize() + "&IsProvince=" + addisProvince + "&PerKgPrice=" + perpricekm,
            success: function (e) {
                result = e.Status;
                swal({
                    title: e.Message,
                    icon: e.Status == SUCCESS ? "success" : "error"
                }).then((sc) => {
                    if (result == SUCCESS && sc) {
                        location.reload()
                    }
                })
            }

        })
    }


}

function updateTransportArea() {
    var result = 0;
    var isProvince = 0;
    var editPerpricekm = $('#editPerpricekm').val().replace(',', '');


    if ($('#edit-val-isprovince').prop('checked') == true) {
        isProvince = 1;
    }
    var perpricekm = $('#editPerpricekm').val().replace(',', '');
    var from_address = $('#from_addressEdit').val();
    var to_address = $('#to_addressEdit').val();
    var perkm = $('#perkmEdit').val();
    var time_ship = $('#time_shipEdit').val();
    var type_transport = $('#type_transportEdit').val();
    var name_transport_area = $('#name_transport_areaEdit').val();

    if (perpricekm == "" || from_address == "" || to_address == "" || perkm == "" || time_ship == "" || type_transport == "" || name_transport_area == "") {
        swal({
            title: "Vui lòng nhập đầy đủ thông tin!",
            icon: 'warning'
        })
        return;
    }
    $.ajax({
        url: "/Config/UpdateDataConfigTransportArea",
        type: "POST",
        data: $('#frmeditTransportArea').serialize() + "&IsProvince=" + isProvince + "&PerKgPrice=" + editPerpricekm,
        success: function (e) {
            result = e.Status;
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload()
                }
            })
        }

    })
}




// Cập nhật giá dịch vụ
function editTransportCost(id) {
    var Type = $("#slEditTypeService").val().trim();
    var TypeTransport = $("#EditTransportType").val();
    var slTypeVehicle = $("#slEditTypeVehicle").val().trim();
    var FirstDistance = $("#EditFirstDistance").val().trim();
    var FirstPrice = $("#EditFirstPrice").val().trim().replace(/,/g, '');
    var PerKmPrice = $("#EditPerKmPrice").val().trim().replace(/,/g, '');
    var FirstWeight = $("#EditFirstWeight").val().trim();
    var PerKg = $("#EditPerKg").val().trim();
    var PerKgPrice = $("#EditPerKgPrice").val().trim().replace(/,/g, '');
    if (Type == 2) {
        if (Type == "" || TransportType == "" || slTypeVehicle == "" || FirstDistance == "" || FirstPrice == "" || PerKmPrice == "" || FirstWeight == "" || PerKg == "" || PerKgPrice == "") {
            swal({
                title: "Vui lòng nhập đầy đủ thông tin!",
                icon: 'warning'
            })
            return;
        }
    } else {
        if (Type == "" || TransportType == "" || slTypeVehicle == "" || FirstDistance == "" || FirstPrice == "" || PerKmPrice == "") {
            swal({
                title: "Vui lòng nhập đầy đủ thông tin!",
                icon: 'warning'
            })
            return;
        }
    }
    $.ajax({
        url: '/Config/SaveEditConfigTransportCost',
        data: {
            ID: id,
            Type: Type,
            TypeTransport: TypeTransport,
            VehicleTypeID: slTypeVehicle,
            FirstDistance: FirstDistance,
            FirstPrice: FirstPrice,
            PerKmPrice: PerKmPrice,
            FirstWeight: FirstWeight,
            PerKg: PerKg,
            PerKgPrice: PerKgPrice,
        },
        success: function (e) {
            if (e == SUCCESS) {
                swal("Thông báo", "Cập nhật thành công", "success");
                window.location = '/Config/Index';

            }
            else {
                swal("Thông báo", "Cập nhật thất bại", "warning");
            }
        }
    })
}

//Load modal edit cấu hình giá dịch vụ
function loadModalEditTransportCost($id) {
    var id = $id;
    $.ajax({
        url: '/Config/ModalEditConfigTransportCost',
        beforeSend: function () {
            $('#modalLoad').modal("show");
        },
        data: { ID: $id },
        success: function (response) {
            $('#modalLoad').modal("hide");
            $('#modalEditTransportCost').html(response);
            $('#EditTransportCost').modal('show');
            //loadListDistrictShop($("#calc_shipping_provinces").val());
            //listUrlImage();
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

// Sửa giá 
function loadModalEditTransportArea($id) {
    var id = $id;
    $.ajax({
        url: '/Config/ModalEditTransportArea',
        beforeSend: function () {
            $('#modalLoad').modal("show");
        },
        data: { ID: $id },
        success: function (response) {
            $('#modalLoad').modal("hide");
            $('#modalEditTransportArea').html(response);
            $('#EditTransportArea').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}


// Chi tiết cấu hình giá chuyển phát nhanh
function loadModelTransportAreaDetail($id) {
    $.ajax({
        url: '/Config/ModalDetailTransportArea',
        beforeSend: function () {
            $('#modalLoad').modal("show");
        },
        data: { transportAreaID: $id },
        success: function (response) {
            $('#modalLoad').modal("hide");
            $('#modalDetailTransportArea').html(response);
            $('#val-transport-area-id').val($id);
            $('#DetailTransportArea').modal('show');
        },
        error: function (result) {
            console.log(result.responseText);
        }
    });
}

// Xóa giá dịch vụ chuyển phát nhanh
function deleteTransportArea(id) {
    swal({
        title: "Bạn chắc chắn muốn xóa bản ghi này không?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Config/DeleteDataConfigTransportArea',
                type: 'POST',
                data: { ID: id },
                success: function (res) {
                    if (res) {
                        swal({
                            title: "",
                            text: "Xóa bản ghi thành công",
                            icon: "success"
                        });
                        window.location = '/Config/Index';

                    }
                    else {
                        swal({
                            title: "",
                            //text: res.Exception,
                            text: "Xóa bản ghi thất bại",
                            icon: "error"
                        })
                    }
                }
            })
        }
    })
}

// Thêm tài xế
function saveAddShip() {
    var avatar = $('#avatar').val();
    var cmnd = $("#card-id").val().replace(/\s\s+/g, ' ');
    var name = $('#txt-name').val().replace(/\s\s+/g, ' ');
    var phone = $("#txt-phone").val().replace(/\s\s+/g, ' ');
    var email = $("#txt-email").val().replace(/\s\s+/g, ' ');
    var carBrand = $('#txt-car-brand').val().replace(/\s\s+/g, ' ');
    var vehicleType = $('#val-verhicle-id').val();
    var carModel = $('#txt-car-model').val().replace(/\s\s+/g, ' ');
    var commission = $('#val-id-commission').val();
    var numberPlate = $('#txt-number-plate').val().replace(/\s\s+/g, ' ');
    var commissionID = $('#val-id-commission').val();
    var isVip = $('#isVip').val();
    var cardImgShiper = "";
    var lstIdArea = "";
    var isInternal = 0;
    if ($('#val-is-internal').prop('checked') == true)
        isInternal = 1;

    $.each($('.card-img-shiper'), function () {
        cardImgShiper += $(this).attr('data-value') + ',';
    })
    $.each($('#shiper-area-table > .val-district-id'), function () {
        lstIdArea += $(this).attr('data-id') + ',';
    })
    var phone_validate = new RegExp("^[0-9]{9,11}");

    var reEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    if (avatar.length == 0) {
        swal({
            title: "Mời chọn ảnh đại diện!",
            text: "",
            icon: "warning"
        })
        return;
    }


    if (name.length == 0) {
        swal({
            title: "",
            text: "Mời nhập tên tài xế!",
            icon: "warning"
        })
        return;
    }

    if (cmnd.length == 0) {
        swal({
            title: "",
            text: "Mời nhập CMND/CCCD!",
            icon: "warning"
        })
        return;
    }

    if (phone.length == 0) {
        swal({
            title: "",
            text: "Mời nhập số điện thoại!",
            icon: "warning"
        })
        return;
    }

    if (!phone_validate.test(phone)) {
        swal({
            title: "",
            text: "Mời nhập số điện thoại bằng số, tối đa 11 chữ số!",
            icon: "warning"
        })
        return;
    }

    if (email.length == 0) {
        swal({
            title: "",
            text: "Mời nhập địa chỉ email!",
            icon: "warning"
        })
        return;
    }
    if (commissionID == null) {
        swal({
            title: "",
            text: "Vui lòng chọn mức hoa hồng cho tài xế!",
            icon: "warning"
        })
        return;
    }

    if (!reEmail.test(email)) {
        swal({
            title: "",
            text: "Mời nhập địa chỉ email theo dạng example@exp.exp",
            icon: "warning"
        })
        return;
    }

    if (vehicleType == null) {
        swal({
            title: "",
            text: "Mời chọn loại xe",
            icon: "warning"
        })
        return;
    }

    if (carBrand.length == 0) {
        swal({
            title: "",
            text: "Mời nhập dòng xe!",
            icon: "warning"
        })
        return;
    }

    if (carModel.length == 0) {
        swal({
            title: "",
            text: "Mời nhập hãng xe!",
            icon: "warning"
        })
        return;
    }


    if (numberPlate.length == 0) {
        swal({
            title: "",
            text: "Mời nhập biển số xe!",
            icon: "warning"
        })
        return;
    }

    if (commission == null) {
        swal({
            title: "",
            text: "Mời chọn mức hoa hồng cho tài xế!",
            icon: "warning"
        })
        return;
    }

    if (cardImgShiper.length == 0) {
        swal({
            title: "",
            text: "Mời thêm ảnh CMDD/CCCD của tài xế!",
            icon: "warning"
        })
        return;
    }

    if (lstIdArea.length == 0) {
        swal({
            title: "",
            text: "Mời thêm khu vực hoạt động của tài xế!",
            icon: "warning"
        })
        return;
    }
    var result = 0;
    $.ajax({
        url: '/Shipper/CreateShipper',
        data: $('#frm-add-shiper').serialize() + "&ShipperProvinces=" + lstIdArea + "&ImgIdentify=" + cardImgShiper + "&IsInternal=" + isInternal,
        beforeSend: function () {
            $("#modalLoad").modal('show');
        },
        type: "POST",
        success: function (e) {
            result = e.Status;
            $("#modalLoad").modal('hide');
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    window.location = "/Shipper/Index";
                }
            })
        }
    })

}

// Sửa tài xế (Shipper)
function SaveEditShiperInfo() {
    var cmnd = $("#card-id").val().replace(/\s\s+/g, ' ');
    var name = $('#txt-name').val().replace(/\s\s+/g, ' ');
    var phone = $("#txt-phone").val().replace(/\s\s+/g, ' ');
    var email = $("#txt-email").val().replace(/\s\s+/g, ' ');
    var carBrand = $('#txt-car-brand').val().replace(/\s\s+/g, ' ');
    var vehicleType = $('#val-verhicle-id').val();
    var carModel = $('#txt-car-model').val().replace(/\s\s+/g, ' ');
    var commission = $('#val-id-commission').val();
    var numberPlate = $('#txt-number-plate').val().replace(/\s\s+/g, ' ');
    var commissionID = $('#val-id-commission').val();
    var isVip = $('#isVip').val();
    var cardImgShiper = "";
    var lstIdArea = "";
    var isInternal = 0;
    if ($('#val-is-internal').prop('checked') == true)
        isInternal = 1;

    $.each($('.img > .card-img-shiper'), function () {
        cardImgShiper += $(this).attr('data-value') + ',';
    })
    $.each($('#shiper-area-table > .val-district-id'), function () {
        lstIdArea += $(this).attr('data-id') + ',';
    })
    var phone_validate = new RegExp("^[0-9]{9,11}");

    var reEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    if (name.length == 0) {
        swal({
            title: "",
            text: "Mời nhập tên tài xế!",
            icon: "warning"
        })
        return;
    }

    if (cmnd.length == 0) {
        swal({
            title: "",
            text: "Mời nhập CMND/CCCD!",
            icon: "warning"
        })
        return;
    }

    if (phone.length == 0) {
        swal({
            title: "",
            text: "Mời nhập số điện thoại!",
            icon: "warning"
        })
        return;
    }

    if (!phone_validate.test(phone)) {
        swal({
            title: "",
            text: "Mời nhập số điện thoại bằng số, tối đa 11 chữ số!",
            icon: "warning"
        })
        return;
    }

    if (email.length == 0) {
        swal({
            title: "",
            text: "Mời nhập địa chỉ email!",
            icon: "warning"
        })
        return;
    }
    if (commissionID == null) {
        swal({
            title: "",
            text: "Vui lòng chọn mức hoa hồng cho tài xế!",
            icon: "warning"
        })
        return;
    }

    if (!reEmail.test(email)) {
        swal({
            title: "",
            text: "Mời nhập địa chỉ email theo dạng example@exp.exp",
            icon: "warning"
        })
        return;
    }

    if (vehicleType == null) {
        swal({
            title: "",
            text: "Mời chọn loại xe",
            icon: "warning"
        })
        return;
    }

    if (carBrand.length == 0) {
        swal({
            title: "",
            text: "Mời nhập dòng xe!",
            icon: "warning"
        })
        return;
    }

    if (carModel.length == 0) {
        swal({
            title: "",
            text: "Mời nhập hãng xe!",
            icon: "warning"
        })
        return;
    }


    if (numberPlate.length == 0) {
        swal({
            title: "",
            text: "Mời nhập biển số xe!",
            icon: "warning"
        })
        return;
    }

    if (commission == null) {
        swal({
            title: "",
            text: "Mời chọn mức hoa hồng cho tài xế!",
            icon: "warning"
        })
        return;
    }

    if (cardImgShiper.length == 0) {
        swal({
            title: "",
            text: "Mời thêm ảnh CMDD/CCCD của tài xế!",
            icon: "warning"
        })
        return;
    }

    if (lstIdArea.length == 0) {
        swal({
            title: "",
            text: "Mời thêm khu vực hoạt động của tài xế!",
            icon: "warning"
        })
        return;
    }
    var result = 0;

    $.ajax({
        url: "/Shipper/EditShipper",
        type: "POST",
        data: $('#frm-edit-shiper-info').serialize() + "&ShipperProvinces=" + lstIdArea + "&ImgIdentify=" + cardImgShiper + "&IsInternal=" + isInternal,
        beforeSend: function () {
            $("#modalLoad").modal('show');
        },
        success: function (e) {
            result = e.Status;
            $("#modalLoad").modal('hide');
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    window.location = "/Shipper/Index";
                }
            })
        }
    })
}

//Sửa cửa hàng
function saveEditShop() {
    var name = ReplaceSpace($('#txt-name').val());
    $('#txt-name').val(name);
    var email = ReplaceSpace($('#txt-email').val());
    $('#txt-email').val(email);
    var phone = ReplaceSpace($('#txt-phone').val());
    $('#txt-phone').val(phone);
    var address = ReplaceSpace($('#txt-address').val());
    $('#txt-address').val(address);
    var district = $('#val-district-id').val();
    var checkEmailAndPhone = checkEmailPhone(email, phone);

    if (name.length == 0) {
        swal({
            title: "",
            text: "Mời nhập tên cửa hàng!",
            icon: "warning"
        })
        return;
    }

    if (address.length == 0) {
        swal({
            title: "",
            text: "Mời nhập địa chỉ shop!",
            icon: "warning"
        })
        return;
    }
    if (checkEmailAndPhone == 1) {
        swal({
            title: "",
            text: "Email sai định dạng",
            icon: "warning"
        })
        return;
    }

    if (checkEmailAndPhone == 2) {
        swal({
            title: "",
            text: "Số điện thoại sai định dạng",
            icon: "warning"
        })
        return;
    }

    if (district == null) {
        swal({
            title: "",
            text: "Vui lòng chọn lại tỉnh/thành phố, quận/huyện!",
            icon: "warning"
        })
        return;
    }

    var result = 0;
    $.ajax({
        url: '/Shop/UpdateShopInfo',
        type: 'POST',
        data: $('#frm-edit-shop-info').serialize(),
        beforeSend: function () {
            $("#modalLoad").modal('show');
        },
        success: function (e) {
            result = e.Status;
            $("#modalLoad").modal('hide');
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload();
                }
            })
        },
        error: function (er) {
            $("#modalLoad").modal('hide');
            console.log(er)
        }
    })

}



//Load Place to input
function LoadPlaceEditShop() {
    if ($('#_Place').val() != "") {
        var longlat = /\/\@(.*),(.*),/.exec($('#_Place').val());
        lat = longlat[1];
        lng = longlat[2];
        $('#_Lati').val(lat);
        $('#_Long').val(lng);
    }
    else {
        swal("!", "Mời nhập vào địa chỉ UrL", "warning");
    }
}

//Load Place to input
function LoadPlaceCreateShop() {
    if ($('#place').val() != "") {
        var longlat = /\/\@(.*),(.*),/.exec($('#place').val());
        if (longlat == null) {
            swal("", "Vui lòng chọn đúng đường dẫn google map", "warning");
        }
        lat = longlat[1];
        lng = longlat[2];
        if (lat == "" || lng == "") {
            swal("", "Vui lòng chọn đúng đường dẫn google map", "warning");
        }
        $('#Lati').val(lat);
        $('#Long').val(lng);
    }
    else {
        swal("!", "Mời nhập vào địa chỉ UrL", "warning");
    }
}

//reset text to default
function resetInputShop() {
    $('#Name').val('');
    $('#ContactName').val('');
    $('#ContactPhone').val('');
    $('#Address').val();
    $('#place').val('');
    $('#Lati').val('');
    $('#Long').val('');
    //$('.Img').remove();
    $("#AddImgLogoPlace").val('');
}
//clear data modal Thêm
function clearModalAdd() {
    $('#addNameCategory').val('');
    $('#addOrderCategory').val('');
    $("#AddImgLogoPlace").val('');
    //$('img').attr('src', '');
    //$('img').attr('data-value', '');
    //$('img').removeAttr('src');
    //$('img').removeAttr('data-value');
    $("#AddLogoPlace").html('<i class="fa fa-camera" style="font-size:30px;"></i>');

}

//reser text 
function resetInputItem() {
    $('#CodeCreate').val("");
    $('#NameCreate').val("");
    $('#PriceCreate').val();
    $('#DivtagImg').remove();
}

//Lấy ra list url của shop cần sửa
function listUrlImage() {
    var url = "";
    $('._lstImage').each(function () {
        if (url == "") {
            url = $(this).attr('src');
        }
        else {
            url = url + "," + $(this).attr('src');
        }
    });
    $('#_txturlImage').val(url);
}

//Load modal edit shop
function loadModalEditShop($id) {
    var id = $id;
    $.ajax({
        url: '/Shop/loadModalEditShop',
        data: { ID: $id },
        beforeSend: function () {
            $('#modalLoad').modal('show');
        },
        success: function (response) {
            $('#modalLoad').modal('hide');
            $('#modalEditShop').html(response);
            $('#EditShop').modal('show');
            listUrlImage();
        },
        error: function (result) {
            $('#modalLoad').modal('hide');
            console.log(result.responseText);
        }
    });
}

//Delete Shop
function DeleteShop($id) {
    var id = $id
    swal({
        title: "Bạn chắc chắn muốn xóa?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((isConFirm) => {
            if (isConFirm) {
                $.ajax({
                    url: '/Shop/DeleteShop',
                    data: { ID: id },
                    success: function (response) {
                        if (response == SUCCESS) {
                            swal({
                                title: "",
                                text: "Xóa thành công!",
                                icon: "success"
                            }).then((sc) => {
                                if (sc) {
                                    location.reload();
                                }
                            });
                        }
                        else {
                            swal({
                                title: "",
                                text: "Lỗi!",
                                icon: "warning"
                            });
                        }
                    }
                });
            }
        })
}

// Import Data Agent
function ImportAgent() {
    var fileUpload = $('#txtFile').get(0);
    var files = fileUpload.files;
    var formData = new FormData();
    formData.append('ExcelFile', files[0]);

    $.ajax({
        type: 'POST',
        url: '/Agent/ImportData',
        contentType: false,
        processData: false,
        data: formData,
        success: function (result) {
            if (result == 1) {
                swal({
                    title: "Import Successful",
                    text: "",
                    icon: "success"
                });
                searchAgent();
            }
            else if (result == -1) {
                swal({
                    title: "Hãy Chọn Một File Excel !",
                    text: "error.",
                    icon: "warning"
                });
            }
            else if (result == 0) {
                swal({
                    title: "Mã Đại Lý Đã Tồn Tại !",
                    text: "error.",
                    icon: "warning"
                });
            }
            else if (result == 3) {
                swal({
                    title: "File import không có dữ liệu",
                    text: "vui lòng kiểm tra lại !",
                    icon: "warning"
                });
            }
            else {
                swal({
                    title: "Sai Định Dạng File !",
                    text: "error.",
                    icon: "warning"
                });
            }
            setTimeout(function () {
                $('#mdImport').modal('hide');
            }, 1000);
        }
    })
}


// Import Data Card
function importCard() {
    var fileUpload = $('#inputExcelFile').get(0);
    var files = fileUpload.files;
    var formData = new FormData();
    formData.append('ExcelFile', files[0]);

    $.ajax({
        type: 'POST',
        url: '/Card/Import',
        contentType: false,
        processData: false,
        data: formData,

        success: function (result) {
            if (result == 1) {
                swal({
                    title: "Import Successful",
                    text: "",
                    icon: "success"
                });
                searchCard();
            }
            else if (result == -1) {
                swal({
                    title: "Hãy Chọn Một File Excel !",
                    text: "error.",
                    icon: "warning"
                });
            }
            else if (result == 0) {
                swal({
                    title: "Seri hoặc Mã thẻ Đã Tồn Tại !",
                    text: "Vui lòng kiểm tra lại",
                    icon: "warning"
                });
            }
            else if (result == -3) {
                swal({
                    title: "File Import Chưa Có Dữ Liệu !",
                    text: "Vui lòng kiểm tra lại",
                    icon: "warning"
                });
            }
            else if (result == -4) {
                swal({
                    title: "Error !",
                    text: "Vui lòng liên vệ với bộ phận hỗ trợ",
                    icon: "warning"
                });
            }
            else if (result == -5) {
                swal({
                    title: "Mã thẻ hoặc seri phải hơn 10 kí tự",
                    text: "Vui lòng kiểm tra lại dữ liệu",
                    icon: "warning"
                });
            }
            else if (result > 20000 && result < 25000) {
                swal({
                    title: "Please complete all information",
                    text: "kiểm tra lại dòng " + (result - 20000),
                    icon: "warning"
                });
            }
            else if (result > 25000 && result < 30000) {
                swal({
                    title: "Seri hoặc mã thẻ được tối đa 15 ký tự",
                    text: "kiểm tra lại dòng " + (result - 25000),
                    icon: "warning"
                });
            }
            else if (result > 30000) {
                swal({
                    title: "Seri trùng với mã thẻ",
                    text: "kiểm tra lại dòng " + (result - 30000),
                    icon: "warning"
                });
            }
            else if (result > 1 && result <= 10000) {
                swal({
                    title: "Seri hoặc Mã thẻ Đã Tồn Tại !",
                    text: "vui lòng kiểm tra lại dòng " + result,
                    icon: "warning"
                });
            }
            else if (result > 10000 && result <= 20000) {
                swal({
                    title: "Dữ Liệu Không Hợp Lệ",
                    text: "vui lòng kiểm tra dòng " + (result - 10000),
                    icon: "warning"
                });
            }
            else {
                swal({
                    title: "Sai Định Dạng File !",
                    text: "error.",
                    icon: "warning"
                });
            }
            setTimeout(function () {
                $('#mdImport').modal('hide');
            }, 1000);
        }
    });

}

// Back customer
function backToIndexCustomer(page) {
    $.ajax({
        type: "POST",
        url: "/Customer/GoHome",
        dataType: 'json',
        crossDomain: true,
        success: function (data) {
            window.location.href = data;
        }
    });
    //$.ajax({
    //    type: "POST",
    //    url: '/Customer/Search',
    //    data: { Page: page },
    //    success: function (result) {
    //        $(' #ListCustomer').html(result);
    //    }
    //})
}


//function hideModelBatchDetail() {
//    $('.modal').hide();
//    $('#mdBatchDetail').hide();
//    $('.modal-backdrop').hide();
//}


//Format money in textbox
function cms_encode_currency_format(num) {
    if (!isNumeric(num)) {
        return '';
    }
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
}

function cms_decode_currency_format(obs) {
    if (obs == '')
        return '';
    else
        return parseInt(obs.replace(/,/g, ''));
}

//function checkAllBox(source) {
//    //var status = $(source).is(":checked")

//    //$("input[name=boxChecked]").each(function () {
//    //    $(this).attr("checked", status)
//    //})
//}

function saveAddPoint() {
    var listChecked = ""
    var boxes = $('input[name=boxChecked]:checked')
    // var point = $('#point').val().trim();
    var point = parseInt($("#point").val().replace(/,/g, ''));
    var description = $.trim($("#description").val());
    if (point == 0 || isNaN(point)) {
        swal("Số điểm phải lớn hơn 0", "", "warning");
        return;
    }
    boxes.each(function () {
        var id = $(this).attr('data-id')
        listChecked += id + ","
    })
    if (listChecked == "") {
        swal("Chưa có đại lý nào được chọn", "", "warning");
        return;
    }
    $.ajax({
        url: '/Agent/AddPoint',
        type: 'POST',
        data: {
            ListChecked: listChecked,
            Point: point,
            Description: description
        },
        success: function (responsive) {
            if (responsive == 1) {
                swal("Thêm điểm Successful", "", "success")
                searchAgent()
                $('#addPoint').modal('hide')
            } else {
                swal("error", "", "warning")
                searchAgent()
                $('#addPoint').modal('hide')
            }
        }
    })
}

function saveConfig() {
    //var distance = $('#txtDistance').val().trim();
    //var timewaiting = $('#txtTimeWaiting').val().trim();
    //var pointaddfirst = $('#txtPointAddFirst').val().trim();
    var distance = parseInt($("#txtDistance").val().replace(/,/g, ''));
    var timewaiting = parseInt($("#txtTimeWaiting").val().replace(/,/g, ''));
    var pointaddfirst = parseInt($("#txtPointAddFirst").val().replace(/,/g, ''));
    if (distance == 0 || isNaN(distance) || timewaiting == 0 || isNaN(timewaiting) || pointaddfirst == 0 || isNaN(pointaddfirst)) {
        swal("Values must be greater than 0 and must not be blank", "", "warning");
        return;
    }

    swal({
        title: "You sure want to save the settings?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true
    }).then((response) => {
        if (response) {
            $.ajax({
                url: '/Config/SaveConfig',
                data: {
                    Distance: distance,
                    TimeWaiting: timewaiting,
                    PointAddFirst: pointaddfirst
                },
                success: function (responsive) {
                    if (responsive == 1) {
                        swal("Save setup successful", "", "success")
                    } else {
                        swal("error", "", "warning")
                    }
                }
            })
        }
    })
}

function GetAgentDetail(id) {

    $.ajax({
        url: '/Agent/GetAgentDetail',
        data: { ID: id },
        success: function (result) {
            $('#View').html(result);
        }
    });
}

function resetPassword(id) {
    swal({
        title: "Are you sure you want to reset password?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((reponse) => {
        if (reponse) {
            $.ajax({
                url: '/User/ResetPassword',
                data: { ID: id },
                success: function (reponsive) {
                    if (reponsive == 1) {
                        swal("Password reset Successful", "", "success")
                    } else {
                        swal("error", "", "warning")
                    }
                }
            })
        }
    })
}

function resetPasswordAgent(id) {
    swal({
        title: "Are you sure you want to reset password?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((reponse) => {
        if (reponse) {
            $.ajax({
                url: '/Agent/ResetPassword',
                data: { ID: id },
                success: function (reponsive) {
                    if (reponsive == 1) {
                        swal("Password reset successfully", "", "success")
                    } else {
                        swal("error", "", "warning")
                    }
                }
            })
        }
    })
}

function showAddPonitAgent(id) {
    $('#addPointAgent').modal('show')
}

function checkEmailPhone(Email, Phone) {
    var reEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    var rePhone = /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/
    if (!reEmail.test(Email)) {
        return 1;
    }
    if (!rePhone.test(Phone)) {
        return 2;
    }

    return 0;
}

//Sửa thông tin đại lý GasViett
function saveEditInforAgent(id) {
    var name = $("#txtNameEdit").val().trim();
    var phone = $("#phoneEdit").val().trim();
    var birthday = $("#bdEdit").val().trim();
    var email = $("#txtEmailEdit").val().trim();
    var address = $("#AddressCreate2").val().trim();
    var sex = ($('input[name=inlineRadioOptions]:checked', '#gender').val());
    var lati = $('#lati2').val().trim();
    var long = $('#long2').val().trim();


    var reEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    var rePhone = /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/

    if (name == null || phone == null || birthday == null || email == null || address == null) {
        swal({
            title: "Please complete all information!",
            text: "",
            icon: "warning",
        });
        return;
    } else if (reEmail.test(email) == false) {
        swal({
            title: "",
            text: "Email invalidate",
            icon: "warning"
        })
    } else if (rePhone.test(phone) == false) {
        swal({
            title: "",
            text: "The phone number is not in the correct format",
            icon: "warning"
        })
    } else {
        $.ajax({
            url: '/Agent/SaveEditInforAgent',
            data: {
                ID: id,
                Name: name,
                Phone: phone,
                DOB: birthday,
                Address: address,
                Email: email,
                Sex: sex,
                Lati: lati,
                Long: long
            },
            type: 'POST',
            success: function (reponsive) {
                if (reponsive == 1) {
                    swal({
                        title: "Successful!",
                        text: "",
                        icon: "success",
                    });
                    window.location = "/Agent/Index";
                } else {
                    if (reponsive == EXISTING) {
                        swal({
                            title: "error!",
                            text: "This phone number is already taken",
                            icon: "warning",
                        });
                    } else {
                        swal({
                            title: "error!",
                            text: "",
                            icon: "warning",
                        });
                        console.log("enterrd")
                    }

                }
            }
        });
    }
}

function intilize() {
    var autocomplete = new google.maps.places.Autocomplete(document.getElementById('AddressCreate'));

    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        var place = autocomplete.getPlace();

        var lati = place.geometry.location.lat();
        var long = place.geometry.location.lng();
        var plusCode = "https://www.google.com/maps/@" + lati + "," + long + ",17z";

        document.getElementById('lati').value = lati;
        document.getElementById('long').value = long;
        document.getElementById('plusCode').value = plusCode;
    })
}

function intilize2() {
    var autocomplete = new google.maps.places.Autocomplete(document.getElementById('AddressCreate2'));

    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        var place = autocomplete.getPlace();

        var lati = place.geometry.location.lat();
        var long = place.geometry.location.lng();
        var plusCode = "https://www.google.com/maps/@" + lati + "," + long + ",17z";

        document.getElementById('lati2').value = lati;
        document.getElementById('long2').value = long;
        document.getElementById('plusCode2').value = plusCode;
    })
}
function showAddViewWasher() {
    window.location.hash = "/add";
    window.location.hash = "/add";
    $.ajax({
        url: '/Washer/AddView',
        type: 'GET',
        success: function (res) {
            $("#View").html(res);
            window.onhashchange = function () { window.location.hash = "/add"; }
        }
    });
}
function comeBacks() {
    //$("#View").empty();
    window.location.href = "/Washer/Index";
}
function backCustomer() {
    window.location.href = "/Customer/Index";
}
function backTransaction() {
    //$("#View").empty();
    window.location.href = "/Transaction/Index";
}
function backCombo() {
    window.location.href = "/Combo/Index";
}
function getWasherDetail(id) {
    window.location.hash = "/detail";
    window.location.hash = "/detail";
    $.ajax({
        url: '/Washer/WasherDetail',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            $("#View").html(res);
            window.onhashchange = function () { window.location.hash = "/detail"; }
        }
    })
}
function backWasher() {
    window.location.href = "/Washer/Index";
}
function backPackage() {
    window.location.href = "/PackageService/Index";
}
function Booking(role) {
    if (role == 2) {
        swal({
            title: "Permission denied",
            text: "",
            icon: "warning"
        });
        return;
    }
    window.location.hash = "/booking";
    window.location.hash = "/booking";
    window.onhashchange = function () { window.location.hash = "/booking"; }
    $.ajax({
        url: '/Transaction/Booking',
        type: 'GET',
        success: function (res) {
            $("#View").html(res);
        }
    })
}



function SaveEditTransaction() {
    var washerID = $("#select-washer").val();
    var orderID = $("#select-washer").attr("orderID");
    $.ajax({
        url: "/Transaction/ChangeWasher",
        type: "POST",
        data: {
            washerID: washerID,
            orderID: orderID
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            if (res.Status == SUCCESS) {
                swal({
                    title: "Success !",
                    text: " Update washer successfully !",
                    icon: "success"
                })
            }
            else {
                swal({
                    title: "Faild !",
                    text: res.Message,
                    icon: "error"
                })
            }
        }
    })
}
function TransactionDetail(id) {
    window.location.hash = "/detail";
    window.location.hash = "/detail";
    $.ajax({
        url: '/Transaction/Detail',
        type: 'GET',
        data: { id: id },
        beforeSend: function () {
            $("#modalLoad").modal('show');
        },
        success: function (res) {
            $("#modalLoad").modal('hide');
            $("#View").html(res);
            window.onhashchange = function () { window.location.hash = "/detail"; }
        }
    })
}
function AddCombo() {
    window.location.hash = "/add";
    window.location.hash = "/add";
    window.onhashchange = function () { window.location.hash = "/add"; }
    $.ajax({
        url: '/Combo/AddComboView',
        type: 'GET',
        success: function (res) {
            $("#View").html(res);
        }
    })
}
function UpdateComboView() {
    window.location.hash = "/detail";
    window.location.hash = "/detail";
    window.onhashchange = function () { window.location.hash = "/detail"; }
    $.ajax({
        url: '/Combo/UpdateComboView',
        type: 'GET',
        success: function (res) {
            $("#View").html(res);
        }
    })
}
function AddPackageServiceView() {
    window.location.hash = "/add";
    window.location.hash = "/add";
    window.onhashchange = function () { window.location.hash = "/add"; }
    $.ajax({
        url: '/PackageService/AddPackageService',
        type: 'GET',
        success: function (res) {
            $("#View").html(res);
        }
    })
}
function PackageServiceDetail(id) {
    window.location.hash = "/detail";
    window.location.hash = "/detail";
    window.onhashchange = function () { window.location.hash = "/detail"; }
    $.ajax({
        url: '/PackageService/PackageServiceDetail',
        type: 'GET',
        data: {
            id: id
        },
        success: function (res) {
            $("#View").html(res);
        }
    })
}
function getReviewsDetail(id) {
    window.location.hash = "/detail";
    window.location.hash = "/detail";
    window.onhashchange = function () { window.location.hash = "/detail"; }
    $.ajax({
        url: '/Reviews/ReviewsDetail',
        data: { ID: id },
        type: 'POST',
        success: function (res) {
            $("#View").html(res);
        }
    })
}
function backReviews() {
    window.location.href = "/Reviews/Index";
}
function CreateWasher() {
    var name = $("#input-add-name").val().trim();
    var phone = $("#input-add-phone").val().trim();
    var email = $("#input-add-email").val().trim();
    var password = $("#input-add-password").val().trim();
    var confirm = $("#input-add-confirm").val().trim();
    var commission = $("#commission").val();
    var inhouse = $("#in-house").prop("checked") ? 1 : 0;
    //var sex = $("#sex").val().trim();
    var dob = $("#dtBirthdayIndex").val().trim();
    var indentification = $("#input-add-identity").val().trim();
    var avatar = $("#AddImgAvartar").attr('src');
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var listArea = [];
    $.each($(".priority-area > tr"), function () {
        listArea.push($(this).find(".district-tr").data('id'))
    });
    var listIdentityImage = [];
    $.each($("._lstImage"), function () {
        listIdentityImage.push($(this).attr('src'));
    });
    //if (name == "" || phone == "" || email == "" || password == "" || sex == "" || dob == "" || indentification == "" ||) {

    //}
    console.log("commission", commission);
    if (avatar == "/Uploads/files/add_img.png") {
        avatar = "";
    }
    console.log("avatar", avatar);
    if (name == "") {
        swal({
            title: "error!",
            text: "Please enter full name",
            icon: "warning",
        });
        return;
    }
    if (name.length > 100) {
        swal({
            title: "error!",
            text: "Full name can only 100 characters",
            icon: "warning",
        });
        return;
    }
    if (phone == "") {
        swal({
            title: "error!",
            text: "please enter phone number",
            icon: "warning",
        });
        return;
    }
    if (!/^[0-9]{10}$/.test(phone)) {
        swal({
            title: "error!",
            text: "Phone is not valid!",
            icon: "warning",
        });
        return;
    }
    if (email == "") {
        swal({
            title: "error!",
            text: "please enter email",
            icon: "warning",
        });
        return;
    }
    if (indentification == "") {
        swal({
            title: "error!",
            text: "please enter identification",
            icon: "warning",
        });
        return;
    }
    if (commission == 0) {
        swal({
            title: "error!",
            text: "please select commission level",
            icon: "warning",
        });
        return;
    }
    if (password == "") {
        swal({
            title: "error!",
            text: "please enter password",
            icon: "warning",
        });
        return;
    }
    if (confirm == "") {
        swal({
            title: "error!",
            text: "please enter confirm password",
            icon: "warning",
        });
        return;
    }
    if (confirm == "") {
        swal({
            title: "error!",
            text: "please enter confirm password",
            icon: "warning",
        });
        return;
    }
    if (dob == "") {
        swal({
            title: "error!",
            text: "please enter Birthday",
            icon: "warning",
        });
        return;
    }
    if (listArea.length == 0) {
        swal({
            title: "error!",
            text: "please select priority area",
            icon: "warning",
        });
        return;
    }
    if (password != confirm) {
        swal({
            title: "error!",
            text: "Confirm  password is not correct",
            icon: "warning",
        });
        return;
    }
    if (!emailReg.test(email)) {
        swal({
            title: "",
            text: "Email is not valid!",
            icon: "warning",
        });
        return;
    }
    if (!/(^[0-9]{9}$)|(^[0-9]{12}$)/.test(indentification)) {
        swal({
            title: "error!",
            text: " Identification invalid",
            icon: "warning",
        });
        return;
    }
    if (!checkdatetime(dob)) {
        swal({
            title: "error!",
            text: " Birthday invalid",
            icon: "warning",
        });
        return;
    }
    $.ajax({
        url: '/Washer/CreateAgent',
        type: 'POST',
        data: {
            Name: name,
            Phone: phone,
            Email: email,
            Password: password,
            Dob: dob,
            identification: indentification,
            AvatarUrl: avatar,
            IdentityImage: listIdentityImage,
            listArea: listArea,
            CommissionID: commission,
            IsInHouse: inhouse
        },
        success: function (res) {
            if (res == 1) {
                swal({
                    title: "",
                    text: "Create new successfully",
                    icon: "success",
                });
                $("#input-add-name").val("");
                $("#input-add-phone").val("");
                $("#input-add-email").val("");
                $("#input-add-password").val("");
                $("#input-add-confirm").val("");
                $("#dtBirthdayIndex").val("");
                $("#input-add-identity").val("");
                $("#AddAvatar").empty();
                $("#AddAvatar").append('<img id="AddImgAvartar" src="/Uploads/files/add_img.png" class="contentImg" alt="your image" onclick="ChangeImage($(this));"/>');
                $(".priority-area").empty();
                $("#_divAddImg").empty();
            }
            else {
                swal({
                    title: "error!",
                    text: "Phone number already in use",
                    icon: "error",
                });
            }
        }
    });
}
function UpdateWasher(id) {
    var name = $("#input-edit-name").val().trim();
    //var phone = $("#input-edit-phone").val().trim();
    var email = $("#input-edit-email").val().trim();
    var commission = $("#commission").val();
    var inhouse = $("#in-house").prop("checked") ? 1 : 0;
    //var sex = $("#sex").val().trim();
    var dob = $("#dtBirthdayIndex").val().trim();
    var indentification = $("#input-edit-identity").val().trim();
    var avatar = $("#AddImgAvartar").attr('src');
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var listArea = [];
    $.each($(".priority-area > tr"), function () {
        listArea.push($(this).find(".district-tr").data('id'))
    });
    var listIdentityImage = [];
    $.each($("._lstImage"), function () {
        listIdentityImage.push($(this).attr('src'));
    });
    console.log("commission", commission);
    if (avatar == "/Uploads/files/add_img.png") {
        avatar = "";
    }
    console.log("avatar", avatar);
    if (name == "") {
        swal({
            title: "error!",
            text: "Please enter full name",
            icon: "warning",
        });
        return;
    }
    if (name.length > 100) {
        swal({
            title: "error!",
            text: "Full name can only 100 characters",
            icon: "warning",
        });
        return;
    }
    if (email == "") {
        swal({
            title: "error!",
            text: "please enter email",
            icon: "warning",
        });
        return;
    }
    if (indentification == "") {
        swal({
            title: "error!",
            text: "please enter identification",
            icon: "warning",
        });
        return;
    }
    if (commission == 0) {
        swal({
            title: "error!",
            text: "please select commission level",
            icon: "warning",
        });
        return;
    }
    if (dob == "") {
        swal({
            title: "error!",
            text: "please enter Birthday",
            icon: "warning",
        });
        return;
    }
    if (listArea.length == 0) {
        swal({
            title: "error!",
            text: "please select priority area",
            icon: "warning",
        });
        return;
    }
    if (!emailReg.test(email)) {
        swal({
            title: "",
            text: "Email is not valid!",
            icon: "warning",
        });
        return;
    }
    if (!/(^[0-9]{9}$)|(^[0-9]{12}$)/.test(indentification)) {
        swal({
            title: "error!",
            text: " Identification invalid",
            icon: "warning",
        });
        return;
    }
    if (!checkdatetime(dob)) {
        swal({
            title: "error!",
            text: " Birthday invalid",
            icon: "warning",
        });
        return;
    }
    $.ajax({
        url: '/Washer/UpdateAgent',
        type: 'POST',
        data: {
            ID: id,
            Name: name,
            Email: email,
            Dob: dob,
            identification: indentification,
            AvatarUrl: avatar,
            IdentityImage: listIdentityImage,
            listArea: listArea,
            CommissionID: commission,
            IsInHouse: inhouse
        },
        success: function (res) {
            if (res == 1) {
                swal({
                    title: "",
                    text: "Update washer successfully",
                    icon: "success",
                });
                //$("#input-edit-name").val("");
                //$("#input-edit-phone").val("");
                //$("#input-edit-email").val("");
                //$("#input-add-password").val("");
                //$("#input-add-confirm").val("");
                //$("#dtBirthdayIndex").val("");
                //$("#input-add-identity").val("");
                //$("#AddAvatar").empty();
                //$("#AddAvatar").append('<img id="AddImgAvartar" src="/Uploads/files/add_img.png" class="contentImg" alt="your image" onclick="ChangeImage($(this));"/>');
                //$(".priority-area").empty();
                //$("#_divAddImg").empty();
            }
            else {
                swal({
                    title: "error!",
                    text: "Update washer failed",
                    icon: "error",
                });
            }
        }
    });
}
function SearchWasher() {
    var codeOrName = $("#txtCode").val().trim();
    var phone = $("#txtPhone").val().trim();
    var email = $("#txtEmail").val().trim();
    var status = $("#SlActive").val();
    var fromDate = $("#dtFromDateIndex").val().trim();
    var toDate = $("#dtTodateIndex").val().trim();
    if (status == 0) {
        status = null;
    }
    $.ajax({
        url: '/Washer/Search',
        data: {
            Page: 1,
            CodeOrName: codeOrName,
            Phone: phone,
            Email: email,
            Status: status,
            FromDate: fromDate,
            ToDate: toDate
        },
        success: function (res) {
            $("#ListWasher").html(res);
        }
    })
}
function ClearFilterWasher() {
    $("#txtCode").val("");
    $("#txtPhone").val("");
    $("#txtEmail").val("");
    $("#SlActive").val(0);
    $("#dtFromDateIndex").val("");
    $("#dtTodateIndex").val("");
    $.ajax({
        url: '/Washer/Search',
        data: {
            Page: 1,
            CodeOrName: "",
            Phone: "",
            Email: "",
            Status: null,
            FromDate: "",
            ToDate: ""
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListWasher").html(res);
        }
    })
}
function GetWasherDetailUpdate(id) {
    window.location.hash = "/update";
    window.location.hash = "/update";
    window.onhashchange = function () { window.location.hash = "/update"; }
    $.ajax({
        url: '/Washer/WasherDetailForUpdate',
        type: 'GET',
        data: { id: id },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#View").html(res);
        }
    })
}
function AddMoneyAgent(type) {
    var money = $("#txtMoney").val().trim();
    var note = $("#noteAddmoney").val().trim();
    if (money == "") {
        swal({
            title: "",
            text: "Please enter money",
            icon: "warning",
        });
        return;
    }
    if (note == "") {
        swal({
            title: "",
            text: "Please enter note",
            icon: "warning",
        });
        return;
    }
    money = cms_decode_currency_format(money);
    var checkType = $("#checkTypeAdd").val().trim();
    if (checkType == 1) {
        var memId = $("#washer").val();
        var lang = $("#washer > option[value=" + memId + "]").attr('class');
        console.log("type money", typeof (money))
        if (memId == 0) {
            swal({
                title: "",
                text: "Please select washer",
                icon: "warning",
            });
            return;
        }
        if (lang == "null") {
            lang = "vi";
        }
        var checkData = {
            memId: memId,
            money: money,
            lang: lang,
            note: note
        }
        console.log("data", checkData);
        $.ajax({
            url: '/Washer/AddMoneyAgent',
            type: 'POST',
            data: {
                memID: memId,
                point: money,
                lang: lang,
                content: note,
                type: type
            },
            success: function (res) {
                swal({
                    title: "",
                    text: "Add money successfully",
                    icon: "success",
                });
                $("#createAddMoney").modal('hide');
            }
        });
    }
    else {
        $.ajax({
            url: '/Washer/AddMultiMoney',
            type: 'POST',
            data: {
                point: money,
                content: note,
                type: type
            },
            success: function (res) {
                if (res.Status == 1) {
                    $("#createAddMoney").modal('hide');
                    RemoveCheckAddMoney();
                    SearchWasher();
                    swal({
                        title: "",
                        text: "Add money successfully",
                        icon: "success",
                    });
                }
                else {
                    swal({
                        title: "",
                        text: res.Message,
                        icon: "success",
                    });
                }
            }
        })
    }

}
//function MinusMoneyAgent() {
//    var money = $("#txtMoney").val().trim();
//    var note = $("#noteAddmoney").val().trim();
//    if (money == "") {
//        swal({
//            title: "",
//            text: "Please enter money",
//            icon: "warning",
//        });
//        return;
//    }
//    if (note == "") {
//        swal({
//            title: "",
//            text: "Please enter note",
//            icon: "warning",
//        });
//        return;
//    }
//    money = cms_decode_currency_format(money);
//    var checkType = $("#checkTypeAdd").val().trim();
//    if (checkType == 1) {
//        var memId = $("#washer").val();
//        var lang = $("#washer > option[value=" + memId + "]").attr('class');
//        console.log("type money", typeof (money))
//        if (memId == 0) {
//            swal({
//                title: "",
//                text: "Please select washer",
//                icon: "warning",
//            });
//            return;
//        }
//        if (lang == "null") {
//            lang = "vi";
//        }
//        var checkData = {
//            memId: memId,
//            money: money,
//            lang: lang,
//            note: note
//        }
//        console.log("data", checkData);
//        $.ajax({
//            url: '/Washer/AddMoneyAgent',
//            type: 'POST',
//            data: {
//                memID: memId,
//                point: money,
//                lang: lang,
//                content: note,
//                type: 2
//            },
//            success: function (res) {
//                if (res == 1) {
//                    swal({
//                        title: "",
//                        text: "Add money successfully",
//                        icon: "success",
//                    });
//                    $("#createAddMoney").modal('hide');
//                }
//                else {
//                    swal({
//                        title: "error!",
//                        text: "Add money failed",
//                        icon: "error",
//                    });
//                }
//            }
//        });
//    }
//    else {
//        $.ajax({
//            url: '/Washer/AddMultiMoney',
//            type: 'POST',
//            data: {
//                point: money,
//                content: note
//            },
//            success: function (res) {
//                if (res.Status == 1) {
//                    $("#createAddMoney").modal('hide');
//                    RemoveCheckAddMoney();
//                    SearchWasher();
//                    swal({
//                        title: "",
//                        text: "Add money successfully",
//                        icon: "success",
//                    });
//                }
//                else {
//                    swal({
//                        title: "",
//                        text: res.Message,
//                        icon: "success",
//                    });
//                }
//            }
//        })
//    }
//}
function RemoveCheckAddMoney() {
    $.ajax({
        url: '/Washer/SaveCheckbox',
        type: 'GET',
        data: { id: null, type: 2 },
        success: function () {
            console.log("success");
        }
    });
}
function DeleteWasher(id) {
    swal({
        title: "Are you sure to delete washer",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Washer/DeleteAgent',
                    data: { id: id },
                    type: "POST",
                    success: function (response) {
                        if (response == 1) {
                            swal({
                                title: "",
                                text: "Delete washer successful!",
                                icon: "success",
                            });
                            SearchWasher();
                        }
                        else {
                            swal({
                                title: "",
                                text: "Delete washer faild!",
                                icon: "error",
                            });
                        }

                    },
                });
            }
        })
}
function ChangeStatusWasher(id, status) {
    swal({
        title: status == 1 ? "Are you sure to deactive washer" : "Are you sure to active washer",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Washer/ChangeStatusAgent',
                    data: { id: id },
                    type: "POST",
                    success: function (response) {
                        if (response == 1) {
                            swal({
                                title: "",
                                text: status == 1 ? "Deactive washer successful!" : "Active washer successful!",
                                icon: "success",
                            });
                            getWasherDetail(id);
                        }
                        else {
                            swal({
                                title: "",
                                text: "Deactive washer faild!",
                                icon: "error",
                            });
                        }

                    },
                });
            }
        })
}
function ChangeStatusCustomer(id, status) {
    swal({
        title: status == 1 ? "Are you sure to active customer?" : "Are you sure to Deactive customer?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Customer/ChangeStatusCustomer',
                    data: { id: id, status: status },
                    type: "POST",
                    success: function (response) {
                        if (response == 1) {
                            swal({
                                title: "",
                                text: status == 2 ? "Deactive customer successful!" : "Active customer successful!",
                                icon: "success",
                            }).then(abc => { window.location = '/Customer/Index' });
                        }
                        else {
                            swal({
                                title: "",
                                text: status == 1 ? "Active washer faild!" : "Deactive washer faild!",
                                icon: "error",
                            });
                        }

                    },
                });
            }
        })
}
function searchInvoice() {
    var codeOrName = $("#txtcodeOrName").val().trim();
    var payment = $("#payment").val();
    var fromDate = $("#dtFromdateIndex").val().trim();
    var toDate = $("#dtTodateIndex").val().trim();
    var package = $("#package").val();
    var agentId = $("#washer").val();
    console.log(fromDate);
    if (payment == 0) {
        payment = null;
    }
    if (fromDate == "") {
        fromDate = null;
    }
    if (toDate == "") {
        toDate = null;
    }
    if (package == 0) {
        package = null;
    }
    if (agentId == 0) {
        agentId = null;
    }
    $.ajax({
        url: '/Invoice/Search',
        data: {
            Page: 1,
            searchKey: codeOrName,
            FromDateStr: fromDate,
            ToDateStr: toDate,
            PaymentType: payment,
            AgentID: agentId,
            ServiceID: package
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListInvoice").html(res);
        }
    })
}
function ClearFilerInvoice() {
    $.ajax({
        url: '/Invoice/Search',
        data: {
            Page: 1,
            searchKey: "",
            FromDateStr: null,
            ToDateStr: null,
            PaymentType: null,
            AgentID: null,
            ServiceID: null
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListInvoice").html(res);
        }
    })
}
function GetInvoiceDetail(id) {
    $.ajax({
        url: '/Invoice/GetInvoiceDetail',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            //var totalPrice = res.Price - 
            $("#invoiceCode").text(res.Code);
            $("#customerName").text(res.CustomerName);
            $("#phoneDetail").text(res.phone);
            $("#addressDetail").text(res.Address);
            $("#dateDetail").text(res.DateStr);
            $("#washerCode").text(res.WasherCode);
            $("#paymentDetail").text(res.PaymentType == 2 ? "VNpay" : "Cash");
            $("#washerCode").text(res.WasherCode);
            $("#execution").text(res.ExecutionTime);
            $("#licencePlate").text(res.CarInfo);
            $("#promocode").text(res.PromotionCode);
            $("#usePoint").text(cms_encode_currency_format(res.UsePoint) + " point");
            $("#listPackage").empty();
            $.each(res.ListService, function () {
                $("#listPackage").append('<tr><td>' + this.ID + '</td><td>' + this.Name + '</td><td>' + cms_encode_currency_format(this.Price) + 'VNĐ</td></tr>');
            })
            $("#priceDetail").text(cms_encode_currency_format(res.Price) + " VNĐ");
            $("#discount").text(cms_encode_currency_format(res.deductionPointStr) + " VNĐ");
            $("#total").text(cms_encode_currency_format(res.TotalPrice) + " VNĐ");
            //cms_decode_currency_format
        }
    })
}
function GetDetailVAT(id) {
    $.ajax({
        url: '/Invoice/GetVAT',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            if (res.Status == 1) {
                var total = 0;
                $("#OFficeName").text(res.Result.OFficeName);
                $("#OfficeVAT").text(res.Result.OfficeVAT);
                $("#CustomerName").text(res.Result.CustomerName);
                $("#PaymentType").text(res.Result.PaymentType);
                $("#OfficeAddress").text(res.Result.OfficeAddress);
                $("#listServiceVAT").empty();
                $.each(res.Result.ListService, function () {
                    $("#listServiceVAT").append('<tr><td>' + this.Name + '</td><td>' + this.QTY + '</td><td>' + this.Unit + '</td><td>' + cms_encode_currency_format(this.price) + '</td><td>' + cms_encode_currency_format(this.TotalPrice) + '</td></tr>');
                    total = total + this.TotalPrice;
                });
                $("#listServiceVAT").append('<tr><td colspan="4">Cộng tiền :</td><td>' + cms_encode_currency_format(total) + '</td></tr>');
                $("#listServiceVAT").append('<tr><td colspan="4">Giảm giá :</td><td>' + cms_encode_currency_format(res.Result.discount) + '</td></tr>');
                $("#listServiceVAT").append('<tr><td colspan="4">Tổng cộng tiền thanh toán :</td><td>' + cms_encode_currency_format(res.Result.TotalPrice) + '</td></tr>');
            }
            else {

            }
        }
    })
}
function SearchTransaction() {
    var name = $("#txtname").val().trim();
    var status = $("#SL_status").val();
    var packageId = $("#package").val();
    var fromDate = $("#dtFromdateIndex").val();
    var toDate = $("#dtTodateIndex").val();
    console.log("package", packageId);
    if (status == 6) {
        status = null;
    }
    if (packageId == 0) {
        packageId = null;
    }
    $.ajax({
        url: '/Transaction/Search',
        type: 'GET',
        data: {
            page: 1,
            search: name,
            fromDate: fromDate,
            toDate: toDate,
            status: status,
            serviceID: packageId
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListTransaction").html(res);
        }
    })
}
function ClearFilerTransaction() {
    $.ajax({
        url: '/Transaction/Search',
        type: 'GET',
        data: {
            page: 1,
            search: "",
            fromDate: "",
            toDate: "",
            status: null,
            serviceID: null
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#txtname").val("");
            $("#SL_status").val(6);
            $("#package").val(0);
            $("#dtFromdateIndex").val("");
            $("#dtTodateIndex").val("");
            $("#ListTransaction").html(res);
        }
    })
}
function searchVehicle() {
    var cusName = $("#txtCusName").val().trim();
    var BrandName = $("#txtCarBrand").val().trim();
    var modelName = $("#txtCarModel").val().trim();
    var isVerify = $("#SlActive").val();
    var fromDateSTR = $("#dtFromdateIndex").val().trim();
    var toDateSTR = $("#dtTodateIndex").val().trim();
    if (isVerify == 2) {
        isVerify = null;
    }
    if (fromDateSTR == "") {
        fromDateSTR = null;
    }
    if (toDateSTR == "") {
        toDateSTR = null;
    }
    $.ajax({
        url: '/Vehicle/Search',
        data: {
            Page: 1,
            CusName: cusName,
            BrandName: BrandName,
            ModelName: modelName,
            isVerify: isVerify,
            FromDate: fromDateSTR,
            ToDate: toDateSTR
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListVehicle").html(res);
        }
    })
}
function clearFilterVehicle() {
    $("#txtCusName").val("");
    $("#txtCarBrand").val("");
    $("#txtCarModel").val("");
    $("#SlActive").val(2);
    $("#dtFromDateIndex").val("");
    $("#dtTodateIndex").val("");
    $.ajax({
        url: '/Vehicle/Search',
        data: {
            Page: 1,
            cusName: "",
            BrandName: "",
            ModelName: "",
            isVerify: null,
            FromDate: "",
            ToDate: ""
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListVehicle").html(res);
        }
    })
}
function getVehicleDetail(id) {
    $.ajax({
        url: '/Vehicle/GetVehicleDetail',
        type: 'GET',
        data: { id: id },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#customerName").text(res.CustomerName);
            $("#registerDate").text(res.RegistrationDateStr);
            $("#brandName").text(res.BrandName);
            $("#carType").text(res.ModelName);
            $("#licensePlate").text(res.LicencePalte);
            $("#carColor").text(res.CarColor);
            $("#washerCode").text(res.WasherCode);
            $("#productDate").text(res.Year);
            $("#carGrade").text(res.SegmetName);
            $("#carStatus").text(res.isVeryfile == 1 ? "Verified" : "Not verified");
            $("#customerAvatar").empty();
            $("#carImage").empty();
            if (res.listImage.length > 0) {
                $("#customerAvatar").append('<img src="' + res.listImage[0] + '" width="100" height="100" onclick="LargeImage(event)"/>');
                $.each(res.listImage, function () {
                    $("#carImage").append('<img src="' + this + '" width="70" height="70" class="inlineblock mr-2" onclick="LargeImage(event)"/>')
                })
            }
            else {
                $("#carImage").append('<span>No image</span>');
            }
            //$("#customerAvatar").append('<img src="' + res.listImage[0] + '" width="100" height="100" onclick="LargeImage(event)"/>');
            //$.each(res.listImage, function () {
            //    $("#carImage").append('<img src="' + this + '" width="70" height="70" class="inlineblock mr-2" onclick="LargeImage(event)"/>')
            //})
            //cms_decode_currency_format
        }
    })
}
function DeleteCarCustomer(id) {
    swal({
        title: "Are you sure to delete this car?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Vehicle/DeleteVehicle',
                    data: { id: id },
                    type: "POST",
                    success: function (response) {
                        if (response.Status == 1) {
                            searchVehicle();
                            swal({
                                title: "",
                                text: "Delete successfully!",
                                icon: "success",
                            });
                        }
                        else {
                            swal({
                                title: "",
                                text: res.Message,
                                icon: "error",
                            });
                        }

                    },
                    error: function () {
                        swal({
                            title: "",
                            text: "The system is maintenance",
                            icon: "error",
                        });
                    }
                });
            }
        })
}

function searchCombo() {
    var Searchkey = $("#txtName").val().trim();
    var FromDate = $("#dtFromdateIndex").val().trim();
    var ToDate = $("#dtTodateIndex").val().trim();
    if (Searchkey == "") {
        Searchkey = null;
    }
    if (FromDate == "") {
        FromDate = null;
    }
    if (ToDate == "") {
        ToDate = null;
    }
    $.ajax({
        url: '/Combo/SearchCombo',
        data: {
            Page: 1,
            Searchkey: Searchkey,
            FromDate: FromDate,
            ToDate: ToDate,
            //isVerify: isVerify,
            //FromDate: fromDateSTR,
            //ToDate: toDateSTR
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListCombo").html(res);
        }
    })
}
function clearFilterCombo() {
    $("#txtName").val("");
    $("#dtFromdateIndex").val("");
    $("#dtTodateIndex").val("");
    $.ajax({
        url: '/Combo/SearchCombo',
        data: {
            Page: 1,
            Searchkey: "",
            FromDate: "",
            ToDate: ""
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListCombo").html(res);
        }
    })
}
function AddTransaction() {
    var cusId = $("#txtCusId").val();
    var cusName = $("#txtname").val().trim();
    var carId = $("#carCus").val();
    var date = $("#dtDateBooking").val().trim();
    var hour = $("#hour-Booking").val();
    var dateBooking = hour + ' ' + date;
    var placeId = $("#placeId").val().trim();
    var note = $("#note").val().trim();
    var isIndoor = $("#typeWash").val();
    var mainService = $("#package").val();
    var adress = $('#txtaddress').val();
    var ListAdditionService = [];
    $.each($(".addtional-id"), function () {
        ListAdditionService.push($(this).text());
    });
    if (cusName == "") {
        swal({
            title: "",
            text: "Please enter customer name!",
            icon: "warning",
        });
        return;
    }
    if (cusId == "") {
        swal({
            title: "",
            text: "Cannot find customerID!",
            icon: "warning",
        });
        return;
    }
    if (carId == 0) {
        swal({
            title: "",
            text: "Please select car customer!",
            icon: "warning",
        });
        return;
    }
    if (dateBooking == "") {
        swal({
            title: "",
            text: "Please select date booking!",
            icon: "warning",
        });
        return;
    }
    if (placeId == "") {
        swal({
            title: "",
            text: "Please select customer address!",
            icon: "warning",
        });
        return;
    }
    if (isIndoor == "") {
        swal({
            title: "",
            text: "Please select type!",
            icon: "warning",
        });
        return;
    }
    if (mainService == 0) {
        swal({
            title: "",
            text: "Please select main service!",
            icon: "warning",
        });
        return;
    }
    $.ajax({
        url: '/Transaction/CreateTransaction',
        type: 'POST',
        data: {
            customerID: cusId,
            CarID: carId,
            BookingDateInput: dateBooking,
            placeID: placeId,
            isInDoor: isIndoor,
            mainService: mainService,
            additionService: ListAdditionService,
            note: note,
            cusAddress: adress
        },
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "",
                    text: "Add transaction successful!",
                    icon: "success",
                });
                setTimeout(function () {
                    window.location.href = "/Transaction/Index";
                }, 500);
            }
            else {
                swal({
                    title: "",
                    text: res.Exception,
                    icon: "warning",
                });
            }
        }

    })
}
function CancelTransaction(id) {
    var content = $.trim($('#txtContentCancelTransaction').val());
    if (content.length == 0) {
        swal({
            title: "Please enter content transaction!",
            text: "",
            icon: "warning"
        })
        return;
    }
    swal({
        title: "Are you sure to cancel this transaction?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Transaction/CancelTransaction',
                    data: { id: id, Content: content },
                    type: "POST",
                    success: function (response) {
                        if (response == "OK") {
                            swal({
                                title: "",
                                text: "Cancel transaction successful!",
                                icon: "success",
                            }).then((success) => {
                                if (success) {
                                    location.reload();
                                }
                            })
                        }
                        else {
                            swal({
                                title: response,
                                text: "",
                                icon: "error",
                            });
                        }

                    },
                });
            }
        })
}
function DeleteTransaction(id, role) {
    if (role != 1) {
        swal({
            title: "Permission denied",
            text: "",
            icon: "warning"
        });
        return;
    }
    swal({
        title: "Are you sure to delete this transaction?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Transaction/DeleteTransaction',
                    data: { id: id },
                    type: "POST",
                    success: function (response) {
                        if (response == 1) {
                            swal({
                                title: "",
                                text: "Delete transaction successful!",
                                icon: "success",
                            });
                            SearchTransaction();
                        }
                        else {
                            swal({
                                title: "",
                                text: "Delete transaction faild!",
                                icon: "error",
                            });
                        }

                    },
                });
            }
        })
}

//Search request infomation 
function searchRequestCR() {
    var keySearch = $.trim($("#txtEmail").val());
    var status = $('#txtStatus').val();
    var fd = $.trim($('#dtFromdateIndex').val());
    var td = $.trim($('#dtTodateIndex').val());

    $.ajax({
        url: "/Request/Search",
        data: { status: status, searchKey: keySearch, fromDate: fd, toDate: td },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (result) {
            $("#modalLoad").modal("hide");
            $('#tableRequest').html(result);
        }
    })
}

//Change status request
function changeStatusRequest(status) {
    var lstID = [];
    var contentReject = $.trim($('#txtContentReject').val());
    $('#tblRequestDT').empty();
    $.each($(".checkbox"), function () {
        if ($(this).prop("checked") == true) {
            lstID.push($(this).val());
            var wsName = $(this).closest("tr").find(".Wsname").text();
            var wsPhone = $(this).closest("tr").find(".Wsphone").text();
            var amount = $(this).closest("tr").find(".amount").text();
            var bank = $(this).closest("tr").find(".bank").text();
            var date = $(this).closest("tr").find(".date").text();
            var acount = $(this).closest("tr").find(".acount").text();
            var owner = $(this).closest("tr").find(".owner").text();
            var id = $(this).val();
            $('#tblRequestDT').append('<tr class="idWS" data-id="' + id + '"><td class="nameWS">' + wsName + '</td><td class="phoneWS">' + wsPhone + '</td><td class="amountWS">' + amount + '</td><td class="bankWS">' + bank + '</td><td class="acountWS">' + acount + '</td><td class="ownerWS">' + owner + '</td><td class="dateWS">' + date + '</td><td><a onclick="RemoveTr(event)"><i class="fa fa-trash-o text-danger"></a></i></td></tr>');
        }
    });

    if (status == 3) {
        lstID = [];
        $.each($("#tblRequestDT > tr"), function () {
            lstID.push($(this).data('id'));
        })

        console.log(lstID);
    }

    if (lstID.length == 0) {
        swal({
            title: "Please select data!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (status == null) {
        $('#mdRequestDT').modal('show');
        return;
    }
    if (status == 10) {
        $('#mdDetailReject').modal('show');
        return;
    }

    swal({
        title: "Are you sure want to change this status ?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            if (status == 0) {
                if (contentReject.length == 0) {
                    swal({
                        title: "Warning!",
                        text: "Please enter reason for reject",
                        icon: "warning"
                    })
                    return
                }
            }
            $.ajax({
                url: "/Request/ChangeSatusRequest",
                data: { Status: status, lstID: lstID, contentReject: contentReject },
                type: "POST",
                dataType: "json",
                success: function (response) {
                    if (response.Status == 1) {
                        swal({
                            title: "Update data successfully",
                            text: "",
                            icon: "success"
                        }).then((success) => {
                            if (success) {
                                location.reload();
                            }
                        })
                        $('#mdDetailReject').modal('hide');
                    } else {
                        swal({
                            title: response.Message,
                            text: "",
                            icon: "error"
                        })
                    }
                },
                error: function (res) {
                    console.log(res);
                }
            });
        }
    });


}
function searchPackageService() {
    var code = $("#txtcode").val().trim();
    var name = $("#txtname").val().trim();
    var fromDate = $("#dtFromdateIndex").val().trim();
    var toDate = $("#dtTodateIndex").val().trim();
    $.ajax({
        url: '/PackageService/Search',
        type: 'GET',
        data: {
            page: 1,
            code: code,
            name: name,
            fromDate: fromDate,
            toDate: toDate
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListPackageService").html(res);
        }
    })
}
function ClearFilerPackageService() {
    $("#txtcode").val("");
    $("#txtname").val("");
    $("#dtFromdateIndex").val("");
    $("#dtTodateIndex").val("");
    $.ajax({
        url: '/PackageService/Search',
        type: 'GET',
        data: {
            page: 1,
            code: "",
            name: "",
            fromDate: "",
            toDate: ""
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListPackageService").html(res);
        }
    })
}
function DeletePackage(id) {
    swal({
        title: "Are you sure want to delete this package?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: "/PackageService/DeleteService",
                data: { id: id },
                type: "POST",
                dataType: "json",
                success: function (response) {
                    if (response.Status == 1) {
                        searchPackageService();
                        swal({
                            title: "",
                            text: "Delete service successfully",
                            icon: "success"
                        });
                    }
                    else {
                        swal({
                            title: "Falied",
                            text: res.Message,
                            title: "",
                            text: res.Exception,
                            icon: "error"
                        });
                    }
                }

            });
        }
    });
}
function CreatePackageService() {
    var packCode = $("#code").val().trim();
    var packName = $("#name").val().trim();
    var packNameVN = $("#nameVN").val().trim();
    var time = $("#time").val().trim();
    var description = $("#description").val().trim();
    var serviceType = $("#serviceType").val();
    var display = $("#display").val().trim();
    var status = $("#status").val();
    var checkDisplayJob = false;
    var listServiceId = [];
    var discount = $("#discount-pack").val().trim();
    if (serviceType == 1) {
        listServiceId = [];
        $.each($(".serviceId"), function () {
            listServiceId.push({ AddtionServiceID: $(this).data('id'), });
        });
    }
    if (serviceType == 2) {
        listServiceId = [];
        $.each($(".serviceId"), function () {
            listServiceId.push({ MainServiceID: $(this).data('id'), });
        });
    }

    var listSegmentPrice = [];
    $.each($(".seg-body > tr"), function () {
        price = cms_decode_currency_format($(this).find(".segPrice").val());
        listSegmentPrice.push({ SegmentID: $(this).find(".segName").data('id'), Price: price });
    });
    var mainImage = $("#AddImgMain").attr('src');
    var thumbnail = $("#AddImgThumbnail").attr('src');
    var icon = $("#AddImgIcon").attr('src');
    var listImage = [];
    var listOrtherImage = [];
    listImage.push({ url: mainImage, Type: 1 });
    listImage.push({ url: thumbnail, Type: 3 });
    $.each($("._lstImage "), function () {
        listImage.push({ url: $(this).attr('src'), Type: 2 });
        listOrtherImage.push({ url: $(this).attr('src'), Type: 2 });
    });
    var listJob = [];
    $.each($(".job-body > tr"), function () {
        listJob.push({ Content: $(this).find(".job-name").text(), DisplayOrder: $(this).find(".displayJob").val() });
    });
    $.each($(".job-body > tr"), function () {
        if ($(this).find(".displayJob").val() == "") {
            checkDisplayJob = true;
        }
    });
    console.log(checkDisplayJob);
    if (packCode == "") {
        swal({
            title: "",
            text: "Please enter package service code!",
            icon: "warning",
        });
        return;
    }
    if (packName == "") {
        swal({
            title: "",
            text: "Please enter package service name english !",
            icon: "warning",
        });
        return;
    }
    if (packNameVN == "") {
        swal({
            title: "",
            text: "Please enter package service name vietnam !",
            icon: "warning",
        });
        return;
    }
    if (time == "") {
        swal({
            title: "",
            text: "Please enter time!",
            icon: "warning",
        });
        return;
    }
    if (description == "") {
        swal({
            title: "",
            text: "Please enter description!",
            icon: "warning",
        });
        return;
    }
    if (display == "") {
        swal({
            title: "",
            text: "Please enter display order!",
            icon: "warning",
        });
        return;
    }
    if (serviceType == 0) {
        swal({
            title: "",
            text: "Please choose service type!",
            icon: "warning",
        });
        return;
    }
    if (discount == "") {
        swal({
            title: "",
            text: "Please enter discount!",
            icon: "warning",
        });
        return;
    }
    if (discount == "") {
        swal({
            title: "",
            text: "Please enter discount!",
            icon: "warning",
        });
        return;
    }
    if (discount == "") {
        swal({
            title: "",
            text: "Please enter discount!",
            icon: "warning",
        });
        return;
    }
    if (listSegmentPrice.length == 0) {
        swal({
            title: "",
            text: "Please add segment!",
            icon: "warning",
        });
        return;
    }
    if (listJob.length == 0) {
        swal({
            title: "",
            text: "Please add job!",
            icon: "warning",
        });
        return;
    }
    if (checkDisplayJob == true) {
        swal({
            title: "",
            text: "Please enter display job!",
            icon: "warning",
        });
        return;
    }
    //check value price
    var checkValuePrice = false;
    $.each($(".segPrice "), function () {
        if ($(this).val() == "") {
            checkValuePrice = true;
        }
    });
    if (checkValuePrice == true) {
        swal({
            title: "",
            text: "Please enter price for segment!",
            icon: "warning",
        });
        return;
    }
    if (discount > 100) {
        swal({
            title: "",
            text: "Promote can not be greater than 100 !",
            icon: "warning",
        });
        return;
    }
    if (serviceType == 1) {
        if (mainImage == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose main image!",
                icon: "warning",
            });
            return;
        }
        if (thumbnail == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose thumbnail!",
                icon: "warning",
            });
            return;
        }
        if (icon == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose icon!",
                icon: "warning",
            });
            return;
        }
        if (icon == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose icon!",
                icon: "warning",
            });
            return;
        }
        if (listOrtherImage.length == 0) {
            swal({
                title: "",
                text: "Please choose orther image!",
                icon: "warning",
            });
            return;
        }
        //var checkData = {
        //    Code: packCode,
        //    IsActive: status,
        //    NameEN: packName,
        //    NameVN: packNameVN,
        //    Description: description,
        //    Icon: icon,
        //    DisplayOrder: display,
        //    Type: serviceType,
        //    Discount: discount,
        //    EstTime: time,
        //    ListServicePrice: listSegmentPrice,
        //    ListMainServiceAdditionService: listServiceId,
        //    ListImage: listImage,
        //    Listjob: listJob
        //}
        //console.log(checkData);
        $.ajax({
            url: '/PackageService/SubmitAddPackage',
            type: 'POST',
            cache: false,
            data: {
                Code: packCode,
                IsActive: status,
                NameEN: packName,
                NameVN: packNameVN,
                Description: description,
                Icon: icon,
                DisplayOrder: display,
                Type: serviceType,
                Discount: discount,
                EstTime: time,
                ListServicePrice: listSegmentPrice,
                ListMainServiceAdditionService: listServiceId,
                ListImage: listImage,
                Listjob: listJob
            },
            success: function (res) {
                if (res == 1) {
                    swal({
                        title: "",
                        text: "Add package service successful!",
                        icon: "success",
                    });
                }
                else {
                    swal({
                        title: "",
                        text: "Add package service failed!",
                        icon: "error",
                    });
                }
            }
        });
    }
    else {
        //var checkData2 = {
        //    Code: packCode,
        //    IsActive: status,
        //    NameEN: packName,
        //    NameVN: packNameVN,
        //    Description: description,
        //    Icon: "",
        //    DisplayOrder: display,
        //    Type: serviceType,
        //    Discount: discount,
        //    EstTime: time,
        //    ListServicePrice: listSegmentPrice,
        //    ListMainServiceAdditionService: listServiceId,
        //    ListImage: [],
        //    Listjob: listJob
        //}
        //console.log(checkData2);
        $.ajax({
            url: '/PackageService/SubmitAddPackage',
            type: 'POST',
            cache: false,
            data: {
                Code: packCode,
                IsActive: status,
                NameEN: packName,
                NameVN: packNameVN,
                Description: description,
                Icon: "",
                DisplayOrder: display,
                Type: serviceType,
                Discount: discount,
                EstTime: time,
                ListServicePrice: listSegmentPrice,
                ListMainServiceAdditionService: listServiceId,
                ListImage: [],
                Listjob: listJob
            },
            success: function (res) {
                if (res == 1) {
                    swal({
                        title: "",
                        text: "Add package service successful!",
                        icon: "success",
                    });
                }
                else {
                    swal({
                        title: "",
                        text: "Add package service failed!",
                        icon: "error",
                    });
                }
            }
        });
    }
}
function UpdatePackageService(id) {
    var packCode = $("#code").val().trim();
    var packName = $("#name").val().trim();
    var packNameVN = $("#nameVN").val().trim();
    var time = $("#time").val().trim();
    var description = $("#description").val().trim();
    var serviceType = $("#serviceType").val();
    var display = $("#display").val().trim();
    var status = $("#status").val();
    var checkDisplayJob = false;
    var listServiceId = [];
    var discount = $("#discount-pack").val().trim();
    if (serviceType == 1) {
        listServiceId = [];
        $.each($(".serviceId"), function () {
            listServiceId.push({ AddtionServiceID: $(this).data('id'), });
        });
    }
    if (serviceType == 2) {
        listServiceId = [];
        $.each($(".serviceId"), function () {
            listServiceId.push({ MainServiceID: $(this).data('id'), });
        });
    }

    var listSegmentPrice = [];
    $.each($(".seg-body > tr"), function () {
        price = cms_decode_currency_format($(this).find(".segPrice").val());
        listSegmentPrice.push({ SegmentID: $(this).find(".segName").data('id'), Price: price });
    });
    var mainImage = $("#AddImgMain").attr('src');
    var thumbnail = $("#AddImgThumbnail").attr('src');
    var icon = $("#AddImgIcon").attr('src');
    var listImage = [];
    var listOrtherImage = [];
    listImage.push({ url: mainImage, Type: 1 });
    listImage.push({ url: thumbnail, Type: 3 });
    $.each($("._lstImage "), function () {
        listImage.push({ url: $(this).attr('src'), Type: 2 });
        listOrtherImage.push({ url: $(this).attr('src'), Type: 2 });
    });
    var listJob = [];
    $.each($(".job-body > tr"), function () {
        listJob.push({ Content: $(this).find(".job-name").text(), DisplayOrder: $(this).find(".displayJob").val(), ID: $(this).find(".job-name").data('id') });
    });
    $.each($(".job-body > tr"), function () {
        if ($(this).find(".displayJob").val() == "") {
            checkDisplayJob = true;
        }
    });
    console.log(checkDisplayJob);
    if (packCode == "") {
        swal({
            title: "",
            text: "Please enter package service code!",
            icon: "warning",
        });
        return;
    }
    if (packName == "") {
        swal({
            title: "",
            text: "Please enter package service name english !",
            icon: "warning",
        });
        return;
    }
    if (packNameVN == "") {
        swal({
            title: "",
            text: "Please enter package service name vietnam !",
            icon: "warning",
        });
        return;
    }
    if (time == "") {
        swal({
            title: "",
            text: "Please enter time!",
            icon: "warning",
        });
        return;
    }
    if (display == "") {
        swal({
            title: "",
            text: "Please enter display order!",
            icon: "warning",
        });
        return;
    }
    if (serviceType == 0) {
        swal({
            title: "",
            text: "Please choose service type!",
            icon: "warning",
        });
        return;
    }
    if (discount == "") {
        swal({
            title: "",
            text: "Please enter discount!",
            icon: "warning",
        });
        return;
    }
    if (discount == "") {
        swal({
            title: "",
            text: "Please enter discount!",
            icon: "warning",
        });
        return;
    }
    if (discount == "") {
        swal({
            title: "",
            text: "Please enter discount!",
            icon: "warning",
        });
        return;
    }
    if (listSegmentPrice.length == 0 || $(".seg-body > tr >td").length == 1) {
        swal({
            title: "",
            text: "Please add segment!",
            icon: "warning",
        });
        return;
    }
    if (listJob.length == 0) {
        swal({
            title: "",
            text: "Please add job!",
            icon: "warning",
        });
        return;
    }
    if (checkDisplayJob == true) {
        swal({
            title: "",
            text: "Please enter display job!",
            icon: "warning",
        });
        return;
    }
    //check value price
    var checkValuePrice = false;
    $.each($(".segPrice "), function () {
        if ($(this).val() == "") {
            checkValuePrice = true;
        }
    });
    if (checkValuePrice == true) {
        swal({
            title: "",
            text: "Please enter price for segment!",
            icon: "warning",
        });
        return;
    }
    if (discount > 100) {
        swal({
            title: "",
            text: "Promote can not be greater than 100 !",
            icon: "warning",
        });
        return;
    }
    if (serviceType == 1) {
        if (mainImage == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose main image!",
                icon: "warning",
            });
            return;
        }
        if (thumbnail == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose thumbnail!",
                icon: "warning",
            });
            return;
        }
        if (icon == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose icon!",
                icon: "warning",
            });
            return;
        }
        if (icon == "/Uploads/files/add_img.png") {
            swal({
                title: "",
                text: "Please choose icon!",
                icon: "warning",
            });
            return;
        }
        if (listOrtherImage.length == 0) {
            swal({
                title: "",
                text: "Please choose orther image!",
                icon: "warning",
            });
            return;
        }
        //var checkData = {
        //    ID: id,
        //    Code: packCode,
        //    IsActive: status,
        //    NameEN: packName,
        //    NameVN: packNameVN,
        //    Description: description,
        //    Icon: icon,
        //    DisplayOrder: display,
        //    Type: serviceType,
        //    Discount: discount,
        //    EstTime: time,
        //    ListServicePrice: listSegmentPrice,
        //    ListMainServiceAdditionService: listServiceId,
        //    ListImage: listImage,
        //    Listjob: listJob
        //}
        //console.log(checkData);
        $.ajax({
            url: '/PackageService/UpdateService',
            type: 'POST',
            cache: false,
            data: {
                ID: id,
                Code: packCode,
                IsActive: status,
                NameEN: packName,
                NameVN: packNameVN,
                Description: description,
                Icon: icon,
                DisplayOrder: display,
                Type: serviceType,
                Discount: discount,
                EstTime: time,
                ListServicePrice: listSegmentPrice,
                ListMainServiceAdditionService: listServiceId,
                ListImage: listImage,
                Listjob: listJob
            },
            success: function (res) {
                if (res == 1) {
                    swal({
                        title: "",
                        text: "Update package service successful!",
                        icon: "success",
                    });
                }
                else {
                    swal({
                        title: "",
                        text: "Update package service failed!",
                        icon: "error",
                    });
                }
            }
        });
    }
    else {
        //var checkData = {
        //    ID: id,
        //    Code: packCode,
        //    IsActive: status,
        //    NameEN: packName,
        //    NameVN: packNameVN,
        //    Description: description,
        //    Icon: "",
        //    DisplayOrder: display,
        //    Type: serviceType,
        //    Discount: discount,
        //    EstTime: time,
        //    ListServicePrice: listSegmentPrice,
        //    ListMainServiceAdditionService: listServiceId,
        //    ListImage: [],
        //    Listjob: listJob
        //}
        //console.log(checkData);
        $.ajax({
            url: '/PackageService/UpdateService',
            type: 'POST',
            cache: false,
            data: {
                ID: id,
                Code: packCode,
                IsActive: status,
                NameEN: packName,
                NameVN: packNameVN,
                Description: description,
                Icon: "",
                DisplayOrder: display,
                Type: serviceType,
                Discount: discount,
                EstTime: time,
                ListServicePrice: listSegmentPrice,
                ListMainServiceAdditionService: listServiceId,
                ListImage: [],
                Listjob: listJob
            },
            success: function (res) {
                if (res == 1) {
                    swal({
                        title: "",
                        text: "Update package service successful!",
                        icon: "success",
                    });
                }
                else {
                    swal({
                        title: "",
                        text: "Update package service failed!",
                        icon: "error",
                    });
                }
            }
        });
    }
}
function CreateCombo() {
    var name = $("#name").val().trim();
    var nameVn = $("#nameVn").val().trim();
    var description = $("#description").val().trim();
    var promote = $("#promote").val().trim();
    //var display = $("#display").val().trim();
    var status = $("#typePromote").val();
    var listSegPrice = [];
    $.each($(".tbody-addprice > tr"), function () {
        var price = cms_decode_currency_format($(this).find(".priceValue").val());
        listSegPrice.push({ SegmentID: $(this).find(".segName").data('id'), BasePrice: price });
    });

    var mainImage = $("#AddImgMain").attr('src');
    var thumbnail = $("#AddImgThumbnail").attr('src');
    var Icon = $("#AddImgIcon").attr('src');
    var listImage = [];
    var listOrtherImage = [];
    listImage.push({ url: mainImage, Type: 1 });
    listImage.push({ url: thumbnail, Type: 3 });
    $.each($("._lstImage "), function () {
        listImage.push({ url: $(this).attr('src'), Type: 2 });
        listOrtherImage.push({ url: $(this).attr('src'), Type: 2 });
    });
    var listMainService = [];
    $.each($(".tbody-addpackageservice > tr"), function () {
        listMainService.push({ ServiceID: $(this).find(".psName").data('id'), Count: $(this).find(".amountValue").val(), DisplayOrder: $(this).find(".displayOrder").val() })
    });
    console.log(listMainService)
    var additionService = [];
    var id1 = $(".tbody-addaddservice").children("tr");
    $.each(id1, function (key, val) {
        var idnew = $(this).children("td").attr("data-id");
        var dt = { ServiceID: idnew };
        additionService.push(dt);
    })
    console.log(additionService);
    if (name == "") {
        swal({
            title: "",
            text: "Please enter name english!",
            icon: "warning",
        });
        return;
    }
    if (nameVn == "") {
        swal({
            title: "",
            text: "Please enter name Vietnam!",
            icon: "warning",
        });
        return;
    }

    if (promote == "") {
        swal({
            title: "",
            text: "Please enter promote!",
            icon: "warning",
        });
        return;
    }
    if (listSegPrice.length == 0) {
        swal({
            title: "",
            text: "Please add price!",
            icon: "warning",
        });
        return;
    }
    if (mainImage == "/Uploads/files/add_img.png") {
        swal({
            title: "",
            text: "Please choose main image!",
            icon: "warning",
        });
        return;
    }
    if (thumbnail == "/Uploads/files/add_img.png") {
        swal({
            title: "",
            text: "Please choose thumbnail!",
            icon: "warning",
        });
        return;
    }
    if (Icon == "/Uploads/files/add_img.png") {
        swal({
            title: "",
            text: "Please select Icon",
            icon: "warning",
        });
        return;
    }
    if (listOrtherImage.length == 0) {
        swal({
            title: "",
            text: "Please choose orther image!",
            icon: "warning",
        });
        return;
    }
    if (listMainService.length == 0) {
        swal({
            title: "",
            text: "Please choose package service!",
            icon: "warning",
        });
        return;
    }
    var checkEmptyDisplay = false;
    $.each($(".displayOrder"), function () {
        if ($(this).val() == "") {
            checkEmptyDisplay = true;
        }
    });
    if (checkEmptyDisplay == true) {
        swal({
            title: "",
            text: "Please enter display order for package service!",
            icon: "warning",
        });
        return;
    }
    if (additionService.length <= 0) {
        swal({
            title: "",
            text: "Please choose addition service!",
            icon: "warning",
        });
        return;
    }
    //check chua nhap gia
    var checkPriceValue = false;
    $.each($(".priceValue"), function () {
        if ($(this).val() == "") {
            checkPriceValue = true;
        }
    })
    if (checkPriceValue == true) {
        swal({
            title: "",
            text: "Please enter price for segment!",
            icon: "warning",
        });
        return;
    }
    var checkData = {
        NameEN: name,
        NameVN: nameVn,
        IsActive: status,
        Description: description,
        Discount: promote,
        Icon: Icon,
        ListIamge: listImage,
        ListPackageService: listMainService,
        AdditionService: additionService,
        ListComboServicePrice: listSegPrice
    };
    console.log(checkData);
    $.ajax({
        url: '/Combo/AddCombo',
        type: 'POST',
        data: checkData,
        success: function (res) {
            if (res == 1) {
                swal({
                    title: "",
                    text: "Create combo successful!",
                    icon: "success",
                });
                window.location.href = "/Combo/Index";
            }
            else {
                swal({
                    title: "Failed",
                    text: "Create Combo Failed!",
                    icon: "warning",
                });
            }
        }
    })
}

//search Notifycation
function searchNotify() {
    var fromDate = $('#dtFromdateIndex').val().trim();
    var toDate = $('#dtTodateIndex').val().trim();
    var type = $('#txtAccountType').val();
    var isActive = $('#txtStatus').val();
    var search = $('#txtName').val();
    $.ajax({

        url: '/Notification/Search',
        type: 'POST',
        data: {
            page: 1,
            search: search,
            type: type,
            isActive: isActive,
            fromDate: fromDate,
            toDate: toDate
        }, beforeSend: function () {
            $("#modalLoad").modal("show");
        }, success: function (res) {
            $('#ListNotification').html(res);
            $("#modalLoad").modal("hide");
        }
    })

}

//clearFilter

function clearFilter() {
    $('#dtFromdateIndex').val("");
    $('#dtTodateIndex').val("");
    $('#txtAccountType').val("All Notify");
    $('#txtStatus').val("Status");
    $('#txtsearch').val("");
    searchNotify();
}

function clearFilterWallet() {
    $("#txtName").val("");
    $("#dtFromDate").val("");
    $("#dtToDate").val("");
    $.ajax({
        url: '/Wallet/Search',
        data: {
            Page: 1,
            Searchkey: "",
            FromDate: "",
            ToDate: ""
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $("#ListWallet").html(res);
        }
    })
}

//function WalletDetail(id) {
//    console.log("id", id);
//    $.ajax({
//        url: '/Wallet/WalletDetail',
//        type: 'GET',
//        data: { id: id },
//        success: function (res) {
//            $("#View").html(res);
//        }
//    })
//}

//ClearFilterCustomer
function clearFilterCustomer() {
    $('#dtFromdate').val("");
    $('#dtTodateIndex').val("");
    $('#txtPhone').val("");
    $('#txtName').val("");
    $('#txtEmail').val("");
    $('#txtActive').val(1);
    //var province = $('#slProvince').val();
    //var district = $('#slDistrict').val();
    //var role = $('#cmbRoleCus').val();
    //var status = $('#cbbStatusCustomer').val();

    $.ajax({
        url: "/Customer/Search",
        data: {
            page: 1,
            fromDate: "",
            toDate: "",
            //City: province,
            //District: district,
            phone: "",
            active: "",
            email: "",
            codeOrName: ""
        },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (result) {
            $("#modalLoad").modal("hide");
            $('#ListCustomer').html(result);
        }
    });
}

function AcceptAndExportLstRequest(status, type) {
    var data = [];
    var lstID = [];

    $.each($('#tblRequestDT > tr'), function () {
        data.push({ Name: $(this).find(".nameWS").text(), Phone: $(this).find(".phoneWS").text(), Amout: $(this).find(".amountWS").text(), Bank: $(this).find(".bankWS").text(), Date: $(this).find(".dateWS").text(), Acount: $(this).find(".acountWS").text(), Owner: $(this).find(".ownerWS").text() });
    });

    $.each($("#tblRequestDT > tr"), function () {
        lstID.push($(this).attr('data-id'));
    });

    if (lstID.length == 0) {
        swal({
            title: "Please select data!",
            text: "",
            icon: "warning"
        })
        return;
    }

    swal({
        title: "Are you sure want change this status ?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: "/Request/ChangeSatusRequest",
                data: { Status: status, lstID: lstID },
                type: "POST",
                dataType: "json",
                success: function (response) {
                    if (response.Status == 1) {
                        swal({
                            title: "Change status successfully",
                            text: "",
                            icon: "success"
                        });
                        if (type == null) {
                            window.location = "/Request/ExportListRequest?Data=" + JSON.stringify(data);
                        }
                        searchRequestCR();
                        $('#tblRequestDT').empty();
                        $('#mdRequestDT').modal('hide');
                    } else {
                        swal({
                            title: response.Message,
                            text: "",
                            icon: "error"
                        })
                    }
                }

            });
        }
    });
}
function getComboDetail(id) {
    $.ajax({
        url: '/Combo/UpdateComboView',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            window.location.href = "/Combo/UpdateComboView?id=" + id;
        }
    })
}
function UpdateCombo(id) {
    var name = $("#name").val().trim();
    var nameVn = $("#nameVn").val().trim();
    var description = $("#description").val().trim();
    var promote = $("#promote").val().trim();
    //var display = $("#display").val().trim();
    var status = $("#typePromote").val();
    var listSegPrice = [];
    $.each($(".tbody-addprice > tr"), function () {
        var price = cms_decode_currency_format($(this).find(".priceValue").val());
        listSegPrice.push({ SegmentID: $(this).find(".segName").data('id'), BasePrice: price });
    });

    var mainImage = $("#AddImgMain").attr('src');
    var thumbnail = $("#AddImgThumbnail").attr('src');
    var icon = $("#AddImgIcon").attr('src');
    var listImage = [];
    var listOrtherImage = [];
    listImage.push({ url: mainImage, Type: 1 });
    listImage.push({ url: thumbnail, Type: 3 });
    $.each($("._lstImage "), function () {
        listImage.push({ url: $(this).attr('src'), Type: 2 });
        listOrtherImage.push({ url: $(this).attr('src'), Type: 2 });
    });
    var listMainService = [];
    $.each($(".tbody-addpackageservice > tr"), function () {
        listMainService.push({ ServiceID: $(this).find(".psName").data('id'), Count: $(this).find(".amountValue").val(), DisplayOrder: $(this).find(".displayOrder").val() })
    });
    var additionService = [];
    var id1 = $(".tbody-addaddservice").children("tr");
    $.each(id1, function (key, val) {
        var idnew = $(this).children("td").attr("data-id");
        var dt = { ServiceID: idnew };
        additionService.push(dt);
    })

    if (name == "") {
        swal({
            title: "",
            text: "Please enter name english!",
            icon: "warning",
        });
        return;
    }
    if (nameVn == "") {
        swal({
            title: "",
            text: "Please enter name Vietnam!",
            icon: "warning",
        });
        return;
    }
    if (promote == "") {
        swal({
            title: "",
            text: "Please enter promote!",
            icon: "warning",
        });
        return;
    }
    if (listSegPrice.length == 0) {
        swal({
            title: "",
            text: "Please add price!",
            icon: "warning",
        });
        return;
    }
    if (mainImage == "/Uploads/files/add_img.png") {
        swal({
            title: "",
            text: "Please choose main image!",
            icon: "warning",
        });
        return;
    }
    if (thumbnail == "/Uploads/files/add_img.png") {
        swal({
            title: "",
            text: "Please choose thumbnail!",
            icon: "warning",
        });
        return;
    }
    if (listOrtherImage.length == 0) {
        swal({
            title: "",
            text: "Please choose orther image!",
            icon: "warning",
        });
        return;
    }
    if (listMainService.length == 0) {
        swal({
            title: "",
            text: "Please choose package service!",
            icon: "warning",
        });
        return;
    }
    var checkEmptyDisplay = false;
    $.each($(".displayOrder"), function () {
        if ($(this).val() == "") {
            checkEmptyDisplay = true;
        }
    });
    if (checkEmptyDisplay == true) {
        swal({
            title: "",
            text: "Please enter display order for package service!",
            icon: "warning",
        });
        return;
    }
    if (additionService.length <= 0) {
        swal({
            title: "",
            text: "Please choose addition service!",
            icon: "warning",
        });
        return;
    }
    //check chua nhap gia
    var checkPriceValue = false;
    $.each($(".priceValue"), function () {
        if ($(this).val() == "") {
            checkPriceValue = true;
        }
    })
    if (checkPriceValue == true) {
        swal({
            title: "",
            text: "Please enter price for segment!",
            icon: "warning",
        });
        return;
    }
    var checkData = {
        ID: id,
        NameEN: name,
        NameVN: nameVn,
        IsActive: status,
        Description: description,
        Discount: promote,
        ListIamge: listImage,
        Icon: icon,
        ListPackageService: listMainService,
        AdditionService: additionService,
        ListComboServicePrice: listSegPrice
    };
    console.log(checkData);
    $.ajax({
        url: '/Combo/UpdateCombo',
        type: 'POST',
        data: checkData,
        success: function (res) {
            if (res == 1) {
                swal({
                    title: "",
                    text: "Update combo successful!",
                    icon: "success",
                });
                window.location.href = "/Combo/Index";
            }
            else {
                swal({
                    title: "",
                    text: "Update combo failed!",
                    icon: "warning",
                });
            }
        }
    })
}
function DeleteCombo(id) {
    swal({
        title: "Are you sure want to delete this combo ?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Combo/DeleteComboService',
                type: 'POST',
                data: { id: id },
                success: function (res) {
                    if (res == 1) {
                        searchCombo();
                        swal({
                            title: "",
                            text: "Delete combo successful!",
                            icon: "success",
                        });
                    }
                    else {
                        swal({
                            title: "",
                            text: res.Message,
                            icon: "success",
                        });
                    }
                }
            })
        }
    });
}
function searchReviewInfo() {
    var searchKey = $.trim($('#txtName').val());
    var rate = $("#txRate").val();
    var fromDate = $.trim($('#dtFromdateIndex').val());
    var toDate = $.trim($('#dtTodateIndex').val());

    if (rate.length > 0) {
        if (parseFloat(rate) > 5 || isNaN(rate) || parseFloat(rate) < 0) {
            swal({
                title: "Average rate number must be less than 5 and must be a positive number!",
                text: "",
                icon: "warning",
            });
            return;
        }
    }

    $.ajax({
        url: '/Reviews/Search',
        data: { searchKey: searchKey, fromDate: fromDate, toDate: toDate, searchRating: rate },
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (response) {
            $("#modalLoad").modal("hide");
            $('#ListReviews').html(response);
        }
    });
}

function saveRatingOfAdmin(id) {
    var rateNumber = $('#txtRate').text();

    if (parseInt(rateNumber) > 5) {
        swal({
            title: "Rate number must be less than 5!",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({

        url: "/Reviews/SaveRateOfAdmin",
        data: { ID: id, NumberRate: rateNumber },
        type: "POST",
        dataType: "Json",
        success: function (Response) {
            if (Response.Status) {
                swal({
                    title: "Update data successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        location.reload();
                    }
                })
            } else {
                swal({
                    title: "Error!",
                    text: "",
                    icon: "error"
                })
            }
        }
    })
}

//Save data update Wallet
function saveWallet() {
    //var desposit = $('#txtMinimunBalanceDesposit').val().replace(/\s\s+/g, ' ');
    //var balanceCash = $('#txtMinimunBalanceCash').val().replace(/\s\s+/g, ' ');
    //var sendMess = $('#txtMinimunBalanceSendMess').val().replace(/\s\s+/g, ' ');
    //if (parseInt(desposit) <= 0 || parseInt(balanceCash) <= 0 || parseInt(sendMess) <= 0 || desposit.length == 0 || balanceCash.length == 0 || sendMess.length == 0) {
    //    swal({
    //        title: "Warning!",
    //        text: "Làm ơn nhập đầy đủ dữ liệu",
    //        icon: "warning"
    //    })
    //    return;
    //}
    var result = 0;
    $.ajax({
        url: "/Config/UpdateWalletDataConfig",
        type: "POST",
        data: $('#frmWallet').serialize(),
        success: function (e) {
            result = e.Status;
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload()
                }
            })
        }
    })
}

//save data update transaction config
function saveUpdateTransactionConfig() {
    //var countdown = $('#countdown').val().replace(/\s\s+/g, ' ');
    //var cancelTransaction = $('#cancelTransaction').val().replace(/\s\s+/g, ' ');
    //var maxcod = $('#maxcod').val().replace(/\s\s+/g, ' ');
    //if (parseInt(countdown) <= 0 || parseInt(cancelTransaction) <= 0 || parseInt(maxcod) <= 0 || countdown.length == 0 || cancelTransaction.length == 0 || maxcod.length == 0) {
    //    swal({
    //        title: "Warning!",
    //        text: "Làm ơn nhập đầy đủ tất cả dữ liệu!!!",
    //        icon: "warning"
    //    })
    //    return;
    //}
    var result = 0;
    $.ajax({
        url: "/Config/UpdateTransactionDataConfig",
        type: "POST",
        data: $('#frmtransaction').serialize(),
        success: function (e) {
            result = e.Status;
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload()
                }
            })
        }
    })
}


function saveUpdateConfigTime() {
    var val = [];

    $.each($('#configtime-table > .val-configtime-id'), function () {
        val.push({ StartTime: $(this).find(".start-time").val(), EndTime: $(this).find(".end-time").val(), Price: parseInt($(this).find(".price").val().replace(/,/g, '')), Description: $(this).find(".desc").val() })
    })

    var result = 0;
    $.ajax({
        url: "/Config/UpdatePeakHourDataConfig",
        type: "POST",
        data: { input: val },
        success: function (e) {
            result = e.Status;
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload()
                }
            })
        }

    })
}


function AddTransportWeight() {
    var arr = [];
    var transportID = $('#val-transport-area-id').val();
    var check = $('.class-tr-transportweight').length;
    if (check == 0) {
        swal({
            title: "Dữ liệu bảng không được để trống",
            icon: "error"
        })
        return;
    }


    $.each($('#transport-weight-table > .class-tr-transportweight'), function () {
        arr.push({ Weight: $(this).find(".transport-weight").val(), Price: $(this).find(".transport-price").val().replace(',', '') })
    })
    var data = { 'TransportAreaID': transportID, 'ListConfigTransportAreaPrice': arr };

    var result = 0;
    $.ajax({
        url: "/Config/AddConfigTranspotAreaPrice",
        type: "POST",
        data: { input: data },
        success: function (e) {
            result = e.Status;
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload()
                }
            })
        }

    })
}

//Update GPS config
function saveUpdateAreaConfig() {
    //var valGPSnotValid = parseInt($('#valGPSnotValid').val());
    //var valMaximumArea = parseInt($('#valMaximumArea').val());
    //if (isNaN(valGPSnotValid) || valGPSnotValid < 0 || isNaN(valMaximumArea) || valMaximumArea < 0) {
    //    swal({
    //        title: "Warning!",
    //        text: "Values in " + "GPS and service area" + " must be positive number",
    //        icon: "warning"
    //    })
    //    return;
    //}

    var result = 0;
    $.ajax({
        url: "/Config/UpdateAreaDataConfig",
        type: "POST",
        data: $('#fromGPS').serialize(),
        success: function (e) {
            result = e.Status;
            swal({
                title: e.Message,
                icon: e.Status == SUCCESS ? "success" : "error"
            }).then((sc) => {
                if (result == SUCCESS && sc) {
                    location.reload()
                }
            })
        }

    })
}
//Update data shift
function saveUpdateShift() {
    var data = [];
    var listDay = [];
    var listShift = [];
    $.each($('.typeShift'), function () {
        listDay = [];
        listShift = [];
        $.each($(this).find('.itemDay'), function () {
            listDay.push({ ID: $(this).data('id'), Day: $(this).data('name') });
        });
        $.each($(this).find('.valCheckbox'), function () {
            listShift.push({ ID: $(this).find('.valIDShift').val(), shift: $(this).find('.valTime').val(), IsActive: $(this).find('.checkbox').prop('checked') == true ? 1 : 0 });
        });
        data.push({ ID: $(this).data('id'), Name: $(this).data('name'), ListDays: listDay, ListShift: listShift });
    });
    console.log("data:", data);
    //$.each($('.valCheckbox'), function () {
    //    data.push({ ID: $(this).find('.valIDShift').val(), time: $(this).find('.valTime').val(), IsActive: $(this).find('.checkbox').prop('checked') == true ? 1 : 0 });
    //});
    setTimeout(function () {
        $.ajax({
            url: '/Config/SaveUpdateShift',
            data: { data: data },
            type: "POST",
            success: function (res) {
                if (res.Status == 1) {
                    swal({
                        title: "Update data successfully",
                        text: "",
                        icon: "success"
                    })
                } else {
                    swal({
                        title: "Error!",
                        text: "",
                        icon: "warning"
                    })
                }
            }
        })
    }, 500);

}

//search commission

function searchCommission() {
    var key = $.trim($('#txtLevelName').val());

    $.ajax({
        url: "/Config/GetListCommision",
        data: { searchKey: key },
        success: function (res) {
            $("#tblCommission").html(res);
        }
    })
}

function addCommission() {
    var name = $.trim($('#txtNameAdd').val());
    var atCare = $.trim($('#txtAutocare').val());
    var duration = $.trim($('#txtDuration').val());
    var process = $.trim($('#txtProcess').val());

    if (name.length == 0) {
        swal({
            title: "Please enter level name",
            text: "",
            icon: "warning"
        })
        return;
    } else if (atCare.length == 0) {
        swal({
            title: "Please enter car autocare academy",
            text: "",
            icon: "warning"
        })
        return;
    } else if (duration.length == 0) {
        swal({
            title: "Please enter duration",
            text: "",
            icon: "warning"
        })
    } else if (process.length == 0) {
        swal({
            title: "Please enter process",
            text: "",
            icon: "warning"
        })
    } else {
        $.ajax({
            url: "/Config/AddCommission",
            data: $("#frmAddCommission").serialize(),
            type: "POST",
            success: function (res) {
                if (res.Status == 1) {
                    swal({
                        title: "Successfully",
                        text: "",
                        icon: "success"
                    })
                    $("#mdAddCommission").modal("hide");
                    searchCommission();
                } else {
                    swal({
                        title: "Error!",
                        text: "",
                        icon: "warning"
                    })
                }
            }
        })
    }
}


//Save data update commission

function SaveEditCommision() {
    var name = $.trim($('#txtNameDT').val());
    var carAutoACD = $.trim($('#txtAutocareDT').val());
    var duration = $.trim($('#txtDurationDT').val());
    var process = $.trim($('#txtProcessDT').val());

    if (name.length == 0) {
        swal({
            title: "Please enter level name",
            text: "",
            icon: "warning"
        })
        return;
    } else if (carAutoACD.length == 0) {
        swal({
            title: "Please enter car autocare academy",
            text: "",
            icon: "warning"
        })
        return;
    } else if (duration.length == 0) {
        swal({
            title: "Please enter duration",
            text: "",
            icon: "warning"
        })
    } else if (process.length == 0) {
        swal({
            title: "Please enter process",
            text: "",
            icon: "warning"
        })
    } else {
        $.ajax({
            url: "/Config/SaveUpdateCommision",
            data: $("#frmCommissionDetail").serialize(),
            type: "POST",
            success: function (res) {
                if (res.Status == 1) {
                    swal({
                        title: "Successfully",
                        text: "",
                        icon: "success"
                    })
                    $("#mdCommissionDetail").modal("hide");
                    searchCommission();
                } else {
                    swal({
                        title: "Error!",
                        text: "",
                        icon: "warning"
                    })
                }
            }
        })
    }
}


//checkdatetime
function checkdatetime(date) {
    var n = new Date();
    var x = n.toLocaleDateString().split("/");
    var t = date.split("/");
    console.log(t);
    if (parseInt(t[2]) > parseInt(x[2])) {
        return false;
    }
    else if (parseInt(t[2]) == parseInt(x[2])) {
        if (parseInt(t[1]) == parseInt(x[0])) {
            if (parseInt(t[0]) >= parseInt(x[1])) {
                //console.log(t[0], n.getDate());
                return false;

            }
        }
        else if (parseInt(t[1]) > parseInt(x[0])) {
            //console.log(t[1], n.getMonth());
            return false;
        }
    }

    return true;
}

function deleteTransportCost(id) {
    debugger;
    swal({
        title: "Bạn có muốn xóa bản ghi này?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Config/DeleteTransportCost',
                    data: { ID: id },
                    type: "POST",
                    success: function (e) {
                        result = e.Status;
                        swal({
                            title: e.Message,
                            icon: e.Status != SUCCESS ? "success" : "error"
                        }).then((sc) => {
                            if (result != SUCCESS && sc) {
                                location.reload()
                            }
                        })
                    }
                })
            }
        })
}

//Delete commission

function delCommission(id) {
    swal({
        title: "Are you sure to delete?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Config/DeleteCommission',
                    data: { ID: id },
                    type: "POST",
                    success: function (response) {
                        if (response.Status == 1) {
                            swal({
                                title: "Deleted successfully",
                                text: "",
                                icon: "success",
                            }).then((success) => {
                                if (success) {
                                    searchCommission();
                                }
                            })
                        } else {
                            swal({
                                title: "Error!",
                                text: "",
                                icon: "warning"
                            })
                        }
                    }
                });
            }
        })
}

//search service area
function SearchServiceArea() {
    var prID = $("#slProvince").val();
    var dtID = $("#slDistrict").val();


    $.ajax({
        url: "/Config/SearchServiceArea",
        data: { Page: 1, ProvinceID: prID, DistrictID: dtID },
        type: "GET",
        success: function (res) {
            $("#tblServiceArea").html(res);
        }
    })
}
function SearchGrade() {
    var search = $("#txtSearch").val().trim();
    var fromDate = $("#txtFromDate").val().trim();
    var toDate = $("#txtToDate").val().trim();
    $.ajax({
        url: '/Grade/Search',
        type: 'GET',
        data: {
            page: 1,
            search: search,
            fDate: fromDate,
            tDate: toDate
        },
        success: function (res) {
            $("#tableGrade").html(res);
        }
    })
}
function ClearFilterGrade() {
    $("#txtSearch").val("");
    $("#txtFromDate").val("");
    $("#txtToDate").val("");
    $.ajax({
        url: '/Grade/Search',
        type: 'GET',
        data: {
            page: 1,
            search: "",
            fDate: "",
            tDate: ""
        },
        success: function (res) {
            $("#tableGrade").html(res);
        }
    })
}
function UpdateTypeCar(id) {

    var nameEN = $("#gradeName_en").val().trim();
    var nameVN = $("#gradeName_vi").val().trim();
    var note = $("#note").val().trim();
    if (nameEN == "") {
        swal({
            title: "",
            text: "Please enter English name",
            icon: "warning"
        })
        return;
    }
    if (nameVN == "") {
        swal({
            title: "",
            text: "Please enter Vietnamese name",
            icon: "warning"
        })
        return;
    }
    if (note == "") {
        swal({
            title: "",
            text: "Please enter description",
            icon: "warning"
        });
        return;
    }

    $.ajax({
        url: '/Grade/UpdateTypeCar',
        type: 'POST',
        data: {
            ID: id,
            nameEN: nameEN,
            nameVN: nameVN,
            note: note
        },
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "Success",
                    text: "Update type car successfully",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        location.reload();
                    }
                })
            }
            else {
                swal({
                    title: "Failed",
                    text: "The system is maintain!",
                    icon: "error"
                });
            }
        },
        error: function () {
            swal({
                title: "Failed",
                text: "The system is maintain!",
                icon: "error"
            });
        }

    })
}
//Load district to add service area
function loadListDistrictArea(prvID) {
    $.ajax({
        url: "/Customer/LoadDistrict",
        data: { ProvinceID: prvID },
        success: function (result) {
            $("#slDistrictArea").empty();
            $('#slDistrictArea').html(result);
        }
    });
}

//Load district to edit service area
function loadListDistrictAreaED(prvID, districtID) {
    $.ajax({
        url: "/Customer/LoadDistrict",
        data: { ProvinceID: prvID },
        success: function (result) {
            $("#slDistrictAreaED").empty();
            $('#slDistrictAreaED').html(result);
            $("#slDistrictAreaED").val(districtID);
        }
    });
}


//Add service area
function AddServiceArea() {
    var provinceID = $('#slProvinceArea').val();
    var districtID = $('#slDistrictArea').val();
    var type = $('#typeArea').val();

    if (provinceID == null || districtID == null || isNaN(provinceID) || isNaN(districtID)) {
        swal({
            title: "Please select province and district",
            text: "",
            icon: "warning"
        })

        return;
    }



    $.ajax({
        url: "/Config/AddServiceArea",
        data: $('#fmAddServiceArea').serialize(),
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "Add service area successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        $('#mdAddServiceArea').modal("hide");
                        SearchServiceArea();
                    }
                });
            } else {
                swal({
                    title: "Error!",
                    text: "",
                    icon: "warning"
                })
            }
        }
    });
}

//Save edit service area 
function SaveEditServiceArea() {
    var provinceID = $('#slProvinceAreaED').val();
    var districtID = $('#slDistrictAreaED').val();

    if (provinceID == null || districtID == null || isNaN(provinceID) || isNaN(districtID)) {
        swal({
            title: "Please select province and district",
            text: "",
            icon: "warning"
        })

        return;
    }

    $.ajax({
        url: "/Config/SaveEditServiceArea",
        data: $("#fmEditServiceArea").serialize(),
        type: "POST",
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "Update data successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        $('#mdEditServiceArea').modal("hide");
                        SearchServiceArea();
                    }
                })
            } else {
                swal({
                    title: "Error!",
                    text: "",
                    icon: "warning"
                })
            }
        }
    })
}

//delete service are

function delServiceArea(id) {
    swal({
        title: "Are you sure want to delete ?",
        text: "",
        icon: "warning"
    }).then((sure) => {
        if (sure) {
            $.ajax({
                url: "/Config/DelServiceArea",
                data: { ID: id },
                type: "POST",
                success: function (res) {
                    if (res.Status = 1) {
                        swal({
                            title: "Deleted successfully",
                            text: "",
                            icon: "success"
                        }).then((success) => {
                            if (success) {
                                SearchServiceArea();
                            }
                        })
                    } else {
                        swal({
                            title: "Error!",
                            text: "",
                            icon: "warning"
                        })
                    }
                }
            })
        }
    })
}

//search Q&A
function seachQA() {
    var sKey = $.trim($('#txtQuestion').val());
    $.ajax({
        url: "/Config/searchQA",
        data: { searchKey: sKey },
        success: function (res) {
            $('#tblQA').html(res);
        }
    })
}

//Add Q&A

function addQA() {
    var answer = $.trim(CKEDITOR.instances['txtContentAnswer'].getData());
    var order = $('#valOrder').val();
    var type = $('#valType').val();
    var question = $.trim($('#txtQuestionAdd').val());

    if (answer.length == 0 || isNaN(order) || question.length == 0 || order.length == 0) {
        swal({
            title: "Please input full data",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (parseInt(order) == 0) {
        swal({
            title: "Order value must be greater than 0",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({

        url: "/Config/AddQA",
        data: { Answer: answer, Type: type, Question: question, OrderDisplay: order },
        type: "POST",
        success: function (res) {
            if (res.Status == 1) {
                $('#valOrder').empty();
                CKEDITOR.instances['txtContentAnswer'].setData("");
                swal({
                    title: "Add data successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        $('#mdAddQA').modal("hide");
                        seachQA();
                    }
                })
            }
        }
    })
}

//save edit data QA
function saveEditQA(id) {
    var answer = $.trim(CKEDITOR.instances['txtContentAnswerED'].getData());
    var order = $('#valOrderED').val();
    var type = $('#valTypeED').val();
    var question = $.trim($('#txtQuestionED').val());

    if (answer.length == 0 || isNaN(order) || question.length == 0 || order.length == 0) {
        swal({
            title: "Please input full data",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/Config/SaveEditQA",
        data: { Answer: answer, Type: type, Question: question, OrderDisplay: order, ID: id },
        type: "POST",
        success: function (res) {
            if (res.Status = 1) {
                swal({
                    title: "Upadte data successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        seachQA();
                        $('#mdEditQA').modal("hide");
                    }
                })
            } else {
                swal({
                    title: "Error!",
                    text: "",
                    icon: "warning"
                })
            }
        }
    })
}

//Del Q&A
function delQA(id) {
    swal({
        title: "Are you want to delete ?",
        text: "",
        icon: "warning"
    }).then((sure) => {
        if (sure) {
            $.ajax({
                url: "/Config/DeleteQA",
                type: "GET",
                data: { ID: id },
                success: function (res) {
                    if (res.Status = 1) {
                        swal({
                            title: "Deleted successfully",
                            text: "",
                            icon: "success"
                        }).then((success) => {
                            if (success) {
                                seachQA();
                            }
                        })
                    } else {
                        swal({
                            title: "Error!",
                            text: "",
                            icon: "warning"
                        })
                    }
                }
            })
        }
    })
}
function searchCarAdd() {
    var status = $('#slStatus').val().trim();
    var fromDate = $('#dtFromDate').val().trim();
    var toDate = $('#dtToDate').val().trim();
    var skey = $.trim($('#txtSkey').val());
    $.ajax({
        url: "/AddCar/Search",
        data: {
            page: 1,
            status: status,
            fromDate: fromDate,
            toDate: toDate,
            searchKey: skey
        },
        type: "GET",
        success: function (result) {
            $('#tableAddCar').html(result);
        }
    });
}
function ClearFilterCarAdd() {
    $.ajax({
        url: "/AddCar/Search",
        data: {
            page: 1,
            status: null,
            fromDate: "",
            toDate: "",
            searchKey: ""
        },
        type: "GET",
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (result) {
            $("#modalLoad").modal("hide");
            $('#tableAddCar').html(result);
        }
    });
}
//save add car management

function saveAddCar(status) {
    var id = $('#valIDAddCar').val();
    var idd = $('#valIDAddCarr').val();
    var contentReject = $.trim($('#txtContentReject').val());

    swal({
        title: "Are you want to change status ?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
    }).then((sure) => {
        if (sure) {

            if (status == 0 && contentReject.length == 0) {
                swal({
                    title: "Please enter content reject!",
                    text: "",
                    icon: "warning"
                })
                return;
            }
            $('#modalLoad').modal("show");
            $.ajax({
                url: "/AddCar/SaveAddCarDetail",
                data: { ID: isNaN(id) || id == null || id == "" ? idd : id, Status: status, Note: contentReject },
                type: "POST",
                success: function (res) {
                    if (res.Status = 1) {
                        swal({
                            title: "Update data successfully",
                            text: "",
                            icon: "success"
                        }).then((success) => {
                            if (success) {
                                $('#modalLoad').modal("hide");
                                $('#contentReject').modal('hide');
                                $('#addCarDetail').modal('hide');
                                $('.modal-backdrop').hide();
                                searchCarAdd();
                            }
                        })
                    } else {
                        swal({
                            title: res.Message,
                            text: "",
                            icon: "warning"
                        })
                    }
                }
            })
        }

    })
}

//add brand in request add car

function saveAddBrand() {
    var brandNane = $.trim($('#txtBrandAdd').val());

    if (brandNane.length == 0) {
        swal({
            title: "Please enter brand name!",
            text: "",
            icon: "warning"
        })

        return;
    }

    $.ajax({
        url: "/AddCar/AddBrand",
        data: { BrandName: brandNane },
        type: "POST",
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "Add new brand successflly",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        $("#txtBrand").val(brandNane);
                        $("#mdAddBrand").modal("hide");
                        $('#createBrand').modal('hide');
                    }
                })
            } else {
                swal({
                    title: res.Message,
                    text: "",
                    icon: "error"
                })
            }
        }
    })
}

function addNewType() {
    var typeName = $.trim($('#txtTypeNameAdd').val());
    var brandName = $('#txtBrandn').text();
    var gradeID = $('#valGradeID').val();
    if (typeName.length == 0) {
        swal({
            title: "Please enter model name!",
            text: "",
            icon: "warning"
        })
        return;
    }
    $.ajax({
        url: "/AddCar/AddNewType",
        data: { BrandName: brandName, TypeName: typeName, GradeID: gradeID },
        type: "POST",
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "Add new car successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        $("#mdAddType").modal("hide");
                        $("#mdAddBrand").modal("hide");
                        $('#mdAddCar').modal("hide");
                        $('#addCarDetail').modal("hide");
                        $('.modal-backdrop').hide();
                        saveAddCar(1);
                    }
                })
            } else {
                swal({
                    title: res.Message,
                    text: "",
                    icon: "error"
                })
            }
        }
    })
}

//save add car request

function saveAddCarInRequest() {
    var brandName = $.trim($('#txtBrand').val());
    var typeName = $.trim($('#txtType').val());
    var gradeID = $('#valGradeID').val();
    if (brandName.length == 0 || typeName.length == 0) {
        swal({
            title: "Please enter full data!",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: "/AddCar/AddNewType",
        data: { BrandName: brandName, TypeName: typeName, GradeID: gradeID },
        type: "POST",
        success: function (res) {
            if (res.Status == 1) {
                swal({
                    title: "Add new car successfully",
                    text: "",
                    icon: "success"
                }).then((success) => {
                    if (success) {
                        $('#mdAddCar').modal("hide");
                        $('#addCarDetail').modal("hide");
                        $('.modal-backdrop').hide();
                        saveAddCar(1);
                    }
                })
            } else {
                swal({
                    title: res.Message,
                    text: "",
                    icon: "error"
                })
            }
        }
    })
}

//delete customer 
function delCus(id) {
    swal({

        title: "Bạn chắc chắn muốn xóa chứ ?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
    }).then((sure) => {
        if (sure) {
            $.ajax({
                url: "/Customer/DelCus",
                data: { ID: id },
                type: "GET",
                success: function (res) {
                    if (res == 1) {
                        swal({
                            title: "Deteted successfully",
                            text: "",
                            icon: "success"
                        }).then((success) => {
                            if (success) {
                                location.reload();
                            }
                        })
                    } else {
                        swal({
                            title: "Error!",
                            text: "",
                            icon: "error"
                        })
                    }
                }
            })
        }
    })
}
function LargeImage(event) {
    $("#showImage").modal('show');
    var src = $(event.target).attr('src');
    $(".divShowImg").empty();
    $(".divShowImg").append('<img src="' + src + '" style="max-width:500px;" />');
}
function SearchCategory() {
    var name = $("#txtName").val().trim();
    var fromDate = $("#dtFromDate").val().trim();
    var toDate = $("#dtToDate").val().trim();
    $.ajax({
        url: '/ProductCategory/Search',
        type: 'GET',
        data: {
            page: 1,
            name: name,
            fromDate: fromDate,
            toDate: toDate
        },
        success: function (res) {
            $("#ListCategory").html(res);
        }
    })
}
//Nút lưu khi thêm mới ServiceCategory
function AddServiceCategory() {
    var _name = $("#addNameCategory").val();
    var _order = $("#addOrderCategory").val();
    var _img = $("#add-img").attr("data-value");
    var _code = $("#addCodeCategory").val();
    if (_name == "" || _order == "" || _img == "" || _code == "") {
        swal({
            title: "",
            text: "Xin hãy điền đầy đủ thông tin",
            icon: "warning"
        });
        return;
    }
    $.ajax({
        url: '/ServiceCategory/Create',
        type: 'POST',
        data: {
            Name: _name,
            Order: _order,
            img: _img,
            code: _code,
        },
        success: function (res) {
            swal({
                title: res.Message,
                icon: res.Status == SUCCESS ? 'success' : 'error'
            }).then((rp) => {
                if (rp) {
                    $("#addProduceCategory").modal('hide');
                    SearchServiceCategory();
                    $('.modal-backdrop').remove();
                }
            })
            //if (res == 1) {
            //    swal({
            //        title: "",
            //        text: "Thêm mới thành công",
            //        icon: "success"
            //    });
            //    $("#addProduceCategory").modal('hide');
            //    SearchServiceCategory();
            //    $('.modal-backdrop').remove();
            //}
            //else {
            //    swal({
            //        title: "",
            //        //text: res.Exception,
            //        text: "Thêm mới thất bại",
            //        icon: "error"
            //    })
            //}
        }
    })
}
function GetByIdCategory(id) {
    $.ajax({
        url: '/ProductCategory/GetById',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            console.log(res.Result);
            $("#txt_id").val(res.Result.ID);
            $("#txt_codeEdit").val(res.Result.Code);
            $("#txt_nameEN_Edit").val(res.Result.Name);
            $("#txt_nameVN_Edit").val(res.Result.NameVN);
            $("#txt_displayEdit").val(res.Result.DisplayOrder);
            $("#sl_status").val(res.Result.Status);
            console.log(res.Result.IsProductCart);
            if (res.Result.IsProductCart && res.Result.Status == 1) {
                $("#sl_status").attr("disabled", true);
            } else {
                $("#sl_status").attr("disabled", false);
            }
        }
    })
}

//Nút lưu ServiceCategory
function SaveServiceCategory() {
    var _id = $("#txtCateID").val();
    var _name = $("#txtCateName").val();
    var _orderDisplay = $("#txtCateOrderDisplay").val();
    var _icon = $("#editImgLogoPlace").attr("data-value");
    if (_icon.length == 0) {
        swal({
            title: "Vui lòng chọn ảnh cho danh mục!",
            icon: "warning"
        })
        return;
    }

    var _active = $("#txtCateIsActiveEdit").val();

    $.ajax({
        url: '/ServiceCategory/SaveServiceCategory',
        type: 'POST',
        data: {
            id: _id,
            Name: _name,
            Icon: _icon,
            IsActive: _active,
            OrderDisplay: _orderDisplay,
        },
        success: function (res) {
            $('#editServicesCategory').modal('hide');
            if (res) {
                swal({
                    title: "",
                    text: "Lưu thành công",
                    icon: "success"
                });
                SearchServiceCategory();
            }
            else {
                swal({
                    title: "",
                    text: "Lưu thất bại",
                    icon: "error"
                })
            }
        }
    })
}
//Binding dữ liệu
function DetailServiceCategory(id) {
    $.ajax({
        url: '/ServiceCategory/GetCategoryInfo',
        type: 'GET',
        data: {
            id: id,
        },
        success: function (res) {
            $("#txtCateID").val(res.ID);
            $("#txtCateName").val(res.Name);
            $("#txtCateOrderDisplay").val(res.OrderDisplay);
            $("#editImgLogoPlace").attr('src', res.Icon);
            $("#editImgLogoPlace").attr('data-value', res._valueIcon);
            $("#txtCateIsActiveEdit").val(res.IsActive);
            $("#editServicesCategory").modal('show');

        }
    })
}
//xóa
function DeleteServiceCate(id) {
    swal({
        title: "Bạn chắc chắn muốn xóa bản ghi này không?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/ServiceCategory/DeleteServiceCategory',
                type: 'POST',
                data: { id: id },
                success: function (res) {
                    swal({
                        title: res.Message,
                        icon: res.Status == SUCCESS ? 'success' : 'error'
                    }).then((rp) => {
                        if (rp) {
                            SearchServiceCategory();
                        }
                    })
                }
            })
        }
    })
}
function SearchOrder() {
    var name = $("#txtName").val().trim();
    var status = $("#sl_order").val().trim();
    var fromDate = $("#dtFromDateIndex").val().trim();
    var toDate = $("#dtTodateIndex").val().trim();
    if (status == 0) {
        status = null;
    }
    $.ajax({
        url: '/Order/Search',
        type: 'GET',
        data: {
            page: 1,
            search: name,
            status: status,
            fromDate: fromDate,
            toDate: toDate
        },
        success: function (res) {
            $("#ListOrder").html(res);
        }
    })
}
function ClearFilterOrder() {
    $("#txtName").val("");
    $("#sl_order").val(0);
    $("#dtFromDateIndex").val("");
    $("#dtTodateIndex").val("");
    $.ajax({
        url: '/Order/Search',
        type: 'GET',
        data: {
            page: 1,
            search: "",
            status: null,
            fromDate: "",
            toDate: ""
        },
        success: function (res) {
            $("#ListOrder").html(res);
        }
    })
}
function SearchProduct() {
    var name = $("#txtName").val().trim();
    var categoryId = $("#sl_category").val().trim();
    var fromDate = $("#dtFromDate").val().trim();
    var toDate = $("#dtToDate").val().trim();
    if (categoryId == 0) {
        categoryId = null;
    }
    $.ajax({
        url: '/Product/Search',
        type: 'GET',
        data: {
            page: 1,
            name: name,
            categoryId: categoryId,
            fromDate: fromDate,
            toDate: toDate
        },
        success: function (res) {
            $("#ListProduct").html(res);
        }
    })
}
function SearchRanking() {
    var name = $("#txtName").val().trim();
    var fromDate = $("#dtFromDate").val().trim();
    var toDate = $("#dtToDate").val().trim();
    $.ajax({
        url: '/Rank/Search',
        type: 'GET',
        data: {
            page: 1,
            name: name,
            fromDate: fromDate,
            toDate: toDate
        },
        success: function (res) {
            $("#ListRank").html(res);
        }
    })
}
function GetOrderDetail(id) {
    $.ajax({
        url: '/Order/Detail',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            $("#txt_id").val(id);
            $("#lb_name").text(res.WasherName);
            $("#lb_phone").text(res.WasherPhone);
            if (!$(".div_Reason").hasClass("d-none")) {
                $(".div_Reason").addClass("d-none");
            }

            if (res.Status == 5) {
                $(".div_Reason").attr("data-show", 1);
            }
            else {
                $(".div_Reason").attr("data-show", 0);
            }
            $("#lb_createdDate").text(res.CreateDateStr);
            if (res.Discount > 0)
                $("#lb_Coupon").text(res.CouponCode + " ( " + res.DiscountStr + " đ )");
            else
                $("#lb_Coupon").text(0 + " đ");
            $("#lb_Base").text(res.BasePriceStr + " đ");
            $("#lb_total").text(res.PriceStr + " đ");

            if (res.Status == 2) {

                var check = true;
                $.each($("#sl_status").children(), function () {
                    if ($(this).val() == 2) {
                        check = false;
                    }
                })
                if (check)
                    $("#sl_status").append('<option id="pen2" value="2">Pending</option>');
            }
            else {
                $("#sl_status option[value='2']").remove();
            }
            $("#sl_status").val(res.Status);

            $("#listOrderDetail").empty();
            $.each(res.ListOrderDetail, function () {
                $("#listOrderDetail").append('<tr><td>' + this.Name + '</td><td>' + this.qty + '</td><td>' + this.PerPriceStr + '</td><td>' + this.TotalPriceStr + '</td></tr>');
            });
        }
    })
}
function ChangeStatusOrder() {
    var id = $("#txt_id").val();
    var status = $("#sl_status").val();
    var note = status == 5 ? $("#txt_Reason").val().trim() : "";
    if (status == 5 && note.length == 0) {
        swal({
            title: "Please enter a reason !",
            icon: "warning"
        });
        return;
    }
    $.ajax({
        url: '/Order/ChangeStatus',
        type: 'POST',
        data: {
            id: id,
            status: status,
            note: note
        },
        success: function (res) {
            if (res.Status == 1) {
                SearchOrder();
                $("#editOrder").modal('hide');
                if (res.Result) {
                    swal({
                        title: "",
                        text: "Change status order successfuly",
                        icon: "success"
                    });
                }
            }
            else {
                swal({
                    title: "",
                    text: res.Message,
                    icon: "error"
                });
            }
        }
    })
}
//function checkEmptyText(input,str) {
//    if (input == "") {
//        swal({
//            title: "",
//            text: str,
//            icon: "warning"
//        });
//        return;
//    }
//}

function AddProduct() {
    var code = $("#txtCode").val().trim();
    var name = $("#txtName").val().trim();
    var nameVN = $("#txtNameVN").val().trim();
    var category = $("#sl_category").val();
    var quantity = $("#txtQuantity").val().trim();
    var price = $("#txtPrice").val().trim();
    var discount = $("#txtAmountCoupon").val().trim();
    var display = $("#txtDisplayOrder").val().trim();
    var description = CKEDITOR.instances['txtContentCreate'].getData();
    var status = $("#sl_status").val();
    var host = window.location.origin;
    var listImage = [];
    var listitem1 = $('.divimage').find(".dropify");
    var fileData = new FormData();
    $.each(listitem1, function (key, value) {
        if ($(this).get(0).files.length != 0) {
            var files = $(this).get(0).files;
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
                //listImage.push(host + "/Uploads/productImages/" + files[i].name);
            }
        }
    })
    if (code == "" || name == "" || category == 0 || quantity == "" || price == "" || discount == "" || display == "" || description == "" || code == "" || status == "" || nameVN == "") {
        swal({
            title: "",
            text: "Please enter full information !",
            icon: "warning"
        });
        return;
    }
    if (discount >= 100) {
        swal({
            title: "",
            text: "Discount can not more than 100 !",
            icon: "warning"
        });
        return;
    }
    //var checkData = {
    //    Code: code,
    //    Name: name,
    //    NameVN: nameVN,
    //    CategoryID: category,
    //    DisplayOrder: display,
    //    QTY: cms_decode_currency_format(quantity),
    //    BasePrice: cms_decode_currency_format(price),
    //    Discount: discount,
    //    Description: description,
    //    Status: status,
    //    ListImage: listImage
    //}
    //console.log("Data:", checkData);
    $.ajax({
        url: '/Product/UploadImage',
        type: 'POST',
        data: fileData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.Status == 1) {
                listImage = [];
                $.each(res.Result, function () {
                    listImage.push(host + "/Uploads/productImages/" + this);
                });
                $.ajax({
                    url: '/Product/SubmitAddProduct',
                    type: 'POST',
                    data: {
                        Code: code,
                        Name: name,
                        NameVN: nameVN,
                        CategoryID: category,
                        DisplayOrder: display,
                        QTY: cms_decode_currency_format(quantity),
                        BasePrice: cms_decode_currency_format(price),
                        Discount: discount,
                        Description: description,
                        Status: status,
                        ListImage: listImage
                    },
                    success: function (res) {
                        if (res.Status == 1) {
                            swal({
                                title: "",
                                text: "Create product successfuly",
                                icon: "success"
                            });
                            setTimeout(function () {
                                window.location.href = "/Product/Index";
                            }, 500);
                        }
                        else {
                            swal({
                                title: "",
                                text: res.Exception,
                                icon: "warning"
                            });
                        }
                    }
                })
            }
            else {
                swal({
                    title: "",
                    text: res.Message,
                    icon: "warning"
                });
            }
        }
    })
}
function EditProduct() {
    var productId = $("#productId").val().trim();
    var code = $("#txtCode").val().trim();
    var name = $("#txtName").val().trim();
    var nameVN = $("#txtNameVN").val().trim();
    var category = $("#sl_category").val();
    var quantity = $("#txtQuantity").val().trim();
    var price = $("#txtPrice").val().trim();
    var discount = $("#txtAmountCoupon").val().trim();
    var display = $("#txtDisplayOrder").val().trim();
    var description = CKEDITOR.instances['txtContentCreate'].getData();
    var status = $("#sl_status").val();
    var host = window.location.origin;
    var listImageAdd = [];
    var listImage = [];
    var listitem1 = $('.divimage').find(".dropify");
    var fileData = new FormData();
    //lay file anh them
    $.each(listitem1, function (key, value) {
        if ($(this).get(0).files.length != 0) {
            var files = $(this).get(0).files;
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
                listImageAdd.push(host + "/Uploads/productImages/" + files[i].name);
            }
        }
    });
    //lay anh cu
    $.each($(".dropify-render img"), function () {
        if ($(this).attr("src").includes("/Uploads/productImages/")) {
            listImage.push($(this).attr("src"));
        }
    });
    if (code == "" || name == "" || category == 0 || quantity == "" || price == "" || discount == "" || display == "" || description == "" || code == "" || status == "" || nameVN == "") {
        swal({
            title: "",
            text: "Please enter full information !",
            icon: "warning"
        });
        return;
    }
    //if (listImage.length == 0) {
    //    swal({
    //        title: "",
    //        text: "Please enter image !",
    //        icon: "warning"
    //    });
    //    return;
    //}
    if (discount > 100) {
        swal({
            title: "",
            text: "Discount can not more than 100 !",
            icon: "warning"
        });
        return;
    }
    var checkData = {
        ProductID: productId,
        Code: code,
        Name: name,
        NameVN: nameVN,
        CategoryID: category,
        DisplayOrder: cms_decode_currency_format(display),
        QTY: cms_decode_currency_format(quantity),
        BasePrice: cms_decode_currency_format(price),
        Discount: discount,
        Description: description,
        Status: status,
        ListImage: listImage
    }
    console.log("Data:", checkData);
    if (listImageAdd.length == 0) {
        $.ajax({
            url: '/Product/SubmitEditProduct',
            type: 'POST',
            data: checkData,
            success: function (res) {
                if (res.Status == 1) {
                    swal({
                        title: "",
                        text: "Update product successfuly",
                        icon: "success"
                    });
                    setTimeout(function () {
                        window.location.href = "/Product/Index";
                    }, 500);
                }
                else {
                    swal({
                        title: "",
                        text: res.Exception,
                        icon: "warning"
                    });
                }
            }
        })
    }
    else {
        $.ajax({
            url: '/Product/UploadImage',
            type: 'POST',
            data: fileData,
            async: false,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.Status == 1) {
                    listImageAdd = [];
                    $.each(res.Result, function () {
                        listImageAdd.push(host + "/Uploads/productImages/" + this);
                    });
                    listImage = listImage.concat(listImageAdd);
                    $.ajax({
                        url: '/Product/SubmitEditProduct',
                        type: 'POST',
                        data: {
                            ProductID: productId,
                            Code: code,
                            Name: name,
                            NameVN: nameVN,
                            CategoryID: category,
                            DisplayOrder: cms_decode_currency_format(display),
                            QTY: cms_decode_currency_format(quantity),
                            BasePrice: cms_decode_currency_format(price),
                            Discount: discount,
                            Description: description,
                            Status: status,
                            ListImage: listImage
                        },
                        success: function (res) {
                            if (res.Status == 1) {
                                swal({
                                    title: "",
                                    text: "Update product successfuly",
                                    icon: "success"
                                });
                                setTimeout(function () {
                                    window.location.href = "/Product/Index";
                                }, 500);
                            }
                            else {
                                swal({
                                    title: "",
                                    text: res.Exception,
                                    icon: "warning"
                                });
                            }
                        }
                    })

                }
                else {
                    swal({
                        title: "",
                        text: res.Message,
                        icon: "warning"
                    });
                }
            }
        })
    }

}
function Imagechang(event) {
    var checkEmptyImage = false;
    var listitem1 = $('.divimage').find(".dropify");
    $.each(listitem1, function (key, value) {
        if ($(this).get(0).files.length == 0 && $(this).closest(".dropify-wrapper").find(".dropify-preview").css('display') == 'none') {
            checkEmptyImage = true;
        }

    })
    if ($(event.target).get(0).files.length > 0 && checkEmptyImage == false) {
        $('.divimage').append('<div style="width:150px" class="divimagehidden mr-2"><input type="file" class="dropify" data-height="150" onchange="Imagechang(event)" /></div>');
        drop = $('.dropify').dropify({
            messages: {
                default: 'Click to select image',
                replace: 'Click to select another image',
                remove: 'Delete image'
            }
        });
        $('.divimagehidden').last().find('.dropify-clear').attr('onclick', 'removeImage(event)');
    }
}
function removeImage(event) {
    $(event.target).closest('.divimagehidden').remove();
}
function ProductDetail(id) {
    $.ajax({
        url: '/Product/GetListCategory',
        type: 'GET',
        success: function (res) {
            $.each(res, function () {
                $("#sl_category").append('<option value="' + this.ID + '">' + this.Name + '</option>');
            })

        }
    })
    $.ajax({
        url: '/Product/GetById',
        type: 'GET',
        data: { id: id },
        success: function (res) {
            $("#txtCode").val(res.Code);
            $("#txtName").val(res.Name);
            $("#txtNameVN").val(res.NameVN);
            $("#sl_category").val(res.CategoryID);
            $("#txtQuantity").val(res.QTY);
            $("#txtPrice").val(res.BasePriceStr);
            $("#txtAmountCoupon").val(res.Discount);
            $("#txtDisplayOrder").val(res.DisplayOrder);
            $("#sl_status").val(res.Status);
            $("#txtContentCreate").text(res.Description);
            $.each(res.ListImage, function (index, value) {
                $(".divimage").append('<div style="width:150px" class="divimagehidden mr-2" id="' + index + '"><input type="file" class="dropify" data-height="150" data-default-file="' + this + '" name="UrlImage" onchange="Imagechang(event)" /></div>');
                $("#" + index).find(".dropify-render img").attr("src", this);
            });
            $(".divimage").append('<div style="width:150px" class="divimagehidden mr-2"><input type="file" class="dropify" data-height="150" name="UrlImage" onchange="Imagechang(event)" /></div>');
            $('.dropify').dropify({
                messages: {
                    default: 'Click to select image',
                    replace: 'Click to select another image',
                    remove: 'Delete image'
                }
            });
            $.each($('.dropify-clear'), function () {
                $(this).attr('onclick', 'removeImage(event)');
            })
        }
    })
}
function DeleteProduct(id) {
    swal({
        title: "Are you sure to delete this product?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Product/DeleteProduct',
                type: 'POST',
                data: { id: id },
                success: function (res) {
                    if (res.Status == 1) {
                        SearchProduct();
                        swal({
                            title: "",
                            text: "Delete product successfully",
                            icon: "success"
                        });
                    }
                    else {
                        swal({
                            title: "",
                            text: res.Exception,
                            icon: "warning"
                        });
                    }
                }
            })
        }
    })
}



function ForgotPassword() {
    var email = $('#txtEmail').val();

    $.ajax({
        url: '/User/ForgotPassword',
        data: { email: email },
        dataType: 'json',
        success: function (res) {
            $('#mdForgotpassword').modal('hide');
            if (res.Status == SUCCESS) {
                swal({
                    title: res.Message,
                    icon: 'success'
                })
            } else {
                swal({
                    title: res.Message,
                    icon: 'error'
                })
            }
        }
    })
}


//Thêm user
function saveUser() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }

    var name = ReplaceSpace($("#txt-name").val());
    var phone = ReplaceSpace($("#txt-phone").val());
    var password = ReplaceSpace($("#txt-pass").val());
    var confirmpass = ReplaceSpace($("#txt-pass-confirm").val());
    var email = ReplaceSpace($('#txt-email').val());
    var typeUser = ReplaceSpace($('#typeUser').val());

    var phone_validate = new RegExp("^[0-9]{9,11}");
    var email_validate = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
    if (name.length == 0 || email.length == 0 || password.length == 0 || confirmpass.length == 0 || phone.length == 0) {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }

    if (password != confirmpass) {
        swal({
            title: "Xác nhận mật khẩu không đúng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (!phone_validate.test(phone)) {
        swal({
            title: "Số điện thoại không đúng định dạng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (!email_validate.test(email)) {
        swal({
            title: "Email không đúng định dạng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: "/User/CreateUser",
        data: { Name: name, Email: email, Phone: phone, Password: password, typeUser: typeUser },
        type: "POST",
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $('#add-user-account').modal("hide");
            swal({
                title: res.Message,
                icon: res.Status == SUCCESS ? 'success' : 'error'
            }).then((rp) => {
                if (rp) {
                    location.reload();
                }
            })
            window.location = '/User/Index';
            searchUser();
            //.then((rp) => {
            //    if (rp) {
            //        searchUser();
            //    }
            //})
        }
    })
}
function searchCustomerReport() {
    var SearchKey = $('#txtCodeOrName').val();
    var ProvinceID = $('#CusProvince').val();
    var FromDate = $('#txtFromDate').val();
    var ToDate = $('#txtToDate').val();
    $.ajax({
        url: "/StatisticCustomer/SearchCustomerReport",
        data: {
            page: 1,
            serchKey: SearchKey, provinceID: ProvinceID, fromDate: FromDate, toDate: ToDate
        },
        success: function (rs) {
            $("#tableCustomerReport").html(rs)
        }
    })
}

function ReplaceSpace(str) {
    return $.trim(str.replace(/\s\s+/g, ' '));
}

function EditUser() {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }

    var id = $('#val-id').val();
    var name = ReplaceSpace($("#txt-name-detail").val());
    var phone = ReplaceSpace($("#txt-phone-detail").val());
    var password = ReplaceSpace($("#txt-pass-edit").val());
    var confirmpass = ReplaceSpace($("#txt-pass-confirm-edit").val());
    var email = ReplaceSpace($('#txt-email-detail').val());

    var phone_validate = new RegExp("^[0-9]{9,11}");
    var email_validate = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
    debugger;
    if (name.length == 0 || email.length == 0 || phone.length == 0) {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }

    if (password.length > password != confirmpass) {
        swal({
            title: "Xác nhận mật khẩu không đúng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (!phone_validate.test(phone)) {
        swal({
            title: "Số điện thoại không đúng định dạng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (!email_validate.test(email)) {
        swal({
            title: "Email không đúng định dạng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: "/User/UpdateUserInfo",
        data: { id: id, name: name, email: email, phone: phone, password: password },
        type: "POST",
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $('#user-detail').modal("hide");
            swal({
                title: res.Message,
                icon: res.Status == SUCCESS ? 'success' : 'error'
            }).then((rp) => {
                if (rp) {
                    searchUser();
                }
            })
        }
    })
}
function SaveEditUser(id) {
    if (!navigator.onLine) {
        swal({
            title: "Kiểm tra kết nối internet!",
            text: "",
            icon: "warning"
        })
        return;
    }

    var name = ReplaceSpace($("#editUserName").val());
    var email = ReplaceSpace($('#editEmail').val());
    var phone = ReplaceSpace($("#editPhone").val());
    var role = ReplaceSpace($("#editRole").val());
    var password = ReplaceSpace($("#editPass").val());
    var confirmpass = ReplaceSpace($("#editConfirmPassword").val());

    var phone_validate = new RegExp("^[0-9]{9,11}");
    var email_validate = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
    debugger;
    if (name.length == 0 || email.length == 0 || phone.length == 0) {
        swal({
            title: "Thông báo",
            text: "Vui lòng nhập đầy đủ thông tin!",
            icon: "warning"
        })
        return;
    }

    if (password.length > password != confirmpass) {
        swal({
            title: "Xác nhận mật khẩu không đúng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (!phone_validate.test(phone)) {
        swal({
            title: "Số điện thoại không đúng định dạng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    if (!email_validate.test(email)) {
        swal({
            title: "Email không đúng định dạng!",
            text: "",
            icon: "warning"
        })
        return;
    }

    $.ajax({
        url: "/User/UpdateUserInfo",
        data: { id: id, name: name, email: email, phone: phone, password: password, role: role },
        type: "POST",
        beforeSend: function () {
            $("#modalLoad").modal("show");
        },
        success: function (res) {
            $("#modalLoad").modal("hide");
            $('#user-detail').modal("hide");
            swal({
                title: res.Message,
                icon: res.Status == SUCCESS ? 'success' : 'error'
            }).then((rp) => {
                if (rp) {
                    location.reload();
                }
            })
            window.location = '/User/Index';
            searchUser();
            //    .then((rp) => {
            //    if (rp) {
            //        searchUser();
            //    }
            //})
        }
    })
}


function SearchComplain() {
    var searchKeyss = $('#txtCodeName').val();
    var types = $('#slTypeService').val();
    $.ajax({
        url: "/Complain/Search",
        data: {
            page: 1,
            searchKey: searchKeyss,
            type: types
        },
        type: "GET",
        beforeSend: function () {
            $("#modalLoad").modal("hide");
        },
        success: function (rs) {
            $("#modalLoad").modal("hide");
            $('#tbl-complain-order').html(rs);
        }
    })
}

function DeActiveShiper(id) {
    swal({
        title: "Bạn chắc chắn muốn thực hiện thao tác này?",
        text: "",
        icon: "warning",
        buttons: ["Cancel", "OK"],
        dangerMode: true,
    }).then((sure) => {
        if (sure)
            $.ajax({
                url: "/Shipper/DeActiveShiper",
                beforeSend: function () {
                    $('#modalLoad').modal('show');
                },
                data: { id: id },
                type: "PUT",
                success: function (res) {
                    swal({
                        title: res.Message,
                        icon: res.Status == SUCCESS ? "success" : "error",
                    }).then((sc) => {
                        window.location.href = "/Shipper/Index";
                    });
                }
            })
    })
}

