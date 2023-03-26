package com.huadu.iepoch.data.http.shipcontainerunloadsortpackbox;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

public class ShipContainerUnloadSortPackBoxScanResultResponse extends ResponseInfo {

    @SerializedName("orderPackBox")
    private ShipContainerUnloadSortPackBoxDetailItem orderPackBox;

    public ShipContainerUnloadSortPackBoxDetailItem getOrderPackBox() {
        return orderPackBox;
    }

    public void setOrderPackBox(ShipContainerUnloadSortPackBoxDetailItem orderPackBox) {
        this.orderPackBox = orderPackBox;
    }
}
