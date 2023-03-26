package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.homedeliverbatch.HomeDeliverBatchEntryDetailResponseInfo;
import com.huadu.iepoch.data.http.homedeliverbatch.HomeDeliverBatchEntryListResponseInfo;
import com.huadu.iepoch.data.http.homedeliverbatch.HomeDeliverBatchListResponseInfo;
import com.huadu.iepoch.data.http.homedeliverbatch.HomeDeliverBatchWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface HomeDeliverBatchRequest {

    @POST("/user/api/home/deliver/batch/warehouse/list")
    Call<HomeDeliverBatchWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/home/deliver/batch/list")
    @FormUrlEncoded
    Call<HomeDeliverBatchListResponseInfo> batchList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/home/deliver/entry/list")
    @FormUrlEncoded
    Call<HomeDeliverBatchEntryListResponseInfo> entryList(@Field("batchId") int batchId);

    @POST("/user/api/home/deliver/entry/detail")
    @FormUrlEncoded
    Call<HomeDeliverBatchEntryDetailResponseInfo> entryDetail(@Field("entryId") int entryId);

    @POST("/user/api/home/deliver/entry/send/fourth/message")
    @FormUrlEncoded
    Call<ResponseInfo> sendFourthMessage(@Field("entryId") int entryId);

    @POST("/user/api/home/deliver/batch/drive/start")
    @FormUrlEncoded
    Call<ResponseInfo> driveStart(@Field("batchId") int batchId, @Field("clientTime") String clientTime);
}
