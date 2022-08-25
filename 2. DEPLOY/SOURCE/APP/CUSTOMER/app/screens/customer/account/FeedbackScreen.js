import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  TextInput,
  FlatList,
  TouchableOpacity
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import Icon from "@component/Icon";
import Button from "@app/components/Button";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { showMessages, showConfirm } from "@app/components/Alert";
import NavigationUtil from "@app/navigation/NavigationUtil";
import reactotron from "reactotron-react-native";

export default class FeedbackScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      error: "",
      note: ""
    };
  }

  requestAddCar() {
    CallApiHelper(API.requestAddCar, this.state.note, this, res => {
      showMessages(R.strings().notif_tab_cus, res.message);
      NavigationUtil.goBack();
    });
  }

  render() {
    const { isLoading, error } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        isError={error}
        titleHeader={R.strings().feedback}
        renderView={
          <>
            <View
              style={{
                flex: 1,
                paddingTop: 20,
                paddingHorizontal: 15
              }}
            >
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 14,
                  paddingBottom: 10,
                  paddingHorizontal: 5
                }}
              >
                {R.strings().car_info}
              </Text>
              <TextInput
                style={styles.textArea}
                placeholderTextColor={colors.gray}
                placeholder={R.strings().enter_car_info}
                numberOfLines={10}
                multiline={true}
                maxLength={256}
                onChangeText={text => this.setState({ note: text })}
              />
              <View style={{ alignItems: "center" }}>
                <Button
                  uppercase
                  action={() => {
                    if (this.state.note == "") {
                      showMessages(
                        R.strings().notif_tab_cus,
                        R.strings().please_enter_your_request
                      );
                    } else this.requestAddCar();
                  }}
                  title={R.strings().feedback}
                  colorText={theme.colors.white}
                  backgroundColor={theme.colors.primary}
                  width="80%"
                  buttonStyle={{ marginVertical: 30 }}
                />
              </View>
            </View>
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  textArea: {
    padding: 15,
    backgroundColor: theme.colors.backgroundInput,
    borderRadius: 5,
    width: "100%",
    height: 157,
    textAlignVertical: "top",
    color: "black"
  }
});
