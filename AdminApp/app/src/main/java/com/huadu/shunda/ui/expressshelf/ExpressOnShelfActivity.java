package com.huadu.shunda.ui.expressshelf;

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
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOnShelfQueryResponseInfo;
import com.huadu.shunda.http.ExpressShelfRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressOnShelfActivity extends BaseActivity {

    private static final int REQ_QR_CODE_EXPRESS = 1;
    private static final int REQ_QR_CODE_STORAGE_LOC = 2;

    private TextView tvAoPinTrackingNo = null;
    private TextView tvTrackingNo = null;
    private TextView tvShelfStatus = null;
    private TextView tvPackageCount = null;
    private TextView tvStatus = null;
    private TextView tvExpressStorageLocName = null;

    private Button btnScanExpress = null;

    private TextView tvStorageLocName = null;
    private TextView tvStorageLocExpressCount = null;
    private TextView tvStorageLocPackageCount = null;

    private Button btnScanStorageLoc = null;

    private TextView tvMatchSuccess = null;
    private TextView tvMatchFail = null;

    private Button btnComplete = null;

    private Switch swAutoContinueScan = null;

    private String scanMessage = "扫描快递单号或者舜达入库号";

    private String trackingNo = "";
    private String storageLocCode = "";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_on_shelf);

        tvAoPinTrackingNo = findViewById(R.id.tvAoPinTrackingNo);
        tvTrackingNo = findViewById(R.id.tvTrackingNo);
        tvShelfStatus = findViewById(R.id.tvShelfStatus);
        tvPackageCount = findViewById(R.id.tvPackageCount);
        tvStatus = findViewById(R.id.tvStatus);
        tvExpressStorageLocName = findViewById(R.id.tvExpressStorageLocName);
        btnScanExpress = findViewById(R.id.btnScanExpress);
        tvStorageLocName = findViewById(R.id.tvStorageLocName);
        tvStorageLocExpressCount = findViewById(R.id.tvStorageLocExpressCount);
        tvStorageLocPackageCount = findViewById(R.id.tvStorageLocPackageCount);
        btnScanStorageLoc = findViewById(R.id.btnScanStorageLoc);
        tvMatchSuccess = findViewById(R.id.tvMatchSuccess);
        tvMatchFail = findViewById(R.id.tvMatchFail);
        btnComplete = findViewById(R.id.btnComplete);
        swAutoContinueScan = findViewById(R.id.swAutoContinueScan);

        btnScanExpress.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_EXPRESS);
            }
        });

        btnScanStorageLoc.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_STORAGE_LOC);
            }
        });

        btnComplete.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onShelfComplete();
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (requestCode == REQ_QR_CODE_EXPRESS) {
            this.scanMessage = "扫描快递单号或者舜达入库号";
        }
        if (requestCode == REQ_QR_CODE_STORAGE_LOC) {
            this.scanMessage = this.trackingNo + ", 扫描仓位编号";
        }

        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", scanMessage);
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(ExpressOnShelfActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", scanMessage);
            startActivityForResult(intent, requestCode);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            if (bundle == null) {
                return;
            }

            String code = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_EXPRESS) {

                // 开始扫快递单号了，把快递单号的信息和仓位信息都设置成空

                clearTrackingNoInfo();
                clearStorageLocInfo();

                onShelfQuery(code, "", true); // 这是仓位号取空的
            }

            if (requestCode == REQ_QR_CODE_STORAGE_LOC) {

                clearStorageLocInfo();

                if (this.trackingNo == null || this.trackingNo.trim().equals("")) {
                    Toast.makeText(ExpressOnShelfActivity.this, "请先扫描快递单号或舜达入库号，再扫描仓位号！", Toast.LENGTH_SHORT).show();
                    return;
                }

                onShelfQuery(this.trackingNo, code, true);
            }
        }
    }

    /**
     * 清空快递单号相关信息，包括保存的缓存和页面显示
     */
    private void clearTrackingNoInfo() {
        this.trackingNo = "";

        this.tvAoPinTrackingNo.setText("舜达入库号：请扫描舜达单号或快递单号！");
        this.tvTrackingNo.setText("快递单号：");
        this.tvShelfStatus.setText("上架状态：");
        this.tvPackageCount.setText("包裹数量：");
        this.tvStatus.setText("快递状态：");
        this.tvExpressStorageLocName.setText("登记仓位：");
    }

    /**
     * 清空仓位号相关信息，包括保存的缓存和页面显示
     */
    private void clearStorageLocInfo() {
        this.storageLocCode = "";

        this.tvStorageLocName.setText("仓位名称：请扫描仓位号标签！");
        this.tvStorageLocExpressCount.setText("快递数量：");
        this.tvStorageLocPackageCount.setText("包裹数量：");
    }

    private void showMatchSuccess() {
        SoundUtil.playSuccess(ExpressOnShelfActivity.this);

        this.tvMatchSuccess.setVisibility(View.VISIBLE);
        this.tvMatchFail.setVisibility(View.GONE);
        this.btnComplete.setVisibility(View.VISIBLE);
    }

    private void showMatchFail() {
        this.tvMatchSuccess.setVisibility(View.GONE);
        this.tvMatchFail.setVisibility(View.VISIBLE);
        this.btnComplete.setVisibility(View.GONE);

        SoundUtil.playError(ExpressOnShelfActivity.this);

        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(ExpressOnShelfActivity.this);
        alertDialogBuilder.setTitle("错误提示");
        alertDialogBuilder.setMessage("快递单号与仓位匹配失败！");
        alertDialogBuilder.setPositiveButton("确定", null);

        AlertDialog alertDialog = alertDialogBuilder.create();
        alertDialog.show();
    }

    private void hideMatchResult() {
        this.tvMatchSuccess.setVisibility(View.GONE);
        this.tvMatchFail.setVisibility(View.GONE);
        this.btnComplete.setVisibility(View.GONE);
    }

    /**
     * @param trackingNo     快递单号或者舜达入库号
     * @param storageLocCode 仓位代码
     * @param checkStatus    是否检查状态
     */
    private void onShelfQuery(String trackingNo, String storageLocCode, boolean checkStatus) {
        ExpressShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressShelfRequest.class);
        Call<ExpressShelfOnShelfQueryResponseInfo> onShelfQueryCall = expressShelfRequest.onShelfQuery(trackingNo, storageLocCode, checkStatus);
        // Dialog mask = MaskDialog.show(ExpressOnShelfActivity.this);
        onShelfQueryCall.enqueue(new Callback<ExpressShelfOnShelfQueryResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressShelfOnShelfQueryResponseInfo> call, Response<ExpressShelfOnShelfQueryResponseInfo> response) {
                // mask.dismiss();

                ExpressShelfOnShelfQueryResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, ExpressOnShelfActivity.this)) {
                    return;
                }

                // 不管是扫快递单号还是扫仓位号，两个都设置一下
                ExpressOnShelfActivity.this.trackingNo = trackingNo;
                ExpressOnShelfActivity.this.storageLocCode = storageLocCode; // 第一次扫的时候，这个值会被设置成空

                ExpressOnShelfActivity.this.tvAoPinTrackingNo.setText("舜达入库号：" + respData.getAoPinTrackingNo());
                ExpressOnShelfActivity.this.tvTrackingNo.setText("快递单号：" + respData.getTrackingNo());
                ExpressOnShelfActivity.this.tvShelfStatus.setText("上架状态：" + respData.getShelfStatus());
                ExpressOnShelfActivity.this.tvPackageCount.setText("包裹数量：" + respData.getPackageCount());
                ExpressOnShelfActivity.this.tvStatus.setText("快递状态：" + respData.getStatus());
                ExpressOnShelfActivity.this.tvExpressStorageLocName.setText("登记仓位：" + respData.getExpressStorageLocName());

                if ("".equals(storageLocCode)) {
                    SoundUtil.playSuccess(ExpressOnShelfActivity.this);

                    hideMatchResult();

                    if (swAutoContinueScan.isChecked()) {
                        startQrCode(REQ_QR_CODE_STORAGE_LOC);
                    }

                } else {
                    ExpressOnShelfActivity.this.tvStorageLocName.setText("仓位名称：" + respData.getStorageLocName());
                    ExpressOnShelfActivity.this.tvStorageLocExpressCount.setText("快递数量：" + respData.getStorageLocExpressCount());
                    ExpressOnShelfActivity.this.tvStorageLocPackageCount.setText("包裹数量：" + respData.getStorageLocPackageCount());

                    if (respData.isMatch()) {
                        showMatchSuccess();

                        if (swAutoContinueScan.isChecked()) {
                            onShelfComplete();
                        }

                    } else {
                        showMatchFail();
                    }
                }
            }

            @Override
            public void onFailure(Call<ExpressShelfOnShelfQueryResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressOnShelfActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void onShelfComplete() {
        String trackingNo = this.trackingNo;
        if (trackingNo == null || trackingNo.trim().equals("")) {
            SoundUtil.playError(ExpressOnShelfActivity.this);

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(ExpressOnShelfActivity.this);
            alertDialogBuilder.setTitle("错误提示");
            alertDialogBuilder.setMessage("请先扫描快递单号或舜达入库号！");
            alertDialogBuilder.setPositiveButton("确定", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();
            return;
        }

        String storageLocCode = this.storageLocCode;
        if (storageLocCode == null || storageLocCode.trim().equals("")) {
            SoundUtil.playError(ExpressOnShelfActivity.this);

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(ExpressOnShelfActivity.this);
            alertDialogBuilder.setTitle("错误提示");
            alertDialogBuilder.setMessage("仓位号还没扫描，请扫描仓位号！");
            alertDialogBuilder.setPositiveButton("确定", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();

            return;
        }

        ExpressShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressShelfRequest.class);
        Call<ResponseInfo> onShelfQueryCall = expressShelfRequest.onShelfComplete(trackingNo, storageLocCode);
        // Dialog mask = MaskDialog.show(ExpressOnShelfActivity.this);
        onShelfQueryCall.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                // mask.dismiss();

                ResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, ExpressOnShelfActivity.this)) {
                    return;
                }

                SoundUtil.playSuccess(ExpressOnShelfActivity.this);

                Toast.makeText(ExpressOnShelfActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                if (swAutoContinueScan.isChecked() == false) {
                    onShelfQuery(trackingNo, storageLocCode, false);
                } else {
                    startQrCode(REQ_QR_CODE_EXPRESS);
                }
            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressOnShelfActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
             */

            finish();
        }

        return true;
    }
}