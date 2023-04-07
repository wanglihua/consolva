package com.huadu.shunda.data.http.expressoffshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressOffShelfExpressListResponseData extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressOffShelfExpressListItem> itemList = new ArrayList<>();

    public List<ExpressOffShelfExpressListItem> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressOffShelfExpressListItem> itemList) {
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
