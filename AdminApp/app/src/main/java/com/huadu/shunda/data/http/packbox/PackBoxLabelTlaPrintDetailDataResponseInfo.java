package com.huadu.shunda.data.http.packbox;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class PackBoxLabelTlaPrintDetailDataResponseInfo extends ResponseInfo {

    @SerializedName("packBoxCode")
    private String packBoxCode;

    @SerializedName("packBoxShortCode")
    private String packBoxShortCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("targetWarehouseName")
    private String targetWarehouseName;

    public String getPackBoxCode() {
        return packBoxCode;
    }

    public void setPackBoxCode(String packBoxCode) {
        this.packBoxCode = packBoxCode;
    }

    public String getPackBoxShortCode() {
        return packBoxShortCode;
    }

    public void setPackBoxShortCode(String packBoxShortCode) {
        this.packBoxShortCode = packBoxShortCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getTargetWarehouseName() {
        return targetWarehouseName;
    }

    public void setTargetWarehouseName(String targetWarehouseName) {
        this.targetWarehouseName = targetWarehouseName;
    }
}
