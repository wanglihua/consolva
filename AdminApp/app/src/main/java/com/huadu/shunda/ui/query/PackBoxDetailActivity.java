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
import com.huadu.shunda.data.http.packbox.PackBoxDetailDataResponseInfo;
import com.huadu.shunda.http.PackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PackBoxDetailActivity extends BaseActivity {

    private static final int REQ_QR_CODE_PACK_BOX_CODE = 1;

    private TextView tvOrderPackBoxCode = null;
    private TextView tvOrderPackBoxShortCode = null;
    private TextView tvOrderPackBoxNum = null;
    private TextView tvOrderPackBoxCount = null;
    private TextView tvOrderPackBoxUnloadCount = null;
    private TextView tvOrderPackBoxStatus = null;
    private TextView tvCustomerCode = null;
    private TextView tvOrderCode = null;
    private TextView tvShipContainerCode = null;
    private TextView tvShipContainerInnerCode = null;
    private TextView tvTargetWarehouseName = null;
    private TextView tvFirstLogisticsTypeName = null;
    private TextView tvFirstLogisticsModeName = null;
    private TextView tvSecondLogisticsTypeName = null;
    private TextView tvPickupStationName = null;
    private TextView tvOrderGoodsTypeName = null;
    private TextView tvExpressGoodsTypeName = null;
    private TextView tvWeight = null;
    private TextView tvVolumeWeight = null;
    private TextView tvVolume = null;
    private TextView tvUserName = null;
    private TextView tvPackTime = null;
    private TextView tvCustomerMobile = null;
    private TextView tvOrderMobile = null;
    private TextView tvConsignee = null;
    private TextView tvAddress = null;
    private TextView tvZipcode = null;

    private Button btnScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pack_box_detail);

        tvOrderPackBoxCode = findViewById(R.id.tvOrderPackBoxCode);
        tvOrderPackBoxShortCode = findViewById(R.id.tvOrderPackBoxShortCode);
        tvOrderPackBoxNum = findViewById(R.id.tvOrderPackBoxNum);
        tvOrderPackBoxCount = findViewById(R.id.tvOrderPackBoxCount);
        tvOrderPackBoxUnloadCount = findViewById(R.id.tvOrderPackBoxUnloadCount);
        tvOrderPackBoxStatus = findViewById(R.id.tvOrderPackBoxStatus);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvShipContainerCode = findViewById(R.id.tvShipContainerCode);
        tvShipContainerInnerCode = findViewById(R.id.tvShipContainerInnerCode);
        tvTargetWarehouseName = findViewById(R.id.tvTargetWarehouseName);
        tvFirstLogisticsTypeName = findViewById(R.id.tvFirstLogisticsTypeName);
        tvFirstLogisticsModeName = findViewById(R.id.tvFirstLogisticsModeName);
        tvSecondLogisticsTypeName = findViewById(R.id.tvSecondLogisticsTypeName);
        tvPickupStationName = findViewById(R.id.tvPickupStationName);
        tvOrderGoodsTypeName = findViewById(R.id.tvOrderGoodsTypeName);
        tvExpressGoodsTypeName = findViewById(R.id.tvExpressGoodsTypeName);
        tvWeight = findViewById(R.id.tvWeight);
        tvVolumeWeight = findViewById(R.id.tvVolumeWeight);
        tvVolume = findViewById(R.id.tvVolume);
        tvUserName = findViewById(R.id.tvUserName);
        tvPackTime = findViewById(R.id.tvPackTime);
        tvCustomerMobile = findViewById(R.id.tvCustomerMobile);
        tvOrderMobile = findViewById(R.id.tvOrderMobile);
        tvConsignee = findViewById(R.id.tvConsignee);
        tvAddress = findViewById(R.id.tvAddress);
        tvZipcode = findViewById(R.id.tvZipcode);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_PACK_BOX_CODE);
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描箱号二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(PackBoxDetailActivity.this, requestCode) == false) {
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
                Call<PackBoxDetailDataResponseInfo> orderPayDetailCall = packBoxRequest.detailData(packBoxCode);
                Dialog mask = MaskDialog.show(PackBoxDetailActivity.this);
                orderPayDetailCall.enqueue(new Callback<PackBoxDetailDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PackBoxDetailDataResponseInfo> call, Response<PackBoxDetailDataResponseInfo> response) {
                        mask.dismiss();

                        PackBoxDetailDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, PackBoxDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(PackBoxDetailActivity.this);

                        tvOrderPackBoxCode.setText("箱号：" + respData.getOrderPackBoxCode());
                        tvOrderPackBoxShortCode.setText("短箱号：" + respData.getOrderPackBoxShortCode());
                        tvOrderPackBoxNum.setText("箱序：" + respData.getOrderPackBoxNum());
                        tvOrderPackBoxCount.setText("总箱数：" + respData.getOrderPackBoxCount());
                        tvOrderPackBoxUnloadCount.setText("已卸货箱数：" + respData.getOrderPackBoxUnloadCount());
                        tvOrderPackBoxStatus.setText("箱号状态：" + respData.getOrderPackBoxStatus());
                        tvCustomerCode.setText("客户ID：" + respData.getCustomerCode());
                        tvOrderCode.setText("订单编号：" + respData.getOrderCode());
                        tvShipContainerCode.setText("集装箱号：" + respData.getShipContainerCode());
                        tvShipContainerInnerCode.setText("出发地船航：" + respData.getShipContainerInnerCode());
                        tvTargetWarehouseName.setText("目的仓：" + respData.getTargetWarehouseName());
                        tvFirstLogisticsTypeName.setText("头程方式：" + respData.getFirstLogisticsTypeName());
                        tvFirstLogisticsModeName.setText("头程物流：" + respData.getFirstLogisticsModeName());
                        tvSecondLogisticsTypeName.setText("二程物流：" + respData.getSecondLogisticsTypeName());
                        tvPickupStationName.setText("取货点：" + respData.getPickupStationName());
                        tvOrderGoodsTypeName.setText("订单物品类型：" + respData.getOrderGoodsTypeName());
                        tvExpressGoodsTypeName.setText("快递单物品类型：" + respData.getExpressGoodsTypeName());
                        tvWeight.setText("实际重：" + respData.getWeight());
                        tvVolumeWeight.setText("体积重：" + respData.getVolumeWeight());
                        tvVolume.setText("体积：" + respData.getVolume());
                        tvUserName.setText("打包工：" + respData.getUserName());
                        tvPackTime.setText("打包时间：" + respData.getPackTime());
                        tvCustomerMobile.setText("注册手机号：" + respData.getCustomerMobile());
                        tvOrderMobile.setText("订单手机号：" + respData.getOrderMobile());
                        tvConsignee.setText("收货人：" + respData.getConsignee());
                        tvAddress.setText("收货地址：" + respData.getAddress());
                        tvZipcode.setText("邮编：" + respData.getZipcode());
                    }

                    @Override
                    public void onFailure(Call<PackBoxDetailDataResponseInfo> call, Throwable t) {
                        Toast.makeText(PackBoxDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
