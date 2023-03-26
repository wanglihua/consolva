package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckDetailResponseInfo;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckListResponseInfo;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckShelfListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface WarehouseCheckRequest {

    @POST("/user/api/customer/service/warehouse/check/shelf/list")
    Call<WarehouseCheckShelfListResponseInfo> shelfList();

    @POST("/user/api/customer/service/warehouse/shelf/check/list")
    @FormUrlEncoded
    Call<WarehouseCheckListResponseInfo> list(@Field("shelfId") int shelfId);

    @POST("/user/api/customer/service/warehouse/check/detail")
    @FormUrlEncoded
    Call<WarehouseCheckDetailResponseInfo> detail(@Field("id") int id);

    @POST("/user/api/customer/service/warehouse/check/set/checked")
    @FormUrlEncoded
    Call<ResponseInfo> setChecked(@Field("id") int id, @Field("replyMessage") String replyMessage);
}
