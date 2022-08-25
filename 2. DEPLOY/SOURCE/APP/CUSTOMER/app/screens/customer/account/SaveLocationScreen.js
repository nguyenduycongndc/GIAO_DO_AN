import React, { useState, useEffect } from "react";
import {
  StyleSheet,
  Text,
  View,
  FlatList,
  TouchableOpacity,
  UIManager,
  LayoutAnimation,
  TextInput
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import { connect } from "react-redux";
import Empty from "@app/components/Empty";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import ModalAlert from "@app/components/ModalAlert";
import reactotron from "reactotron-react-native";
import { callAPIHook } from "@app/utils/CallApiHelper";
import {
  searchPlaceAutoComplete,
  AddLocation,
  DeleteLocation,
  UpdateLocation
} from "@api";
import Loading from "@app/components/Loading";
import { showConfirm, showMessages } from "@app/components/Alert";
let timeOutSearch = null;
let isSubmit = false;
export default connect(
  state => ({ reducer: state }),
  require("@action")
)(props => {
  const [isAddLocation, useModal] = useState(false);
  const [resSearchPlace, useResSearchPlace] = useState([]);
  const [searchKey, useSearchKey] = useState("");
  const [isLoading, useLoading] = useState(false);
  const [isUpdate, useUpdate] = useState("");
  const [isLoadingSeachLocation, useLoadingSeachLocation] = useState(false);
  const [Name, useName] = useState("");
  const [Description, useDescription] = useState("");
  const [PlaceId, usePlaceId] = useState("");
  const [placeHolder, useplaceHolder] = useState("");
  const toggleModal = () => useModal(!isAddLocation);
  const emptyScreen = () => <Empty description={R.strings().empty_location} />;
  const addLocation = () => {
    const valid = props.reducer.userReducer.data?.listLocation?.length >= 5;
    return (
      <>
        <TouchableOpacity
          onPress={toggleModal}
          disabled={valid}
          children={
            <Text
              style={[
                styles.add_location,
                {
                  backgroundColor: valid ? colors.gray : colors.primary
                }
              ]}
              children={R.strings().add_location}
            />
          }
        />
        <Text
          style={{
            fontFamily: R.fonts.quicksand_light,
            fontSize: 13,
            textAlign: "center"
          }}
          children={R.strings().hold_to_delete_location}
        />
        <Text
          style={{
            fontFamily: R.fonts.quicksand_light,
            fontSize: 13,
            textAlign: "center"
          }}
          children={R.strings().save_max_location}
        />
      </>
    );
  };
  useEffect(() => {}, [PlaceId]);
  const search = async () => {
    if (searchKey.trim() != "") {
      callAPIHook(
        searchPlaceAutoComplete,
        searchKey.trim(),
        useLoadingSeachLocation,
        res => useResSearchPlace(res.data.predictions)
      );
    } else useResSearchPlace([]);
  };
  const addLocationView = (
    <>
      <Text
        style={{
          fontFamily: R.fonts.quicksand_medium,
          fontSize: 16,
          textAlign: "center"
        }}
        children={R.strings().select_location}
      />
      <TextInput
        style={styles.text_input}
        placeholderTextColor={colors.gray}
        placeholder={R.strings().name}
        value={Name}
        maxLength={256}
        onChangeText={useName}
      />
      <TextInput
        style={styles.text_input}
        value={searchKey}
        maxLength={256}
        onChangeText={text => {
          useSearchKey(text);
          if (text.trim().length > 0) {
            if (timeOutSearch) clearTimeout(timeOutSearch);
            timeOutSearch = setTimeout(search, 1000);
          } else {
            clearTimeout(timeOutSearch);
            useResSearchPlace([]);
          }
        }}
        placeholderTextColor={colors.gray}
        placeholder={placeHolder || R.strings().address}
      />
      {isLoadingSeachLocation && <Loading />}
      {resSearchPlace.length > 0 && (
        <FlatList
          keyboardShouldPersistTaps={true}
          style={[styles.textItemResSearch]}
          data={resSearchPlace}
          renderItem={({ item }) => (
            <TouchableOpacity
              style={styles.item_location}
              onPress={() => {
                useSearchKey(item.description);
                usePlaceId(item.place_id);
                useDescription(item.description);
                useResSearchPlace([]);
              }}
              children={
                <Text
                  style={styles.text_location}
                  numberOfLines={2}
                  children={item.description}
                />
              }
            />
          )}
        />
      )}
    </>
  );
  const clearDataSeach = () => {
    useName("");
    useDescription("");
    usePlaceId("");
    useSearchKey("");
    useResSearchPlace([]);
    useUpdate("");
    useplaceHolder("");
  };

  const ModalAlertAddLocation = () => (
    <ModalAlert
      isVisible={isAddLocation}
      contentView={addLocationView}
      validSubmit={!!Name && !!PlaceId && !!searchKey}
      onSubmit={() => {
        isSubmit = true;
        toggleModal();
      }}
      onClose={() => {
        isSubmit = false;
        toggleModal();
      }}
      onModalHide={() => {
        if (isSubmit) {
          const api = isUpdate ? UpdateLocation : AddLocation;
          const payload = isUpdate
            ? { PlaceId, Name, CustomerAddress: Description, ID: isUpdate }
            : { PlaceId, Name, CustomerAddress: Description };
          callAPIHook(
            api,
            payload,
            useLoading,
            props.getUserInfoAction,
            null,
            clearDataSeach
          );
        } else clearDataSeach();
      }}
    />
  );
  const renderItem = ({ item, index }) => (
    <TouchableOpacity
      style={styles.root_item}
      onPress={() => {
        toggleModal();
        useName(item.name);
        useDescription(item.description);
        usePlaceId("");
        useSearchKey("");
        useplaceHolder(item.address);
        useUpdate(item.id);
      }}
      onLongPress={() => {
        showConfirm(R.strings().delete_location, "", () =>
          callAPIHook(
            DeleteLocation,
            item.id,
            useLoading,
            props.getUserInfoAction
          )
        );
      }}
    >
      <Text
        style={{ fontFamily: R.fonts.quicksand_bold, fontSize: 16 }}
        children={item?.name || R.strings().location_info}
      />
      <Text
        style={{ fontFamily: R.fonts.quicksand_medium, fontSize: 14 }}
        children={item?.address || R.strings().address}
      />
    </TouchableOpacity>
  );
  const { data, isLoading: isLoadingUser } = props?.reducer?.userReducer;
  return (
    <ScreenComponent
      titleHeader={R.strings().location_info}
      isLoading={isLoading || isLoadingUser}
      back
      renderView={
        <>
          {ModalAlertAddLocation()}
          <FlatList
            ListHeaderComponent={addLocation}
            ListEmptyComponent={emptyScreen}
            data={data?.listLocation || []}
            renderItem={renderItem}
          />
        </>
      }
    />
  );
});

const styles = StyleSheet.create({
  root_item: {
    margin: 10,
    backgroundColor: colors.backgroundGray,
    padding: 10,
    borderRadius: 10,
    overflow: "hidden"
  },
  text_location: {
    width: "95%",
    fontSize: 14,
    fontFamily: R.fonts.quicksand_medium,
    paddingHorizontal: 5
  },
  item_location: {
    marginBottom: 3,
    paddingHorizontal: 10,
    paddingVertical: 4,
    flexDirection: "row",
    alignItems: "center",
    paddingRight: 5
  },
  text_input: {
    borderRadius: 5,
    borderWidth: 0.5,
    padding: 10,
    margin: 10,
    fontFamily: R.fonts.quicksand_regular,
    fontSize: 14,
    color: "black"
  },
  add_location: {
    width: "95%",
    color: colors.white,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    padding: 10,
    borderRadius: 5,
    overflow: "hidden",
    alignSelf: "center",
    marginVertical: 10,
    textAlign: "center"
  }
});
