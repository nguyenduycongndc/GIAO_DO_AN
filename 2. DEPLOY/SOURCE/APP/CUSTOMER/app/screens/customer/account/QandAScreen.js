import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  TextInput,
  FlatList,
  TouchableOpacity,
  ScrollView
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import Icon from "@component/Icon";
import Button from "@app/components/Button";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import HTML from "react-native-render-html";
import reactotron from "reactotron-react-native";

export default class QandAScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      error: "",
      getQA: []
    };
  }

  componentDidMount() {
    this.getQA();
  }

  getQA() {
    CallApiHelper(API.getQA, {}, this, res => {
      const arrayQA = res.result.map(value => {
        value.isOpen = false;
        return value;
      });
      this.setState({ getQA: arrayQA });
    });
  }

  render() {
    const { isLoading, error, getQA } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        isError={error}
        titleHeader={"Q&A"}
        renderView={
          <>
            <ScrollView
              style={{
                backgroundColor: theme.colors.backgroundColor,
                flex: 1,
                paddingTop: 10
              }}
            >
              {/* <FlatList
                contentContainerStyle={{ paddingVertical: 5, flexGrow: 1 }}
                data={getQA}
                keyExtractor={(item, index) => index.toString()}
                renderItem={this._renderQandA}
              /> */}
              {getQA.map((item, index) => this._renderQandA(item, index))}
            </ScrollView>
          </>
        }
      />
    );
  }

  collapseQA = (item, index) => {
    const arrQA = [...this.state.getQA];
    arrQA[index].isOpen = !arrQA[index].isOpen;
    this.setState({ getQA: arrQA });
  };

  _renderQandA = (item, index) => {
    return (
      <View style={{ marginBottom: 10 }}>
        <TouchableOpacity
          onPress={() => this.collapseQA(item, index)}
          style={{
            paddingHorizontal: 14,
            paddingVertical: 15,
            backgroundColor: theme.colors.white,
            flexDirection: "row",
            justifyContent: "space-between",
            alignItems: "center",
            borderWidth: 0.5,
            borderColor: theme.colors.gray
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              width: "90%",
              color: colors.primary
            }}
          >
            {item.question}
          </Text>
          {!item.isOpen ? (
            <Icon.Ionicons name={"ios-arrow-down"} size={20} />
          ) : (
            <Icon.Ionicons name={"ios-arrow-up"} size={20} />
          )}
        </TouchableOpacity>
        {item.isOpen && (
          <View
            style={{
              paddingHorizontal: 14,
              paddingVertical: 15,
              backgroundColor: theme.colors.white,
              flexDirection: "row",
              justifyContent: "space-between",
              alignItems: "center",
              borderWidth: 0.5,
              borderColor: theme.colors.gray
            }}
          >
            {/* <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 14,
                textAlign: "left",
                flex: 1,
                width: "90%",
                marginLeft: 15
              }}
            >
              {item.answer}
            </Text> */}
            <HTML html={item.answer} containerStyle={{ marginLeft: 15 }} />
          </View>
        )}
      </View>
    );
  };
}

const styles = StyleSheet.create({});
