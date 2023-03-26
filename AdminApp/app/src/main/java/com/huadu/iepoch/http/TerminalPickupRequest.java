package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.terminalpickup.TerminalPickupResponseInfo;
import com.huadu.iepoch.data.http.terminalpickup.TerminalPickupStationListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface TerminalPickupRequest {

    @POST("/user/api/terminal/pickup/station/list")
    @FormUrlEncoded
    Call<TerminalPickupStationListResponseInfo> stationList(@Field("userId") int userId);


    @POST("/user/api/terminal/pickup")
    @FormUrlEncoded
    Call<TerminalPickupResponseInfo> pickup(@Field("pickupStationId") int pickupStationId, @Field("pickupCode") String pickupCode);

    @POST("/user/api/terminal/pickup/order/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupOrderConfirm(@Field("pickupStationId") int pickupStationId, @Field("pickupCode") String pickupCode, @Field("clientTime") String clientTime);

    @POST("/user/api/terminal/pickup/pack-box/confirm")
    @FormUrlEncoded
    Call<ResponseInfo> pickupPackBoxConfirm(@Field("pickupStationId") int pickupStationId, @Field("pickupCode") String pickupCode, @Field("packBoxCode") String packBoxCode, @Field("clientTime") String clientTime);
}
