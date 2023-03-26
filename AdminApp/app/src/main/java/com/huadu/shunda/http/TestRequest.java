package com.huadu.shunda.http;



import com.huadu.shunda.data.http.test.TestResponseInfo;

import retrofit2.Call;
import retrofit2.http.POST;

public interface TestRequest {

    @POST("/user/api/test")
    Call<TestResponseInfo> index();
}
