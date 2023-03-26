package com.huadu.shunda.data.http.expressshelf;

import com.google.gson.annotations.SerializedName;

public class ExpressShelfOffShelfShelfItemInfo {

    @SerializedName("id")
    private int id;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    @SerializedName("name")
    private String name;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    @SerializedName("expressCount")
    private int expressCount;

    public int getExpressCount() {
        return expressCount;
    }

    public void setExpressCount(int expressCount) {
        this.expressCount = expressCount;
    }
}
