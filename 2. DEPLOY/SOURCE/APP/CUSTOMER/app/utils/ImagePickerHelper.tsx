import ImagePicker, { ImagePickerOptions } from "react-native-image-picker";
import ImageResizer from "react-native-image-resizer";
import { Dimensions } from "react-native";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import reactotron from "@app/reactotron/ReactotronConfig";
const maxWidth = Dimensions.get("screen").width;
const maxHeight = Dimensions.get("screen").height;
// export const options: ImagePickerOptions = {

// };
const imagePickerHelper = res => {
  const options = {
    title: R.strings().select,
    cancelButtonTitle: R.strings().cancel,
    chooseFromLibraryButtonTitle: R.strings().from_library,
    takePhotoButtonTitle: R.strings().take_photo,
    storageOptions: {
      skipBackup: true,
      path: "images"
    },
    tintColor: colors.black
  };
  try {
    ImagePicker.showImagePicker(options, response => {
      if (response.didCancel) {
        console.log("User cancelled image picker");
      } else if (response.error) {
        console.log("ImagePicker Error: ", response.error);
      } else if (response.customButton) {
        console.log("User tapped custom button: ", response.customButton);
      } else {
        var actualWidth = response.width,
          actualHeight = response.height;
        var imgRatio = actualWidth / actualHeight;
        var maxRatio = maxHeight / maxHeight;
        if (actualHeight > maxHeight || actualWidth > maxWidth) {
          if (imgRatio < maxRatio) {
            imgRatio = maxHeight / actualHeight;
            actualWidth = imgRatio * actualWidth;
            actualHeight = maxHeight;
          } else if (imgRatio > maxRatio) {
            imgRatio = maxWidth / actualWidth;
            actualHeight = imgRatio * actualHeight;
            actualWidth = maxWidth;
          } else {
            actualHeight = maxHeight;
            actualWidth = maxWidth;
          }
        }

        const source = { uri: response.uri };
        _resizeImage(source.uri, actualWidth, actualHeight, res);
      }
    });
  } catch (error) {
    alert(JSON.stringify(error));
  }
};

const _resizeImage = async (uri, actualWidth, actualHeight, res) => {
  var url = null;
  try {
    const response = await ImageResizer.createResizedImage(
      uri,
      actualWidth,
      actualHeight,
      "JPEG",
      70,
      0,
      ""
    );
    url = response.uri;
  } catch (error) {
    console.log(error);

    url = uri;
  }
  if (typeof res) res(uri);
};

export default imagePickerHelper;
