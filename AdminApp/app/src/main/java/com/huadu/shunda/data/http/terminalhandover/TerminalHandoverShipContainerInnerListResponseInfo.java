package com.huadu.shunda.data.http.terminalhandover;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class TerminalHandoverShipContainerInnerListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<TerminalHandoverShipContainerInnerListItemInfo> itemList = new ArrayList<>();

    public List<TerminalHandoverShipContainerInnerListItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<TerminalHandoverShipContainerInnerListItemInfo> itemList) {
        this.itemList = itemList;
    }
}
