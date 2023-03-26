package com.huadu.shunda.data.http.terminalhandover;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class TerminalHandoverNotHandoverListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<TerminalHandoverNotHandoverListItemInfo> itemList = new ArrayList<>();

    public List<TerminalHandoverNotHandoverListItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<TerminalHandoverNotHandoverListItemInfo> itemList) {
        this.itemList = itemList;
    }
}
