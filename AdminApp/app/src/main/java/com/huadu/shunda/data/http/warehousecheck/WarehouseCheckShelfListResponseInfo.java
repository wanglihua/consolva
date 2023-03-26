package com.huadu.shunda.data.http.warehousecheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.List;

public class WarehouseCheckShelfListResponseInfo extends ResponseInfo {

    @SerializedName("shelfList")
    private List<WarehouseCheckShelfListItemInfo> shelfList;

    @SerializedName("totalCheckCount")
    private int totalCheckCount;

    public List<WarehouseCheckShelfListItemInfo> getShelfList() {
        return shelfList;
    }

    public void setShelfList(List<WarehouseCheckShelfListItemInfo> shelfList) {
        this.shelfList = shelfList;
    }

    public int getTotalCheckCount() {
        return totalCheckCount;
    }

    public void setTotalCheckCount(int totalCheckCount) {
        this.totalCheckCount = totalCheckCount;
    }
}
