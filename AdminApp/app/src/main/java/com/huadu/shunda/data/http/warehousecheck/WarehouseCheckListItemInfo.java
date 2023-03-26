package com.huadu.shunda.data.http.warehousecheck;

import com.google.gson.annotations.SerializedName;

public class WarehouseCheckListItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("trackingNo")
    private String trackingNo;

    @SerializedName("aoPinTrackingNo")
    private String aoPinTrackingNo;

    @SerializedName("statusName")
    private String statusName;

    @SerializedName("shelfName")
    private String shelfName;

    @SerializedName("storageLocFullName")
    private String storageLocFullName;

    @SerializedName("claimShelfStatusName")
    private String claimShelfStatusName;

    @SerializedName("chatMessage")
    private String chatMessage;

    @SerializedName("replyMessage")
    private String replyMessage;

    @SerializedName("chatCustomerCode")
    private String chatCustomerCode;

    @SerializedName("chatUserLoginName")
    private String chatUserLoginName;

    @SerializedName("createTime")
    private String createTime;

    @SerializedName("updateTime")
    private String updateTime;

    @SerializedName("toOnShelf")
    public boolean toOnShelf;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getTrackingNo() {
        return trackingNo;
    }

    public void setTrackingNo(String trackingNo) {
        this.trackingNo = trackingNo;
    }

    public String getAoPinTrackingNo() {
        return aoPinTrackingNo;
    }

    public void setAoPinTrackingNo(String aoPinTrackingNo) {
        this.aoPinTrackingNo = aoPinTrackingNo;
    }

    public String getStatusName() {
        return statusName;
    }

    public void setStatusName(String statusName) {
        this.statusName = statusName;
    }

    public String getShelfName() {
        return shelfName;
    }

    public void setShelfName(String shelfName) {
        this.shelfName = shelfName;
    }

    public String getStorageLocFullName() {
        return storageLocFullName;
    }

    public void setStorageLocFullName(String storageLocFullName) {
        this.storageLocFullName = storageLocFullName;
    }

    public String getClaimShelfStatusName() {
        return claimShelfStatusName;
    }

    public void setClaimShelfStatusName(String claimShelfStatusName) {
        this.claimShelfStatusName = claimShelfStatusName;
    }

    public String getChatMessage() {
        return chatMessage;
    }

    public void setChatMessage(String chatMessage) {
        this.chatMessage = chatMessage;
    }

    public String getReplyMessage() {
        return replyMessage;
    }

    public void setReplyMessage(String replyMessage) {
        this.replyMessage = replyMessage;
    }

    public String getChatCustomerCode() {
        return chatCustomerCode;
    }

    public void setChatCustomerCode(String chatCustomerCode) {
        this.chatCustomerCode = chatCustomerCode;
    }

    public String getChatUserLoginName() {
        return chatUserLoginName;
    }

    public void setChatUserLoginName(String chatUserLoginName) {
        this.chatUserLoginName = chatUserLoginName;
    }

    public String getCreateTime() {
        return createTime;
    }

    public void setCreateTime(String createTime) {
        this.createTime = createTime;
    }

    public String getUpdateTime() {
        return updateTime;
    }

    public void setUpdateTime(String updateTime) {
        this.updateTime = updateTime;
    }

    public boolean getToOnShelf() {
        return toOnShelf;
    }

    public void setToOnShelf(boolean toOnShelf) {
        this.toOnShelf = toOnShelf;
    }
}
