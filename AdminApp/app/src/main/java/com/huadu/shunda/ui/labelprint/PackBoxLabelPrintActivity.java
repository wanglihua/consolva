package com.huadu.shunda.ui.labelprint;

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
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxLabelPrintDetailDataResponseInfo;
import com.huadu.shunda.http.PackBoxLabelPrintRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PackBoxLabelPrintActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX = 101;

    private TextView tvPackBoxCode = null;
    private TextView tvPackBoxShortCode = null;
    private TextView tvOrderCode = null;
    private TextView tvCustomerCode = null;
    private TextView tvTargetWarehouseName = null;

    private Button btnScan = null;
    private Button btnPrint = null;
    private Switch swAutoPrint = null;

    /**
     * 这个就使用短箱号了
     */
    private String orderPackBoxCode = "";


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pack_box_label_print);

        tvPackBoxCode = findViewById(R.id.tvPackBoxCode);
        tvPackBoxCode.setText("箱号：");

        tvPackBoxShortCode = findViewById(R.id.tvPackBoxShortCode);
        tvPackBoxShortCode.setText("短箱号：");

        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvOrderCode.setText("订单编号：");

        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvCustomerCode.setText("客户ID：");

        tvTargetWarehouseName = findViewById(R.id.tvTargetWarehouseName);
        tvTargetWarehouseName.setText("目的仓：");

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                orderPackBoxCode = "";
                startQrCode(REQ_QR_CODE_PACK_BOX);
            }
        });

        btnPrint = findViewById(R.id.btnPrint);
        btnPrint.setVisibility(View.GONE);
        btnPrint.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                sendPrint();
            }
        });

        swAutoPrint = findViewById(R.id.swAutoPrint);
    }

    @Override
    protected void onStart() {
        super.onStart();
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "标签打印扫描箱号");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(PackBoxLabelPrintActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "标签打印扫描箱号");
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

                PackBoxLabelPrintRequest packBoxLabelPrintRequest = RetrofitUtil.createRequest(PackBoxLabelPrintRequest.class);
                Call<PackBoxLabelPrintDetailDataResponseInfo> loadCall = packBoxLabelPrintRequest.packBoxDetail(packBoxCode);
                Dialog mask = MaskDialog.show(PackBoxLabelPrintActivity.this);
                loadCall.enqueue(new Callback<PackBoxLabelPrintDetailDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PackBoxLabelPrintDetailDataResponseInfo> call, Response<PackBoxLabelPrintDetailDataResponseInfo> response) {
                        mask.dismiss();

                        PackBoxLabelPrintDetailDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, PackBoxLabelPrintActivity.this)) {
                            return;
                        }

                        orderPackBoxCode = respData.getPackBoxShortCode();

                        tvPackBoxCode.setText("箱号：" + respData.getPackBoxCode());
                        tvPackBoxShortCode.setText("短箱号：" + respData.getPackBoxShortCode());
                        tvOrderCode.setText("订单编号：" + respData.getOrderCode());
                        tvCustomerCode.setText("客户ID：" + respData.getCustomerCode());
                        tvTargetWarehouseName.setText("目的仓：" + respData.getTargetWarehouseName());

                        SoundUtil.playSuccess(PackBoxLabelPrintActivity.this);

                        Toast.makeText(PackBoxLabelPrintActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        if (swAutoPrint.isChecked()) {
                            sendPrint();
                        } else {
                            btnPrint.setVisibility(View.VISIBLE);
                        }
                    }

                    @Override
                    public void onFailure(Call<PackBoxLabelPrintDetailDataResponseInfo> call, Throwable t) {
                        Toast.makeText(PackBoxLabelPrintActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        }
    }

    private void sendPrint() {
        PackBoxLabelPrintRequest packBoxLabelPrintRequest = RetrofitUtil.createRequest(PackBoxLabelPrintRequest.class);
        Call<ResponseInfo> printCall = packBoxLabelPrintRequest.print(orderPackBoxCode);
        Dialog mask = MaskDialog.show(PackBoxLabelPrintActivity.this);
        printCall.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                mask.dismiss();

                ResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, PackBoxLabelPrintActivity.this)) {
                    return;
                }

                SoundUtil.playSuccess(PackBoxLabelPrintActivity.this);
                Toast.makeText(PackBoxLabelPrintActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                btnPrint.setVisibility(View.GONE);
            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(PackBoxLabelPrintActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
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
            Intent intent = new Intent(this, PackBoxLoadShipContainerInnerListActivity.class);
            startActivity(intent);
             */

            finish();
        }

        return true;
    }
}
