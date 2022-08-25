import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  Image,
  TextInput
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import InfoItem from "@app/components/InfoItem";
import R from "@app/assets/R";
import theme from "@theme";
import I18n from "@i18";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import {
  RowAvatarInfo,
  RowLableInfo,
  WasherInfo
} from "@app/components/FormRow";

export class FeedbackScreen extends Component {
  static propTypes = {
    prop: PropTypes
  };

  render() {
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().order_tab_cus}
        renderView={
          <>
            <ScrollView
              style={{ backgroundColor: theme.colors.backgroundColor }}
            >
              <View
                style={{ backgroundColor: theme.colors.defaultBg, padding: 10 }}
              >
                <Text style={styles.lable}>Lý do huỷ lịch</Text>
                {this.renderReason("Tôi có việc đột xuất")}
                {this.renderReason("Tôi không liên lạc được với khách hàng.")}
                {this.renderReason("Khách hàng yêu cầu tôi hủy giao dịch")}
                {this.renderReason(
                  "Tôi không thể đến kịp thời gian khách hàng yêu cầu"
                )}
                {this.renderReason("Khách hàng có hành động bạo lực")}
                {this.renderReason("Lý do khác")}
                <TextInput
                  style={styles.textInput}
                  placeholder="Nhập lý do..."
                  multiline={true}
                />
              </View>
            </ScrollView>
          </>
        }
      />
    );
  }
  renderReason(title) {
    return (
      <TouchableOpacity style={styles.titleBox}>
        <Text style={styles.title}>{title}</Text>
      </TouchableOpacity>
    );
  }
}
const styles = StyleSheet.create({
  titleBox: {
    width: "100%",
    borderRadius: 5,
    marginVertical: 2,
    fontSize: 14,
    padding: 10,
    backgroundColor: theme.colors.white,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 1
    },
    shadowOpacity: 0.22,
    shadowRadius: 2.22,
    elevation: 3,
    alignItems: "center"
  },
  title: {
    textAlign: "center",
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.textColor,
    fontSize: 14
  },
  textInput: {
    width: "100%",
    backgroundColor: theme.colors.white,
    borderRadius: 5,
    borderWidth: 0.25,
    marginVertical: 10,
    fontFamily: R.fonts.quicksand_light,
    color: theme.colors.black,
    fontSize: 14,
    padding: 10,
    minHeight: 100,
    color: "black"
  },
  lable: {
    fontFamily: R.fonts.quicksand_bold,
    color: theme.colors.textColor,
    fontSize: 14,
    marginBottom: 10
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(FeedbackScreen);
