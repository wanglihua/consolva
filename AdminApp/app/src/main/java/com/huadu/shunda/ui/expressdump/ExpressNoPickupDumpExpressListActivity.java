package com.huadu.shunda.ui.expressdump;

import android.app.Dialog;
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

import com.huadu.shunda.R;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressdump.ExpressDumpExpressItemInfo;
import com.huadu.shunda.data.http.expressdump.ExpressDumpExpressListResponseInfo;
import com.huadu.shunda.data.http.expressdump.ExpressDumpNextExpressResponseInfo;
import com.huadu.shunda.data.http.expressphoto.ExpressPhotoExpressPhotoListItem;
import com.huadu.shunda.data.http.expressphoto.ExpressPhotoExpressPhotoListResponseData;
import com.huadu.shunda.http.ExpressPhotoRequest;
import com.huadu.shunda.http.ExpressNoPickupDumpRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanWithImagesActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressNoPickupDumpExpressListActivity extends BaseActivity {

    private static final int REFRESH_EXPRESS_DATA_LIST = 100100;
    private static final int REQ_QR_CODE_EXPRESS = 1;

    private int shelfId;
    private int targetStorageLocId;
    private String targetStorageLocName;

    private TextView tvTargetStorageName;
    private Button btnScan;
    private TextView tvExpressListLabel;
    private ListView lvExpressList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_no_pickup_dump_express_list);

        Intent intent = getIntent();

        shelfId = intent.getIntExtra("shelfId", 0);
        targetStorageLocId = intent.getIntExtra("targetStorageLocId", 0);
        targetStorageLocName = intent.getStringExtra("targetStorageLocName");

        tvTargetStorageName = findViewById(R.id.tvTargetStorageName);
        tvTargetStorageName.setText("堆存目的仓位：" + targetStorageLocName);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_EXPRESS);
            }
        });

        tvExpressListLabel = findViewById(R.id.tvExpressListLabel);

        lvExpressList = findViewById(R.id.lvExpressList);
        lvExpressList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                int claimId = (int) map.get("id");

                ExpressNoPickupDumpRequest expressNoPickupDumpRequest = RetrofitUtil.createRequest(ExpressNoPickupDumpRequest.class);
                Call<ExpressDumpNextExpressResponseInfo> express = expressNoPickupDumpRequest.express(shelfId, claimId);

                Dialog mask = MaskDialog.show(ExpressNoPickupDumpExpressListActivity.this);
                express.enqueue(new Callback<ExpressDumpNextExpressResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressDumpNextExpressResponseInfo> call, Response<ExpressDumpNextExpressResponseInfo> response) {
                        mask.dismiss();

                        if (!RetrofitUtil.processResponse(response, ExpressNoPickupDumpExpressListActivity.this)) {
                            return;
                        }

                        ExpressDumpNextExpressResponseInfo respData = response.body();

                        ExpressDumpExpressItemInfo express = respData.getExpress();
                        if (express == null) {
                            loadExpressList(); // 这里不调用的话，最后一个快递单扫描后，还留在列表那里
                            return;
                        }

                        final String[] labelText = {"快递单号：\r" + express.getTrackingNo()
                                + "\n舜达入库号：" + express.getAoPinTrackingNo()
                                + "\n包裹数量：" + express.getPackageCount()
                                + "\n仓位：" + express.getStorageLocName()};

                        boolean hasPdaScanner = PdaUtil.hasPdaScanner(ExpressNoPickupDumpExpressListActivity.this);
                        // boolean hasPdaScanner = true; // 测试的时候临时启用
                        if (hasPdaScanner == false) {
                            if (PermissionUtil.checkCameraStoragePermissions(ExpressNoPickupDumpExpressListActivity.this, REQ_QR_CODE_EXPRESS) == false) {
                                return;
                            }
                        }

                        ExpressPhotoRequest expressPhotoRequest = RetrofitUtil.createRequest(ExpressPhotoRequest.class);
                        Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> imageListCall = expressPhotoRequest.expressPhotoList(express.getTrackingNo());
                        // Dialog mask = MaskDialog.show(activity); 不显示这个
                        imageListCall.enqueue(new Callback<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>>() {
                            @Override
                            public void onResponse(Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> call, Response<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> response) {
                                // mask.dismiss();
                                if (!RetrofitUtil.processResponse(response, ExpressNoPickupDumpExpressListActivity.this)) {
                                    return;
                                }

                                ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData> respData = response.body();

                                List<ExpressPhotoExpressPhotoListItem> imageItemList = respData.getData().getPhotoList();
                                List<String> imageList = new ArrayList<>();
                                for (ExpressPhotoExpressPhotoListItem imageItem : imageItemList) {
                                    imageList.add(imageItem.getBigImageUrl());
                                }

                                if (hasPdaScanner) {
                                    Intent intent = new Intent(ExpressNoPickupDumpExpressListActivity.this, ScanWithImagesActivity.class);
                                    labelText[0] = labelText[0].replace("\n", "<br/>");
                                    intent.putExtra("labelText", labelText[0]);
                                    ScanWithImagesActivity.setImageList(imageList);

                                    startActivityForResult(intent, REQ_QR_CODE_EXPRESS);
                                } else {
                                    Intent intent = new Intent(ExpressNoPickupDumpExpressListActivity.this, CaptureActivity.class);
                                    intent.putExtra("labelText", labelText[0]);
                                    intent.putExtra("hasImageList", true);
                                    CaptureActivity.setImageList(imageList);

                                    startActivityForResult(intent, REQ_QR_CODE_EXPRESS);
                                }
                            }

                            @Override
                            public void onFailure(Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> call, Throwable t) {
                                Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        });

                        return;
                    }

                    @Override
                    public void onFailure(Call<ExpressDumpNextExpressResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
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
        ExpressNoPickupDumpRequest expressNoPickupDumpRequest = RetrofitUtil.createRequest(ExpressNoPickupDumpRequest.class);
        Call<ExpressDumpExpressListResponseInfo> warehouseListCall = expressNoPickupDumpRequest.expressList(ExpressNoPickupDumpExpressListActivity.this.shelfId);
        // Dialog mask = MaskDialog.show(ExpressOffShelfExpressListActivity.this);
        warehouseListCall.enqueue(new Callback<ExpressDumpExpressListResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressDumpExpressListResponseInfo> call, Response<ExpressDumpExpressListResponseInfo> response) {
                // mask.dismiss();

                if (!RetrofitUtil.processResponse(response, ExpressNoPickupDumpExpressListActivity.this)) {
                    return;
                }

                ExpressDumpExpressListResponseInfo respData = response.body();

                Message msg = new Message();
                msg.what = REFRESH_EXPRESS_DATA_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<ExpressDumpExpressListResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void startQrCode(int requestCode) {
        ExpressNoPickupDumpRequest expressNoPickupDumpRequest = RetrofitUtil.createRequest(ExpressNoPickupDumpRequest.class);
        Call<ExpressDumpNextExpressResponseInfo> nextExpress = expressNoPickupDumpRequest.nextExpress(shelfId);

        Dialog mask = MaskDialog.show(ExpressNoPickupDumpExpressListActivity.this);
        nextExpress.enqueue(new Callback<ExpressDumpNextExpressResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressDumpNextExpressResponseInfo> call, Response<ExpressDumpNextExpressResponseInfo> response) {
                mask.dismiss();

                if (!RetrofitUtil.processResponse(response, ExpressNoPickupDumpExpressListActivity.this)) {
                    return;
                }

                ExpressDumpNextExpressResponseInfo respData = response.body();

                ExpressDumpExpressItemInfo express = respData.getExpress();
                if (express == null) {
                    loadExpressList(); // 这里不调用的话，最后一个快递单扫描后，还留在列表那里
                    return;
                }

                final String[] labelText = {"快递单号：\r" + express.getTrackingNo()
                        + "\n舜达入库号：" + express.getAoPinTrackingNo()
                        + "\n包裹数量：" + express.getPackageCount()
                        + "\n仓位：" + express.getStorageLocName()};

                boolean hasPdaScanner = PdaUtil.hasPdaScanner(ExpressNoPickupDumpExpressListActivity.this);
                // boolean hasPdaScanner = true; // 测试的时候临时启用
                if (hasPdaScanner == false) {
                    if (PermissionUtil.checkCameraStoragePermissions(ExpressNoPickupDumpExpressListActivity.this, requestCode) == false) {
                        return;
                    }
                }

                ExpressPhotoRequest expressPhotoRequest = RetrofitUtil.createRequest(ExpressPhotoRequest.class);
                Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> imageListCall = expressPhotoRequest.expressPhotoList(express.getTrackingNo());
                // Dialog mask = MaskDialog.show(activity); 不显示这个
                imageListCall.enqueue(new Callback<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>>() {
                    @Override
                    public void onResponse(Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> call, Response<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> response) {
                        // mask.dismiss();

                        if (!RetrofitUtil.processResponse(response, ExpressNoPickupDumpExpressListActivity.this)) {
                            return;
                        }

                        ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData> respData = response.body();

                        List<ExpressPhotoExpressPhotoListItem> imageItemList = respData.getData().getPhotoList();
                        List<String> imageList = new ArrayList<>();
                        for (ExpressPhotoExpressPhotoListItem imageItem : imageItemList) {
                            imageList.add(imageItem.getBigImageUrl());
                        }

                        if (hasPdaScanner) {
                            Intent intent = new Intent(ExpressNoPickupDumpExpressListActivity.this, ScanWithImagesActivity.class);
                            labelText[0] = labelText[0].replace("\n", "<br/>");
                            intent.putExtra("labelText", labelText[0]);
                            ScanWithImagesActivity.setImageList(imageList);

                            startActivityForResult(intent, requestCode);
                        } else {
                            Intent intent = new Intent(ExpressNoPickupDumpExpressListActivity.this, CaptureActivity.class);
                            intent.putExtra("labelText", labelText[0]);
                            intent.putExtra("hasImageList", true);
                            CaptureActivity.setImageList(imageList);

                            startActivityForResult(intent, requestCode);
                        }
                    }

                    @Override
                    public void onFailure(Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> call, Throwable t) {
                        Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });

                return;
            }

            @Override
            public void onFailure(Call<ExpressDumpNextExpressResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String trackingNo = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_EXPRESS) {

                ExpressNoPickupDumpRequest expressNoPickupDumpRequest = RetrofitUtil.createRequest(ExpressNoPickupDumpRequest.class);
                Call<ResponseInfo> offShelfComplete = expressNoPickupDumpRequest.complete(ExpressNoPickupDumpExpressListActivity.this.shelfId, ExpressNoPickupDumpExpressListActivity.this.targetStorageLocId, trackingNo);
                Dialog mask = MaskDialog.show(ExpressNoPickupDumpExpressListActivity.this);
                offShelfComplete.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        if (!RetrofitUtil.processResponse(response, ExpressNoPickupDumpExpressListActivity.this)) {
                            return;
                        }

                        ResponseInfo respData = response.body();

                        SoundUtil.playSuccess(ExpressNoPickupDumpExpressListActivity.this);
                        Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        startQrCode(REQ_QR_CODE_EXPRESS);

                        return;

                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    ExpressDumpExpressListResponseInfo expressListResponse = (ExpressDumpExpressListResponseInfo) msg.obj;

                    tvExpressListLabel.setText("货架" + expressListResponse.getShelfName() + " 长期未认领 待堆存快递");

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ExpressDumpExpressItemInfo item : expressListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("trackingNo", item.getTrackingNo());
                        dataMap.put("aoPinTrackingNo", "舜达入库号：" + item.getAoPinTrackingNo());
                        // dataMap.put("customerCode", "客户ID：" + item.getCustomerCode());
                        dataMap.put("packageCount", "包裹数量：" + item.getPackageCount());
                        dataMap.put("storageLocName", "仓位：" + item.getStorageLocName());
                        dataMap.put("addTime", "入库时间：" + item.getAddTime());
                        dataMap.put("dumpStatus", "堆存状态：" + item.getDumpStatus());

                        listDataMap.add(dataMap);
                    }

                    if (expressListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(ExpressNoPickupDumpExpressListActivity.this
                                , listDataMap
                                , R.layout.item_express_no_pickup_dump_express_list
                                , new String[]{"trackingNo", "aoPinTrackingNo", "customerCode", "packageCount", "storageLocName", "addTime", "dumpStatus"}
                                , new int[]{R.id.tvTrackingNo, R.id.tvAoPinTrackingNo, R.id.tvCustomerCode, R.id.tvPackageCount, R.id.tvStorageLocName, R.id.tvAddTime, R.id.tvDumpStatus});

                        lvExpressList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressNoPickupDumpExpressListActivity.this, expressListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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