package com.huadu.iepoch.ui.unload;

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
import com.huadu.iepoch.data.http.stockplace.StockPlaceItemInfo;
import com.huadu.iepoch.data.http.stockplace.StockPlaceListDataResponseInfo;
import com.huadu.iepoch.http.ShipContainerInnerRequest;
import com.huadu.iepoch.http.StockPlaceRequest;
import com.huadu.iepoch.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockPlaceListActivity extends BaseActivity {

    private static final int REFRESH_STOCK_PLACE_DATA_LIST = 100300;

    private int shipContainerId;

    private ListView lvStockPlaceList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_place_list);

        shipContainerId = getIntent().getIntExtra("shipContainerId", 0);

        lvStockPlaceList = findViewById(R.id.lvStockPlaceList);
        lvStockPlaceList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(StockPlaceListActivity.this, StockPlaceDetailActivity.class);
                intent.putExtra("stockPlaceId", (int) map.get("id"));
                intent.putExtra("shipContainerId", shipContainerId);
                startActivity(intent);
            }
        });

        lvStockPlaceList.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                final int stockPlaceId = (int) map.get("id");
                final String stockPlaceName = (String) map.get("name");

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(StockPlaceListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("删除托盘将同时删除存放在托盘上的箱号，确认要删除托盘 " + stockPlaceName + "吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {


                        StockPlaceRequest stockPlaceRequest = RetrofitUtil.createRequest(StockPlaceRequest.class);
                        Call<ResponseInfo> stackPlaceDeleteCall = stockPlaceRequest.delete(stockPlaceId);
                        Dialog mask = MaskDialog.show(StockPlaceListActivity.this);
                        stackPlaceDeleteCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (!RetrofitUtil.checkResponse(respData, StockPlaceListActivity.this)) {
                                    return;
                                }

                                SoundUtil.playSuccess(StockPlaceListActivity.this);

                                loadListData();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(StockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                StockPlaceRequest stockPlaceRequest = RetrofitUtil.createRequest(StockPlaceRequest.class);
                Call<StockPlaceListDataResponseInfo> listDataCall = stockPlaceRequest.listData(shipContainerId);
                // Dialog mask = MaskDialog.show(StockPlaceListActivity.this);
                listDataCall.enqueue(new Callback<StockPlaceListDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockPlaceListDataResponseInfo> call, Response<StockPlaceListDataResponseInfo> response) {
                        // mask.dismiss();

                        StockPlaceListDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, StockPlaceListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_STOCK_PLACE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockPlaceListDataResponseInfo> call, Throwable t) {
                        Toast.makeText(StockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });

            }
        }).start();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_unload_stock_place_list, menu);

        // 菜单 创建托盘
        MenuItem menuItemCreateStockPlace = menu.findItem(R.id.menu_item_create_stock_place);

        menuItemCreateStockPlace.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {

                // Toast.makeText(StockPlaceListActivity.this, "create stock place", Toast.LENGTH_SHORT).show();
                Intent intent = new Intent(StockPlaceListActivity.this, StockPlaceAddActivity.class);
                intent.putExtra("shipContainerId", shipContainerId);
                startActivity(intent);

                return true;
            }
        });

        // 菜单 完成卸货
        MenuItem menuItemUnloadComplete = menu.findItem(R.id.menu_item_unload_complete);

        menuItemUnloadComplete.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(StockPlaceListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("确认要完成卸货吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                        ShipContainerInnerRequest shipContainerInnerRequest = RetrofitUtil.createRequest(ShipContainerInnerRequest.class);
                        Call<ResponseInfo> unloadCompleteCall = shipContainerInnerRequest.complete(shipContainerId);
                        Dialog mask = MaskDialog.show(StockPlaceListActivity.this);
                        unloadCompleteCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (respData == null) {

                                    SoundUtil.playError(StockPlaceListActivity.this);

                                    // Toast.makeText(StockPlaceListActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                    AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(StockPlaceListActivity.this);
                                    alertDialogBuilder.setTitle("错误提示");
                                    alertDialogBuilder.setMessage("未能接收到服务器返回数据！");
                                    alertDialogBuilder.setPositiveButton("确定", null);

                                    AlertDialog alertDialog = alertDialogBuilder.create();
                                    alertDialog.show();

                                    return;
                                }

                                SoundUtil.playSuccess(StockPlaceListActivity.this);

                                Toast.makeText(StockPlaceListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                                return;
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(StockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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

                    StockPlaceListDataResponseInfo listDataResponse = (StockPlaceListDataResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (StockPlaceItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(StockPlaceListActivity.this
                                , listDataMap
                                , R.layout.item_unload_stock_place_list
                                , new String[]{"name"}
                                , new int[]{R.id.tvName});

                        lvStockPlaceList.setAdapter(adapter);
                    } else {
                        Toast.makeText(StockPlaceListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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


