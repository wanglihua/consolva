package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.logisitcshomedeliverpickup.LogisticsHomeDeliverPickupResponseInfo;
import com.huadu.iepoch.data.http.logisitcshomedeliverpickup.LogisticsHomeDeliverPickupWarehouseListResponseInfo;
import com.huadu.iepoch.data.http.ResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface LogisticsHomeDeliverPickupRequest {
    @POST("/user/api/logistics/home/deliver/pickup/warehouse/list")
    Call<LogisticsHomeDeliverPickupWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/logistics/home/deliver/pickup")
    @FormUrlEncoded
    Call<LogisticsHomeDeliverPickupResponseInfo> pickup(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("userId") int userId);

    @POST("/user/api/logistics/home/deliver/pickup/order/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupOrderConfirm(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("clientTime") String clientTime);

    @POST("/user/api/logistics/home/deliver/pickup/pack-box/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupPackBoxConfirm(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("packBoxCode") String packBoxCode, @Field("clientTime") String clientTime);
}
