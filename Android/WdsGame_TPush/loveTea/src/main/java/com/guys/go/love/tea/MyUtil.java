package com.guys.go.love.tea;

import android.app.ActivityManager;
import android.app.ActivityManager.RunningServiceInfo;
import android.content.Context;
import android.util.Log;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;
import java.util.Random;
import java.util.Set;

public class MyUtil {
    public static boolean logger = true;

    public static int getDayOfWeek() {
        int weekday = 0;
        Calendar c = Calendar.getInstance();
        c.setTime(new Date(System.currentTimeMillis()));
        weekday = c.get(Calendar.DAY_OF_WEEK);
        return weekday;
    }


    public static Long getCurrentTime() {
        try {
            Date date = new Date();
            SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            return df.parse(df.format(date)).getTime();
        } catch (Exception e) {
        }
        return null;
    }

    public static String getLongTimeToDateTime(Long l) {
        Date date = new Date(l);
        SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault());
        String t = df.format(date);
        Log.i("NowTime:", "���ڵ�ʱ����:" + t);
        return t;
    }

    public static boolean isServiceRunning(Context ctx, String filePath) {
        ActivityManager manager = (ActivityManager) ctx.getSystemService(Context.ACTIVITY_SERVICE);
        for (RunningServiceInfo service : manager.getRunningServices(Integer.MAX_VALUE)) {
            if (filePath.equalsIgnoreCase(service.service.getClassName())) {
                return true;
            }
        }
        return false;
    }

    public static String join(Set<String> set) {
        boolean first = true;
        if (set.size() == 0) {
            return "";
        }
        StringBuilder sb = new StringBuilder();
        for (String s : set) {
            if (s.equals("")) {
                continue;
            }
            if (first) {
                first = false;
            } else {
                sb.append(",");
            }
            sb.append(s);
        }
        return sb.toString();
    }


    public static String getRandomStringByLength(int length) {
        String base = "abcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        StringBuffer sb = new StringBuffer();
        for (int i = 0; i < length; i++) {
            int number = random.nextInt(base.length());
            sb.append(base.charAt(number));
        }
        return sb.toString();
    }

}
