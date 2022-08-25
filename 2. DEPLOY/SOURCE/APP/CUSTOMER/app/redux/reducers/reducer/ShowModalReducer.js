import {
  SHOW_REQUIRE_LOGIN,
  HIDEN_REQUIRE_LOGIN,
  SHOW_CONFIRM,
  SHOW_MESSAGE,
  HIDE_CONFIRM,
  HIDE_MESSAGE
} from "@app/redux/actions/type";
import R from "@app/assets/R";

const initialState = {
  visiable: false,
  showMessage: {
    visiable: false,
    title: R.strings().notif_tab_cus,
    content: "",
    action: null
  },
  showConfirm: {
    visiable: false,
    title: R.strings().notif_tab_cus,
    content: "",
    action: null,
    textCancel: R.strings().cancel,
    textConfirm: R.strings().confirm
  }
};

export default function(state = initialState, action) {
  const isSubmit = action?.isSubmit;
  switch (action.type) {
    case SHOW_REQUIRE_LOGIN: {
      return {
        ...state,
        visiable: true
      };
    }
    case SHOW_CONFIRM: {
      return {
        ...state,
        showConfirm: {
          ...action.payload,
          visiable: true
        }
      };
    }
    case SHOW_MESSAGE: {
      return {
        ...state,
        showMessage: {
          ...action.payload,
          visiable: true
        }
      };
    }
    case HIDEN_REQUIRE_LOGIN: {
      return {
        ...state,
        visiable: false
      };
    }
    case HIDE_MESSAGE: {
      return {
        ...state,
        showMessage: {
          ...action.payload,
          visiable: false
        }
      };
    }
    case HIDE_CONFIRM: {
      return {
        ...state,
        showConfirm: {
          ...action.payload,
          visiable: false
        },
        isSubmit
      };
    }
    default:
      return state;
  }
}
