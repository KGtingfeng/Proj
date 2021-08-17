package com.guys.go.love.tea;//package com.guys.go.paint;
//
//import android.app.Activity;
//import android.content.Intent;
//import android.os.Bundle;
//import android.os.Message;
//import android.util.Log;
//import android.view.View;
//import android.widget.FrameLayout;
//
//import com.bytedance.sdk.openadsdk.*;
//import com.bytedance.sdk.openadsdk.AdSlot;
//import com.bytedance.sdk.openadsdk.TTAdConstant;
//import com.bytedance.sdk.openadsdk.TTAdNative;
//import com.bytedance.sdk.openadsdk.TTAppDownloadListener;
//import com.bytedance.sdk.openadsdk.TTSplashAd;
//
//public class SplashActivity extends Activity implements WeakHandler.IHandler{
//    private static final String TAG = "SplashActivity";
//    /**********************穿山甲ad***************/
////    String appId="5043516";测试
//    String appId="5045807";
//    private TTAdNative mTTAdNative;
//    private TTRewardVideoAd mttRewardVideoAd;
//    private boolean mHasShowDownloadActive = false;
//
//
//    //开屏广告加载发生超时但是SDK没有及时回调结果的时候，做的一层保护。
//    private final WeakHandler mHandler = new WeakHandler(this);
//    //开屏广告加载超时时间,建议大于3000,这里为了冷启动第一次加载到广告并且展示,示例设置了3000ms
//    private static final int AD_TIME_OUT = 3000;
//    private static final int MSG_GO_MAIN = 1;
//    //开屏广告是否已经加载
//    private boolean mHasLoaded;
//    private FrameLayout mSplashContainer;
//    //是否强制跳转到主页面
//    private boolean mForceGoMain;
//
//    @SuppressWarnings("RedundantCast")
//    @Override
//    protected void onCreate(Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(com.bytedance.sdk.openadsdk.R.layout.activity_splash);
//        mSplashContainer = (FrameLayout) findViewById(com.bytedance.sdk.openadsdk.R.id.splash_container);
//        initAd();
//    }
//
//
//    public void initAd(){
//
//        // 申请部分权限,建议在sdk初始化前申请,如：READ_PHONE_STATE、ACCESS_COARSE_LOCATION及ACCESS_FINE_LOCATION权限，
//        // 以获取更好的广告推荐效果，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
//        TTAdSdk.getAdManager().requestPermissionIfNecessary(this);
//        //强烈建议在应用对应的Application#onCreate()方法中调用，避免出现content为null的异常
//        TTAdSdk.init(this,
//                new TTAdConfig.Builder()
//                        .appId(appId)
//                        .useTextureView(false) //使用TextureView控件播放视频,默认为SurfaceView,当有SurfaceView冲突的场景，可以使用TextureView
//                        .appName("叶罗丽")
//                        .titleBarTheme(TTAdConstant.TITLE_BAR_THEME_DARK)
//                        .allowShowNotify(true) //是否允许sdk展示通知栏提示
//                        .allowShowPageWhenScreenLock(true) //是否在锁屏场景支持展示广告落地页
//                        .debug(true) //测试阶段打开，可以通过日志排查问题，上线时去除该调用
//                        .directDownloadNetworkType(TTAdConstant.NETWORK_STATE_WIFI, TTAdConstant.NETWORK_STATE_3G) //允许直接下载的网络状态集合
//                        .supportMultiProcess(false) //是否支持多进程，true支持
//                        //.httpStack(new MyOkStack3())//自定义网络库，demo中给出了okhttp3版本的样例，其余请自行开发或者咨询工作人员。
//                        .build());
//
//        mTTAdNative = TTAdSdk.getAdManager().createAdNative(getApplicationContext());
//
//        loadSplashAd();
//
//    }
//
//
//    /**
//     * 加载开屏广告 这里需要配置
//     */
//    private void loadSplashAd() {
//
//        //step3:创建开屏广告请求参数AdSlot,具体参数含义参考文档
//        AdSlot adSlot = new AdSlot.Builder()
//                .setCodeId("887314397")
//                .setSupportDeepLink(true)
////                .setImageAcceptedSize(960, 640)
////                .setExpressViewAcceptedSize(960,640) sdk版本太低，不支持个性化配置
//                .build();
//        //step4:请求广告，调用开屏广告异步请求接口，对请求回调的广告作渲染处理
//        mTTAdNative.loadSplashAd(adSlot, new TTAdNative.SplashAdListener() {
//            @Override
////            @MainThread
//            public void onError(int code, String message) {
//                Log.d(TAG, message);
//                mHasLoaded = true;
//                showToast(message);
//                goToMainActivity();
//            }
//
//            @Override
////            @MainThread
//            public void onTimeout() {
//                mHasLoaded = true;
//                showToast("开屏广告加载超时");
//                goToMainActivity();
//            }
//
//            @Override
////            @MainThread
//            public void onSplashAdLoad(TTSplashAd ad) {
//                Log.d(TAG, "开屏广告请求成功");
//                mHasLoaded = true;
//                mHandler.removeCallbacksAndMessages(null);
//                if (ad == null) {
//                    return;
//                }
//                //获取SplashView
//                View view = ad.getSplashView();
//                if (view != null) {
//                    mSplashContainer.removeAllViews();
//
//                    //把SplashView 添加到ViewGroup中,注意开屏广告view：width >=70%屏幕宽；height >=50%屏幕高
//                    mSplashContainer.addView(view);
//                    //设置不开启开屏广告倒计时功能以及不显示跳过按钮,如果这么设置，您需要自定义倒计时逻辑
//                    //ad.setNotAllowSdkCountdown();
//                }else {
//                    goToMainActivity();
//                }
//
//                //设置SplashView的交互监听器
//                ad.setSplashInteractionListener(new TTSplashAd.AdInteractionListener() {
//                    @Override
//                    public void onAdClicked(View view, int type) {
//                        Log.d(TAG, "onAdClicked");
//                        showToast("开屏广告点击");
//                    }
//
//                    @Override
//                    public void onAdShow(View view, int type) {
//                        Log.d(TAG, "onAdShow");
//                        showToast("开屏广告展示");
//                    }
//
//                    @Override
//                    public void onAdSkip() {
//                        Log.d(TAG, "onAdSkip");
//                        showToast("开屏广告跳过");
//                        goToMainActivity();
//
//                    }
//
//                    @Override
//                    public void onAdTimeOver() {
//                        Log.d(TAG, "onAdTimeOver");
//                        showToast("开屏广告倒计时结束");
//                        goToMainActivity();
//                    }
//                });
//                if(ad.getInteractionType() == TTAdConstant.INTERACTION_TYPE_DOWNLOAD) {
//                    ad.setDownloadListener(new TTAppDownloadListener() {
//                        boolean hasShow = false;
//
//                        @Override
//                        public void onIdle() {
//
//                        }
//
//                        @Override
//                        public void onDownloadActive(long totalBytes, long currBytes, String fileName, String appName) {
//                            if (!hasShow) {
//                                showToast("下载中...");
//                                hasShow = true;
//                            }
//                        }
//
//                        @Override
//                        public void onDownloadPaused(long totalBytes, long currBytes, String fileName, String appName) {
//                            showToast("下载暂停...");
//
//                        }
//
//                        @Override
//                        public void onDownloadFailed(long totalBytes, long currBytes, String fileName, String appName) {
//                            showToast("下载失败...");
//
//                        }
//
//                        @Override
//                        public void onDownloadFinished(long totalBytes, String fileName, String appName) {
//
//                        }
//
//                        @Override
//                        public void onInstalled(String fileName, String appName) {
//
//                        }
//                    });
//                }
//            }
//        }, AD_TIME_OUT);
//
//    }
//
//    @Override
//    protected void onResume() {
//        //判断是否该跳转到主页面
//        if (mForceGoMain) {
//            mHandler.removeCallbacksAndMessages(null);
//            goToMainActivity();
//        }
//        super.onResume();
//    }
//
//    @Override
//    protected void onStop() {
//        super.onStop();
//        mForceGoMain = true;
//    }
//
//
//    /**
//     * 跳转到主页面
//     */
//    private void goToMainActivity() {
//        Intent intent = new Intent(SplashActivity.this, MainActivity.class);
//        startActivity(intent);
//        mSplashContainer.removeAllViews();
//        this.finish();
//    }
//
//    private void showToast(String msg) {
////        Toast.makeText(this, msg,Toast.LENGTH_LONG).show();;
//    }
//
//    @Override
//    public void handleMsg(Message msg) {
//        if (msg.what == MSG_GO_MAIN) {
//            if (!mHasLoaded) {
//                showToast("广告已超时，跳到主页面");
//                goToMainActivity();
//            }
//        }
//    }
//}
