package com.huadu.shunda.ui.pickup.terminal;

import android.app.Dialog;
import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.huadu.shunda.R;
import com.huadu.shunda.core.ClientTimeUtil;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.packbox.PackBoxDetailDataResponseInfo;
import com.huadu.shunda.http.TerminalPickupRequest;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TerminalPickupPackBoxActivity extends AppCompatActivity {

    public static PackBoxDetailDataResponseInfo packBoxDetailDataResponse = null;

    private int pickupStationId;
    private String pickupCode;

    private TextView tvOrderPackBoxCode = null;
    private TextView tvOrderPackBoxShortCode = null;
    private TextView tvOrderPackBoxNum = null;
    private TextView tvOrderPackBoxCount = null;
    private TextView tvCustomerCode = null;
    private TextView tvOrderCode = null;
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

    private Button btnPickupConfirm = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_terminal_pickup_pack_box);
        Intent intent = getIntent();

        pickupStationId = intent.getIntExtra("pickupStationId", 0);
        pickupCode = intent.getStringExtra("pickupCode");

        tvOrderPackBoxCode = findViewById(R.id.tvOrderPackBoxCode);
        tvOrderPackBoxShortCode = findViewById(R.id.tvOrderPackBoxShortCode);
        tvOrderPackBoxNum = findViewById(R.id.tvOrderPackBoxNum);
        tvOrderPackBoxCount = findViewById(R.id.tvOrderPackBoxCount);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
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

        btnPickupConfirm = findViewById(R.id.btnPickupConfirm);

        btnPickupConfirm.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                confirmPackBoxPickup();
            }
        });

        setPackBoxDetail();
    }

    private void setPackBoxDetail() {
        tvOrderPackBoxCode.setText("箱号：" + packBoxDetailDataResponse.getOrderPackBoxCode());
        tvOrderPackBoxShortCode.setText("短箱号：" + packBoxDetailDataResponse.getOrderPackBoxShortCode());
        tvOrderPackBoxNum.setText("箱序：" + packBoxDetailDataResponse.getOrderPackBoxNum());
        tvOrderPackBoxCount.setText("总箱数：" + packBoxDetailDataResponse.getOrderPackBoxCount());
        tvCustomerCode.setText("客户ID：" + packBoxDetailDataResponse.getCustomerCode());
        tvOrderCode.setText("订单编号：" + packBoxDetailDataResponse.getOrderCode());
        tvTargetWarehouseName.setText("目的仓：" + packBoxDetailDataResponse.getTargetWarehouseName());
        tvFirstLogisticsTypeName.setText("头程方式：" + packBoxDetailDataResponse.getFirstLogisticsTypeName());
        tvFirstLogisticsModeName.setText("头程物流：" + packBoxDetailDataResponse.getFirstLogisticsModeName());
        tvSecondLogisticsTypeName.setText("二程物流：" + packBoxDetailDataResponse.getSecondLogisticsTypeName());
        tvPickupStationName.setText("取货点：" + packBoxDetailDataResponse.getPickupStationName());
        tvOrderGoodsTypeName.setText("订单物品类型：" + packBoxDetailDataResponse.getOrderGoodsTypeName());
        tvExpressGoodsTypeName.setText("快递单物品类型：" + packBoxDetailDataResponse.getExpressGoodsTypeName());
        tvWeight.setText("实际重：" + packBoxDetailDataResponse.getWeight());
        tvVolumeWeight.setText("体积重：" + packBoxDetailDataResponse.getVolumeWeight());
        tvVolume.setText("体积：" + packBoxDetailDataResponse.getVolume());
        tvUserName.setText("打包工：" + packBoxDetailDataResponse.getUserName());
        tvPackTime.setText("打包时间：" + packBoxDetailDataResponse.getPackTime());
    }

    private void confirmPackBoxPickup() {
        String clientTime = ClientTimeUtil.getClientTime();

        TerminalPickupRequest terminalPickupRequest = RetrofitUtil.createRequest(TerminalPickupRequest.class);
        Call<ResponseInfo> pickupConfirmCall = terminalPickupRequest.pickupPackBoxConfirm(pickupStationId, pickupCode, packBoxDetailDataResponse.getOrderPackBoxShortCode(), clientTime);
        Dialog mask = MaskDialog.show(TerminalPickupPackBoxActivity.this);
        pickupConfirmCall.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                mask.dismiss();

                ResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, TerminalPickupPackBoxActivity.this)) {
                    return;
                }

                SoundUtil.playSuccess(TerminalPickupPackBoxActivity.this);

                Toast.makeText(TerminalPickupPackBoxActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();
                finish();
            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(TerminalPickupPackBoxActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
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