package com.huadu.shunda.data.http.expressoffshelf;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressOffShelfShelfListResponseData extends ResponseInfo {

    @SerializedName("shelfList")
    private List<ExpressOffShelfShelfListItem> shelfList = new ArrayList<>();

    public List<ExpressOffShelfShelfListItem> getShelfList() {
        return shelfList;
    }

    public void setShelfList(List<ExpressOffShelfShelfListItem> shelfList) {
        this.shelfList = shelfList;
    }
}
