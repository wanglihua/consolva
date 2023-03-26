package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressReturnOffShelfExpressListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressReturnOffShelfExpressItemInfo> itemList = new ArrayList<>();

    public List<ExpressReturnOffShelfExpressItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressReturnOffShelfExpressItemInfo> itemList) {
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
