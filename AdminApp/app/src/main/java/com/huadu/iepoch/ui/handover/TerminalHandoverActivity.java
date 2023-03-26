package com.huadu.iepoch.ui.handover;

import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.LoginCache;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.PdaUtil;
import com.huadu.iepoch.core.PermissionUtil;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.terminalhandover.TerminalHandoverNotHandoverListItemInfo;
import com.huadu.iepoch.data.http.terminalhandover.TerminalHandoverNotHandoverListResponseInfo;
import com.huadu.iepoch.http.TerminalHandoverRequest;
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

public class TerminalHandoverActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX = 1;

    private static final int REFRESH_NOT_HANDOVER_DATA_LIST = 100100;

    private int pickupStationId;
    private String pickupStationName;

    private int shipContainerInnerPickupStationId;

    private int shipContainerInnerId;
    private String shipContainerInnerCode;

    private TextView tvPickupStationName = null;
    private TextView tvShipContainerInnerCode = null;

    private Button btnScan = null;
    private Switch swAutoContinueScan;
    private ListView lvNotHandoverList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_terminal_handover);

        Intent intent = getIntent();

        shipContainerInnerPickupStationId = intent.getIntExtra("shipContainerInnerPickupStationId", 0);

        pickupStationId = intent.getIntExtra("pickupStationId", 0);
        pickupStationName = intent.getStringExtra("pickupStationName");

        shipContainerInnerId = intent.getIntExtra("shipContainerInnerId", 0);
        shipContainerInnerCode = intent.getStringExtra("shipContainerInnerCode");

        tvPickupStationName = findViewById(R.id.tvPickupStationName);
        tvShipContainerInnerCode = findViewById(R.id.tvShipContainerInnerCode);
        btnScan = findViewById(R.id.btnScan);
        lvNotHandoverList = findViewById(R.id.lvNotHandoverList);

        tvPickupStationName.setText("取货点：" + pickupStationName);
        tvShipContainerInnerCode.setText("船航批次：" + shipContainerInnerCode);

        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PACK_BOX);
            }
        });

        swAutoContinueScan = findViewById(R.id.swAutoContinueScan);
    }

    @Override
    protected void onStart() {
        super.onStart();

        loadNotHandoveList();
    }

    private void loadNotHandoveList() {
        TerminalHandoverRequest terminalHandoverRequest = RetrofitUtil.createRequest(TerminalHandoverRequest.class);
        Call<TerminalHandoverNotHandoverListResponseInfo> notHandoverListCall = terminalHandoverRequest.notHandoverList(shipContainerInnerId, pickupStationId);
        // Dialog mask = MaskDialog.show(TerminalHandoverActivity.this);
        notHandoverListCall.enqueue(new Callback<TerminalHandoverNotHandoverListResponseInfo>() {
            @Override
            public void onResponse(Call<TerminalHandoverNotHandoverListResponseInfo> call, Response<TerminalHandoverNotHandoverListResponseInfo> response) {
                // mask.dismiss();

                ResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, TerminalHandoverActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_NOT_HANDOVER_DATA_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<TerminalHandoverNotHandoverListResponseInfo> call, Throwable t) {
                Toast.makeText(TerminalHandoverActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_NOT_HANDOVER_DATA_LIST:

                    TerminalHandoverNotHandoverListResponseInfo listDataResponse = (TerminalHandoverNotHandoverListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (TerminalHandoverNotHandoverListItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("customerCode", "客户ID：" + item.getCustomerCode());
                        dataMap.put("orderCode", "订单编号：" + item.getOrderCode());
                        dataMap.put("notHandoverPackBoxCount", "未交接箱数：" + item.getNotHandoverPackBoxCount());
                        dataMap.put("notHandoverPackBoxes", "未交接箱序：" + item.getNotHandoverPackBoxes());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(TerminalHandoverActivity.this
                                , listDataMap
                                , R.layout.item_terminal_handover_not_handover_list
                                , new String[]{"customerCode", "orderCode", "notHandoverPackBoxCount", "notHandoverPackBoxes"}
                                , new int[]{R.id.tvCustomerCode, R.id.tvOrderCode, R.id.tvNotHandoverPackBoxCount, R.id.tvNotHandoverPackBoxes});

                        lvNotHandoverList.setAdapter(adapter);
                    } else {
                        Toast.makeText(TerminalHandoverActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                    break;
                default:
                    break;
            }
        }
    }

    private void startQrCode(int requestCode) {

        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "交接扫描箱号二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(TerminalHandoverActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "交接扫描箱号二维码");
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

                TerminalHandoverRequest terminalHandoverRequest = RetrofitUtil.createRequest(TerminalHandoverRequest.class);
                Call<ResponseInfo> handoverCall = terminalHandoverRequest.handover(pickupStationId, packBoxCode, LoginCache.getUserId());
                Dialog mask = MaskDialog.show(TerminalHandoverActivity.this);
                handoverCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, TerminalHandoverActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(TerminalHandoverActivity.this);
                        Toast.makeText(TerminalHandoverActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        if (swAutoContinueScan.isChecked()) {
                            startQrCode(REQ_QR_CODE_PACK_BOX);
                        }

                        loadNotHandoveList();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(TerminalHandoverActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_terminal_handover, menu);

        // 菜单 完成移交
        MenuItem menuItemUnloadComplete = menu.findItem(R.id.menu_item_handover_complete);

        menuItemUnloadComplete.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(TerminalHandoverActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("确认要完成移交吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                        TerminalHandoverRequest terminalHandoverRequest = RetrofitUtil.createRequest(TerminalHandoverRequest.class);
                        Call<ResponseInfo> handoverCompleteCall = terminalHandoverRequest.complete(shipContainerInnerPickupStationId);
                        Dialog mask = MaskDialog.show(TerminalHandoverActivity.this);
                        handoverCompleteCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (respData == null) {
                                    SoundUtil.playError(TerminalHandoverActivity.this);

                                    // Toast.makeText(TerminalHandoverActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                    AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(TerminalHandoverActivity.this);
                                    alertDialogBuilder.setTitle("错误提示");
                                    alertDialogBuilder.setMessage("未能接收到服务器返回数据！");
                                    alertDialogBuilder.setPositiveButton("确定", null);

                                    AlertDialog alertDialog = alertDialogBuilder.create();
                                    alertDialog.show();

                                    return;
                                }

                                SoundUtil.playSuccess(TerminalHandoverActivity.this);

                                Toast.makeText(TerminalHandoverActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                                return;
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(TerminalHandoverActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                });

                alertDialogBuilder.setNegativeButton("取消", null);

                AlertDialog alertDialog = alertDialogBuilder.create();
                alertDialog.show();


                return true;
            }
        });

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            /*
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
             */

            finish();
        }

        return true;
    }
}
