package com.huadu.shunda.data.http.packbox;

import com.google.gson.annotations.SerializedName;

public class PackBoxShipResponseData {

    @SerializedName("packBoxCode")
    private String packBoxCode;

    @SerializedName("orderCode")
    private String orderCode;

    @SerializedName("customerCode")
    private String customerCode;

    @SerializedName("countryName")
    private String countryName;

    @SerializedName("firstModeName")
    private String firstModeName;

    @SerializedName("arrearsAmount")
    private String arrearsAmount;

    @SerializedName("hasArrears")
    private Boolean hasArrears;

    public String getPackBoxCode() {
        return packBoxCode;
    }

    public void setPackBoxCode(String packBoxCode) {
        this.packBoxCode = packBoxCode;
    }

    public String getOrderCode() {
        return orderCode;
    }

    public void setOrderCode(String orderCode) {
        this.orderCode = orderCode;
    }

    public String getCustomerCode() {
        return customerCode;
    }

    public void setCustomerCode(String customerCode) {
        this.customerCode = customerCode;
    }

    public String getCountryName() {
        return countryName;
    }

    public void setCountryName(String countryName) {
        this.countryName = countryName;
    }

    public String getFirstModeName() {
        return firstModeName;
    }

    public void setFirstModeName(String firstModeName) {
        this.firstModeName = firstModeName;
    }

    public String getArrearsAmount() {
        return arrearsAmount;
    }

    public void setArrearsAmount(String arrearsAmount) {
        this.arrearsAmount = arrearsAmount;
    }

    public Boolean getHasArrears() {
        return hasArrears;
    }

    public void setHasArrears(Boolean hasArrears) {
        this.hasArrears = hasArrears;
    }
}