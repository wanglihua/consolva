package com.huadu.shunda.data.http.shipcontainerinner;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ShipContainerInnerLoadDetailDataResponseInfo extends ResponseInfo {
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

    @SerializedName("containerVolume")
    private String containerVolume;

    @SerializedName("actualImportVolume")
    private String actualImportVolume;

    @SerializedName("itemList")
    private List<ShipContainerNotLoadListItemInfo> itemList = new ArrayList<>();

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

    public List<ShipContainerNotLoadListItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ShipContainerNotLoadListItemInfo> itemList) {
        this.itemList = itemList;
    }
}
