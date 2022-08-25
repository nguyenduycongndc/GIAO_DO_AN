package com.car.rect.washer;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
// import com.vnpay.authentication.VNP_AuthenticationActivity;

public class SDKMerchantModule extends ReactContextBaseJavaModule {

    private static final int CALLBACK_CODE = 676;
    public SDKMerchantModule(ReactApplicationContext reactContext) {
        super(reactContext);
    }

    @Override
    public String getName() {
        return "SDKMerchantModule";
    }

    @ReactMethod
    public void openMerchantModule(String url) {
        Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
        browserIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        getReactApplicationContext().startActivity(browserIntent);
        // Intent intent = new Intent(getReactApplicationContext(), VNP_AuthenticationActivity.class);
        // intent.putExtra("url", url);
        // intent.putExtra("scheme", "washer");
        // intent.putExtra("tmn_code", "EVERGL01");
        // getReactApplicationContext().startActivityForResult(intent, CALLBACK_CODE, Bundle.EMPTY);
    }
}
