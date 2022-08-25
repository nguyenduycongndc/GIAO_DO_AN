import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  RefreshControl,
  TouchableOpacity,
  StyleSheet
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import InfoItem from "../../../components/InfoItem";
import StepWashingImg from "../../../components/StepWashingImg";
import { connect } from "react-redux";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import {
  SCREEN_ROUTER_CUSTOMER,
  ORDER_TYPE,
  REDUCER_CUSTOM,
  PAYMENT_METHOD
} from "@constant";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { getOrder, setState } from "@app/redux/actions";
import Empty from "@app/components/Empty";
import { GET_ORDER_PROCESSING } from "@app/redux/actions/type";
import reactotron from "@app/reactotron/ReactotronConfig";
import ImageScratch from "@app/components/ImageScratch";
import Detail from "./detail/Detail";
import Icon from "@component/Icon";
import Button from "@app/components/Button";

export class ProcessingOrderScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      status: ORDER_TYPE.ORDER_STATUS_WASHING,
      placeID: "",
      fromDate: "",
      toDate: "",
      distance: "",
      page: 1,
      paymentType: PAYMENT_METHOD.CASH
    };
    props.setState(SCREEN_ROUTER_CUSTOMER.PROCESSING_ORDER, {
      resetData: this.resetData
    });
  }
  _renderPayMentMethod = (type, title, action) => {
    const checkType = this.state.paymentType == type;
    return (
      <TouchableOpacity
        onPress={action}
        style={[
          styles.root_payment_select,
          { borderColor: checkType ? colors.primary : colors.grayBorder }
        ]}
      >
        {checkType && (
          <Icon.MaterialCommunityIcons
            name={"check-bold"}
            size={20}
            style={styles.icon_checked}
          />
        )}
        <Text
          style={[
            styles.title,
            {
              color: checkType ? colors.primary : colors.nameText
            }
          ]}
        >
          {title}
        </Text>
      </TouchableOpacity>
    );
  };
  resetData = () => this.getData();

  componentDidMount = () => this.getData();

  getData = () => this.props.getOrder(GET_ORDER_PROCESSING, this.state);

  renderProcessingOrder = data => (
    <>
      <InfoItem
        userInfo={data}
        onPress={() =>
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, data)
        }
      />
      <ImageScratch
        listImage={data.carNote.listImage}
        note={data.carNote.note}
      />

      <StepWashingImg data={data.listImageRequire.map(value => value)} />
      <View
        style={{
          marginVertical: 20
        }}
      />
      <Detail
        showAddition={[1, 2, 3]}
        data={data}
        showHeader={false}
        status={R.strings().processing}
        showStepWashing={false}
        renderButton={
          <>
            <View style={styles.root_footer}>
              <View style={{ flexDirection: "row", alignItems: "center" }}>
                {this._renderPayMentMethod(
                  PAYMENT_METHOD.CASH,
                  R.strings().cash,
                  () =>
                    this.setState({
                      paymentType: PAYMENT_METHOD.CASH
                    })
                )}
                {this._renderPayMentMethod(PAYMENT_METHOD.VNPAY, "VNPay", () =>
                  this.setState({
                    paymentType: PAYMENT_METHOD.VNPAY
                  })
                )}
              </View>

              <Button
                width="95%"
                title={"Xác nhận"}
                backgroundColor={colors.primary}
                colorText={colors.white}
                uppercase
                action={() => {}}
              />
            </View>
          </>
        }
      />
    </>
  );

  renderEmptyProcressing = () => (
    <Empty
      marginTop={height / 4.55}
      sourceImage={R.images.empty_procressing_order}
      description={R.strings().no_order_in_process}
    />
  );

  render() {
    const { isLoading, data, error } = this.props.orderProcessing;
    return (
      <ScreenComponent
        isLoading={isLoading}
        isError={error}
        reload={this.getData}
        renderView={
          <>
            <ScrollView
              refreshControl={
                <RefreshControl
                  refreshing={isLoading}
                  onRefresh={this.getData}
                />
              }
              style={{ backgroundColor: theme.colors.backgroundColor }}
              children={
                data.length > 0
                  ? this.renderProcessingOrder(data[0])
                  : this.renderEmptyProcressing()
              }
            />
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  orderProcessing: state[REDUCER_CUSTOM.ORDER_PROCESSING]
});

const mapDispatchToProps = {
  getOrder,
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ProcessingOrderScreen);

const styles = StyleSheet.create({
  root_footer: {
    paddingHorizontal: 0,
    paddingTop: 9,
    width: "100%",
    backgroundColor: colors.white,
    bottom: 0,
    justifyContent: "space-around",
    alignItems: "center"
  },
  root_payment_select: {
    paddingVertical: 17,
    justifyContent: "center",
    alignItems: "center",
    borderWidth: 0.5,
    width: "48%"
  },
  icon_checked: {
    color: theme.colors.primary,
    position: "absolute",
    top: 0,
    right: 0
  },
  title: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14
  }
});
