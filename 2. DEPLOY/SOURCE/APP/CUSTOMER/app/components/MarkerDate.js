import React, { Component } from "react";
import { Text, View } from "react-native";
import { CalendarList } from "react-native-calendars";
import theme from "@theme";
import Button from "@app/components/Button";
import R from "@app/assets/R";
const ONE_DAY = 864e5;
function getFutureDates(startDay, endDay) {
  let start = Date.parse(startDay);
  let end = Date.parse(endDay);
  const array = {};
  let dateString = new Date(start).toISOString().split("T")[0];
  if (end == start) {
    array[dateString] = {
      color: theme.colors.calenderPicker,
      startingDay: true,
      endingDay: true
    };
    return array;
  }
  array[dateString] = { color: theme.colors.calenderPicker, startingDay: true };

  start += ONE_DAY;
  while (start != end) {
    dateString = new Date(start).toISOString().split("T")[0];
    start += ONE_DAY;
    array[dateString] = { color: theme.colors.calenderPicker };
  }
  dateString = new Date(start).toISOString().split("T")[0];
  array[dateString] = { color: theme.colors.calenderPicker, endingDay: true };
  return array;
}

export default class MarkerDate extends Component {
  state = {
    startDay: new Date().toISOString().split("T")[0],
    endDay: new Date().toISOString().split("T")[0]
  };

  render() {
    const { onPressDelete, onPressSelect } = this.props;
    const { endDay, startDay, onDayPress } = this.props;
    let currentDate = new Date().toISOString().split("T")[0];
    return (
      <View style={{ flex: 1 }}>
        <View style={{ flex: 1 }}>
          <CalendarList
            theme={{
              calendarBackground: theme.colors.white,
              selectedDayTextColor: theme.colors.white
            }}
            current={currentDate}
            pastScrollRange={24}
            futureScrollRange={24}
            markedDates={getFutureDates(startDay, endDay)}
            markingType={"period"}
            onDayPress={date => {
              onDayPress(date);
            }}
          />
        </View>
        <View
          style={{
            flexDirection: "row",
            justifyContent: "space-around"
          }}
        >
          <Button
            action={onPressDelete}
            title={R.strings().delete}
            backgroundColor={theme.colors.white}
            borderColor={theme.colors.red}
            colorText={theme.colors.red}
            borderWidth={1}
            buttonStyle={{ paddingVertical: 10 }}
          />
          <Button
            action={() => onPressSelect(this.state)}
            title={R.strings().select}
            backgroundColor={theme.colors.white}
            borderColor={theme.colors.textColor}
            colorText={theme.colors.textColor}
            borderWidth={1}
          />
        </View>
      </View>
    );
  }
}
