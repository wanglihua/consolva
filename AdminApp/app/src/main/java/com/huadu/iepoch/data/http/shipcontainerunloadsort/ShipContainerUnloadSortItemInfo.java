package com.huadu.iepoch.data.http.shipcontainerunloadsort;

import com.google.gson.annotations.SerializedName;

public class ShipContainerUnloadSortItemInfo {
    
    @SerializedName("shipContainerId")
    private int shipContainerId;

    @SerializedName("shipContainerCode")
    private String shipContainerCode;

    @SerializedName("arriveTime")
    private String arriveTime;

    public int getShipContainerId() {
        return shipContainerId;
    }

    public void setShipContainerId(int shipContainerId) {
        this.shipContainerId = shipContainerId;
    }

    public String getShipContainerCode() {
        return shipContainerCode;
    }

    public void setShipContainerCode(String shipContainerCode) {
        this.shipContainerCode = shipContainerCode;
    }

    public String getArriveTime() {
        return arriveTime;
    }

    public void setArriveTime(String arriveTime) {
        this.arriveTime = arriveTime;
    }
}
