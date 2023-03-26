package com.huadu.shunda.ui.expressshelf;

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
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SettingUtil;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfShelfItemInfo;
import com.huadu.shunda.data.http.expressshelf.ExpressShelfOffShelfShelfListResponseInfo;
import com.huadu.shunda.http.ExpressShelfRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressOffShelfShelfListActivity extends BaseActivity {

    private static final int REFRESH_SHELF_DATA_LIST = 100100;

    private static final int REQ_QR_CODE_TRACKING_NO = 1;

    private static int currentShelfId = 0;

    private int warehouseId;
    private String warehouseCode;
    private String warehouseName;

    private ListView lvShelfList = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_off_shelf_shelf_list);

        Intent intent = getIntent();

        warehouseId = intent.getIntExtra("warehouseId", 0);
        warehouseCode = intent.getStringExtra("warehouseCode");
        warehouseName = intent.getStringExtra("warehouseName");

        lvShelfList = findViewById(R.id.lvShelfList);
        lvShelfList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                int shelfId = (int) map.get("id");
                String shelfName = (String) map.get("name");

                ExpressOffShelfShelfListActivity.currentShelfId = shelfId; // 设置当前货架

                boolean shouldSkipExpressList = SettingUtil.getExpressOffShelfShouldSkipExpressList();
                if (shouldSkipExpressList) {
                    ExpressOffShelfScanUtil.startScanTrackingNo(REQ_QR_CODE_TRACKING_NO, shelfId, ExpressOffShelfShelfListActivity.this);
                } else {
                    Intent intent = new Intent(ExpressOffShelfShelfListActivity.this, ExpressOffShelfExpressListActivity.class);
                    intent.putExtra("shelfId", shelfId);
                    intent.putExtra("shelfName", shelfName);

                    startActivity(intent);
                }
            }
        });
    }

    @Override
    public void onStart() {
        super.onStart();

        new Thread(new Runnable() {
            @Override
            public void run() {

                ExpressShelfRequest expressShelfRequest = RetrofitUtil.createRequest(ExpressShelfRequest.class);
                Call<ExpressShelfOffShelfShelfListResponseInfo> warehouseListCall = expressShelfRequest.offShelfShelfList(ExpressOffShelfShelfListActivity.this.warehouseId);
                // Dialog mask = MaskDialog.show(ExpressOffShelfShelfListActivity.this);
                warehouseListCall.enqueue(new Callback<ExpressShelfOffShelfShelfListResponseInfo>() {
                    @Override
                    public void onResponse(Call<ExpressShelfOffShelfShelfListResponseInfo> call, Response<ExpressShelfOffShelfShelfListResponseInfo> response) {
                        // mask.dismiss();

                        ExpressShelfOffShelfShelfListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, ExpressOffShelfShelfListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_SHELF_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<ExpressShelfOffShelfShelfListResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressOffShelfShelfListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });


            }
        }).start();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String trackingNo = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_TRACKING_NO) {
                ExpressOffShelfScanUtil.finishScanTrackingNo(REQ_QR_CODE_TRACKING_NO, ExpressOffShelfShelfListActivity.currentShelfId, trackingNo, ExpressOffShelfShelfListActivity.this, false);
            }
        }
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_SHELF_DATA_LIST:

                    ExpressShelfOffShelfShelfListResponseInfo warehouseListResponse = (ExpressShelfOffShelfShelfListResponseInfo) msg.obj;

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (ExpressShelfOffShelfShelfItemInfo item : warehouseListResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("name", item.getName());
                        dataMap.put("nameExpressCount", item.getName() + " (" + item.getExpressCount() + ")");

                        listDataMap.add(dataMap);
                    }

                    if (warehouseListResponse.getSuccess()) {
                        SimpleAdapter adapter = new SimpleAdapter(ExpressOffShelfShelfListActivity.this
                                , listDataMap
                                , R.layout.item_express_off_shelf_shelf_list
                                , new String[]{"nameExpressCount"}
                                , new int[]{R.id.tvName});

                        lvShelfList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressOffShelfShelfListActivity.this, warehouseListResponse.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                    break;
                default:
                    break;
            }
        }
    }

    /**
     * 菜单项ID
     */
    private static final int MENU_ITEM_ID_SHOULD_SKIP_EXPRESS_LIST = 0x11b;

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuItem menuItem = menu.add(0, MENU_ITEM_ID_SHOULD_SKIP_EXPRESS_LIST, 0, "是否跳过快递单列表");
        menuItem.setCheckable(true);

        // 读取配置设置是否选中
        boolean shouldExpressList = SettingUtil.getExpressOffShelfShouldSkipExpressList();
        menuItem.setChecked(shouldExpressList);

        return true;
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

        if (item.getItemId() == MENU_ITEM_ID_SHOULD_SKIP_EXPRESS_LIST) {
            boolean shouldExpressList = SettingUtil.getExpressOffShelfShouldSkipExpressList();
            SettingUtil.saveExpressOffShelfShouldSkipExpressList(!shouldExpressList);
            item.setChecked(!shouldExpressList);
        }

        return true;
    }
}
