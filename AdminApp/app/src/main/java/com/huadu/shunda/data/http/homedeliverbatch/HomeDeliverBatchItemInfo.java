package com.huadu.shunda.data.http.homedeliverbatch;

import com.google.gson.annotations.SerializedName;

public class HomeDeliverBatchItemInfo {

    @SerializedName("id")
    private int id;

    @SerializedName("code")
    private String code;

    @SerializedName("planDeliverDate")
    private String planDeliverDate;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getCode() {
        return code;
    }

    public void setCode(String code) {
        this.code = code;
    }

    public String getPlanDeliverDate() {
        return planDeliverDate;
    }

    public void setPlanDeliverDate(String planDeliverDate) {
        this.planDeliverDate = planDeliverDate;
    }
}
