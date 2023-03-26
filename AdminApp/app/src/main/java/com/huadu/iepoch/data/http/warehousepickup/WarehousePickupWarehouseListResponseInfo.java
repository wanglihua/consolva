package com.huadu.iepoch.data.http.warehousepickup;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class WarehousePickupWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<WarehousePickupWarehouseItemInfo> itemList = new ArrayList<>();

    public List<WarehousePickupWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<WarehousePickupWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
