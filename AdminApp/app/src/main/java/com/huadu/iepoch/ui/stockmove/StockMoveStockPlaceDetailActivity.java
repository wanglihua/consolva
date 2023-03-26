package com.huadu.iepoch.ui.stockmove;

import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.PdaUtil;
import com.huadu.iepoch.core.PermissionUtil;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.stockmove.StockMovePackBoxItemInfo;
import com.huadu.iepoch.data.http.stockmove.StockMoveStockPlaceDetailResponseInfo;
import com.huadu.iepoch.http.StockMoveRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.pda.ScanActivity;
import com.huadu.iepoch.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockMoveStockPlaceDetailActivity extends BaseActivity {

    private static final int REFRESH_STOCK_PLACE_DETAIL_DATA_LIST = 100400;

    private int stockPlaceId;

    private TextView tvStockPlaceName = null;
    private AutoCompleteTextView etNewName = null;
    private Button btnRename = null;

    private TextView tvPackBoxCount = null;
    private ListView lvPackBoxList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_move_stock_place_detail);

        Intent intent = getIntent();
        stockPlaceId = intent.getIntExtra("stockPlaceId", 0);

        tvStockPlaceName = findViewById(R.id.tvStockPlaceName);
        etNewName = findViewById(R.id.etNewName);
        btnRename = findViewById(R.id.btnRename);
        btnRename.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                String newName = etNewName.getText().toString();

                if ("".equals(newName)) {
                    Toast.makeText(StockMoveStockPlaceDetailActivity.this, "请输入托盘新的名称！", Toast.LENGTH_SHORT).show();
                    return;
                }

                StockMoveRequest stockCheckRequest = RetrofitUtil.createRequest(StockMoveRequest.class);
                Call<ResponseInfo> stockCheckCall = stockCheckRequest.stockPlaceRename(stockPlaceId, newName);
                Dialog mask = MaskDialog.show(StockMoveStockPlaceDetailActivity.this);
                stockCheckCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, StockMoveStockPlaceDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(StockMoveStockPlaceDetailActivity.this);

                        Toast.makeText(StockMoveStockPlaceDetailActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        loadDetailData();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(StockMoveStockPlaceDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        });

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

                StockMoveRequest stockPlaceRequest = RetrofitUtil.createRequest(StockMoveRequest.class);
                Call<StockMoveStockPlaceDetailResponseInfo> stockPlaceDetailCall = stockPlaceRequest.stockPlaceDetail(stockPlaceId);
                // Dialog mask = MaskDialog.show(StockMoveStockPlaceDetailActivity.this);
                stockPlaceDetailCall.enqueue(new Callback<StockMoveStockPlaceDetailResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockMoveStockPlaceDetailResponseInfo> call, Response<StockMoveStockPlaceDetailResponseInfo> response) {
                        // mask.dismiss();

                        StockMoveStockPlaceDetailResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, StockMoveStockPlaceDetailActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_STOCK_PLACE_DETAIL_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockMoveStockPlaceDetailResponseInfo> call, Throwable t) {
                        Toast.makeText(StockMoveStockPlaceDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });


            }
        }).start();
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "盘点扫描二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(StockMoveStockPlaceDetailActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "盘点扫描二维码");
            startActivityForResult(intent, requestCode);
        }
    }

    /*


    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String packBoxCode = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_PACK_BOX) {

                StockMoveRequest stockCheckRequest = RetrofitUtil.createRequest(StockMoveRequest.class);
                Call<ResponseInfo> stockCheckCall = stockCheckRequest.stockCheck(stockPlaceId, packBoxCode);
                Dialog mask = MaskDialog.show(StockMoveStockPlaceDetailActivity.this);
                stockCheckCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, StockMoveStockPlaceDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(StockMoveStockPlaceDetailActivity.this);

                        Toast.makeText(StockMoveStockPlaceDetailActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        if (swAutoContinueScan.isChecked()) {
                            ,startQrCode(REQ_QR_CODE_PACK_BOX);
                        }

                        loadDetailData();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(StockMoveStockPlaceDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        }
    }


     */

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
                StockMoveStockPlaceDetailResponseInfo detailDataResponse = (StockMoveStockPlaceDetailResponseInfo) msg.obj;

                tvStockPlaceName.setText(detailDataResponse.getName());

                tvPackBoxCount.setText("共 " + detailDataResponse.getPackBoxItemCount() + " 件");

                List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                for (StockMovePackBoxItemInfo item : detailDataResponse.getPackBoxItemList()) {
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

                    if (item.getStockCheckTime() == null) {
                        dataMap.put("stockCheckTime", "盘点时间：无");
                    } else {
                        dataMap.put("stockCheckTime", "盘点时间：" + item.getStockCheckTime());
                    }

                    listDataMap.add(dataMap);
                }

                if (detailDataResponse.getSuccess()) {
                    SimpleAdapter adapter = new SimpleAdapter(StockMoveStockPlaceDetailActivity.this
                            , listDataMap
                            , R.layout.item_stock_check_pack_box_list
                            , new String[]{"boxCode", "orderCode", "customerCode", "shipContainer", "unloadTime", "stockCheckTime"}
                            , new int[]{R.id.tvBoxCode, R.id.tvOrderCode, R.id.tvCustomerCode, R.id.tvShipContainer, R.id.tvUnloadTime, R.id.tvStockCheckTime});

                    lvPackBoxList.setAdapter(adapter);
                } else {
                    Toast.makeText(StockMoveStockPlaceDetailActivity.this, detailDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
                }
            }
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();
        }

        return true;
    }
}
