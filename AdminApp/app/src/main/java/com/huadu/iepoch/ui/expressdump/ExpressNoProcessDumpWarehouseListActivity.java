package com.huadu.iepoch.ui.expressdump;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpWarehouseItemInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpWarehouseListResponseInfo;
import com.huadu.iepoch.http.ExpressNoProcessDumpRequest;
import com.huadu.iepoch.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressNoProcessDumpWarehouseListActivity extends BaseActivity {

    private static final int REFRESH_WAREHOUSE_DATA_LIST = 100100;

    private TextView tvTitle = null;
    private ListView lvWarehouseList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_no_process_dump_warehouse_list);

        tvTitle = findViewById(R.id.tvTitle);
        tvTitle.setText("长期未出库快递 仓库列表");

        lvWarehouseList = findViewById(R.id.lvWarehouseList);
        lvWarehouseList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(ExpressNoProcessDumpWarehouseListActivity.this, ExpressNoProcessDumpShelfListActivity.class);
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

                ExpressNoProcessDumpRequest warehouseProcessRequest = RetrofitUtil.createRequest(ExpressNoProcessDumpRequest.class);
                Call<ExpressDumpWarehouseListResponseInfo> warehouseListCall = warehouseProcessRequest.warehouseList();
                // Dialog mask = MaskDialog.show(ExpressOffShelfWarehouseListActivity.this);
                warehouseListCall.enqueue(new Callback<ExpressDumpWarehouseListResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressDumpWarehouseListResponseInfo> call, Response<ExpressDumpWarehouseListResponseInfo> response) {
                        // mask.dismiss();

                        ExpressDumpWarehouseListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, ExpressNoProcessDumpWarehouseListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_WAREHOUSE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ExpressDumpWarehouseListResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressNoProcessDumpWarehouseListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    ExpressDumpWarehouseListResponseInfo warehouseListResponse = (ExpressDumpWarehouseListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ExpressDumpWarehouseItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("code", item.getCode());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(ExpressNoProcessDumpWarehouseListActivity.this
                                , listDataMap
                                , R.layout.item_express_no_process_dump_warehouse_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvWarehouseList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressNoProcessDumpWarehouseListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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