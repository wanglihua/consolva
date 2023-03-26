package com.huadu.iepoch.data.http.terminalpickup;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class TerminalPickupStationListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<TerminalPickupStationListItemInfo> itemList = new ArrayList<>();

    public List<TerminalPickupStationListItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<TerminalPickupStationListItemInfo> itemList) {
        this.itemList = itemList;
    }
}
