package com.huadu.iepoch.ui.pickup.booking;

import android.app.Dialog;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.pickupbooking.PickupBookingItemInfo;
import com.huadu.iepoch.http.PickupBookingRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.photo.PhotoOrderActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PickupBookingDetailActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_INNER_DETAIL_DATA_LIST = 100400;

    private int bookingId;

    public static PickupBookingItemInfo pickupBookingItem = null;

    private TextView tvOrderCode = null;
    private TextView tvCustomerCode = null;
    private TextView tvStatusName = null;
    private TextView tvWarehouseName = null;
    private TextView tvPickupStationName = null;
    private TextView tvPickupBookingTime = null;
    private TextView tvStockPlaceList = null;

    private Button btnConfirm = null;

    private ListView lvNotLoadList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pickup_booking_detail);

        bookingId = getIntent().getIntExtra("bookingId", 0);

        tvOrderCode = findViewById(R.id.tvOrderCode);
        tvCustomerCode = findViewById(R.id.tvCustomerCode);
        tvStatusName = findViewById(R.id.tvStatusName);
        tvWarehouseName = findViewById(R.id.tvWarehouseName);
        tvPickupStationName = findViewById(R.id.tvPickupStationName);
        tvPickupBookingTime = findViewById(R.id.tvPickupBookingTime);
        tvStockPlaceList = findViewById(R.id.tvStockPlaceList);

        tvOrderCode.setText(pickupBookingItem.getOrderCode());
        tvCustomerCode.setText(pickupBookingItem.getCustomerCode());
        tvStatusName.setText("状态：" + pickupBookingItem.getStatusName());
        tvWarehouseName.setText("仓库：" + pickupBookingItem.getWarehouseName());
        tvPickupStationName.setText("取货点：" + pickupBookingItem.getPickupStationName());
        tvPickupBookingTime.setText("预约取货时间：" + pickupBookingItem.getPickupBookingTime());
        tvStockPlaceList.setText(pickupBookingItem.getStockPlaceList());

        btnConfirm = findViewById(R.id.btnConfirm);
        btnConfirm.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                PickupBookingRequest pickupBookingRequest = RetrofitUtil.createRequest(PickupBookingRequest.class);
                Call<ResponseInfo> confirmCall = pickupBookingRequest.confirm(bookingId);
                Dialog mask = MaskDialog.show(PickupBookingDetailActivity.this);
                confirmCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, PickupBookingDetailActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(PickupBookingDetailActivity.this);
                        Toast.makeText(PickupBookingDetailActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(PickupBookingDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        });
    }

    @Override
    protected void onStart() {
        super.onStart();
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
