import ModalAlert from "./ModalAlert";
import React, { useState } from "react";
import { View, TextInput, StyleSheet, Image, Text } from "react-native";
import theme, { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
import RateStar from "./RateStar";
import callAPI, { callAPIHook } from "@app/utils/CallApiHelper";
import { CreateReview } from "@api";
import { connect } from "react-redux";
import Props from "@app/utils/Props";
import { GET_ORDER_HISTORY } from "@app/redux/actions/type";
import { SCREEN_ROUTER_CUSTOMER } from "@app/constants/C";
import FastImage from "./FastImage";
import reactotron from "reactotron-react-native";
type ModalRatinglProps = Props & RatingAlert;
interface RatingAlert {
  agentAvatar;
  agentName;
  item?;
  agentID?;
  isModalVisible?: boolean;
  toggleModal?: () => void;
  submitModal?: () => void;
}
let isSubmit =false
export default connect(
  state => ({
    reducer: state
  }),
  require("@action/")
)((props: ModalRatinglProps) => {
  const {
    agentAvatar,
    agentName,
    agentID,
    isModalVisible,
    toggleModal,
    submitModal,
    item
  } = props;
  const [note, useNote] = useState("");
  const [star, useStar] = useState(5);
  const onSubmit = () => {
    callAPI(
      CreateReview,
      {
        orderServiceID: item.orderServiceID,
        point: star,
        Note: note
      },
      null,
      res => {
        var state = props.reducer.state.historyOrder;
        if(state?.resetData)
        state.resetData();
        resetState()
      }
    );
  };

  const resetState = () => {
    useNote("");
    useStar(5);
  };

  const onClose = () => {
    resetState();
    toggleModal();
  };
  return (
    <ModalAlert
      onClose={() => {
        isSubmit=false
        toggleModal();
      }}
      onSubmit={() => {
        isSubmit=true
        if (toggleModal) toggleModal()
      }}
      onModalHide={() => {
        if (isSubmit) {
          if (submitModal) submitModal();
        onSubmit();
        }
      }}
      validSubmit={note != ""}
      isVisible={isModalVisible}
      contentView={
        <View style={styles.modal}>
          <FastImage
            style={{ width: 82, height: 82, borderRadius: 82 / 2 }}
            source={item?.agentAvatar ? { uri: item?.agentAvatar } : R.images.avatarDemo}
          />
          <Text
            style={styles.agentName_text}
            children={item?.agentName || R.strings().washer}
          />
          <RateStar
            numberStar={star}
            style={{ marginTop: 5 }}
            size={30}
            isShowNumber
            rating
            onFinishRating={useStar}
          />
          <View style={styles.textInput}>
            <TextInput
              placeholder={R.strings().type_something}
              placeholderTextColor={colors.gray}
              multiline
              value={note}
              onChangeText={useNote}
              onSubmitEditing={submitModal}
              style={{
                height: 100,
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 14,
                textAlignVertical: "top",
                color: "black"
              }}
              maxLength={256}
            />
          </View>
        </View>
      }
    />
  );
});

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
    backgroundColor: theme.colors.backgroundColor,
    alignItems: "center"
  },
  modal: {
    // marginHorizontal:14,
    paddingHorizontal: 14,
    marginTop: 10,
    backgroundColor: theme.colors.white,
    alignItems: "center",
    // paddingTop: 26,
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
  }
});
