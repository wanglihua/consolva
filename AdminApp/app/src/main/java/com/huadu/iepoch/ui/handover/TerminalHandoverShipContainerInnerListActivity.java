package com.huadu.iepoch.ui.handover;

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
import com.huadu.iepoch.data.http.terminalhandover.TerminalHandoverShipContainerInnerListItemInfo;
import com.huadu.iepoch.data.http.terminalhandover.TerminalHandoverShipContainerInnerListResponseInfo;
import com.huadu.iepoch.http.TerminalHandoverRequest;
import com.huadu.iepoch.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TerminalHandoverShipContainerInnerListActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_DATA_LIST = 100100;

    private int pickupStationId;
    private String pickupStationName;

    private ListView lvShipContainerInnerList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_terminal_handover_ship_container_inner_list);

        Intent intent = getIntent();
        pickupStationId = intent.getIntExtra("pickupStationId", 0);
        pickupStationName = intent.getStringExtra("pickupStationName");

        lvShipContainerInnerList = findViewById(R.id.lvShipContainerInnerList);
        lvShipContainerInnerList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                int shipContainerInnerPickupStationId = (int) (map.get("id"));
                int shipContainerInnerId = (int) (map.get("shipContainerInnerId"));
                String shipContainerInnerCode = (String) (map.get("shipContainerInnerCode"));

                Intent intent = new Intent(TerminalHandoverShipContainerInnerListActivity.this, TerminalHandoverActivity.class);
                intent.putExtra("shipContainerInnerPickupStationId", shipContainerInnerPickupStationId);
                intent.putExtra("pickupStationId", pickupStationId);
                intent.putExtra("pickupStationName", pickupStationName);
                intent.putExtra("shipContainerInnerId", shipContainerInnerId);
                intent.putExtra("shipContainerInnerCode", shipContainerInnerCode);

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

                TerminalHandoverRequest shipContainerInnerRequest = RetrofitUtil.createRequest(TerminalHandoverRequest.class);
                Call<TerminalHandoverShipContainerInnerListResponseInfo> listDataCall = shipContainerInnerRequest.shipContainerInnerList(pickupStationId);
                // Dialog mask = MaskDialog.show(TerminalHandoverShipContainerInnerListActivity.this);
                listDataCall.enqueue(new Callback<TerminalHandoverShipContainerInnerListResponseInfo>() {
                    @Override
                    public void onResponse(Call<TerminalHandoverShipContainerInnerListResponseInfo> call, Response<TerminalHandoverShipContainerInnerListResponseInfo> response) {
                        // mask.dismiss();

                        TerminalHandoverShipContainerInnerListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, TerminalHandoverShipContainerInnerListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHIP_CONTAINER_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<TerminalHandoverShipContainerInnerListResponseInfo> call, Throwable t) {
                        Toast.makeText(TerminalHandoverShipContainerInnerListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    TerminalHandoverShipContainerInnerListResponseInfo listDataResponse = (TerminalHandoverShipContainerInnerListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (TerminalHandoverShipContainerInnerListItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("shipContainerInnerId", item.getShipContainerInnerId());
                        dataMap.put("shipContainerInnerCode", item.getShipContainerInnerCode());
                        dataMap.put("toWarehouseId", item.getToWarehouseId());
                        dataMap.put("toWarehouseName", "目的仓：" + item.getToWarehouseName());
                        dataMap.put("actualCheckinTime", "实际到仓时间：" + item.getActualCheckinTime());
                        dataMap.put("totalPackageCount", "本取货点箱数：" + item.getTotalPackageCount() + "箱");
                        dataMap.put("totalVolume", "本取货点体积：" + item.getTotalVolume() + "立方");
                        dataMap.put("totalActualWeight", "本取货点总重量：" + item.getTotalActualWeight() + "Kg");
                        dataMap.put("maxSideLength", "本取货点最长边：" + item.getMaxSideLength() + "Cm");

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(TerminalHandoverShipContainerInnerListActivity.this
                                , listDataMap
                                , R.layout.item_terminal_handover_ship_container_inner_list
                                , new String[]{"shipContainerInnerCode", "toWarehouseName", "actualCheckinTime", "totalPackageCount", "totalVolume", "totalActualWeight", "maxSideLength"}
                                , new int[]{R.id.tvShipContainerInnerCode, R.id.tvToWarehouseName, R.id.tvActualCheckinTime, R.id.tvTotalPackageCount, R.id.tvTotalVolume, R.id.tvTotalActualWeight, R.id.tvMaxSideLength});

                        lvShipContainerInnerList.setAdapter(adapter);
                    } else {
                        Toast.makeText(TerminalHandoverShipContainerInnerListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
