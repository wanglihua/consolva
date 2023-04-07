package com.huadu.shunda.data.http.expressoffshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressOffShelfWarehouseListResponseData extends ResponseInfo {

    @SerializedName("warehouseList")
    private List<ExpressOffShelfWarehouseListItem> warehouseList = new ArrayList<>();

    public List<ExpressOffShelfWarehouseListItem> getWarehouseList() {
        return warehouseList;
    }

    public void setWarehouseList(List<ExpressOffShelfWarehouseListItem> warehouseList) {
        this.warehouseList = warehouseList;
    }
}
