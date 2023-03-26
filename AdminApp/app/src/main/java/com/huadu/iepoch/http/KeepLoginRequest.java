package com.huadu.iepoch.http;

import com.huadu.iepoch.data.http.ResponseInfo;

import retrofit2.Call;
import retrofit2.http.POST;

public interface KeepLoginRequest {

    @POST("/user/api/keep/login/check")
    Call<ResponseInfo> check();
}
