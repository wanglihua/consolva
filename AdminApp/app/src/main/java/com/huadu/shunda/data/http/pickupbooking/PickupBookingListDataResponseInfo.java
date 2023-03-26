package com.huadu.shunda.data.http.pickupbooking;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class PickupBookingListDataResponseInfo extends ResponseInfo {

    @SerializedName("itemList")
    private List<PickupBookingItemInfo> itemList = new ArrayList<>();

    public List<PickupBookingItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<PickupBookingItemInfo> itemList) {
        this.itemList = itemList;
    }
}
