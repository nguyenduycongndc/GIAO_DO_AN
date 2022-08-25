import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  TouchableOpacity,
  Image,
  Dimensions,
  KeyboardAvoidingView,
  Platform,
  Keyboard,
  TouchableWithoutFeedback
} from "react-native";
import Modal from "react-native-modal";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import reactotron from "reactotron-react-native";

const { height, width } = Dimensions.get("window");

interface Props {
  isVisible?: boolean;
  backdrop?: boolean;
  onSubmit?: () => void;
  onClose?: () => void;
  onModalHide?: () => void;
  contentView?: React.ReactNode;
  title?: string;
  textCancel?;
  textSubmit?: string;
  validSubmit?: boolean;
}

export default class ModalAlert extends Component<Props> {
  constructor(props) {
    super(props);
  }
  renderButton() {
    const {
      onSubmit,
      onClose,
      textSubmit = R.strings().confirm,
      textCancel = R.strings().cancel,
      validSubmit = true
    } = this.props;

    return (
      <View
        style={{
          flexDirection: "row",
          marginHorizontal: 20,
          marginBottom: 4
        }}
      >
        <TouchableOpacity
          style={[styles.button, { borderRightWidth: 0.5 }]}
          onPress={onClose}
        >
          <Text style={[styles.textCancel]}>{textCancel}</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.button}
          onPress={onSubmit}
          disabled={!validSubmit}
        >
          <Text
            style={[
              styles.textSubmit,
              { color: validSubmit ? colors.primary : colors.backgroundColor }
            ]}
          >
            {textSubmit}
          </Text>
        </TouchableOpacity>
      </View>
    );
  }
  renderHeader = () => {
    const { title } = this.props;
    return (
      <Text
        style={{
          fontFamily: R.fonts.quicksand_bold,
          fontSize: 16,
          textAlign: "center"
        }}
        children={title}
      />
    );
  };

  render() {
    const {
      contentView,
      isVisible,
      backdrop,
      onClose,
      onModalHide
    } = this.props;
    return (
      <Modal
        onModalHide={() => {
          if (onModalHide) onModalHide();
        }}
        isVisible={isVisible}
        onBackdropPress={() => {
          if (backdrop) onClose();
        }}
        animationInTiming={1}
        animationOutTiming={1}
        animationIn="bounceIn"
        animationOut="bounceOut"
      >
        <KeyboardAvoidingView
          keyboardVerticalOffset={100}
          behavior={Platform.OS == "ios" ? "padding" : undefined}
          children={
            <TouchableWithoutFeedback
              onPress={Keyboard.dismiss}
              children={
                <View style={styles.contentStyle}>
                  {this.renderHeader()}
                  {contentView}
                  {this.renderButton()}
                </View>
              }
            />
          }
        />
      </Modal>
    );
  }
}

const styles = StyleSheet.create({
  contentStyle: {
    width: width * 0.9,
    backgroundColor: "white",
    borderRadius: 5,
    borderWidth: 1,
    borderColor: "#2E384D",
    alignSelf: "center",
    padding: 10
  },
  line: {
    width: width * 0.9,
    height: 0.2,
    backgroundColor: theme.colors.line
  },
  icClose: {
    width: 20,
    height: 20,
    position: "absolute",
    top: 0,
    right: 12
  },
  textTitle: {
    fontSize: 16,
    fontFamily: R.fonts.roboto_medium,
    flex: 1,
    textAlign: "center"
  },
  button: {
    flex: 1,
    borderColor: theme.colors.line,
    marginVertical: 6,
    paddingVertical: 6
  },
  textCancel: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    textAlign: "center",
    marginRight: 20,
    color: colors.gray
  },
  textSubmit: {
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.primary,
    fontSize: 16,
    textAlign: "center",
    marginLeft: 20
  }
});
