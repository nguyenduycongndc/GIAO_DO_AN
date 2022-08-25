import React, { Component } from "react";
import {
  Image,
  StyleSheet,
  TouchableOpacity,
  View,
  Text,
  ScrollView
} from "react-native";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import I18n from "@i18";
import Icon from "@app/components/Icon";
import RateStar from "@app/components/RateStar";
import NumberFormat from "react-number-format";
import FastImage, { Avatar } from "@app/components/FastImage";
export class RowImageLable extends Component {
  render() {
    const {
      lable,
      image,
      position = "left",
      size = 14,
      disableTouch = true,
      onPress,
      style,
      textStyle,
      textColor = theme.colors.textColor
    } = this.props;
    const loadingImage = image ? (
      image
    ) : (
      <FastImage
        style={{ width: size, height: size }}
        source={R.images.Icon_diamond_medal}
      />
    );
    return (
      <TouchableOpacity
        style={[
          {
            flexDirection: "row",
            alignItems: "center"
          },
          style
        ]}
        disabled={disableTouch}
        onPress={onPress}
      >
        {position == "left" && loadingImage}
        <Text
          style={[
            {
              fontFamily: R.fonts.quicksand_medium,
              fontSize: size,
              marginLeft: 5,
              marginRight: 5,
              color: textColor
            },
            textStyle
          ]}
        >
          {lable}
        </Text>
        {position == "right" && loadingImage}
      </TouchableOpacity>
    );
  }
}

export class RowCarInfor extends Component {
  render() {
    const {
      name = "",
      licensePlate = "",
      image = R.images.carImg
    } = this.props;
    return (
      <View
        style={{
          flexDirection: "row",
          backgroundColor: colors.white,
          marginVertical: 5,
          padding: 10
        }}
        children={
          <>
            <FastImage
              source={image}
              style={{ width: width / 7, height: width / 7, borderRadius: 10 }}
            />
            <View
              style={{
                alignSelf: "center",
                paddingHorizontal: 10
              }}
              children={
                <>
                  <Text
                    style={{
                      fontFamily: R.fonts.quicksand_bold,
                      fontSize: 20,
                      color: colors.primary,
                      padding: 2.5
                    }}
                    children={name}
                  />
                  <Text
                    style={{
                      fontFamily: R.fonts.quicksand_medium,
                      fontSize: 20,
                      color: colors.primary,
                      padding: 2.5
                    }}
                    children={licensePlate}
                  />
                </>
              }
            />
          </>
        }
      />
    );
  }
}

export class Dropdown extends Component {
  state = {
    show: false,
    select: -1
  };

  renderList = list =>
    list.map((item, index) => (
      <TouchableOpacity
        onPress={() => this.onPress(index, item)}
        style={[
          {
            top: 40 * index - 1 + 30,
            position: "absolute",
            backgroundColor: theme.colors.white,
            width: "95%",
            padding: 10,
            marginHorizontal: 10,
            borderWidth: 0.3,
            borderRadius: 5,
            overflow: "hidden"
          },
          this.props.styleDropdown
        ]}
        children={
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14
            }}
            children={item}
          />
        }
      />
    ));
  onPress = (index, item) =>
    this.setState({ show: false, select: index }, () =>
      this.props.onSelect(index, item)
    );
  switch = () => this.setState({ show: !this.state.show });
  render() {
    const { show } = this.state;
    const {
      data,
      style,
      defaultValue,
      lable = R.strings().address
    } = this.props;
    return (
      <>
        <RowImageLable
          onPress={this.switch}
          disableTouch={false}
          lable={lable}
          size={12}
          position="right"
          textColor={theme.colors.nameText}
          image={<Icon.MaterialIcons name="keyboard-arrow-down" size={12} />}
        />
        {show && this.renderList(data)}
      </>
    );
  }
}
export class AvatarInfo extends Component {
  render() {
    const {
      name,
      code,
      avatarSource,
      showRateStar = null,
      rateStarBottom = false,
      borderBottom,
      style,
      address
    } = this.props;

    return (
      <View>
        <View
          style={[
            style,
            {
              flexDirection: "row",
              alignItems: "center",
              padding: 8,
              paddingVertical: 15,
              backgroundColor: theme.colors.white
            }
          ]}
        >
          <Avatar source={avatarSource} />
          <View style={{ marginHorizontal: 13, flex: 1 }}>
            <View style={{ flexDirection: "row", width: "100%" }}>
              <Text style={[styles.textInfo, { flex: 1 }]}>{name}</Text>
              {showRateStar != null && !rateStarBottom && (
                <RateStar
                  readonly
                  numberStar={showRateStar}
                  size={15}
                  isShowNumber
                />
              )}
            </View>
            <Text style={styles.textInfo}>
              {code || R.strings().not_update_yet}
            </Text>
            <Text style={styles.textInfo}>
              {address || R.strings().not_update_yet}
            </Text>
            {/* {showRateStar != null && rateStarBottom ? (
              <RateStar numberStar={showRateStar} size={15} isShowNumber />
            ) : (
            )} */}
          </View>
        </View>
        {borderBottom && (
          <View
            style={{
              marginHorizontal: 5,
              height: 0.5,
              backgroundColor: theme.colors.gray
            }}
          />
        )}
      </View>
    );
  }
}

