import React, { useState } from "react";
import {
  StyleSheet,
  Text,
  View,
  ActivityIndicator,
  TouchableOpacity,
  Image
} from "react-native";
import FastImage, { FastImageProps } from "react-native-fast-image";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";

const FastImg = (props: FastImageProps) => {
  const [imageLoad, useImageLoad] = useState(false);
  const [error, useEror] = useState(false);
  const [reloadKey, useReloadKey] = useState(new Date().getTime().toString());
  const reloadImage = () => {
    useReloadKey(new Date().getTime().toString());
  };
  var { source } = props;

  if (typeof source == "object")
    source = source.uri
      ? {
          ...source,
          priority: FastImage.priority.high,
          // uri:
          //   source.uri.split(":")[0] == "http" ||
          //   source.uri.split(":")[0] == "https"
          //     ? `${source.uri}?${reloadKey}`
          //   : `${source.uri}`
          uri: source.uri
        }
      : R.images.ic_symbol;
  return (
    <FastImage
      {...props}
      children={
        imageLoad ? (
          <View
            style={{
              backgroundColor: colors.grayBorder,
              flex: 1,
              overflow: "hidden"
            }}
            children={
              <ActivityIndicator
                color={colors.primary}
                style={{
                  flex: 1
                }}
              />
            }
          />
        ) : error ? (
          <TouchableOpacity
            style={{
              flex: 1,
              justifyContent: "center"
            }}
            children={
              <Image
                style={{
                  alignSelf: "center"
                }}
                source={R.images.ic_try_again}
                resizeMode="center"
              />
            }
            onPress={reloadImage}
          />
        ) : (
          props.children
        )
      }
      onLoadStart={() => {
        useEror(false);
        useImageLoad(true);
      }}
      onLoadEnd={() => {
        useImageLoad(false);
      }}
      onError={() => {
        useEror(true);
        useImageLoad(false);
      }}
      source={source}
    />
  );
};

type AvatarProps = FastImageProps & { onPress? };

export const Avatar = (props: AvatarProps) => {
  return (
    <TouchableOpacity
      disabled={!props.onPress}
      style={{ overflow: "hidden" }}
      onPress={props.onPress}
      children={
        <FastImg
          {...props}
          style={[{ width: 50, height: 50, borderRadius: 25 }, props.style]}
          resizeMode="contain"
        />
      }
    />
  );
};
export default FastImg;
