package com.huadu.shunda.data.http.expressphoto;

import com.google.gson.annotations.SerializedName;

import java.util.List;

public class ExpressPhotoExpressPhotoListResponseData {
    @SerializedName("trackingNo")
    private String trackingNo;

    @SerializedName("photoList")
    private List<ExpressPhotoExpressPhotoListItem> photoList;

    public String getTrackingNo() {
        return trackingNo;
    }

    public void setTrackingNo(String trackingNo) {
        this.trackingNo = trackingNo;
    }

    public List<ExpressPhotoExpressPhotoListItem> getPhotoList() {
        return photoList;
    }

    public void setPhotoList(List<ExpressPhotoExpressPhotoListItem> photoList) {
        this.photoList = photoList;
    }
}
