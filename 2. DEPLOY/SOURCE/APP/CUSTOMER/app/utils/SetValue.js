export default function setValue(defaultValue, obj, ...value) {
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
