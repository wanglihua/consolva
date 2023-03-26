package com.huadu.shunda.data.http.logisitcshomedeliverpickup;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class LogisticsHomeDeliverPickupWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<LogisticsHomeDeliverPickupWarehouseItemInfo> itemList = new ArrayList<>();

    public List<LogisticsHomeDeliverPickupWarehouseItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<LogisticsHomeDeliverPickupWarehouseItemInfo> itemList) {
        this.itemList = itemList;
    }
}
