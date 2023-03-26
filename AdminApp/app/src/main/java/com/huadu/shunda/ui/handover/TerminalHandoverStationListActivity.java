package com.huadu.shunda.ui.handover;

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
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.terminalhandover.TerminalHandoverStationListItemInfo;
import com.huadu.shunda.data.http.terminalhandover.TerminalHandoverStationListResponseInfo;
import com.huadu.shunda.http.TerminalHandoverRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TerminalHandoverStationListActivity extends BaseActivity {

    private static final int REFRESH_PICKUP_STATION_LIST = 100100;

    private ListView lvPickupStationList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_terminal_handover_station_list);

        lvPickupStationList = findViewById(R.id.lvPickupStationList);
        lvPickupStationList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                int pickupStationId = (int) map.get("id");
                String pickupStationName = (String) map.get("name");

                Intent intent = new Intent(TerminalHandoverStationListActivity.this, TerminalHandoverShipContainerInnerListActivity.class);
                intent.putExtra("pickupStationId", pickupStationId);
                intent.putExtra("pickupStationName", pickupStationName);
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

                TerminalHandoverRequest terminalHandoverRequest = RetrofitUtil.createRequest(TerminalHandoverRequest.class);
                Call<TerminalHandoverStationListResponseInfo> stationListCall = terminalHandoverRequest.stationList(LoginCache.getUserId());
                // Dialog mask = MaskDialog.show(TerminalHandoverStationListActivity.this);
                stationListCall.enqueue(new Callback<TerminalHandoverStationListResponseInfo>() {
                    @Override
                    public void onResponse(Call<TerminalHandoverStationListResponseInfo> call, Response<TerminalHandoverStationListResponseInfo> response) {
                        // mask.dismiss();

                        TerminalHandoverStationListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, TerminalHandoverStationListActivity.this)) {
                            return;
                        }
                        
                        Message msg = new Message();
                        msg.what = REFRESH_PICKUP_STATION_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<TerminalHandoverStationListResponseInfo> call, Throwable t) {
                        Toast.makeText(TerminalHandoverStationListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_PICKUP_STATION_LIST:

                    TerminalHandoverStationListResponseInfo listDataResponse = (TerminalHandoverStationListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (TerminalHandoverStationListItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(TerminalHandoverStationListActivity.this
                                , listDataMap
                                , R.layout.item_terminal_handover_pickup_station_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvPickupStationList.setAdapter(adapter);
                    } else {
                        Toast.makeText(TerminalHandoverStationListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
