package com.car.rect.customer;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.facebook.react.ReactActivity;
import com.facebook.react.ReactInstanceManager;
import com.facebook.react.bridge.Arguments;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContext;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;

public class MainActivity extends ReactActivity {

    /**
     * Returns the name of the main component registered from JavaScript. This is used to schedule
     * rendering of the component.
     */
    @Override
    protected String getMainComponentName() {
        return "CarRect";
    }

    @Override
    protected void onResume() {
        super.onResume();
        Intent intent = getIntent();
        Uri data = intent.getData();
        if (data != null) {
            WritableMap params = Arguments.createMap();
            ReactInstanceManager mReactInstanceManager = getReactNativeHost().getReactInstanceManager();
            ReactApplicationContext context = (ReactApplicationContext) mReactInstanceManager.getCurrentReactContext();
            mReactInstanceManager.addReactInstanceEventListener(validContext -> {
                validContext.getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                        .emit("SEND_URI", data.toString());
            });
        }
    }
}
