const CHANGE_LANGUAGE = "change_language";
module.exports = {
  CHANGE_LANGUAGE,
  changeLanguage: payload => ({
    type: CHANGE_LANGUAGE,
    payload: payload
  })
};
