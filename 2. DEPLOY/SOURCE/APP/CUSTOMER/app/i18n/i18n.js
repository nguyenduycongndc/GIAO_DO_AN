import I18n from 'react-native-i18n';

import en from './locales/en';
import vi from './locales/vi';

I18n.fallbacks = false;

I18n.translations = {
  vi,
  en
};
// I18n.locale = "en";
I18n.locale = "vi";
export default I18n;