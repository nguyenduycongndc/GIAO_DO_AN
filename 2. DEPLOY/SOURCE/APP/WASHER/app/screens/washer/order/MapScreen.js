import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Keyboard,
  TouchableOpacity,
  Linking,
  Platform
} from "react-native";
import MapView, {
  Marker,
  PROVIDER_GOOGLE,
  PROVIDER_DEFAULT
} from "react-native-maps";
import Geolocation from "react-native-geolocation-service";
import ScreenComponent from "@app/components/ScreenComponent";
import reactotron from "reactotron-react-native";
import R from "@app/assets/R";
import FastImage from "react-native-fast-image";
import { showMessages } from "@app/components/Alert";
import { connect } from "react-redux";

class MapScreen extends Component {
  constructor(props) {
    super(props);
    const item = this.props.navigation.getParam("item");
    this.state = {
      region: {
        latitude: item.lati,
        longitude: item.longi,
        latitudeDelta: 0.01,
        longitudeDelta: 0.01
      }
    };
  }

  render() {
    const item = this.props.navigation.getParam("item");
    return (
      <ScreenComponent
        titleHeader={R.strings().customer_location}
        back
        renderView={
          <View style={{ flex: 1 }}>
            <MapView
              ref={ref => {
                this.map = ref;
              }}
              onTouchStart={Keyboard.dismiss}
              showsUserLocation
              provider={PROVIDER_GOOGLE} // remove if not using Google Maps
              style={[styles.map]}
              region={this.state.region}
            >
              <Marker
                onDragEnd={this.onDragEnd}
                draggable
                title={`${R.strings().customer} - ${item.customerName}`}
                // description = {item.customer}
                coordinate={{
                  latitude: item.lati,
                  longitude: item.longi
                }}
              />
            </MapView>

            <View
              style={{
                position: "absolute",
                right: 10,
                bottom: 10
              }}
            >
              <TouchableOpacity onPress={() => this.gotToMyLocation()}>
                <FastImage
                  style={{
                    width: 60,
                    height: 60,
                    borderRadius: 30,
                    marginBottom: 15
                  }}
                  source={R.images.ic_my_location}
                />
              </TouchableOpacity>

              <TouchableOpacity
                onPress={() => {
                  Linking.openURL(
                    `https://www.google.com/maps/dir/?api=1&destination=${
                      item.lati
                    },${item.longi}&travelmode=driving`
                  );
                }}
              >
                <FastImage
                  style={{ width: 60, height: 60, borderRadius: 30 }}
                  source={R.images.ic_go_direction}
                />
              </TouchableOpacity>
            </View>
          </View>
        }
      />
    );
  }

  componentDidMount() {
    if (Platform.OS === "ios") Geolocation.requestAuthorization();
  }

  gotToMyLocation() {
    if (Platform.OS === "ios") {
      let { lati, longi } = this.props.locationState;
      if (lati && longi)
        this.map.animateCamera({
          center: {
            longitude: longi,
            latitude: lati
          }
        });
      else
        showMessages(
          R.strings().notice,
          R.strings().please_turn_on_device_location
        );
    } else
      Geolocation.getCurrentPosition(
        ({ coords }) => {
          this.map.animateCamera({
            center: coords
          });
        },
        error => {
          showMessages(
            R.strings().notice,
            R.strings().please_turn_on_device_location
          );
        },
        { enableHighAccuracy: true, timeout: 15000, maximumAge: 10000 }
      );
  }
}

const styles = StyleSheet.create({
  map: {
    ...StyleSheet.absoluteFillObject,
    flex: 1
  }
});
const mapStateToProps = state => ({
  locationState: state.locationReducer
});

const mapDispatchToProps = {};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(MapScreen);
