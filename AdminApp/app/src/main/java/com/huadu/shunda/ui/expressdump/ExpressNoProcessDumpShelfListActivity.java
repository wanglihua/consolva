package com.huadu.shunda.ui.expressdump;

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

import com.huadu.shunda.R;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.expressdump.ExpressDumpShelfItemInfo;
import com.huadu.shunda.data.http.expressdump.ExpressDumpShelfListResponseInfo;
import com.huadu.shunda.http.ExpressNoProcessDumpRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressNoProcessDumpShelfListActivity extends BaseActivity {

    private static final int REFRESH_SHELF_DATA_LIST = 100100;

    private int warehouseId;
    private String warehouseCode;
    private String warehouseName;

    private TextView tvTitle = null;
    private ListView lvShelfList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_no_process_dump_shelf_list);

        Intent intent = getIntent();

        warehouseId = intent.getIntExtra("warehouseId", 0);
        warehouseCode = intent.getStringExtra("warehouseCode");
        warehouseName = intent.getStringExtra("warehouseName");

        tvTitle = findViewById(R.id.tvTitle);
        tvTitle.setText("长期未出库快递单 " + warehouseName + " 货架列表");

        lvShelfList = findViewById(R.id.lvShelfList);
        lvShelfList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(ExpressNoProcessDumpShelfListActivity.this, ExpressNoProcessDumpTargetShelfListActivity.class);
                intent.putExtra("warehouseId", warehouseId);
                intent.putExtra("warehouseCode", warehouseCode);
                intent.putExtra("warehouseName", warehouseName);
                intent.putExtra("shelfId", (int) map.get("id"));

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

                ExpressNoProcessDumpRequest expressNoProcessDumpRequest = RetrofitUtil.createRequest(ExpressNoProcessDumpRequest.class);
                Call<ExpressDumpShelfListResponseInfo> warehouseListCall = expressNoProcessDumpRequest.shelfList(ExpressNoProcessDumpShelfListActivity.this.warehouseId);
                // Dialog mask = MaskDialog.show(ExpressOffShelfShelfListActivity.this);
                warehouseListCall.enqueue(new Callback<ExpressDumpShelfListResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressDumpShelfListResponseInfo> call, Response<ExpressDumpShelfListResponseInfo> response) {
                        // mask.dismiss();

                        ExpressDumpShelfListResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, ExpressNoProcessDumpShelfListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHELF_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ExpressDumpShelfListResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressNoProcessDumpShelfListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_SHELF_DATA_LIST:

                    ExpressDumpShelfListResponseInfo warehouseListResponse = (ExpressDumpShelfListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ExpressDumpShelfItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(ExpressNoProcessDumpShelfListActivity.this
                                , listDataMap
                                , R.layout.item_express_no_process_dump_shelf_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvShelfList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressNoProcessDumpShelfListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
