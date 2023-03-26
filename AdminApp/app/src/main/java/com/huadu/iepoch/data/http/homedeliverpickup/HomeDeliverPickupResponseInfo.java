package com.huadu.iepoch.data.http.homedeliverpickup;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class HomeDeliverPickupResponseInfo extends ResponseInfo {

    @SerializedName("warehouseId")
    private int warehouseId;

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

    @SerializedName("notUnloadPackBoxCount")
    private int notUnloadPackBoxCount;

    @SerializedName("notUnloadPackBoxNums")
    private String notUnloadPackBoxNums;

    @SerializedName("description")
    private String description;

    public int getWarehouseId() {
        return warehouseId;
    }

    public void setWarehouseId(int warehouseId) {
        this.warehouseId = warehouseId;
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

    public int getNotUnloadPackBoxCount() {
        return notUnloadPackBoxCount;
    }

    public void setNotUnloadPackBoxCount(int notUnloadPackBoxCount) {
        this.notUnloadPackBoxCount = notUnloadPackBoxCount;
    }

    public String getNotUnloadPackBoxNums() {
        return notUnloadPackBoxNums;
    }

    public void setNotUnloadPackBoxNums(String notUnloadPackBoxNums) {
        this.notUnloadPackBoxNums = notUnloadPackBoxNums;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}
