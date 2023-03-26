package com.huadu.iepoch.data.http.stockcheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockCheckWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<StockCheckWarehouseItemInfo> itemList = new ArrayList<>();

    public List<StockCheckWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<StockCheckWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
