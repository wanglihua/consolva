package com.huadu.shunda.data.http.expressimageshow;

import com.google.gson.annotations.SerializedName;

public class ExpressImageShowImageItemInfo {

    @SerializedName("id")
    private int id;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    @SerializedName("smallImageUrl")
    private String smallImageUrl;

    public String getSmallImageUrl() {
        return smallImageUrl;
    }

    public void setSmallImageUrl(String smallImageUrl) {
        this.smallImageUrl = smallImageUrl;
    }

    @SerializedName("bigImageUrl")
    private String bigImageUrl;

    public String getBigImageUrl() {
        return bigImageUrl;
    }

    public void setBigImageUrl(String bigImageUrl) {
        this.bigImageUrl = bigImageUrl;
    }
}
