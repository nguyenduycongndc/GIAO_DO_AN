import React, { Component } from "react";
import {
  View,
  SafeAreaView,
  ViewProps,
  StatusBar,
  Keyboard,
  Platform,
  Text,
  TouchableWithoutFeedback
} from "react-native";
import RNHeader from "./RNHeader";
import R from "@app/assets/R";
import Error from "./Error";
import Loading from "./Loading";
import theme, { colors } from "@app/constants/Theme";
import RequsetLoginScreen from "@app/components/RequsetLoginScreen";
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
  titleHeader?: string;

  isLogin?: boolean;

  reload?: () => void;

  onBack?: () => void;

  header?: JSX.Element;

  dialogLoading?: boolean;
  isSafeAre?: boolean;
}

export default class ScreenComponent extends Component<Props, ViewProps> {
  constructor(props) {
    super(props);
  }
  renderBody() {
    const { isLoading, isError, reload, renderView, isLogin } = this.props;
    if (isLoading) return <Loading />;
    if (isError) return <Error reload={reload} />;
    if (isLogin) return <RequsetLoginScreen />;
    return renderView;
  }

  render() {
    const {
      titleHeader,
      rightComponent,
      leftComponent,
      back,
      dialogLoading,
      onBack,
      header,
      isSafeAre = true
    } = this.props;
    return (
      <View
        style={{ flex: 1 }}
        onTouchStart={() => Keyboard.dismiss()}
        // onPress={Keyboard.dismiss}
        children={
          <View style={{ flex: 1, backgroundColor: colors.backgroundColor }}>
            {titleHeader && (
              <RNHeader
                titleHeader={titleHeader}
                back={back}
                onBack={onBack}
                rightComponent={rightComponent}
                leftComponent={leftComponent}
              />
            )}
            {!!header && (
              <View
                style={{
                  paddingTop: Platform.OS == "ios" ? 30 : 10,
                  backgroundColor: colors.primary
                }}
                children={header}
              />
            )}
            <StatusBar translucent />
            {isSafeAre ? (
              <SafeAreaView style={{ flex: 1 }}>
                <View
                  style={{ flex: 1, backgroundColor: "white" }}
                  children={this.renderBody()}
                />
              </SafeAreaView>
            ) : (
              <View
                style={{ flex: 1, backgroundColor: "white" }}
                children={this.renderBody()}
              />
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
                    {R.strings().loading}
                  </Text>
                </View>
              </View>
            )}
          </View>
        }
      />
    );
  }
}
