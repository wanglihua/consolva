package com.huadu.iepoch.http;


import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.shipcontainerinner.ShipContainerInnerLoadDetailDataResponseInfo;
import com.huadu.iepoch.data.http.shipcontainerinner.ShipContainerInnerLoadListDataResponseInfo;
import com.huadu.iepoch.data.http.shipcontainerinner.ShipContainerInnerUnloadListDataResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface ShipContainerInnerRequest {

    @POST("/user/api/ship/container/inner/load/list/data")
    Call<ShipContainerInnerLoadListDataResponseInfo> loadListData();

    @POST("/user/api/ship/container/inner/load/detail/data")
    @FormUrlEncoded
    Call<ShipContainerInnerLoadDetailDataResponseInfo> loadDetailData(@Field("shipContainerId") int shipContainerInnerId); // 这里API参数名称取错了，将错就错了

    @POST("/user/api/ship/container/inner/unload/list/data")
    @FormUrlEncoded
    Call<ShipContainerInnerUnloadListDataResponseInfo> unloadListData(@Field("userId") int userId);

    @POST("/user/api/ship/container/inner/unload/complete")
    @FormUrlEncoded
    Call<ResponseInfo> complete(@Field("shipContainerInnerId") int shipContainerInnerId);
}
