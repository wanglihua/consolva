package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOnShelfQueryResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfPackingQueryResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressShelfRequest {

    @POST("/user/api/express/shelf/on/shelf/query")
    @FormUrlEncoded
    Call<ExpressShelfOnShelfQueryResponseInfo> onShelfQuery(@Field("TrackingNo") String trackingNo, @Field("StorageLocCode") String storageLocCode, @Field("CheckStatus") boolean checkStatus);

    @POST("/user/api/express/shelf/on/shelf/complete")
    @FormUrlEncoded
    Call<ResponseInfo> onShelfComplete(@Field("TrackingNo") String trackingNo, @Field("StorageLocCode") String storageLocCode);

    @POST("/user/api/express/shelf/packing/query")
    @FormUrlEncoded
    Call<ExpressShelfPackingQueryResponseInfo> packingQuery(@Field("TrackingNo") String trackingNo, @Field("StorageLocCode") String storageLocCode, @Field("CheckStatus") boolean checkStatus);

    @POST("/user/api/express/shelf/packing/complete")
    @FormUrlEncoded
    Call<ResponseInfo> packingComplete(@Field("TrackingNo") String trackingNo, @Field("StorageLocCode") String storageLocCode);
}
