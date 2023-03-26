package com.huadu.iepoch.data.http.packbox;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class PackBoxOrderPayDetailResponseInfo extends ResponseInfo {

    @SerializedName("orderPackBoxCode")
    private String orderPackBoxCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("customerMobile")
    private String customerMobile;

    @SerializedName("orderMobile")
    private String orderMobile;

    @SerializedName("arrearsAmount")
    private String arrearsAmount;

    @SerializedName("hasArrearsAmount")
    private Boolean hasArrearsAmount;

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

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getCustomerMobile() {
        return customerMobile;
    }

    public void setCustomerMobile(String customerMobile) {
        this.customerMobile = customerMobile;
    }

    public String getOrderMobile() {
        return orderMobile;
    }

    public void setOrderMobile(String orderMobile) {
        this.orderMobile = orderMobile;
    }

    public String getArrearsAmount() {
        return arrearsAmount;
    }

    public void setArrearsAmount(String arrearsAmount) {
        this.arrearsAmount = arrearsAmount;
    }

    public Boolean getHasArrearsAmount() {
        return hasArrearsAmount;
    }

    public void setHasArrearsAmount(Boolean hasArrearsAmount) {
        this.hasArrearsAmount = hasArrearsAmount;
    }
}
