package com.huadu.shunda.data.http.expressdump;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressDumpExpressListResponseInfo extends ResponseInfo {

    @SerializedName("shelfName")
    private String shelfName;

    public String getShelfName() {
        return shelfName;
    }

    public void setShelfName(String shelfName) {
        this.shelfName = shelfName;
    }

    @SerializedName("itemList")
    private List<ExpressDumpExpressItemInfo> itemList = new ArrayList<>();

    public List<ExpressDumpExpressItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressDumpExpressItemInfo> itemList) {
        this.itemList = itemList;
    }
}
