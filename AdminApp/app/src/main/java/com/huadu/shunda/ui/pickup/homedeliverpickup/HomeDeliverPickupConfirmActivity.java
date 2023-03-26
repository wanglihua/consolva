package com.huadu.shunda.ui.pickup.homedeliverpickup;

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
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.homedeliverpickup.HomeDeliverPickupResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxDetailDataResponseInfo;
import com.huadu.shunda.http.HomeDeliverPickupRequest;
import com.huadu.shunda.http.PackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomeDeliverPickupConfirmActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX_CODE = 1;

    public static HomeDeliverPickupResponseInfo homeDeliverPickupResponse = null;

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
        setContentView(R.layout.activity_home_deliver_pickup_confirm);

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

        if (homeDeliverPickupResponse != null) {

            if (homeDeliverPickupResponse.getCanPickup()) {
                btnPickupConfirm.setVisibility(View.VISIBLE);
                btnPackBoxScan.setVisibility(View.VISIBLE);
            } else {
                btnPickupConfirm.setVisibility(View.GONE);
                btnPackBoxScan.setVisibility(View.GONE);
            }

            tvCustomerCode.setText("客户ID：" + homeDeliverPickupResponse.getCustomerCode());
            tvOrderCode.setText("订单编号：" + homeDeliverPickupResponse.getOrderCode());
            tvArrearsAmount.setText("欠费金额：" + homeDeliverPickupResponse.getArrearsAmount());
            tvTotalPackBoxCount.setText("总箱数：" + homeDeliverPickupResponse.getTotalPackBoxCount());
            tvShipContainerCustomerCode.setText("船航批次：" + homeDeliverPickupResponse.getShipContainerCustomerCode());
            tvOrderStatus.setText("订单状态：" + homeDeliverPickupResponse.getOrderStatus());
            tvDeliverPackBoxCount.setText("已交付箱数：" + homeDeliverPickupResponse.getDeliverPackBoxCount());
            tvNotUnloadPackBoxCount.setText("漏卸箱数：" + homeDeliverPickupResponse.getNotUnloadPackBoxCount());
            tvNotUnloadPackBoxNums.setText("漏卸箱序：" + homeDeliverPickupResponse.getNotUnloadPackBoxNums());
            tvDescription.setText("备注信息：" + homeDeliverPickupResponse.getDescription());
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

        HomeDeliverPickupRequest homeDeliverPickupRequest = RetrofitUtil.createRequest(HomeDeliverPickupRequest.class);
        Call<HomeDeliverPickupResponseInfo> pickupCall = homeDeliverPickupRequest.pickup(warehouseId, pickupCode);
        // Dialog mask = MaskDialog.show(HomeDeliverPickupConfirmActivity.this);
        pickupCall.enqueue(new Callback<HomeDeliverPickupResponseInfo>() {
            @Override
            public void onResponse(Call<HomeDeliverPickupResponseInfo> call, Response<HomeDeliverPickupResponseInfo> response) {
                // mask.dismiss();

                HomeDeliverPickupResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, HomeDeliverPickupConfirmActivity.this)) {
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
            public void onFailure(Call<HomeDeliverPickupResponseInfo> call, Throwable t) {
                Toast.makeText(HomeDeliverPickupConfirmActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描箱号");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(HomeDeliverPickupConfirmActivity.this, requestCode) == false) {
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
                Dialog mask = MaskDialog.show(HomeDeliverPickupConfirmActivity.this);
                orderPayDetailCall.enqueue(new Callback<PackBoxDetailDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PackBoxDetailDataResponseInfo> call, Response<PackBoxDetailDataResponseInfo> response) {
                        mask.dismiss();

                        PackBoxDetailDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, HomeDeliverPickupConfirmActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(HomeDeliverPickupConfirmActivity.this);

                        Intent intent = new Intent(HomeDeliverPickupConfirmActivity.this, HomeDeliverPickupPackBoxActivity.class);
                        intent.putExtra("warehouseId", warehouseId);
                        intent.putExtra("pickupCode", pickupCode);
                        HomeDeliverPickupPackBoxActivity.packBoxDetailDataResponse = respData;

                        startActivity(intent);
                    }

                    @Override
                    public void onFailure(Call<PackBoxDetailDataResponseInfo> call, Throwable t) {
                        Toast.makeText(HomeDeliverPickupConfirmActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        }
    }

    private class ButtonPickupConfirmClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverPickupConfirmActivity.this);
            alertDialogBuilder.setTitle("确认");
            alertDialogBuilder.setMessage("确实要确认订单交付吗？");
            alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int i) {

                    String clientTime = ClientTimeUtil.getClientTime();

                    HomeDeliverPickupRequest homeDeliverPickupRequest = RetrofitUtil.createRequest(HomeDeliverPickupRequest.class);
                    Call<ResponseInfo> pickupConfirmCall = homeDeliverPickupRequest.pickupOrderConfirm(homeDeliverPickupResponse.getWarehouseId(), homeDeliverPickupResponse.getPickupCode(), clientTime);
                    Dialog mask = MaskDialog.show(HomeDeliverPickupConfirmActivity.this);
                    pickupConfirmCall.enqueue(new Callback<ResponseInfo>() {
                        @Override
                        public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                            mask.dismiss();

                            ResponseInfo respData = response.body();

                            if (!RetrofitUtil.checkResponse(respData, HomeDeliverPickupConfirmActivity.this)) {
                                return;
                            }

                            SoundUtil.playSuccess(HomeDeliverPickupConfirmActivity.this);

                            Toast.makeText(HomeDeliverPickupConfirmActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();
                            finish();
                        }

                        @Override
                        public void onFailure(Call<ResponseInfo> call, Throwable t) {
                            Toast.makeText(HomeDeliverPickupConfirmActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
