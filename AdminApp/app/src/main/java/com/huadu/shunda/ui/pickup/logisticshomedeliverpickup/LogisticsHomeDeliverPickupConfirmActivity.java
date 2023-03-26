package com.huadu.shunda.ui.pickup.logisticshomedeliverpickup;

import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;

import com.huadu.shunda.R;
import com.huadu.shunda.core.ClientTimeUtil;
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.logisitcshomedeliverpickup.LogisticsHomeDeliverPickupResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxDetailDataResponseInfo;
import com.huadu.shunda.http.LogisticsHomeDeliverPickupRequest;
import com.huadu.shunda.http.PackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LogisticsHomeDeliverPickupConfirmActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX_CODE = 1;

    public static LogisticsHomeDeliverPickupResponseInfo logisticsHomeDeliverPickupResponse = null;

    private int warehouseId;
    private String pickupCode;
    private String name;

    private TextView tvCustomerCode = null;
    private TextView tvOrderCode = null;
    private TextView tvArrearsAmount = null;
    private TextView tvTotalPackBoxCount = null;
    private TextView tvShipContainerCustomerCode = null;
    private TextView tvOrderStatus = null;
    private TextView tvDeliverPackBoxCount = null;
    private TextView tvNotUnloadPackBoxCount = null;
    private TextView tvNotUnloadPackBoxNums = null;
    private TextView tvDescription = null;
    private Button btnPickupConfirm = null;
    private Button btnPackBoxScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_logistics_home_deliver_confirm);

        Intent intent = getIntent();

        warehouseId = intent.getIntExtra("warehouseId", 0);
        pickupCode = intent.getStringExtra("pickupCode");
        name = intent.getStringExtra("name");

        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvArrearsAmount = findViewById(R.id.tvArrearsAmount);
        tvTotalPackBoxCount = findViewById(R.id.tvTotalPackBoxCount);
        tvShipContainerCustomerCode = findViewById(R.id.tvShipContainerCustomerCode);
        tvOrderStatus = findViewById(R.id.tvOrderStatus);
        tvDeliverPackBoxCount = findViewById(R.id.tvDeliverPackBoxCount);
        tvNotUnloadPackBoxCount = findViewById(R.id.tvNotUnloadPackBoxCount);
        tvNotUnloadPackBoxNums = findViewById(R.id.tvNotUnloadPackBoxNums);
        tvDescription = findViewById(R.id.tvDescription);

        btnPickupConfirm = findViewById(R.id.btnPickupConfirm);
        btnPackBoxScan = findViewById(R.id.btnPackBoxScan);

        btnPickupConfirm.setOnClickListener(new ButtonPickupConfirmClickListener());

        if (logisticsHomeDeliverPickupResponse != null) {

            if (logisticsHomeDeliverPickupResponse.getCanPickup()) {
                btnPickupConfirm.setVisibility(View.VISIBLE);
                btnPackBoxScan.setVisibility(View.VISIBLE);
            } else {
                btnPickupConfirm.setVisibility(View.GONE);
                btnPackBoxScan.setVisibility(View.GONE);
            }

            tvCustomerCode.setText("客户ID：" + logisticsHomeDeliverPickupResponse.getCustomerCode());
            tvOrderCode.setText("订单编号：" + logisticsHomeDeliverPickupResponse.getOrderCode());
            tvArrearsAmount.setText("欠费金额：" + logisticsHomeDeliverPickupResponse.getArrearsAmount());
            tvTotalPackBoxCount.setText("总箱数：" + logisticsHomeDeliverPickupResponse.getTotalPackBoxCount());
            tvShipContainerCustomerCode.setText("船航批次：" + logisticsHomeDeliverPickupResponse.getShipContainerCustomerCode());
            tvOrderStatus.setText("订单状态：" + logisticsHomeDeliverPickupResponse.getOrderStatus());
            tvDeliverPackBoxCount.setText("已交付箱数：" + logisticsHomeDeliverPickupResponse.getDeliverPackBoxCount());
            tvNotUnloadPackBoxCount.setText("漏卸箱数：" + logisticsHomeDeliverPickupResponse.getNotUnloadPackBoxCount());
            tvNotUnloadPackBoxNums.setText("漏卸箱序：" + logisticsHomeDeliverPickupResponse.getNotUnloadPackBoxNums());
            tvDescription.setText("备注信息：" + logisticsHomeDeliverPickupResponse.getDescription());
        }

        btnPackBoxScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PACK_BOX_CODE);
            }
        });
    }

    @Override
    protected void onStart() {
        super.onStart();

        LogisticsHomeDeliverPickupRequest logisticsHomeDeliverPickupRequest = RetrofitUtil.createRequest(LogisticsHomeDeliverPickupRequest.class);
        Call<LogisticsHomeDeliverPickupResponseInfo> pickupCall = logisticsHomeDeliverPickupRequest.pickup(warehouseId, pickupCode, LoginCache.getUserId());
        // Dialog mask = MaskDialog.show(LogisticsHomeDeliverPickupConfirmActivity.this);
        pickupCall.enqueue(new Callback<LogisticsHomeDeliverPickupResponseInfo>() {
            @Override
            public void onResponse(Call<LogisticsHomeDeliverPickupResponseInfo> call, Response<LogisticsHomeDeliverPickupResponseInfo> response) {
                // mask.dismiss();

                LogisticsHomeDeliverPickupResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, LogisticsHomeDeliverPickupConfirmActivity.this)) {
                    return;
                }

                tvCustomerCode.setText("客户ID：" + respData.getCustomerCode());
                tvOrderCode.setText("订单编号：" + respData.getOrderCode());
                tvArrearsAmount.setText("欠费金额：" + respData.getArrearsAmount());
                tvTotalPackBoxCount.setText("总箱数：" + respData.getTotalPackBoxCount());
                tvShipContainerCustomerCode.setText("船航批次：" + respData.getShipContainerCustomerCode());
                tvOrderStatus.setText("订单状态：" + respData.getOrderStatus());
                tvDeliverPackBoxCount.setText("已交付箱数：" + respData.getDeliverPackBoxCount());
                tvNotUnloadPackBoxCount.setText("漏卸箱数：" + respData.getNotUnloadPackBoxCount());
                tvNotUnloadPackBoxNums.setText("漏卸箱序：" + respData.getNotUnloadPackBoxNums());
                tvDescription.setText("备注信息：" + respData.getDescription());
            }

            @Override
            public void onFailure(Call<LogisticsHomeDeliverPickupResponseInfo> call, Throwable t) {
                Toast.makeText(LogisticsHomeDeliverPickupConfirmActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描箱号");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(LogisticsHomeDeliverPickupConfirmActivity.this, requestCode) == false) {
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
            final String packBoxCode = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_PACK_BOX_CODE) {

                PackBoxRequest packBoxRequest = RetrofitUtil.createRequest(PackBoxRequest.class);
                Call<PackBoxDetailDataResponseInfo> orderPayDetailCall = packBoxRequest.detailData(packBoxCode);
                Dialog mask = MaskDialog.show(LogisticsHomeDeliverPickupConfirmActivity.this);
                orderPayDetailCall.enqueue(new Callback<PackBoxDetailDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PackBoxDetailDataResponseInfo> call, Response<PackBoxDetailDataResponseInfo> response) {
                        mask.dismiss();

                        PackBoxDetailDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, LogisticsHomeDeliverPickupConfirmActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(LogisticsHomeDeliverPickupConfirmActivity.this);

                        Intent intent = new Intent(LogisticsHomeDeliverPickupConfirmActivity.this, LogisticsHomeDeliverPickupPackBoxActivity.class);
                        intent.putExtra("warehouseId", warehouseId);
                        intent.putExtra("pickupCode", logisticsHomeDeliverPickupResponse.getPickupCode());
                        LogisticsHomeDeliverPickupPackBoxActivity.packBoxDetailDataResponse = respData;

                        startActivity(intent);
                    }

                    @Override
                    public void onFailure(Call<PackBoxDetailDataResponseInfo> call, Throwable t) {
                        Toast.makeText(LogisticsHomeDeliverPickupConfirmActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        }
    }

    private class ButtonPickupConfirmClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(LogisticsHomeDeliverPickupConfirmActivity.this);
            alertDialogBuilder.setTitle("确认");
            alertDialogBuilder.setMessage("确实要确认订单交付吗？");
            alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int i) {

                    String clientTime = ClientTimeUtil.getClientTime();

                    LogisticsHomeDeliverPickupRequest logisticsHomeDeliverPickupRequest = RetrofitUtil.createRequest(LogisticsHomeDeliverPickupRequest.class);
                    Call<ResponseInfo> pickupConfirmCall = logisticsHomeDeliverPickupRequest.pickupOrderConfirm(logisticsHomeDeliverPickupResponse.getWarehouseId(), logisticsHomeDeliverPickupResponse.getPickupCode(), clientTime);
                    Dialog mask = MaskDialog.show(LogisticsHomeDeliverPickupConfirmActivity.this);
                    pickupConfirmCall.enqueue(new Callback<ResponseInfo>() {
                        @Override
                        public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                            mask.dismiss();

                            ResponseInfo respData = response.body();

                            if (!RetrofitUtil.checkResponse(respData, LogisticsHomeDeliverPickupConfirmActivity.this)) {
                                return;
                            }

                            SoundUtil.playSuccess(LogisticsHomeDeliverPickupConfirmActivity.this);

                            Toast.makeText(LogisticsHomeDeliverPickupConfirmActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();
                            finish();
                        }

                        @Override
                        public void onFailure(Call<ResponseInfo> call, Throwable t) {
                            Toast.makeText(LogisticsHomeDeliverPickupConfirmActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                    });

                }
            });

            alertDialogBuilder.setNegativeButton("取消", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();


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
