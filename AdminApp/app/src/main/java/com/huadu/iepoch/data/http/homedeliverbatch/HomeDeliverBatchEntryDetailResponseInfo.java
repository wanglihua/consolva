package com.huadu.iepoch.data.http.homedeliverbatch;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class HomeDeliverBatchEntryDetailResponseInfo extends ResponseInfo {

    @SerializedName("targetWarehouse")
    private String targetWarehouse;

    @SerializedName("batchCode")
    private String batchCode;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("orderStatus")
    private String orderStatus;

    @SerializedName("messageStatus")
    private String messageStatus;

    @SerializedName("deliverAddress")
    private String deliverAddress;

    @SerializedName("packBoxList")
    private List<String> packBoxList = new ArrayList<>();

    public String getTargetWarehouse() {
        return targetWarehouse;
    }

    public void setTargetWarehouse(String targetWarehouse) {
        this.targetWarehouse = targetWarehouse;
    }

    public String getBatchCode() {
        return batchCode;
    }

    public void setBatchCode(String batchCode) {
        this.batchCode = batchCode;
    }

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public String getOrderStatus() {
        return orderStatus;
    }

    public void setOrderStatus(String orderStatus) {
        this.orderStatus = orderStatus;
    }

    public String getMessageStatus() {
        return messageStatus;
    }

    public void setMessageStatus(String messageStatus) {
        this.messageStatus = messageStatus;
    }

    public String getDeliverAddress() {
        return deliverAddress;
    }

    public void setDeliverAddress(String deliverAddress) {
        this.deliverAddress = deliverAddress;
    }

    public List<String> getPackBoxList() {
        return packBoxList;
    }

    public void setPackBoxList(List<String> packBoxList) {
        this.packBoxList = packBoxList;
    }
}
