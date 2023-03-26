package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressReturnOffShelfExpressListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressReturnOffShelfWarehouseListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressReturnOffShelfExpressDetailResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressReturnOffShelfRequest {

    @POST("/user/api/express/return/off/shelf/warehouse/list")
    Call<ExpressReturnOffShelfWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/express/return/off/shelf/express/list")
    @FormUrlEncoded
    Call<ExpressReturnOffShelfExpressListResponseInfo> expressList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/express/return/off/shelf/express/detail")
    @FormUrlEncoded
    Call<ExpressReturnOffShelfExpressDetailResponseInfo> expressDetail(@Field("claimGoodsId") int claimGoodsId);

    @POST("/user/api/express/return/off/shelf/complete")
    @FormUrlEncoded
    Call<ResponseInfo> complete(@Field("ClaimId") int claimId, @Field("TrackingNo") String trackingNo);
}
