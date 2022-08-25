import React, { Component } from "react";
import { Text, View, TouchableOpacity, StyleSheet } from "react-native";
import R from "@R";
import theme from "@theme";

export default class RadioButtons extends Component {
  state = {
    value: null,
    check: 0
  };

  render() {
    const { options } = this.props;
    const { value, check } = this.state;
    return (
      <View style={styles.container}>
        {options.map(item => {
          return (
            <View key={[item.key]} style={styles.buttonContainer}>
              <TouchableOpacity
                style={{ flexDirection: "row" }}
                onPress={() => {
                  this.setState({
                    value: item.key,
                    check: item.key == R.strings().indoor ? 1 : 0
                  });
                }}
              >
                <View style={styles.circle}>
                  {value === item.key && <View style={styles.checkedCircle} />}
                </View>
                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_bold,
                    fontSize: 13,
                    marginLeft: 20
                  }}
                >
                  {item.text}
                </Text>
              </TouchableOpacity>
            </View>
          );
        })}
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    justifyContent: "flex-start",
    alignItems: "center",
    paddingTop: 12,
    marginBottom: 12,
    marginLeft: 7
  },
  buttonContainer: {
    flexDirection: "row",
    justifyContent: "space-around",
    alignItems: "center",
    // marginBottom: 30,
    marginRight: 30
  },

  circle: {
    height: 17,
    width: 17,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: theme.colors.gray,
    alignItems: "center",
    justifyContent: "center"
  },

  checkedCircle: {
    width: 12,
    height: 12,
    borderRadius: 7,
    backgroundColor: theme.colors.primary
  }
});
