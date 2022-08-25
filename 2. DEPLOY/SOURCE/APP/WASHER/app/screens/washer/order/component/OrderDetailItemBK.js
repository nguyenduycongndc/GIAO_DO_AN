import React, { Component } from "react";
import { Text, StyleSheet, View, Image } from "react-native";
import R from "@app/assets/R";
import theme from "@app/constants/Theme";
import reactotron from "reactotron-react-native";
import { renStatus, numberWithCommas } from "@app/constants/Functions";

export class UserItem extends Component {
  render() {
    const { item } = this.props;
    return (
      <View style={styles.container}>
        <View style={{
          flexDirection: "row",
          justifyContent: 'space-between'
        }}>
          <View style={{ flexDirection: "row" }}>
            <Text style={styles.txtGray}>{R.strings().invoice}</Text>
            <Text style={[styles.txtBold, { color: "black", marginLeft: 4 }]}>
              {item.code}
            </Text>
          </View>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              color: theme.colors.primary
            }}
          >{renStatus(item.status)}</Text>
        </View>
        <View style={styles.lines} />

        <View style={{
          flexDirection: "row",
          alignItems: 'center'
        }}>
          <Image
            style={styles.imgAvatar}
            source={
              item.customerAvatar
                ? { uri: item.customerAvatar }
                : R.images.ic_user
            }
          />
          <View style={{ marginLeft: 15 }}>
            <Text style={styles.txtGray}>{item.customerName}</Text>
            <Text style={styles.txtGray}>{item.customerPhone}</Text>
            <Text style={styles.txtGray}>{item.customerAddress}</Text>
          </View>
        </View>
        <View style={styles.lines} />
      </View >
    );
  }
}

const styles = StyleSheet.create({
  container: {
    width: "100%",
    backgroundColor: "white",
    paddingHorizontal: 8,
    paddingVertical: 12
  },
  txtGray: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.nameText
  },
  txtBold: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_bold
  },
  lines: {
    height: 0.5,
    width: width,
    backgroundColor: theme.colors.grayBorder,
    marginTop: 10
  },
  imgAvatar: {
    width: 48,
    height: 48,
    marginLeft: 8,
    borderRadius: 24
    // resizeMode: "contain"
  }
});
