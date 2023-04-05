package com.huadu.shunda.ui.query;

import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
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
import com.huadu.shunda.data.http.packbox.PackBoxOrderPayDetailResponseInfo;
import com.huadu.shunda.http.PackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PackBoxShipActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX_CODE = 1;

    private TextView tvPackBoxCode = null;
    private TextView tvOrderCode = null;
    private TextView tvCustomerCode = null;
    private TextView tvCustomerMobile = null;
    private TextView tvOrderMobile = null;
    private TextView tvArrearsAmount = null;

    private Button btnScan = null;

    private Switch swAutoContinueScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pack_box_ship);

        tvPackBoxCode = findViewById(R.id.tvPackBoxCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvCustomerMobile = findViewById(R.id.tvCustomerMobile);
        tvOrderMobile = findViewById(R.id.tvOrderMobile);
        tvArrearsAmount = findViewById(R.id.tvArrearsAmount);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PACK_BOX_CODE);
            }
        });

        swAutoContinueScan = findViewById(R.id.swAutoContinueScan);
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描箱号二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(PackBoxShipActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "扫描箱号二维码");
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
                Call<PackBoxOrderPayDetailResponseInfo> orderPayDetailCall = packBoxRequest.orderPayDetail(packBoxCode);
                Dialog mask = MaskDialog.show(PackBoxShipActivity.this);
                orderPayDetailCall.enqueue(new Callback<PackBoxOrderPayDetailResponseInfo>() {
                    @Override
                    public void onResponse(Call<PackBoxOrderPayDetailResponseInfo> call, Response<PackBoxOrderPayDetailResponseInfo> response) {
                        mask.dismiss();

                        PackBoxOrderPayDetailResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, PackBoxShipActivity.this)) {
                            return;
                        }

                        tvPackBoxCode.setText("箱号：" + respData.getOrderPackBoxCode());
                        tvOrderCode.setText("订单编号：" + respData.getOrderCode());
                        tvCustomerCode.setText("客户ID：" + respData.getCustomerCode());
                        tvCustomerMobile.setText("注册手机号：" + respData.getCustomerMobile());
                        tvOrderMobile.setText("订单手机号：" + respData.getOrderMobile());
                        tvArrearsAmount.setText("欠费金额：" + respData.getArrearsAmount());

                        if (respData.getHasArrearsAmount()) {
                            SoundUtil.playError(PackBoxShipActivity.this);
                            tvArrearsAmount.setTextColor(getResources().getColor(R.color.colorRed));
                        } else {
                            SoundUtil.playSuccess(PackBoxShipActivity.this);
                            tvArrearsAmount.setTextColor(getResources().getColor(R.color.colorGreen));

                            if (swAutoContinueScan.isChecked()) {
                                Toast.makeText(PackBoxShipActivity.this, "扫描订单 " + respData.getOrderCode() + " 无欠费！", Toast.LENGTH_SHORT).show();

                                startQrCode(REQ_QR_CODE_PACK_BOX_CODE);
                            }
                        }
                    }

                    @Override
                    public void onFailure(Call<PackBoxOrderPayDetailResponseInfo> call, Throwable t) {
                        Toast.makeText(PackBoxShipActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

            /*
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
            */
        }

        return true;
    }
}
