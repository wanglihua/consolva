package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpExpressListResponseInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpNextExpressResponseInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpShelfListResponseInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpStorageLocListResponseInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ExpressNoProcessDumpRequest {

    @POST("/user/api/express/no/process/dump/warehouse/list")
    Call<ExpressDumpWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/express/no/process/dump/shelf/list")
    @FormUrlEncoded
    Call<ExpressDumpShelfListResponseInfo> shelfList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/express/no/process/dump/express/list")
    @FormUrlEncoded
    Call<ExpressDumpExpressListResponseInfo> expressList(@Field("shelfId") int shelfId);

    @POST("/user/api/express/no/process/dump/next/express")
    @FormUrlEncoded
    Call<ExpressDumpNextExpressResponseInfo> nextExpress(@Field("shelfId") int shelfId);

    @POST("/user/api/express/no/process/dump/express")
    @FormUrlEncoded
    Call<ExpressDumpNextExpressResponseInfo> express(@Field("shelfId") int shelfId, @Field("claimId") int claimId);

    @POST("/user/api/express/no/process/dump/target/shelf/list")
    @FormUrlEncoded
    Call<ExpressDumpShelfListResponseInfo> targetShelfList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/express/no/pickup/dump/target/storage/loc/list")
    @FormUrlEncoded
    Call<ExpressDumpStorageLocListResponseInfo> targetStorageLocList(@Field("shelfId") int shelfId);

    @POST("/user/api/express/no/process/dump/storage/loc/new")
    @FormUrlEncoded
    Call<ResponseInfo> newStorageLoc(@Field("shelfId") int shelfId);

    @POST("/user/api/express/no/process/dump/complete")
    @FormUrlEncoded
    Call<ResponseInfo> complete(@Field("ShelfId") int shelfId, @Field("TargetStorageId") int targetStorageId, @Field("TrackingNo") String trackingNo);
}
