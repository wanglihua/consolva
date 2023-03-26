package com.huadu.shunda.http;


import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.warehousepickup.WarehousePickupResponseInfo;
import com.huadu.shunda.data.http.warehousepickup.WarehousePickupWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface WarehousePickupRequest {

    @POST("/user/api/warehouse/pickup/warehouse/list")
    Call<WarehousePickupWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/warehouse/pickup")
    @FormUrlEncoded
    Call<WarehousePickupResponseInfo> pickup(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode);

    @POST("/user/api/warehouse/pickup/order/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupOrderConfirm(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("clientTime") String clientTime);

    @POST("/user/api/warehouse/pickup/pack-box/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupPackBoxConfirm(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("packBoxCode") String packBoxCode, @Field("clientTime") String clientTime);
}
