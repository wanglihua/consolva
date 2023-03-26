package com.huadu.shunda.http;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxDetailResponse;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxNotScanListResponse;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxScanResultResponse;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ShipContainerUnloadSortPackBoxRequest {

    @POST("/user/api/ship/container/unload/sort/pack/box/scan/result")
    @FormUrlEncoded
    Call<ShipContainerUnloadSortPackBoxScanResultResponse> scanResult(@Field("shipContainerId") int shipContainerId, @Field("orderPackBoxCode") String orderPackBoxCode);

    @POST("/user/api/ship/container/unload/sort/pack/box/detail")
    @FormUrlEncoded
    Call<ShipContainerUnloadSortPackBoxDetailResponse> packBoxDetail(@Field("shipContainerOrderPackBoxId") int shipContainerOrderPackBoxId);

    @POST("/user/api/ship/container/unload/sort/not/scan/list")
    @FormUrlEncoded
    Call<ShipContainerUnloadSortPackBoxNotScanListResponse> notScanList(@Field("shipContainerId") int shipContainerId);

    @POST("/user/api/ship/container/unload/sort/pack/box/set/position")
    @FormUrlEncoded
    Call<ResponseInfo> setPosition(@Field("shipContainerOrderPackBoxId") int shipContainerOrderPackBoxId, @Field("positionName") String positionName, @Field("multiOrderOnePos") boolean multiOrderOnePos);
}
