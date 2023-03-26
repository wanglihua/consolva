package com.huadu.iepoch.data.http.homedeliverbatch;

import com.google.gson.annotations.SerializedName;

public class HomeDeliverBatchEntryItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("deliverNum")
    private int deliverNum;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("customerPhone")
    private String customerPhone;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("packBoxCount")
    private int packBoxCount;

    @SerializedName("orderStatus")
    private String orderStatus;

    @SerializedName("orderStatusFinished")
    private boolean orderStatusFinished;

    @SerializedName("messageStatus")
    private String messageStatus;

    @SerializedName("messageStatusSent")
    private boolean messageStatusSent;

    @SerializedName("consignee")
    private String consignee;

    @SerializedName("address")
    private String address;

    @SerializedName("company")
    private String company;

    @SerializedName("confirmStatus")
    private String confirmStatus;

    @SerializedName("confirmMessage")
    private String confirmMessage;

    @SerializedName("showButton")
    private boolean showButton;

    @SerializedName("estimateDeliverTime")
    private String estimateDeliverTime;

    @SerializedName("actualDeliverTime")
    private String actualDeliverTime;

    @SerializedName("deliverTimeDiff")
    private String deliverTimeDiff;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getDeliverNum() {
        return deliverNum;
    }

    public void setDeliverNum(int deliverNum) {
        this.deliverNum = deliverNum;
    }

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getCustomerPhone() {
        return customerPhone;
    }

    public void setCustomerPhone(String customerPhone) {
        this.customerPhone = customerPhone;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public int getPackBoxCount() {
        return packBoxCount;
    }

    public void setPackBoxCount(int packBoxCount) {
        this.packBoxCount = packBoxCount;
    }

    public String getOrderStatus() {
        return orderStatus;
    }

    public void setOrderStatus(String orderStatus) {
        this.orderStatus = orderStatus;
    }

    public boolean getOrderStatusFinished() {
        return orderStatusFinished;
    }

    public void setOrderStatusFinished(boolean orderStatusFinished) {
        this.orderStatusFinished = orderStatusFinished;
    }

    public String getMessageStatus() {
        return messageStatus;
    }

    public void setMessageStatus(String messageStatus) {
        this.messageStatus = messageStatus;
    }

    public boolean getMessageStatusSent() {
        return messageStatusSent;
    }

    public void setMessageStatusSent(boolean messageStatusSent) {
        this.messageStatusSent = messageStatusSent;
    }

    public String getConsignee() {
        return consignee;
    }

    public void setConsignee(String consignee) {
        this.consignee = consignee;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getCompany() {
        return company;
    }

    public void setCompany(String company) {
        this.company = company;
    }

    public String getConfirmStatus() {
        return confirmStatus;
    }

    public void setConfirmStatus(String confirmStatus) {
        this.confirmStatus = confirmStatus;
    }

    public String getConfirmMessage() {
        return confirmMessage;
    }

    public void setConfirmMessage(String confirmMessage) {
        this.confirmMessage = confirmMessage;
    }

    public boolean getShowButton() {
        return showButton;
    }

    public void setShowButton(boolean showButton) {
        this.showButton = showButton;
    }

    public String getEstimateDeliverTime() {
        return estimateDeliverTime;
    }

    public void setEstimateDeliverTime(String estimateDeliverTime) {
        this.estimateDeliverTime = estimateDeliverTime;
    }

    public String getActualDeliverTime() {
        return actualDeliverTime;
    }

    public void setActualDeliverTime(String actualDeliverTime) {
        this.actualDeliverTime = actualDeliverTime;
    }

    public String getDeliverTimeDiff() {
        return deliverTimeDiff;
    }

    public void setDeliverTimeDiff(String deliverTimeDiff) {
        this.deliverTimeDiff = deliverTimeDiff;
    }
}
