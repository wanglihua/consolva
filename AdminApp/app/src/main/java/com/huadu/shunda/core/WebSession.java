package com.huadu.shunda.core;

public class WebSession {
    private static String sessionId;

    public static String getSessionId() {
        if(sessionId == null) {
            return "";
        }

        return sessionId;
    }

    public static void setSessionId(String sessionId) {
        WebSession.sessionId = sessionId;
    }
}
