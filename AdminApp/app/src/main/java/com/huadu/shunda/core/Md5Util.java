package com.huadu.shunda.core;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;

public class Md5Util {

    public static String encrypt(String dataStr) {
        try {
            MessageDigest messageDigest = MessageDigest.getInstance("MD5");
            messageDigest.update(dataStr.getBytes(StandardCharsets.UTF_8));
            byte digest[] = messageDigest.digest();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < digest.length; i++) {
                sb.append(Integer.toHexString((0x000000FF & digest[i]) | 0xFFFFFF00).substring(6));
            }
            return sb.toString();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return "";
    }
}
