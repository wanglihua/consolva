package com.huadu.iepoch.data.http.stockmove;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockMoveWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<StockMoveWarehouseItemInfo> itemList = new ArrayList<>();

    public List<StockMoveWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<StockMoveWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
