package com.huadu.shunda.ui.stockcheck;

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
import com.huadu.shunda.data.http.stockcheck.StockCheckWarehouseItemInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckWarehouseListResponseInfo;
import com.huadu.shunda.http.StockCheckRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockCheckWarehouseListActivity extends BaseActivity {

    private static final int REFRESH_WAREHOUSE_DATA_LIST = 100100;

    private ListView lvWarehouseList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_check_warehouse_list);

        lvWarehouseList = findViewById(R.id.lvWarehouseList);
        lvWarehouseList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);


                Intent intent = new Intent(StockCheckWarehouseListActivity.this, StockCheckBillListActivity.class);
                intent.putExtra("warehouseId", (int) map.get("id"));

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

                StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                Call<StockCheckWarehouseListResponseInfo> warehouseListCall = stockCheckRequest.warehouseList();
                // Dialog mask = MaskDialog.show(StockCheckWarehouseListActivity.this);
                warehouseListCall.enqueue(new Callback<StockCheckWarehouseListResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockCheckWarehouseListResponseInfo> call, Response<StockCheckWarehouseListResponseInfo> response) {
                        // mask.dismiss();

                        StockCheckWarehouseListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, StockCheckWarehouseListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_WAREHOUSE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockCheckWarehouseListResponseInfo> call, Throwable t) {
                        Toast.makeText(StockCheckWarehouseListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    StockCheckWarehouseListResponseInfo warehouseListResponse = (StockCheckWarehouseListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (StockCheckWarehouseItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("code", item.getCode());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(StockCheckWarehouseListActivity.this
                                , listDataMap
                                , R.layout.item_stock_check_warehouse_list
                                , new String[]{"code", "name"}
                                , new int[]{R.id.tvCode, R.id.tvName});

                        lvWarehouseList.setAdapter(adapter);
                    } else {
                        Toast.makeText(StockCheckWarehouseListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
        }

        return true;
    }
}
