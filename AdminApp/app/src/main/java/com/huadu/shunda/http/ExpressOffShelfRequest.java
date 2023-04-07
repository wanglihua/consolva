package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfExpressListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfNextExpressResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfShelfListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressOffShelfRequest {

    @POST("/user/api/express/shelf/off/shelf/warehouse/list")
    Call<ExpressShelfOffShelfWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/express/shelf/off/shelf/shelf/list")
    @FormUrlEncoded
    Call<ExpressShelfOffShelfShelfListResponseInfo> shelfList(@Field("warehouseId") int warehouseId);


    @POST("/user/api/express/shelf/off/shelf/express/list")
    @FormUrlEncoded
    Call<ExpressShelfOffShelfExpressListResponseInfo> expressList(@Field("shelfId") int shelfId);


    @POST("/user/api/express/shelf/off/shelf/next/express")
    @FormUrlEncoded
    Call<ExpressShelfOffShelfNextExpressResponseInfo> nextExpress(@Field("shelfId") int shelfId);


    @POST("/user/api/express/shelf/off/shelf/complete")
    @FormUrlEncoded
    Call<ResponseInfo> complete(@Field("shelfId") int shelfId, @Field("trackingNo") String trackingNo);
}
