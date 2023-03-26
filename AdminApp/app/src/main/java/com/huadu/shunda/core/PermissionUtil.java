package com.huadu.shunda.core;

import android.Manifest;
import android.app.Activity;
import android.content.pm.PackageManager;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import java.util.ArrayList;
import java.util.List;

public class PermissionUtil {

    public static boolean checkPermission(Activity activity, String[] permissions) {
        return checkPermission(activity, permissions, 0);
    }

    public static boolean checkPermission(Activity activity, String[] permissions, int requestCode) {
        if (permissions == null || permissions.length == 0) {
            return true;
        }

        List<String> permissionNeedList = new ArrayList<>();

        for (String permission : permissions) {
            if (ContextCompat.checkSelfPermission(activity, permission) != PackageManager.PERMISSION_GRANTED) {
                permissionNeedList.add(permission);
            }
        }

        if (permissionNeedList.size() != 0) {
            ActivityCompat.requestPermissions(activity, permissionNeedList.toArray(new String[0]), requestCode);
            return false;
        }

        return true;
    }

    public static boolean checkCameraStoragePermissions(Activity activity, int requestCode) {
        return PermissionUtil.checkPermission(activity,
                new String[]{Manifest.permission.CAMERA, Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE},
                requestCode);
    }
}
