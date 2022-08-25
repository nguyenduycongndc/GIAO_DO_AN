import React, { Component } from "react";
import {
  ScrollView,
  Text,
  StyleSheet,
  View,
  TouchableOpacity,
  Image,
  Clipboard
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
import ButtonPrimary from "@app/components/ButtonPrimary";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import * as API from "@app/constants/Api";
import CallApiHelper from "@app/utils/CallApiHelper";
import QRCode from "react-native-qrcode-svg";
import { Avatar } from "react-native-elements";
import Toast from "@app/components/Toast";

// import QRCodeScanner from "react-native-qrcode-scanner";
export class QRCodeScreen extends Component {
  initState = {};
  state = this.initState;
  render() {
    const { UserInfoState } = this.props;
    return (
      <ScreenComponent
        titleHeader={R.strings().personal_information}
        back
        renderView={
          <ScrollView>
            <Toast
              ref="toast"
              defaultCloseDelay={250}
              position="bottom"
              positionValue={200}
              fadeInDuration={750}
              fadeOutDuration={1000}
              opacity={0.8}
            />
            <View
              style={{
                width: width,
                height: width,
                justifyContent: "center",
                alignItems: "center",
                backgroundColor: theme.colors.backgroundColor
              }}
            >
              <View
                style={{
                  backgroundColor: "white",
                  padding: 20
                }}
              >
                <QRCode
                  value={UserInfoState.data.code}
                  size={width / 3}
                  color="black"
                  backgroundColor="white"
                  logo={R.images.ic_booking}
                />
              </View>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_bold,
                  fontSize: 14,
                  marginTop: 20,
                  minHeight: 40,
                  backgroundColor: "#DDDDDD",
                  paddingVertical: 10,
                  paddingHorizontal: 80,
                  borderRadius: 20,
                  overflow: "hidden",
                  borderWidth: 0.5,
                  borderColor: theme.colors.grayBorder
                }}
                onPress={() => {
                  Clipboard.setString(UserInfoState.data.code);
                  this.refs.toast.show(R.strings().copy_success, 2000);
                }}
              >
                {UserInfoState.data.code}
              </Text>
            </View>
            <View
              style={{
                width: "100%",
                backgroundColor: "white",
                minHeight: 50,
                flexDirection: "row",
                alignItems: "center"
              }}
            >
              <Avatar
                rounded
                source={
                  UserInfoState.data.urlAvatar
                    ? { uri: UserInfoState.data.urlAvatar }
                    : R.images.ic_symbol
                }
                overlayContainerStyle={{ backgroundColor: "white" }}
                style={styles.avatar}
                onPress={() => this._pickImage()}
              />
              <View>
                <Text style={styles.info_text}>{UserInfoState.data.name}</Text>
                <Text style={[styles.info_text, { fontSize: 12 }]}>
                  {UserInfoState.data.phone}
                </Text>
              </View>
            </View>

            <View
              style={{
                marginTop: 10,
                backgroundColor: "white"
              }}
            >
              <View
                style={{
                  flexDirection: "row",
                  alignItems: "center"
                }}
              >
                <Image
                  style={{
                    marginVertical: 10,
                    marginLeft: 20,
                    width: 25,
                    height: 25
                  }}
                  source={R.images.ic_tips}
                />

                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_bold,
                    fontSize: 14,
                    marginLeft: 10,
                    color: "#15D108"
                  }}
                >
                  {R.strings().tips}
                </Text>
              </View>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 14,
                  marginLeft: 10,
                  color: theme.colors.black,
                  marginBottom: 10,
                  paddingHorizontal: 10
                }}
              >
                {R.strings().tips_content}
              </Text>
            </View>
          </ScrollView>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(QRCodeScreen);
const styles = StyleSheet.create({
  avatar: {
    width: 60,
    height: 60,
    borderRadius: width / 10,
    overflow: "hidden",
    borderWidth: 0.5,
    margin: 10,
    marginLeft: 20,
    borderColor: theme.colors.grayBorder
  },
  info_text: {
    fontFamily: R.fonts.quicksand_semi_bold,
    fontSize: 16,
    marginHorizontal: 10,
    marginVertical: 3
  }
});
