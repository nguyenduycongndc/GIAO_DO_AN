import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  GestureResponderEvent,
  TouchableOpacity,
  View
} from "react-native";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
interface Props {
  item?: any;
  selected?: boolean;
  onPress?: (e: any) => void;
  onPressDelete?: (e: any) => void;
}
export default class BankAccount extends Component<Props> {
  render() {
    const { item: e, selected, onPress, onPressDelete } = this.props;
    return (
      <TouchableOpacity
        disabled={!onPress}
        onPress={() => onPress(e)}
        style={[
          styles.item_bank_account,
          {
            backgroundColor: selected ? colors.select : colors.white
          }
        ]}
        children={
          <>
            <View
              style={{ flexDirection: "row" }}
              children={
                <>
                  <Text style={styles.text_bank_name} children={e.bankName} />
                  {onPressDelete && (
                    <TouchableOpacity
                      onPress={() => onPressDelete(e)}
                      style={styles.root_text_remove}
                      children={
                        <Text style={styles.text_remove} children="Xoá" />
                      }
                    />
                  )}
                </>
              }
            />
            <Text style={styles.text_info_account} children={e.acount} />
            <Text style={styles.text_info_account} children={e.acountOwner} />
            <Text style={styles.text_info_account} children={e.code} />
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  item_bank_account: {
    borderRadius: 5,
    borderWidth: 0.5,
    padding: 15,
    marginVertical: 2.5,
    marginHorizontal: 10,
    marginTop: 10,
    backgroundColor: colors.white
  },
  text_bank_name: {
    color: colors.primary,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    marginBottom: 10
  },
  text_info_account: {
    color: colors.gray,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginVertical: 1
  },
  text_remove: {
    color: colors.red,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  root_text_remove: {
    position: "absolute",
    right: 0,
    paddingHorizontal: 10
  }
});
