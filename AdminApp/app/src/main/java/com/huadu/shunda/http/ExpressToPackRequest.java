package com.huadu.shunda.http;

import com.huadu.shunda.data.http.expresstopack.ExpressToPackShelfListResponseInfo;
import com.huadu.shunda.data.http.expresstopack.ExpressToPackWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

/**
 * 这个没用起来
 */
public interface ExpressToPackRequest {

    @POST("/user/api/express/to/pack/warehouse/list")
    // @FormUrlEncoded
    Call<ExpressToPackWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/express/to/pack/shelf/list")
    @FormUrlEncoded
    Call<ExpressToPackShelfListResponseInfo> shelfList(@Field("warehouseId") int warehouseId);
}
