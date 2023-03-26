package com.huadu.iepoch.ui.expressdump;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpStorageLocItemInfo;
import com.huadu.iepoch.data.http.expressdump.ExpressDumpStorageLocListResponseInfo;
import com.huadu.iepoch.http.ExpressNoPickupDumpRequest;
import com.huadu.iepoch.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressNoPickupDumpTargetStorageLocListActivity extends BaseActivity {

    private static final int REFRESH_SHELF_DATA_LIST = 100100;

    private int shelfId;
    private int targetShelfId;

    private TextView tvTitle = null;
    private Button btnNewStorageLoc = null;
    private ListView lvStorageLocList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_no_pickup_dump_target_storage_loc_list);

        Intent intent = getIntent();

        shelfId = intent.getIntExtra("shelfId", 0);
        targetShelfId = intent.getIntExtra("targetShelfId", 0);

        tvTitle = findViewById(R.id.tvTitle);
        //  tvTitle.setText("长期未认领快递堆存 " + warehouseName + " 选择堆存目的货架");

        btnNewStorageLoc = findViewById(R.id.btnNewStorageLoc);
        btnNewStorageLoc.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                ExpressNoPickupDumpRequest expressNoPickupDumpRequest = RetrofitUtil.createRequest(ExpressNoPickupDumpRequest.class);
                Call<ResponseInfo> storageLocListCall = expressNoPickupDumpRequest.newStorageLoc(ExpressNoPickupDumpTargetStorageLocListActivity.this.targetShelfId);
                // Dialog mask = MaskDialog.show(ExpressOffShelfShelfListActivity.this);
                storageLocListCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        // mask.dismiss();

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, ExpressNoPickupDumpTargetStorageLocListActivity.this)) {
                            return;
                        }

                        Toast.makeText(ExpressNoPickupDumpTargetStorageLocListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        loadStorageLocList();
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressNoPickupDumpTargetStorageLocListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        });

        lvStorageLocList = findViewById(R.id.lvStorageLocList);
        lvStorageLocList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(ExpressNoPickupDumpTargetStorageLocListActivity.this, ExpressNoPickupDumpExpressListActivity.class);
                intent.putExtra("targetStorageLocId", (int) map.get("id"));
                intent.putExtra("targetStorageLocName", (String) map.get("name"));
                intent.putExtra("shelfId", shelfId);

                startActivity(intent);
            }
        });
    }

    @Override
    public void onStart() {
        super.onStart();
        loadStorageLocList();
    }

    private void loadStorageLocList() {
        new Thread(new Runnable() {
            @Override
            public void run() {

                ExpressNoPickupDumpRequest expressNoPickupDumpRequest = RetrofitUtil.createRequest(ExpressNoPickupDumpRequest.class);
                Call<ExpressDumpStorageLocListResponseInfo> storageLocListCall = expressNoPickupDumpRequest.targetStorageLocList(ExpressNoPickupDumpTargetStorageLocListActivity.this.targetShelfId);
                // Dialog mask = MaskDialog.show(ExpressOffShelfShelfListActivity.this);
                storageLocListCall.enqueue(new Callback<ExpressDumpStorageLocListResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressDumpStorageLocListResponseInfo> call, Response<ExpressDumpStorageLocListResponseInfo> response) {
                        // mask.dismiss();

                        ExpressDumpStorageLocListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, ExpressNoPickupDumpTargetStorageLocListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHELF_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ExpressDumpStorageLocListResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressNoPickupDumpTargetStorageLocListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    ExpressDumpStorageLocListResponseInfo storageLocListResponse = (ExpressDumpStorageLocListResponseInfo) msg.obj;

                    tvTitle.setText("长期未认领快递 " + storageLocListResponse.getWarehouseName() + " " + storageLocListResponse.getShelfName() + " 选择堆存目的仓位：");


                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ExpressDumpStorageLocItemInfo item : storageLocListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (storageLocListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(ExpressNoPickupDumpTargetStorageLocListActivity.this
                                , listDataMap
                                , R.layout.item_express_no_pickup_dump_target_storage_loc_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvStorageLocList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressNoPickupDumpTargetStorageLocListActivity.this, storageLocListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
