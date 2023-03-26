package com.huadu.iepoch.data.http.stockcheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.stockplace.StockPlaceCheckBillItemInfo;

import java.util.ArrayList;
import java.util.List;

public class StockCheckBillListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<StockPlaceCheckBillItemInfo> itemList = new ArrayList<>();

    public List<StockPlaceCheckBillItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<StockPlaceCheckBillItemInfo> itemList) {
        this.itemList = itemList;
    }
}
