package com.huadu.shunda.data.http.expressphoto;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class GetOrderCodeByPackBoxCodeResponseData extends ResponseInfo {

    @SerializedName("packBoxCode")
    private String packBoxCode;

    @SerializedName("orderCode")
    private String orderCode;

    public String getPackBoxCode() {
        return packBoxCode;
    }

    public void setPackBoxCode(String packBoxCode) {
        this.packBoxCode = packBoxCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }
}
