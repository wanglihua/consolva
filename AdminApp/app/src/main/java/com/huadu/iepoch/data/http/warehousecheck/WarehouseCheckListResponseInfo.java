package com.huadu.iepoch.data.http.warehousecheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.List;

public class WarehouseCheckListResponseInfo extends ResponseInfo {

    @SerializedName("checkCount")
    private int checkCount;

    @SerializedName("warehouseCheckList")
    private List<WarehouseCheckListItemInfo> warehouseCheckList;

    public int getCheckCount() {
        return checkCount;
    }

    public void setCheckCount(int checkCount) {
        this.checkCount = checkCount;
    }

    public List<WarehouseCheckListItemInfo> getWarehouseCheckList() {
        return warehouseCheckList;
    }

    public void setWarehouseCheckList(List<WarehouseCheckListItemInfo> warehouseCheckList) {
        this.warehouseCheckList = warehouseCheckList;
    }
}
