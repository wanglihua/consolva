package com.huadu.shunda.data.http.expressoffshelf;

import com.google.gson.annotations.SerializedName;

public class ExpressOffShelfExpressListItem {

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

    @SerializedName("checkinTrackingNo")
    private String checkinTrackingNo;

    public String getCheckinTrackingNo() {
        return checkinTrackingNo;
    }

    public void setCheckinTrackingNo(String checkinTrackingNo) {
        this.checkinTrackingNo = checkinTrackingNo;
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

    @SerializedName("addUserName")
    private String addUserName;

    public String getAddUserName() {
        return addUserName;
    }

    public void setAddUserName(String addUserName) {
        this.addUserName = addUserName;
    }

    @SerializedName("addTime")
    private String addTime;

    public String getAddTime() {
        return addTime;
    }

    public void setAddTime(String addTime) {
        this.addTime = addTime;
    }

    @SerializedName("notOnShelf")
    private boolean notOnShelf;

    public boolean getNotOnShelf() {
        return notOnShelf;
    }

    public void setNotOnShelf(boolean notOnShelf) {
        this.notOnShelf = notOnShelf;
    }

    @SerializedName("shelfStatus")
    private String shelfStatus;

    public String getShelfStatus() {
        return shelfStatus;
    }

    public void setShelfStatus(String shelfStatus) {
        this.shelfStatus = shelfStatus;
    }
}
