import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  TouchableOpacity,
  ScrollView
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER, ASYNC_STORAGE } from "@constant";
import theme from "@app/constants/Theme";
import AsyncStorage from "@react-native-community/async-storage";
import FastImage from "@app/components/FastImage";
import Loading from "@app/components/Loading";
import DialogLoading from "@app/components/DialogLoading";
import analytics from '@react-native-firebase/analytics';
export class IntroStep3Screen extends Component {
  state = {
    hImg: 1,
    loading: true
  };
  render() {
    const listIntro = this.props.navigation.state.params;
    const { hImg, loading } = this.state;

    return (
      <ScreenComponent
        renderView={
          <View style={styles.container}>
            {this.renderBottom(R.strings().continue, async () => {
              await AsyncStorage.setItem(ASYNC_STORAGE.INTRO, "true");
              analytics().logEvent('intro_step_3', {
                item: 'Intro Screen',
                description: 'Intro 3 success'
              })
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.MAIN);
            })}
            <FastImage
              source={
                listIntro.length > 0
                  ? { uri: listIntro[2].listImage }
                  : R.images.splash
              }
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
  container: {
    backgroundColor: theme.colors.white,
    flex: 1,
    // padding: 10,
    flexDirection: "column-reverse"
  },
  textFont: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    alignItems: "center",
    marginBottom: 20
  },
  textStyle: {
    color: theme.colors.active,
    textDecorationLine: "underline"
  },
  viewButton: {
    paddingHorizontal: 15,
    backgroundColor: theme.colors.white
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
    width: theme.dimension.width,
    position: "absolute",
    paddingBottom: 40,
    paddingTop: 20,
    bottom: 0
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(IntroStep3Screen);
