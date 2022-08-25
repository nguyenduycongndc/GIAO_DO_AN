import { SEND_LOCATION_SELECT, CLEAR_LOCATION_SELECT } from "@action/type";
import analytics from '@react-native-firebase/analytics';

const INITIAL_STATE = {
  location: "",
  name: "",
  lon: "",
  lat: ""
};

export default (state = INITIAL_STATE, action) => {
  switch (action.type) {
    case SEND_LOCATION_SELECT:
      analytics().logEvent('select_location_booking', {
        name: action.payload.name
      })
      return {
        ...state,
        location: action.payload.location,
        name: action.payload.name,
        lon: action.payload.lon,
        lat: action.payload.lat
      };
    case CLEAR_LOCATION_SELECT:
      return INITIAL_STATE;
    default:
      return state;
  }
};
