package com.huadu.shunda.ui.expressshelf;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.huadu.shunda.R;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.expressoffshelf.ExpressOffShelfExpressListItem;
import com.huadu.shunda.data.http.expressoffshelf.ExpressOffShelfExpressListResponseData;
import com.huadu.shunda.http.ExpressOffShelfRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.experssimageshow.ExpressImageListActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressOffShelfExpressListActivity extends BaseActivity {

    private static final int REFRESH_EXPRESS_DATA_LIST = 100100;
    private static final int REFRESH_EXPRESS_IMAGE_LIST = 100200;

    private static final int REQ_QR_CODE_TRACKING_NO = 1;

    private int shelfId;
    private String shelfName;

    private TextView tvShelfName;
    private Button btnScan;
    private TextView tvNotOffShelfListLabel;
    private ListView lvExpressList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_off_shelf_express_list);

        Intent intent = getIntent();

        shelfId = intent.getIntExtra("shelfId", 0);
        shelfName = intent.getStringExtra("shelfName");

        tvShelfName = findViewById(R.id.tvShelfName);
        tvShelfName.setText("货架：" + shelfName);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                ExpressOffShelfScanUtil.startScanTrackingNo(REQ_QR_CODE_TRACKING_NO, shelfId, ExpressOffShelfExpressListActivity.this);
            }
        });

        tvNotOffShelfListLabel = findViewById(R.id.tvNotOffShelfListLabel);

        lvExpressList = findViewById(R.id.lvExpressList);
        lvExpressList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                /*
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);

                Intent intent = new Intent(ExpressOffShelfExpressListActivity.this, ExpressImageListActivity.class);
                intent.putExtra("trackingNo", (String) (map.get("trackingNo")));

                startActivity(intent);
                 */

                ExpressOffShelfExpressListItem map = (ExpressOffShelfExpressListItem) parent.getItemAtPosition(position);

                Intent intent = new Intent(ExpressOffShelfExpressListActivity.this, ExpressImageListActivity.class);
                intent.putExtra("trackingNo", map.getTrackingNo());

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
                loadExpressList();
            }
        }).start();
    }

    public void loadExpressList() {
        ExpressOffShelfRequest expressOffShelfRequest = RetrofitUtil.createRequest(ExpressOffShelfRequest.class);
        Call<ResponseDataInfo<ExpressOffShelfExpressListResponseData>> warehouseListCall = expressOffShelfRequest.expressList(ExpressOffShelfExpressListActivity.this.shelfId);
        // Dialog mask = MaskDialog.show(ExpressOffShelfExpressListActivity.this);
        warehouseListCall.enqueue(new Callback<ResponseDataInfo<ExpressOffShelfExpressListResponseData>>() {
            @Override
            public void onResponse(Call<ResponseDataInfo<ExpressOffShelfExpressListResponseData>> call, Response<ResponseDataInfo<ExpressOffShelfExpressListResponseData>> response) {
                // mask.dismiss();

                ResponseDataInfo<ExpressOffShelfExpressListResponseData> respData = response.body();

                if (!RetrofitUtil.processResponse(response, ExpressOffShelfExpressListActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_EXPRESS_DATA_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<ResponseDataInfo<ExpressOffShelfExpressListResponseData>> call, Throwable t) {
                Toast.makeText(ExpressOffShelfExpressListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            String trackingNo = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_TRACKING_NO) {
                ExpressOffShelfScanUtil.finishScanTrackingNo(REQ_QR_CODE_TRACKING_NO, shelfId, trackingNo, ExpressOffShelfExpressListActivity.this, true);
            }
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            // 获得授权
            ExpressOffShelfScanUtil.startScanTrackingNo(REQ_QR_CODE_TRACKING_NO, shelfId, ExpressOffShelfExpressListActivity.this);
        } else {
            // 被禁止授权
            Toast.makeText(this, "请至权限中心打开本应用的相机访问权限", Toast.LENGTH_LONG).show();
        }
    }

    private MessageHandler messageHandler = new MessageHandler();

    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case REFRESH_EXPRESS_DATA_LIST:

                    ResponseDataInfo<ExpressOffShelfExpressListResponseData> expressListResponse = (ResponseDataInfo<ExpressOffShelfExpressListResponseData>) msg.obj;

                    if (expressListResponse.getSuccess()) {
                        /*
                        List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                        for (ExpressShelfOffShelfExpressItemInfo item : expressListResponse.getItemList()) {
                            Map<String, Object> dataMap = new HashMap<String, Object>();
                            dataMap.put("id", item.getId());
                            dataMap.put("trackingNo", item.getTrackingNo());
                            dataMap.put("aoPinTrackingNo", "舜达入库号：" + item.getAoPinTrackingNo());
                            dataMap.put("customerCode", "客户ID：" + item.getCustomerCode());
                            dataMap.put("packageCount", "包裹数量：" + item.getPackageCount());
                            dataMap.put("storageLocName", "仓位：" + item.getStorageLocName());
                            dataMap.put("addUserName", item.getAddUserName()); // 这个自己有标签
                            dataMap.put("addTime", item.getAddTime()); // 这个自己有标签

                            listDataMap.add(dataMap);
                        }

                        // 待下架快递列表：
                        tvNotOffShelfListLabel.setText("待下架快递列表 (" + expressListResponse.getItemCount() + ")：");

                        SimpleAdapter adapter = new SimpleAdapter(ExpressOffShelfExpressListActivity.this
                                , listDataMap
                                , R.layout.item_express_off_shelf_express_list
                                , new String[]{"trackingNo", "aoPinTrackingNo", "customerCode", "packageCount", "storageLocName", "addUserName", "addTime"}
                                , new int[]{R.id.tvTrackingNo, R.id.tvAoPinTrackingNo, R.id.tvCustomerCode, R.id.tvPackageCount, R.id.tvStorageLocName, R.id.tvAddUserName, R.id.tvAddTime});

                        lvExpressList.setAdapter(adapter);
                         */

                        ExpressOffShelfExpressListAdapter adapter = new ExpressOffShelfExpressListAdapter(ExpressOffShelfExpressListActivity.this, expressListResponse.getData().getItemList());
                        lvExpressList.setAdapter(adapter);
                    } else {
                        Toast.makeText(ExpressOffShelfExpressListActivity.this, expressListResponse.getMessage(), Toast.LENGTH_SHORT).show();
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