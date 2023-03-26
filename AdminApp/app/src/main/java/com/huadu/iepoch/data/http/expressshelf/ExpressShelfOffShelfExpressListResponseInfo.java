package com.huadu.iepoch.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressShelfOffShelfExpressListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressShelfOffShelfExpressItemInfo> itemList = new ArrayList<>();

    public List<ExpressShelfOffShelfExpressItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressShelfOffShelfExpressItemInfo> itemList) {
        this.itemList = itemList;
    }

    @SerializedName("itemCount")
    private int itemCount;

    public int getItemCount() {
        return itemCount;
    }

    public void setItemCount(int itemCount) {
        this.itemCount = itemCount;
    }
}
