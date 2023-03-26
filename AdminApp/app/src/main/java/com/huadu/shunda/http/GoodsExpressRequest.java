package com.huadu.shunda.http;


import com.huadu.shunda.data.http.goodsexpress.GoodsExpressQueryResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface GoodsExpressRequest {

    @POST("/user/api/goods/express/query")
    @FormUrlEncoded
    Call<GoodsExpressQueryResponseInfo> query(@Field("expressTrackingNo") String expressTrackingNo);
}
