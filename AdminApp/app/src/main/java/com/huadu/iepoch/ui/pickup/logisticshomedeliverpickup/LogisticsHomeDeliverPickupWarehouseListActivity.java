package com.huadu.iepoch.ui.pickup.logisticshomedeliverpickup;

import android.app.Dialog;
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

import androidx.appcompat.app.AlertDialog;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.data.http.logisitcshomedeliverpickup.LogisticsHomeDeliverPickupWarehouseItemInfo;
import com.huadu.iepoch.data.http.logisitcshomedeliverpickup.LogisticsHomeDeliverPickupWarehouseListResponseInfo;
import com.huadu.iepoch.http.LogisticsHomeDeliverPickupRequest;
import com.huadu.iepoch.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LogisticsHomeDeliverPickupWarehouseListActivity extends BaseActivity {

    private static final int REFRESH_WAREHOUSE_DATA_LIST = 100100;

    private ListView lvWarehouseList = null;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_logistics_home_deliver_warehouse_list);

        lvWarehouseList = findViewById(R.id.lvWarehouseList);
        lvWarehouseList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(LogisticsHomeDeliverPickupWarehouseListActivity.this, LogisticsHomeDeliverPickupActivity.class);
                intent.putExtra("warehouseId", (int) map.get("id"));
                intent.putExtra("code", (String) map.get("code"));
                intent.putExtra("name", (String) map.get("name"));

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

                LogisticsHomeDeliverPickupRequest warehousePickupRequest = RetrofitUtil.createRequest(LogisticsHomeDeliverPickupRequest.class);
                Call<LogisticsHomeDeliverPickupWarehouseListResponseInfo> warehouseListCall = warehousePickupRequest.warehouseList();
                // Dialog mask = MaskDialog.show(LogisticsHomeDeliverPickupWarehouseListActivity.this);
                warehouseListCall.enqueue(new Callback<LogisticsHomeDeliverPickupWarehouseListResponseInfo>() {
                    @Override
                    public void onResponse(Call<LogisticsHomeDeliverPickupWarehouseListResponseInfo> call, Response<LogisticsHomeDeliverPickupWarehouseListResponseInfo> response) {
                        // mask.dismiss();

                        LogisticsHomeDeliverPickupWarehouseListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, LogisticsHomeDeliverPickupWarehouseListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_WAREHOUSE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<LogisticsHomeDeliverPickupWarehouseListResponseInfo> call, Throwable t) {
                        Toast.makeText(LogisticsHomeDeliverPickupWarehouseListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_WAREHOUSE_DATA_LIST:

                    LogisticsHomeDeliverPickupWarehouseListResponseInfo warehouseListResponse = (LogisticsHomeDeliverPickupWarehouseListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (LogisticsHomeDeliverPickupWarehouseItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("code", item.getCode());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(LogisticsHomeDeliverPickupWarehouseListActivity.this
                                , listDataMap
                                , R.layout.item_logistics_home_deliver_pickup_warehouse_list
                                , new String[]{"code", "name"}
                                , new int[]{R.id.tvCode, R.id.tvName});

                        lvWarehouseList.setAdapter(adapter);
                    } else {
                        Toast.makeText(LogisticsHomeDeliverPickupWarehouseListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
            finish();

            /*
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
             */
        }

        return true;
    }
}
