package com.huadu.shunda.data.http.expressdump;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressDumpStorageLocListResponseInfo extends ResponseInfo {

    @SerializedName("warehouseId")
    private int warehouseId;

    public int getWarehouseId() {
        return warehouseId;
    }

    public void setWarehouseId(int warehouseId) {
        this.warehouseId = warehouseId;
    }

    @SerializedName("warehouseName")
    private String warehouseName;

    public String getWarehouseName() {
        return warehouseName;
    }

    public void setWarehouseName(String warehouseName) {
        this.warehouseName = warehouseName;
    }

    @SerializedName("shelfId")
    private int shelfId;

    public int getShelfId() {
        return shelfId;
    }

    public void setShelfId(int shelfId) {
        this.shelfId = shelfId;
    }

    @SerializedName("shelfName")
    private String shelfName;

    public String getShelfName() {
        return shelfName;
    }

    public void setShelfName(String shelfName) {
        this.shelfName = shelfName;
    }

    @SerializedName("itemList")
    private List<ExpressDumpStorageLocItemInfo> itemList = new ArrayList<>();

    public List<ExpressDumpStorageLocItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressDumpStorageLocItemInfo> itemList) {
        this.itemList = itemList;
    }
}
