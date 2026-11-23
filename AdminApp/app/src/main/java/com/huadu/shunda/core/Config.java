package com.huadu.shunda.core;

import com.huadu.shunda.BuildConfig;

import java.util.ArrayList;
import java.util.List;

public class Config {

    private static String host = "";

    public static void setHost(String host) {
        Config.host = host;
    }

    public static String getUrlBase() {
        /*
        if (BuildConfig.DEBUG) {
            return "http://" + Config.host;
        } else {
            return "https://" + Config.host;
        }
         */

        return Config.host;
    }

    public static List<String> getHostList() {
        if (BuildConfig.DEBUG) {
            return new ArrayList<String>() {{
                add("http://192.168.0.2:9001");
                add("http://192.168.0.3:9001");
                add("http://192.168.0.4:9001");

                add("https://abc.com"); // 这样测试版也能连接正式服务器
            }};
        } else {
            return new ArrayList<String>() {{
                add("https://abc.com");
            }};
        }
    }

    public static String getOssEndPoint() {
        if (BuildConfig.DEBUG) {
            return "oss-cn-hongkong.aliyuncs.com";
        } else {
            return "oss-cn-hongkong.aliyuncs.com";
        }
    }

    public static String getOssBucketName() {
        if (BuildConfig.DEBUG) {
            return "bucketName";
        } else {
            return "bucketName";
        }
    }

    public static String getOssAccessKeyId() {
        if (BuildConfig.DEBUG) {
            return "AccessKeyId";
        } else {
            return "AccessKeyId";
        }
    }

    public static String getOssAccessKeySecret() {
        if (BuildConfig.DEBUG) {
            return "AccessKeySecret";
        } else {
            return "AccessKeySecret";
        }
    }
}
