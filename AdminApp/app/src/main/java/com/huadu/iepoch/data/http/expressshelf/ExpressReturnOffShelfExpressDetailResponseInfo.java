package com.huadu.iepoch.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class ExpressReturnOffShelfExpressDetailResponseInfo extends ResponseInfo {

    @SerializedName("express")
    private ExpressReturnOffShelfExpressItemInfo express;

    public ExpressReturnOffShelfExpressItemInfo getExpress() {
        return express;
    }

    public void setExpress(ExpressReturnOffShelfExpressItemInfo express) {
        this.express = express;
    }
}
