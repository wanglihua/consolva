package com.huadu.shunda.http;


import com.huadu.shunda.data.http.packbox.PackBoxDetailDataResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxOrderPayDetailResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface PackBoxRequest {

    @POST("/admin/app/pack/box/ship")
    @FormUrlEncoded
    Call<PackBoxOrderPayDetailResponseInfo> ship(@Field("packBoxCode") String packBoxCode);

    @POST("/user/api/pack/box/detail/data")
    @FormUrlEncoded
    Call<PackBoxDetailDataResponseInfo> detailData(@Field("packBoxCode") String packBoxCode);
}
