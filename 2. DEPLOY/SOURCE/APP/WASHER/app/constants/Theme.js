import { Dimensions, Platform, StatusBar, StyleSheet } from "react-native";
const dimension = ({ width, height } = Dimensions.get("window"));

const colors = {
  primary: "#0089CE",
  primaryDark: "#125183",
  primaryDark1: "#2E384D",
  bottombarBg: "#fafafa",
  headerColor: "#0089CE",
  headerTitle: "#FFFF",
  active: "#1b75bc",
  defaultBg: "#F5F6FA",
  inactive: "#DBDBDB",
  indicator: "#24277e",
  borderTopColor: "#dedede",
  nameText: "#5D5D5D",
  backgroundColor: "#F5F6FA",
  gray: "#707070",
  backgroundGray: "#f5f5f5",
  grayPlus: "#999999",
  grayButton: "#e7e7e7",
  grayBorder: "#c5c5c5",
  white: "#ffffff",
  textColor: "#003E5F",
  red: "#FD0000",
  calenderPicker: "#EBF6FE",
  grayDivide: "#B6B6B6",
  black: "#000000",
  orange: "#FDD600",
  green: "#15D108",
  greenLight: "#00CE4C",
  darkBlue: "#003E5F",
  backgroundCircle: "#f7f7f7",
  backgroundColorButtonGray: "#c9c9c9",
  backgroundZalo: "#0068FF",
  text_input: "#f3f3f3",
  orange_text: "#F97E2C",
  select: "#E4FAFF"
};

const sizes = {};

const styles = StyleSheet.create({});

export { colors, sizes, styles, dimension };
const theme = { colors, sizes, styles, dimension };
export default theme;
