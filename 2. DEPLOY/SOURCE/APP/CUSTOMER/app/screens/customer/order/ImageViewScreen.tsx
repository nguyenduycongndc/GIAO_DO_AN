import React, { Component } from "react";
import {
  Text,
  Modal,
  Dimensions,
  Image,
  TouchableOpacity,
  Platform,
  View,
  ActivityIndicator
} from "react-native";
import { Icon } from "react-native-elements";
import ImageViewer from "react-native-image-zoom-viewer";
import reactotron from "reactotron-react-native";
import Props from "@app/utils/Props";
import NavigationUtil from "@app/navigation/NavigationUtil";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import FastImage from "@app/components/FastImage";
import { colors } from "@app/constants/Theme";
import Loading from "@app/components/Loading";
// import DialogLoading from "@app/components/DialogLoading";
const { height, width } = Dimensions.get("screen");

type ImageViewerProps = Props & {
  navigation: {
    state: { params: Params };
  };
};
interface State {
  index: number;
  images: (any)[];
  loading;
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
      loading: true
    };
  }

  render() {
    const { index, images, loading } = this.state;
    const { renderFooter } = this.props.navigation.state.params;
    console.log(index);

    return (
      <>
        <ImageViewer
          imageUrls={images}
          menus={elem => {
            elem.cancel();
            return <></>;
          }}
          renderArrowLeft={() =>
            index != 0 && (
              <FastImage
                source={R.images.ic_left}
                style={{ width: 50, height: 50 }}
                tintColor="white"
              />
            )
          }
          onChange={index => {
            this.setState({ index });
          }}
          renderArrowRight={() =>
            index != images.length - 1 && (
              <FastImage
                source={R.images.ic_right}
                style={{ width: 50, height: 50 }}
                tintColor="white"
              />
            )
          }
          renderIndicator={(curIndex, size) => <Text />}
          enablePreload
          loadingRender={() => <ActivityIndicator />}
          renderImage={props => (
            <View
              style={{
                flexDirection: "column-reverse",
                flex: 1
              }}
            >
              {/* {loading && <DialogLoading />} */}
              <FastImage
                onLoadEnd={() => this.setState({ loading: false })}
                {...props}
                style={{ width: "100%", height: "97%", alignSelf: "center" }}
                resizeMode="contain"
              />
            </View>
          )}
          index={index}
        />

        <TouchableOpacity
          style={{
            position: "absolute",
            top: 35,
            left: 10,
            backgroundColor: "rgba(225,225,225,0.6)",
            padding: 5,
            borderRadius: 10,
            overflow: "hidden"
          }}
          onPress={NavigationUtil.goBack}
        >
          <Icon type="ion-icon" name="close" size={30} color="black" />
        </TouchableOpacity>
        {renderFooter}
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
