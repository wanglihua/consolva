package com.huadu.iepoch.core;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.ScansManager;

import com.huadu.iepoch.ui.pda.Pda2ScannerUtil;

public class PdaUtil {

    @SuppressLint("WrongConstant")
    public static boolean hasPdaScanner(Activity activity) {
        ScansManager scansManager = (ScansManager) activity.getSystemService("scans");
        if (scansManager != null) {
            return true;
        }

        boolean connected = Pda2ScannerUtil.GetConnected();
        if (connected) {
            Pda2ScannerUtil.Connect(activity);
        }

        return connected;
    }
}
