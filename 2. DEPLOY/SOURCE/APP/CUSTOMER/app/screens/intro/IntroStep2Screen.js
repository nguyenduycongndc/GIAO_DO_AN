import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  TouchableOpacity,
  Dimensions
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  SCREEN_ROUTER_INTRO,
  SCREEN_ROUTER_CUSTOMER,
  ASYNC_STORAGE
} from "@constant";
import theme from "@app/constants/Theme";
import { connect } from "react-redux";
import FastImage from "@app/components/FastImage";
import Loading from "@app/components/Loading";
import DialogLoading from "@app/components/DialogLoading";
import analytics from "@react-native-firebase/analytics";
import AsyncStorage from "@react-native-community/async-storage";
export class IntroStep2Screen extends Component {
  state = {
    hImg: 1,
    loading: true
  };
  render() {
    const { listIntro, index } = this.props.navigation.state.params;
    return (
      <ScreenComponent
        renderView={
          <View
            style={{
              backgroundColor: theme.colors.white,
              flex: 1,
              flexDirection: "column-reverse"
            }}
          >
            {this.renderBottom(R.strings().continue, async () => {
              analytics().logEvent("intro_step_2", {
                item: "Intro Screen",
                description: "Intro 2 success"
              });
              if (!!listIntro && listIntro.length == index + 1) {
                await AsyncStorage.setItem(ASYNC_STORAGE.INTRO, "true");
                NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.MAIN);
              } else
                NavigationUtil.push(SCREEN_ROUTER_INTRO.STEP2, {
                  listIntro,
                  index: index + 1
                });
            })}
            <FastImage
              source={
                listIntro.length > 0
                  ? { uri: listIntro[index].listImage }
                  : R.images.splash
              }
              onLoadEnd={() => this.setState({ loading: false })}
              style={{
                flex: 1
              }}
              resizeMode="cover"
            />
          </View>
        }
      />
    );
  }
  renderButton(lable) {
    return (
      <View
        style={styles.location}
        children={
          <Text style={[styles.textButton, { color: theme.colors.black }]}>
            {lable}
          </Text>
        }
      />
    );
  }
  renderBottom(lable, onPress) {
    let isCancel = R.strings().continue != lable;
    return (
      <TouchableOpacity
        onLayout={eve => {
          this.setState({
            hImg: eve.nativeEvent.layout.height
          });
        }}
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
const styles = StyleSheet.create({
  textStyle: {
    color: theme.colors.active,
    textDecorationLine: "underline"
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
  locationButton: {
    flexDirection: "row",
    alignItems: "center",
    width: "100%",
    justifyContent: "center"
  },
  location: {
    backgroundColor: theme.colors.white,
    borderWidth: 0.25,
    borderColor: theme.colors.gray,
    margin: 5,
    alignItems: "center",
    justifyContent: "center",
    color: colors.white,
    borderRadius: 15,
    width: 150,
    height: 30
  },
  bottom: {
    width: "100%"
  }
});
const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(IntroStep2Screen);
