package com.huadu.shunda.core;

import com.huadu.shunda.data.http.login.LoginResponseData;

public class LoginCache {

    private static int userId;

    private static String loginName;

    private static String password;

    private static LoginResponseData loginResponse;

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

    public static LoginResponseData getLoginResponse() {
        return loginResponse;
    }

    public static void setLoginResponse(LoginResponseData loginResponse) {
        LoginCache.loginResponse = loginResponse;
    }

    public static void clear() {
        userId = 0;
        loginName = "";
        password = "";
        loginResponse = null;
    }
}
