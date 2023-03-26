package com.huadu.iepoch.data.http.warehousecheck;

import com.google.gson.annotations.SerializedName;

public class WarehouseCheckShelfListItemInfo {

    @SerializedName("shelfId")
    private int shelfId;

    @SerializedName("shelfName")
    private String shelfName;

    @SerializedName("orderNum")
    private int orderNum;

    @SerializedName("checkCount")
    private int checkCount;

    public int getShelfId() {
        return shelfId;
    }

    public void setShelfId(int shelfId) {
        this.shelfId = shelfId;
    }

    public String getShelfName() {
        return shelfName;
    }

    public void setShelfName(String shelfName) {
        this.shelfName = shelfName;
    }

    public int getOrderNum() {
        return orderNum;
    }

    public void setOrderNum(int orderNum) {
        this.orderNum = orderNum;
    }

    public int getCheckCount() {
        return checkCount;
    }

    public void setCheckCount(int checkCount) {
        this.checkCount = checkCount;
    }
}
