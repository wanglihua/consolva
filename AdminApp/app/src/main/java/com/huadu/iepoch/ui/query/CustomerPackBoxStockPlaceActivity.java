package com.huadu.iepoch.ui.query;

import android.app.Dialog;
import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.customerpackboxstockplace.CustomerPackBoxStockPlaceListResponseInfo;
import com.huadu.iepoch.data.http.customerpackboxstockplace.StockPackBoxCountItem;
import com.huadu.iepoch.http.CustomerPackBoxStockPlaceRequest;
import com.huadu.iepoch.ui.BaseActivity;
import com.huadu.iepoch.ui.pickup.warehouse.WarehousePickupWarehouseDetailActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class CustomerPackBoxStockPlaceActivity extends BaseActivity {

    private static final int REFRESH_STOCK_PACK_BOX_COUNT_DATA_LIST = 100100;

    private AutoCompleteTextView etCustomerCode = null;
    private Button btnQuery = null;
    private ListView lvStockPlacePackBoxCountList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_customer_pack_box_stock_place);

        etCustomerCode = findViewById(R.id.etCustomerCode);
        btnQuery = findViewById(R.id.btnQuery);

        lvStockPlacePackBoxCountList = findViewById(R.id.lvStockPlacePackBoxCountList);

        btnQuery.setOnClickListener(new ButtonQueryClickListener());
    }

    private class ButtonQueryClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            String customerCode = etCustomerCode.getText().toString();

            if ("".equals(customerCode)) {
                Toast.makeText(CustomerPackBoxStockPlaceActivity.this, "会员ID不能为空！", Toast.LENGTH_SHORT).show();
                return;
            }

            // 隐藏输入键盘
            InputMethodManager imm = (InputMethodManager) view.getContext().getSystemService(Context.INPUT_METHOD_SERVICE);
            if (imm != null) {
                imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
            }

            CustomerPackBoxStockPlaceRequest stockPlaceRequest = RetrofitUtil.createRequest(CustomerPackBoxStockPlaceRequest.class);
            Call<CustomerPackBoxStockPlaceListResponseInfo> addCall = stockPlaceRequest.stockPlaceList(customerCode);
            Dialog mask = MaskDialog.show(CustomerPackBoxStockPlaceActivity.this);
            addCall.enqueue(new Callback<CustomerPackBoxStockPlaceListResponseInfo>() {
                @Override
                public void onResponse(Call<CustomerPackBoxStockPlaceListResponseInfo> call, Response<CustomerPackBoxStockPlaceListResponseInfo> response) {
                    mask.dismiss();

                    CustomerPackBoxStockPlaceListResponseInfo respData = response.body();

                    if (!RetrofitUtil.checkResponse(respData, CustomerPackBoxStockPlaceActivity.this)) {
                        return;
                    }

                    SoundUtil.playSuccess(CustomerPackBoxStockPlaceActivity.this);

                    Message msg = new Message();
                    msg.what = REFRESH_STOCK_PACK_BOX_COUNT_DATA_LIST;
                    msg.obj = respData;

                    messageHandler.sendMessage(msg);

                    // tvStockPlaceList.setText(respData.getStockPlaceList());
                }

                @Override
                public void onFailure(Call<CustomerPackBoxStockPlaceListResponseInfo> call, Throwable t) {
                    Toast.makeText(CustomerPackBoxStockPlaceActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        }
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);

            if (msg.what == REFRESH_STOCK_PACK_BOX_COUNT_DATA_LIST) {
                CustomerPackBoxStockPlaceListResponseInfo respData = (CustomerPackBoxStockPlaceListResponseInfo) msg.obj;

                List<StockPackBoxCountItem> stockPackBoxCountItemList = respData.getStockPackBoxCountItemList();

                if (stockPackBoxCountItemList == null) {
                    return;
                }

                List<Map<String, Object>> dataMapList = new ArrayList<>();

                if (stockPackBoxCountItemList.size() == 1 && stockPackBoxCountItemList.get(0).getWarehouseId() == 0) {

                    StockPackBoxCountItem item = stockPackBoxCountItemList.get(0);

                    Map<String, Object> dataMap = new HashMap<>();
                    dataMap.put("seqNum", "");
                    dataMap.put("warehouseName", item.getWarehouseName());
                    dataMap.put("stockPlaceName", "");
                    dataMap.put("orderCode", "");
                    dataMap.put("packBoxCount", "");

                    dataMapList.add(dataMap);
                } else {
                    for (StockPackBoxCountItem item : stockPackBoxCountItemList) {
                        Map<String, Object> dataMap = new HashMap<>();
                        dataMap.put("seqNum", String.valueOf(item.getSeqNum()) + "、");
                        dataMap.put("warehouseName", item.getWarehouseName());
                        dataMap.put("stockPlaceName", item.getStockPlaceName());
                        dataMap.put("orderCode", item.getOrderCode());
                        dataMap.put("packBoxCount", String.valueOf(item.getPackBoxCount()) + "箱");

                        dataMapList.add(dataMap);
                    }
                }

                SimpleAdapter adapter = new SimpleAdapter(CustomerPackBoxStockPlaceActivity.this
                        , dataMapList
                        , R.layout.item_stock_place_pack_box_count_list
                        , new String[]{"seqNum", "warehouseName", "stockPlaceName", "orderCode", "packBoxCount"}
                        , new int[]{R.id.tvSeqNum, R.id.tvWarehouseName, R.id.tvStockPlaceName, R.id.tvOrderCode, R.id.tvPackBoxCount});

                lvStockPlacePackBoxCountList.setAdapter(adapter);
            }
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();

            /*
            Intent intent = new Intent(this, StockPlaceListActivity.class);
            intent.putExtra("shipContainerId", shipContainerId);
            startActivity(intent);
             */
        }

        return true;
    }
}