
package com.guys.go.love.tea;

import android.annotation.SuppressLint;
import android.app.ActivityManager;
import android.app.PendingIntent;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.ComponentName;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.content.pm.ActivityInfo;
import android.media.MediaScannerConnection;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Debug;
import android.os.Environment;
import android.os.FileUtils;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.os.Parcel;
import android.os.PowerManager;
import android.provider.MediaStore;
import android.provider.Settings;
import android.text.TextUtils;
import android.text.format.Formatter;
import android.util.Log;
import android.view.Window;
import android.view.WindowManager;
import android.widget.CompoundButton;

import com.alipay.sdk.app.PayTask;
import com.guys.go.love.tea.MyPushService.MyPushBinder;
import com.tencent.android.tpush.XGIOperateCallback;
import com.tencent.android.tpush.XGPushConfig;
import com.tencent.android.tpush.XGPushManager;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelpay.PayReq;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.tencent.mobileqq.openpay.api.IOpenApi;
import com.tencent.mobileqq.openpay.constants.OpenConstants;
import com.tencent.mobileqq.openpay.data.pay.PayApi;
import com.unity3d.player.UnityPlayer;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.OutputStream;
import java.net.FileNameMap;
import java.net.URLConnection;
import java.util.Date;
import java.util.List;
import java.util.Map;

//@SuppressLint("NewApi")
public class MainActivity extends com.unity3d.player.UnityPlayerActivity implements IWXAPIEventHandler {

    PendingIntent pendingActivityIntent = null;
    MyPushService myPushService;
    MyPushBinder pushBinder;
    PurchaseOfficer pInstance;
    public static MainActivity instance = null;
    public static final String TAG = "DDS";
    String DeviceId = "";
    String AppId = "wx2577571ccde9c33a";
    // IWXAPI 是第三方app和微信通信的openapi接口
    private com.tencent.mm.opensdk.openapi.IWXAPI api;
    private ActivityManager mActivityManager = null;

    //qq支付
    IOpenApi openApi;
    String QQ_APPID="1107203182";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        instance = this;
        // 微信注册
        api = WXAPIFactory.createWXAPI(this, AppId);
        api.registerApp(AppId);
//        api.handleIntent(getIntent(), this);
//        openApi= OpenApiFactory.getInstance(this,QQ_APPID);

