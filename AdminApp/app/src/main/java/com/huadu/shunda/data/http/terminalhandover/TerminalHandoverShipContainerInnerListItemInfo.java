package com.huadu.shunda.data.http.terminalhandover;

import com.google.gson.annotations.SerializedName;

public class TerminalHandoverShipContainerInnerListItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("shipContainerInnerId")
    private int shipContainerInnerId;

    @SerializedName("shipContainerInnerCode")
    private String shipContainerInnerCode;

    @SerializedName("toWarehouseId")
    private int toWarehouseId;

    @SerializedName("toWarehouseName")
    private String toWarehouseName;

    @SerializedName("actualCheckinTime")
    private String actualCheckinTime;

    @SerializedName("totalPackageCount")
    private String totalPackageCount;

    @SerializedName("totalVolume")
    private String totalVolume;

    @SerializedName("totalActualWeight")
    private String totalActualWeight;

    @SerializedName("maxSideLength")
    private String maxSideLength;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getShipContainerInnerId() {
        return shipContainerInnerId;
    }

    public void setShipContainerInnerId(int shipContainerInnerId) {
        this.shipContainerInnerId = shipContainerInnerId;
    }

    public String getShipContainerInnerCode() {
        return shipContainerInnerCode;
    }

    public void setShipContainerInnerCode(String shipContainerInnerCode) {
        this.shipContainerInnerCode = shipContainerInnerCode;
    }

    public int getToWarehouseId() {
        return toWarehouseId;
    }

    public void setToWarehouseId(int toWarehouseId) {
        this.toWarehouseId = toWarehouseId;
    }

    public String getToWarehouseName() {
        return toWarehouseName;
    }

    public void setToWarehouseName(String toWarehouseName) {
        this.toWarehouseName = toWarehouseName;
    }

    public String getActualCheckinTime() {
        return actualCheckinTime;
    }

    public void setActualCheckinTime(String actualCheckinTime) {
        this.actualCheckinTime = actualCheckinTime;
    }

    public String getTotalPackageCount() {
        return totalPackageCount;
    }

    public void setTotalPackageCount(String totalPackageCount) {
        this.totalPackageCount = totalPackageCount;
    }

    public String getTotalVolume() {
        return totalVolume;
    }

    public void setTotalVolume(String totalVolume) {
        this.totalVolume = totalVolume;
    }

    public String getTotalActualWeight() {
        return totalActualWeight;
    }

    public void setTotalActualWeight(String totalActualWeight) {
        this.totalActualWeight = totalActualWeight;
    }

    public String getMaxSideLength() {
        return maxSideLength;
    }

    public void setMaxSideLength(String maxSideLength) {
        this.maxSideLength = maxSideLength;
    }
}
