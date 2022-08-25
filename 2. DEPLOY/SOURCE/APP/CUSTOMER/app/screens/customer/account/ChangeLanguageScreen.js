import ScreenComponent from "@app/components/ScreenComponent";
import R from "@R";
import theme, { colors } from "@app/constants/Theme";
import AsyncStorage from "@react-native-community/async-storage";
import React, { Component } from "react";
import { Text, TouchableOpacity, StyleSheet, View, Image } from "react-native";
import { ASYNC_STORAGE } from "@constant";
import { changeLanguage } from "@action_language";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import FastImage from "@app/components/FastImage";
export class ChangeLanguageScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      lang: ""
    };
  }
  componentDidMount() {
    this._checkLang();
  }

  async _checkLang() {
    const lang = await AsyncStorage.getItem(ASYNC_STORAGE.LANG);
    this.setState({
      lang: lang
    });
    // return lang.toString();
  }
  changeLanguage = lang => {
    AsyncStorage.setItem(ASYNC_STORAGE.LANG, lang);
    this.props.changeLanguage({ locale: lang, isIntro: false });
  };
  render() {
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().language}
        renderView={
          <>
            <View style={styles.view}>
              <TouchableOpacity
                onPress={() => {
                  this.changeLanguage("vi");
                }}
                style={styles.lang}
              >
                {this.state.lang == "vi" ? (
                  <View style={styles.circle}>
                    <View style={styles.checkedCircle} />
                  </View>
                ) : (
                  <View style={{ width: 17 }} />
                )}
                <FastImage
                  source={R.images.VietnamFlag}
                  style={{ width: 24.5, height: 17, marginHorizontal: 9 }}
                />
                <Text>Tiếng Việt</Text>
              </TouchableOpacity>
              <TouchableOpacity
                onPress={() => {
                  this.changeLanguage("en");
                }}
                style={styles.lang}
              >
                {this.state.lang == "en" ? (
                  <View style={styles.circle}>
                    <View style={styles.checkedCircle} />
                  </View>
                ) : (
                  <View style={{ width: 17 }} />
                )}
                <FastImage
                  source={R.images.EnglandFlag}
                  style={{ width: 24.5, height: 17, marginHorizontal: 9 }}
                />
                <Text>English</Text>
              </TouchableOpacity>
            </View>
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  view: {
    backgroundColor: theme.colors.backgroundColor,
    flex: 1
  },
  lang: {
    paddingVertical: 19,
    paddingHorizontal: 9,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "flex-start",
    borderBottomColor: theme.colors.gray,
    borderBottomWidth: 0.5
  },
  circle: {
    height: 17,
    width: 17,
    borderRadius: 10,
    borderWidth: 0.5,
    borderColor: theme.colors.gray,
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: theme.colors.white
  },

  checkedCircle: {
    width: 11,
    height: 11,
    borderRadius: 7,
    backgroundColor: theme.colors.primary
  }
});

const mapStateToProps = state => ({
  lang: state.lang
});

const mapDispatchToProps = {
  changeLanguage
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ChangeLanguageScreen);
