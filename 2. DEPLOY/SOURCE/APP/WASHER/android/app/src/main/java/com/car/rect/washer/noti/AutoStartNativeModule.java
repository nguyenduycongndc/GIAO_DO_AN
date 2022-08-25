package com.car.rect.washer.noti;

import android.app.NotificationManager;
import android.content.Context;

import com.facebook.react.bridge.Callback;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;

import javax.annotation.Nonnull;

public class AutoStartNativeModule extends ReactContextBaseJavaModule {
    private ReactApplicationContext context = getReactApplicationContext();

    public AutoStartNativeModule(@Nonnull ReactApplicationContext reactContext) {
        super(reactContext);
    }

    @Nonnull
    @Override
    public String getName() {
        return "NativeModule";
    }


    @ReactMethod
    public void getNotificationData(Callback callback) {
        callback.invoke(Noti.notifyData.toString());
        clearNotificationData();
    }

    @ReactMethod
    public void clearNotificationData() {
        Noti.notifyData = new StringBuilder();
    }

    @ReactMethod
    public void dismissAllNotification() {
//        NotificationManager nMgr = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
//        nMgr.cancelAll();
        Noti.dismissNotify(context);
    }


}