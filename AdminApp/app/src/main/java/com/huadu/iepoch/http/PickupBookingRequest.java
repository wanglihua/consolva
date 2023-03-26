package com.huadu.iepoch.http;


import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.pickupbooking.PickupBookingListDataResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface PickupBookingRequest {

    /**
     *
     * @param warehouseId 这里实际传的好像是 0，服务器那边也要注意处理
     * @param pickupStationId
     * @return
     */
    @POST("/user/api/pickup/booking/list/data")
    @FormUrlEncoded
    Call<PickupBookingListDataResponseInfo> listData(@Field("warehouseId") int warehouseId, @Field("pickupStationId") int pickupStationId);

    @POST("/user/api/pickup/booking/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> confirm(@Field("id") int id);
}
