package com.huadu.shunda.data.http.stockcheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockCheckStockPlaceDetailResponseInfo extends ResponseInfo {

    @SerializedName("name")
    private String name;

    @SerializedName("packBoxItemCount")
    private int packBoxItemCount;

    @SerializedName("packBoxItemList")
    private List<StockCheckPackBoxItemInfo> packBoxItemList = new ArrayList<>();

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getPackBoxItemCount() {
        return packBoxItemCount;
    }

    public void setPackBoxItemCount(int packBoxItemCount) {
        this.packBoxItemCount = packBoxItemCount;
    }

    public List<StockCheckPackBoxItemInfo> getPackBoxItemList() {
        return packBoxItemList;
    }

    public void setPackBoxItemList(List<StockCheckPackBoxItemInfo> packBoxItemList) {
        this.packBoxItemList = packBoxItemList;
    }
}
