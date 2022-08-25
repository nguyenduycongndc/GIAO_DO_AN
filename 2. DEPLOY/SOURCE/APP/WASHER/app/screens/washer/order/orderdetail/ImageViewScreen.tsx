import React, { Component } from "react";
import {
  Text,
  Modal,
  Dimensions,
  Image,
  TouchableOpacity,
  Platform
} from "react-native";
import { Icon } from "react-native-elements";
import ImageViewer from "react-native-image-zoom-viewer";
import reactotron from "reactotron-react-native";
import Props from "@app/utils/Props";
import NavigationUtil from "@app/navigation/NavigationUtil";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import FastImage from "react-native-fast-image";
import { colors } from "@app/constants/Theme";
const { height, width } = Dimensions.get("window");

type ImageViewerProps = Props & {
  navigation: {
    state: { params: Params };
  };
};
interface State {
  index: number;
  images: (any)[];
  isShowing: boolean;
}
export default class ImageViewerScreen extends Component<
  ImageViewerProps,
  State
> {
  constructor(props: ImageViewerProps) {
    super(props);
    var { index = 0, uris, data } = props.navigation.state.params;
    var images = [];
    if (data) images = data;
    else {
      const map = uris.map(({ after, before }) => ({
        urlAfter: after.url,
        urlBefore: before.url
      }));
      images = toArray(
        map.map(e => [
          Object.values(e)[1] != null && { url: Object.values(e)[1] },
          Object.values(e)[0] != null && { url: Object.values(e)[0] }
        ])
      );
      const currentImage = images[index];
      images = images.filter(e => e != false);
      index = images.indexOf(currentImage);
    }
    this.state = {
      index: index,
      images: images,
      isShowing: true
    };
  }

  setTime = setTimeout(() => {
    this.setState({ isShowing: false });
  }, 1000);

  setTimeoutShow = () => {
    clearTimeout(this.setTime);
    this.setTime = setTimeout(() => {
      this.setState({ isShowing: false });
    }, 2000);
  };

  render() {
    const { index, images, isShowing } = this.state;
    const { renderFooter } = this.props.navigation.state.params;
    return (
      <>
        <ImageViewer
          onClick={() =>
            this.setState({ isShowing: true }, () => this.setTimeoutShow())
          }
          imageUrls={images}
          menus={elem => {
            elem.cancel();
            return <></>;
          }}
          renderIndicator={(curIndex, size) =>
            isShowing && (
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_bold,
                  color: colors.white,
                  position: "absolute",
                  top: 45,
                  left: width / 2 - 15,
                  fontSize: 22
                }}
                children={`${curIndex}/${size}`}
              />
            )
          }
          renderFooter={index => isShowing && renderFooter}
          enablePreload
          renderImage={props => <FastImage {...props} />}
          index={index}
        />

        {isShowing && (
          <TouchableOpacity
            style={{
              position: "absolute",
              top: 50,
              left: 10
            }}
            onPress={NavigationUtil.goBack}
          >
            <Icon type="ion-icon" name="arrow-back" size={30} color="#FFF" />
          </TouchableOpacity>
        )}
      </>
    );
  }
}
/* Recursion Method */
function toArray(array) {
  var result = [];

  function toarray(array) {
    for (var l = array.length, i = 0; i < l; i++) {
      if (Array.isArray(array[i])) {
        toarray(array[i]);
      } else {
        result.push(array[i]);
      }
    }
    return result;
  }

  return toarray(array);
}
export interface Params {
  index: number;
  uris?: (UrisEntity)[] | null;
  renderFooter?: JSX.Element;
  data?: (UrisImagePicker)[] | null;
}
export interface UrisEntity {
  name: string;
  before: BeforeOrAfter;
  after: BeforeOrAfter;
  order: number;
}
export interface UrisImagePicker {
  url: string;
}
export interface BeforeOrAfter {
  imageRequireID: number;
  url: string;
  dateStr: string;
  date?: null;
}
