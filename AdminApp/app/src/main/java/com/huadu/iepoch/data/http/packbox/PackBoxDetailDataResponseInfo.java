package com.huadu.iepoch.data.http.packbox;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class PackBoxDetailDataResponseInfo extends ResponseInfo {

    @SerializedName("orderPackBoxCode")
    private String orderPackBoxCode;

    @SerializedName("orderPackBoxShortCode")
    private String orderPackBoxShortCode;

    @SerializedName("orderPackBoxNum")
    private String orderPackBoxNum;

    @SerializedName("orderPackBoxCount")
    private String orderPackBoxCount;

    @SerializedName("orderPackBoxUnloadCount")
    private String orderPackBoxUnloadCount;

    @SerializedName("orderPackBoxStatus")
    private String orderPackBoxStatus;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("auSortContainerCode")
    private String auSortContainerCode;

    @SerializedName("shipContainerInnerCode")
    private String shipContainerInnerCode;

    @SerializedName("shipContainerCode")
    private String shipContainerCode;

    @SerializedName("targetWarehouseName")
    private String targetWarehouseName;

    @SerializedName("firstLogisticsTypeName")
    private String firstLogisticsTypeName;

    @SerializedName("firstLogisticsModeName")
    private String firstLogisticsModeName;

    @SerializedName("secondLogisticsTypeName")
    private String secondLogisticsTypeName;

    @SerializedName("pickupStationName")
    private String pickupStationName;

    @SerializedName("orderGoodsTypeName")
    private String orderGoodsTypeName;

    @SerializedName("expressGoodsTypeName")
    private String expressGoodsTypeName;

    @SerializedName("weight")
    private String weight;

    @SerializedName("volumeWeight")
    private String volumeWeight;

    @SerializedName("volume")
    private String volume;

    @SerializedName("userName")
    private String userName;

    @SerializedName("packTime")
    private String packTime;

    @SerializedName("customerMobile")
    private String customerMobile;

    @SerializedName("orderMobile")
    private String orderMobile;

    @SerializedName("consignee")
    private String consignee;

    @SerializedName("address")
    private String address;

    @SerializedName("zipcode")
    private String zipcode;

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

    public String getOrderPackBoxNum() {
        return orderPackBoxNum;
    }

    public void setOrderPackBoxNum(String orderPackBoxNum) {
        this.orderPackBoxNum = orderPackBoxNum;
    }

    public String getOrderPackBoxCount() {
        return orderPackBoxCount;
    }

    public void setOrderPackBoxCount(String orderPackBoxCount) {
        this.orderPackBoxCount = orderPackBoxCount;
    }

    public String getOrderPackBoxUnloadCount() {
        return orderPackBoxUnloadCount;
    }

    public void setOrderPackBoxUnloadCount(String orderPackBoxUnloadCount) {
        this.orderPackBoxUnloadCount = orderPackBoxUnloadCount;
    }

    public String getOrderPackBoxStatus() {
        return orderPackBoxStatus;
    }

    public void setOrderPackBoxStatus(String orderPackBoxStatus) {
        this.orderPackBoxStatus = orderPackBoxStatus;
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

    public String getAuSortContainerCode() {
        return auSortContainerCode;
    }

    public void setAuSortContainerCode(String auSortContainerCode) {
        this.auSortContainerCode = auSortContainerCode;
    }

    public String getShipContainerInnerCode() {
        return shipContainerInnerCode;
    }

    public void setShipContainerInnerCode(String shipContainerInnerCode) {
        this.shipContainerInnerCode = shipContainerInnerCode;
    }

    public String getShipContainerCode() {
        return shipContainerCode;
    }

    public void setShipContainerCode(String shipContainerCode) {
        this.shipContainerCode = shipContainerCode;
    }

    public String getTargetWarehouseName() {
        return targetWarehouseName;
    }

    public void setTargetWarehouseName(String targetWarehouseName) {
        this.targetWarehouseName = targetWarehouseName;
    }

    public String getFirstLogisticsTypeName() {
        return firstLogisticsTypeName;
    }

    public void setFirstLogisticsTypeName(String firstLogisticsTypeName) {
        this.firstLogisticsTypeName = firstLogisticsTypeName;
    }

    public String getFirstLogisticsModeName() {
        return firstLogisticsModeName;
    }

    public void setFirstLogisticsModeName(String firstLogisticsModeName) {
        this.firstLogisticsModeName = firstLogisticsModeName;
    }

    public String getSecondLogisticsTypeName() {
        return secondLogisticsTypeName;
    }

    public void setSecondLogisticsTypeName(String secondLogisticsTypeName) {
        this.secondLogisticsTypeName = secondLogisticsTypeName;
    }

    public String getPickupStationName() {
        return pickupStationName;
    }

    public void setPickupStationName(String pickupStationName) {
        this.pickupStationName = pickupStationName;
    }

    public String getOrderGoodsTypeName() {
        return orderGoodsTypeName;
    }

    public void setOrderGoodsTypeName(String orderGoodsTypeName) {
        this.orderGoodsTypeName = orderGoodsTypeName;
    }

    public String getExpressGoodsTypeName() {
        return expressGoodsTypeName;
    }

    public void setExpressGoodsTypeName(String expressGoodsTypeName) {
        this.expressGoodsTypeName = expressGoodsTypeName;
    }

    public String getWeight() {
        return weight;
    }

    public void setWeight(String weight) {
        this.weight = weight;
    }

    public String getVolumeWeight() {
        return volumeWeight;
    }

    public void setVolumeWeight(String volumeWeight) {
        this.volumeWeight = volumeWeight;
    }

    public String getVolume() {
        return volume;
    }

    public void setVolume(String volume) {
        this.volume = volume;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getPackTime() {
        return packTime;
    }

    public void setPackTime(String packTime) {
        this.packTime = packTime;
    }

    public String getCustomerMobile() {
        return customerMobile;
    }

    public void setCustomerMobile(String customerMobile) {
        this.customerMobile = customerMobile;
    }

    public String getOrderMobile() {
        return orderMobile;
    }

    public void setOrderMobile(String orderMobile) {
        this.orderMobile = orderMobile;
    }

    public String getConsignee() {
        return consignee;
    }

    public void setConsignee(String consignee) {
        this.consignee = consignee;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getZipcode() {
        return zipcode;
    }

    public void setZipcode(String zipcode) {
        this.zipcode = zipcode;
    }
}


