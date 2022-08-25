function nonAccentVietnamese(str) {
  str = str.toLowerCase();
  str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
  str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
  str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
  str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
  str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
  str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
  str = str.replace(/đ/g, "d");
  str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng
  str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư
  return str;
}
const fetch = require("node-fetch");
const url = `https://play.google.com/store/apps/details?id=com.car.rect.washer`;
fetch(url + "&hl=vi")
  .then(response => response.text())
  .then(html => {
    const index = html.indexOf("Phiên bản hiện tại");
    const nextIndex = html
      .substr(index, 200)
      .toLowerCase()
      .split(`class="htlgb">`);
    var str = nonAccentVietnamese(nextIndex[2]);
    for (let i = 65; i <= 90; i++)
      str = str.split(String.fromCharCode(i).toLowerCase()).join("");
    const version = str
      .split(`>`)
      .join("")
      .split(`<`)
      .join("")
      .split(`/`)
      .join("")
      .split(`=`)
      .join("")
      .split(`"`)
      .join("")
      .trim();

    const { exec } = require("child_process");
    const pushAndroid =
      "appcenter codepush release-react -a Apps-Windsoft/CAR_RECT_CUSTOMER_ANDROID -d prod -t " +
      str;
    const pushIos =
      "appcenter codepush release-react -a Apps-Windsoft/CAR_RECT_CUSTOMER_IOS -d prod -t " +
      str;
    exec(pushAndroid + " && " + pushIos, (err, out, outErr) => {
      console.log(out);
    });
  });
