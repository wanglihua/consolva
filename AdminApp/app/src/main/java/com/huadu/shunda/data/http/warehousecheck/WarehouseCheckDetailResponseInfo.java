package com.huadu.shunda.data.http.warehousecheck;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class WarehouseCheckDetailResponseInfo extends ResponseInfo {

    @SerializedName("warehouseCheck")
    private WarehouseCheckDetailInfo warehouseCheck;

    public WarehouseCheckDetailInfo getWarehouseCheck() {
        return warehouseCheck;
    }

    public void setWarehouseCheck(WarehouseCheckDetailInfo warehouseCheck) {
        this.warehouseCheck = warehouseCheck;
    }
}
