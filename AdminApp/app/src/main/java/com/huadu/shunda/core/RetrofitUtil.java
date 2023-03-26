package com.huadu.shunda.core;

import android.content.Context;

import androidx.appcompat.app.AlertDialog;

import com.huadu.shunda.data.http.ResponseInfo;

import java.io.IOException;
import java.util.concurrent.TimeUnit;

import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import retrofit2.Retrofit;
import retrofit2.adapter.rxjava.RxJavaCallAdapterFactory;
import retrofit2.converter.gson.GsonConverterFactory;

public class RetrofitUtil {

    public static String versionName = "version name";

    public static String deviceData = "device data";

    /*

    private static final Retrofit retrofit = new Retrofit.Builder()
            .client(new OkHttpClient
                    .Builder()
                    .addInterceptor(new Interceptor() {
                        @Override
                        public Response intercept(Interceptor.Chain chain) throws IOException {

                            Request.Builder builder = chain.request().newBuilder()
                                    .header("userId", "11249")
                                    .header("sessionId", "155056366467311249");

                            Request build = builder.build();
                            return chain.proceed(build);
                        }
                    })
                    .connectTimeout(15000, TimeUnit.MILLISECONDS)
                    .readTimeout(15000, TimeUnit.MILLISECONDS)
                    .build()
            )
            .baseUrl(Config.getUrlBase())
            .addConverterFactory(GsonConverterFactory.create())
            .addCallAdapterFactory(RxJavaCallAdapterFactory.create())
            .build();

    public static <T> T createRequest(final Class<T> requestInterface) {
        return retrofit.create(requestInterface);
    }

    */

    public static <T> T createRequest(final Class<T> requestInterface) {

        Retrofit retrofit = new Retrofit.Builder()
                .client(new OkHttpClient
                        .Builder()
                        .addInterceptor(new Interceptor() {
                            @Override
                            public Response intercept(Interceptor.Chain chain) throws IOException {

                                String token = LoginCache.getToken();
                                if (token == null) {
                                    token = "";
                                }

                                Request.Builder builder = chain.request().newBuilder()
                                        .header("user_api_token", token)
                                        .header("User-Agent", "shunda app " + versionName + " " + deviceData);

                                Request build = builder.build();
                                return chain.proceed(build);
                            }
                        })
                        .connectTimeout(15000, TimeUnit.MILLISECONDS)
                        .readTimeout(15000, TimeUnit.MILLISECONDS)
                        .build()
                )
                .baseUrl(Config.getUrlBase())
                .addConverterFactory(GsonConverterFactory.create())
                .addCallAdapterFactory(RxJavaCallAdapterFactory.create())
                .build();

        return retrofit.create(requestInterface);
    }

    public static boolean checkResponse(ResponseInfo response, Context context) {

        if (response == null) {
            SoundUtil.playError(context);

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context);
            alertDialogBuilder.setTitle("错误提示");
            alertDialogBuilder.setMessage("未能接收到服务器返回数据！");
            alertDialogBuilder.setPositiveButton("确定", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();
            return false;
        }

        if (!response.getSuccess()) {
            SoundUtil.playError(context);

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context);
            alertDialogBuilder.setTitle("错误提示");
            alertDialogBuilder.setMessage(response.getMessage());
            alertDialogBuilder.setPositiveButton("确定", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();

            return false;
        }

        return true;
    }
}
