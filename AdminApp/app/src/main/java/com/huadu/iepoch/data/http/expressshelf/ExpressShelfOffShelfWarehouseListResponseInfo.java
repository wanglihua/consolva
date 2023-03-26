package com.huadu.iepoch.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressShelfOffShelfWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressShelfOffShelfWarehouseItemInfo> itemList = new ArrayList<>();

    public List<ExpressShelfOffShelfWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressShelfOffShelfWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
