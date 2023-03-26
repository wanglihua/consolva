package com.huadu.shunda.data.http.shipcontainerunloadsortpackbox;

import com.google.gson.annotations.SerializedName;

public class ShipContainerUnloadSortPackBoxDetailItem {

    @SerializedName("shipContainerOrderPackBoxId")
    private int shipContainerOrderPackBoxId;

    @SerializedName("orderPackBoxId")
    private int orderPackBoxId;

    @SerializedName("shipContainerCode")
    private String shipContainerCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("orderPackBoxCode")
    private String orderPackBoxCode;

    @SerializedName("orderPackBoxShortCode")
    private String orderPackBoxShortCode;

    @SerializedName("positionName")
    private String positionName;

    @SerializedName("orderPositionNames")
    private String orderPositionNames;

    @SerializedName("orderFirstPosition")
    private String orderFirstPosition;

    @SerializedName("totalBoxCount")
    private String totalBoxCount;

    @SerializedName("scannedBoxCount")
    private String scannedBoxCount;

    @SerializedName("noScannedBoxCount")
    private String noScannedBoxCount;

    public int getShipContainerOrderPackBoxId() {
        return shipContainerOrderPackBoxId;
    }

    public void setShipContainerOrderPackBoxId(int shipContainerOrderPackBoxId) {
        this.shipContainerOrderPackBoxId = shipContainerOrderPackBoxId;
    }

    public int getOrderPackBoxId() {
        return orderPackBoxId;
    }

    public void setOrderPackBoxId(int orderPackBoxId) {
        this.orderPackBoxId = orderPackBoxId;
    }

    public String getShipContainerCode() {
        return shipContainerCode;
    }

    public void setShipContainerCode(String shipContainerCode) {
        this.shipContainerCode = shipContainerCode;
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

    public String getOrderPackBoxCode() {
        return orderPackBoxCode;
    }

    public void setOrderPackBoxCode(String orderPackBoxCode) {
        this.orderPackBoxCode = orderPackBoxCode;
    }

    public String getOrderPackBoxShortCode() {
        return orderPackBoxShortCode;
    }

    public void setOrderPackBoxShortCode(String orderPackBoxShortCode) {
        this.orderPackBoxShortCode = orderPackBoxShortCode;
    }

    public String getPositionName() {
        return positionName;
    }

    public void setPositionName(String positionName) {
        this.positionName = positionName;
    }

    public String getOrderPositionNames() {
        return orderPositionNames;
    }

    public void setOrderPositionNames(String orderPositionNames) {
        this.orderPositionNames = orderPositionNames;
    }

    public String getOrderFirstPosition() {
        return orderFirstPosition;
    }

    public void setOrderFirstPosition(String orderFirstPosition) {
        this.orderFirstPosition = orderFirstPosition;
    }

    public String getTotalBoxCount() {
        return totalBoxCount;
    }

    public void setTotalBoxCount(String totalBoxCount) {
        this.totalBoxCount = totalBoxCount;
    }

    public String getScannedBoxCount() {
        return scannedBoxCount;
    }

    public void setScannedBoxCount(String scannedBoxCount) {
        this.scannedBoxCount = scannedBoxCount;
    }

    public String getNoScannedBoxCount() {
        return noScannedBoxCount;
    }

    public void setNoScannedBoxCount(String noScannedBoxCount) {
        this.noScannedBoxCount = noScannedBoxCount;
    }
}
