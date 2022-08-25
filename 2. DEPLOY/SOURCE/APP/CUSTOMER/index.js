/**
 * @format
 */

import { AppRegistry } from "react-native";
import App from "./App";
import { name as appName } from "./app.json";
import "intl";

AppRegistry.registerComponent(appName, () => App);
