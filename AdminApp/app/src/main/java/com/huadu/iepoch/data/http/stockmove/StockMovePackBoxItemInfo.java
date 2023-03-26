package com.huadu.iepoch.data.http.stockmove;

import com.google.gson.annotations.SerializedName;

public class StockMovePackBoxItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("boxCode")
    private String boxCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("unloadTime")
    private String unloadTime;

    @SerializedName("stockCheckTime")
    private String stockCheckTime;

    @SerializedName("shipContainer")
    private String shipContainer;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getBoxCode() {
        return boxCode;
    }

    public void setBoxCode(String boxCode) {
        this.boxCode = boxCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getUnloadTime() {
        return unloadTime;
    }

    public void setUnloadTime(String unloadTime) {
        this.unloadTime = unloadTime;
    }

    public String getStockCheckTime() {
        return stockCheckTime;
    }

    public void setStockCheckTime(String stockCheckTime) {
        this.stockCheckTime = stockCheckTime;
    }

    public String getShipContainer() {
        return shipContainer;
    }

    public void setShipContainer(String shipContainer) {
        this.shipContainer = shipContainer;
    }
}
