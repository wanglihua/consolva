package com.huadu.shunda.ui.expressshelf;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.widget.Toast;

import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressimageshow.ExpressImageShowImageItemInfo;
import com.huadu.shunda.data.http.expressimageshow.ExpressImageShowImageListResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfExpressItemInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfNextExpressResponseInfo;
import com.huadu.shunda.http.ExpressImageShowRequest;
import com.huadu.shunda.http.ExpressShelfRequest;
import com.huadu.shunda.ui.pda.ScanWithImagesActivity;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressOffShelfScanUtil {

    public static void startScanTrackingNo(int requestCode, int shelfId, Activity activity) {
        ExpressShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressShelfRequest.class);
        Call<ExpressShelfOffShelfNextExpressResponseInfo> offShelfNextExpress = expressShelfRequest.offShelfNextExpress(shelfId);
        Dialog mask = MaskDialog.show(activity);
        offShelfNextExpress.enqueue(new Callback<ExpressShelfOffShelfNextExpressResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressShelfOffShelfNextExpressResponseInfo> call, Response<ExpressShelfOffShelfNextExpressResponseInfo> response) {
                mask.dismiss();

                ExpressShelfOffShelfNextExpressResponseInfo respData = response.body();

                if (!RetrofitUtil.processResponse(response, activity)) {
                    return;
                }

                ExpressShelfOffShelfExpressItemInfo express = respData.getExpress();
                if (express == null) {
                    return;
                }

                final String[] labelText = {"快递单号：<font color=green><big><big>" + express.getTrackingNo() + "</big></big></font>"
                        + "\n舜达入库号：" + express.getAoPinTrackingNo()
                        + "\n包裹数量：" + express.getPackageCount()
                        + "\n客户ID：" + express.getCustomerCode()
                        + "\n仓位：<font color=green><big><big>" + express.getStorageLocName() + "</big></big></font>"};

                boolean hasPdaScanner = PdaUtil.hasPdaScanner(activity);
                // boolean hasPdaScanner = true; // 测试的时候临时启用
                if (hasPdaScanner == false) {
                    if (PermissionUtil.checkCameraStoragePermissions(activity, requestCode) == false) {
                        return;
                    }
                }

                ExpressImageShowRequest expressImageShowRequest = RetrofitUtil.createRequest(ExpressImageShowRequest.class);
                Call<ExpressImageShowImageListResponseInfo> imageListCall = expressImageShowRequest.imageListCn(express.getTrackingNo());
                // Dialog mask = MaskDialog.show(activity); 不显示这个
                imageListCall.enqueue(new Callback<ExpressImageShowImageListResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressImageShowImageListResponseInfo> call, Response<ExpressImageShowImageListResponseInfo> response) {
                        // mask.dismiss();

                        ExpressImageShowImageListResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, activity)) {
                            return;
                        }

                        List<ExpressImageShowImageItemInfo> imageItemList = respData.getImageList();
                        List<String> imageList = new ArrayList<>();
                        for (ExpressImageShowImageItemInfo imageItem : imageItemList) {
                            imageList.add(imageItem.getBigImageUrl());
                        }

                        if (hasPdaScanner) {
                            Intent intent = new Intent(activity, ScanWithImagesActivity.class);
                            labelText[0] = labelText[0].replace("\n", "<br/>");
                            intent.putExtra("labelText", labelText[0]);
                            ScanWithImagesActivity.setImageList(imageList);

                            activity.startActivityForResult(intent, requestCode);
                        } else {
                            Intent intent = new Intent(activity, CaptureActivity.class);
                            intent.putExtra("labelText", labelText[0]);
                            intent.putExtra("hasImageList", true);
                            CaptureActivity.setImageList(imageList);

                            activity.startActivityForResult(intent, requestCode);
                        }
                    }

                    @Override
                    public void onFailure(Call<ExpressImageShowImageListResponseInfo> call, Throwable t) {
                        Toast.makeText(activity, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });

                return;
            }

            @Override
            public void onFailure(Call<ExpressShelfOffShelfNextExpressResponseInfo> call, Throwable t) {
                Toast.makeText(activity, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    public static void finishScanTrackingNo(int requestCode, int shelfId, String trackingNo, Activity activity, boolean fromExpressList) {
        ExpressShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressShelfRequest.class);
        Call<ResponseInfo> offShelfComplete = expressShelfRequest.offShelfComplete(shelfId, trackingNo);
        Dialog mask = MaskDialog.show(activity);
        offShelfComplete.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                mask.dismiss();

                ResponseInfo respData = response.body();

                if (!RetrofitUtil.processResponse(response, activity)) {
                    return;
                }

                SoundUtil.playSuccess(activity);

                Toast.makeText(activity, respData.getMessage(), Toast.LENGTH_SHORT).show();

                if (fromExpressList) {
                    ((ExpressOffShelfExpressListActivity) activity).loadExpressList();
                }

                startScanTrackingNo(requestCode, shelfId, activity);

                return;
            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(activity, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
