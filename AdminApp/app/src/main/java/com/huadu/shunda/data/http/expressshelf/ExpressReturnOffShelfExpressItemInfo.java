package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;

public class ExpressReturnOffShelfExpressItemInfo {

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

    @SerializedName("status")
    private String status;

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
}
