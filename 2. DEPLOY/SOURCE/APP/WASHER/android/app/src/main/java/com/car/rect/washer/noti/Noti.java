package com.car.rect.washer.noti;


import android.app.KeyguardManager;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.util.Log;
import android.view.WindowManager;

import androidx.annotation.NonNull;

import com.car.rect.washer.MainActivity;
import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;

import org.json.JSONException;
import org.json.JSONObject;

import java.sql.Timestamp;

public class Noti extends FirebaseMessagingService {
    public static StringBuilder notifyData = new StringBuilder();
    private static final String TAG = "notification_date";
    private static final int NOTI_ORDER_STATUS_CONFIRM = 2;

    public static void dismissNotify(Context context) {
        NotificationManager nMgr = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        nMgr.cancelAll();
    }

    @Override
    public void onMessageReceived(@NonNull RemoteMessage remoteMessage) {
        super.onMessageReceived(remoteMessage);
        try {

            JSONObject jsonObject = new JSONObject(remoteMessage.getData().get("custom"));
            int type = jsonObject.getJSONObject("a").getInt("type");
            if (type == NOTI_ORDER_STATUS_CONFIRM) {
                long timeSend = jsonObject.getJSONObject("a").getInt("timeSend");
                long timeWait = jsonObject.getJSONObject("a").getInt("timeWait");
                long timeNow = new Timestamp(System.currentTimeMillis()).getTime() / 1000;
                boolean validate = (timeWait - timeNow + timeSend) > 0;
                    if (validate) {
                    notifyData = new StringBuilder();
                    Intent intent = new Intent(getBaseContext(), MainActivity.class);
                    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    startActivity(intent);
                    notifyData.append(remoteMessage.getData().get("custom"));
                } else dismissNotify(this);
            }

        } catch (JSONException e) {
            Log.d(TAG, e.getMessage());
        }
    }

}