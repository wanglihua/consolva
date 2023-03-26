package com.huadu.shunda.ui.pickup.homedeliverbatch;

import android.Manifest;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Message;
import android.provider.MediaStore;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import com.alibaba.sdk.android.oss.ClientException;
import com.alibaba.sdk.android.oss.OSS;
import com.alibaba.sdk.android.oss.OSSClient;
import com.alibaba.sdk.android.oss.ServiceException;
import com.alibaba.sdk.android.oss.callback.OSSCompletedCallback;
import com.alibaba.sdk.android.oss.callback.OSSProgressCallback;
import com.alibaba.sdk.android.oss.common.auth.OSSStsTokenCredentialProvider;
import com.alibaba.sdk.android.oss.internal.OSSAsyncTask;
import com.alibaba.sdk.android.oss.model.ObjectMetadata;
import com.alibaba.sdk.android.oss.model.PutObjectRequest;
import com.alibaba.sdk.android.oss.model.PutObjectResult;
import com.huadu.shunda.R;
import com.huadu.shunda.core.ClientTimeUtil;
import com.huadu.shunda.core.Config;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PhotoUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.domain.OrderPhotoType;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.homedeliverbatch.HomeDeliverBatchEntryItemInfo;
import com.huadu.shunda.data.http.homedeliverbatch.HomeDeliverBatchEntryListResponseInfo;
import com.huadu.shunda.http.HomeDeliverBatchRequest;
import com.huadu.shunda.http.LogisticsOrderPhotoRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.zhihu.matisse.Matisse;
import com.zhihu.matisse.MimeType;
import com.zhihu.matisse.engine.impl.GlideEngine;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import permissions.dispatcher.NeedsPermission;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomeDeliverBatchEntryListActivity extends BaseActivity {

    private static final int REFRESH_ENTRY_DATA_LIST = 100100;

    public static final int REQ_CAPTURE_PHOTO = 200;
    public static final int REQ_SELECT_PHOTO = 300;

    public static final int REQ_CAPTURE_PHOTO_FAIL = 400;
    public static final int REQ_SELECT_PHOTO_FAIL = 500;

    private int batchId = 0;
    private String batchCode = "";

    private ListView lvEntryList = null;
    private LinearLayout lytDriveStart = null;
    private Button btnDriveStart = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home_deliver_batch_entry_list);

        Intent intent = getIntent();
        batchId = intent.getIntExtra("batchId", 0);
        batchCode = intent.getStringExtra("batchCode");

        setTitle("批次 " + batchCode);

        lytDriveStart = findViewById(R.id.lytDriveStart);

        btnDriveStart = findViewById(R.id.btnDriveStart);
        btnDriveStart.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("确认此时司机送货出发码？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                        String clientTime = ClientTimeUtil.getClientTime();
                        HomeDeliverBatchRequest homeDeliverBatchRequest = RetrofitUtil.createRequest(HomeDeliverBatchRequest.class);
                        Call<ResponseInfo> call = homeDeliverBatchRequest.driveStart(batchId, clientTime);
                        Dialog mask = MaskDialog.show(HomeDeliverBatchEntryListActivity.this);
                        call.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchEntryListActivity.this)) {
                                    return;
                                }

                                SoundUtil.playSuccess(HomeDeliverBatchEntryListActivity.this);

                                Toast.makeText(HomeDeliverBatchEntryListActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();

                                loadEntryList();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(HomeDeliverBatchEntryListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        });

                    }
                });

                alertDialogBuilder.setNegativeButton("取消", null);

                AlertDialog alertDialog = alertDialogBuilder.create();
                alertDialog.show();
            }
        });

        lvEntryList = findViewById(R.id.lvEntryList);
        lvEntryList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                /*
                搬到按钮里面去处理了
                Map<String, Object> map = (Map<String, Object>) parent.getItemAtPosition(position);
                int entryId = (int) map.get("id");

                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("确认30分钟内将送达本订单包裹吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                        HomeDeliverBatchRequest homeDeliverBatchRequest = RetrofitUtil.createRequest(HomeDeliverBatchRequest.class);
                        Call<ResponseInfo> sendFourthMessageCall = homeDeliverBatchRequest.sendFourthMessage(entryId);
                        Dialog mask = MaskDialog.show(HomeDeliverBatchEntryListActivity.this);
                        sendFourthMessageCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchEntryListActivity.this)) {
                                    return;
                                }

                                SoundUtil.playSuccess(HomeDeliverBatchEntryListActivity.this);

                                Toast.makeText(HomeDeliverBatchEntryListActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();

                                loadEntryList();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(HomeDeliverBatchEntryListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        });

                    }
                });

                alertDialogBuilder.setNegativeButton("取消", null);

                AlertDialog alertDialog = alertDialogBuilder.create();
                alertDialog.show();
                 */
            }
        });
    }

    @Override
    public void onStart() {
        super.onStart();

        // 拍照上传的图片后，列表数据会在这里刷新，但发送消息后需要自己刷新
        loadEntryList();
    }

    public void loadEntryList() {
        new Thread(new Runnable() {
            @Override
            public void run() {

                HomeDeliverBatchRequest homeDeliverBatchRequest = RetrofitUtil.createRequest(HomeDeliverBatchRequest.class);
                Call<HomeDeliverBatchEntryListResponseInfo> warehouseListCall = homeDeliverBatchRequest.entryList(batchId);
                // Dialog mask = MaskDialog.show(HomeDeliverBatchWarehouseListActivity.this);
                warehouseListCall.enqueue(new Callback<HomeDeliverBatchEntryListResponseInfo>() {
                    @Override
                    public void onResponse(Call<HomeDeliverBatchEntryListResponseInfo> call, Response<HomeDeliverBatchEntryListResponseInfo> response) {
                        // mask.dismiss();

                        HomeDeliverBatchEntryListResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchEntryListActivity.this)) {
                            return;
                        }

                        Message msg = new Message();
                        msg.what = REFRESH_ENTRY_DATA_LIST;
                        msg.obj = respData;

                        messageHandler.sendMessage(msg);
                    }

                    @Override
                    public void onFailure(Call<HomeDeliverBatchEntryListResponseInfo> call, Throwable t) {
                        Toast.makeText(HomeDeliverBatchEntryListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
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
                case REFRESH_ENTRY_DATA_LIST:

                    HomeDeliverBatchEntryListResponseInfo listResponse = (HomeDeliverBatchEntryListResponseInfo) msg.obj;

                    if (listResponse.getShowDriveStart()) {
                        lytDriveStart.setVisibility(View.VISIBLE);
                    } else {
                        lytDriveStart.setVisibility(View.GONE);
                    }

                    List<Map<String, Object>> listDataMap = new ArrayList<Map<String, Object>>();
                    for (HomeDeliverBatchEntryItemInfo item : listResponse.getItemList()) {
                        Map<String, Object> dataMap = new HashMap<String, Object>();
                        dataMap.put("id", item.getId());
                        dataMap.put("deliverNum", item.getDeliverNum());
                        dataMap.put("orderCodeLabel", item.getDeliverNum() + ". 订单 " + item.getOrderCode());
                        dataMap.put("orderCode", item.getOrderCode());
                        dataMap.put("customerPhone", item.getCustomerPhone());
                        dataMap.put("customerPhoneLabel", "电话： " + item.getCustomerPhone() + "  " + item.getConsignee());
                        dataMap.put("packBoxCount", "共 " + item.getPackBoxCount() + " 件");
                        dataMap.put("orderStatus", item.getOrderStatus());
                        dataMap.put("orderStatusFinished", item.getOrderStatusFinished());
                        dataMap.put("messageStatus", item.getMessageStatus());
                        dataMap.put("messageStatusSent", item.getMessageStatusSent());
                        dataMap.put("addressLabel", "地址： " + item.getAddress());
                        dataMap.put("address", item.getAddress());
                        dataMap.put("companyLabel", "公司/店铺名： " + item.getCompany());
                        dataMap.put("company", item.getCompany());
                        dataMap.put("confirmMessage", "客户留言： " + item.getConfirmMessage());
                        dataMap.put("confirmStatus", "客户确认状态： " + item.getConfirmStatus());
                        dataMap.put("deliverTime", "预计交付：" + item.getEstimateDeliverTime() + "，实际：" + item.getActualDeliverTime() + "，偏差：" + item.getDeliverTimeDiff() + "Min.");
                        dataMap.put("showButton", item.getShowButton());

                        listDataMap.add(dataMap);
                    }

                    if (listResponse.getSuccess()) {
                        HomeDeliverBatchEntryListAdapter adapter = new HomeDeliverBatchEntryListAdapter(HomeDeliverBatchEntryListActivity.this
                                , listDataMap
                                , R.layout.item_home_deliver_batch_entry_list
                                , new String[]{"orderCodeLabel", "customerPhoneLabel", "packBoxCount", "orderStatus", "messageStatus", "addressLabel", "confirmStatus", "confirmMessage", "deliverTime"}
                                , new int[]{R.id.tvOrderCode, R.id.tvPhone, R.id.tvPackBoxCount, R.id.tvOrderStatus, R.id.tvMessageStatus, R.id.tvAddress, R.id.tvConfirmStatus, R.id.tvConfirmMessage, R.id.tvDeliverTime});

                        lvEntryList.setAdapter(adapter);
                    } else {
                        Toast.makeText(HomeDeliverBatchEntryListActivity.this, listResponse.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                    break;
                default:
                    break;
            }
        }
    }

    private String orderCodeCache = "";

    private Uri imageCaptureUri;

    @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE})
    protected void showCapturePhoto(String orderCode, int reqCode) {
        this.orderCodeCache = orderCode;

        Intent intentTakePhoto = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);

        imageCaptureUri = Uri.fromFile(new File(Environment.getExternalStorageDirectory(), "temp_order_photo_" + String.valueOf(System.currentTimeMillis()) + ".jpg"));
        intentTakePhoto.putExtra(MediaStore.EXTRA_OUTPUT, imageCaptureUri);
        intentTakePhoto.putExtra("return-data", true);

        // startActivityForResult(intentTakePhoto, REQ_CAPTURE_PHOTO);
        startActivityForResult(intentTakePhoto, reqCode);
    }

    @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE})
    protected void onCapturePhoto(Intent data, OrderPhotoType orderPhotoType) {
        // begin 上传至阿里云OSS

        byte[] imageBytes = PhotoUtil.compressImageFileToBytes(imageCaptureUri.getPath(), HomeDeliverBatchEntryListActivity.this);

        // 构造上传请求。
        String photoKey = PhotoUtil.newPhotoStoreName(); // 获取新的文件名，一般都是以 .jpg 结尾

        ObjectMetadata objectMetadata = new ObjectMetadata();
        objectMetadata.setContentType("image/jpg");

        PutObjectRequest put = new PutObjectRequest(Config.getOssBucketName(), photoKey, imageBytes, objectMetadata);

        // 异步上传时可以设置进度回调。
        put.setProgressCallback(new OSSProgressCallback<PutObjectRequest>() {
            @Override
            public void onProgress(PutObjectRequest request, long currentSize, long totalSize) {
            }
        });

        OSS oss = new OSSClient(getApplicationContext(), Config.getOssEndPoint(), new OSSStsTokenCredentialProvider(Config.getOssAccessKeyId(), Config.getOssAccessKeySecret(), ""));

        OSSAsyncTask<PutObjectResult> task = oss.asyncPutObject(put, new OSSCompletedCallback<PutObjectRequest, PutObjectResult>() {
            @Override
            public void onSuccess(PutObjectRequest request, PutObjectResult result) {
                LogisticsOrderPhotoRequest photoRequest = RetrofitUtil.createRequest(LogisticsOrderPhotoRequest.class);

                String orderCode = orderCodeCache;
                String clientTime = ClientTimeUtil.getClientTime();
                // Call<ResponseInfo> createCall = photoRequest.create(orderCode, "", OrderPhotoType.Deliver.getId(), photoKey, clientTime);
                Call<ResponseInfo> createCall = photoRequest.create(orderCode, "", orderPhotoType.getId(), photoKey, clientTime);
                // Dialog mask = MaskDialog.show(HomeDeliverBatchEntryListActivity.this);
                createCall.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        // mask.dismiss();

                        if (imageCaptureUri != null && imageCaptureUri.getPath() != null) {
                            File tempOrderPhoto = new File(imageCaptureUri.getPath());
                            if (tempOrderPhoto.exists()) {
                                tempOrderPhoto.delete();
                            }
                        }

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchEntryListActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(HomeDeliverBatchEntryListActivity.this);
                        Toast.makeText(HomeDeliverBatchEntryListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        loadEntryList(); // 重新加载一下，保证列表更新
                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(HomeDeliverBatchEntryListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }

            @Override
            public void onFailure(PutObjectRequest request, ClientException clientExcepion, ServiceException serviceException) {
                // 请求异常。
                if (clientExcepion != null) {
                    // 本地异常，如网络异常等。
                    // clientExcepion.printStackTrace();

                    Toast.makeText(HomeDeliverBatchEntryListActivity.this, "oss错误: " + clientExcepion.getMessage(), Toast.LENGTH_LONG).show();
                    return;
                }
                if (serviceException != null) {
                    // 服务异常。
                    Toast.makeText(HomeDeliverBatchEntryListActivity.this, "oss错误: " + serviceException.getMessage(), Toast.LENGTH_LONG).show();
                    return;
                }

                Toast.makeText(HomeDeliverBatchEntryListActivity.this, "oss上传错误", Toast.LENGTH_LONG).show();
            }
        });
        // task.cancel(); // 可以取消任务。
        // task.waitUntilFinished(); // 等待上传完成。

        // end 上传至阿里云OSS
    }

    @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE})
    public void showSelectPhoto(String orderCode, int reqCode) {
        this.orderCodeCache = orderCode;
        Matisse.from(HomeDeliverBatchEntryListActivity.this)
                //选择图片
                .choose(MimeType.ofImage())
                //是否只显示选择的类型的缩略图，就不会把所有图片视频都放在一起，而是需要什么展示什么
                .showSingleMediaType(true)
                //这两行要连用 是否在选择图片中展示照相 和适配安卓7.0 FileProvider
                //.capture(true)
                //.captureStrategy(new CaptureStrategy(false, "PhotoProvider"))
                //有序选择图片 123456...
                .countable(true)
                //最大选择数量为9
                .maxSelectable(9)
                //选择方向
                .restrictOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT)
                //界面中缩略图的质量
                .thumbnailScale(0.8f)
                //蓝色主题
                .theme(R.style.Matisse_Zhihu)
                //Glide加载方式
                .imageEngine(new GlideEngine())
                //请求码
                // .forResult(REQ_SELECT_PHOTO);
                .forResult(reqCode);
    }

    @NeedsPermission(Manifest.permission.READ_EXTERNAL_STORAGE)
    protected void onSelectPhoto(Intent data, OrderPhotoType orderPhotoType) {
        List<Uri> selectedPhotoList = Matisse.obtainResult(data);

        for (Uri selectedPhoto : selectedPhotoList) {
            String[] proj = {MediaStore.Images.Media.DATA};
            Cursor actualimagecursor = this.managedQuery(selectedPhoto, proj, null, null, null);
            int actual_image_column_index = actualimagecursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
            actualimagecursor.moveToFirst();
            String img_path = actualimagecursor.getString(actual_image_column_index);

            // begin 上传至阿里云OSS

            byte[] imageBytes = PhotoUtil.compressImageFileToBytes(img_path, HomeDeliverBatchEntryListActivity.this);

            // 构造上传请求。
            String photoKey = PhotoUtil.newPhotoStoreName(); // 获取新的文件名，一般都是以 .jpg 结尾

            ObjectMetadata objectMetadata = new ObjectMetadata();
            objectMetadata.setContentType("image/jpg");

            PutObjectRequest put = new PutObjectRequest(Config.getOssBucketName(), photoKey, imageBytes, objectMetadata);

            // 异步上传时可以设置进度回调。
            put.setProgressCallback(new OSSProgressCallback<PutObjectRequest>() {
                @Override
                public void onProgress(PutObjectRequest request, long currentSize, long totalSize) {
                }
            });

            OSS oss = new OSSClient(getApplicationContext(), Config.getOssEndPoint(), new OSSStsTokenCredentialProvider(Config.getOssAccessKeyId(), Config.getOssAccessKeySecret(), ""));

            OSSAsyncTask<PutObjectResult> task = oss.asyncPutObject(put, new OSSCompletedCallback<PutObjectRequest, PutObjectResult>() {
                @Override
                public void onSuccess(PutObjectRequest request, PutObjectResult result) {
                    LogisticsOrderPhotoRequest photoRequest = RetrofitUtil.createRequest(LogisticsOrderPhotoRequest.class);

                    String orderCode = orderCodeCache;
                    String clientTime = ClientTimeUtil.getClientTime();
                    // Call<ResponseInfo> createCall = photoRequest.create(orderCode, "", OrderPhotoType.Deliver.getId(), photoKey, clientTime);
                    Call<ResponseInfo> createCall = photoRequest.create(orderCode, "", orderPhotoType.getId(), photoKey, clientTime);
                    // Dialog mask = MaskDialog.show(HomeDeliverBatchEntryListActivity.this);
                    createCall.enqueue(new Callback<ResponseInfo>() {
                        @Override
                        public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                            // mask.dismiss();

                            ResponseInfo respData = response.body();

                            if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchEntryListActivity.this)) {
                                return;
                            }

                            SoundUtil.playSuccess(HomeDeliverBatchEntryListActivity.this);
                            Toast.makeText(HomeDeliverBatchEntryListActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                            loadEntryList(); // 重新加载一下，保证列表更新
                        }

                        @Override
                        public void onFailure(Call<ResponseInfo> call, Throwable t) {
                            Toast.makeText(HomeDeliverBatchEntryListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                    });
                }

                @Override
                public void onFailure(PutObjectRequest request, ClientException clientExcepion, ServiceException serviceException) {
                    // 请求异常。
                    if (clientExcepion != null) {
                        // 本地异常，如网络异常等。
                        // clientExcepion.printStackTrace();

                        Toast.makeText(HomeDeliverBatchEntryListActivity.this, "oss错误: " + clientExcepion.getMessage(), Toast.LENGTH_LONG).show();
                        return;
                    }
                    if (serviceException != null) {
                        // 服务异常。
                        Toast.makeText(HomeDeliverBatchEntryListActivity.this, "oss错误: " + serviceException.getMessage(), Toast.LENGTH_LONG).show();
                        return;
                    }

                    Toast.makeText(HomeDeliverBatchEntryListActivity.this, "oss上传错误", Toast.LENGTH_LONG).show();
                }
            });
            // task.cancel(); // 可以取消任务。
            // task.waitUntilFinished(); // 等待上传完成。

            // end 上传至阿里云OSS
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        try {
            if (resultCode == RESULT_OK) {
                if (requestCode == REQ_CAPTURE_PHOTO) {
                    onCapturePhoto(data, OrderPhotoType.Deliver);
                } else if (requestCode == REQ_SELECT_PHOTO) {
                    onSelectPhoto(data, OrderPhotoType.Deliver);
                } else if (requestCode == REQ_CAPTURE_PHOTO_FAIL) {
                    onCapturePhoto(data, OrderPhotoType.DeliverFail);
                } else if (requestCode == REQ_SELECT_PHOTO_FAIL) {
                    onSelectPhoto(data, OrderPhotoType.DeliverFail);
                }
            }
        } catch (Exception exp) {
            exp.printStackTrace();
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
