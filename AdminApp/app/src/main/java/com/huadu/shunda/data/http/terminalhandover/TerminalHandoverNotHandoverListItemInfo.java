package com.huadu.shunda.data.http.terminalhandover;

import com.google.gson.annotations.SerializedName;

public class TerminalHandoverNotHandoverListItemInfo {

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("notHandoverPackBoxCount")
    private int notHandoverPackBoxCount;

    @SerializedName("notHandoverPackBoxes")
    private String notHandoverPackBoxes;

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public int getNotHandoverPackBoxCount() {
        return notHandoverPackBoxCount;
    }

    public void setNotHandoverPackBoxCount(int notHandoverPackBoxCount) {
        this.notHandoverPackBoxCount = notHandoverPackBoxCount;
    }

    public String getNotHandoverPackBoxes() {
        return notHandoverPackBoxes;
    }

    public void setNotHandoverPackBoxes(String notHandoverPackBoxes) {
        this.notHandoverPackBoxes = notHandoverPackBoxes;
    }
}
