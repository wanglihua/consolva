package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressShelfOffShelfShelfListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressShelfOffShelfShelfItemInfo> itemList = new ArrayList<>();

    public List<ExpressShelfOffShelfShelfItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressShelfOffShelfShelfItemInfo> itemList) {
        this.itemList = itemList;
    }
}
