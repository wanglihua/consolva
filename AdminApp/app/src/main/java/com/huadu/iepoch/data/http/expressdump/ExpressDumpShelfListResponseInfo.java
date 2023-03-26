package com.huadu.iepoch.data.http.expressdump;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressDumpShelfListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ExpressDumpShelfItemInfo> itemList = new ArrayList<>();

    public List<ExpressDumpShelfItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ExpressDumpShelfItemInfo> itemList) {
        this.itemList = itemList;
    }
}
