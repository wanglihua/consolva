package com.huadu.shunda.ui.unload;

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
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.shipcontainerinner.ShipContainerInnerListItemInfo;
import com.huadu.shunda.data.http.shipcontainerinner.ShipContainerInnerUnloadListDataResponseInfo;
import com.huadu.shunda.http.ShipContainerInnerRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PackBoxUnloadShipContainerInnerListActivity extends BaseActivity {

    private static final int REFRESH_SHIP_CONTAINER_DATA_LIST = 100100;

    private ListView lvShipContainerInnerList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pack_box_unload_ship_container_inner_list);

        lvShipContainerInnerList = findViewById(R.id.lvShipContainerInnerList);
        lvShipContainerInnerList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(PackBoxUnloadShipContainerInnerListActivity.this, StockPlaceListActivity.class);
                intent.putExtra("shipContainerId", (int) map.get("id"));
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

                ShipContainerInnerRequest shipContainerInnerRequest = RetrofitUtil.createRequest(ShipContainerInnerRequest.class);
                Call<ShipContainerInnerUnloadListDataResponseInfo> listDataCall = shipContainerInnerRequest.unloadListData(LoginCache.getUserId());
                // Dialog mask = MaskDialog.show(PackBoxUnloadShipContainerInnerListActivity.this);
                listDataCall.enqueue(new Callback<ShipContainerInnerUnloadListDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<ShipContainerInnerUnloadListDataResponseInfo> call, Response<ShipContainerInnerUnloadListDataResponseInfo> response) {
                        // mask.dismiss();

                        ShipContainerInnerUnloadListDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, PackBoxUnloadShipContainerInnerListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHIP_CONTAINER_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ShipContainerInnerUnloadListDataResponseInfo> call, Throwable t) {
                        Toast.makeText(PackBoxUnloadShipContainerInnerListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_SHIP_CONTAINER_DATA_LIST:

                    ShipContainerInnerUnloadListDataResponseInfo listDataResponse = (ShipContainerInnerUnloadListDataResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ShipContainerInnerListItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("code", item.getCode());
                        dataMap.put("goodsTypeName", "物品类型：" + item.getGoodsTypeName());
                        dataMap.put("containerCode", "实际箱号：" + item.getContainerCode());
                        dataMap.put("toWarehouseId", item.getToWarehouseId());
                        dataMap.put("toWarehouseName", "目的仓：" + item.getToWarehouseName());
                        dataMap.put("actualArriveTime", "实际到港时间：" + item.getActualArriveTime());
                        dataMap.put("actualCheckinTime", "实际到仓时间：" + item.getActualCheckinTime());
                        dataMap.put("containerVolume", "集装箱体积：" + item.getContainerVolume() + "立方");
                        dataMap.put("actualImportVolume", "导入体积：" + item.getActualImportVolume() + "立方");

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(PackBoxUnloadShipContainerInnerListActivity.this
                                , listDataMap
                                , R.layout.item_pack_box_unload_ship_container_inner_list
                                , new String[]{"code", "goodsTypeName", "containerCode", "toWarehouseName", "actualArriveTime", "actualCheckinTime", "containerVolume", "actualImportVolume"}
                                , new int[]{R.id.tvCode, R.id.tvGoodsTypeName, R.id.tvContainerCode, R.id.tvToWarehouseName, R.id.tvActualArriveTime, R.id.tvActualCheckinTime, R.id.tvContainerVolume, R.id.tvActualImportVolume});

                        lvShipContainerInnerList.setAdapter(adapter);
                    } else {
                        Toast.makeText(PackBoxUnloadShipContainerInnerListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
