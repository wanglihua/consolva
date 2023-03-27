package com.huadu.shunda.ui.unload;

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
import com.huadu.shunda.data.http.pallet.PalletItemInfo;
import com.huadu.shunda.data.http.pallet.PalletListDataResponseInfo;
import com.huadu.shunda.http.PalletRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PalletListActivity extends BaseActivity {

    private static final int REFRESH_PALLET_DATA_LIST = 100200;

    private int warehouseId;

    private ListView lvPalletList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pallet_list);

        warehouseId = getIntent().getIntExtra("warehouseId", 0);

        lvPalletList = findViewById(R.id.lvPalletList);
        lvPalletList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                Toast.makeText(PalletListActivity.this, "Hello!", Toast.LENGTH_SHORT).show();

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                /*

                Intent intent = new Intent(PalletListActivity.this, PalletListActivity.class);
                //intent.putExtra("machine_code", map.get("machine_code").toString());
                startActivity(intent);

                 */
            }
        });
    }

    @Override
    public void onStart() {
        super.onStart();

        new Thread(new Runnable() {
            @Override
            public void run() {

                PalletRequest palletRequest = RetrofitUtil.createRequest(PalletRequest.class);
                Call<PalletListDataResponseInfo> listDataCall = palletRequest.listData(warehouseId);
                // Dialog mask = MaskDialog.show(PalletListActivity.this);
                listDataCall.enqueue(new Callback<PalletListDataResponseInfo>() {
                    @Override
                    public void onResponse(Call<PalletListDataResponseInfo> call, Response<PalletListDataResponseInfo> response) {
                        // mask.dismiss();

                        PalletListDataResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, PalletListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_PALLET_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<PalletListDataResponseInfo> call, Throwable t) {
                        Toast.makeText(PalletListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_PALLET_DATA_LIST:

                    PalletListDataResponseInfo listDataResponse = (PalletListDataResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (PalletItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("codeAndName", item.getCode() + " " + item.getName());
                        dataMap.put("hasGoods", "是否有货：" + item.getHasGoods());
                        dataMap.put("statusName", "状态：" + item.getStatusName());
                        dataMap.put("warehouseName", "仓库：" + item.getWarehouseName());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(PalletListActivity.this
                                , listDataMap
                                , R.layout.item_pallet_list
                                , new String[]{"codeAndName", "hasGoods", "statusName", "warehouseName"}
                                , new int[]{R.id.tvCodeAndName, R.id.tvHasGoods, R.id.tvStatusName, R.id.tvWarehouseName});

                        lvPalletList.setAdapter(adapter);
                    } else {
                        Toast.makeText(PalletListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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
