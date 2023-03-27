package com.huadu.shunda.core;

import android.content.Context;

import androidx.appcompat.app.AlertDialog;

import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.login.LoginResponseInfo;

import java.io.IOException;
import java.util.Comparator;
import java.util.List;
import java.util.concurrent.TimeUnit;

import okhttp3.Headers;
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

    private static final String WEB_SESSION_KEY = "session-id";

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

                                String sessionId = WebSession.getSessionId();
                                if (sessionId == null) {
                                    sessionId = "";
                                }

                                Request.Builder builder = chain.request().newBuilder()
                                        .header(WEB_SESSION_KEY, sessionId)
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

    public static <T extends ResponseInfo> boolean processResponse(retrofit2.Response<T> response, Context context) {
        if (response.code() != 200) {
            String message = "服务器返回状态码错误，状态码" + response.code() + "！";
            showError(message, context);
            return false;
        }

        Headers headers = response.headers();
        if (headers == null) {
            showError("未能接收到服务器返回 http header！", context);
            return false;
        }

        // 保存返回的 session id
        String sessionId = "";
        List<String> sessionIdList = headers.values(WEB_SESSION_KEY);
        if (sessionIdList != null && sessionIdList.size() != 0) {
            if (sessionIdList.size() == 1) {
                sessionId = sessionIdList.get(0);
            } else {
                for (String s : sessionIdList) {
                    if (sessionId.compareTo(s) < 0) { // 取值最大的一个
                        sessionId = s;
                    }
                }
            }
        }

        WebSession.setSessionId(sessionId);


        ResponseInfo responseData = response.body();
        if (responseData == null) {
            showError("未能接收到服务器返回数据！", context);
            return false;
        }

        if (!responseData.getSuccess()) {
            showError(responseData.getMessage(), context);
            return false;
        }

        return true;
    }

    private static void showError(String message, Context context) {
        SoundUtil.playError(context);

        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context);
        alertDialogBuilder.setTitle("错误提示");
        alertDialogBuilder.setMessage(message);
        alertDialogBuilder.setPositiveButton("确定", null);

        AlertDialog alertDialog = alertDialogBuilder.create();
        alertDialog.show();
    }
}
