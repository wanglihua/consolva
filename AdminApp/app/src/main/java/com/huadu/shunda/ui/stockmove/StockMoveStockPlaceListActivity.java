package com.huadu.shunda.ui.stockmove;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.KeyEvent;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.widget.AdapterView;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import com.huadu.shunda.R;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.stockmove.StockMoveStockPlaceItemInfo;
import com.huadu.shunda.data.http.stockmove.StockMoveStockPlaceListResponseInfo;
import com.huadu.shunda.http.StockMoveRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockMoveStockPlaceListActivity extends BaseActivity {

    private static final int REFRESH_STOCK_PLACE_DATA_LIST = 100300;

    private int warehouseId;

    private EditText etNameQuery = null;
    private ListView lvStockPlaceList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_move_stock_place_list);

        warehouseId = getIntent().getIntExtra("warehouseId", 0);

        lvStockPlaceList = findViewById(R.id.lvStockPlaceList);
        lvStockPlaceList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(StockMoveStockPlaceListActivity.this, StockMoveStockPlaceDetailActivity.class);
                intent.putExtra("stockPlaceId", (int) map.get("id"));
                startActivity(intent);
            }
        });

        etNameQuery = findViewById(R.id.etNameQuery);
        etNameQuery.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {
                // loadListData();
                return false;
            }
        });

        etNameQuery.setOnEditorActionListener(new TextView.OnEditorActionListener(){
            @Override
            public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {

                if(actionId == EditorInfo.IME_ACTION_SEARCH) {
                    loadListData();
                }

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

                String nameQuery = etNameQuery.getText().toString();
                StockMoveRequest stockMoveRequest = RetrofitUtil.createRequest(StockMoveRequest.class);
                Call<StockMoveStockPlaceListResponseInfo> stockPlaceListCall = stockMoveRequest.stockPlaceList(warehouseId, nameQuery);
                // Dialog mask = MaskDialog.show(StockMoveStockPlaceListActivity.this);
                stockPlaceListCall.enqueue(new Callback<StockMoveStockPlaceListResponseInfo>() {
                    @Override
                    public void onResponse(Call<StockMoveStockPlaceListResponseInfo> call, Response<StockMoveStockPlaceListResponseInfo> response) {
                        // mask.dismiss();

                        StockMoveStockPlaceListResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, StockMoveStockPlaceListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_STOCK_PLACE_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<StockMoveStockPlaceListResponseInfo> call, Throwable t) {
                        Toast.makeText(StockMoveStockPlaceListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_STOCK_PLACE_DATA_LIST:

                    StockMoveStockPlaceListResponseInfo listDataResponse = (StockMoveStockPlaceListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (StockMoveStockPlaceItemInfo item : listDataResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());
                        dataMap.put("createTime", item.getCreateTime());

                        listDataMap.add(dataMap);
                    }

                    if (listDataResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(StockMoveStockPlaceListActivity.this
                                , listDataMap
                                , R.layout.item_stock_move_stock_place_list
                                , new String[]{"name", "createTime"}
                                , new int[]{R.id.tvName, R.id.tvCreateTime});

                        lvStockPlaceList.setAdapter(adapter);
                    } else {
                        Toast.makeText(StockMoveStockPlaceListActivity.this, listDataResponse.getMessage(), Toast.LENGTH_SHORT).show();
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


