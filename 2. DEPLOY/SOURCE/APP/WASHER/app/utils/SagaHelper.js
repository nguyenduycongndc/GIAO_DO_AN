import { put, call } from "redux-saga/effects";
import reactotron from "reactotron-react-native";
// const watch = takeEvery(
//   GET_ACTION,
//   APINetwork,
//   API.request,
//   (action) => console.log("Trước lúc call"),
//   (action) => console.log("call api và chưa put success"),
//   (action) => console.log("put success")
// );
export default function* APINetwork(
  request,
  beforeCallApi,
  betweenCallApiAndPutSuccess,
  afterPutSuccess,
  action
) {
  const act = action
    ? action
    : afterPutSuccess
    ? afterPutSuccess
    : betweenCallApiAndPutSuccess
    ? betweenCallApiAndPutSuccess
    : beforeCallApi;
  // console.log(act);

  try {
    if (typeof beforeCallApi == "function") beforeCallApi(act);
    const res = yield call(request, act.payload);
    if (typeof betweenCallApiAndPutSuccess == "function")
      betweenCallApiAndPutSuccess(act);
    yield put({
      type: `${act.type}_success`,
      payload: res.result,
      params: act.payload?.status,
      body: act.payload
    });
    if (typeof afterPutSuccess == "function") afterPutSuccess(act);
  } catch (error) {
    console.log("erroR", error);
    yield put({ type: `${act.type}_fail`, payload: error });
  }
}
