package com.huadu.iepoch.ui.load;

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

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.LoginCache;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.PdaUtil;
import com.huadu.iepoch.core.PermissionUtil;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.shipcontainerinner.ShipContainerInnerLoadDetailDataResponseInfo;
import com.huadu.iepoch.data.http.shipcontainerinner.ShipContainerNotLoadListItemInfo;
import com.huadu.iepoch.http.PackBoxRequest;
import com.huadu.iepoch.http.ShipContainerInnerRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.pda.ScanActivity;
import com.huadu.iepoch.ui.zxing.QrScanConstants;
import com.huadu.iepoch.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PackBoxLoadShipContainerInnerDetailActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_INNER_DETAIL_DATA_LIST = 100400;
    private static final int REQ_QR_CODE_PACK_BOX = 1;

    private int shipContainerId;

    private TextView tvCode = null;
    private TextView tvGoodsTypeName = null;
    private TextView tvContainerCode = null;
    private TextView tvToWarehouseName = null;
    private TextView tvContainerVolume = null;
    private TextView tvActualImportVolume = null;

    private Button btnScan = null;

    private Switch swAutoContinueScan = null;
    private ListView lvNotLoadList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pack_box_load_ship_container_inner_detail);

        shipContainerId = getIntent().getIntExtra("shipContainerId", 0);

        tvCode = findViewById(R.id.tvCode);
        tvGoodsTypeName = findViewById(R.id.tvGoodsTypeName);
        tvContainerCode = findViewById(R.id.tvContainerCode);
        tvToWarehouseName = findViewById(R.id.tvToWarehouseName);
        tvContainerVolume = findViewById(R.id.tvContainerVolume);
        tvActualImportVolume = findViewById(R.id.tvActualImportVolume);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PACK_BOX);
            }
        });

        swAutoContinueScan = findViewById(R.id.swAutoContinueScan);
        lvNotLoadList = findViewById(R.id.lvNotLoadList);
    }

    @Override
    protected void onStart() {
        super.onStart();

        new Thread(new Runnable() {
            @Override
            public void run() {
                loadNotLoadListData();
            }
        }).start();
    }

    private void loadNotLoadListData() {
        ShipContainerInnerRequest shipContainerInnerRequest = RetrofitUtil.createRequest(ShipContainerInnerRequest.class);
        Call<ShipContainerInnerLoadDetailDataResponseInfo> loadDetailDataCall = shipContainerInnerRequest.loadDetailData(shipContainerId);
        // Dialog mask = MaskDialog.show(PackBoxLoadShipContainerInnerDetailActivity.this);
        loadDetailDataCall.enqueue(new Callback<ShipContainerInnerLoadDetailDataResponseInfo>() {
            @Override
            public void onResponse(Call<ShipContainerInnerLoadDetailDataResponseInfo> call, Response<ShipContainerInnerLoadDetailDataResponseInfo> response) {
                // mask.dismiss();

                ShipContainerInnerLoadDetailDataResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, PackBoxLoadShipContainerInnerDetailActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_SHIP_CONTAINER_INNER_DETAIL_DATA_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<ShipContainerInnerLoadDetailDataResponseInfo> call, Throwable t) {
                Toast.makeText(PackBoxLoadShipContainerInnerDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "装箱扫描箱号二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(PackBoxLoadShipContainerInnerDetailActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "装箱扫描箱号二维码");
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
                Call<ResponseInfo> loadCall = packBoxRequest.load(shipContainerId, packBoxCode, LoginCache.getUserId());
                Dialog mask = MaskDialog.show(PackBoxLoadShipContainerInnerDetailActivity.this);
                loadCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, PackBoxLoadShipContainerInnerDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(PackBoxLoadShipContainerInnerDetailActivity.this);
                        Toast.makeText(PackBoxLoadShipContainerInnerDetailActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        if (swAutoContinueScan.isChecked()) {
                            startQrCode(REQ_QR_CODE_PACK_BOX);
                        }
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(PackBoxLoadShipContainerInnerDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

            if (msg.what == REFRESH_SHIP_CONTAINER_INNER_DETAIL_DATA_LIST) {
                ShipContainerInnerLoadDetailDataResponseInfo detailDataResponse = (ShipContainerInnerLoadDetailDataResponseInfo) msg.obj;

                tvCode.setText(detailDataResponse.getCode());
                tvGoodsTypeName.setText("物品类型：" + detailDataResponse.getGoodsTypeName());
                tvContainerCode.setText("实际箱号：" + detailDataResponse.getContainerCode());
                tvToWarehouseName.setText("目的仓：" + detailDataResponse.getToWarehouseName());
                tvContainerVolume.setText("集装箱体积：" + detailDataResponse.getContainerVolume());
                tvActualImportVolume.setText("导入体积：" + detailDataResponse.getActualImportVolume());


                List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                for (ShipContainerNotLoadListItemInfo item : detailDataResponse.getItemList()) {
                    Map<String, Object> dataMap = new HashMap<String, Object>();
                    dataMap.put("customerCode", "客户ID：" + item.getCustomerCode());
                    dataMap.put("orderCode", "订单编号：" + item.getOrderCode());
                    dataMap.put("notLoadPackBoxCount", "未装箱箱数：" + item.getNotLoadPackBoxCount());
                    dataMap.put("notLoadPackBoxes", "未装箱箱序：" + item.getNotLoadPackBoxes());

                    listDataMap.add(dataMap);
                }

                if (detailDataResponse.getSuccess()) {
                    SimpleAdapter adapter = new SimpleAdapter(PackBoxLoadShipContainerInnerDetailActivity.this
                            , listDataMap
                            , R.layout.item_packbox_load_not_load_list
                            , new String[]{"customerCode", "orderCode", "notLoadPackBoxCount", "notLoadPackBoxes"}
                            , new int[]{R.id.tvCustomerCode, R.id.tvOrderCode, R.id.tvNotLoadPackBoxCount, R.id.tvNotLoadPackBoxes});

                    lvNotLoadList.setAdapter(adapter);
                } else {
                    Toast.makeText(PackBoxLoadShipContainerInnerDetailActivity.this, detailDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
