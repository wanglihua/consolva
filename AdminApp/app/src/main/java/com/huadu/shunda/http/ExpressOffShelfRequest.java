package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressoffshelf.ExpressOffShelfExpressListResponseData;
import com.huadu.shunda.data.http.expressoffshelf.ExpressOffShelfNextExpressResponseData;
import com.huadu.shunda.data.http.expressoffshelf.ExpressOffShelfShelfListResponseData;
import com.huadu.shunda.data.http.expressoffshelf.ExpressOffShelfWarehouseListResponseData;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressOffShelfRequest {

    @POST("/admin/app/express/off/shelf/warehouse/list")
    Call<ResponseDataInfo<ExpressOffShelfWarehouseListResponseData>> warehouseList();

    @POST("/user/api/express/shelf/off/shelf/shelf/list")
    @FormUrlEncoded
    Call<ResponseDataInfo<ExpressOffShelfShelfListResponseData>> shelfList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/express/shelf/off/shelf/express/list")
    @FormUrlEncoded
    Call<ResponseDataInfo<ExpressOffShelfExpressListResponseData>> expressList(@Field("shelfId") int shelfId);

    @POST("/user/api/express/shelf/off/shelf/next/express")
    @FormUrlEncoded
    Call<ResponseDataInfo<ExpressOffShelfNextExpressResponseData>> nextExpress(@Field("shelfId") int shelfId);

    @POST("/user/api/express/shelf/off/shelf/complete")
    @FormUrlEncoded
    Call<ResponseInfo> complete(@Field("shelfId") int shelfId, @Field("trackingNo") String trackingNo);
}
