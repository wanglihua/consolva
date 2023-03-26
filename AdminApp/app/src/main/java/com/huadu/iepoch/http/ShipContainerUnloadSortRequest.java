package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.shipcontainerunloadsort.ShipContainerUnloadSortDetailResponse;
import com.huadu.iepoch.data.http.shipcontainerunloadsort.ShipContainerUnloadSortListResponse;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ShipContainerUnloadSortRequest {

    @POST("/user/api/ship/container/unload/sort/list")
    Call<ShipContainerUnloadSortListResponse> shipContainerList();

    @POST("/user/api/ship/container/unload/sort/detail")
    @FormUrlEncoded
    Call<ShipContainerUnloadSortDetailResponse> shipContainerDetail(@Field("shipContainerId") int shipContainerId);
}
