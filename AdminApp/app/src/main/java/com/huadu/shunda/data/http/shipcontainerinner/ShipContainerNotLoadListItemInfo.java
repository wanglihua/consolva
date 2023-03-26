package com.huadu.shunda.data.http.shipcontainerinner;

import com.google.gson.annotations.SerializedName;

public class ShipContainerNotLoadListItemInfo {

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("notLoadPackBoxCount")
    private int notLoadPackBoxCount;

    @SerializedName("notLoadPackBoxes")
    private String notLoadPackBoxes;

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public int getNotLoadPackBoxCount() {
        return notLoadPackBoxCount;
    }

    public void setNotLoadPackBoxCount(int notLoadPackBoxCount) {
        this.notLoadPackBoxCount = notLoadPackBoxCount;
    }

    public String getNotLoadPackBoxes() {
        return notLoadPackBoxes;
    }

    public void setNotLoadPackBoxes(String notLoadPackBoxes) {
        this.notLoadPackBoxes = notLoadPackBoxes;
    }
}
