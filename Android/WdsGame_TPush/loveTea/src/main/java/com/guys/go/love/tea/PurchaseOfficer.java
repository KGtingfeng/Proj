package com.guys.go.love.tea;

import android.content.Context;
import android.os.Handler;
import android.util.Log;

//import com.arcsoft.hpay100.HPaySdkAPI;
//import com.arcsoft.hpay100.HPaySdkCallback;
//import com.arcsoft.hpay100.HPaySdkResult;

public class PurchaseOfficer {
	public static PurchaseOfficer mInstance = null;
	private Context mContext;
	public static int ALiPay = 100;
	private Handler handler;

	public PurchaseOfficer(Context context) {
		mContext = context;
		handler = new Handler();
	}

	public static PurchaseOfficer getInstance(Context context) {
		if (mInstance == null) {
			mInstance = new PurchaseOfficer(context);
		}
		return mInstance;
	}

	// 初始化短信SDK
	public void SMS_InitSMS() {
		Log.w("初始化短信", "初始化短信");
//		HPaySdkAPI.setLogDebug(true); // 可以查看日志，如果发版 此处代码删除
//		String merid = "2000217"; // 以分配为准
//		String appid = "247f6d1048db11e6984d7edafd2b4edb"; // 以分配为准
//		String channelid = "wdsgame";// 推广渠道(cp自己填写)
//		String orientation = "0"; // 0 横屏 1竖屏
//		String cpname = "深圳智梦星科技有限公司";
//		String kfphone = "400120000";// CP客服电话(需修改)
//		HPaySdkAPI.initHPaySdk(mContext, merid, appid, channelid, orientation,
//				cpname, kfphone);
//
//		// 获取到用户信息的时候设置(非必传)
//		String userid = ""; // 用户id
//		String userPhone = ""; // 用户手机号 (取不到传"")
//		String userLevel = ""; // 用户等级 (取不到传"")
//		HPaySdkAPI.setUserInfo(userid, userPhone, userLevel);
	}

	public void SMS_PurchaseById(String orderid, String payid, String price, String name, String type) {
//		int tmpPrice = Integer.parseInt(price);
//		int codeType = 0;// 0 点播 2 包月
//		int tmpType = Integer.parseInt(type);
//		switch (tmpType) {
//		// 话费
//		case 1:
//			HPaySdkAPI.startHPaySdk(MainActivity.instance, codeType, orderid,
//					payid, tmpPrice, name, new HPaySdk());
//			break;
//		// 微信
//		case 2:
//			HPaySdkAPI.startHPaySdkWeixin(MainActivity.instance, codeType,
//					orderid, payid, tmpPrice, name, new HPaySdk());
//			break;
//		// 支付宝
//		case 3:
//			HPaySdkAPI.startHPaySdkAlipay(MainActivity.instance, codeType,
//					orderid, payid, tmpPrice, name, new HPaySdk());
//			break;
//		default:
//			break;
//		}

	}

//	private class HPaySdk implements HPaySdkCallback {
//		@Override
//		public void payResult(HPaySdkResult sdkResult) {
//			String msg;
//			Log.e("dalongTest", "sdkResult.getPayStatus():" + sdkResult);
//			// 计费成功和失败的，是包月 还是点播 代码
//			switch (sdkResult.getPayStatus()) {
//			case HPaySdkResult.PAY_STATUS_SUCCESS:
//				// TODO 订单提交成功(根据项目具体需求提示)
//				Toast.makeText(mContext, "订单提交成功", Toast.LENGTH_LONG).show();
//				// TODO 需去服务器检查订单是否真正成功
//				break;
//			case HPaySdkResult.PAY_STATUS_FAILED:
//				// 支付异常
//				msg = sdkResult.getFailedMsg(); // 异常原因
//				Toast.makeText(mContext, msg, Toast.LENGTH_LONG).show();
//				boolean isQueryToServer = sdkResult.getQuery();
//				if (isQueryToServer) { // 短信发送异常，但是无法确定短信是否发送成功（发送成功有可能扣费）
//					// TODO 需去服务器检查订单是否真正成功
//				} else {
//					int payType = sdkResult.getPayType(); // 1：支付宝 2：话费 3：微信
//					if (payType == 2) {
//						msg = "非常抱歉，您的手机号所在地区暂不支持话费充值，请您选择其他充值方式！";
//					}
//					Toast.makeText(mContext, msg, Toast.LENGTH_LONG).show();
//				}
//				break;
//			case HPaySdkResult.PAY_STATUS_CANCEL:
//				// TODO 支付取消(根据项目具体需求提示)
//				Toast.makeText(mContext, "支付取消", Toast.LENGTH_LONG).show();
//				// 不需要去服务器检查订单
//				break;
//			}
//		}
//	}

}