export class RowAvatarInfo extends Component {
  render() {
    const {
      onPress,
      header, // {code,status}
      info, // {name, phoneNumber, address, avatarSource}
      dateTime,
      carInfo, // {name,licensePlate }
      washerInfo, //{avatarSource, name , code , address, star, positionStar(vi tri sao o trong man hinh)}
      editTime
    } = this.props;
    return (
      <View style={styles.view} onPress={onPress}>
        {header && (
          <View
            style={{
              flexDirection: "row",
              paddingVertical: 10,
              paddingHorizontal: 10,
              backgroundColor: theme.colors.white
            }}
          >
            <Text style={[styles.textInfo, { flex: 1 }]}>
              {R.strings().bill}
              <Text style={{ fontWeight: "bold", flex: 1 }}>{` ${
                header.code
              }`}</Text>
            </Text>
            <Text
              style={[
                styles.textInfo,
                { color: header.textColor, fontWeight: "bold" }
              ]}
            >
              {header.status}
            </Text>
          </View>
        )}
        {header && (
          <View
            style={{
              marginHorizontal: 9,
              height: 1,
              backgroundColor: theme.colors.grayBorder
            }}
          />
        )}
        <AvatarInfo
          avatarSource={info.avatarSource}
          code={info.name}
          address={info.numberPhone}
          name={info.address}
        />
        <View
          style={{
            marginHorizontal: 9,
            height: 1,
            backgroundColor: theme.colors.grayBorder
          }}
        />
        <RowImageLable
          style={{
            padding: 10,
            backgroundColor: theme.colors.white
          }}
          lable={dateTime}
          disableTouch={!editTime}
          onPress={editTime}
          image={
            <FastImage
              style={{ width: 14, height: 14 }}
              source={R.images.ic_clock}
            />
          }
        />

        <WasherInfo carInfo={carInfo} washerInfo={washerInfo} />
      </View>
    );
  }
}

export class WasherInfo extends Component {
  render() {
    const {
      carInfo, // {name,licensePlate }
      washerInfo //{avatarSource, name , code , address, star, positionStar(vi tri sao o trong man hinh)}
    } = this.props;
    return (
      <View>
        {washerInfo && (
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              paddingHorizontal: 10,
              marginTop: 15
            }}
          >
            {R.strings().washer_info}
          </Text>
        )}
        {washerInfo && (
          <AvatarInfo
            avatarSource={washerInfo?.avatarSource}
            code={washerInfo?.code}
            name={washerInfo?.name}
            showRateStar={washerInfo?.star}
            rateStarBottom={washerInfo?.positionStar}
            style={{
              marginTop: 15
            }}
          />
        )}
        {carInfo && (
          <View
            style={{
              flexDirection: "row",
              alignItems: "center",
              paddingHorizontal: 7,
              paddingVertical: 7,
              backgroundColor: theme.colors.white,
              marginVertical: 15
            }}
          >
            <FastImage
              source={carInfo.carImage}
              style={{ width: 55, height: 55, borderRadius: 7 }}
            />
            <View
              style={{
                marginHorizontal: 13,
                justifyContent: "space-around",
                paddingVertical: 10
              }}
            >
              <Text style={[styles.carInfo, { fontWeight: "bold" }]}>
                {carInfo.carModel}
              </Text>
              <Text style={styles.carInfo}>{carInfo.licensePlate}</Text>
            </View>
          </View>
        )}
      </View>
    );
  }
}

