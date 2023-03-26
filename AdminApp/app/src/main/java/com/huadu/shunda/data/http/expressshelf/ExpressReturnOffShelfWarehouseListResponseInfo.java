package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressReturnOffShelfWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressReturnOffShelfWarehouseItemInfo> itemList = new ArrayList<>();

    public List<ExpressReturnOffShelfWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressReturnOffShelfWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
