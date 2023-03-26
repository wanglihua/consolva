package com.huadu.shunda.http;


import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.stockmove.StockMoveStockPlaceDetailResponseInfo;
import com.huadu.shunda.data.http.stockmove.StockMoveStockPlaceListResponseInfo;
import com.huadu.shunda.data.http.stockmove.StockMoveWarehouseListResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface StockMoveRequest {

    @POST("/user/api/stock/move/warehouse/list")
    Call<StockMoveWarehouseListResponseInfo> warehouseList();

    @POST("/user/api/stock/move/stock/place/list")
    @FormUrlEncoded
    Call<StockMoveStockPlaceListResponseInfo> stockPlaceList(@Field("warehouseId") int warehouseId, @Field("nameQuery") String nameQuery);

    @POST("/user/api/stock/move/stock/place/detail")
    @FormUrlEncoded
    Call<StockMoveStockPlaceDetailResponseInfo> stockPlaceDetail(@Field("stockPlaceId") int stockPlaceId);

    @POST("/user/api/stock/move/stock/place/rename")
    @FormUrlEncoded
    Call<ResponseInfo> stockPlaceRename(@Field("stockPlaceId") int stockPlaceId, @Field("newName") String newName);
}
