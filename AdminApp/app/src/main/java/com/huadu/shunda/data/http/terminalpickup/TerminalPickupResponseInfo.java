package com.huadu.shunda.data.http.terminalpickup;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class TerminalPickupResponseInfo extends ResponseInfo {

    @SerializedName("pickupStationId")
    private int pickupStationId;

    @SerializedName("pickupCode")
    private String pickupCode;

    @SerializedName("canPickup")
    private boolean canPickup;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("arrearsAmount")
    private String arrearsAmount;

    @SerializedName("totalPackBoxCount")
    private int totalPackBoxCount;

    @SerializedName("shipContainerCustomerCode")
    private String shipContainerCustomerCode;

    @SerializedName("orderStatus")
    private String orderStatus;

    @SerializedName("deliverPackBoxCount")
    private int deliverPackBoxCount;

    @SerializedName("notHandoverPackBoxCount")
    private int notHandoverPackBoxCount;

    @SerializedName("notHandoverPackBoxNums")
    private String notHandoverPackBoxNums;

    @SerializedName("description")
    private String description;

    public int getPickupStationId() {
        return pickupStationId;
    }

    public void setPickupStationId(int pickupStationId) {
        this.pickupStationId = pickupStationId;
    }

    public String getPickupCode() {
        return pickupCode;
    }

    public void setPickupCode(String pickupCode) {
        this.pickupCode = pickupCode;
    }

    public boolean getCanPickup() {
        return canPickup;
    }

    public void setCanPickup(boolean canPickup) {
        this.canPickup = canPickup;
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

    public String getArrearsAmount() {
        return arrearsAmount;
    }

    public void setArrearsAmount(String arrearsAmount) {
        this.arrearsAmount = arrearsAmount;
    }

    public int getTotalPackBoxCount() {
        return totalPackBoxCount;
    }

    public void setTotalPackBoxCount(int totalPackBoxCount) {
        this.totalPackBoxCount = totalPackBoxCount;
    }

    public String getShipContainerCustomerCode() {
        return shipContainerCustomerCode;
    }

    public void setShipContainerCustomerCode(String shipContainerCustomerCode) {
        this.shipContainerCustomerCode = shipContainerCustomerCode;
    }

    public String getOrderStatus() {
        return orderStatus;
    }

    public void setOrderStatus(String orderStatus) {
        this.orderStatus = orderStatus;
    }

    public int getDeliverPackBoxCount() {
        return deliverPackBoxCount;
    }

    public void setDeliverPackBoxCount(int deliverPackBoxCount) {
        this.deliverPackBoxCount = deliverPackBoxCount;
    }

    public int getNotHandoverPackBoxCount() {
        return notHandoverPackBoxCount;
    }

    public void setNotHandoverPackBoxCount(int notHandoverPackBoxCount) {
        this.notHandoverPackBoxCount = notHandoverPackBoxCount;
    }

    public String getNotHandoverPackBoxNums() {
        return notHandoverPackBoxNums;
    }

    public void setNotHandoverPackBoxNums(String notHandoverPackBoxNums) {
        this.notHandoverPackBoxNums = notHandoverPackBoxNums;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}
