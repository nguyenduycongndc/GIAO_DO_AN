import R from "@app/assets/R";
import Empty from "@app/components/Empty";
import ScreenComponent from "@app/components/ScreenComponent";
import * as API from "@app/constants/Api";
import { ORDER_STATUS, SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  getListOrderService,
  getListOrderHistoryService,
  navigateTab,
  getUserInfo,
  updateProcessingImage
} from "@app/redux/actions";
import CallApiHelper from "@app/utils/CallApiHelper";
import Button from "@component/Button";
import ButtonPrimary from "@component/ButtonPrimary";
import theme, { colors } from "@theme";
import React, { Component } from "react";
import {
  RefreshControl,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
  Image,
  FlatList,
  Dimensions
} from "react-native";
import FastImage from "react-native-fast-image";
import { connect } from "react-redux";
import reactotron from "reactotron-react-native";
import ModalAlert from "@app/components/ModalAlert";
import InfoItem from "@app/components/InfoItem";
import {
  InfoCustomer,
  InfoWasher,
  InfoCar,
  InfoService,
  InfoPayment,
  InfoContact
} from "./component/OrderDetailItem";
import { showMessages, showConfirm } from "@app/components/Alert";
import LoadableImage from "@app/components/LoadableImage";
import WarningStatus from "./WarningStatus";
import ActionSheet from "react-native-actionsheet";
import ImageResizer from "react-native-image-resizer";
import ImagePicker from "react-native-image-picker";
import { OrderDetailScreen } from "./orderdetail/OrderDetailScreen";

var isBefore = null;
var index = null;