        mActivityManager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
        System.out.println("mActivityManager " + mActivityManager);


    }
    /********************************************** * weichat Start ******/

    public void BuyWGameShopItem(String appid, String partnerId, String prepayId, String packageValue, String nonceStr,
                           String timeStamp, String sign) {
        PayReq payReq = new PayReq();
        payReq.appId = appid;
        payReq.partnerId = partnerId;
        payReq.prepayId = prepayId;
        payReq.nonceStr = nonceStr;
        payReq.timeStamp = timeStamp;
        payReq.packageValue = packageValue;
        payReq.sign = sign;
        CallWeiChatPay(payReq);
    }
   public  PayReq payReq;
    public void CallWeiChatPay(final PayReq payReq) {
        this.payReq=payReq;
        instance.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                api.sendReq(payReq);
//                Intent intent=new Intent(MainActivity.this,WXPayEntryActivity.class);
//                startActivity(intent);
            }
        });
    }

    @Override
    public void onReq(BaseReq arg0) {
        // TODO Auto-generated method stub
        System.out.println("simon ------------------------------------------------------onReq");
    }

    @Override
    public void onResp(BaseResp resp) {
        // TODO Auto-generated method stub
        System.out.println("simon ------------------------------------------------------onResp: " + resp.errCode);
//        Toast.makeText(instance, "返回数据码  " + resp.errCode, Toast.LENGTH_SHORT).show();
        if (resp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {
            // success
            if (resp.errCode == 0) {
                payCallback(true);
            } else {
                payCallback(false);
            }
        }
    }


    /********************************************** * end Start ******/

    /********************************************** * alipay Start ******/
    private static final int SDK_PAY_FLAG = 1;

    public void BuyAGameShopItem(final String orderInfo) {
        Runnable payRunnable = new Runnable() {
            @Override
            public void run() {
                PayTask alipay = new PayTask(MainActivity.this);
                Map<String, String> result = alipay.payV2(orderInfo, true);
                System.out.println("simon msp " + result.toString());
                Log.i("orderInfo===", orderInfo);
                Message msg = new Message();
                msg.what = SDK_PAY_FLAG;
                msg.obj = result;
                mHandler.sendMessage(msg);
            }
        };

        Thread payThread = new Thread(payRunnable);
        payThread.start();
    }

    @SuppressLint("HandlerLeak")
    private Handler mHandler = new Handler() {
        @SuppressWarnings("unused")
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case SDK_PAY_FLAG: {
                    @SuppressWarnings("unchecked")
                    PayResult payResult = new PayResult((Map<String, String>) msg.obj);
                    /**
                     * 对于支付结果，请商户依赖服务端的异步通知结果。同步通知结果，仅作为支付结束的通知。
                     */
                    String resultInfo = payResult.getResult();// 同步返回需要验证的信息
                    String resultStatus = payResult.getResultStatus();
                    // 判断resultStatus 为9000则代表支付成功
                    if (TextUtils.equals(resultStatus, "9000")) {
                        // 该笔订单是否真实支付成功，需要依赖服务端的异步通知。
                        // 处理支付结果
                        payCallback(true);
                    } else {
                        // 该笔订单真实的支付结果，需要依赖服务端的异步通知。
                        payCallback(false);
                        Log.d("simon", resultInfo.toString());
                    }
                    break;
                }

                default:
                    break;
            }
        }

        ;
    };



    /************************************************************ alipay End ******/

    /********************************************** * qqPay Start ******/
    public void payQQ(final String qqAppid, String nonceStr, String timeStamp, String tokenId,
                      String bargainorId, String sign) {
        boolean isSupport = openApi.isMobileQQSupportApi(OpenConstants.API_NAME_PAY);
        if(isSupport){
//            Toast.makeText(this,"创建订单",Toast.LENGTH_LONG).show();
            PayApi api=new PayApi();
            api.appId=qqAppid;
            api.serialNumber="1";
            api.nonce=nonceStr;
            api.timeStamp= Long.parseLong(timeStamp);
            api.tokenId=tokenId;
            api.pubAcc="pubAcc=";//手Q公众帐号
            api.pubAccHint="";
            api.bargainorId=bargainorId;//商户号
            api.sigType="HMAC-SHA1";
            api.sig=sign;
            api.callbackScheme="qwallet"+qqAppid;
            if (api.checkParams()) {
                openApi.execApi(api);
            }
        }
    }

    /************************************************************ qqPay End ******/

    void Log(String str) {
        Log.d(TAG, str);
    }

    public void StartInitPhoneData() {
        UnityPlayer.UnitySendMessage("WDS_ANDROID_UNITY_MSG", "InitData", DeviceId);
    }

    public void restartActivity() {
        android.os.Process.killProcess(android.os.Process.myPid());
        Intent i = getBaseContext().getPackageManager().getLaunchIntentForPackage(getBaseContext().getPackageName());
        i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        startActivity(i);
    }

    public void copyUrl(String url) {
        CopyTextToClipboard(url);
    }

    public void payCallback(boolean state) {
        System.out.println("payCallback  state " + state);
        String payResult = state ? "1" : "0";
        UnityPlayer.UnitySendMessage("WDS_ANDROID_UNITY_MSG", "PayResultCallback", payResult);

    }

    private ServiceConnection mConnection = new ServiceConnection() {
        public void onServiceConnected(ComponentName className, IBinder localBinder) {
            MainActivity.this.pushBinder = (MyPushBinder) localBinder;
            MainActivity.this.myPushService = pushBinder.getService();
            afterConnected();
            Log.w(TAG, "bind success");
        }

        public void onServiceDisconnected(ComponentName arg0) {
            MainActivity.this.myPushService = null;
            MainActivity.this.pushBinder = null;
            Log.w(TAG, "unbind success");
        }
    };

    public void afterConnected() {
        Log.w(TAG, new Date() + "");
        // PNotification_SendMessageHD(MyPushService.DISABLE_ALL, 0L, "", "");
        // regLoginBack();
    }

    public void notifyClock(String key, String seconds, String title, String content) {
        Log.w(TAG, "NotifyClock key  " + key);
        Log.w(TAG, "NotifyClock time  " + seconds);
        Log.w(TAG, "NotifyClock title  " + title);
        Log.w(TAG, "NotifyClock content  " + content);
        long time = Integer.valueOf(seconds) * 1000L;
        PNotification_SendMessageHD(key, time, title, content);
    }

    public void PNotification_SendMessageHD(String key, long time, String title, String content) {

        time = System.currentTimeMillis() + time;
        Log.w(TAG, "PNotification_SendMessageHD key  " + key);
        Log.w(TAG, "PNotification_SendMessageHD time  " + time);
        Log.w(TAG, "PNotification_SendMessageHD title  " + title);
        Log.w(TAG, "PNotification_SendMessageHD content  " + content);
        try {
            Parcel p = Parcel.obtain();
            p.writeString(key);
            p.writeLong(time);
            p.writeString(title);
            p.writeString(content);
            Parcel result = Parcel.obtain();
            pushBinder.transact(MyPushService.TIME_NOTICE, p, result, 0);
            p.recycle();
            result.recycle();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @SuppressLint("NewApi")
    public void CopyTextToClipboard(final String text) {
        runOnUiThread(new Runnable() {

            @Override
            public void run() {
                ClipboardManager clipboardManager = (ClipboardManager) instance
                        .getSystemService(Context.CLIPBOARD_SERVICE);
                ClipData clipData = ClipData.newPlainText("label", text);
                clipboardManager.setPrimaryClip(clipData);
            }
        });
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
//		unbindService(mConnection);
    }

    @Override
    protected void onRestart() {
        super.onRestart();

    }

    @Override
    protected void onStart() {
        super.onStart();

    }

    PowerManager.WakeLock wakeLock;

    @Override
    protected void onResume() {
        // TODO Auto-generated method stub
        super.onResume();
        PowerManager pm = (PowerManager) getSystemService(Context.POWER_SERVICE);

        wakeLock = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "loli:Lock");
        wakeLock.acquire();

    }

    @Override
    protected void onPause() {
        // TODO Auto-generated method stub
        Thread t = new Thread() {
            public void run() {
                wakeLock.release();
            }
        };
        t.start();
        super.onPause();

    }

    @Override
    public void onAttachedToWindow() {
        super.onAttachedToWindow();

        Log.w(TAG, "onAttachedToWindow");
        Window window = getWindow();
        window.addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    }


    public String GetDeviceID() {

        String DeviceId;
        DeviceId = Settings.System.getString(getContentResolver(), Settings.System.ANDROID_ID);
        return DeviceId;

    }






    void setAdOrienation() {
        //初始化为横屏
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
    }



    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        if (isChecked) {

        } else {

        }
    }




    void Tips(String msg){
//       Toast.makeText(this, msg, Toast.LENGTH_LONG).show();
//       System.err.println("[simon] " + msg);
    }



    /**************************************内存查看工具*/
    /**
     * 获取当前app占用内存
     *
     * @return
     */
    public String getMemoryInfo() {
        System.out.println("simon ccc");
        // 系统总内存
        String message = getTotalMemory();
        // 系统可用内存
        message += "-" + getAvailMemory();
        // 当前App占用的内存
        message += "-" + getCurrentProcessMemory();
        return message;
    }


    @SuppressLint("NewApi")
    private String getCurrentProcessMemory() {
        String pkgName = this.getPackageName();
        List<ActivityManager.RunningAppProcessInfo> appList = mActivityManager
                .getRunningAppProcesses();
        for (ActivityManager.RunningAppProcessInfo appInfo : appList) {
            if (appInfo.processName.equals(pkgName)) {
                int[] pidArray = new int[] { appInfo.pid };
                Debug.MemoryInfo[] memoryInfo = mActivityManager
                        .getProcessMemoryInfo(pidArray);
                float temp = (float) memoryInfo[0].getTotalPrivateDirty() / 1024.0f;
                return String.format("%.2f", temp)+"MB";
            }
        }
        return "获取失败";
    }

    /**
     * 获取手机总可用内存大小
     *
     * @return
     */
    private String getTotalMemory() {
        String str1 = "/proc/meminfo";// 系统内存信息文件
        String str2;
        String[] arrayOfString;

        try {
            FileReader localFileReader = new FileReader(str1);
            BufferedReader localBufferedReader = new BufferedReader(
                    localFileReader, 8192);
            str2 = localBufferedReader.readLine();// 读取meminfo第一行，系统总内存大小

            arrayOfString = str2.split("\\s+");
            localBufferedReader.close();
            float temp = Integer.valueOf(arrayOfString[1])/1048576.0f;
            return String.format("%.2f", temp)+"GB";
        } catch (IOException e) {
            return "获取失败";
        }
    }

    /**
     * 获取系统可用内存信息
     *
     * @return
     */
    private String getAvailMemory() {

        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        mActivityManager.getMemoryInfo(mi);
        // 转化为mb
        return Formatter.formatFileSize(this, mi.availMem);
    }



    public  void SaveFile(String url){
//        System.out.println("SaveFile0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 " + url);
        File file = new File(url);
        System.out.println("file:"+file);
        if(file.isFile()){
            System.out.println("文件存在");
            StartSaveFile(file);
        }else {
            System.out.println("文件不存在");
        }
    }


    //保存图片
    public  void StartSaveFile(File file){
        String mimeType = getMimeType(file);
        System.out.println("********************************  dds " + Build.VERSION.SDK_INT  +   "  " +  Build.VERSION_CODES.Q );
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q)
        {
            String fileName = file.getName();
            ContentValues values = new ContentValues();
            values.put(MediaStore.MediaColumns.DISPLAY_NAME,fileName);
            values.put(MediaStore.MediaColumns.MIME_TYPE, mimeType);
            values.put(MediaStore.MediaColumns.RELATIVE_PATH, Environment.DIRECTORY_DCIM);
            ContentResolver contentResolver = this.getContentResolver();
            Uri uri = contentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
            if(uri == null){
                System.out.println("图片保存失败");
                return;
            }
            try {
                OutputStream out = contentResolver.openOutputStream(uri);
                FileInputStream fis = new FileInputStream(file);
                FileUtils.copy(fis,out);
                fis.close();
                out.close();
                System.out.println("图片保存成功");
            } catch (IOException e) {
                e.printStackTrace();
            }
        }else {
            MediaScannerConnection.scanFile(this, new String[]{file.getPath()}, new String[]{mimeType}, (path, uri) -> {
                //ToastUtils.showShort("图片已成功保存到" + path);
                System.out.println("success");
            });
        }
    }


    public static String getMimeType(File file){
        FileNameMap fileNameMap = URLConnection.getFileNameMap();
        String type = fileNameMap.getContentTypeFor(file.getName());
        return type;
    }

    public  int  GetAndroidSystemVersion(){
        int version =0;
        if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q){
            version=1;
        }
