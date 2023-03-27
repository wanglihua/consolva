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
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckShelfListItemInfo;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckShelfListResponseInfo;
import com.huadu.shunda.http.WarehouseCheckRequest;
import com.huadu.shunda.ui.BaseActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressWarehouseCheckShelfListActivity extends BaseActivity {

    private static final int REFRESH_SHELF_LIST = 100100;

    private TextView tvWarehouseCheckCount = null;
    private ListView lvWarehouseCheckShelfList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_warehouse_check_shelf_list);

        this.tvWarehouseCheckCount = findViewById(R.id.tvWarehouseCheckCount);
        this.lvWarehouseCheckShelfList = findViewById(R.id.lvWarehouseCheckShelfList);
        this.lvWarehouseCheckShelfList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                WarehouseCheckShelfListItemInfo item = (WarehouseCheckShelfListItemInfo) parent.getItemAtPosition(position);
                int shelfId = item.getShelfId();
                String shelfName = item.getShelfName();

                Intent intent = new Intent(ExpressWarehouseCheckShelfListActivity.this, ExpressWarehouseCheckListActivity.class);
                intent.putExtra("shelfId", shelfId);
                intent.putExtra("shelfName", shelfName);

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
                loadWarehouseCheckShelfList();
            }
        }).start();
    }

    public void loadWarehouseCheckShelfList() {
        WarehouseCheckRequest warehouseCheckRequest = RetrofitUtil.createRequest(WarehouseCheckRequest.class);
        Call<WarehouseCheckShelfListResponseInfo> warehouseCheckListCall = warehouseCheckRequest.shelfList();
        // Dialog mask = MaskDialog.show(ExpressWarehouseCheckShelfListActivity.this);
        warehouseCheckListCall.enqueue(new Callback<WarehouseCheckShelfListResponseInfo>() {
            @Override
            public void onResponse(Call<WarehouseCheckShelfListResponseInfo> call, Response<WarehouseCheckShelfListResponseInfo> response) {
                // mask.dismiss();

                WarehouseCheckShelfListResponseInfo respData = response.body();

                if (!RetrofitUtil.processResponse(response, ExpressWarehouseCheckShelfListActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_SHELF_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<WarehouseCheckShelfListResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressWarehouseCheckShelfListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_SHELF_LIST:

                    WarehouseCheckShelfListResponseInfo shelfListResponse = (WarehouseCheckShelfListResponseInfo) msg.obj;

                    if (shelfListResponse.getSuccess() == false) {
                        Toast.makeText(ExpressWarehouseCheckShelfListActivity.this, shelfListResponse.getMessage(), Toast.LENGTH_SHORT).show();
                        return;
                    }

                    ExpressWarehouseCheckShelfListAdapter adapter = new ExpressWarehouseCheckShelfListAdapter(ExpressWarehouseCheckShelfListActivity.this, shelfListResponse.getShelfList());
                    lvWarehouseCheckShelfList.setAdapter(adapter);

                    tvWarehouseCheckCount.setText("仓库需核实总数量：" + shelfListResponse.getTotalCheckCount());

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