package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.expressimageshow.ExpressImageShowImageListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressImageShowRequest {

    @POST("/user/api/express/photo/show/image/list/cn")
    @FormUrlEncoded
    Call<ExpressImageShowImageListResponseInfo> imageListCn(@Field("trackingNo") String trackingNo);

    @POST("/user/api/express/photo/show/image/list/hk")
    @FormUrlEncoded
    Call<ExpressImageShowImageListResponseInfo> imageListHk(@Field("trackingNo") String trackingNo);
}
