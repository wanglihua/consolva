package com.huadu.iepoch.data.http.shipcontainerunloadsort;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class ShipContainerUnloadSortDetailResponse extends ResponseInfo {

    @SerializedName("shipContainer")
    private ShipContainerUnloadSortItemInfo shipContainer;

    public ShipContainerUnloadSortItemInfo getShipContainer() {
        return shipContainer;
    }

    public void setShipContainer(ShipContainerUnloadSortItemInfo shipContainer) {
        this.shipContainer = shipContainer;
    }
}
