package com.huadu.shunda.http;

import com.huadu.shunda.data.http.homedeliverpickup.HomeDeliverPickupResponseInfo;
import com.huadu.shunda.data.http.homedeliverpickup.HomeDeliverPickupWarehouseListResponseInfo;
import com.huadu.shunda.data.http.ResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface HomeDeliverPickupRequest {

    @POST("/user/api/home/deliver/pickup/warehouse/list")
    Call<HomeDeliverPickupWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/home/deliver/pickup")
    @FormUrlEncoded
    Call<HomeDeliverPickupResponseInfo> pickup(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode);

    @POST("/user/api/home/deliver/pickup/order/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupOrderConfirm(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("clientTime") String clientTime);

    @POST("/user/api/home/deliver/pickup/pack-box/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupPackBoxConfirm(@Field("warehouseId") int warehouseId, @Field("pickupCode") String pickupCode, @Field("packBoxCode") String packBoxCode, @Field("clientTime") String clientTime);
}
