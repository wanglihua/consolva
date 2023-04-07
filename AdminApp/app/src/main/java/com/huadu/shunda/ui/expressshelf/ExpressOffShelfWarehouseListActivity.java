package com.huadu.shunda.ui.expressshelf;

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
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfWarehouseItemInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfWarehouseListResponseInfo;
import com.huadu.shunda.http.ExpressShelfRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressOffShelfWarehouseListActivity extends BaseActivity {

    private static final int REFRESH_WAREHOUSE_DATA_LIST = 100100;

    private ListView lvWarehouseList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_off_shelf_warehouse_list);

        lvWarehouseList = findViewById(R.id.lvWarehouseList);
        lvWarehouseList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(ExpressOffShelfWarehouseListActivity.this, ExpressOffShelfShelfListActivity.class);
                intent.putExtra("warehouseId", (int) map.get("id"));
                intent.putExtra("warehouseCode", (String) map.get("code"));
                intent.putExtra("warehouseName", (String) map.get("name"));

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

                ExpressShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressShelfRequest.class);
                Call<ExpressShelfOffShelfWarehouseListResponseInfo> warehouseListCall = expressShelfRequest.offShelfWarehouseList();
                // Dialog mask = MaskDialog.show(ExpressOffShelfWarehouseListActivity.this);
                warehouseListCall.enqueue(new Callback<ExpressShelfOffShelfWarehouseListResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressShelfOffShelfWarehouseListResponseInfo> call, Response<ExpressShelfOffShelfWarehouseListResponseInfo> response) {
                        // mask.dismiss();

                        ExpressShelfOffShelfWarehouseListResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, ExpressOffShelfWarehouseListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_WAREHOUSE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ExpressShelfOffShelfWarehouseListResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressOffShelfWarehouseListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    ExpressShelfOffShelfWarehouseListResponseInfo warehouseListResponse = (ExpressShelfOffShelfWarehouseListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ExpressShelfOffShelfWarehouseItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("code", item.getCode());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(ExpressOffShelfWarehouseListActivity.this
                                , listDataMap
                                , R.layout.item_express_off_shelf_warehouse_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvWarehouseList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressOffShelfWarehouseListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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