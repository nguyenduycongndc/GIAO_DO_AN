import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  TouchableOpacity,
  Dimensions
} from "react-native";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import { SCREEN_ROUTER_WASHER } from "@constant";
import FastImage from "react-native-fast-image";
import NavigationUtil from "@app/navigation/NavigationUtil";
import reactotron from "@app/reactotron/ReactotronConfig";
const { width, height } = Dimensions.get("screen");
import ProgressFastImage from "@app/components/ProgressFastImage";
import LoadableImage from "./LoadableImage";
interface Props {
  data?: (ListImageRequireEntity)[] | any | null;
}
interface ListImageRequireEntity {
  name: string;
  before: BeforeOrAfter;
  after: BeforeOrAfter;
  order: number;
}
interface BeforeOrAfter {
  imageRequireID: number;
  url: string;
  dateStr: string;
  date?: null;
}
export default (props: Props) => {
  return props.data.map((val, index) => (
    <View style={styles.view}>
      <View style={styles.root_big_dot}>
        <View style={styles.big_dot} />
        <Text style={styles.text_big_dot} children={val.name} />
      </View>
      {val && (
        <>
          {renderItem(val, index * 2, props.data)}
          {index != props.data.length - 1 && <View style={styles.end_line} />}
        </>
      )}
    </View>
  ));
};

function renderItem(
  value: ListImageRequireEntity,
  index,
  data?: (ListImageRequireEntity)[]
) {
  return (
    <>
      <View style={styles.root_small_dot}>
        <View style={styles.root_text_small_dot} />
      </View>
      <View style={styles.imgView}>
        {_itemImg(
          value.before.url,
          R.strings().before,
          value.before.dateStr,
          index,
          data
        )}
        {_itemImg(
          value.after.url,
          R.strings().after,
          value.after.dateStr,
          index + 1,
          data
        )}
      </View>
    </>
  );
}

function _itemImg(source, title, time, index, uris) {
  return (
    <TouchableOpacity
      onPress={() =>
        NavigationUtil.navigate(SCREEN_ROUTER_WASHER.IMAGE_VIEWER, {
          index,
          uris
        })
      }
      disabled={source == null}
      children={
        <>
          <LoadableImage
            style={styles.img_style}
            resizeMode={FastImage.resizeMode.cover}
            source={
              source != null
                ? { uri: source, priority: FastImage.priority.low }
                : R.images.bg_car_rect
            }
          />
          <View style={styles.img_item}>
            <Text style={styles.title} children={title} />
            {time.length != 0 && (
              <>
                <View style={styles.line_vertical} />
                <Text style={styles.time} children={time} />
              </>
            )}
          </View>
        </>
      }
    />
  );
}

const styles = StyleSheet.create({
  view: {
    marginHorizontal: 10
    // backgroundColor:'gray'
  },
  root_big_dot: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 4
  },
  imgView: {
    // height: 116,
    paddingHorizontal: 0,
    backgroundColor: theme.colors.white,
    borderRadius: 7,
    paddingVertical: 7,
    flexDirection: "row",
    justifyContent: "space-around",
    // position: "relative",
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,

    elevation: 5
  },
  img_item: {
    // width: 73,
    // height: 15,
    backgroundColor: "rgba(112,112,112,0.4)",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-around",
    borderRadius: 20,
    position: "absolute",
    bottom: 3,
    marginLeft: 3,
    padding: 5
  },
  big_dot: {
    width: 11,
    height: 11,
    backgroundColor: theme.colors.primary,
    borderRadius: 50,
    marginHorizontal: 5
  },
  root_small_dot: {
    flexDirection: "row",
    alignItems: "center",
    marginTop: 10,
    marginBottom: 7
  },
  small_dot: {
    width: 7,
    height: 7,
    backgroundColor: "black",
    borderRadius: 50,
    marginHorizontal: 7,
    position: "relative"
  },
  root_text_small_dot: {
    height: 35,
    width: 0.5,
    backgroundColor: "black",
    position: "absolute",
    marginLeft: 10.25
  },
  text_big_dot: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.primary
  },
  end_line: {
    height: 15,
    width: 0.5,
    backgroundColor: "black",
    marginLeft: 10.25,
    marginBottom: -5
  },
  img_style: {
    flex: 1,
    aspectRatio: 15 / 9,
    borderRadius: 5,
    height: height / 8
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
    height: "200%",
    backgroundColor: theme.colors.white
  },
  time: {
    fontSize: 10,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.white,
    textAlignVertical: "center"
  }
});
