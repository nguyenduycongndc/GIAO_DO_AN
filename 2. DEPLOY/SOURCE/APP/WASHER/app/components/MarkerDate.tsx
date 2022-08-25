import React, { Component } from "react";
import { Text, View } from "react-native";
import { CalendarList } from "react-native-calendars";
import theme from "@theme";
import Button from "@app/components/Button";
import R from "@app/assets/R";
import reactotron from "reactotron-react-native";
const ONE_DAY = 864e5;
const defaultPropsMarking = {
  textColor: "black",
  startingDay: false,
  color: "black",
  selected: false,
  endingDay: false,
  disabled: false
};
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
  array[dateString] = {
    color: theme.colors.calenderPicker,
    startingDay: true
  };

  start += ONE_DAY;
  while (start != end) {
    dateString = new Date(start).toISOString().split("T")[0];
    start += ONE_DAY;
    array[dateString] = { color: theme.colors.calenderPicker };
  }
  dateString = new Date(start).toISOString().split("T")[0];
  array[dateString] = {
    color: theme.colors.calenderPicker,
    startDay: true,
    endingDay: true
  };
  return array;
}

interface Props {
  onPressSelect?: (startDay: string, endDay: string) => void;
  onPressDelete?: () => void;
  endDay?: string;
  startDay?: string;
  onDayPress?: () => void;
}

export default class MarkerDate extends Component<Props> {
  state = {
    startDay: this.props.startDay ? this.props.startDay : this.getCurrentDay(),
    endDay: this.props.endDay ? this.props.endDay : this.getCurrentDay()
  };
  setDay(startDay, endDay) {
    this.setState({
      ...this.state,
      endDay: endDay,
      startDay: startDay
    });
  }
  getCurrentDay() {
    return new Date().toISOString().split("T")[0];
  }
  render() {
    const { onPressDelete, onPressSelect } = this.props;
    const { endDay, startDay } = this.state;
    let currentDate = new Date().toISOString().split("T")[0];
    const array = getFutureDates(startDay, endDay);
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
              // console.log(date.dateString);
              var dateValue = Date.parse(date.dateString);
              if (
                dateValue == Date.parse(startDay) ||
                dateValue == Date.parse(endDay)
              ) {
                this.setDay(date.dateString, date.dateString);
              } else if (dateValue < Date.parse(startDay)) {
                this.setDay(date.dateString, endDay);
              } else {
                this.setDay(startDay, date.dateString);
              }
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
            action={() => onPressSelect(startDay, endDay)}
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
