package com.huadu.shunda.http;


import com.huadu.shunda.data.http.pallet.PalletListDataResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface PalletRequest {

    @POST("/user/api/pallet/list/data")
    @FormUrlEncoded
    Call<PalletListDataResponseInfo> listData(@Field("warehouseId")int warehouseId);
}
