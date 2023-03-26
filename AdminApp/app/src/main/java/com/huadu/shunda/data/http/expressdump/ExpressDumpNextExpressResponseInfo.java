package com.huadu.shunda.data.http.expressdump;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class ExpressDumpNextExpressResponseInfo extends ResponseInfo {

    @SerializedName("express")
    private ExpressDumpExpressItemInfo express;

    public ExpressDumpExpressItemInfo getExpress() {
        return express;
    }

    public void setExpress(ExpressDumpExpressItemInfo express) {
        this.express = express;
    }
}
