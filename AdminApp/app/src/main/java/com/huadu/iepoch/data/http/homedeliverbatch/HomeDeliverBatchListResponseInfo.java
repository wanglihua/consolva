package com.huadu.iepoch.data.http.homedeliverbatch;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class HomeDeliverBatchListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<HomeDeliverBatchItemInfo> itemList = new ArrayList<>();

    public List<HomeDeliverBatchItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<HomeDeliverBatchItemInfo> itemList) {
        this.itemList = itemList;
    }
}
