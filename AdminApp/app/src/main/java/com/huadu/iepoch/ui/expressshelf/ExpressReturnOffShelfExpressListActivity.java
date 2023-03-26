package com.huadu.iepoch.ui.expressshelf;

import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.PdaUtil;
import com.huadu.iepoch.core.PermissionUtil;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.expressimageshow.ExpressImageShowImageItemInfo;
import com.huadu.iepoch.data.http.expressimageshow.ExpressImageShowImageListResponseInfo;
import com.huadu.iepoch.data.http.expressshelf.ExpressReturnOffShelfExpressDetailResponseInfo;
import com.huadu.iepoch.data.http.expressshelf.ExpressReturnOffShelfExpressItemInfo;
import com.huadu.iepoch.data.http.expressshelf.ExpressReturnOffShelfExpressListResponseInfo;
import com.huadu.iepoch.http.ExpressImageShowRequest;
import com.huadu.iepoch.http.ExpressReturnOffShelfRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.expressdump.ExpressNoPickupDumpExpressListActivity;
import com.huadu.iepoch.ui.pda.ScanActivity;
import com.huadu.iepoch.ui.pda.ScanWithImagesActivity;
import com.huadu.iepoch.ui.zxing.QrScanConstants;
import com.huadu.iepoch.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressReturnOffShelfExpressListActivity extends BaseActivity {

    private static final int REFRESH_EXPRESS_DATA_LIST = 100100;
    private static final int REQ_QR_CODE_EXPRESS = 1;

    private int warehouseId;
    private String warehouseCode;
    private String warehouseName;

    private TextView tvWarehouseName;
    private Button btnScan;
    private TextView tvNotOffShelfListLabel;
    private ListView lvExpressList;

    private int currentClaimId = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_return_off_shelf_express_list);

        Intent intent = getIntent();

        warehouseId = intent.getIntExtra("warehouseId", 0);
        warehouseCode = intent.getStringExtra("warehouseCode");
        warehouseName = intent.getStringExtra("warehouseName");

        tvWarehouseName = findViewById(R.id.tvWarehouseName);
        tvWarehouseName.setText("仓库：" + warehouseName);

        tvNotOffShelfListLabel = findViewById(R.id.tvNotOffShelfListLabel);

        lvExpressList = findViewById(R.id.lvExpressList);
        lvExpressList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                int claimId = (int) map.get("id");

                startQrCode(claimId);
            }
        });
    }

    @Override
    public void onStart() {
        super.onStart();

        new Thread(new Runnable() {
            @Override
            public void run() {
                loadExpressList();
            }
        }).start();
    }

    private void loadExpressList() {
        ExpressReturnOffShelfRequest expressReturnOffShelfRequest = RetrofitUtil.createRequest(ExpressReturnOffShelfRequest.class);
        Call<ExpressReturnOffShelfExpressListResponseInfo> expressListCall = expressReturnOffShelfRequest.expressList(ExpressReturnOffShelfExpressListActivity.this.warehouseId);
        // Dialog mask = MaskDialog.show(ExpressOffShelfExpressListActivity.this);
        expressListCall.enqueue(new Callback<ExpressReturnOffShelfExpressListResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressReturnOffShelfExpressListResponseInfo> call, Response<ExpressReturnOffShelfExpressListResponseInfo> response) {
                // mask.dismiss();

                ExpressReturnOffShelfExpressListResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, ExpressReturnOffShelfExpressListActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_EXPRESS_DATA_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<ExpressReturnOffShelfExpressListResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressReturnOffShelfExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void startQrCode(int claimId) {
        this.currentClaimId = claimId;

        ExpressReturnOffShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressReturnOffShelfRequest.class);
        Call<ExpressReturnOffShelfExpressDetailResponseInfo> offShelfNextExpress = expressShelfRequest.expressDetail(claimId);
        Dialog mask = MaskDialog.show(ExpressReturnOffShelfExpressListActivity.this);
        offShelfNextExpress.enqueue(new Callback<ExpressReturnOffShelfExpressDetailResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressReturnOffShelfExpressDetailResponseInfo> call, Response<ExpressReturnOffShelfExpressDetailResponseInfo> response) {
                mask.dismiss();

                ExpressReturnOffShelfExpressDetailResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, ExpressReturnOffShelfExpressListActivity.this)) {
                    return;
                }

                ExpressReturnOffShelfExpressItemInfo express = respData.getExpress();
                if (express == null) {
                    loadExpressList(); // 这里不调用的话，最后一个快递单扫描后，还留在列表那里
                    return;
                }

                final String[] labelText = {"快递单号：\r" + express.getTrackingNo()
                        + "\n澳拼入库号：" + express.getAoPinTrackingNo()
                        + "\n包裹数量：" + express.getPackageCount()
                        + "\n客户ID：" + express.getCustomerCode()
                        + "\n仓位：" + express.getStorageLocName()};

                boolean hasPdaScanner = PdaUtil.hasPdaScanner(ExpressReturnOffShelfExpressListActivity.this);
                // boolean hasPdaScanner = true; // 测试的时候临时启用
                if (hasPdaScanner == false) {
                    if (PermissionUtil.checkCameraStoragePermissions(ExpressReturnOffShelfExpressListActivity.this, REQ_QR_CODE_EXPRESS) == false) {
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

                        if (!RetrofitUtil.checkResponse(respData, ExpressReturnOffShelfExpressListActivity.this)) {
                            return;
                        }

                        List<ExpressImageShowImageItemInfo> imageItemList = respData.getImageList();
                        List<String> imageList = new ArrayList<>();
                        for (ExpressImageShowImageItemInfo imageItem : imageItemList) {
                            imageList.add(imageItem.getBigImageUrl());
                        }

                        if (hasPdaScanner) {
                            Intent intent = new Intent(ExpressReturnOffShelfExpressListActivity.this, ScanWithImagesActivity.class);
                            labelText[0] = labelText[0].replace("\n", "<br/>");
                            intent.putExtra("labelText", labelText[0]);
                            ScanWithImagesActivity.setImageList(imageList);

                            startActivityForResult(intent, REQ_QR_CODE_EXPRESS);
                        } else {
                            Intent intent = new Intent(ExpressReturnOffShelfExpressListActivity.this, CaptureActivity.class);
                            intent.putExtra("labelText", labelText[0]);
                            intent.putExtra("hasImageList", true);
                            CaptureActivity.setImageList(imageList);

                            startActivityForResult(intent, REQ_QR_CODE_EXPRESS);
                        }
                    }

                    @Override
                    public void onFailure(Call<ExpressImageShowImageListResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressReturnOffShelfExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }

            @Override
            public void onFailure(Call<ExpressReturnOffShelfExpressDetailResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressReturnOffShelfExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String expressTrackingNo = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_EXPRESS) {

                ExpressReturnOffShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressReturnOffShelfRequest.class);
                Call<ResponseInfo> offShelfComplete = expressShelfRequest.complete(currentClaimId, expressTrackingNo);
                Dialog mask = MaskDialog.show(ExpressReturnOffShelfExpressListActivity.this);
                offShelfComplete.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, ExpressReturnOffShelfExpressListActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(ExpressReturnOffShelfExpressListActivity.this);
                        Toast.makeText(ExpressReturnOffShelfExpressListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(ExpressReturnOffShelfExpressListActivity.this);
                        alertDialogBuilder.setTitle("退运下架成功");
                        alertDialogBuilder.setMessage(respData.getMessage());
                        alertDialogBuilder.setPositiveButton("确定", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                loadExpressList();
                            }
                        });

                        AlertDialog alertDialog = alertDialogBuilder.create();
                        alertDialog.show();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressReturnOffShelfExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            // 获得授权
            startQrCode(requestCode);
        } else {
            // 被禁止授权
            Toast.makeText(this, "请至权限中心打开本应用的相机访问权限", Toast.LENGTH_LONG).show();
        }
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_EXPRESS_DATA_LIST:

                    ExpressReturnOffShelfExpressListResponseInfo expressListResponse = (ExpressReturnOffShelfExpressListResponseInfo) msg.obj;

                    if (expressListResponse.getSuccess()) {
                        List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                        for (ExpressReturnOffShelfExpressItemInfo item : expressListResponse.getItemList()) {
                            Map<String, Object> dataMap = new HashMap<String, Object>();
                            dataMap.put("id", item.getId());
                            dataMap.put("trackingNo", item.getTrackingNo());
                            dataMap.put("aoPinTrackingNo", "澳拼入库号：" + item.getAoPinTrackingNo());
                            dataMap.put("customerCode", "客户ID：" + item.getCustomerCode());
                            dataMap.put("packageCount", "包裹数量：" + item.getPackageCount());
                            dataMap.put("storageLocName", "仓位：" + item.getStorageLocName());

                            listDataMap.add(dataMap);
                        }

                        // 待退运下架快递列表：
                        tvNotOffShelfListLabel.setText("待退运下架快递列表 (" + expressListResponse.getItemCount() + ")：");

                        SimpleAdapter adapter = new SimpleAdapter(ExpressReturnOffShelfExpressListActivity.this
                                , listDataMap
                                , R.layout.item_express_return_off_shelf_express_list
                                , new String[]{"trackingNo", "aoPinTrackingNo", "customerCode", "packageCount", "storageLocName"}
                                , new int[]{R.id.tvTrackingNo, R.id.tvAoPinTrackingNo, R.id.tvCustomerCode, R.id.tvPackageCount, R.id.tvStorageLocName});

                        lvExpressList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressReturnOffShelfExpressListActivity.this, expressListResponse.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                    break;
                default:
                    break;
            }
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();

            /*
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
             */
        }

        return true;
    }
}