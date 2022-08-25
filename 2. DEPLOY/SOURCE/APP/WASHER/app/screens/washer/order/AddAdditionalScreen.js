import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import { RefreshControl } from "react-native";
import Empty from "@app/components/Empty";
import R from "@app/assets/R";
import callAPI from "@app/utils/CallApiHelper";
import { AdditionServiceExtra, GetListService } from "@app/constants/Api";
import { colors } from "@app/constants/Theme";
import { CheckBox } from "react-native-elements";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";

class AddAdditionalScreen extends Component {
  state = {
    data: [],
    check: []
  };
  getData = () => {
    const orderServiceID = this.props.navigation.state.params;
    callAPI(GetListService, orderServiceID, this, res => {
      this.setState({
        data: res.result.listInput
      });
    });
  };

  onCheck = id => {
    const index = this.state.check.indexOf(id);
    if (index == -1) this.state.check.push(id);
    else this.state.check.splice(index, 1);
    this.forceUpdate();
  };

  componentWillMount() {
    this.getData();
  }
  render() {
    const orderServiceID = this.props.navigation.state.params;
    const additionService = this.state.check;
    return (
      <ScreenComponent
        titleHeader="Thêm dịch vụ phụ"
        back
        isLoading={this.state?.isLoading}
        renderView={
          <>
            <FlatList
              showsVerticalScrollIndicator={false}
              refreshControl={<RefreshControl onRefresh={this.getData} />}
              ListEmptyComponent={
                <Empty description={R.strings().notify_emplty} />
              }
              ListFooterComponent={
                this.state.data.length > 0 && (
                  <TouchableOpacity
                    onPress={() => {
                      callAPI(
                        AdditionServiceExtra,
                        {
                          orderServiceID,
                          additionService
                        },
                        this,
                        res => {
                          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER);
                        }
                      );
                    }}
                    children={
                      <Text
                        style={styles.text_add_service}
                        children="Thêm dịch vụ"
                      />
                    }
                  />
                )
              }
              keyExtractor={(item, index) => index.toString()}
              data={this.state.data}
              renderItem={({ item, index }) => (
                <TouchableOpacity
                  onPress={() => this.onCheck(item.serviceID)}
                  style={styles.root_item}
                >
                  <CheckBox
                    checked={this.state.check.includes(item.serviceID)}
                    onPress={() => this.onCheck(item.serviceID)}
                    onIconPress={() => this.onCheck(item.serviceID)}
                  />
                  <View
                    style={{ alignSelf: "center" }}
                    children={
                      <>
                        <Text
                          style={styles.text_cate_name}
                          children={item.cateName}
                        />
                        <Text
                          style={styles.text_description}
                          children={item.description}
                        />
                      </>
                    }
                  />
                </TouchableOpacity>
              )}
            />
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AddAdditionalScreen);
const styles = StyleSheet.create({
  text_description: {
    fontFamily: R.fonts.quicksand_regular,
    fontSize: 14,
    margin: 5,
    marginHorizontal: 10
  },
  text_cate_name: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    color: colors.primary,
    margin: 5
  },
  root_item: {
    backgroundColor: colors.white,
    margin: 5,
    borderRadius: 5,
    flexDirection: "row"
  },
  text_add_service: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    color: colors.white,
    backgroundColor: colors.primary,
    textAlign: "center",
    margin: 5,
    padding: 10,
    borderRadius: 5,
    overflow: "hidden"
  }
});
