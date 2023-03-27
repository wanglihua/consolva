package com.huadu.shunda.http;


import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.login.LoginResponseData;

import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.POST;

public interface LoginRequest {

    @POST("/admin/app/login")
    @FormUrlEncoded
    Call<ResponseDataInfo<LoginResponseData>> login(@Field("loginName") String loginName, @Field("password") String password);
}
