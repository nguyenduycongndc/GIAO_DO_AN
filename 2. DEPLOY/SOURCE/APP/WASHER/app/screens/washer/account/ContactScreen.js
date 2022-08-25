import React, { Component } from "react";
import { Text, StyleSheet, View, Image, TouchableOpacity } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import ProgressFastImage from "@app/components/ProgressFastImage";
import FastImage from "react-native-fast-image";
import { DEEP_LINK } from "@app/constants/Constants";
import { callPhone, deepLink } from "@app/constants/Functions";
export default class ContactScreen extends Component {
  renderView = (label, view) => (
    <>
      {label != "" && <Text style={styles.label_view} children={label} />}
      <View style={styles.view} children={view} />
    </>
  );
  renderText = (lable, address, phone) => (
    <View>
      <Text style={styles.view_text} children={lable} />
      <Text style={styles.text_address} children={address} />
      <Text style={styles.text_phone} children={phone} />
    </View>
  );
  render() {
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().contact_operator}
        renderView={
          <>
            <View
              style={{
                backgroundColor: theme.colors.backgroundColor,
                flex: 1
              }}
            >
              {this.renderView(
                R.strings().hotline_contact,
                <TouchableOpacity
                  style={{ flexDirection: "row" }}
                  onPress={() => callPhone("0355108995")}
                >
                  {this.renderText(
                    "Bộ phận kỹ thuật",
                    "Số 3, Ngọc Khánh, Ba Đình, Hà Nội",
                    "0355108995"
                  )}
                  <Image source={R.images.ic_call} style={styles.imaga_call} />
                </TouchableOpacity>
              )}
              {this.renderView(
                "Liên hệ qua các phương thức khác",
                <View
                  style={{
                    flexDirection: "row",
                    backgroundColor: theme.colors.white
                  }}
                >
                  <ProgressFastImage
                    uri={R.images.ic_gmail}
                    resizeMode={FastImage.resizeMode.contain}
                    style={styles.image}
                    onPress={() =>
                      deepLink(DEEP_LINK.GMAIL, "info@carrect.vn", "", "")
                    }
                  />
                  <ProgressFastImage
                    uri={R.images.ic_mess}
                    resizeMode={FastImage.resizeMode.contain}
                    style={styles.image}
                    onPress={() => deepLink(DEEP_LINK.MESS_FB, "carrect.vn")}
                  />
                  <ProgressFastImage
                    uri={R.images.ic_zalo}
                    resizeMode={FastImage.resizeMode.contain}
                    style={styles.image}
                    onPress={() => deepLink(DEEP_LINK.ZALO, "0947757293")}
                  />
                </View>
              )}
              {this.renderView(
                "Cộng đồng của chúng tôi",
                <View style={{ flexDirection: "row" }}>
                  <ProgressFastImage
                    uri={R.images.ic_fb}
                    resizeMode={FastImage.resizeMode.contain}
                    style={styles.image}
                    onPress={() =>
                      deepLink(DEEP_LINK.FACEBOOK, "105594494303543")
                    }
                  />
                  <ProgressFastImage
                    style={styles.image}
                    resizeMode={FastImage.resizeMode.contain}
                    uri={R.images.ic_instagram}
                    onPress={() => deepLink(DEEP_LINK.INSTA, "Carrect_vn")}
                  />
                </View>
              )}
            </View>
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  text_address: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13,
    color: colors.gray,
    paddingVertical: 2.5
  },
  view_text: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.primaryDark,
    paddingBottom: 2.5
  },
  view: {
    backgroundColor: colors.white,
    padding: 10,
    borderTopWidth: 0.25,
    borderColor: theme.colors.gray,
    borderBottomWidth: 0.25,
    marginBottom: 10
  },
  label_view: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 5,
    paddingTop: 15,
    color: colors.primaryDark
  },
  image: {
    alignSelf: "center",
    flex: 1,
    height: 50,
    width: 50,
    resizeMode: "contain"
  },
  imaga_call: {
    width: 40,
    height: 40,
    tintColor: colors.primary,
    alignSelf: "center",
    position: "absolute",
    right: 10
  },
  text_phone: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13,
    color: colors.gray,
    paddingVertical: 2.5
  }
});
