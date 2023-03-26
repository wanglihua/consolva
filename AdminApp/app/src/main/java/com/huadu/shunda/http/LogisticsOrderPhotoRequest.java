package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface LogisticsOrderPhotoRequest {

    /*

    @POST("/user/api/logistics/order/photo/upload")
    @Multipart
    Call<ResponseInfo> upload(@Part("orderCode") String orderCode, @Part("expressTrackingNo") String expressTrackingNo, @Part("photoTypeId") int photoTypeId, @Part() MultipartBody.Part imageFile);

    @POST("/user/api/logistics/order/photo/upload/base64")
    @FormUrlEncoded
    Call<ResponseInfo> uploadBase64(@Field("orderCode") String orderCode, @Field("expressTrackingNo") String expressTrackingNo, @Field("photoTypeId") int photoTypeId, @Field("imageFileData") String imageFileData);

     */

    @POST("/user/api/logistics/order/photo/create")
    @FormUrlEncoded
    Call<ResponseInfo> create(@Field("orderCode") String orderCode, @Field("expressTrackingNo") String expressTrackingNo, @Field("photoTypeId") int photoTypeId, @Field("imageFileName") String imageFileName, @Field("clientTime") String clientTime); // orderCode 也可以是箱号
}
