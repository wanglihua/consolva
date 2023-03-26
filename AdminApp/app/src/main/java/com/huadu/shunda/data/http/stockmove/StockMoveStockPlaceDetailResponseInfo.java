package com.huadu.shunda.data.http.stockmove;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockMoveStockPlaceDetailResponseInfo extends ResponseInfo {

    @SerializedName("name")
    private String name;

    @SerializedName("packBoxItemCount")
    private int packBoxItemCount;

    @SerializedName("packBoxItemList")
    private List<StockMovePackBoxItemInfo> packBoxItemList = new ArrayList<>();

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

    public List<StockMovePackBoxItemInfo> getPackBoxItemList() {
        return packBoxItemList;
    }

    public void setPackBoxItemList(List<StockMovePackBoxItemInfo> packBoxItemList) {
        this.packBoxItemList = packBoxItemList;
    }
}
