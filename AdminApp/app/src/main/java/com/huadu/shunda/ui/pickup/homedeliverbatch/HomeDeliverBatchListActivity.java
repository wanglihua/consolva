package com.huadu.shunda.ui.pickup.homedeliverbatch;

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
import com.huadu.shunda.data.http.homedeliverbatch.HomeDeliverBatchItemInfo;
import com.huadu.shunda.data.http.homedeliverbatch.HomeDeliverBatchListResponseInfo;
import com.huadu.shunda.http.HomeDeliverBatchRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomeDeliverBatchListActivity extends BaseActivity {

    private static final int REFRESH_BATCH_DATA_LIST = 100100;

    private int warehouseId = 0;
    private String warehouseCode = "";
    private String warehouseName = "";

    private ListView lvBatchList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home_deliver_batch_list);

        Intent intent = getIntent();
        warehouseId = intent.getIntExtra("warehouseId", 0);
        warehouseCode = intent.getStringExtra("warehouseCode");
        warehouseName = intent.getStringExtra("warehouseName");

        setTitle(warehouseName + "送货批次");

        lvBatchList = findViewById(R.id.lvBatchList);
        lvBatchList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(HomeDeliverBatchListActivity.this, HomeDeliverBatchEntryListActivity.class);
                intent.putExtra("batchId", (int) map.get("id"));
                intent.putExtra("batchCode", (String) map.get("code"));

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

                HomeDeliverBatchRequest homeDeliverBatchRequest = RetrofitUtil.createRequest(HomeDeliverBatchRequest.class);
                Call<HomeDeliverBatchListResponseInfo> batchListCall = homeDeliverBatchRequest.batchList(warehouseId);
                // Dialog mask = MaskDialog.show(HomeDeliverBatchWarehouseListActivity.this);
                batchListCall.enqueue(new Callback<HomeDeliverBatchListResponseInfo>() {
                    @Override
                    public void onResponse(Call<HomeDeliverBatchListResponseInfo> call, Response<HomeDeliverBatchListResponseInfo> response) {
                        // mask.dismiss();

                        HomeDeliverBatchListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_BATCH_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<HomeDeliverBatchListResponseInfo> call, Throwable t) {
                        Toast.makeText(HomeDeliverBatchListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_BATCH_DATA_LIST:

                    HomeDeliverBatchListResponseInfo warehouseListResponse = (HomeDeliverBatchListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (HomeDeliverBatchItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("code", item.getCode());
                        dataMap.put("planDeliverDate", item.getPlanDeliverDate());

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(HomeDeliverBatchListActivity.this
                                , listDataMap
                                , R.layout.item_home_deliver_batch_list
                                , new String[]{"code", "planDeliverDate"}
                                , new int[]{R.id.tvCode, R.id.tvPlanDeliverDate});

                        lvBatchList.setAdapter(adapter);
                    } else {
                        Toast.makeText(HomeDeliverBatchListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
