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
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortOrderListItem;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxDetailItem;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxNotScanListResponse;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxScanResultResponse;
import com.huadu.shunda.http.ShipContainerUnloadSortPackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class UnloadSortShipContainerDetailActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_NOT_SCAN_LIST = 100400;
    private static final int REQ_SCAN_QR_CODE_PACK_BOX = 1;

    private int shipContainerId;
    private String shipContainerCode;
    private String orderPackBoxCodeCache;
    private int shipContainerOrderPackBoxId;
    private String positionNameCache;

    private TextView tvShipContainerCode = null;
    private TextView tvOrderCode = null;
    private TextView tvCustomerCode = null;
    private TextView tvOrderPackBoxCode = null;
    private TextView tvOrderPackBoxShortCode = null;
    private TextView tvTotalBoxCount = null;
    private TextView tvScannedBoxCount = null;
    private TextView tvNoScannedBoxCount = null;
    private TextView tvOrderPosition = null;
    private TextView tvPosition = null;
    private Switch swMPosition = null;

    private Button btnScan = null;

    private TextView tvShipContainerScannedCount = null;
    private ListView lvNoScannedBoxList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_unload_sort_ship_container_detail);

        shipContainerId = getIntent().getIntExtra("shipContainerId", 0);
        shipContainerCode = getIntent().getStringExtra("shipContainerCode");
        orderPackBoxCodeCache = "";

        tvShipContainerCode = findViewById(R.id.tvShipContainerCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvOrderPackBoxCode = findViewById(R.id.tvOrderPackBoxCode);
        tvOrderPackBoxShortCode = findViewById(R.id.tvOrderPackBoxShortCode);
        tvTotalBoxCount = findViewById(R.id.tvTotalBoxCount);
        tvScannedBoxCount = findViewById(R.id.tvScannedBoxCount);
        tvNoScannedBoxCount = findViewById(R.id.tvNoScannedBoxCount);
        tvOrderPosition = findViewById(R.id.tvOrderPosition);
        tvPosition = findViewById(R.id.tvPosition);
        swMPosition = findViewById(R.id.swMPosition);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_SCAN_QR_CODE_PACK_BOX);
            }
        });

        tvShipContainerCode.setText("箱号: " + shipContainerCode);
        hideScannedDetail();

        tvShipContainerScannedCount = findViewById(R.id.tvShipContainerScannedCount);
        lvNoScannedBoxList = findViewById(R.id.lvNoScannedBoxList);
    }

    @Override
    protected void onStart() {
        super.onStart();

        new Thread(new Runnable() {
            @Override
            public void run() {
                if (orderPackBoxCodeCache != null && !orderPackBoxCodeCache.equals("")) {
                    loadScanResult(orderPackBoxCodeCache, false);
                }

                loadNotLoadListData();
            }
        }).start();
    }

    private void hideScannedDetail() {
        // tvShipContainerCode.setText("");
        tvOrderCode.setText("");
        tvCustomerCode.setText("");
        tvOrderPackBoxCode.setText("");
        tvOrderPackBoxShortCode.setText("");
        tvTotalBoxCount.setText("");
        tvScannedBoxCount.setText("");
        tvNoScannedBoxCount.setText("");
        tvOrderPosition.setText("");
        tvPosition.setText("");
    }

    private void loadNotLoadListData() {
        ShipContainerUnloadSortPackBoxRequest shipContainerOrderPackBoxRequest = RetrofitUtil.createRequest(ShipContainerUnloadSortPackBoxRequest.class);
        Call<ShipContainerUnloadSortPackBoxNotScanListResponse> loadDetailDataCall = shipContainerOrderPackBoxRequest.notScanList(shipContainerId);
        loadDetailDataCall.enqueue(new Callback<ShipContainerUnloadSortPackBoxNotScanListResponse>() {
            @Override
            public void onResponse(Call<ShipContainerUnloadSortPackBoxNotScanListResponse> call, Response<ShipContainerUnloadSortPackBoxNotScanListResponse> response) {
                ShipContainerUnloadSortPackBoxNotScanListResponse respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, UnloadSortShipContainerDetailActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_SHIP_CONTAINER_NOT_SCAN_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<ShipContainerUnloadSortPackBoxNotScanListResponse> call, Throwable t) {
                Toast.makeText(UnloadSortShipContainerDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void loadScanResult(String orderPackBoxCode, boolean afterScan) {
        // hideScannedDetail();

        Dialog mask = null;
        if (afterScan) {
            mask = MaskDialog.show(UnloadSortShipContainerDetailActivity.this);
        }

        ShipContainerUnloadSortPackBoxRequest shipContainerOrderPackBoxRequest = RetrofitUtil.createRequest(ShipContainerUnloadSortPackBoxRequest.class);
        Call<ShipContainerUnloadSortPackBoxScanResultResponse> loadCall = shipContainerOrderPackBoxRequest.scanResult(shipContainerId, orderPackBoxCode);

        Dialog finalMask = mask;
        loadCall.enqueue(new Callback<ShipContainerUnloadSortPackBoxScanResultResponse>() {
            @Override
            public void onResponse(Call<ShipContainerUnloadSortPackBoxScanResultResponse> call, Response<ShipContainerUnloadSortPackBoxScanResultResponse> response) {
                if (finalMask != null) {
                    finalMask.dismiss();
                }

                ShipContainerUnloadSortPackBoxScanResultResponse respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, UnloadSortShipContainerDetailActivity.this)) {
                    return;
                }

                SoundUtil.playSuccess(UnloadSortShipContainerDetailActivity.this);

                ShipContainerUnloadSortPackBoxDetailItem orderPackBox = respData.getOrderPackBox();

                shipContainerOrderPackBoxId = orderPackBox.getShipContainerOrderPackBoxId();
                orderPackBoxCodeCache = orderPackBox.getOrderPackBoxCode();

                tvShipContainerCode.setText("集装箱号: " + orderPackBox.getShipContainerCode());
                tvOrderCode.setText("订单编号: " + orderPackBox.getOrderCode());
                tvCustomerCode.setText("客户ID: " + orderPackBox.getCustomerCode());
                tvOrderPackBoxCode.setText("箱号: " + orderPackBox.getOrderPackBoxCode());
                tvOrderPackBoxShortCode.setText("短箱号: " + orderPackBox.getOrderPackBoxShortCode());
                tvTotalBoxCount.setText("订单装箱总数: " + orderPackBox.getTotalBoxCount());
                tvScannedBoxCount.setText("订单已扫码箱数: " + orderPackBox.getScannedBoxCount());
                tvNoScannedBoxCount.setText("订单未扫码箱数 " + orderPackBox.getNoScannedBoxCount());
                tvOrderPosition.setText("订单位置: " + orderPackBox.getOrderPositionNames());
                tvPosition.setText("箱号位置: " + orderPackBox.getPositionName());

                if (!"".equals(orderPackBox.getPositionName())) {
                    positionNameCache = orderPackBox.getPositionName();
                } else {
                    positionNameCache = orderPackBox.getOrderFirstPosition();
                }

                if (afterScan) {
                    // Toast.makeText(UnloadSortShipContainerDetailActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                    Intent intent = new Intent(UnloadSortShipContainerDetailActivity.this, UnloadSortOrderPackBoxPositionActivity.class);
                    intent.putExtra("shipContainerOrderPackBoxId", shipContainerOrderPackBoxId);
                    intent.putExtra("multiOrderOnePos", swMPosition.isChecked());

                    // startActivity(intent);
                    startActivityForResult(intent, UnloadSortOrderPackBoxPositionActivity.REQ_SCAN_AGAIN);
                }
            }

            @Override
            public void onFailure(Call<ShipContainerUnloadSortPackBoxScanResultResponse> call, Throwable t) {
                Toast.makeText(UnloadSortShipContainerDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描箱号");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(UnloadSortShipContainerDetailActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "扫描箱号");
            startActivityForResult(intent, requestCode);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String packBoxCode = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_SCAN_QR_CODE_PACK_BOX) {
                loadScanResult(packBoxCode, true);
            }

            if (requestCode == UnloadSortOrderPackBoxPositionActivity.REQ_SCAN_AGAIN) {
                boolean scanAgain = data.getExtras().getBoolean("scanAgain", false);
                if (scanAgain) {
                    startQrCode(REQ_SCAN_QR_CODE_PACK_BOX);
                }
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
            Toast.makeText(this, "请授予相机权限", Toast.LENGTH_LONG).show();
        }
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);

            if (msg.what == REFRESH_SHIP_CONTAINER_NOT_SCAN_LIST) {
                ShipContainerUnloadSortPackBoxNotScanListResponse notScanListResponse = (ShipContainerUnloadSortPackBoxNotScanListResponse) msg.obj;

                tvShipContainerScannedCount.setText(String.format(Locale.CHINA, "已扫码箱号: %d / %d", notScanListResponse.getScannedCount(), notScanListResponse.getTotalCount()));

                List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                for (ShipContainerUnloadSortOrderListItem item : notScanListResponse.getNotScannedOrderList()) {
                    Map<String, Object> dataMap = new HashMap<String, Object>();
                    List<String> notScannedBoxNumList = item.getNotScannedBoxNumList();
                    if (notScannedBoxNumList == null) {
                        notScannedBoxNumList = new ArrayList<>();
                    }
                    int notScannedBoxCount = notScannedBoxNumList.size();
                    StringBuilder notScannedBoxNumsBuilder = new StringBuilder(); // String.join(",", notScannedBoxNumList);
                    for (int index = 0; index < notScannedBoxCount; index++) {
                        notScannedBoxNumsBuilder.append(notScannedBoxNumList.get(index));
                        if (index != notScannedBoxCount - 1) {
                            notScannedBoxNumsBuilder.append(", ");
                        }
                    }
                    String notScannedBoxNums = notScannedBoxNumsBuilder.toString();

                    StringBuilder orderPackBoxCodeBuilder = new StringBuilder();
                    orderPackBoxCodeBuilder.append(item.getSeqNo());
                    orderPackBoxCodeBuilder.append(". ");
                    orderPackBoxCodeBuilder.append(item.getOrderCode());
                    orderPackBoxCodeBuilder.append(" / 未扫");
                    orderPackBoxCodeBuilder.append(notScannedBoxCount);
                    orderPackBoxCodeBuilder.append("箱(");
                    orderPackBoxCodeBuilder.append(notScannedBoxNums);
                    orderPackBoxCodeBuilder.append(") / 共");
                    orderPackBoxCodeBuilder.append(item.getPackBoxLoadCount());
                    orderPackBoxCodeBuilder.append("箱");
                    orderPackBoxCodeBuilder.append("\n    ");
                    orderPackBoxCodeBuilder.append(item.getPositions());

                    dataMap.put("orderPackBoxCode", orderPackBoxCodeBuilder.toString());
                    listDataMap.add(dataMap);
                }

                if (notScanListResponse.getSuccess()) {
                    SimpleAdapter adapter = new SimpleAdapter(UnloadSortShipContainerDetailActivity.this
                            , listDataMap
                            , R.layout.item_unload_sort_not_scan_list
                            , new String[]{"orderPackBoxCode"}
                            , new int[]{R.id.tvOrderPackBoxCode});

                    lvNoScannedBoxList.setAdapter(adapter);
                } else {
                    Toast.makeText(UnloadSortShipContainerDetailActivity.this, notScanListResponse.getMessage(), Toast.LENGTH_SHORT).show();
                }

            }
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            /*
            Intent intent = new Intent(this, PackBoxLoadShipContainerInnerListActivity.class);
            startActivity(intent);
             */

            finish();
        }

        return true;
    }
}
