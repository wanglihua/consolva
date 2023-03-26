package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.packbox.PackBoxLabelPrintDetailDataResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface PackBoxLabelTlaPrintRequest {

    @POST("/user/api/pack/box/label/tla/pack/box/detail")
    @FormUrlEncoded
    Call<PackBoxLabelPrintDetailDataResponseInfo> packBoxDetail(@Field("packBoxCode") String packBoxCode);

    @POST("/user/api/pack/box/label/tla/print")
    @FormUrlEncoded
    Call<ResponseInfo> print(@Field("packBoxCode") String packBoxCode);
}
