package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfExpressListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfNextExpressResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfShelfListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfWarehouseListResponseInfo;
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

    @POST("/user/api/express/shelf/off/shelf/warehouse/list")
    Call<ExpressShelfOffShelfWarehouseListResponseInfo> offShelfWarehouseList();

    @POST("/user/api/express/shelf/off/shelf/shelf/list")
    @FormUrlEncoded
    Call<ExpressShelfOffShelfShelfListResponseInfo> offShelfShelfList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/express/shelf/off/shelf/express/list")
    @FormUrlEncoded
    Call<ExpressShelfOffShelfExpressListResponseInfo> offShelfExpressList(@Field("shelfId") int shelfId);

    @POST("/user/api/express/shelf/off/shelf/next/express")
    @FormUrlEncoded
    Call<ExpressShelfOffShelfNextExpressResponseInfo> offShelfNextExpress(@Field("shelfId") int shelfId);

    @POST("/user/api/express/shelf/off/shelf/complete")
    @FormUrlEncoded
    Call<ResponseInfo> offShelfComplete(@Field("ShelfId") int shelfId, @Field("TrackingNo") String trackingNo);

    @POST("/user/api/express/shelf/packing/query")
    @FormUrlEncoded
    Call<ExpressShelfPackingQueryResponseInfo> packingQuery(@Field("TrackingNo") String trackingNo, @Field("StorageLocCode") String storageLocCode, @Field("CheckStatus") boolean checkStatus);

    @POST("/user/api/express/shelf/packing/complete")
    @FormUrlEncoded
    Call<ResponseInfo> packingComplete(@Field("TrackingNo") String trackingNo, @Field("StorageLocCode") String storageLocCode);
}
