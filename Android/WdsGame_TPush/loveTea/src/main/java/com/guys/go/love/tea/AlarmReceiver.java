package com.guys.go.love.tea;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class AlarmReceiver extends BroadcastReceiver {
    private final static String TAG = "AlarmReceiver";
    private final static String SYSTEM_BROADCAST_ACTION = "android.intent.action.BOOT_COMPLETED";

    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent.getAction().equals(SYSTEM_BROADCAST_ACTION)) {
            Intent it = new Intent(context, MyPushService.class);
            it.putExtra("sys", true);
            context.startService(it);
        }
    }
}
