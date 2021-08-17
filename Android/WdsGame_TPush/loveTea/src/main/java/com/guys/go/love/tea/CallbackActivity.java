package com.guys.go.love.tea;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.tencent.mobileqq.openpay.api.IOpenApi;
import com.tencent.mobileqq.openpay.api.IOpenApiListener;
import com.tencent.mobileqq.openpay.api.OpenApiFactory;
import com.tencent.mobileqq.openpay.data.base.BaseResponse;
import com.tencent.mobileqq.openpay.data.pay.PayResponse;

public class CallbackActivity extends Activity implements IOpenApiListener {
    private static final String TAG = "CallbackActivity";
    String appId = "1107203182";
    IOpenApi openApi;
    public static CallbackActivity instance = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        instance=this;
        openApi = OpenApiFactory.getInstance(this,appId);
        openApi.handleIntent(getIntent(), this);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        openApi.handleIntent(intent, this);
    }
    @Override
    public void onOpenResponse(BaseResponse response)
    {
        String message;
        if (response == null) {
            // 不能识别的intent
//            Toast.makeText(this,"不能识别的intent： ",Toast.LENGTH_LONG).show();
            return;
        } else {
//            Toast.makeText(this,"QQ支付",Toast.LENGTH_LONG).show();
            if (response instanceof PayResponse) {
                // 支付回调响应
                PayResponse payResponse = (PayResponse) response;
                if (payResponse.isSuccess()) {
                    // 支付成功，这个支付结果不能作为发货的依据
                    if (!payResponse.isPayByWeChat()) {
//                        payCallback
                    }
                    payCallback(true);
                }else{
                    payCallback(false);
                }
            } else {
                // 不能识别的响应
                Log.e("simon","不能识别的响应");
            }
        }
    }


    public void payCallback(boolean status)
    {
        MainActivity.instance.payCallback(status);
        instance.onDestroy();
        this.finish();
    }

}
