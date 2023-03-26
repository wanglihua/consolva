package com.huadu.shunda.core;

import android.annotation.SuppressLint;

import java.text.SimpleDateFormat;
import java.util.Date;

public class ClientTimeUtil {
    public static String getClientTime() {
        @SuppressLint("SimpleDateFormat") String clientTime = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(new Date());
        return clientTime;
    }
}
