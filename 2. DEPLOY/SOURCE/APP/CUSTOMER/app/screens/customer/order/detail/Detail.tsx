import React, { useEffect, useState } from "react";
import {
  StyleSheet,
  Text,
  ScrollView,
  RefreshControl,
  View,
  TouchableOpacity
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import {
  RowAvatarInfo,
  WasherInfo,
  RowCarInfor,
  RowLableInfo
} from "@app/components/FormRow";
import {
  ServicePack,
  Promotion,
  PaymentDetail
} from "@app/components/PaymentDetail";
import theme, { colors } from "@app/constants/Theme";
import Props, { DataEntity } from "@app/utils/Props";
import StepWashingImg from "@app/components/StepWashingImg";
import reactotron from "@app/reactotron/ReactotronConfig";
import { ORDER_TYPE } from "@constant";
type ProcessDetailProps = {
  data: Data;
  getData?: () => void;
  isLoading?;
  showStepWashing?: boolean;
  showHeader?: boolean;
  showAddition?: [];
  status?: string;
  renderButton?: JSX.Element;
  rightComponent?: JSX.Element;
};
type Data = DataEntity;

export default (props: ProcessDetailProps) => {
  const renderWasherInfo = (data: Data) => (
    <WasherInfo
      washerInfo={{
        name: data?.agentName,
        avatarSource: data?.agentAvatar
          ? { uri: data?.agentAvatar }
          : R.images.ic_booking,
        code: data?.agentPhone,
        star: data?.agentRating
      }}
    />
  );

  const renderAddtionService = (data: []) => (
    <View
      style={{ backgroundColor: colors.white, marginTop: 10 }}
      collapsable
      children={
        <>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              color: colors.red,
              margin: 10
            }}
            children="Dịch vụ thêm"
          />
          {data.map(elem => (
            <>
              <RowLableInfo
                lable={"Khủ trùn UV"}
                title={`${999999} đ`}
                color={theme.colors.textColor}
                borderBottom
              />
            </>
          ))}
          <PaymentDetail totalPrice={999999} />
        </>
      }
    />
  );

  const renderUpdatingWasherInfo = () => (
    <Text
      children={R.strings().updating_washer_infor}
      style={styles.text_updating_washer_info}
    />
  );
  const {
    isLoading = false,
    data,
    getData = () => {},
    status = "",
    renderButton = <View style={{ marginBottom: 10 }} />,
    showStepWashing = true,
    showHeader = true,
    showAddition = [],
    rightComponent = <View />
  } = props;

  const waiting_for_washer = !data?.agentName;
  const colorStatus = status => {
    switch (status) {
      case R.strings().canceled:
        return colors.red;
      case R.strings().upcoming:
      case R.strings().search_washer:
        return colors.green;
      case R.strings().processing:
      case R.strings().complete:
        return colors.primary;
      default:
        return colors.primary;
    }
  };
  const getDateTime = dateStr => {
    var result = {
      time: "",
      day: ""
    };
    if (!dateStr) return result;
    var dateTime = dateStr.split(" ");
    if (dateTime.length > 1)
      return (result = {
        time: dateTime[0],
        day: dateTime[1]
      });
    return result;
  };
  useEffect(getData, []);
  const urlAvatar = data =>
    data?.customerAvatar ? { uri: data?.customerAvatar } : R.images.avatarDemo;
  return (
    <ScreenComponent
      titleHeader={showHeader && R.strings().order_detail}
      back
      isLoading={isLoading}
      rightComponent={rightComponent}
      renderView={
        <ScrollView
          refreshControl={
            <RefreshControl refreshing={isLoading} onRefresh={getData} />
          }
          showsVerticalScrollIndicator={false}
          style={{
            backgroundColor: theme.colors.backgroundColor,
            paddingBottom: 20
          }}
          children={
            <>
              <RowAvatarInfo
                header={{
                  code: data?.code || data?.serviceCode,
                  status: status,
                  textColor: colorStatus(status)
                }}
                info={{
                  name: data?.customerName,
                  avatarSource: urlAvatar(data),
                  numberPhone: data?.customerPhone,
                  address: data?.customerAddress
                }}
                dateTime={
                  status == R.strings().complete
                    ? `${getDateTime(data?.startDateStr).time} - ${
                        getDateTime(data?.completionDateStr).time
                      } ${getDateTime(data?.completionDateStr).day}`
                    : data?.bookDateStr || data?.bookingDateStr
                }
                distance={data?.distance}
              />
              {waiting_for_washer
                ? renderUpdatingWasherInfo()
                : renderWasherInfo(data)}
              <RowCarInfor
                name={`${data?.car?.carBrand} (${data?.car?.carModel})`}
                licensePlate={data?.car?.licensePlates}
                image={
                  data?.car?.listImage.length > 0
                    ? { uri: data?.car?.listImage[0].url }
                    : R.images.carDemo
                }
              />
              <ServicePack supportPack={data?.listService || []} />
              <Promotion
                code={data?.couponCode}
                priceDiscout={data?.couponPoint}
                requestService={data?.note}
                noteForMaster={data?.reasonNote}
                yourPoint={data?.usePoint}
              />
              <PaymentDetail
                totalPrice={data?.totalPrice}
                usePoint={data?.usePoint}
                paymentType={data?.paymentType}
              />
              {showStepWashing && (
                <StepWashingImg
                  data={data?.listImageRequire?.map(value => value) || []}
                />
              )}
              {showAddition && renderAddtionService(showAddition)}
              {renderButton}
            </>
          }
        />
      }
    />
  );
};

