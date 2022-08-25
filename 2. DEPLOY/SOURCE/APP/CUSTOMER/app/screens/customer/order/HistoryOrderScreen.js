import R from "@app/assets/R";
import InfoItem from "@app/components/InfoItem";
import { BOOKING } from "@constant";
import theme from "@theme";
import React, { Component } from "react";
import {
  FlatList,
  StyleSheet,
  Text,
  View,
  RefreshControl,
  Platform
} from "react-native";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER, ORDER_TYPE, REDUCER_CUSTOM } from "@constant";
import { getOrder, setState } from "@app/redux/actions";
import ScreenComponent from "@app/components/ScreenComponent";
import Dropdown from "@component/ModalDropdown";
import ModalAlertRating from "@app/components/ModalAlertRating";
import Empty from "@app/components/Empty";
import { GET_ORDER_HISTORY } from "@app/redux/actions/type";
import reactotron from "reactotron-react-native";
const initState = {
  isModalVisible: false,
  status: ORDER_TYPE.ORDER_HISTORY,
  placeID: "",
  fromDate: "",
  toDate: "",
  distance: "",
  page: 1,
  star: 3,
  itemRate: null
};
export class HistoryOrderScreen extends Component {
  constructor(props) {
    super(props);
    this.state = { ...initState };
    props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
      resetData: this.resetData
    });
  }

  componentDidMount = () => this.getData();

  resetData = () => {
    this.setState({ page: 1 }, this.getData);
  };

  getData = () => this.props.getOrder(GET_ORDER_HISTORY, this.state);
  renderHeaderLable = () => {
    const orderType = [
      null,
      ORDER_TYPE.ORDER_STATUS_COMPLETE,
      ORDER_TYPE.ORDER_STATUS_CANCEL
    ];
    return (
      <Dropdown
        defaultValue={R.strings().status}
        data={[R.strings().all, R.strings().complete, R.strings().cancel]}
        style={styles.root_dropdown}
        dropDownStyle={styles.dropDownStyle}
        renderRow={text => (
          <Text style={styles.text_dropdown} children={text} />
        )}
        onSelect={(i, val) => {
          var state = this.state;
          state["status"] = orderType[i];
          state["page"] = 1;
          this.setState(state, this.getData);
        }}
      />
    );
  };
  toggleModal = item => {
    this.setState({
      isModalVisible: !this.state.isModalVisible,
      star: 3,
      itemRate: item || null
    });
  };
  submitModal = () => {
    this.toggleModal();
  };
  renderModal = () => {
    return (
      <ModalAlertRating
        {...this.props.orderHistory}
        item={this.state.itemRate}
        isModalVisible={this.state.isModalVisible}
        // submitModal={this.submitModal}
        toggleModal={() => this.toggleModal(this.state.itemRate)}
      />
    );
  };
  renderEmpty = () => (
    <Empty
      marginTop={height / 6}
      sourceImage={R.images.empty_procressing_order}
      description={R.strings().no_order_in_history}
    />
  );

  renderList = () => {
    const { isLoading, data, isLastPage, isLoadMore } = this.props.orderHistory;
    return (
      <FlatList
        refreshControl={
          <RefreshControl refreshing={isLoading} onRefresh={this.resetData} />
        }
        removeClippedSubviews={true}
        onMomentumScrollBegin={() =>
          (this.onEndReachedCalledDuringMomentum = false)
        }
        showsHorizontalScrollIndicator={false}
        onEndReachedThreshold={0.5}
        onEndReached={() => {
          if (!this.onEndReachedCalledDuringMomentum) {
            if (!isLastPage && !isLoadMore)
              this.setState({ page: this.state.page + 1 }, this.getData);
            this.onEndReachedCalledDuringMomentum = true;
          }
        }}
        ListEmptyComponent={this.renderEmpty}
        contentContainerStyle={{ paddingVertical: 5 }}
        data={data}
        keyExtractor={(item, index) => index.toString()}
        renderItem={({ item, index }) => (
          <InfoItem
            userInfo={item}
            onPress={() => {
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, item);
            }}
            type={BOOKING.HISTORY}
            onPressRate={this.toggleModal}
            onPressReset={() => {
              const {
                additionService,
                mainService,
                carID,
                isInDoor,
                comboID,
                note = "",
                reasonNote = ""
              } = item;
              this.props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
                orderService: {
                  reasonNote,
                  additionService,
                  mainService,
                  carID,
                  isInDoor,
                  comboID,
                  isBookingNow: 0,
                  note,
                  agentCode: "",
                  BookingDateInput: "",
                  couponCode: "",
                  PaymentType: "",
                  UsePoint: 0
                },
                reOrder: true
              });
              this.props.toggleModal();
            }}
          />
        )}
      />
    );
  };
  render() {
    const { isLoading, error } = this.props.orderHistory;
    return (
      <>
        {this.renderHeaderLable()}
        <ScreenComponent
          isError={error}
          reload={this.getData}
          isLoading={isLoading}
          renderView={
            <View style={styles.view}>
              {this.renderList()}
              {this.renderModal()}
            </View>
          }
        />
      </>
    );
  }
}

const styles = StyleSheet.create({
  agentName_text: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 22,
    color: theme.colors.primary,
    paddingVertical: 5
  },
  agentID_text: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: theme.colors.textColor,
    paddingVertical: 5
  },
  view: {
    flex: 1,
    backgroundColor: theme.colors.backgroundColor
  },
  modal: {
    // marginHorizontal:14,
    paddingHorizontal: 14,
    marginTop: 20,
    backgroundColor: theme.colors.white,
    alignItems: "center",
    paddingTop: 26,
    borderRadius: 10
  },
  textInput: {
    borderWidth: 0.5,
    borderColor: theme.colors.black,
    borderRadius: 5,
    padding: 5,
    width: "100%",
    marginTop: 15,
    marginBottom: 13
  },
  button: {
    justifyContent: "center",
    alignItems: "center",
    paddingHorizontal: 20,
    paddingVertical: 5
  },
  text_dropdown: {
    textAlignVertical: "center",
    paddingHorizontal: 10,
    paddingVertical: 12,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  root_dropdown: {
    alignSelf: "flex-start",
    padding: 10,
    width: "100%",
    backgroundColor: theme.colors.backgroundColor
  },
  dropDownStyle: {
    width: "100%",
    marginTop: Platform.OS != "ios" ? -25 : 0
    // borderRadius: 5,
    // marginHorizontal: 5
  }
});

const mapStateToProps = state => ({
  orderHistory: state[REDUCER_CUSTOM.ORDER_HISTORY],
  toggleModal:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING].toggleModal
});

const mapDispatchToProps = {
  getOrder,
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(HistoryOrderScreen);
