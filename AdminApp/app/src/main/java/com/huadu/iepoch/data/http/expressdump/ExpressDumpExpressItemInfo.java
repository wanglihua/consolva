package com.huadu.iepoch.data.http.expressdump;

import com.google.gson.annotations.SerializedName;

public class ExpressDumpExpressItemInfo {

    @SerializedName("id")
    private int id;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    @SerializedName("trackingNo")
    private String trackingNo;

    public String getTrackingNo() {
        return trackingNo;
    }

    public void setTrackingNo(String trackingNo) {
        this.trackingNo = trackingNo;
    }

    @SerializedName("aoPinTrackingNo")
    private String aoPinTrackingNo;

    public String getAoPinTrackingNo() {
        return aoPinTrackingNo;
    }

    public void setAoPinTrackingNo(String aoPinTrackingNo) {
        this.aoPinTrackingNo = aoPinTrackingNo;
    }

    @SerializedName("packageCount")
    private int packageCount;

    public int getPackageCount() {
        return packageCount;
    }

    public void setPackageCount(int packageCount) {
        this.packageCount = packageCount;
    }

    @SerializedName("storageLocName")
    private String storageLocName;

    public String getStorageLocName() {
        return storageLocName;
    }

    public void setStorageLocName(String storageLocName) {
        this.storageLocName = storageLocName;
    }

    @SerializedName("customerCode")
    private String customerCode;

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    @SerializedName("addTime")
    private String addTime;

    public String getAddTime() {
        return addTime;
    }

    public void setAddTime(String addTime) {
        this.addTime = addTime;
    }

    @SerializedName("dumpStatus")
    private String dumpStatus;

    public String getDumpStatus() {
        return dumpStatus;
    }

    public void setDumpStatus(String dumpStatus) {
        this.dumpStatus = dumpStatus;
    }
}
