package com.huadu.shunda.ui.pickup.booking;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Toast;

import com.huadu.shunda.R;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.pickupbooking.PickupBookingItemInfo;
import com.huadu.shunda.data.http.pickupbooking.PickupBookingListDataResponseInfo;
import com.huadu.shunda.http.PickupBookingRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PickupBookingListActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_DATA_LIST = 100100;

    private int warehouseId = 0;
    private int pickupStationId = 0;

    private ListView lvPickupBookingList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pickup_booking_list);

        warehouseId = getIntent().getIntExtra("warehouseId", 0);
        pickupStationId = getIntent().getIntExtra("pickupStationId", 0);

        lvPickupBookingList = findViewById(R.id.lvPickupBookingList);
        lvPickupBookingList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(PickupBookingListActivity.this, PickupBookingDetailActivity.class);
                intent.putExtra("bookingId", (int) map.get("id"));
                PickupBookingDetailActivity.pickupBookingItem = (PickupBookingItemInfo) map.get("item");
                startActivity(intent);
            }
        });
    }

    @Override
    public void onStart() {
        super.onStart();

        new Thread(new Runnable() {
            @Override
            public void run() {

                PickupBookingRequest pickupBookingRequest = RetrofitUtil.createRequest(PickupBookingRequest.class);
                Call<PickupBookingListDataResponseInfo> listDataCall = pickupBookingRequest.listData(warehouseId, pickupStationId);
                // Dialog mask = MaskDialog.show(PickupBookingListActivity.this);
                listDataCall.enqueue(new Callback<PickupBookingListDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PickupBookingListDataResponseInfo> call, Response<PickupBookingListDataResponseInfo> response) {
                        // mask.dismiss();

                        PickupBookingListDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, PickupBookingListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHIP_CONTAINER_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<PickupBookingListDataResponseInfo> call, Throwable t) {
                        Toast.makeText(PickupBookingListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });


            }
        }).start();
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_SHIP_CONTAINER_DATA_LIST:

                    PickupBookingListDataResponseInfo listDataResponse = (PickupBookingListDataResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (PickupBookingItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("item", item);
                        dataMap.put("id", item.getId());
                        dataMap.put("orderCode", item.getOrderCode());
                        dataMap.put("statusName", "状态：" + item.getStatusName());
                        dataMap.put("warehouseName", "仓库：" + item.getWarehouseName());
                        dataMap.put("pickupStationName", "取货点：" + item.getPickupStationName());
                        dataMap.put("pickupBookingTime", "预约取货时间：" + item.getPickupBookingTime());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(PickupBookingListActivity.this
                                , listDataMap
                                , R.layout.item_pickup_booking_list
                                , new String[]{"orderCode", "statusName", "warehouseName", "pickupStationName", "pickupBookingTime"}
                                , new int[]{R.id.tvOrderCode, R.id.tvStatusName, R.id.tvWarehouseName, R.id.tvPickupStationName, R.id.tvPickupBookingTime});

                        lvPickupBookingList.setAdapter(adapter);
                    } else {
                        Toast.makeText(PickupBookingListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                    break;
                default:
                    break;
            }
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
