package com.huadu.iepoch.core;

import com.huadu.iepoch.data.http.login.LoginResponseInfo;

public class LoginCache {

    private static int userId;

    private static String loginName;

    private static String password;

    private static String token;

    private static LoginResponseInfo loginResponse;

    public static int getUserId() {
        return userId;
    }

    public static void setUserId(int userId) {
        LoginCache.userId = userId;
    }

    public static String getLoginName() {
        return loginName;
    }

    public static void setLoginName(String loginName) {
        LoginCache.loginName = loginName;
    }

    public static String getPassword() {
        return password;
    }

    public static void setPassword(String password) {
        LoginCache.password = password;
    }

    public static String getToken() {
        return token;
    }

    public static void setToken(String token) {
        LoginCache.token = token;
    }

    public static LoginResponseInfo getLoginResponse() {
        return loginResponse;
    }

    public static void setLoginResponse(LoginResponseInfo loginResponse) {
        LoginCache.loginResponse = loginResponse;
    }

    public static void clear() {
        userId = 0;
        loginName = "";
        password = "";
        token = "";
        loginResponse = null;
    }
}
