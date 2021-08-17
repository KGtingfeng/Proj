package com.guys.go.love.tea;

import android.app.Service;
import android.content.Intent;
import android.os.Binder;
import android.os.IBinder;
import android.util.Log;

import androidx.annotation.Nullable;

import com.unity3d.player.UnityPlayer;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.Map;

public class MyService extends Service {

    public static final String TAG = "SPS";
    //client 可以通过Binder获取Service实例
    public class MyBinder extends Binder {
        public com.guys.go.love.tea.MyService getService() {
            return com.guys.go.love.tea.MyService.this;
        }
    }

    ServiceData data;
    boolean isBind=false;

    //通过binder实现调用者client与Service之间的通信
    private MyBinder binder = new MyBinder();


    @Override
    public void onCreate() {
        super.onCreate();
        Log.v(TAG,"onCreate");

    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {


        Log.v(TAG,"onStartCommand");
        return START_NOT_STICKY;
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        Log.v(TAG,"onBind");
        isBind=true;
        HeartBeat();

        return binder;
    }

    @Override
    public boolean onUnbind(Intent intent) {
        Log.v(TAG,"onUnbind");
        isBind=false;
        return false;
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
    }

    public void SetData(ServiceData serviceData){
        data=serviceData;
        Log.v(TAG,"SetData");
    }

    private void HeartBeat(){

        new Thread() {
            @Override
            public void run() {
                super.run();

                try{
                    Thread.sleep(10000);
                }catch (Exception e){
                }

                Map<String,String> map=new HashMap<String,String>();
                map.put("userToken",data.token);
                map.put("userId",data.id);
                map.put("playerId",data.playerId);
                sendPostMessage(map,"utf-8");
                if(isBind){
                    HeartBeat();
                }
            }
        }.start();

    }


    public void sendPostMessage(Map<String, String> params,String encode){
        StringBuffer buffer = new StringBuffer();
        try {//把请求的主体写入正文！！
            if(params != null&&!params.isEmpty()){
                //迭代器
                for(Map.Entry<String, String> entry : params.entrySet()){
                    buffer.append(entry.getKey()).append("=").
                            append(URLEncoder.encode(entry.getValue(),encode)).
                            append("&");
                }
            }
            URL url;
                url = new URL(data.path);

                //删除最后一个字符&，多了一个;主体设置完毕
                buffer.deleteCharAt(buffer.length()-1);
                byte[] mydata = buffer.toString().getBytes();
                HttpURLConnection connection = (HttpURLConnection) url.openConnection();
                connection.setConnectTimeout(3000);
                connection.setDoInput(true);//表示从服务器获取数据
                connection.setDoOutput(true);//表示向服务器写数据

                connection.setRequestMethod("POST");
                //是否使用缓存
                connection.setUseCaches(false);
                //表示设置请求体的类型是文本类型
                connection.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");

                connection.setRequestProperty("Content-Length", String.valueOf(mydata.length));
                connection.connect();   //连接，不写也可以。。？？有待了解

                //获得输出流，向服务器输出数据
                OutputStream outputStream = connection.getOutputStream();
                outputStream.write(mydata,0,mydata.length);
                outputStream.flush();
                outputStream.close();
                //获得服务器响应的结果和状态码
                int responseCode = connection.getResponseCode();
                if(responseCode == HttpURLConnection.HTTP_OK){
                    String io =  changeInputeStream( connection.getInputStream(),encode) ;
                    Log.v(TAG,io);
                    UnityPlayer.UnitySendMessage("SPS_UNITY_MSG", "OnHeartBeat", io);

                }else {
                    Log.v(TAG,"HTTP_ERROR");
                }

        } catch (UnsupportedEncodingException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }


    }


    /**
     * 将一个输入流转换成字符串
     * @param inputStream
     * @param encode
     * @return
     */
    private String changeInputeStream(InputStream inputStream, String encode) {
        //通常叫做内存流，写在内存中的
        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
        byte[] data = new byte[1024];
        int len = 0;
        String result = "";
        if(inputStream != null){
            try {
                while((len = inputStream.read(data))!=-1){
                    data.toString();

                    outputStream.write(data, 0, len);
                }
                //result是在服务器端设置的doPost函数中的
                result = new String(outputStream.toByteArray(),encode);
                outputStream.flush();
            } catch (IOException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
        }
        return result;
    }

}
