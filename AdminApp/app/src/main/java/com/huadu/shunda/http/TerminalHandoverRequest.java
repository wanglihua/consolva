package com.huadu.shunda.http;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.terminalhandover.TerminalHandoverNotHandoverListResponseInfo;
import com.huadu.shunda.data.http.terminalhandover.TerminalHandoverShipContainerInnerListResponseInfo;
import com.huadu.shunda.data.http.terminalhandover.TerminalHandoverStationListResponseInfo;

public interface TerminalHandoverRequest {

    @POST("/user/api/terminal/handover/station/list")
    @FormUrlEncoded
    Call<TerminalHandoverStationListResponseInfo> stationList(@Field("userId") int userId);

    @POST("/user/api/terminal/handover/ship/container/inner/list")
    @FormUrlEncoded
    Call<TerminalHandoverShipContainerInnerListResponseInfo> shipContainerInnerList(@Field("pickupStationId") int pickupStationId);

    @POST("/user/api/terminal/handover/not/handover/list")
    @FormUrlEncoded
    Call<TerminalHandoverNotHandoverListResponseInfo> notHandoverList(@Field("shipContainerInnerId") int shipContainerInnerId, @Field("pickupStationId") int pickupStationId);

    @POST("/user/api/terminal/handover")
    @FormUrlEncoded
    Call<ResponseInfo> handover(@Field("pickupStationId") int pickupStationId, @Field("packBoxCode") String packBoxCode, @Field("userId") int userId);

    @POST("/user/api/terminal/handover/complete")
    @FormUrlEncoded
    Call<ResponseInfo> complete(@Field("shipContainerInnerPickupStationId") int shipContainerInnerPickupStationId);
}
