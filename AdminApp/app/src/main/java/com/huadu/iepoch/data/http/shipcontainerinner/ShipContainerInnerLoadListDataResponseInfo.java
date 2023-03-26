package com.huadu.iepoch.data.http.shipcontainerinner;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ShipContainerInnerLoadListDataResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<ShipContainerInnerListItemInfo> itemList = new ArrayList<>();

    public List<ShipContainerInnerListItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<ShipContainerInnerListItemInfo> itemList) {
        this.itemList = itemList;
    }
}
