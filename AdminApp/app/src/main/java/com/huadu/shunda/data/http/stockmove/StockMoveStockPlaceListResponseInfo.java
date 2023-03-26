package com.huadu.shunda.data.http.stockmove;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockMoveStockPlaceListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<StockMoveStockPlaceItemInfo> itemList = new ArrayList<>();

    public List<StockMoveStockPlaceItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<StockMoveStockPlaceItemInfo> itemList) {
        this.itemList = itemList;
    }
}
