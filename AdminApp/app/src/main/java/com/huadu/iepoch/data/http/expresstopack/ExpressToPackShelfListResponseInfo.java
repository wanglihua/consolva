package com.huadu.iepoch.data.http.expresstopack;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.List;

public class ExpressToPackShelfListResponseInfo extends ResponseInfo {

    @SerializedName("shelfList")
    private List<ExpressToPackShelfItemInfo> shelfList;

    public List<ExpressToPackShelfItemInfo> getShelfList() {
        return shelfList;
    }

    public void setShelfList(List<ExpressToPackShelfItemInfo> shelfList) {
        this.shelfList = shelfList;
    }
}
