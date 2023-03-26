package com.huadu.shunda.ui.unload;

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
import com.huadu.shunda.data.http.shipcontainerunloadsort.ShipContainerUnloadSortItemInfo;
import com.huadu.shunda.data.http.shipcontainerunloadsort.ShipContainerUnloadSortListResponse;
import com.huadu.shunda.http.ShipContainerUnloadSortRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class UnloadSortShipContainerListActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_DATA_LIST = 100100;

    private ListView lvShipContainerList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_unload_sort_ship_container_list);

        lvShipContainerList = findViewById(R.id.lvShipContainerList);
        lvShipContainerList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(UnloadSortShipContainerListActivity.this, UnloadSortShipContainerDetailActivity.class);
                intent.putExtra("shipContainerId", (int) map.get("shipContainerId"));
                intent.putExtra("shipContainerCode", (String) map.get("shipContainerCode"));

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

                ShipContainerUnloadSortRequest shipContainerRequest = RetrofitUtil.createRequest(ShipContainerUnloadSortRequest.class);
                Call<ShipContainerUnloadSortListResponse> listDataCall = shipContainerRequest.shipContainerList();
                listDataCall.enqueue(new Callback<ShipContainerUnloadSortListResponse>() {
                    @Override
                    public void onResponse(Call<ShipContainerUnloadSortListResponse> call, Response<ShipContainerUnloadSortListResponse> response) {
                        ShipContainerUnloadSortListResponse respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, UnloadSortShipContainerListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHIP_CONTAINER_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ShipContainerUnloadSortListResponse> call, Throwable t) {
                        Toast.makeText(UnloadSortShipContainerListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    ShipContainerUnloadSortListResponse listDataResponse = (ShipContainerUnloadSortListResponse) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ShipContainerUnloadSortItemInfo item : listDataResponse.getShipContainerList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("shipContainerId", item.getShipContainerId());
                        dataMap.put("shipContainerCode", item.getShipContainerCode());
                        dataMap.put("arriveTime", "到港日期：" + item.getArriveTime());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(UnloadSortShipContainerListActivity.this
                                , listDataMap
                                , R.layout.item_unload_sort_ship_container_list
                                , new String[]{"shipContainerCode", "arriveTime"}
                                , new int[]{R.id.tvShipContainerCode, R.id.tvArriveTime});

                        lvShipContainerList.setAdapter(adapter);
                    } else {
                        Toast.makeText(UnloadSortShipContainerListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
