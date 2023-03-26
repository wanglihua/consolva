package com.huadu.iepoch.ui.pda;

import android.app.Activity;

import com.rsk.scandemon6603.Scanner;

public class Pda2ScannerUtil {
    private static boolean connected = false;

    public static boolean GetConnected() {
        return connected;
    }

    // 程序启动界面或主界面就调用好这个
    // 后面过程中多次调用，以便加快反应速度
    public static void Connect(Activity activity) {
        Scanner pda2Scanner = Scanner.getInstance();

        // Toast.makeText(activity, pda2Scanner.getPrefix(), Toast.LENGTH_LONG).show();

        pda2Scanner.connect(activity, new Scanner.ScannerConnection() {
            @Override
            public void onConnected() {
                connected = true;
            }
        });
    }
}
