package com.huadu.shunda.data.http.shipcontainerunloadsort;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ShipContainerUnloadSortListResponse extends ResponseInfo {

    @SerializedName("shipContainerList")
    private List<ShipContainerUnloadSortItemInfo> shipContainerList = new ArrayList<>();

    public List<ShipContainerUnloadSortItemInfo> getShipContainerList() {
        return shipContainerList;
    }

    public void setShipContainerList(List<ShipContainerUnloadSortItemInfo> shipContainerList) {
        this.shipContainerList = shipContainerList;
    }

}
