package com.huadu.shunda.data.http.expressdump;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressDumpWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressDumpWarehouseItemInfo> itemList = new ArrayList<>();

    public List<ExpressDumpWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressDumpWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
