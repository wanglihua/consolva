package com.huadu.iepoch.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class ExpressShelfPackingQueryResponseInfo extends ResponseInfo {

    @SerializedName("aoPinTrackingNo")
    private String aoPinTrackingNo;

    public String getAoPinTrackingNo() {
        return aoPinTrackingNo;
    }

    public void setAoPinTrackingNo(String aoPinTrackingNo) {
        this.aoPinTrackingNo = aoPinTrackingNo;
    }

    @SerializedName("trackingNo")
    private String trackingNo;

    public String getTrackingNo() {
        return trackingNo;
    }

    public void setTrackingNo(String trackingNo) {
        this.trackingNo = trackingNo;
    }

    @SerializedName("orderCode")
    private String orderCode;

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    @SerializedName("addTime")
    private String addTime;

    public String getAddTime() {
        return addTime;
    }

    public void setAddTime(String addTime) {
        this.addTime = addTime;
    }

    @SerializedName("offShelfTime")
    private String offShelfTime;

    public String getOffShelfTime() {
        return offShelfTime;
    }

    public void setOffShelfTime(String offShelfTime) {
        this.offShelfTime = offShelfTime;
    }

    @SerializedName("packageCount")
    private int packageCount;

    public int getPackageCount() {
        return packageCount;
    }

    public void setPackageCount(int packageCount) {
        this.packageCount = packageCount;
    }

    @SerializedName("status")
    private String status;

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    @SerializedName("shelfStatus")
    private String shelfStatus;

    public String getShelfStatus() {
        return shelfStatus;
    }

    public void setShelfStatus(String shelfStatus) {
        this.shelfStatus = shelfStatus;
    }

    @SerializedName("orderPackingStorageLocName")
    private String orderPackingStorageLocName;

    public String getOrderPackingStorageLocName() {
        return orderPackingStorageLocName;
    }

    public void setOrderPackingStorageLocName(String orderPackingStorageLocName) {
        this.orderPackingStorageLocName = orderPackingStorageLocName;
    }

    @SerializedName("storageLocCode")
    private String storageLocCode;

    public String getStorageLocCode() {
        return storageLocCode;
    }

    public void setStorageLocCode(String storageLocCode) {
        this.storageLocCode = storageLocCode;
    }

    @SerializedName("storageLocName")
    private String storageLocName;

    public String getStorageLocName() {
        return storageLocName;
    }

    public void setStorageLocName(String storageLocName) {
        this.storageLocName = storageLocName;
    }

    @SerializedName("storageLocExpressCount")
    private int storageLocExpressCount;

    public int getStorageLocExpressCount() {
        return storageLocExpressCount;
    }

    public void setStorageLocExpressCount(int storageLocExpressCount) {
        this.storageLocExpressCount = storageLocExpressCount;
    }

    @SerializedName("storageLocPackageCount")
    private int storageLocPackageCount;

    public int getStorageLocPackageCount() {
        return storageLocPackageCount;
    }

    public void setStorageLocPackageCount(int storageLocPackageCount) {
        this.storageLocPackageCount = storageLocPackageCount;
    }

    @SerializedName("match")
    private Boolean match;

    public Boolean isMatch() {
        return match;
    }

    public void setMatch(Boolean match) {
        this.match = match;
    }
}