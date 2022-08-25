import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@theme";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  View,
  TouchableOpacity,
  FlatList,
  Dimensions,
  BackHandler,
  TextInput,
  Linking,
  KeyboardAvoidingView,
  Keyboard
} from "react-native";
import { connect } from "react-redux";
import Button from "@app/components/Button";
import Icon from "@component/Icon";
import { RowAvatarInfo } from "@app/components/FormRow";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  PAYMENT_METHOD,
  SCREEN_ROUTER_CUSTOMER,
  TYPE_SERVICE,
  TYPE_NAVIGATION,
  REDUCER_CUSTOM,
  UPLOAD_IMAGE_TYPE,
  TYPE_REASON,
  TYPE_DISCOUNT,
  VNPAY_MINIUM_MONEY,
  TYPE_COUPON,
  RELEASE_CHANNEL,
  CHANNEL
} from "@constant";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import NumberFormat from "react-number-format";
import { ServicePack, Promotion } from "@app/components/PaymentDetail";
import { getUserInfoAction, sendVoucher, setState } from "@action";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scrollview";
import { showConfirm, showMessages } from "@app/components/Alert";
import { formatMoney, getCurrentTimeString } from "@app/constants/Functions";
const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;
import ModalAlert from "@app/components/ModalAlert";
import callAPI from "../../../utils/CallApiHelper";
import analytics from "@react-native-firebase/analytics";
var extraServiceID = [];

var listAddServiceBooking = [];

let isSubmitNoteForMaster = false,
  isSubmitRequestService = false;
