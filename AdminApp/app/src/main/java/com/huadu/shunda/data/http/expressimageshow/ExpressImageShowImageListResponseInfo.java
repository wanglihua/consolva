package com.huadu.shunda.data.http.expressimageshow;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.List;

public class ExpressImageShowImageListResponseInfo extends ResponseInfo {

    @SerializedName("trackingNo")
    private String trackingNo;

    public String getTrackingNo() {
        return trackingNo;
    }

    public void setTrackingNo(String trackingNo) {
        this.trackingNo = trackingNo;
    }

    @SerializedName("imageList")
    private List<ExpressImageShowImageItemInfo> imageList;

    public List<ExpressImageShowImageItemInfo> getImageList() {
        return imageList;
    }

    public void setImageList(List<ExpressImageShowImageItemInfo> imageList) {
        this.imageList = imageList;
    }
}
