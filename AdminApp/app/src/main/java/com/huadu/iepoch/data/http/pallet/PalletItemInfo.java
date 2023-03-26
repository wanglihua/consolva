package com.huadu.iepoch.data.http.pallet;

import com.google.gson.annotations.SerializedName;

public class PalletItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("code")
    private String code;

    @SerializedName("name")
    private String name;

    @SerializedName("hasGoods")
    private String hasGoods;

    @SerializedName("statusName")
    private String statusName;

    @SerializedName("warehouseId")
    private int warehouseId;

    @SerializedName("warehouseName")
    private String warehouseName;

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

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getHasGoods() {
        return hasGoods;
    }

    public void setHasGoods(String hasGoods) {
        this.hasGoods = hasGoods;
    }

    public String getStatusName() {
        return statusName;
    }

    public void setStatusName(String statusName) {
        this.statusName = statusName;
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
}
