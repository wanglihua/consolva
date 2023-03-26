package com.huadu.iepoch.data.http.shipcontainerunloadsortpackbox;

import com.google.gson.annotations.SerializedName;

import java.util.List;

public class ShipContainerUnloadSortOrderListItem {

    @SerializedName("seqNo")
    private int seqNo;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("notScannedBoxNumList")
    private List<String> notScannedBoxNumList;

    @SerializedName("packBoxLoadCount")
    private int packBoxLoadCount;

    @SerializedName("positions")
    private String positions;

    public int getSeqNo() {
        return seqNo;
    }

    public void setSeqNo(int seqNo) {
        this.seqNo = seqNo;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public List<String> getNotScannedBoxNumList() {
        return notScannedBoxNumList;
    }

    public void setNotScannedBoxNumList(List<String> notScannedBoxNumList) {
        this.notScannedBoxNumList = notScannedBoxNumList;
    }

    public int getPackBoxLoadCount() {
        return packBoxLoadCount;
    }

    public void setPackBoxLoadCount(int packBoxLoadCount) {
        this.packBoxLoadCount = packBoxLoadCount;
    }

    public String getPositions() {
        return positions;
    }

    public void setPositions(String positions) {
        this.positions = positions;
    }
}
