package com.huadu.shunda.data.http;

import com.google.gson.annotations.SerializedName;

public class ResponseDataInfo<T> extends ResponseInfo {
    @SerializedName("data")
    private T data;

    public T getData() {
        return data;
    }

    public void setData(T data) {
        this.data = data;
    }
}
