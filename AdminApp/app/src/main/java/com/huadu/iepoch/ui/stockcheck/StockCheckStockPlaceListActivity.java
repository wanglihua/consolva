package com.huadu.iepoch.ui.stockcheck;

import android.app.Dialog;
import android.content.DialogInterface;
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

import androidx.appcompat.app.AlertDialog;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.data.http.stockcheck.StockCheckStockPlaceItemInfo;
import com.huadu.iepoch.data.http.stockcheck.StockCheckStockPlaceListResponseInfo;
import com.huadu.iepoch.http.StockCheckRequest;
import com.huadu.iepoch.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockCheckStockPlaceListActivity extends BaseActivity {

    private static final int REFRESH_STOCK_PLACE_DATA_LIST = 100300;

    private int billId;

    private ListView lvStockPlaceList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_move_stock_place_list);

        billId = getIntent().getIntExtra("billId", 0);

        lvStockPlaceList = findViewById(R.id.lvStockPlaceList);
        lvStockPlaceList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(StockCheckStockPlaceListActivity.this, StockCheckStockPlaceDetailActivity.class);
                intent.putExtra("stockPlaceId", (int) map.get("id"));
                startActivity(intent);
            }
        });

        lvStockPlaceList.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                final int stockPlaceId = (int) map.get("id");
                final String stockPlaceName = (String) map.get("name");

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(StockCheckStockPlaceListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("删除托盘将同时删除存放在托盘上的箱号，确认要删除托盘 " + stockPlaceName + "吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {


                        StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                        Call<ResponseInfo> stackPlaceDeleteCall = stockCheckRequest.stockPlaceDelete(stockPlaceId);
                        Dialog mask = MaskDialog.show(StockCheckStockPlaceListActivity.this);
                        stackPlaceDeleteCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (!RetrofitUtil.checkResponse(respData, StockCheckStockPlaceListActivity.this)) {
                                    return;
                                }

                                SoundUtil.playSuccess(StockCheckStockPlaceListActivity.this);
                                loadListData();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(StockCheckStockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
    }

    @Override
    public void onStart() {
        super.onStart();

        loadListData();
    }

    private void loadListData() {
        new Thread(new Runnable() {
            @Override
            public void run() {

                StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                Call<StockCheckStockPlaceListResponseInfo> stockPlaceListCall = stockCheckRequest.stockPlaceList(billId);
                // Dialog mask = MaskDialog.show(StockCheckStockPlaceListActivity.this);
                stockPlaceListCall.enqueue(new Callback<StockCheckStockPlaceListResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockCheckStockPlaceListResponseInfo> call, Response<StockCheckStockPlaceListResponseInfo> response) {
                        // mask.dismiss();

                        StockCheckStockPlaceListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, StockCheckStockPlaceListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_STOCK_PLACE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockCheckStockPlaceListResponseInfo> call, Throwable t) {
                        Toast.makeText(StockCheckStockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });

            }
        }).start();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_stock_check_stock_place_list, menu);
        MenuItem menuItemCreateStockPlace = menu.findItem(R.id.menu_item_create_stock_place);
        MenuItem menuItemBillComplete = menu.findItem(R.id.menu_item_bill_complete);

        menuItemCreateStockPlace.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {

                // Toast.makeText(StockPlaceListActivity.this, "create stock place", Toast.LENGTH_SHORT).show();
                Intent intent = new Intent(StockCheckStockPlaceListActivity.this, StockCheckStockPlaceAddActivity.class);
                intent.putExtra("billId", billId);
                startActivity(intent);

                return true;
            }
        });

        menuItemBillComplete.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(StockCheckStockPlaceListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("确实要完成当前盘点单吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {


                        StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
                        Call<ResponseInfo> billCompleteCall = stockCheckRequest.billComplete(billId);
                        Dialog mask = MaskDialog.show(StockCheckStockPlaceListActivity.this);
                        billCompleteCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (!RetrofitUtil.checkResponse(respData, StockCheckStockPlaceListActivity.this)) {
                                    return;
                                }

                                SoundUtil.playSuccess(StockCheckStockPlaceListActivity.this);
                                Toast.makeText(StockCheckStockPlaceListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(StockCheckStockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

        return super.onCreateOptionsMenu(menu);
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_STOCK_PLACE_DATA_LIST:

                    StockCheckStockPlaceListResponseInfo listDataResponse = (StockCheckStockPlaceListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (StockCheckStockPlaceItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(StockCheckStockPlaceListActivity.this
                                , listDataMap
                                , R.layout.item_stock_check_stock_place_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvStockPlaceList.setAdapter(adapter);
                    } else {
                        Toast.makeText(StockCheckStockPlaceListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
            Intent intent = new Intent(this, PackBoxUnloadShipContainerInnerListActivity.class);
            startActivity(intent);
             */
        }

        return true;
    }
}


