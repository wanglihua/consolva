package com.huadu.iepoch.data.http.shipcontainerunloadsortpackbox;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ShipContainerUnloadSortPackBoxNotScanListResponse extends ResponseInfo {

    @SerializedName("totalCount")
    private int totalCount;

    @SerializedName("notScannedCount")
    private int notScannedCount;

    @SerializedName("scannedCount")
    private int scannedCount;

    @SerializedName("notScannedOrderList")
    private List<ShipContainerUnloadSortOrderListItem> notScannedOrderList = new ArrayList<>();

    public int getTotalCount() {
        return totalCount;
    }

    public void setTotalCount(int totalCount) {
        this.totalCount = totalCount;
    }

    public int getNotScannedCount() {
        return notScannedCount;
    }

    public void setNotScannedCount(int notScannedCount) {
        this.notScannedCount = notScannedCount;
    }

    public int getScannedCount() {
        return scannedCount;
    }

    public void setScannedCount(int scannedCount) {
        this.scannedCount = scannedCount;
    }

    public List<ShipContainerUnloadSortOrderListItem> getNotScannedOrderList() {
        return notScannedOrderList;
    }

    public void setNotScannedOrderList(List<ShipContainerUnloadSortOrderListItem> notScannedOrderList) {
        this.notScannedOrderList = notScannedOrderList;
    }
}
