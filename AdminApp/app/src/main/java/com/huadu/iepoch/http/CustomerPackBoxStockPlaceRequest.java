package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.customerpackboxstockplace.CustomerPackBoxStockPlaceListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface CustomerPackBoxStockPlaceRequest {

    @POST("/user/api/customer/pack/box/stock/place/list")
    @FormUrlEncoded
    Call<CustomerPackBoxStockPlaceListResponseInfo> stockPlaceList(@Field("customerCode") String customerCode);
}
