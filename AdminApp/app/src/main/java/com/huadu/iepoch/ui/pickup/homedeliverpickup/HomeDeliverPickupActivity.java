package com.huadu.iepoch.ui.pickup.homedeliverpickup;

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
import com.huadu.iepoch.data.http.homedeliverpickup.HomeDeliverPickupResponseInfo;
import com.huadu.iepoch.http.HomeDeliverPickupRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.pda.ScanActivity;
import com.huadu.iepoch.ui.zxing.QrScanConstants;
import com.huadu.iepoch.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomeDeliverPickupActivity extends BaseActivity {

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
        setContentView(R.layout.activity_home_deliver_pickup);

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
            if (PermissionUtil.checkCameraStoragePermissions(HomeDeliverPickupActivity.this, requestCode) == false) {
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

                HomeDeliverPickupRequest homeDeliverPickupRequest = RetrofitUtil.createRequest(HomeDeliverPickupRequest.class);
                Call<HomeDeliverPickupResponseInfo> pickupCall = homeDeliverPickupRequest.pickup(warehouseId, pickupCode);
                Dialog mask = MaskDialog.show(HomeDeliverPickupActivity.this);
                pickupCall.enqueue(new Callback<HomeDeliverPickupResponseInfo>() {
                    @Override
                    public void onResponse(Call<HomeDeliverPickupResponseInfo> call, Response<HomeDeliverPickupResponseInfo> response) {
                        mask.dismiss();

                        HomeDeliverPickupResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, HomeDeliverPickupActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(HomeDeliverPickupActivity.this);

                        Intent intent = new Intent(HomeDeliverPickupActivity.this, HomeDeliverPickupConfirmActivity.class);
                        HomeDeliverPickupConfirmActivity.homeDeliverPickupResponse = respData;
                        intent.putExtra("warehouseId", warehouseId);
                        intent.putExtra("pickupCode", pickupCode);
                        intent.putExtra("name", name); // 这个貌似没用
                        startActivity(intent);
                    }

                    @Override
                    public void onFailure(Call<HomeDeliverPickupResponseInfo> call, Throwable t) {
                        Toast.makeText(HomeDeliverPickupActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
