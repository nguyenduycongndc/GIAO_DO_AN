import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  RefreshControl,
  TouchableOpacity,
  StyleSheet,
  Image,
  TextInput
} from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import {
  getOrderServiceDetail,
  changeStatusOrder,
  GetContentReason
} from "@api";
import reactotron from "reactotron-react-native";
import theme from "@app/constants/Theme";
import {
  ORDER_STATUS,
  TYPE_ORDER_OF,
  TYPE_LIST_REASON_CANCEL
} from "@constant";
import {
  navigateTab,
  getListOrderPendingService,
  getListOrderService,
  getListOrderUpcommingService
} from "@app/redux/actions";
import {
  InfoCustomer,
  InfoCar,
  InfoWasher,
  InfoService,
  InfoPayment,
  InfoContact
} from "../component/OrderDetailItem";
import NavigationUtil from "@app/navigation/NavigationUtil";
import Button from "@app/components/Button";
import StepWashingImg from "@app/components/StepWashingImg";
import ModalAlert from "@app/components/ModalAlert";
import { showMessages, showConfirm } from "@app/components/Alert";

export class OrderDetailScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: true,
      orderID: this.props?.navigation?.getParam("orderID", {}),
      data: {},
      error: null,
      dialogLoading: false,
      listReason: [],
      reason: "",
      isVisible: false,
      isSendRequest: false
    };
  }

  componentDidMount() {
    this.getData();
  }

  getData = async () => {
    try {
      this.setState(
        {
          ...this.state,
          isLoading: true
        },
        async () => {
          const res = await getOrderServiceDetail({
            orderServiceID: this.state.orderID
          });
          const listReason = await GetContentReason(TYPE_LIST_REASON_CANCEL);
          this.setState({
            ...this.state,
            isLoading: false,
            data: res.result,
            listReason: listReason.result
          });
        }
      );
    } catch (error) {
      this.setState({
        ...this.state,
        isLoading: false,
        error: error
      });
    }
  };

  toggleModal = () => {
    this.setState({ isVisible: !this.state.isVisible });
  };

  renderModalCancel = () => {
    const { listReason, reason, isVisible } = this.state;
    return (
      <ModalAlert
        contentView={
          <>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 12,
                paddingVertical: 10
              }}
              children={R.strings().message_warning_cancel_order}
            />
            {listReason.map(e => (
              <TouchableOpacity
                onPress={() => {
                  this.setState({ reason: e });
                }}
                children={<Text style={styles.item_note} children={e} />}
              />
            ))}
            <TextInput
              placeholder={R.strings().require_note_cancel}
              style={styles.text_input_note_cancel}
              multiline
              value={reason}
              onChangeText={text => this.setState({ reason: text })}
              scrollEnabled={false}
              maxLength={300}
            />
          </>
        }
        textSubmit={R.strings().cancel_order}
        onSubmit={() => {
          this.setState({ isSendRequest: true, isVisible: false });
        }}
        onModalHide={() => {
          if (this.state.isSendRequest)
            this.setState({ dialogLoading: true }, this.onCancelOrder);
        }}
        validSubmit={reason.trim().length > 0}
        onClose={this.toggleModal}
        isVisible={isVisible}
      />
    );
  };

  onCancelOrder = async () => {
    try {
      const { data, reason } = this.state;
      const res = await changeStatusOrder({
        orderServiceID: data.orderServiceID,
        status: ORDER_STATUS.CANCEL_BOOKING_NOW,
        reason: reason
      });
      if (data.status == ORDER_STATUS.CONFIRMED) {
        this.props.getListOrderUpcommingService({
          status: ORDER_STATUS.CONFIRMED,
          page: 1
        });
      } else
        this.props.getListOrderPendingService({
          status: ORDER_STATUS.PENDING,
          page: 1
        });
      this.setState({ dialogLoading: false, isSendRequest: false }, () => {
        showMessages(
          R.strings().notice,
          R.strings().cancel_order_success,
          NavigationUtil.goBack
        );
      });
    } catch (error) {
      this.setState({ dialogLoading: false, isSendRequest: false });
    }
  };

  render() {
    const { isLoading, error, data, dialogLoading } = this.state;
    return (
      <ScreenComponent
        isLoading={isLoading}
        isError={error != null}
        reload={this.getData}
        back={true}
        titleHeader={R.strings().transaction_detail}
        dialogLoading={dialogLoading}
        renderView={
          !isLoading &&
          error == null && (
            <View
              style={{
                flex: 1
              }}
            >
              <ScrollView
                style={{
                  flex: 1,
                  backgroundColor: theme.colors.backgroundColor
                }}
                refreshControl={
                  <RefreshControl
                    refreshing={isLoading}
                    onRefresh={() => this.getData()}
                  />
                }
              >
                <InfoCustomer item={data} />
                {data.status == ORDER_STATUS.CONFIRMED && (
                  <InfoContact item={data} />
                )}
                {data.status != ORDER_STATUS.PENDING &&
                  data.status != ORDER_STATUS.PENDING_ADMIN &&
                  data.status != ORDER_STATUS.PENDING_ALL &&
                  data.status != ORDER_STATUS.REJECTED && (
                    <InfoWasher item={data} />
                  )}
                <InfoCar item={data} />
                <InfoService item={data} />
                <InfoPayment item={data} />
                {data.status === ORDER_STATUS.COMPLETED && (
                  <View>
                    <Text style={styles.car_status}>
                      {R.strings().car_status}
                    </Text>
                    <StepWashingImg
                      data={data.listImageRequire.map(value => value)}
                    />
                  </View>
                )}
              </ScrollView>
              {this._buttomFunc(data)}
              {this.renderModalCancel()}
            </View>
          )
        }
      />
    );
  }

  _buttomFunc = data => {
    switch (data.status) {
      // nhan order hoac tu choi
      case ORDER_STATUS.PENDING:
      case ORDER_STATUS.PENDING_ADMIN:
      case ORDER_STATUS.PENDING_ALL:
        return (
          <View
            style={{
              flexDirection: "row",
              paddingHorizontal: 10,
              backgroundColor: theme.colors.white
            }}
          >
            {data.isFirstWasher == TYPE_ORDER_OF.MINE && (
              <TouchableOpacity
                style={[
                  styles.action_button,
                  {
                    backgroundColor: theme.colors.red
                  }
                ]}
                onPress={() => {
                  showConfirm(
                    R.strings().notice,
                    R.strings().mess_confirm_cancel_order,
                    this.toggleModal
                  );
                  // NavigationUtil.goBack();
                }}
              >
                <Text style={styles.start_button_text}>
                  {R.strings().reject}
                </Text>
              </TouchableOpacity>
            )}

            <TouchableOpacity
              style={styles.action_button}
              onPress={() => {
                showConfirm(
                  R.strings().notice,
                  R.strings().confirm_accept_booking,
                  () => this._acceptOrder(data)
                );
              }}
            >
              <Text style={styles.start_button_text}>{R.strings().accept}</Text>
            </TouchableOpacity>
          </View>
        );
      case ORDER_STATUS.CONFIRMED:
        return (
          <View
            style={{
              flexDirection: "row",
              paddingHorizontal: 10,
              backgroundColor: theme.colors.white
            }}
          >
            {/* {data.isBookingNow == 1 && (
              <TouchableOpacity
                style={[
                  styles.action_button,
                  {
                    backgroundColor: theme.colors.red
                  }
                ]}
                onPress={() => {
                  showConfirm(
                    R.strings().notice,
                    R.strings().mess_confirm_cancel_order,
                    this.toggleModal
                  );
                }}
              >
                <Text style={styles.start_button_text}>
                  {R.strings().reject}
                </Text>
              </TouchableOpacity>
            )} */}
            <TouchableOpacity
              style={styles.action_button}
              onPress={() => {
                showConfirm(
                  R.strings().notice,
                  R.strings().confirm_start_washing,
                  () => this._startWashing(data)
                );
              }}
            >
              <Text style={styles.start_button_text}>{R.strings().start}</Text>
            </TouchableOpacity>
          </View>
        );
      default:
        return <View />;
    }
  };

  _washingImages = async data => {
    // return <View></View>
  };

  _acceptOrder = async data => {
    this.setState(
      {
        ...this.state,
        dialogLoading: true
      },
      async () => {
        try {
          const res = await changeStatusOrder({
            orderServiceID: data.orderServiceID,
            status: ORDER_STATUS.CONFIRMED
          });
          this.props.getListOrderPendingService({
            status: ORDER_STATUS.PENDING,
            page: 1
          });
          this.props.getListOrderUpcommingService({
            status: ORDER_STATUS.CONFIRMED,
            page: 1
          });
          this.setState({
            ...this.state,
            dialogLoading: false
          });
          NavigationUtil.goBack();
          this.props.navigateTab(2);
        } catch (error) {
          console.log("error", error);
          this.setState({
            ...this.state,
            dialogLoading: false
          });
        }
      }
    );
  };

  _startWashing = async data => {
    this.setState(
      {
        ...this.state,
        dialogLoading: true
      },
      async () => {
        try {
          const res = await changeStatusOrder({
            orderServiceID: data.orderServiceID,
            status: ORDER_STATUS.START
          });
          this.props.getListOrderService({ status: ORDER_STATUS.WASHING });
          this.props.getListOrderUpcommingService({
            status: ORDER_STATUS.CONFIRMED,
            page: 1
          });
          this.setState({
            ...this.state,
            dialogLoading: false
          });
          NavigationUtil.goBack();
          this.props.navigateTab(0);
        } catch (error) {
          console.log("error", error);
          this.setState({
            ...this.state,
            dialogLoading: false
          });
        }
      }
    );
  };
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {
  navigateTab,
  getListOrderService,
  getListOrderPendingService,
  getListOrderUpcommingService
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderDetailScreen);

const styles = StyleSheet.create({
  car_status: {
    marginVertical: 15,
    marginLeft: 10,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 18,
    color: theme.colors.primaryDark
  },
  action_button: {
    flex: 1,
    paddingVertical: 12,
    margin: 5,
    borderRadius: 5,
    textAlign: "center",
    backgroundColor: theme.colors.primary,
    justifyContent: "center",
    alignItems: "center"
  },
  start_button: {
    // flex: 1,
    paddingVertical: 12,
    margin: 5,
    borderRadius: 5,
    textAlign: "center",
    backgroundColor: theme.colors.primary,
    justifyContent: "center",
    alignItems: "center"
  },
  start_button_text: {
    textAlign: "center",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: theme.colors.white,
    textTransform: "uppercase"
  },
  item_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 10,
    marginBottom: 10,
    borderRadius: 10,
    borderWidth: 0.5,
    color: theme.colors.primary
  },
  text_input_note_cancel: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    borderRadius: 5,
    borderWidth: 0.5,
    padding: 10,
    textAlignVertical: "top",
    height: 100,
    color: "black"
  }
});