const styles = StyleSheet.create({
  text_updating_washer_info: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 20,
    alignSelf: "center",
    paddingVertical: 10,
    backgroundColor: colors.backgroundColor,
    color: colors.gray
  }
});
export interface Params {
  serviceName: string;
  carNote: CarNoteOrParkNote;
  reasonCancel?: null;
  parkNote: CarNoteOrParkNote;
  listService?: ListServiceEntity[] | null;
  timeStartOrder: number;
  uri: string;
  additionService?: null;
  mainService?: null;
  car: Car;
  longi: number;
  lati: number;
  isInDoor: number;
  comboID: number;
  couponCode?: null;
  placeID?: null;
  bookingDateInput?: null;
  isBookingNow: number;
  agentCode?: null;
  firstID: number;
  comboCode: string;
  orderServiceID: number;
  carID: number;
  paymentType: number;
  code: string;
  listImageRequire?: ListImageRequireEntity[] | null;
  totalPrice: number;
  basePrice: number;
  couponPoint: number;
  usePoint: number;
  status: number;
  completionDateStr: string;
  confirmDateStr: string;
  createDateStr: string;
  bookDateStr: string;
  estBookDateStr: string;
  createDate: string;
  confirmDate?: null;
  successDate?: null;
  customerName: string;
  customerPhone: string;
  customerAddress: string;
  agentName: string;
  agentPhone: string;
  agentAvatar: string;
  rating: number;
  note: string;
  noteRate: string;
  distance: number;
  customerID: number;
  agentID?: null;
  bookingDate: string;
  estBookDate: string;
}
export interface CarNoteOrParkNote {
  note?: null;
  listImage?: null[] | null;
}
export interface ListServiceEntity {
  name: string;
  price: number;
  type: number;
}
export interface Car {
  carID: number;
  carModel: string;
  carModelID: number;
  carBrand: string;
  carBrandID: number;
  licensePlates: string;
  manufacturingDate: string;
  registrationDate: string;
  registrationDateStr: string;
  carColor: string;
  listImage?: null[] | null;
  status?: null;
  vehicleRegistration?: null;
}
export interface ListImageRequireEntity {
  name: string;
  before: BeforeOrAfter;
  after: BeforeOrAfter;
  order: number;
}
export interface BeforeOrAfter {
  imageRequireID: number;
  url?: null;
  dateStr: string;
  date?: null;
}
