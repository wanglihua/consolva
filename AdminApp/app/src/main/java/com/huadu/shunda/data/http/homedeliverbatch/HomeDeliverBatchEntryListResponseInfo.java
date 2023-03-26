package com.huadu.shunda.data.http.homedeliverbatch;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class HomeDeliverBatchEntryListResponseInfo extends ResponseInfo {

    @SerializedName("driveStatus")
    private String driveStatus;

    @SerializedName("driveStartTime")
    private String driveStartTime;

    @SerializedName("showDriveStart")
    private boolean showDriveStart;

    @SerializedName("itemList")
    private List<HomeDeliverBatchEntryItemInfo> itemList = new ArrayList<>();

    public String getDriveStatus() {
        return driveStatus;
    }

    public void setDriveStatus(String driveStatus) {
        this.driveStatus = driveStatus;
    }

    public String getDriveStartTime() {
        return driveStartTime;
    }

    public void setDriveStartTime(String driveStartTime) {
        this.driveStartTime = driveStartTime;
    }

    public boolean getShowDriveStart() {
        return showDriveStart;
    }

    public void setShowDriveStart(boolean showDriveStart) {
        this.showDriveStart = showDriveStart;
    }

    public List<HomeDeliverBatchEntryItemInfo> getItemList() {
        return itemList;
    }

    public void setItemList(List<HomeDeliverBatchEntryItemInfo> itemList) {
        this.itemList = itemList;
    }
}
