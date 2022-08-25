import React from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  FlatList,
  Image
} from "react-native";
import R from "@app/assets/R";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER } from "@app/constants/C";
import FastImage from "@app/components/FastImage";
import reactotron from "@app/reactotron/ReactotronConfig";

export default props => {
  const { listImage = [], note = "" } = props;
  const renderItem = ({ item, index }) => {
    return (
      <TouchableOpacity
        style={[
          styles.image,
          {
            marginRight: index == listImage.length - 1 ? 10 : 5
          }
        ]}
        onPress={() =>
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.IMAGE_VIEWER, {
            index,
            data: listImage.map(e => ({ url: e.image }))
          })
        }
      >
        <FastImage
          source={{ uri: item.image }}
          style={{ flex: 1, width: null, height: null, borderRadius: 3 }}
          resizeMode="stretch"
        />
      </TouchableOpacity>
    );
  };

  return (
    <View style={styles.containerScratch}>
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
      {!!listImage && (
        <FlatList
          style={{ marginHorizontal: 7, marginTop: 5 }}
          data={listImage}
          numColumns={3}
          renderItem={renderItem}
        />
      )}
      {note && !!note.length && (
        <Text
          style={{
            fontFamily: R.fonts.quicksand_regular,
            fontSize: 14,
            marginLeft: 15
          }}
        >
          {note}
        </Text>
      )}
    </View>
  );
};
const styles = StyleSheet.create({
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
  }
});