const maxWidth = Dimensions.get("screen").width;
const maxHeight = Dimensions.get("screen").height;
export class OrderProcessingScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isShowDetail: true,
      dialogLoading: false,
      isVisible: false,
      isSendRequest: false,
      dataSearch: {
        status: ORDER_STATUS.WASHING,
        placeID: "",
        fromDate: "",
        toDate: "",
        distance: "",
        page: 1,
        serviceID: "",
        comboID: ""
      },
      currentPosButtonY: 0
    };
  }
  componentDidMount() {
    this.getData();
  }
  getData = () => {
    this.props.getListOrderService(this.state.dataSearch);
  };
  renderImageScratch = () => {
    const { carNote, orderServiceID } = this.props.ProcessingServiceState.data;
    return (
      <View style={styles.containerScratch}>
        <TouchableOpacity
          onPress={() =>
            NavigationUtil.navigate(SCREEN_ROUTER_WASHER.UPLOAD_IMAGE_SCRATCH, {
              listImage: carNote.listImage,
              note: carNote.note,
              orderServiceID: orderServiceID,
              dataSearch: this.state.dataSearch
            })
          }
          style={styles.btnImageScratch}
          activeOpacity={0.4}
        >
          <Text
            style={[
              styles.txtNormal,
              {
                fontSize: 14,
                fontFamily: R.fonts.quicksand_semi_bold,
                marginTop: 0,
                marginLeft: 12
              }
            ]}
          >
            {R.strings().image_scratch}
          </Text>
          <Image source={R.images.ic_right_arrow} style={styles.icArrow} />
        </TouchableOpacity>
        {!!carNote.listImage && (
          <View>
            <FlatList
              style={{ marginHorizontal: 7, marginTop: 5 }}
              data={carNote.listImage}
              numColumns={3}
              renderItem={this.renderItem}
            />
            {carNote.note && !!carNote.note.length && (
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_regular,
                  fontSize: 14,
                  marginLeft: 15
                }}
              >
                {carNote.note}
              </Text>
            )}
          </View>
        )}
      </View>
    );
  };

  renderItem = ({ item, index }) => {
    const { listImage } = this.props.ProcessingServiceState.data.carNote;
    return (
      <TouchableOpacity
        style={[
          styles.image,
          {
            marginRight: index == listImage.length - 1 ? 10 : 5
          }
        ]}
        onPress={() => {
          var listUrl = [];
          listImage.map(e =>
            listUrl.push({
              url: e.image
            })
          );
          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.IMAGE_VIEWER, {
            data: listUrl,
            index: index
          });
        }}
      >
        <LoadableImage
          source={{ uri: item.image }}
          style={{ flex: 1, width: null, height: null, borderRadius: 3 }}
          resizeMode={FastImage.resizeMode.cover}
        />
      </TouchableOpacity>
    );
  };

  getIconService() {
    const { data } = this.props.ProcessingServiceState;
    var service = data.listService.find(
      service => service.name === data.serviceName
    );
    return service;
  }

  renderActionSheet() {
    return (
      <ActionSheet
        ref={o => (this.ActionSheet = o)}
        title={R.strings().select_picture}
        options={[R.strings().image_library, "Camera", R.strings().cancel]}
        cancelButtonIndex={2}
        onPress={indexAction => {
          var isBefore = this.isBefore;
          var index = this.index;
          const data = {
            isBefore,
            listImageRequire: this.props.ProcessingServiceState.data
              .listImageRequire,
            index,
            orderServiceID: this.props.ProcessingServiceState.data
              .orderServiceID
          };
          if (indexAction == 0) {
            this.showPickerImage(data);
          }
          if (indexAction == 1) {
            this.handleNavigateCamera(data);
          }
        }}
      />
    );
  }

  showPickerImage = data => {
    try {
      const options = {};
      ImagePicker.launchImageLibrary(options, response => {
        if (response.didCancel) {
          // console.log("User cancelled photo picker");
        } else if (response.error) {
          // console.log("ImagePicker Error: ", response.error);
        } else if (response.customButton) {
          // console.log("User tapped custom button: ", response.customButton);
        } else {
          // You can also display the image using data:
          // let source = { uri: 'data:image/jpeg;base64,' + response.data };
          var actualWidth = response.width,
            actualHeight = response.height;
          var imgRatio = actualWidth / actualHeight;
          var maxRatio = maxHeight / maxHeight;
          if (actualHeight > maxHeight || actualWidth > maxWidth) {
            if (imgRatio < maxRatio) {
              imgRatio = maxHeight / actualHeight;
              actualWidth = parseInt(imgRatio * actualWidth);
              actualHeight = parseInt(maxHeight);
            } else if (imgRatio > maxRatio) {
              imgRatio = maxWidth / actualWidth;
              actualHeight = parseInt(imgRatio * actualHeight);
              actualWidth = parseInt(maxWidth);
            } else {
              actualHeight = maxHeight;
              actualWidth = maxWidth;
            }
          }

          const source = { uri: response.uri };
          this._resizeImage(source.uri, actualWidth, actualHeight, data);
        }
      });
    } catch (error) {
      console.log("error", error);
    }
  };

  _resizeImage = async (uri, actualWidth, actualHeight, data) => {
    try {
      const response = await ImageResizer.createResizedImage(
        uri,
        actualWidth,
        actualHeight,
        "JPEG",
        70,
        0,
        ""
      );
      this.upLoadImage(response.uri, data);
    } catch (error) {
      console.log(error);
      this.upLoadImage(uri, data);
    }
  };

  upLoadImage = (uri, data) => {
    this.setState({ dialogLoading: true }, async () => {
      const formData = new FormData();
      formData.append("image", {
        name: `images1`,
        type: "image/jpeg",
        filename: "image.png",
        uri: uri
      });
      const { isBefore, listImageRequire, index, orderServiceID } = data;
      const payload = {
        data: formData,
        serviceID: orderServiceID,
        imageRequiteID: isBefore
          ? listImageRequire[index].before.imageRequireID
          : listImageRequire[index].after.imageRequireID
      };

      CallApiHelper(
        API.uploadImage,
        payload,
        this,
        res => {
          this.props.updateProcessingImage({
            data: res.result,
            isBefore,
            index
          });
          this.setState(
            {
              ...this.state,
              dialogLoading: false
            },
            () => {}
          );
        },
        error => {}
      );
    });
  };

  handleNavigateCamera = data => {
    NavigationUtil.navigate(SCREEN_ROUTER_WASHER.CAMERA_SCREEN, {
      data: data
    });
  };

  showActionSheet = () => {
    this.ActionSheet.show();
  };

  render() {
    const { isShowDetail } = this.state;
    const { ProcessingServiceState, UserInfoState } = this.props;
    return (
      <ScreenComponent
        isLoading={ProcessingServiceState.isLoading || UserInfoState.isLoading}
        reload={() => this.getData()}
        isError={ProcessingServiceState.error != null}
        dialogLoading={this.state.dialogLoading}
        renderView={
          !ProcessingServiceState.isLoading &&
          ProcessingServiceState.error == null && (
            <View
              style={{
                flex: 1,
                backgroundColor: "red"
              }}
            >
              <ScrollView
                style={{
                  flex: 1,
                  backgroundColor: theme.colors.backgroundColor
                }}
                removeClippedSubviews={false}
                onContentSizeChange={() => {
                  const {
                    currentPosButtonX,
                    currentPosButtonY,
                    isShowDetail
                  } = this.state;
                  if (
                    !!currentPosButtonX &&
                    !!currentPosButtonY &&
                    isShowDetail
                  )
                    this.scrollRef.scrollTo({
                      x: currentPosButtonX,
                      y: currentPosButtonY,
                      animated: true
                    });
                }}
                ref={ref => (this.scrollRef = ref)}
                refreshControl={
                  <RefreshControl
                    refreshing={ProcessingServiceState.isLoading}
                    onRefresh={() =>
                      this.setState({ isShowDetail: false }, this.getData)
                    }
                  />
                }
              >
                {this.renderActionSheet()}
                {/* {this.renderWarningStatus()} */}
                {ProcessingServiceState.data == null && (
                  <Empty marginTop={height / 5} />
                )}
                {ProcessingServiceState.data && (
                  <View>
                    <WarningStatus data={UserInfoState.data} />
                    <InfoItem
                      orderDetail={ProcessingServiceState.data}
                      icon={this.getIconService()}
                    />
                    {ProcessingServiceState.data.status == ORDER_STATUS.START &&
                      !this.canStartWashing() && (
                        <Text
                          style={{
                            fontSize: 13,
                            fontFamily: R.fonts.quicksand_light,
                            color: theme.colors.primaryDark,
                            marginLeft: 10,
                            fontStyle: "italic"
                          }}
                          children={`${R.strings().warning_start_washing}`}
                        />
                      )}
                    {this.renderImageScratch()}
                    {ProcessingServiceState.data.listImageRequire &&
                      ProcessingServiceState.data.listImageRequire.map(
                        (item, index) => {
                          return this._stepItem(item, index);
                        }
                      )}
                    <View
                      style={{ marginHorizontal: 13 }}
                      ref={ref => {
                        this.marker = ref;
                      }}
                      onLayout={({ nativeEvent }) => {
                        if (this.marker) {
                          this.marker.measure(
                            (x, y, width, height, pageX, pageY) => {
                              this.setState({
                                currentPosButtonY: pageY,
                                currentPosButtonX: pageX
                              });
                            }
                          );
                        }
                      }}
                    >
                      <Button
                        title={R.strings().view_detail}
                        backgroundColor={theme.colors.white}
                        borderColor={theme.colors.grayBorder}
                        colorText={theme.colors.primary}
                        width={"100%"}
                        buttonStyle={{ height: 42 }}
                        action={() => {
                          this.setState({
                            ...this.state,
                            isShowDetail: !this.state.isShowDetail
                          });
                        }}
                      />
                    </View>
                    {this.renderDetailOrder()}
                    <TouchableOpacity
                      onPress={() => {
                        NavigationUtil.navigate(
                          SCREEN_ROUTER_WASHER.ADD_ADDITIONAL,
                          ProcessingServiceState.data?.orderServiceID
                        );
                      }}
                      children={
                        <Text
                          style={styles.add_additional}
                          children="Thêm dịch vụ phụ"
                        />
                      }
                    />
                    {this.renderAction()}
                    {this.renderModal()}
                  </View>
                )}
              </ScrollView>
            </View>
          )
        }
      />
    );
  }
  renderDetailOrder() {
    const { isShowDetail } = this.state;
    const { data } = this.props.ProcessingServiceState;
    if (!isShowDetail) return;
    return (
      <View>
        <InfoCustomer item={data} isShowStatus={false} />
        <InfoWasher item={data} />
        <InfoCar item={data} />
        <InfoService item={data} />
        <InfoPayment item={data} />
      </View>
    );
  }

  onToggleModal = () => {
    this.setState({ isVisible: !this.state.isVisible });
  };

  renderModal() {
    const { isVisible } = this.state;
    return (
      <ModalAlert
        title={R.strings().confirm_pay}
        isVisible={isVisible}
        onModalHide={() => {
          if (this.state.isSendRequest) this.onChangeStatusOrder();
        }}
        onClose={this.onToggleModal}
        onSubmit={() =>
          this.setState({ isSendRequest: true, isVisible: false })
        }
        contentView={
          <View style={{ alignItems: "center", marginBottom: 15 }}>
            <Text
              style={{
                fontSize: 14,
                fontFamily: R.fonts.quicksand_regular
              }}
            >
              {R.strings().content_confirm_pay}
            </Text>
          </View>
        }
      />
    );
  }

  onChangeStatusOrder = () => {
    const { data } = this.props.ProcessingServiceState;
    this.setState(
      {
        ...this.state,
        dialogLoading: true
      },
      () => {
        CallApiHelper(
          API.changeStatusOrder,
          {
            orderServiceID: data.orderServiceID,
            status:
              data.status == ORDER_STATUS.WASHING
                ? ORDER_STATUS.COMPLETED
                : ORDER_STATUS.WASHING
          },
          this,
          async res => {
            if (data.status == ORDER_STATUS.WASHING) {
              this.props.getListOrderHistoryService({ page: 1, status: "" });
              this.props.navigateTab(3);
              NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_DETAIL, {
                orderID: data.orderServiceID
              });
            }
            this.setState({
              ...this.state,
              dialogLoading: false,
              isSendRequest: false
            });
            this.getData();
          },
          error => {
            console.log("error", error);
            this.setState({
              ...this.state,
              dialogLoading: false,
              isSendRequest: false
            });
          }
        );
      }
    );
  };

  renderAction() {
    const { data } = this.props.ProcessingServiceState;
    if (data.status == ORDER_STATUS.START && !this.canStartWashing()) return;
    if (data.status == ORDER_STATUS.WASHING && !this._canComplete()) return;

    return (
      <ButtonPrimary
        onPress={() => {
          if (data.status == ORDER_STATUS.WASHING) this.onToggleModal();
          else this.onChangeStatusOrder();
        }}
        text={
          data.status == ORDER_STATUS.WASHING
            ? R.strings().complete
            : R.strings().start_washing
        }
        style={styles.button_complete}
      />
    );
  }

  canStartWashing = () => {
    const { ProcessingServiceState } = this.props;
    let canWashing = true;
    for (let imageRequire of ProcessingServiceState.data.listImageRequire) {
      if (imageRequire.before.url == null) return false;
    }
    if (
      !ProcessingServiceState.data.carNote.listImage.length &&
      !ProcessingServiceState.data.carNote.note
    )
      return false;
    return canWashing;
  };

  _sperator = (index, size, label) => {
    return (
      <View>
        <View
          style={{
            flexDirection: "row",
            marginTop: 13,
            marginLeft: 8,
            fontFamily: R.fonts.quicksand_medium,
            lineHeight: 14,
            fontSize: 11,
            color: "black"
          }}
        >
          <View>
            {index != 0 && (
              <View
                style={{
                  width: 1,
                  height: 20,
                  backgroundColor: "black",
                  marginLeft: 15
                }}
              />
            )}
            <View
              style={{
                width: 6,
                height: 6,
                borderRadius: 3,
                backgroundColor: "black",
                marginLeft: 12.5
              }}
            />

            <View
              style={{
                width: 1,
                flex: 1,
                minHeight: 5,
                backgroundColor: "black",
                marginLeft: 15
              }}
            />
          </View>

          <Text style={styles.txtNormal}>{label}</Text>
        </View>
      </View>
    );
  };

  _stepItem = (item, index) => {
    const { ProcessingServiceState } = this.props;
    const listSize = ProcessingServiceState.data.listImageRequire.length;
    return (
      <View>
        {this._sperator(index, listSize, item.name)}
        <View
          style={{
            flexDirection: "row",
            justifyContent: "center",
            alignContent: "center",
            backgroundColor: "white",
            marginHorizontal: 10,
            borderRadius: 5,
            elevation: 2,
            shadowOffset: {
              height: 3,
              width: 0
            }
          }}
        >
          {this._imageStep(item, index, item.before.dateStr, true)}
          {this._imageStep(item, index, item.after.dateStr)}
        </View>
      </View>
    );
  };

  _canComplete = () => {
    const { ProcessingServiceState } = this.props;
    let canCompete = true;
    for (let imageRequire of ProcessingServiceState.data.listImageRequire) {
      if (imageRequire.before.url == null || imageRequire.after.url == null)
        return false;
    }

    return canCompete;
  };

  _imageStep = (item, index, time, isBefore = false) => {
    const url = isBefore ? item.before.url : item.after.url;
    const { data } = this.props.ProcessingServiceState;
    return (
      <TouchableOpacity
        style={{
          flex: 1,
          margin: 10
        }}
        onPress={() => {
          if (
            !isBefore &&
            data.status == ORDER_STATUS.START &&
            !this.canStartWashing()
          ) {
            showMessages(R.strings().notice, R.strings().must_start_washing);
            return;
          }
          if (
            !isBefore &&
            data.status == ORDER_STATUS.START &&
            this.canStartWashing()
          ) {
            showConfirm(
              R.strings().notice,
              `${
                R.strings().you_want
              }${R.strings().start_washing.toLowerCase()}?`,
              this.onChangeStatusOrder,
              "",
              R.strings().confirm
            );
            return;
          }
          this.showActionSheet();
          this.isBefore = isBefore;
          this.index = index;
        }}
      >
        <LoadableImage
          style={{
            flex: 1,
            aspectRatio: 15 / 9,
            borderRadius: 5
          }}
          resizeMode={FastImage.resizeMode.cover}
          source={
            url == null
              ? R.images.ic_car_thumb
              : {
                  uri: url
                }
          }
        />
        <View style={styles.img_item}>
          <Text style={styles.title}>
            {isBefore ? R.strings().before : R.strings().after}
          </Text>
          {!!time.length && (
            <>
              <View style={styles.line_vertical} />
              <Text style={styles.title}>{time}</Text>
            </>
          )}
        </View>
      </TouchableOpacity>
    );
  };
}

