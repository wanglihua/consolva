package com.huadu.shunda.http;


import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.packbox.GetOrderCodeByPackBoxCodeResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxDetailDataResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxOrderPayDetailResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxStockPlaceDataResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface PackBoxRequest {

    @POST("/user/api/pack/box/load")
    @FormUrlEncoded
    Call<ResponseInfo> load(@Field("shipContainerInnerId") int shipContainerInnerId, @Field("packBoxCode") String packBoxCode, @Field("userId") int userId);

    @POST("/user/api/pack/box/unload")
    @FormUrlEncoded
    Call<ResponseInfo> unload(@Field("stockPlaceId") int stockPlaceId, @Field("packBoxCode") String packBoxCode, @Field("userId") int userId);

    @POST("/user/api/pack/box/order/pay/detail")
    @FormUrlEncoded
    Call<PackBoxOrderPayDetailResponseInfo> orderPayDetail(@Field("packBoxCode") String packBoxCode);

    @POST("/user/api/pack/box/get/order/code/by/pack/box/code")
    @FormUrlEncoded
    Call<GetOrderCodeByPackBoxCodeResponseInfo> getOrderCodeByPackBoxCode(@Field("packBoxCode") String packBoxCode);

    @POST("/user/api/pack/box/detail/data")
    @FormUrlEncoded
    Call<PackBoxDetailDataResponseInfo> detailData(@Field("packBoxCode") String packBoxCode);

    @POST("/user/api/pack/box/stock/place/data")
    @FormUrlEncoded
    Call<PackBoxStockPlaceDataResponseInfo> stockPlaceData(@Field("packBoxCode") String packBoxCode);
}
