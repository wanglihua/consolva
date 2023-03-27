package com.huadu.shunda.ui.query;

import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.huadu.shunda.R;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.packbox.PackBoxStockPlaceDataResponseInfo;
import com.huadu.shunda.http.PackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PackBoxStockPlaceActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX_CODE = 1;

    private TextView tvOrderPackBoxCode = null;
    private TextView tvOrderPackBoxShortCode = null;
    private TextView tvCustomerCode = null;
    private TextView tvOrderCode = null;
    private TextView tvArrearsAmount = null;
    private TextView tvTotalPackBoxCount = null;
    private TextView tvShipContainerCustomerCode = null;
    private TextView tvOrderStatus = null;
    private TextView tvDeliverPackBoxCount = null;
    private TextView tvNotUnloadPackBoxCount = null;
    private TextView tvNotUnloadPackBoxNums = null;
    private TextView tvStockPlaceList = null;
    private Button btnPackBoxScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pack_box_stock_place);

        tvOrderPackBoxCode = findViewById(R.id.tvOrderPackBoxCode);
        tvOrderPackBoxShortCode = findViewById(R.id.tvOrderPackBoxShortCode);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvArrearsAmount = findViewById(R.id.tvArrearsAmount);
        tvTotalPackBoxCount = findViewById(R.id.tvTotalPackBoxCount);
        tvShipContainerCustomerCode = findViewById(R.id.tvShipContainerCustomerCode);
        tvOrderStatus = findViewById(R.id.tvOrderStatus);
        tvDeliverPackBoxCount = findViewById(R.id.tvDeliverPackBoxCount);
        tvNotUnloadPackBoxCount = findViewById(R.id.tvNotUnloadPackBoxCount);
        tvNotUnloadPackBoxNums = findViewById(R.id.tvNotUnloadPackBoxNums);
        tvStockPlaceList = findViewById(R.id.tvStockPlaceList);

        btnPackBoxScan = findViewById(R.id.btnPackBoxScan);

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
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描箱号");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(PackBoxStockPlaceActivity.this, requestCode) == false) {
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
                Call<PackBoxStockPlaceDataResponseInfo> orderPayDetailCall = packBoxRequest.stockPlaceData(packBoxCode);
                Dialog mask = MaskDialog.show(PackBoxStockPlaceActivity.this);
                orderPayDetailCall.enqueue(new Callback<PackBoxStockPlaceDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PackBoxStockPlaceDataResponseInfo> call, Response<PackBoxStockPlaceDataResponseInfo> response) {
                        mask.dismiss();

                        PackBoxStockPlaceDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, PackBoxStockPlaceActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(PackBoxStockPlaceActivity.this);

                        tvOrderPackBoxCode.setText("箱号：" + respData.getOrderPackBoxCode());
                        tvOrderPackBoxShortCode.setText("短箱号：" + respData.getOrderPackBoxShortCode());
                        tvCustomerCode.setText("客户ID：" + respData.getCustomerCode());
                        tvOrderCode.setText("订单编号：" + respData.getOrderCode());
                        tvArrearsAmount.setText("欠费金额：" + respData.getArrearsAmount());
                        tvTotalPackBoxCount.setText("总箱数：" + respData.getTotalPackBoxCount());
                        tvShipContainerCustomerCode.setText("船航批次：" + respData.getShipContainerCustomerCode());
                        tvOrderStatus.setText("订单状态：" + respData.getOrderStatus());
                        tvDeliverPackBoxCount.setText("已交付箱数：" + respData.getDeliverPackBoxCount());
                        tvNotUnloadPackBoxCount.setText("漏卸箱数：" + respData.getNotUnloadPackBoxCount());
                        tvNotUnloadPackBoxNums.setText("漏卸箱序：" + respData.getNotUnloadPackBoxNums());
                        tvStockPlaceList.setText("托盘信息：" + respData.getStockPlaceList());
                    }

                    @Override
                    public void onFailure(Call<PackBoxStockPlaceDataResponseInfo> call, Throwable t) {
                        Toast.makeText(PackBoxStockPlaceActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();
        }

        return true;
    }
}
