package com.huadu.shunda.ui.stockcheck;

import android.app.Dialog;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Toast;

import com.huadu.shunda.R;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.stockcheck.StockCheckBillAddResponseInfo;
import com.huadu.shunda.data.http.stockcheck.StockCheckBillListResponseInfo;
import com.huadu.shunda.data.http.stockplace.StockPlaceCheckBillItemInfo;
import com.huadu.shunda.http.StockCheckRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockCheckBillListActivity extends BaseActivity {

    private static final int REFRESH_WAREHOUSE_DATA_LIST = 100100;

    private int warehouseId;

    private ListView lvBillList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_check_bill_list);

        warehouseId = getIntent().getIntExtra("warehouseId", 0);

        lvBillList = findViewById(R.id.lvBillList);
        lvBillList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(StockCheckBillListActivity.this, StockCheckStockPlaceListActivity.class);
                intent.putExtra("billId", (int) map.get("id"));

                startActivity(intent);
            }
        });

        /*

        lvBillList.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                final int billId = (int) map.get("id");
                final String billName = (String) map.get("name");

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(StockCheckBillListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("删除盘点单 " + billName + "吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                        StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                        Call<ResponseInfo> billDeleteCall = stockCheckRequest.billDelete(billId);
                        billDeleteCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                ResponseInfo respData = response.body();

                                if (respData == null) {
                                    Toast.makeText(StockCheckBillListActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                    return;
                                }

                                if (!respData.getSuccess()) {
                                    Toast.makeText(StockCheckBillListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                                    return;
                                }

                                loadBillList();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(StockCheckBillListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                });
                alertDialogBuilder.setNegativeButton("取消", null);

                AlertDialog alertDialog = alertDialogBuilder.create();
                alertDialog.show();

                return true;
            }
        });

        */
    }

    @Override
    public void onStart() {
        super.onStart();

        loadBillList();
    }

    private void loadBillList() {
        new Thread(new Runnable() {
            @Override
            public void run() {

                StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                // Dialog mask = MaskDialog.show(StockCheckBillListActivity.this);
                Call<StockCheckBillListResponseInfo> billListCall = stockCheckRequest.billList(warehouseId);
                billListCall.enqueue(new Callback<StockCheckBillListResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockCheckBillListResponseInfo> call, Response<StockCheckBillListResponseInfo> response) {
                        // mask.dismiss();

                        StockCheckBillListResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, StockCheckBillListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_WAREHOUSE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockCheckBillListResponseInfo> call, Throwable t) {
                        Toast.makeText(StockCheckBillListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });


            }
        }).start();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_stock_check_bill_list, menu);
        MenuItem menuItemCreateStockPlace = menu.findItem(R.id.menu_item_create_stock_check_bill);

        menuItemCreateStockPlace.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {

                StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                Call<StockCheckBillAddResponseInfo> stockPlaceListCall = stockCheckRequest.billAdd(warehouseId);
                Dialog mask = MaskDialog.show(StockCheckBillListActivity.this);
                stockPlaceListCall.enqueue(new Callback<StockCheckBillAddResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockCheckBillAddResponseInfo> call, Response<StockCheckBillAddResponseInfo> response) {
                        mask.dismiss();

                        StockCheckBillAddResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, StockCheckBillListActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(StockCheckBillListActivity.this);

                        loadBillList();
                    }

                    @Override
                    public void onFailure(Call<StockCheckBillAddResponseInfo> call, Throwable t) {
                        Toast.makeText(StockCheckBillListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });

                return true;
            }
        });

        return super.onCreateOptionsMenu(menu);
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_WAREHOUSE_DATA_LIST:

                    StockCheckBillListResponseInfo billListResponse = (StockCheckBillListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (StockPlaceCheckBillItemInfo item : billListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());
                        dataMap.put("status", item.getStatus());

                        listDataMap.add(dataMap);
                    }

                    if (billListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(StockCheckBillListActivity.this
                                , listDataMap
                                , R.layout.item_stock_check_bill_list
                                , new String[]{"name", "status"}
                                , new int[]{R.id.tvName, R.id.tvStatus});

                        lvBillList.setAdapter(adapter);
                    } else {
                        Toast.makeText(StockCheckBillListActivity.this, billListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
