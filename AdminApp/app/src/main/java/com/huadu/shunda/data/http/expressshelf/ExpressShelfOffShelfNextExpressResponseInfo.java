package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class ExpressShelfOffShelfNextExpressResponseInfo extends ResponseInfo {

    @SerializedName("express")
    private ExpressShelfOffShelfExpressItemInfo express;

    public ExpressShelfOffShelfExpressItemInfo getExpress() {
        return express;
    }

    public void setExpress(ExpressShelfOffShelfExpressItemInfo express) {
        this.express = express;
    }
}
