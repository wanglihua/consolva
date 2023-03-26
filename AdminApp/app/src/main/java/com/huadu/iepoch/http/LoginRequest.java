package com.huadu.iepoch.http;


import com.huadu.iepoch.data.http.login.LoginResponseInfo;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface LoginRequest {

    @POST("/user/api/login")
    @FormUrlEncoded
    Call<LoginResponseInfo> login(@Field("LoginName") String loginName, @Field("Password") String password);
}
