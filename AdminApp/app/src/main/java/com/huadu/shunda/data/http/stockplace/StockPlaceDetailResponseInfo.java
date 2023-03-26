package com.huadu.shunda.data.http.stockplace;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class StockPlaceDetailResponseInfo extends ResponseInfo {

    @SerializedName("name")
    private String name;

    @SerializedName("packBoxItemCount")
    private int packBoxItemCount;

    @SerializedName("packBoxItemList")
    private List<StockPlacePackBoxItemInfo> packBoxItemList = new ArrayList<>();

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

    public List<StockPlacePackBoxItemInfo> getPackBoxItemList() {
        return packBoxItemList;
    }

    public void setPackBoxItemList(List<StockPlacePackBoxItemInfo> packBoxItemList) {
        this.packBoxItemList = packBoxItemList;
    }
}
