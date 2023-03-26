package com.huadu.iepoch.data.http.terminalhandover;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class TerminalHandoverStationListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<TerminalHandoverStationListItemInfo> itemList = new ArrayList<>();

    public List<TerminalHandoverStationListItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<TerminalHandoverStationListItemInfo> itemList) {
        this.itemList = itemList;
    }
}
