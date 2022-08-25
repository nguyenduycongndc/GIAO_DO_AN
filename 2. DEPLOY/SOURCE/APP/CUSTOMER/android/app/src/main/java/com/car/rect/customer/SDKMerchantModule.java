package com.car.rect.customer;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;

public class SDKMerchantModule extends ReactContextBaseJavaModule {

    private static final int CALLBACK_CODE = 676;

    public SDKMerchantModule(ReactApplicationContext reactContext) {
        super(reactContext);
    }

    @Override
    public String getName() {
        return "SDKMerchantModule";
    }


}
