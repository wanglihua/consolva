package com.huadu.shunda.data.http.stockplace;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockPlaceListDataResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<StockPlaceItemInfo> itemList = new ArrayList<>();

    public List<StockPlaceItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<StockPlaceItemInfo> itemList) {
        this.itemList = itemList;
    }
}
