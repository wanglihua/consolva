package com.huadu.shunda.data.http.stockcheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockCheckStockPlaceListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<StockCheckStockPlaceItemInfo> itemList = new ArrayList<>();

    public List<StockCheckStockPlaceItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<StockCheckStockPlaceItemInfo> itemList) {
        this.itemList = itemList;
    }
}
