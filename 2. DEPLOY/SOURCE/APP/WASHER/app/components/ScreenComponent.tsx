import React, { Component } from "react";
import {
  Text,
  View,
  SafeAreaView,
  ScrollView,
  RefreshControl,
  ViewProps,
  StatusBar,
  KeyboardAvoidingView,
  Platform,
  ViewStyle,
  Keyboard
} from "react-native";
import RNHeader from "./RNHeader";
import Error from "./Error";
import Loading from "./Loading";
import reactotron from "reactotron-react-native";
import theme from "@app/constants/Theme";
import { StyleProp } from "react-native";
import { connect } from "react-redux";
import { BarIndicator } from "react-native-indicators";
interface Props {
  /**
   * View hiển thị
   */
  renderView: JSX.Element;
  /**
   * State hiện thị màn hình Loading
   */
  isLoading?: boolean;
  /**
   * State hiện thị màn hình Lỗi
   */
  isError?: object | boolean;
  /**
   * Có nút back
   */
  back?: boolean;
  /**
   * View nút phải
   */
  rightComponent?: JSX.Element;
  /**
   * View nút trái
   */
  leftComponent?: JSX.Element;
  /**
   * Title thanh header
   */
  titleHeader: string;

  reload?: () => void;

  /**
   * loading dialog
   */
  dialogLoading?: boolean;
  isSafeArea?: boolean;
}

export default class ScreenComponent extends Component<Props, ViewProps> {
  constructor(props) {
    super(props);
  }
  renderBody() {
    const {
      isLoading = false,
      isError = false,
      reload,
      renderView
    } = this.props;
    if (isLoading) return <Loading />;
    if (isError) return <Error reload={reload} />;
    return renderView;
  }

  render() {
    const {
      titleHeader,
      rightComponent,
      leftComponent,
      back,
      dialogLoading,
      isSafeArea = true
    } = this.props;
    return (
      <View
        style={{ flex: 1, backgroundColor: theme.colors.backgroundColor }}
        onTouchStart={Keyboard.dismiss}
      >
        {titleHeader && (
          <RNHeader
            titleHeader={titleHeader}
            back={back}
            rightComponent={rightComponent}
            leftComponent={leftComponent}
          />
        )}
        <StatusBar translucent />
        {isSafeArea ? (
          <SafeAreaView style={{ flex: 1 }}>{this.renderBody()}</SafeAreaView>
        ) : (
          <View style={{ flex: 1 }}>{this.renderBody()}</View>
        )}
        {dialogLoading && (
          <View
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              right: 0,
              bottom: 0,
              justifyContent: "center",
              alignItems: "center",
              backgroundColor: "rgba(0, 0, 0, 0.6)",
              elevation: Platform.OS == "android" ? 4 : 0
            }}
          >
            <View
              style={{
                height: 140,
                backgroundColor: "white",
                padding: 30,
                borderRadius: 10
              }}
            >
              <BarIndicator color={theme.colors.indicator} />
              <Text
                style={{
                  color: theme.colors.indicator
                }}
              >
                Vui lòng đợi..
              </Text>
            </View>
          </View>
        )}
      </View>
    );
  }
}
