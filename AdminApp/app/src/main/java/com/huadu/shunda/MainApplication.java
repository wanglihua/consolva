package com.huadu.shunda;

import android.app.Application;
import android.content.Context;
import android.os.Build;
import android.os.StrictMode;

public class MainApplication extends Application {
    private static Context context = null;

    @Override
    public void onCreate() {
        super.onCreate();

        context = getApplicationContext();

        StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
        StrictMode.setVmPolicy(builder.build());
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            builder.detectFileUriExposure();
        }
    }

    public static Context getContext() {
        return context;
    }
}
