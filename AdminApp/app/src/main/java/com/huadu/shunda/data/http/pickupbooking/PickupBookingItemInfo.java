package com.huadu.shunda.data.http.pickupbooking;

import com.google.gson.annotations.SerializedName;

public class PickupBookingItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("orderId")
    private int orderId;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("customerId")
    private int customerId;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("warehouseId")
    private int warehouseId;

    @SerializedName("warehouseName")
    private String warehouseName;

    @SerializedName("pickupStationId")
    private int pickupStationId;

    @SerializedName("pickupStationName")
    private String pickupStationName;

    @SerializedName("pickupBookingTime")
    private String pickupBookingTime;

    @SerializedName("statusId")
    private int statusId;

    @SerializedName("statusName")
    private String statusName;

    @SerializedName("stockPlaceList")
    private String stockPlaceList;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getOrderId() {
        return orderId;
    }

    public void setOrderId(int orderId) {
        this.orderId = orderId;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public int getCustomerId() {
        return customerId;
    }

    public void setCustomerId(int customerId) {
        this.customerId = customerId;
    }

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public int getWarehouseId() {
        return warehouseId;
    }

    public void setWarehouseId(int warehouseId) {
        this.warehouseId = warehouseId;
    }

    public String getWarehouseName() {
        return warehouseName;
    }

    public void setWarehouseName(String warehouseName) {
        this.warehouseName = warehouseName;
    }

    public int getPickupStationId() {
        return pickupStationId;
    }

    public void setPickupStationId(int pickupStationId) {
        this.pickupStationId = pickupStationId;
    }

    public String getPickupStationName() {
        return pickupStationName;
    }

    public void setPickupStationName(String pickupStationName) {
        this.pickupStationName = pickupStationName;
    }

    public String getPickupBookingTime() {
        return pickupBookingTime;
    }

    public void setPickupBookingTime(String pickupBookingTime) {
        this.pickupBookingTime = pickupBookingTime;
    }

    public int getStatusId() {
        return statusId;
    }

    public void setStatusId(int statusId) {
        this.statusId = statusId;
    }

    public String getStatusName() {
        return statusName;
    }

    public void setStatusName(String statusName) {
        this.statusName = statusName;
    }

    public String getStockPlaceList() {
        return stockPlaceList;
    }

    public void setStockPlaceList(String stockPlaceList) {
        this.stockPlaceList = stockPlaceList;
    }
}
