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
import androidx.appcompat.app.AlertDialog;

import com.huadu.shunda.R;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.packbox.PackBoxShipResponseData;
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
    private TextView tvCountryName = null;
    private TextView tvFirstModeName = null;
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
        tvCountryName = findViewById(R.id.tvCountryName);
        tvFirstModeName = findViewById(R.id.tvFirstModeName);
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
                Call<ResponseDataInfo<PackBoxShipResponseData>> orderPayDetailCall = packBoxRequest.ship(packBoxCode);
                Dialog mask = MaskDialog.show(PackBoxShipActivity.this);
                orderPayDetailCall.enqueue(new Callback<ResponseDataInfo<PackBoxShipResponseData>>() {
                    @Override
                    public void onResponse(Call<ResponseDataInfo<PackBoxShipResponseData>> call, Response<ResponseDataInfo<PackBoxShipResponseData>> response) {
                        mask.dismiss();

                        if (!RetrofitUtil.processResponse(response, PackBoxShipActivity.this)) {
                            return;
                        }

                        PackBoxShipResponseData respData = response.body().getData();

                        tvPackBoxCode.setText("箱号：" + respData.getPackBoxCode());
                        tvOrderCode.setText("订单编号：" + respData.getOrderCode());
                        tvCustomerCode.setText("客户ID：" + respData.getCustomerCode());
                        tvCountryName.setText("国家：" + respData.getCountryName());
                        tvFirstModeName.setText("物流：" + respData.getFirstModeName());
                        tvArrearsAmount.setText("欠费金额：" + respData.getArrearsAmount());

                        if (respData.getHasArrears()) {
                            SoundUtil.playError(PackBoxShipActivity.this);
                            tvArrearsAmount.setTextColor(getResources().getColor(R.color.colorRed));

                            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(PackBoxShipActivity.this);
                            alertDialogBuilder.setTitle("错误提示");
                            alertDialogBuilder.setMessage(response.body().getMessage());
                            alertDialogBuilder.setPositiveButton("确定", null);

                            AlertDialog alertDialog = alertDialogBuilder.create();
                            alertDialog.show();
                        } else {
                            SoundUtil.playSuccess(PackBoxShipActivity.this);
                            tvArrearsAmount.setTextColor(getResources().getColor(R.color.colorGreen));

                            Toast.makeText(PackBoxShipActivity.this, response.body().getMessage(), Toast.LENGTH_SHORT).show();

                            if (swAutoContinueScan.isChecked()) {
                                startQrCode(REQ_QR_CODE_PACK_BOX_CODE);
                            }
                        }
                    }

                    @Override
                    public void onFailure(Call<ResponseDataInfo<PackBoxShipResponseData>> call, Throwable t) {
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
