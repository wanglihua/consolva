package com.huadu.iepoch.data.http.pallet;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class PalletListDataResponseInfo extends ResponseInfo {
    @SerializedName("itemList")
    private List<PalletItemInfo> itemList = new ArrayList<>();

    public List<PalletItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<PalletItemInfo> itemList) {
        this.itemList = itemList;
    }
}
