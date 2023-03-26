package com.huadu.shunda.ui.unload;

import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.huadu.shunda.R;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxDetailItem;
import com.huadu.shunda.data.http.shipcontainerunloadsortpackbox.ShipContainerUnloadSortPackBoxDetailResponse;
import com.huadu.shunda.http.ShipContainerUnloadSortPackBoxRequest;
import com.huadu.shunda.ui.BaseActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class UnloadSortOrderPackBoxPositionActivity extends BaseActivity {

    private static String positionNameRemember = "";

    public static final int REQ_SCAN_AGAIN = 5;

    private int shipContainerOrderPackBoxId;
    private boolean multiOrderOnePos;

    private TextView tvShipContainerCode = null;
    private TextView tvOrderCode = null;
    private TextView tvCustomerCode = null;
    private TextView tvOrderPackBoxCode = null;
    private TextView tvOrderPackBoxShortCode = null;
    private TextView tvTotalBoxCount = null;
    private TextView tvScannedBoxCount = null;
    private TextView tvNoScannedBoxCount = null;
    private TextView tvOrderPosition = null;

    private EditText etPositionName = null;

    private Button btnSetPosition = null;
    private Button btnSetPositionAndScanAgain = null;

    private Button btnCancel = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_unload_sort_order_pack_box_position);

        shipContainerOrderPackBoxId = getIntent().getIntExtra("shipContainerOrderPackBoxId", 0);
        multiOrderOnePos = getIntent().getBooleanExtra("multiOrderOnePos", false);

        tvShipContainerCode = findViewById(R.id.tvShipContainerCode);
        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvOrderPackBoxCode = findViewById(R.id.tvOrderPackBoxCode);
        tvOrderPackBoxShortCode = findViewById(R.id.tvOrderPackBoxShortCode);
        tvTotalBoxCount = findViewById(R.id.tvTotalBoxCount);
        tvScannedBoxCount = findViewById(R.id.tvScannedBoxCount);
        tvNoScannedBoxCount = findViewById(R.id.tvNoScannedBoxCount);
        tvOrderPosition = findViewById(R.id.tvOrderPosition);

        etPositionName = findViewById(R.id.etPositionName);

        // etPositionName.setText(positionName);
        // tvOrderPackBoxCode.setText("Box Code: " + orderPackBoxCode);

        btnSetPosition = findViewById(R.id.btnSetPosition);
        btnSetPosition.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                setOrderPackBoxCodePosition(false);
            }
        });

        btnSetPositionAndScanAgain = findViewById(R.id.btnSetPositionAndScanAgain);
        btnSetPositionAndScanAgain.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                setOrderPackBoxCodePosition(true);
            }
        });

        btnCancel = findViewById(R.id.btnCancel);
        btnCancel.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                finish();
            }
        });
    }

    @Override
    protected void onStart() {
        super.onStart();

        loadOderPackBoxDetail(shipContainerOrderPackBoxId);
    }

    private void loadOderPackBoxDetail(int shipContainerOrderPackBoxId) {
        // hideScannedDetail();

        ShipContainerUnloadSortPackBoxRequest shipContainerOrderPackBoxRequest = RetrofitUtil.createRequest(ShipContainerUnloadSortPackBoxRequest.class);
        Call<ShipContainerUnloadSortPackBoxDetailResponse> loadCall = shipContainerOrderPackBoxRequest.packBoxDetail(shipContainerOrderPackBoxId);
        loadCall.enqueue(new Callback<ShipContainerUnloadSortPackBoxDetailResponse>() {
            @Override
            public void onResponse(Call<ShipContainerUnloadSortPackBoxDetailResponse> call, Response<ShipContainerUnloadSortPackBoxDetailResponse> response) {
                ShipContainerUnloadSortPackBoxDetailResponse respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, UnloadSortOrderPackBoxPositionActivity.this)) {
                    return;
                }

                SoundUtil.playSuccess(UnloadSortOrderPackBoxPositionActivity.this);

                ShipContainerUnloadSortPackBoxDetailItem orderPackBox = respData.getOrderPackBox();

                tvShipContainerCode.setText("集装箱号: " + orderPackBox.getShipContainerCode());
                tvOrderCode.setText("订单编号: " + orderPackBox.getOrderCode());
                tvCustomerCode.setText("客户ID: " + orderPackBox.getCustomerCode());
                tvOrderPackBoxCode.setText("箱号: " + orderPackBox.getOrderPackBoxCode());
                tvOrderPackBoxShortCode.setText("短箱号: " + orderPackBox.getOrderPackBoxShortCode());
                tvTotalBoxCount.setText("订单装箱总数: " + orderPackBox.getTotalBoxCount());
                tvScannedBoxCount.setText("已扫码箱数: " + orderPackBox.getScannedBoxCount());
                tvNoScannedBoxCount.setText("未扫码箱数: " + orderPackBox.getNoScannedBoxCount());
                tvOrderPosition.setText("订单位置: " + orderPackBox.getOrderPositionNames());

                String positionName = orderPackBox.getPositionName();
                if (positionName != null && "".equals(positionName.trim()) == false) {
                    etPositionName.setText(positionName);
                } else if (multiOrderOnePos) {
                    etPositionName.setText(UnloadSortOrderPackBoxPositionActivity.positionNameRemember);
                } else {
                    etPositionName.setText("");
                }

                btnSetPosition.setVisibility(View.VISIBLE);

                Toast.makeText(UnloadSortOrderPackBoxPositionActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onFailure(Call<ShipContainerUnloadSortPackBoxDetailResponse> call, Throwable t) {
                Toast.makeText(UnloadSortOrderPackBoxPositionActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void setOrderPackBoxCodePosition(boolean scanAgain) {
        String positionNameInput = etPositionName.getText().toString();

        ShipContainerUnloadSortPackBoxRequest shipContainerOrderPackBoxRequest = RetrofitUtil.createRequest(ShipContainerUnloadSortPackBoxRequest.class);
        Call<ResponseInfo> loadCall = shipContainerOrderPackBoxRequest.setPosition(shipContainerOrderPackBoxId, positionNameInput, multiOrderOnePos);
        loadCall.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                ResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, UnloadSortOrderPackBoxPositionActivity.this)) {
                    return;
                }

                SoundUtil.playSuccess(UnloadSortOrderPackBoxPositionActivity.this);

                Toast.makeText(UnloadSortOrderPackBoxPositionActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                UnloadSortOrderPackBoxPositionActivity.positionNameRemember = positionNameInput; // 记忆最后一次输入

                Intent intent = new Intent();
                intent.putExtra("scanAgain", scanAgain);
                setResult(RESULT_OK, intent);
                finish();
            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(UnloadSortOrderPackBoxPositionActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();
        }

        return true;
    }
}
