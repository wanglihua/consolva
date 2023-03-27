package com.huadu.shunda.ui.expressshelf;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.huadu.shunda.R;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckListItemInfo;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckListResponseInfo;
import com.huadu.shunda.http.WarehouseCheckRequest;
import com.huadu.shunda.ui.BaseActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressWarehouseCheckListActivity extends BaseActivity {

    private static final int REFRESH_WAREHOUSE_CHECK_LIST = 100100;

    private TextView tvWarehouseCheckCount;
    private ListView lvWarehouseCheckList;

    private int shelfId;
    private String shelfName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_warehouse_check_list);

        Intent intent = getIntent();

        this.shelfId = intent.getIntExtra("shelfId", 0);
        this.shelfName = intent.getStringExtra("shelfName");

        this.tvWarehouseCheckCount = findViewById(R.id.tvWarehouseCheckCount);

        this.lvWarehouseCheckList = findViewById(R.id.lvWarehouseCheckList);
        this.lvWarehouseCheckList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                WarehouseCheckListItemInfo item = (WarehouseCheckListItemInfo) parent.getItemAtPosition(position);
                int warehouseCheckId = item.getId();
                String trackingNo = item.getTrackingNo();

                Intent intent = new Intent(ExpressWarehouseCheckListActivity.this, ExpressWarehouseCheckDetailActivity.class);
                intent.putExtra("warehouseCheckId", warehouseCheckId);
                intent.putExtra("trackingNo", trackingNo);

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
                loadWarehouseCheckList();
            }
        }).start();
    }

    public void loadWarehouseCheckList() {
        WarehouseCheckRequest warehouseCheckRequest = RetrofitUtil.createRequest(WarehouseCheckRequest.class);
        Call<WarehouseCheckListResponseInfo> warehouseCheckListCall = warehouseCheckRequest.list(this.shelfId);
        // Dialog mask = MaskDialog.show(ExpressWarehouseCheckListActivity.this);
        warehouseCheckListCall.enqueue(new Callback<WarehouseCheckListResponseInfo>() {
            @Override
            public void onResponse(Call<WarehouseCheckListResponseInfo> call, Response<WarehouseCheckListResponseInfo> response) {
                // mask.dismiss();

                WarehouseCheckListResponseInfo respData = response.body();

                if (!RetrofitUtil.processResponse(response, ExpressWarehouseCheckListActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_WAREHOUSE_CHECK_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<WarehouseCheckListResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressWarehouseCheckListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_WAREHOUSE_CHECK_LIST:

                    WarehouseCheckListResponseInfo warehouseCheckListResponse = (WarehouseCheckListResponseInfo) msg.obj;

                    if (warehouseCheckListResponse.getSuccess() == false) {
                        Toast.makeText(ExpressWarehouseCheckListActivity.this, warehouseCheckListResponse.getMessage(), Toast.LENGTH_SHORT).show();
                        return;
                    }

                    ExpressWarehouseCheckListActivity.this.tvWarehouseCheckCount.setText("货架 " + shelfName + " 需核实数量：" + warehouseCheckListResponse.getCheckCount());

                    ExpressWarehouseCheckListAdapter adapter = new ExpressWarehouseCheckListAdapter(ExpressWarehouseCheckListActivity.this, warehouseCheckListResponse.getWarehouseCheckList());
                    lvWarehouseCheckList.setAdapter(adapter);

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