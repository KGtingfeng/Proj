package com.guys.go.love.tea;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Build;

import androidx.annotation.RequiresApi;

import java.util.Random;

public class MyReceiver extends BroadcastReceiver {
    public static final String TITLE_KEY = "TITLE_KEY";
    public static final String BODY_KEY = "body_key";
    public static final String SUBJECT_KEY = "subject_key";

    @RequiresApi(api = Build.VERSION_CODES.JELLY_BEAN)
    @Override
    public void onReceive(Context context, Intent intent) {
        String title = intent.getStringExtra(TITLE_KEY);
        String subject = intent.getStringExtra(SUBJECT_KEY);
        String body = intent.getStringExtra(BODY_KEY);


        NotificationManager nManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        long when = System.currentTimeMillis();


        Notification.Builder builder = new Notification.Builder(context);
        builder.setSmallIcon(R.drawable.ic_launcher); //设置图标

        builder.setTicker("ticker");
        builder.setContentTitle(title); //设置标题
        builder.setContentText(body); //消息内容
        builder.setWhen(when); //发送时间
        builder.setDefaults(Notification.DEFAULT_SOUND); //设置默认的提示音，振动方式，灯光
        builder.setAutoCancel(true);//打开程序后图标消失

        intent = new Intent(context, MainActivity.class);
        PendingIntent pIntent = PendingIntent.getActivity(context, 10, intent, 0);

        builder.setContentIntent(pIntent);
        Notification notification1 = builder.build();
        nManager.notify(new Random().nextInt(10000), notification1); // 通过通知管理器发送通知


    }

}
