package com.huadu.iepoch.data.http.homedeliverpickup;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class HomeDeliverPickupWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<HomeDeliverPickupWarehouseItemInfo> itemList = new ArrayList<>();

    public List<HomeDeliverPickupWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<HomeDeliverPickupWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