export class BookingScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const orderService = navigation.getParam("orderService");
    const listAddService = navigation.getParam("listAddService", {});
    const carInfo = props.tabBookingScreenState.carInfo;
    this.state = {
      step: 1,
      isLoading: false,
      error: "",
      listService: [],
      carInfo: carInfo,
      extraServiceIDState: [],
      paymentMethod: "",
      totalMoney: "",
      baseMoney: "",
      listAddService,
      isCheckSwitch: false,
      discountMoney: 0,
      orderService: {
        ...orderService,
        additionService: [],
        UsePoint: props.UserInfoState.data.point,
        PaymentType: listAddService.serviceID
          ? PAYMENT_METHOD.CASH
          : PAYMENT_METHOD.VNPAY
      },
      payload: {
        carID: orderService.carID,
        type: TYPE_SERVICE.ADDITION
      }
    };
    props.setState(SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE, {
      changeTime: this.changeTime,
      setDialogLoading: this.setDialogLoading
    });
  }

  setDialogLoading = (isLoad, callBack) => {
    this.setState(
      {
        isDialogLoading: isLoad
      },
      callBack
    );
  };

  componentWillUnmount = () => {
    this.props.setState(SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE, {
      changeTime: null,
      setDialogLoading: null
    });
  };

  changeTime = () => {
    this.setState({
      orderService: {
        ...this.state.orderService,
        BookingDateInput: this.props.BookingDateInput
      }
    });
  };
  handleDeepLinkingRequests = () => {
    Linking.getInitialURL()
      .then(url => {
        if (url) this.navigate(url);
      })
      .catch(error => console.log(error));
  };
  trackingEventBooking = async (status, typePayment = "VNPAY") => {
    const { data } = this.props.UserInfoState;
    var eventName = "booking_vnpay_" + status;
    analytics().logEvent(eventName, {
      id: data.memberID,
      item: "Booking " + typePayment + status,
      phone: data.phone,
      name: data.name
    });
  };
  navigate = async url => {
    const SUCCESS = "success",
      FAILED = "failed";
    try {
      if (this.state.step == 3) {
        const route = url.replace(/.*?:\/\//g, "");
        const state = route.split("/")[0];
        var id = route.split("/")[1];
        BackHandler.removeEventListener("hardwareBackPress", this.onBack);
        console.log(url);
        if (id == parseInt(id)) {
          if (state == SUCCESS) {
            this.trackingEventBooking(SUCCESS);
            NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER, {
              orderServiceID: id
            });
          } else {
            this.trackingEventBooking(FAILED);
            showMessages(
              R.strings().notif_tab_cus,
              R.strings().pay_failed,
              () => {
                Linking.removeAllListeners("url");
                this.props.navigation.dismiss();
                this.props.getUserInfoAction();
              }
            );
          }
        } else {
          if (state == SUCCESS) {
            this.trackingEventBooking(SUCCESS);
            NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER, {
              comboID: id
            });
          } else {
            this.trackingEventBooking(FAILED);
            showMessages(
              R.strings().notif_tab_cus,
              R.strings().pay_failed,
              () => {
                Linking.removeAllListeners("url");
                this.props.navigation.dismiss();
                this.props.getUserInfoAction();
              }
            );
          }
        }
      }
    } catch (error) { }
  };
  componentDidMount = () => {
    Keyboard.addListener("keyboardWillShow", event => {
      this.setState({
        isShowKeyboard: true
      });
    });
    Keyboard.addListener("keyboardWillHide", event => {
      this.setState({
        isShowKeyboard: false
      });
    });
    Linking.addEventListener("url", event => {
      this.navigate(event.url);
    });
    this.handleDeepLinkingRequests();
    this.props.getUserInfoAction();
    this.getReason();
    this.getListService();
    this.getListCouponUnworn();
  };

  componentWillUnmount() {
    BackHandler.removeEventListener("hardwareBackPress", this.onBack);
    this.props.sendVoucher(TYPE_DISCOUNT.NORMAL, "", 0);
    Linking.removeAllListeners("url");
    this.resetCouponCode();
  }

  componentWillMount() {
    BackHandler.addEventListener("hardwareBackPress", this.onBack);
    (extraServiceID = []), (listAddServiceBooking = []);
  }

  getReason = () => {
    callAPI(API.GetContentReason, TYPE_REASON.REQUEST_SERVICE, this, res =>
      this.setState({
        listRequestService: res.result
      })
    );
    callAPI(API.GetContentReason, TYPE_REASON.NOTE_FOR_MASTER, this, res =>
      this.setState({
        listNoteForMaster: res.result
      })
    );
  };

  getListService = () => {
    CallApiHelper(
      API.getListService,
      this.state.payload,
      this,
      res => {
        this.setState({
          listService: res.result.listInput
        });
        if (this.props.orderHistory.reOrder) {
          const { additionService } = this.props.orderHistory.orderService;
          res.result.listInput.forEach(item => {
            if (additionService.includes(item.serviceID)) {
              extraServiceID.push(item.serviceID);
              this.state.orderService.additionService.push(item.serviceID);
              listAddServiceBooking.push(item);
              this.setState(
                {
                  extraServiceIDState: extraServiceID,
                  step: 2
                },
                this.checkTotalMoney
              );
            }
          });

          this.props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
            reOrder: false
          });
        }
      },
      null,
      null,
      "isDialogLoading"
    );
  };

  createOrder = () => {
    const {
      noteMoreDetail,
      imagesMoreDetail
    } = this.props.tabBookingScreenState;
    const { selectLocation } = this.props;

    const body = {
      ...this.state.orderService,
      CustomerAddress: selectLocation?.name
    };
    const { data } = this.props.UserInfoState;

    CallApiHelper(API.createOrder, body, this, async res => {
      if (body.UsePoint != 0) {
        analytics().logEvent("booking_use_point", {
          id: data.memberID,
          description: "booking use point",
          phone: data.phone,
          name: data.name
        });
        this.trackingEventBooking("success", "CASH");
      }
      if (imagesMoreDetail.length > 0 && !this.props.orderHistory.reOrder) {
        const data = new FormData();
        imagesMoreDetail.forEach((value, index) => {
          data.append(`${index}`, {
            name: `image${index}`,
            type: "image/jpeg",
            uri: value.url
          });
        });
        CallApiHelper(
          API.UploadCarImageService,
          {
            orderServiceID: res.result.orderServiceID,
            type: UPLOAD_IMAGE_TYPE.PARK,
            note: noteMoreDetail,
            images: data
          },
          this
        );
      }
      if (this.state.orderService.PaymentType == PAYMENT_METHOD.VNPAY) {
        CallApiHelper(
          API.GetUrlVNPay,
          res.result.transactionID,
          this,
          res => Linking.openURL(res.result),
          null,
          () => this.setState({ step: 3 }, this.resetCouponCode)
        );
      } else this.onCreateSuccess(res);
    });
  };
  resetCouponCode = () => {
    this.setState(
      {
        orderService: {
          ...this.state.orderService,
          couponCode: ""
        }
      },
      () => {
        this.props.sendVoucher(TYPE_DISCOUNT.NORMAL, "", 0);
        this.checkTotalMoney(0);
      }
    );
  };

  onCreateSuccess = res => {
    this.resetCouponCode();
    BackHandler.removeEventListener("hardwareBackPress", this.onBack);
    NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER, {
      orderServiceID: res.result.orderServiceID
    });
  };

  checkTotalMoney = (
    discount = this.props.voucherState.discount || 0,
    typeDiscount = this.props.voucherState.typeDiscount || 2
  ) => {
    var totalMoney = 0;
    var totalService = listAddServiceBooking.concat(this.state.listAddService);
    for (let i = 0; i < totalService.length; i++) {
      totalMoney = totalMoney + totalService[i].price;
    }
    const { isCheckSwitch } = this.state;

    var usePoint = isCheckSwitch;
    const { UserInfoState } = this.props;

    const getUsePoint = money => {
      return isCheckSwitch;
      // ? Math.max(UserInfoState.data.point - money, 0)
      // : UserInfoState.data.point;
    };
    var money = 0,
      moneyNotUsePoint = 0;
    if (typeDiscount == TYPE_DISCOUNT.PERCENT) {
      money = Math.max(
        totalMoney * (1 - parseInt(discount) / 100) - usePoint,
        0
      );
      moneyNotUsePoint = Math.max(
        totalMoney * (1 - parseInt(discount) / 100),
        0
      );
    } else {
      money = Math.max(totalMoney - usePoint - discount, 0);
      moneyNotUsePoint = Math.max(totalMoney - discount, 0);
    }

    this.setState({
      totalMoney: money,
      baseMoney: totalMoney,
      usePoint: getUsePoint(moneyNotUsePoint),
      moneyLeftWhenUsePoint:
        UserInfoState.data.point - getUsePoint(moneyNotUsePoint)
    });
  };
  getListCouponUnworn = () => {
    CallApiHelper(API.getListCoupon, TYPE_COUPON.UNWORN, this, res => {
      this.props.sendVoucher(TYPE_DISCOUNT.NORMAL, "", 0, res.result);
    });
  };
  onBack = () => {
    switch (this.state.step) {
      case 1:
        NavigationUtil.goBack();
        break;
      case 2:
        this.setState({ step: 1 });
        break;
      case 3:
        showConfirm(
          R.strings().notif_tab_cus,
          R.strings().are_u_sure_to_cancel,
          () => {
            this.setState({ step: 2 });
          }
        );
        break;
      default:
        break;
    }
    return true;
  };

  render() {
    const { isLoading, error, isDialogLoading } = this.state;
    const { dialogLoading = false } = this.props.booking;
    if (
      this.state.totalMoney < VNPAY_MINIUM_MONEY &&
      this.state.orderService.PaymentType != PAYMENT_METHOD.CASH &&
      this.state.listAddService.serviceID
    )
      this.setState({
        orderService: {
          ...this.state.orderService,
          PaymentType: PAYMENT_METHOD.CASH
        }
      });
    return (
      <ScreenComponent
        back
        onBack={this.onBack}
        isLoading={isLoading}
        dialogLoading={dialogLoading || isDialogLoading}
        isError={error}
        titleHeader={R.strings().booking}
        renderView={
          <>
            <View style={styles.view}>
              <View style={styles.stepIndicator}>
                {this._renderStepIndicator(1)}
                {this._renderStepIndicator(2)}
                {this._renderStepIndicator(3)}
              </View>
              {this._renderStep()}
            </View>
          </>
        }
      />
    );
  }

  _renderStep = () => {
    switch (this.state.step) {
      case 1:
        return this._renderExtraService();
      case 2:
        return this._renderConfirmBooking();
      case 3:
        return this._renderVNPay();
    }
  };

  _renderStepIndicator(step) {
    return (
      <View
        style={{
          flexDirection: "row",
          justifyContent: "center",
          alignItems: "center"
        }}
      >
        <View
          style={{
            width: 24,
            height: 24,
            borderRadius: 24 / 2,
            borderColor: theme.colors.gray,
            borderWidth: 0.5,
            backgroundColor:
              this.state.step >= step
                ? theme.colors.primary
                : theme.colors.backgroundCircle,
            alignItems: "center",
            justifyContent: "center"
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color:
                this.state.step >= step
                  ? theme.colors.white
                  : theme.colors.black
            }}
          >
            {step}
          </Text>
        </View>
        {step != 3 && (
          <View
            style={{
              height: 0.5,
              width: 20,
              backgroundColor: theme.colors.gray
            }}
          />
        )}
      </View>
    );
  }
  _renderExtraService() {
    const {
      cateName,
      description
    } = this.props.navigation.state.params.listAddService;

    return (
      <View style={{ flex: 1, width: "100%", alignItems: "center" }}>
        <View
          style={{
            backgroundColor: colors.white,
            marginVertical: 10,
            width: "100%"
          }}
          children={
            <>
              <Text style={styles.cateName}>{cateName}</Text>
              <Text style={styles.description}>{description}</Text>
            </>
          }
        />

        {!!this.state.listAddService.serviceID && (
          <>
            <Text style={styles.viewTitle}>{R.strings().extra_service}</Text>
            <FlatList
              showsVerticalScrollIndicator={false}
              contentContainerStyle={{ flex: 1, width: windowWidth }}
              data={this.state.listService}
              keyExtractor={(item, index) => index.toString()}
              renderItem={this._renderExtraServiceItem}
            />
          </>
        )}
        <View style={styles.footer}>
          <Button
            width="90%"
            title={R.strings().next}
            backgroundColor={theme.colors.primary}
            colorText={theme.colors.white}
            // forward
            uppercase
            action={() => {
              this.setState(
                {
                  step: 2,
                  discountMoney: this.props.voucherState.discount,
                  orderService: {
                    ...this.state.orderService,
                    couponCode: this.props.voucherState.code
                  }
                },
                () => this.checkTotalMoney()
              );
            }}
          />
        </View>
      </View>
    );
  }

  onChangeTextDiscount = text => {
    this.setState(
      {
        orderService: {
          ...this.state.orderService,
          couponCode: text.toUpperCase()
        }
      },
      () => {
        const index = this.props.voucherState.list
          .map(e => e.couponCode)
          .indexOf(text.toUpperCase());
        var discount = 0;
        if (index != -1) {
          const { list } = this.props.voucherState;
          this.props.sendVoucher(
            list[index].typeDiscount,
            text.toUpperCase(),
            list[index].discount
          );
          this.checkTotalMoney(list[index].discount, list[index].typeDiscount);
        } else {
          this.props.sendVoucher(TYPE_DISCOUNT.NORMAL, text.toUpperCase(), 0);
          this.checkTotalMoney(discount);
        }
      }
    );
  };
  checkExtraServiceItem(id) {
    if (extraServiceID.length == 0) return false;
    else {
      for (let i = 0; i < extraServiceID.length; i++) {
        if (extraServiceID[i] === id) {
          return true;
        }
      }
      return false;
    }
  }

  removeID(id) {
    for (var i = 0; i < extraServiceID.length; i++) {
      if (extraServiceID[i] == id) {
        extraServiceID.splice(i, 1);
        listAddServiceBooking.splice(i, 1);
        this.state.orderService.additionService.splice(i, 1);
      }
    }
  }

  _renderExtraServiceItem = ({ item, index }) => {
    const { extraServiceIDState } = this.state;
    return (
      <TouchableOpacity
        style={[
          styles.extraServiceItem,
          {
            backgroundColor:
              extraServiceIDState.indexOf(item.serviceID) == -1
                ? theme.colors.white
                : theme.colors.selectItem
          }
        ]}
        onPress={() => {
          if (this.checkExtraServiceItem(item.serviceID) == false) {
            extraServiceID.push(item.serviceID);
            this.state.orderService.additionService.push(item.serviceID);
            listAddServiceBooking.push(item);
            this.setState({
              extraServiceIDState: extraServiceID
            });
          } else {
            this.removeID(item.serviceID);
            this.setState({
              extraServiceIDState: extraServiceID
            });
          }
        }}
      >
        <View
          style={{
            flexDirection: "row",
            justifyContent: "space-between",
            alignItems: "center"
          }}
        >
          <View style={{ flexDirection: "row", alignItems: "center" }}>
            <Icon.FontAwesome
              name={
                extraServiceIDState.indexOf(item.serviceID) == -1
                  ? "square-o"
                  : "check-square-o"
              }
              size={22}
              color={theme.colors.darkBlue}
            />
            <Text style={styles.txtCateName}>{item.cateName}</Text>
          </View>

          <NumberFormat
            value={item.price}
            displayType="text"
            thousandSeparator
            suffix="đ"
            prefix={item.value > 0 && "+"}
            renderText={value => (
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_bold,
                  fontSize: 14,
                  color: theme.colors.darkBlue
                }}
                children={value}
              />
            )}
          />
        </View>
        {!!item.description && (
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 12,
              marginHorizontal: 28,
              marginTop: 5
            }}
            children={item.description}
          />
        )}
      </TouchableOpacity>
    );
  };

  _renderConfirmBooking = () => {
    const { UserInfoState } = this.props;
    const textButtonPay =
      this.state.orderService.PaymentType == PAYMENT_METHOD.VNPAY
        ? R.strings().pay
        : R.strings().confirm;
    return (
      <View style={{ width: "100%", flex: 1 }}>
        <ModalAlert
          contentView={
            <>
              {this.state.listRequestService?.map(e => (
                <TouchableOpacity
                  onPress={() =>
                    this.setState({
                      orderService: {
                        ...this.state.orderService,
                        note: e
                      },
                      isShowRequestService: false,
                      note: e
                    })
                  }
                  children={<Text style={styles.item_note} children={e} />}
                />
              ))}
              <TextInput
                style={styles.modal_textinput}
                multiline
                value={this.state.note || ""}
                maxLength={256}
                numberOfLines={3}
                scrollEnabled={false}
                placeholderTextColor={colors.gray}
                placeholder={R.strings().request}
                onChangeText={note => {
                  this.setState({ note });
                }}
              />
            </>
          }
          isVisible={this.state.isShowRequestService}
          onClose={() => {
            isSubmitRequestService = false;
            this.setState({
              isShowRequestService: false
            });
          }}
          onModalHide={() => {
            if (isSubmitRequestService)
              this.setState({
                orderService: {
                  ...this.state.orderService,
                  note: this.state.note
                }
              });
          }}
          onSubmit={() => {
            isSubmitRequestService = true;
            this.setState({
              isShowRequestService: false
            });
          }}
        />
        <ModalAlert
          contentView={
            <>
              {this.state.listNoteForMaster?.map(e => (
                <TouchableOpacity
                  onPress={() =>
                    this.setState({
                      orderService: {
                        ...this.state.orderService,
                        reasonNote: e
                      },
                      isShowNoteForMaster: false,
                      reasonNote: e
                    })
                  }
                  children={<Text style={styles.item_note} children={e} />}
                />
              ))}
              <TextInput
                multiline
                maxLength={256}
                numberOfLines={3}
                value={this.state.reasonNote || ""}
                scrollEnabled={false}
                style={styles.modal_textinput}
                placeholderTextColor={colors.gray}
                placeholder={R.strings().choose_your_visit_request}
                onChangeText={reasonNote => {
                  this.setState({ reasonNote });
                }}
              />
            </>
          }
          isVisible={this.state.isShowNoteForMaster}
          onClose={() => {
            isSubmitNoteForMaster = false;
            this.setState({
              isShowNoteForMaster: false
            });
          }}
          onModalHide={() => {
            if (isSubmitNoteForMaster)
              this.setState({
                orderService: {
                  ...this.state.orderService,
                  reasonNote: this.state.reasonNote || ""
                }
              });
          }}
          onSubmit={() => {
            isSubmitNoteForMaster = true;
            this.setState({
              isShowNoteForMaster: false
            });
          }}
        />
        <Text style={styles.viewTitle}>{R.strings().confirm_booking}</Text>
        <KeyboardAwareScrollView
          contentContainerStyle={{
            backgroundColor: theme.colors.backgroundColor,
            paddingBottom: height / 4
          }}
          heightOffset={this.state.heightFooter || 0}
        >
          <RowAvatarInfo
            info={{
              name: UserInfoState.data.name,
              avatarSource: UserInfoState.data.urlAvatar
                ? { uri: UserInfoState.data.urlAvatar }
                : R.images.ic_symbol,
              numberPhone: UserInfoState.data.phone,
              address: UserInfoState.data.address
            }}
            carInfo={{
              carImage:
                this.state.carInfo?.listImage?.length > 0
                  ? { uri: this.state.carInfo?.listImage[0].url }
                  : R.images.carDemo,
              carModel: this.state.carInfo.carBrand,
              licensePlate: this.state.carInfo.licensePlates
            }}
            dateTime={
              this.state.orderService.isBookingNow == 0
                ? this.state.orderService.BookingDateInput
                : getCurrentTimeString()
            }
            editTime={() => {
              this.setDialogLoading(true, this.props.toggleModal);
            }}
          />
          <ServicePack
            supportPack={
              [this.state.listAddService].concat(listAddServiceBooking) || []
            }
          />
          <Promotion
            edit
            onPressVoucher={() => {
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.PROMOTION, {
                type: TYPE_NAVIGATION.BOOKING,
                checkTotalMoney: this.checkTotalMoney
              });
            }}
            baseMoney={this.state.baseMoney}
            priceDiscout={this.props.voucherState.discount}
            priceDiscoutType={this.props.voucherState.typeDiscount}
            value={this.props.voucherState.code}
            resetCouponCode={this.resetCouponCode}
            yourPoint={UserInfoState.data.point}
            yourPointHave={this.state.moneyLeftWhenUsePoint || 0}
            isCheckSwitch={this.state.isCheckSwitch}
            onChangeTextDiscount={this.onChangeTextDiscount}
            toggleSwitch={usePoint => {
              this.setState(
                {
                  isCheckSwitch: usePoint
                },
                () => {
                  this.checkTotalMoney();
                }
              );
            }}
            requestService={this.state.orderService.note}
            noteForMaster={this.state.orderService.reasonNote}
            onPressRequest={() =>
              this.setState({
                isShowRequestService: true
              })
            }
            onPressVisitationRequest={() =>
              this.setState({
                isShowNoteForMaster: true
              })
            }
            vatCheck={this.state.vatCheck}
            onVatCheck={() => {
              this.setState({
                vatCheck: !this.state.vatCheck
              });
            }}
          />
        </KeyboardAwareScrollView>
        <KeyboardAvoidingView
          behavior="position"
          keyboardVerticalOffset={this.state.heightFooter - 15 || 0}
          children={
            <View style={styles.footerStep2}
              onLayout={event => this.setState({
                heightFooter: event.nativeEvent.layout.height
              })}
            >
              {!this.state.isShowKeyboard && (
                <View style={{ flexDirection: "row", alignItems: "center" }}>
                  {!!this.state.listAddService.serviceID &&
                    this._renderPayMentMethod(
                      PAYMENT_METHOD.CASH,
                      R.strings().cash,
                      () =>
                        this.setState({
                          orderService: {
                            ...this.state.orderService,
                            PaymentType: PAYMENT_METHOD.CASH
                          }
                        })
                    )}
                  {RELEASE_CHANNEL == CHANNEL.PRODUCT &&
                    this.state.totalMoney >= VNPAY_MINIUM_MONEY &&
                    this._renderPayMentMethod(
                      PAYMENT_METHOD.VNPAY,
                      "VNPay",
                      () =>
                        this.setState({
                          orderService: {
                            ...this.state.orderService,
                            PaymentType: PAYMENT_METHOD.VNPAY
                          }
                        })
                    )}
                </View>
              )}
              <Button
                width="90%"
                title={
                  textButtonPay +
                  " - " +
                  formatMoney(this.state.totalMoney) +
                  "đ"
                }
                disabled={!this.state.orderService.PaymentType}
                backgroundColor={
                  this.state.orderService.PaymentType
                    ? theme.colors.primary
                    : theme.colors.gray
                }
                colorText={theme.colors.white}
                uppercase
                action={() => {
                  if (
                    UserInfoState.data?.cancelOrder == 0 &&
                    this.state.orderService.PaymentType == PAYMENT_METHOD.CASH
                  )
                    showMessages(
                      R.strings().notif_tab_cus,
                      R.strings().message_cancel_many_time
                    );
                  else
                    showConfirm(
                      R.strings().notif_tab_cus,
                      R.strings().message_after_booking,
                      () => {
                        const state = {
                          orderService: {
                            ...this.state.orderService,
                            couponCode: this.props.voucherState.code,
                            UsePoint: !this.state.isCheckSwitch ? 0 : 1
                          }
                        };
                        this.setState(state, this.createOrder);
                      }
                    );
                }}
              />
            </View>
          }
        />
      </View>
    );
  };
  _renderPayMentMethod = (type, title, action) => {
    return (
      <TouchableOpacity
        onPress={action}
        style={{
          paddingVertical: 17,
          justifyContent: "center",
          alignItems: "center",
          borderColor:
            this.state.orderService.PaymentType == type
              ? theme.colors.primary
              : theme.colors.grayBorder,
          borderWidth: 0.5,
          width:
            !!this.state.listAddService.serviceID &&
              this.state.totalMoney >= VNPAY_MINIUM_MONEY
              ? "45%"
              : "90%"
        }}
      >
        {this.state.orderService.PaymentType == type && (
          <Icon.MaterialCommunityIcons
            name={"check-bold"}
            size={20}
            style={{
              color: theme.colors.primary,
              position: "absolute",
              top: 0,
              right: 0
            }}
          />
        )}

        <Text
          style={{
            fontFamily: R.fonts.quicksand_bold,
            fontSize: 14,
            color:
              this.state.paymentMethod == type
                ? theme.colors.primary
                : theme.colors.nameText
          }}
        >
          {title}
        </Text>
      </TouchableOpacity>
    );
  };
  _renderVNPay() {
    return (
      <View style={{ flex: 1, width: "100%", alignItems: "center" }}>
        <Text style={styles.viewTitle}>{R.strings().confirm_booking}</Text>
        <Text style={styles.text_vnpay_waiting}>
          {R.strings().waiting_vnpay}
        </Text>
        <View style={styles.VNPayView}>
          <Button
            width="45%"
            title={R.strings().cancel}
            backgroundColor={theme.colors.backgroundColorButtonGray}
            colorText={theme.colors.white}
            uppercase
            action={() => {
              showConfirm(
                R.strings().notif_tab_cus,
                R.strings().are_u_sure_to_cancel,
                () => {
                  this.setState({ step: 2 });
                }
              );
            }}
          />
        </View>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  modal_textinput: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    borderRadius: 10,
    borderWidth: 0.5,
    padding: 10,
    textAlignVertical: "top",
    color: "black",
    height: 100
  },
  cateName: {
    fontFamily: R.fonts.quicksand_bold,
    textAlign: "center",
    fontSize: 20,
    color: colors.primary
  },
  description: {
    fontFamily: R.fonts.quicksand_bold,
    textAlign: "center",
    fontSize: 14,
    margin: 10,
    color: colors.gray
  },
  view: {
    flex: 1,
    backgroundColor: theme.colors.backgroundColor,
    alignItems: "center"
  },
  stepIndicator: {
    paddingTop: 10,
    width: "100%",
    flexDirection: "row",
    justifyContent: "center",
    alignItems: "center"
  },
  viewTitle: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.gray,
    paddingVertical: 5,
    marginBottom: 10,
    textTransform: "uppercase",
    textAlign: "center"
  },
  footer: {
    paddingHorizontal: 12,
    paddingVertical: 9,
    width: "100%",
    position: "absolute",
    backgroundColor: theme.colors.white,
    bottom: 0,
    flexDirection: "row",
    justifyContent: "space-around"
  },
  extraServiceItem: {
    width: "100%",
    paddingVertical: 10,
    marginBottom: 10,
    paddingVertical: 10,
    paddingLeft: 10,
    paddingRight: 15
  },
  txtCateName: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: theme.colors.darkBlue,
    marginLeft: 10
  },
  footerStep2: {
    paddingHorizontal: 0,
    paddingTop: 9,
    width: "100%",
    position: "absolute",
    backgroundColor: theme.colors.white,
    bottom: 0,
    justifyContent: "space-around",
    alignItems: "center"
  },
  VNPayView: {
    paddingHorizontal: 12,
    paddingVertical: 9,
    width: "100%",
    position: "absolute",
    backgroundColor: theme.colors.white,
    bottom: 0,
    flexDirection: "row",
    justifyContent: "space-around"
  },
  item_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 10,
    marginBottom: 10,
    borderRadius: 10,
    borderWidth: 0.5,
    color: colors.primary
  },
  text_vnpay_waiting: {
    fontFamily: R.fonts.quicksand_bold,
    color: colors.primary,
    fontSize: 22,
    marginVertical: height / 4
  }
});

const mapStateToProps = state => ({
  UserInfoState: state.userReducer,
  selectLocation: state.selectLocation,
  voucherState: state.voucherReducer,
  tabBookingScreenState: state.state.BookingCustomer,
  booking: state.state.booking,
  orderHistory:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER],
  bookingService:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE],
  toggleModal:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING].toggleModal,
  BookingDateInput:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING].orderService
      .BookingDateInput
});

const mapDispatchToProps = {
  getUserInfoAction,
  sendVoucher,
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(BookingScreen);
