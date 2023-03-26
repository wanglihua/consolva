package com.huadu.shunda.http;


import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckBillAddResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckBillListResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckStockPlaceAddResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckStockPlaceDetailResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckStockPlaceListResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface StockCheckRequest {

    @POST("/user/api/stock/check/warehouse/list")
    Call<StockCheckWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/stock/check/bill/list")
    @FormUrlEncoded
    Call<StockCheckBillListResponseInfo> billList(@Field("warehouseId") int warehouseId);

    @POST("/user/api/stock/check/bill/add")
    @FormUrlEncoded
    Call<StockCheckBillAddResponseInfo> billAdd(@Field("warehouseId") int warehouseId);

    @POST("/user/api/stock/check/bill/delete")
    @FormUrlEncoded
    Call<ResponseInfo> billDelete(@Field("billId") int billId);

    @POST("/user/api/stock/check/bill/complete")
    @FormUrlEncoded
    Call<ResponseInfo> billComplete(@Field("billId") int billId);

    @POST("/user/api/stock/check/stock/place/list")
    @FormUrlEncoded
    Call<StockCheckStockPlaceListResponseInfo> stockPlaceList(@Field("billId") int billId);

    @POST("/user/api/stock/check/stock/place/add")
    @FormUrlEncoded
    Call<StockCheckStockPlaceAddResponseInfo> stockPlaceAdd(@Field("billId") int billId, @Field("name") String name);

    @POST("/user/api/stock/check/stock/place/delete")
    @FormUrlEncoded
    Call<ResponseInfo> stockPlaceDelete(@Field("stockPlaceId") int stockPlaceId);

    @POST("/user/api/stock/check/stock/place/detail")
    @FormUrlEncoded
    Call<StockCheckStockPlaceDetailResponseInfo> stockPlaceDetail(@Field("stockPlaceId") int stockPlaceId);

    @POST("/user/api/stock/check")
    @FormUrlEncoded
    Call<ResponseInfo> stockCheck(@Field("stockPlaceId") int stockPlaceId, @Field("packBoxCode") String packBoxCode);

    @POST("/user/api/stock/check/stock/place/remove/pack/box")
    @FormUrlEncoded
    Call<ResponseInfo> stockPlaceRemovePackBox(@Field("stockPlaceId")int stockPlaceId, @Field("packBoxId")int packBoxId);
}
