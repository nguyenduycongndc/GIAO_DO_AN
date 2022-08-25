var reName = new RegExp("^[a-zA-Z]{1}");
var reAddress = new RegExp("^[a-zA-Z0-9]{1}");
var rePhone = new RegExp("^[0]{1}[0-9]{9,11}");
var reAge = new RegExp("^[0-9]{1}");
var reEmail = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;

function Send_Mail_Driver() {
    var name_driver = $("#name_driver").val();
    var phone_driver = $("#phone_driver").val();
    var email_driver = $("#email_driver").val();
    var address_driver = $("#address_driver").val();
    var age_driver = $("#age_driver").val();

    if (!reName.test(name_driver)) {
        swal({
            title: "",
            text: "Tên của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reAddress.test(address_driver)) {
        swal({
            title: "",
            text: "Địa chỉ của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reEmail.test(email_driver)) {
        swal({
            title: "",
            text: "Địa chỉ email của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reAge.test(age_driver)) {
        swal({
            title: "",
            text: "Mời nhập chính xác tuổi của bạn!",
            icon: "warning"
        })
        return;
    } else if (!rePhone.test(phone_driver)) {
        swal({
            title: "",
            text: "Số điện thoại của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Cảm ơn!",
            text: "Chúng tôi sẽ liên hệ với bạn sớm nhất có thể!!!!",
            icon: "success",
            button: "OK!",
        }).then((ok) => {
            location.reload();
        })
        Email.send({
            Host: "smtp.gmail.com",
            Username: "sport.weship@gmail.com",
            Password: "jbgceknqdnugwnjp",
            To: 'hr.weship@gmail.com',
            From: 'sport.weship@gmail.com',
            Subject: "Đăng ký trở thành tài xế WeShip",
            Body: '<h3>Có người dùng muốn đăng ký trở thành tài xế của WeShip: </h3>' +
                '<h4>Thông tin của người dùng: </h4>' +
                'Tên người dùng: ' + name_driver +
                '<br> Số điện thoại: ' + phone_driver +
                '<br> Địa chỉ: ' + address_driver +
                '<br> Tuổi: ' + age_driver +
                '<br> Email: ' + email_driver +
                '<br> Hãy liên hệ với người dùng sớm nhất có thể!!!',
        })

    }
}

function Send_Contact() {
    var contact_name = $("#contact_name").val();
    var contact_phone = $("#contact_phone").val();
    var contact_email = $("#contact_email").val();
    var contact_message = $("#contact_message").val();

    if (!reName.test(contact_name)) {
        swal({
            title: "",
            text: "Tên của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!rePhone.test(contact_phone)) {
        swal({
            title: "",
            text: "Số điện thoại của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reEmail.test(contact_email)) {
        swal({
            title: "",
            text: "Địa chỉ email của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reAddress.test(contact_message)) {
        swal({
            title: "",
            text: "Mời nhập lời nhắn bạn muốn gửi tới chúng tôi!",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Cảm ơn!",
            text: "Chúng tôi sẽ liên hệ với bạn sớm nhất có thể!!!!",
            icon: "success",
            button: "OK!",
        }).then((ok) => {
            location.reload();
        })
        Email.send({
            Host: "smtp.gmail.com",
            Username: "sport.weship@gmail.com",
            Password: "jbgceknqdnugwnjp",
            To: 'hr.weship@gmail.com',
            From: 'sport.weship@gmail.com',
            Subject: "Thư liên hệ của người dùng WeShip",
            Body: '<h3>Có người dùng gửi thư liên hệ tới WeShip: </h3>' +
                '<h4>Thông tin của người dùng: </h4>' +
                'Tên người dùng: ' + contact_name +
                '<br> Số điện thoại: ' + contact_phone +
                '<br> Email: ' + contact_email +
                '<br> Lời nhắn: ' + contact_message +
                '<br> Hãy liên hệ với người dùng sớm nhất có thể!!!',
        })
    }
}

function Send_Recruitment() {
    debugger;
    var your_name = $("#your_name").val();
    var your_email = $("#your_email").val();
    var your_age = $("#your_age").val();
    var your_phone = $("#your_phone").val();
    var file = $("#your_file")[0].files[0];

    //var fileType = file.type;
    //var match = ["application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"];

    if (!reName.test(your_name)) {
        swal({
            title: "",
            text: "Tên của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reEmail.test(your_email)) {
        swal({
            title: "",
            text: "Địa chỉ email của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reAge.test(your_age)) {
        swal({
            title: "",
            text: "Mời nhập chính xác tuổi của bạn!",
            icon: "warning"
        })
        return;
    } else if (!rePhone.test(your_phone)) {
        swal({
            title: "",
            text: "Số điện thoại của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    }
    else if (file == "" || file == null) {
        swal({
            title: "",
            text: "Mời chọn cv mà bạn muốn gửi!",
            icon: "warning"
        })
        return;
    } else {
        //if (!((fileType == match[0]) || (fileType == match[1]))) {
        //    swal({
        //        title: "Cảnh báo!",
        //        text: "Vui lòng chọn định dạng file .pdf!",
        //        icon: "warning"
        //    })
        //    return;
        //} else {
            var reader = new FileReader();
            reader.readAsBinaryString(file);
            reader.onload = function () {
                var dataUri = "data:" + file.type + ";base64," + btoa(reader.result);
                swal({
                    title: "Cảm ơn!",
                    text: "Chúng tôi sẽ liên hệ với bạn sớm nhất có thể!!!!",
                    icon: "success",
                    button: "OK!",
                }).then((ok) => {
                    location.reload();
                })
                Email.send({
                    SecureToken: "********",
                    Host: "smtp.gmail.com",
                    Username: "sport.weship@gmail.com",
                    Password: "jbgceknqdnugwnjp",
                    To: 'hr.weship@gmail.com',
                    From: 'sport.weship@gmail.com',
                    Subject: "Thư ứng tuyển công việc của ứng viên tới WeShip",
                    Attachments: [
                        {
                            name: file.name,
                            data: dataUri
                        }
                    ],
                    Body: '<h3>Có ứng viên gửi CV ứng tuyển tới WeShip: </h3>' +
                        '<h4>Thông tin của ứng viên: </h4>' +
                        'Tên ứng viên: ' + your_name +
                        '<br> Số điện thoại: ' + your_phone +
                        '<br> Email: ' + your_email +
                        '<br> Tuổi: ' + your_age +
                        '<br> Hãy liên hệ với ứng viên sớm nhất có thể!!!',
                })
            //}

        }
    }


}

function Send_Cooperate() {
    var business_name = $("#business_name").val();
    var business_chain = $("#business_chain").val();
    var business_phone = $("#business_phone").val();
    var business_email = $("#business_email").val();
    var business_type = $("#business_type").val();
    var select_provinces = $("#select_provinces option:selected").text();
    var select_district = $("#select_district option:selected").text();
    var business_desc = $("#business_desc").val();

    if (!reName.test(business_name)) {
        swal({
            title: "",
            text: "Tên doanh nghiệp của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reName.test(business_chain)) {
        swal({
            title: "",
            text: "Tên của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (select_provinces == "Tỉnh/Thành phố") {
        swal({
            title: "",
            text: "Mời chọn Tỉnh/Thành phố nơi doanh nghiệp bạn đang hoạt động!",
            icon: "warning"
        })
        return;
    } else if (select_district == "Quận/Huyện") {
        swal({
            title: "",
            text: "Mời chọn Quận/Huyện nơi doanh nghiệp bạn đang hoạt động!",
            icon: "warning"
        })
        return;
    } else if (!rePhone.test(business_phone)) {
        swal({
            title: "",
            text: "Số điện thoại của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (!reEmail.test(business_email)) {
        swal({
            title: "",
            text: "Địa chỉ email của bạn không hợp lệ!",
            icon: "warning"
        })
        return;
    } else if (business_type == 0 || business_type == null) {
        swal({
            title: "",
            text: "Mời chọn loại hình doanh nghiệp của bạn!",
            icon: "warning"
        })
        return;
    } else if (!reAddress.test(business_desc)) {
        swal({
            title: "",
            text: "Mời nhập lời nhắn bạn muốn gửi tới chúng tôi!!",
            icon: "warning"
        })
        return;
    }
    else {
        swal({
            title: "Cảm ơn!",
            text: "Chúng tôi sẽ liên hệ với bạn sớm nhất có thể!!!!",
            icon: "success",
            button: "OK!",
        }).then((ok) => {
            location.reload();
        })
        Email.send({
            Host: "smtp.gmail.com",
            Username: "sport.weship@gmail.com",
            Password: "jbgceknqdnugwnjp",
            To: 'hr.weship@gmail.com',
            From: 'sport.weship@gmail.com',
            Subject: "Thư hợp tác của doanh nghiệp tới WeShip",
            Body: '<h3>Có doanh nghiệp gửi thư muốn hợp tác tới WeShip: </h3>' +
                '<h4>Thông tin của doanh nghiệp: </h4>' +
                'Tên doanh nghiệp: ' + business_name +
                '<br> Người đại diện: ' + business_chain +
                '<br> Số điện thoại: ' + business_phone +
                '<br> Email: ' + business_email +
                '<br> Địa chỉ: ' + select_district + '/' + select_provinces +
                '<br> Loại hình doanh nghiệp: ' + business_type +
                '<br> Lời nhắn: ' + business_desc +
                '<br> Hãy liên hệ với doanh nghiệp này sớm nhất có thể!!!',
        })
    }
}

