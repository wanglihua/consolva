package com.huadu.iepoch.data.http.customerpackboxstockplace;

import com.google.gson.annotations.SerializedName;
import com.huadu.iepoch.data.http.ResponseInfo;

import java.util.List;

public class CustomerPackBoxStockPlaceListResponseInfo extends ResponseInfo {

    @SerializedName("stockPackBoxCountItemList")
    private List<StockPackBoxCountItem> stockPackBoxCountItemList;
    
    public List<StockPackBoxCountItem> getStockPackBoxCountItemList() {
        return stockPackBoxCountItemList;
    }

    public void setStockPackBoxCountItemList(List<StockPackBoxCountItem> stockPackBoxCountItemList) {
        this.stockPackBoxCountItemList = stockPackBoxCountItemList;
    }
}
