import R from "@app/assets/R";

export default function setValue(obj, ...value) {
  var defaultValue = R.strings().not_already_update;
  try {
    var val = obj;
    value.map(e => {
      val = val[e];
    });
    return val;
  } catch (error) {
    return defaultValue;
  }
}
