package com.huadu.shunda.ui.unload;

import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.huadu.shunda.R;
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.stockplace.StockPlaceDetailResponseInfo;
import com.huadu.shunda.data.http.stockplace.StockPlacePackBoxItemInfo;
import com.huadu.shunda.http.PackBoxRequest;
import com.huadu.shunda.http.StockPlaceRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockPlaceDetailActivity extends BaseActivity {

    private static final int REFRESH_STOCK_PLACE_DETAIL_DATA_LIST = 100400;
    private static final int REQ_QR_CODE_PACK_BOX = 1;

    private int stockPlaceId;
    private int shipContainerId;

    private TextView tvStockPlaceName = null;
    private Button btnScan = null;
    private Switch swAutoContinueScan;
    private TextView tvPackBoxCount = null;
    private ListView lvPackBoxList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_place_detail);

        Intent intent = getIntent();
        stockPlaceId = intent.getIntExtra("stockPlaceId", 0);
        shipContainerId = intent.getIntExtra("shipContainerId", 0);

        tvStockPlaceName = findViewById(R.id.tvStockPlaceName);
        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PACK_BOX);
            }
        });

        swAutoContinueScan = findViewById(R.id.swAutoContinueScan);
        tvPackBoxCount = findViewById(R.id.tvPackBoxCount);
        lvPackBoxList = findViewById(R.id.lvPackBoxList);

        // 扫描完成后会手动加载数据
        loadDetailData();
    }

    @Override
    protected void onStart() {
        super.onStart();

        // 扫描完成后会手动加载数据
        // loadDetailData();
    }

    private void loadDetailData() {
        new Thread(new Runnable() {
            @Override
            public void run() {

                StockPlaceRequest stockPlaceRequest = RetrofitUtil.createRequest(StockPlaceRequest.class);
                Call<StockPlaceDetailResponseInfo> detailCall = stockPlaceRequest.detail(stockPlaceId);
                // Dialog mask = MaskDialog.show(StockPlaceDetailActivity.this);
                detailCall.enqueue(new Callback<StockPlaceDetailResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockPlaceDetailResponseInfo> call, Response<StockPlaceDetailResponseInfo> response) {
                        // mask.dismiss();

                        StockPlaceDetailResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, StockPlaceDetailActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_STOCK_PLACE_DETAIL_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockPlaceDetailResponseInfo> call, Throwable t) {
                        Toast.makeText(StockPlaceDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });


            }
        }).start();
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "卸货扫描箱号二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(StockPlaceDetailActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "卸货扫描箱号二维码");
            startActivityForResult(intent, requestCode);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String packBoxCode = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_PACK_BOX) {

                PackBoxRequest packBoxRequest = RetrofitUtil.createRequest(PackBoxRequest.class);
                Call<ResponseInfo> unloadCall = packBoxRequest.unload(stockPlaceId, packBoxCode, LoginCache.getUserId());
                Dialog mask = MaskDialog.show(StockPlaceDetailActivity.this);
                unloadCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, StockPlaceDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(StockPlaceDetailActivity.this);

                        Toast.makeText(StockPlaceDetailActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        if (swAutoContinueScan.isChecked()) {
                            startQrCode(REQ_QR_CODE_PACK_BOX);
                        }

                        loadDetailData();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(StockPlaceDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

            if (msg.what == REFRESH_STOCK_PLACE_DETAIL_DATA_LIST) {
                StockPlaceDetailResponseInfo detailDataResponse = (StockPlaceDetailResponseInfo) msg.obj;

                tvStockPlaceName.setText(detailDataResponse.getName());

                tvPackBoxCount.setText("共 " + detailDataResponse.getPackBoxItemCount() + " 件");

                List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                for (StockPlacePackBoxItemInfo item : detailDataResponse.getPackBoxItemList()) {
                    Map<String, Object> dataMap = new HashMap<String, Object>();
                    dataMap.put("id", item.getId());
                    dataMap.put("boxCode", "箱号：" + item.getBoxCode());
                    dataMap.put("orderCode", "订单编号：" + item.getOrderCode());
                    dataMap.put("customerCode", "客户ID：" + item.getCustomerCode());
                    dataMap.put("shipContainer", "船航批次：" + item.getShipContainer());

                    if (item.getUnloadTime() == null) {
                        dataMap.put("unloadTime", "卸货时间：无");
                    } else {
                        dataMap.put("unloadTime", "卸货时间：" + item.getUnloadTime());
                    }

                    listDataMap.add(dataMap);
                }

                if (detailDataResponse.getSuccess()) {
                    SimpleAdapter adapter = new SimpleAdapter(StockPlaceDetailActivity.this
                            , listDataMap
                            , R.layout.item_stock_place_pack_box_list
                            , new String[]{"boxCode", "orderCode", "customerCode", "shipContainer", "unloadTime"}
                            , new int[]{R.id.tvBoxCode, R.id.tvOrderCode, R.id.tvCustomerCode, R.id.tvShipContainer, R.id.tvUnloadTime});

                    lvPackBoxList.setAdapter(adapter);
                } else {
                    Toast.makeText(StockPlaceDetailActivity.this, detailDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
                }
            }
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {

            finish();

            /*
            Intent intent = new Intent(this, StockPlaceListActivity.class);
            intent.putExtra("shipContainerId", shipContainerId);
            startActivity(intent);
             */
        }

        return true;
    }
}
