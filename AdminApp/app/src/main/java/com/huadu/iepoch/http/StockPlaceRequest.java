package com.huadu.iepoch.http;


import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.stockplace.StockPlaceAddResponseInfo;
import com.huadu.iepoch.data.http.stockplace.StockPlaceDetailResponseInfo;
import com.huadu.iepoch.data.http.stockplace.StockPlaceListDataResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface StockPlaceRequest {

    @POST("/user/api/stock/place/list/data")
    @FormUrlEncoded
    Call<StockPlaceListDataResponseInfo> listData(@Field("shipContainerId") int shipContainerId);

    @POST("/user/api/stock/place/add")
    @FormUrlEncoded
    Call<StockPlaceAddResponseInfo> add(@Field("shipContainerId") int shipContainerId, @Field("name")  String name);

    @POST("/user/api/stock/place/detail")
    @FormUrlEncoded
    Call<StockPlaceDetailResponseInfo> detail(@Field("id") int id);

    @POST("/user/api/stock/place/delete")
    @FormUrlEncoded
    Call<ResponseInfo> delete(@Field("id") int id);
}
