package com.guys.go.love.tea;

public class AliPayMgr {

    private static AliPayMgr aliPayMgr = null;
    private static final int SDK_PAY_FLAG = 1;
    private static final int SDK_AUTH_FLAG = 2;

    public static AliPayMgr instance() {
        if (aliPayMgr == null) {
            aliPayMgr = new AliPayMgr();
//			aliPayMgr.Init();
        }
        return aliPayMgr;
    }

    private AliPayMgr() {

    }

    public void Init() {
    }


}
