package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class ExpressShelfOnShelfQueryResponseInfo extends ResponseInfo {

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

    @SerializedName("addTime")
    private String addTime;

    public String getAddTime() {
        return addTime;
    }

    public void setAddTime(String addTime) {
        this.addTime = addTime;
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

    @SerializedName("expressStorageLocName")
    private String expressStorageLocName;

    public String getExpressStorageLocName() {
        return expressStorageLocName;
    }

    public void setExpressStorageLocName(String expressStorageLocName) {
        this.expressStorageLocName = expressStorageLocName;
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
    private boolean match;

    public boolean isMatch() {
        return match;
    }

    public void setMatch(boolean match) {
        this.match = match;
    }
}