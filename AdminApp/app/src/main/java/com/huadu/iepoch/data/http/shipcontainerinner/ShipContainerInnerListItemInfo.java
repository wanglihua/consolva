package com.huadu.iepoch.data.http.shipcontainerinner;

import com.google.gson.annotations.SerializedName;

public class ShipContainerInnerListItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("code")
    private String code;

    @SerializedName("goodsTypeName")
    private String goodsTypeName;

    @SerializedName("containerCode")
    private String containerCode;

    @SerializedName("toWarehouseId")
    private int toWarehouseId;

    @SerializedName("toWarehouseName")
    private String toWarehouseName;

    @SerializedName("actualArriveTime")
    private String actualArriveTime;

    @SerializedName("actualCheckinTime")
    private String actualCheckinTime;

    @SerializedName("containerVolume")
    private String containerVolume;

    @SerializedName("actualImportVolume")
    private String actualImportVolume;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getCode() {
        return code;
    }

    public void setCode(String code) {
        this.code = code;
    }

    public String getGoodsTypeName() {
        return goodsTypeName;
    }

    public void setGoodsTypeName(String goodsTypeName) {
        this.goodsTypeName = goodsTypeName;
    }

    public String getContainerCode() {
        return containerCode;
    }

    public void setContainerCode(String containerCode) {
        this.containerCode = containerCode;
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

    public String getActualArriveTime() {
        return actualArriveTime;
    }

    public void setActualArriveTime(String actualArriveTime) {
        this.actualArriveTime = actualArriveTime;
    }

    public String getActualCheckinTime() {
        return actualCheckinTime;
    }

    public void setActualCheckinTime(String actualCheckinTime) {
        this.actualCheckinTime = actualCheckinTime;
    }

    public String getContainerVolume() {
        return containerVolume;
    }

    public void setContainerVolume(String containerVolume) {
        this.containerVolume = containerVolume;
    }

    public String getActualImportVolume() {
        return actualImportVolume;
    }

    public void setActualImportVolume(String actualImportVolume) {
        this.actualImportVolume = actualImportVolume;
    }
}
