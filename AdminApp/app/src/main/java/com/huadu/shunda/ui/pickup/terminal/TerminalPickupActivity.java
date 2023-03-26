package com.huadu.shunda.ui.pickup.terminal;

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
import com.huadu.shunda.data.http.terminalpickup.TerminalPickupResponseInfo;
import com.huadu.shunda.http.TerminalPickupRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TerminalPickupActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PICKUP_CODE = 1;

    private int pickupStationId;
    private String pickupStationName;

    private TextView tvPickupStationName = null;
    private Button btnScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_terminal_pickup);

        Intent intent = getIntent();
        pickupStationId = intent.getIntExtra("pickupStationId", 0);
        pickupStationName = intent.getStringExtra("pickupStationName");

        tvPickupStationName = findViewById(R.id.tvPickupStationName);
        btnScan = findViewById(R.id.btnScan);

        tvPickupStationName.setText(pickupStationName);

        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PICKUP_CODE);
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
            intent.putExtra("labelText", "扫描取货二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(TerminalPickupActivity.this, requestCode) == false) {
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

                TerminalPickupRequest terminalPickupRequest = RetrofitUtil.createRequest(TerminalPickupRequest.class);
                Call<TerminalPickupResponseInfo> pickupCall = terminalPickupRequest.pickup(pickupStationId, pickupCode);
                Dialog mask = MaskDialog.show(TerminalPickupActivity.this);
                pickupCall.enqueue(new Callback<TerminalPickupResponseInfo>() {
                    @Override
                    public void onResponse(Call<TerminalPickupResponseInfo> call, Response<TerminalPickupResponseInfo> response) {
                        mask.dismiss();

                        TerminalPickupResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, TerminalPickupActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(TerminalPickupActivity.this);

                        Intent intent = new Intent(TerminalPickupActivity.this, TerminalPickupConfirmActivity.class);
                        TerminalPickupConfirmActivity.terminalPickupResponse = respData;
                        intent.putExtra("pickupStationId", pickupStationId);
                        intent.putExtra("pickupCode", pickupCode);
                        intent.putExtra("name", pickupStationName);

                        startActivity(intent);
                    }

                    @Override
                    public void onFailure(Call<TerminalPickupResponseInfo> call, Throwable t) {
                        Toast.makeText(TerminalPickupActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
            /*
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
             */

            finish();
        }

        return true;
    }
}
