package com.guys.go.love.tea;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.util.Log;

import java.util.Arrays;
import java.util.Date;
import java.util.Set;
import java.util.TreeSet;


public class MessageNotification {
    private static MessageNotification mInstance = null;
    private final static int notificationID = 1000;
    private int messageNotificationID = 1000;
    private NotificationManager mNotificationManager = null;
    private Context mContext;


    public final static int DONE = 2;
    public final static int READY = 1;
    public final static int UNSET = 0;
    public final static String SHARE_PRE_FILE = "my_preferences";
    public final static String allNotifyKey = "allNotifyKey";
    public final static String IS_IN_GAME = "isInGame";

    public final static String SUFFIX_TASK = ".task";
    public final static String SUFFIX_TIME = ".time";
    public final static String SUFFIX_TITLE = ".title";
    public final static String SUFFIX_CONTENT = ".content";
    public final static String FULL_HEART = "fullHeart";
    private SharedPreferences sharedPrefsFile;

    private MessageNotification(Context context) {
        mContext = context;
        sharedPrefsFile = context.getSharedPreferences(SHARE_PRE_FILE, 0);
        mNotificationManager = (NotificationManager) mContext.getSystemService(Context.NOTIFICATION_SERVICE);
    }

    public synchronized static MessageNotification getInstance(Context context) {
        if (mInstance == null) {
            mInstance = new MessageNotification(context);
        }
        return mInstance;
    }

//    public void sendNotification(String key) {
//        Notification mNotification = new Notification();
//        mNotification.icon = R.drawable.ic_launcher;
//
//        mNotification.tickerText = mContext.getText(R.string.app_name);
//        mNotification.defaults = Notification.DEFAULT_SOUND;
//        Intent intent = new Intent(mContext, MainActivity.class);
//        PendingIntent mPendingIntent = PendingIntent.getActivity(mContext, 0, intent, PendingIntent.FLAG_ONE_SHOT);
//
//        mNotification.flags |= Notification.FLAG_AUTO_CANCEL;
//        String title = sharedPrefsFile.getString(key + SUFFIX_TITLE, "");
//        String content = sharedPrefsFile.getString(key + SUFFIX_CONTENT, "");
//
//        mNotification.setLatestEventInfo(mContext, title, content, mPendingIntent);
//
//        mNotificationManager.notify(key.hashCode(), mNotification);
//
//        changeNotificationStatus(key, DONE);
//    }


    public void sendNotification(String key) {
        String title = sharedPrefsFile.getString(key + SUFFIX_TITLE, "");
        String content = sharedPrefsFile.getString(key + SUFFIX_CONTENT, "");


        Notification.Builder builder = new Notification.Builder(mContext);
        builder.setSmallIcon(R.drawable.ic_launcher); //设置图标

        builder.setTicker("叶罗丽");
        builder.setContentTitle(title); //设置标题
        builder.setContentText(content); //消息内容
        builder.setWhen(System.currentTimeMillis()); //发送时间
        builder.setDefaults(Notification.DEFAULT_SOUND); //设置默认的提示音，振动方式，灯光
        builder.setAutoCancel(true);//打开程序后图标消失

        Intent intent = new Intent(mContext, MainActivity.class);
        PendingIntent mPendingIntent = PendingIntent.getActivity(mContext, 0, intent, PendingIntent.FLAG_ONE_SHOT);

        builder.setContentIntent(mPendingIntent);
        Notification notification1 = builder.build();
        mNotificationManager.notify(124, notification1); // 通过通知管理器发送通知

    }


    public void sendNotification(int id, String cnt) {

        String title = "叶罗丽";
        String content = cnt;


        Notification.Builder builder = new Notification.Builder(mContext);
        builder.setSmallIcon(R.drawable.ic_launcher); //设置图标

        builder.setTicker("叶罗丽");
        builder.setContentTitle(title); //设置标题
        builder.setContentText(content); //消息内容
        builder.setWhen(System.currentTimeMillis()); //发送时间
        builder.setDefaults(Notification.DEFAULT_SOUND); //设置默认的提示音，振动方式，灯光
        builder.setAutoCancel(true);//打开程序后图标消失

        Intent intent = new Intent(mContext, MainActivity.class);
        PendingIntent mPendingIntent = PendingIntent.getActivity(mContext, 0, intent, PendingIntent.FLAG_ONE_SHOT);

        builder.setContentIntent(mPendingIntent);
        Notification notification1 = builder.build();
        mNotificationManager.notify(id, notification1); // 通过通知管理器发送通知
    }


//    public void sendNotification(int id, String cnt) {
//
//        Date dt = new Date();
//        Notification mNotification = new Notification();
//        mNotification.icon = R.drawable.ic_launcher;
//
//        mNotification.tickerText = mContext.getText(R.string.app_name);
//        mNotification.defaults = Notification.DEFAULT_SOUND;
//        Intent intent = new Intent(mContext, MainActivity.class);
//        PendingIntent mPendingIntent = PendingIntent.getActivity(mContext, 0, intent, PendingIntent.FLAG_ONE_SHOT);
//
//        mNotification.flags |= Notification.FLAG_AUTO_CANCEL;
//        String title = "Ҷ����";
//        String content = cnt;
//
//        mNotification.setLatestEventInfo(mContext, title, content, mPendingIntent);
//
//        mNotificationManager.notify(id, mNotification);
//    }


    public int getNotificationID() {
        return notificationID;
    }

    private void changeNotificationStatus(String key, int status) {
        sharedPrefsFile.edit().putInt(key + SUFFIX_TASK, status).commit();
    }

    public Set<String> getAllKeys() {
        String str = sharedPrefsFile.getString(allNotifyKey, "");
        Set<String> keyset = new TreeSet<String>(Arrays.asList(str.split(",")));
        return keyset;
    }

    public void saveNotificationInfo(String key, long time, String title, String content) {
        Editor editor = sharedPrefsFile.edit();
        editor.putInt(key + SUFFIX_TASK, UNSET).commit();
        Set<String> keyset = getAllKeys();
        keyset.add(key);
        editor.putString(allNotifyKey, MyUtil.join(keyset));
        editor.putLong(key + SUFFIX_TIME, time);
        editor.putString(key + SUFFIX_TITLE, title);
        editor.putString(key + SUFFIX_CONTENT, content);
        editor.putInt(key + SUFFIX_TASK, READY);
        editor.commit();

        Log.w("writing:" + key, new Date(time) + title);
    }

    public int getNotificationStatus(String key) {
        return sharedPrefsFile.getInt(key + SUFFIX_TASK, UNSET);
    }

    public void enableAll() {
        Log.w("enableinfo", DONE + "");
        sharedPrefsFile.edit().putInt(IS_IN_GAME, DONE).commit();
    }

    public void disableAll() {
        Log.w("enableinfo", UNSET + "");
        sharedPrefsFile.edit().putInt(IS_IN_GAME, UNSET).commit();
    }

    public int checkIsInGame() {
        return sharedPrefsFile.getInt(IS_IN_GAME, UNSET);
    }

    public long getNotificationTime(String key) {
        return sharedPrefsFile.getLong(key + SUFFIX_TIME, Long.MAX_VALUE);
    }

    public void resetNotificationStatus(String key) {
        changeNotificationStatus(key, UNSET);
    }


}