export class RowLableInfo extends Component {
  render() {
    const {
      onPress,
      lable,
      title,
      borderTop,
      borderBottom,
      textSize = 14,
      color = colors.gray,
      style,
      titleStyle,
      lableStyle
    } = this.props;
    return (
      <>
        {borderTop && (
          <View
            style={{
              marginHorizontal: 5,
              height: 0.25,
              backgroundColor: theme.colors.gray
            }}
          />
        )}
        <View style={[styles.rowLableInfo, style]}>
          <Text
            style={[
              {
                fontFamily: R.fonts.quicksand_medium,
                fontSize: textSize,
                color: color,
                flex: 1
              },
              lableStyle
            ]}
          >
            {lable}
          </Text>
          <NumberFormat
            value={title}
            displayType="text"
            thousandSeparator
            suffix="đ"
            prefix={title > 0 && "+"}
            renderText={value => (
              <Text
                style={[
                  {
                    fontFamily: R.fonts.quicksand_medium,
                    fontSize: textSize,
                    color: value ? color : colors.grayBorder
                  },
                  titleStyle
                ]}
                children={value || R.strings().none}
              />
            )}
          />
          {/* <Text
            style={[
              {
                fontFamily: R.fonts.quicksand_medium,
                fontSize: textSize,
                color: color
              },
              titleStyle
            ]}
          >
            {title}
          </Text> */}
        </View>
        {borderBottom && (
          <View
            style={{
              marginHorizontal: 9,
              height: 1,
              backgroundColor: theme.colors.grayBorder
            }}
          />
        )}
      </>
    );
  }
}

export class RowLableInfoText extends Component {
  render() {
    const {
      onPress,
      lable,
      title,
      borderTop,
      borderBottom,
      textSize = 14,
      color = colors.gray,
      style,
      titleStyle,
      lableStyle
    } = this.props;
    return (
      <>
        {borderTop && (
          <View
            style={{
              marginHorizontal: 5,
              height: 0.25,
              backgroundColor: theme.colors.gray
            }}
          />
        )}
        <View style={[styles.rowLableInfo, style]}>
          <Text
            style={[
              {
                fontFamily: R.fonts.quicksand_medium,
                fontSize: textSize,
                color: color,
                flex: 1
              },
              lableStyle
            ]}
          >
            {lable}
          </Text>
          <Text
            style={[
              {
                fontFamily: R.fonts.quicksand_medium,
                fontSize: textSize,
                color: title ? color : colors.grayBorder,
                maxWidth: width / 2,
                textAlign: "right"
              },
              titleStyle
            ]}
            children={title || R.strings().none}
          />
          {/* <Text
            style={[
              {
                fontFamily: R.fonts.quicksand_medium,
                fontSize: textSize,
                color: color
              },
              titleStyle
            ]}
          >
            {title}
          </Text> */}
        </View>
        {borderBottom && (
          <View
            style={{
              marginHorizontal: 9,
              height: 1,
              backgroundColor: theme.colors.grayBorder
            }}
          />
        )}
      </>
    );
  }
}
const styles = StyleSheet.create({
  view: {
    width: "100%",
    backgroundColor: theme.colors.defaultBg
  },
  textInfo: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.nameText
  },
  carInfo: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 20,
    color: theme.colors.primary
  },
  rowLableInfo: {
    flexDirection: "row",
    padding: 10,
    paddingVertical: 12,
    backgroundColor: theme.colors.white
  }
});
