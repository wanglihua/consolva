package com.huadu.shunda.data.http.homedeliverbatch;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class HomeDeliverBatchWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<HomeDeliverBatchWarehouseItemInfo> itemList = new ArrayList<>();

    public List<HomeDeliverBatchWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<HomeDeliverBatchWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
