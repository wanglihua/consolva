package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressphoto.GetOrderCodeByPackBoxCodeResponseData;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressPhotoRequest {
    @POST("/admin/app/express/photo/create")
    @FormUrlEncoded
    Call<ResponseInfo> create(@Field("orderCode") String orderCode, @Field("trackingNo") String trackingNo, @Field("photoTypeId") int photoTypeId, @Field("imageFileName") String imageFileName);

    @POST("/admin/app/express/photo/get/order/code/by/pack/box/code")
    @FormUrlEncoded
    Call<ResponseDataInfo<GetOrderCodeByPackBoxCodeResponseData>> getOrderCodeByPackBoxCode(@Field("packBoxCode") String packBoxCode);
}
