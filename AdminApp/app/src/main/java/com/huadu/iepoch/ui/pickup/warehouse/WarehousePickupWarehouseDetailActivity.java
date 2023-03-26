package com.huadu.iepoch.ui.pickup.warehouse;

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

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.PdaUtil;
import com.huadu.iepoch.core.PermissionUtil;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.warehousepickup.WarehousePickupResponseInfo;
import com.huadu.iepoch.http.WarehousePickupRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.pda.ScanActivity;
import com.huadu.iepoch.ui.zxing.QrScanConstants;
import com.huadu.iepoch.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class WarehousePickupWarehouseDetailActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PICKUP_CODE = 1;

    private int warehouseId;
    private String code;
    private String name;

    private TextView tvCode = null;
    private TextView tvName = null;

    private Button btnScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_warehouse_pickup_warehouse_detail);

        Intent intent = getIntent();

        warehouseId = intent.getIntExtra("warehouseId", 0);
        code = intent.getStringExtra("code");
        name = intent.getStringExtra("name");

        tvCode = findViewById(R.id.tvCode);
        tvCode.setText(code);

        tvName = findViewById(R.id.tvName);
        tvName.setText(name);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PICKUP_CODE);
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描取货二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(WarehousePickupWarehouseDetailActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "扫描取货二维码");
            startActivityForResult(intent, requestCode);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String pickupCode = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_PICKUP_CODE) {

                WarehousePickupRequest warehousePickupRequest = RetrofitUtil.createRequest(WarehousePickupRequest.class);
                Call<WarehousePickupResponseInfo> pickupCall = warehousePickupRequest.pickup(warehouseId, pickupCode);
                Dialog mask = MaskDialog.show(WarehousePickupWarehouseDetailActivity.this);
                pickupCall.enqueue(new Callback<WarehousePickupResponseInfo>() {
                    @Override
                    public void onResponse(Call<WarehousePickupResponseInfo> call, Response<WarehousePickupResponseInfo> response) {
                        mask.dismiss();

                        WarehousePickupResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, WarehousePickupWarehouseDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(WarehousePickupWarehouseDetailActivity.this);

                        Intent intent = new Intent(WarehousePickupWarehouseDetailActivity.this, WarehousePickupActivity.class);
                        WarehousePickupActivity.warehousePickupResponse = respData;
                        intent.putExtra("warehouseId", warehouseId);
                        intent.putExtra("pickupCode", pickupCode);
                        startActivity(intent);
                    }

                    @Override
                    public void onFailure(Call<WarehousePickupResponseInfo> call, Throwable t) {
                        Toast.makeText(WarehousePickupWarehouseDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
            Intent intent = new Intent(this, WarehousePickupWarehouseListActivity.class);
            startActivity(intent);
             */
        }

        return true;
    }
}
