package com.huadu.shunda.data.http.expresstopack;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressToPackWarehouseListResponseInfo extends ResponseInfo {

    @SerializedName("warehouseList")
    private List<ExpressToPackWarehouseItemInfo> warehouseList = new ArrayList<>();

    public List<ExpressToPackWarehouseItemInfo> getWarehouseList() {
        return warehouseList;
    }

    public void setWarehouseList(List<ExpressToPackWarehouseItemInfo> warehouseList) {
        this.warehouseList = warehouseList;
    }
}