//        System.out.println("RecordAndroidSystemVersion " + version);
        return version;
    }

    public void scanFile(String filePath) {
        Log.i("Unity", "------------filePath"+filePath);
        Intent scanIntent = new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE);
        scanIntent.setData(Uri.fromFile(new File(filePath)));
        this.sendBroadcast(scanIntent);

    }


    public void InitTPush(){
        Log.e("TPush","*********************InitTPush");
        XGPushConfig.enableDebug(this,true);
        XGPushManager.registerPush(this, new XGIOperateCallback() {
            @Override
            public void onSuccess(Object data, int flag) {
                //token在设备卸载重装的时候有可能会变
                Log.e("TPush", "注册成功，设备token为：" + data);
                UnityPlayer.UnitySendMessage("Andorid_Manager", "OnRegisterPushSucess",data.toString());

            }

            @Override
            public void onFail(Object data, int errCode, String msg) {
                Log.e("TPush", "注册失败，错误码：" + errCode + ",错误信息：" + msg);
            }
        });
        Log.e("TPush","*********************InitTPush   Finish");

    }
    //************************************ Service
    private ServiceConnection conn = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName name, IBinder binder) {
            MyService.MyBinder myBinder = (MyService.MyBinder) binder;
            myBinder.getService().SetData(serviceData);

        }

        @Override
        public void onServiceDisconnected(ComponentName name) {
        }
    };

    ServiceData serviceData=new ServiceData();
    public void StartService(String path,String token,String id,String playerId){
        serviceData.path=path;
        serviceData.token=token;
        serviceData.id=id;
        serviceData.playerId=playerId;
        Intent intent = new Intent(this, MyService.class);
        Log.v(TAG,"StartService");
        bindService(intent, conn, BIND_AUTO_CREATE);
    }

    public void StopService(){

        Log.v(TAG,"StopService");
        unbindService(conn);
    }

}
