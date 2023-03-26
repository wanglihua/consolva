package com.huadu.shunda.data.http.customerpackboxstockplace;

import com.google.gson.annotations.SerializedName;

public class StockPackBoxCountItem {

    @SerializedName("seqNum")
    private int seqNum;

    @SerializedName("warehouseId")
    private int warehouseId;

    @SerializedName("warehouseName")
    private String warehouseName;

    @SerializedName("stockPlaceId")
    private int stockPlaceId;

    @SerializedName("stockPlaceName")
    private String stockPlaceName;

    @SerializedName("orderId")
    private int orderId;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("packBoxCount")
    private int packBoxCount;

    public int getSeqNum() {
        return seqNum;
    }

    public void setSeqNum(int seqNum) {
        this.seqNum = seqNum;
    }

    public int getWarehouseId() {
        return warehouseId;
    }

    public void setWarehouseId(int warehouseId) {
        this.warehouseId = warehouseId;
    }

    public String getWarehouseName() {
        return warehouseName;
    }

    public void setWarehouseName(String warehouseName) {
        this.warehouseName = warehouseName;
    }

    public int getStockPlaceId() {
        return stockPlaceId;
    }

    public void setStockPlaceId(int stockPlaceId) {
        this.stockPlaceId = stockPlaceId;
    }

    public String getStockPlaceName() {
        return stockPlaceName;
    }

    public void setStockPlaceName(String stockPlaceName) {
        this.stockPlaceName = stockPlaceName;
    }

    public int getOrderId() {
        return orderId;
    }

    public void setOrderId(int orderId) {
        this.orderId = orderId;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public int getPackBoxCount() {
        return packBoxCount;
    }

    public void setPackBoxCount(int packBoxCount) {
        this.packBoxCount = packBoxCount;
    }
}