const styles = StyleSheet.create({
  add_additional: {
    color: colors.white,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    padding: 10,
    margin: 10,
    backgroundColor: colors.primary,
    textAlign: "center",
    borderRadius: 5,
    overflow: "hidden"
  },
  button_complete: { marginTop: 30, marginBottom: 30 },
  containerScratch: {
    borderTopWidth: 0.25,
    borderBottomWidth: 0.25,
    borderColor: "#707070",
    backgroundColor: "white",
    marginVertical: 10,
    justifyContent: "center",
    paddingVertical: 12
  },
  btnImageScratch: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center"
  },
  txtNormal: {
    marginTop: 13,
    marginLeft: 8,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 11,
    color: "black"
  },
  icArrow: {
    width: 8,
    height: 14,
    tintColor: "black",
    marginRight: 12
  },
  image: {
    flex: 1 / 3,
    height: 93,
    margin: 3,
    borderRadius: 3
  },
  img_item: {
    // width: 73,
    backgroundColor: "rgba(112,112,112,0.4)",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-around",
    borderRadius: 20,
    position: "absolute",
    bottom: 4,
    left: 6,
    paddingHorizontal: 14,
    paddingVertical: 3,
    justifyContent: "center"
  },
  title: {
    fontSize: 10,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.white,
    textAlignVertical: "center"
  },
  line_vertical: {
    width: 1,
    marginHorizontal: 5,
    backgroundColor: theme.colors.white
  },
  car_status: {
    marginVertical: 15,
    marginLeft: 10,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 18,
    color: theme.colors.primaryDark
  }
});

const mapStateToProps = state => ({
  ProcessingServiceState: state.processingServiceReducer,
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getListOrderService,
  getListOrderHistoryService,
  navigateTab,
  getUserInfo,
  updateProcessingImage
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderProcessingScreen);
