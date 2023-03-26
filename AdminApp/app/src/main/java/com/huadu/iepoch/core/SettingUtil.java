package com.huadu.iepoch.core;

import android.content.SharedPreferences;

import com.huadu.iepoch.MainApplication;

import static android.content.Context.MODE_PRIVATE;

public class SettingUtil {

    private static final String sharedPreferenceName = "config";
    private static final String loginNameKey = "login_name";
    private static final String passwordKey = "password";
    private static final String hostKey = "host";
    private static final String expressOffShelfShouldSkipExpressListKey = "expressOffShelfShouldSkipExpressList";

    public static String getLoginName() {
        return MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE).getString(loginNameKey, "");
    }

    public static String getPassword() {
        return MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE).getString(passwordKey, "");
    }

    public static String getHost() {
        return MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE).getString(hostKey, "");
    }

    public static boolean getExpressOffShelfShouldSkipExpressList() {
        return MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE).getBoolean(expressOffShelfShouldSkipExpressListKey, true);
    }

    public static void saveLoginName(String loginName) {
        SharedPreferences preferences = MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putString(loginNameKey, loginName);
        editor.apply();
    }

    public static void savePassword(String password) {
        SharedPreferences preferences = MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putString(passwordKey, password);
        editor.apply();
    }

    public static void saveHost(String host) {
        SharedPreferences preferences = MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putString(hostKey, host);
        editor.apply();
    }

    public static void saveExpressOffShelfShouldSkipExpressList(boolean shouldSkipExpressList) {
        SharedPreferences preferences = MainApplication.getContext().getSharedPreferences(sharedPreferenceName, MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putBoolean(expressOffShelfShouldSkipExpressListKey, shouldSkipExpressList);
        editor.apply();
    }
}
