package com.huadu.shunda.data.http.packbox;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class GetOrderCodeByPackBoxCodeResponseInfo extends ResponseInfo {

    @SerializedName("orderPackBoxCode")
    private String orderPackBoxCode;

    @SerializedName("orderCode")
    private String orderCode;

    public String getOrderPackBoxCode() {
        return orderPackBoxCode;
    }

    public void setOrderPackBoxCode(String orderPackBoxCode) {
        this.orderPackBoxCode = orderPackBoxCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }
}
