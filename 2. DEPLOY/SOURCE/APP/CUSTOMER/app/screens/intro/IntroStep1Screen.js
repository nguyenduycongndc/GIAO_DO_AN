import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  TouchableOpacity,
  ScrollView
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_INTRO, ASYNC_STORAGE } from "@constant";
import ModalMessage from "@app/components/ModalMessage";
import callAPI from "@app/utils/CallApiHelper";
import { GetListIntro } from "@api";
import { changeLanguage } from "@action_language";
import { connect } from "react-redux";
import AsyncStorage from "@react-native-community/async-storage";
import FastImage from "@app/components/FastImage";
import analytics from "@react-native-firebase/analytics";
class IntroStep1Screen extends Component {
  state = {
    modalVisiable: false,
    index: 0,
    loading: true,
    data: {
      listIntro: [
        {
          content: "vi",
          listImage: "http://118.27.192.110/Uploads/image/appIntro/C1.PNG"
        },
        {
          content: "vi",
          listImage:
            "http://118.27.192.110/Uploads/image/appIntro/intro_2_vi.PNG"
        },
        {
          content: "vi",
          listImage:
            "http://118.27.192.110/Uploads/image/appIntro/intro_3_en.PNG"
        }
      ],
      listProlicy: [
        {
          name: "Điều khoản và điều kiện",
          type: 1,
          content: `Điều khoản chung`
        },
        {
          name: "Chính sách bảo mật",
          type: 2,
          content: "Chính sách bảo mật"
        }
      ]
    },
    hImg: 1
  };

  getData = () => {
    callAPI(GetListIntro, 1, this, res => {
      this.setState({
        data: res.result
      });
    });
  };
  componentDidMount() {
    this.getData();
  }

  toggalModal = (index = 0) =>
    this.setState({
      modalVisiable: !this.state.modalVisiable,
      index
    });
  render() {
    const { listIntro, listProlicy } = this.state.data;
    const { index } = this.state;

    return (
      <ScreenComponent
        dialogLoading={this.state.isLoading}
        renderView={
          <View
            style={{
              backgroundColor: theme.colors.white,
              flex: 1,
              flexDirection: "column-reverse"
            }}
          >
            <View
              onLayout={eve => {
                this.setState({
                  hImg: eve.nativeEvent.layout.height
                });
              }}
              style={{ flexDirection: "column-reverse" }}
              children={
                <>
                  {this.renderBottom(R.strings().continue, async () => {
                    analytics().logEvent("intro_step_1", {
                      item: "Intro Screen",
                      description: "Intro 1 success"
                    });
                    NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP2, {
                      listIntro,
                      index: 1
                    });
                  })}
                </>
              }
            />
            <FastImage
              source={
                listIntro.length > 0
                  ? { uri: listIntro[0].listImage }
                  : R.images.splash
              }
              style={{
                flexDirection: "column-reverse",
                flex: 1
              }}
              resizeMode="cover"
              children={
                <>
                  <View
                    style={{
                      alignSelf: "center",
                      backgroundColor: "rgba(225,225,225,0.9)",
                      width: "100%",
                      paddingVertical: 10
                    }}
                  >
                    <View style={styles.bottom}>
                      <Text
                        style={styles.text}
                        children={R.strings().string_intro_1}
                      />
                    </View>
                    <View style={styles.bottom}>
                      <Text
                        onPress={() => this.toggalModal(0)}
                        style={styles.textStyle}
                        children={R.strings().string_intro_2}
                      />
                      <Text style={styles.text} children={", "} />
                      <Text
                        onPress={() => this.toggalModal(1)}
                        style={styles.textStyle}
                        children={R.strings().string_intro_3}
                      />
                    </View>
                    <View style={styles.bottom}>
                      <Text
                        style={styles.text}
                        children={R.strings().string_intro_4}
                      />
                      <Text
                        style={styles.text}
                        children={R.strings().string_intro_5}
                      />
                      <Text
                        style={styles.text}
                        children={R.strings().string_intro_6}
                      />
                    </View>
                    <TouchableOpacity
                      onPress={() => {
                        const lang = this.props.lang == "vi" ? "en" : "vi";
                        AsyncStorage.setItem(
                          ASYNC_STORAGE.LANG,
                          lang,
                          this.getData
                        );
                        this.props.changeLanguage({
                          locale: lang,
                          isIntro: true
                        });
                      }}
                      style={styles.root_change_lang}
                      children={
                        <>
                          <FastImage
                            source={
                              this.props.lang == "vi"
                                ? R.images.ic_vn
                                : R.images.ic_en
                            }
                            style={styles.img_lang}
                          />
                          <Text
                            style={styles.text_lang}
                            children={" " + this.props.lang.toUpperCase()}
                          />
                        </>
                      }
                    />
                  </View>
                </>
              }
            />
            <ModalMessage
              contentView={
                <ScrollView
                  style={{ maxHeight: height * 0.6 }}
                  showsVerticalScrollIndicator={false}
                  children={
                    <Text
                      style={{
                        textAlign: "center",
                        fontFamily: R.fonts.quicksand_medium,
                        fontSize: 14,
                        paddingVertical: 15,
                        color: "black"
                      }}
                      children={listProlicy[index].content}
                    />
                  }
                />
              }
              isVisible={this.state.modalVisiable}
              title={listProlicy[index].name}
              onClose={() => this.toggalModal(0)}
            />
          </View>
        }
      />
    );
  }
  renderBottom(lable, onPress) {
    let isCancel = R.strings().continue != lable;
    return (
      <TouchableOpacity
        onPress={onPress}
        style={[styles.buttonStyle, isCancel ? styles.cancelButton : {}]}
        children={
          <Text
            style={[
              styles.textButton,
              { color: isCancel ? theme.colors.gray : theme.colors.white }
            ]}
          >
            {lable}
          </Text>
        }
      />
    );
  }
}

export default connect(
  state => ({
    lang: state.lang.lang
  }),
  {
    changeLanguage
  }
)(IntroStep1Screen);
const styles = StyleSheet.create({
  textStyle: {
    color: theme.colors.active,
    textDecorationLine: "underline",
    fontFamily: R.fonts.quicksand_medium
  },
  viewButton: {
    paddingHorizontal: 15
  },
  textRules: { textAlign: "center", padding: 40 },
  textButton: {
    color: theme.colors.white
  },
  buttonStyle: {
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: colors.primary,
    color: colors.white,
    borderColor: colors.primary,
    paddingVertical: 15,
    width: "100%",
    alignSelf: "center"
  },
  cancelButton: {
    backgroundColor: theme.colors.backgroundGray,
    borderWidth: 0.25,
    borderColor: theme.colors.gray
  },
  bottom: {
    flexDirection: "row",
    justifyContent: "center",
    alignItems: "center"
  },
  text: {
    fontFamily: R.fonts.quicksand_medium
  },
  root_change_lang: {
    alignSelf: "center",
    borderRadius: 25,
    borderWidth: 0.5,
    flexDirection: "row",
    paddingHorizontal: 8,
    marginVertical: 15
  },
  img_lang: {
    height: 30,
    width: 30
  },
  text_lang: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 12,
    textAlignVertical: "center",
    alignSelf: "center"
  }
});
