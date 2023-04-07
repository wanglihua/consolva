package com.huadu.shunda.ui.expressshelf;

import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import com.bumptech.glide.Glide;
import com.huadu.shunda.R;
import com.huadu.shunda.core.ClipboardUtil;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.ResponseDataInfo;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.data.http.expressphoto.ExpressPhotoExpressPhotoListItem;
import com.huadu.shunda.data.http.expressphoto.ExpressPhotoExpressPhotoListResponseData;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckDetailInfo;
import com.huadu.shunda.data.http.warehousecheck.WarehouseCheckDetailResponseInfo;
import com.huadu.shunda.http.ExpressPhotoRequest;
import com.huadu.shunda.http.WarehouseCheckRequest;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressWarehouseCheckDetailActivity extends BaseActivity {

    private int warehouseCheckId;
    private String trackingNo;


    private TextView tvTrackingNo = null;
    private TextView tvAoPinTrackingNo = null;
    private TextView tvStorageLocFullName = null;
    private TextView tvClaimShelfStatus = null;
    private TextView tvChatCustomerCode = null;
    private TextView tvChatUserLoginName = null;
    private TextView tvStatus = null;
    private TextView tvCreateTime = null;
    private TextView tvChatMessage = null;
    private EditText etReplyMessage = null;
    private Button btnSetChecked = null;

    private ImageView ivBigImage;
    private TextView tvImageNum;

    private List<String> imageList = new ArrayList<>();
    private int imageIndex = 0;

    private String aoPinTrackingNo;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_warehouse_check_detail);

        Intent intent = getIntent();

        warehouseCheckId = intent.getIntExtra("warehouseCheckId", 0);
        trackingNo = intent.getStringExtra("trackingNo");

        tvTrackingNo = findViewById(R.id.tvTrackingNo);
        tvAoPinTrackingNo = findViewById(R.id.tvAoPinTrackingNo);
        tvStorageLocFullName = findViewById(R.id.tvStorageLocFullName);
        tvClaimShelfStatus = findViewById(R.id.tvClaimShelfStatus);
        tvChatCustomerCode = findViewById(R.id.tvChatCustomerCode);
        tvChatUserLoginName = findViewById(R.id.tvChatUserLoginName);
        tvStatus = findViewById(R.id.tvStatus);
        tvCreateTime = findViewById(R.id.tvCreateTime);
        tvChatMessage = findViewById(R.id.tvChatMessage);
        etReplyMessage = findViewById(R.id.etReplyMessage);
        btnSetChecked = findViewById(R.id.btnSetChecked);

        tvTrackingNo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                ClipboardUtil.copyToClipboard(ExpressWarehouseCheckDetailActivity.this, trackingNo);
                Toast.makeText(ExpressWarehouseCheckDetailActivity.this, "快递单号成功复制到剪贴板！", Toast.LENGTH_LONG).show();
            }
        });

        tvAoPinTrackingNo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                ClipboardUtil.copyToClipboard(ExpressWarehouseCheckDetailActivity.this, aoPinTrackingNo);
                Toast.makeText(ExpressWarehouseCheckDetailActivity.this, "舜达入库号成功复制到剪贴板！", Toast.LENGTH_LONG).show();
            }
        });

        btnSetChecked.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(ExpressWarehouseCheckDetailActivity.this);
                alertDialogBuilder.setTitle("确认");
                alertDialogBuilder.setMessage("确实要设置已完成吗？");
                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                        String replyMessage = etReplyMessage.getText().toString();

                        WarehouseCheckRequest warehouseCheckRequest = RetrofitUtil.createRequest(WarehouseCheckRequest.class);
                        Call<ResponseInfo> setCheckedCall = warehouseCheckRequest.setChecked(warehouseCheckId, replyMessage);
                        Dialog mask = MaskDialog.show(ExpressWarehouseCheckDetailActivity.this);
                        setCheckedCall.enqueue(new Callback<ResponseInfo>() {
                            @Override
                            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                mask.dismiss();

                                ResponseInfo respData = response.body();

                                if (!RetrofitUtil.processResponse(response, ExpressWarehouseCheckDetailActivity.this)) {
                                    return;
                                }

                                SoundUtil.playSuccess(ExpressWarehouseCheckDetailActivity.this);

                                Toast.makeText(ExpressWarehouseCheckDetailActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();
                                finish();
                            }

                            @Override
                            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                Toast.makeText(ExpressWarehouseCheckDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                            }

                        });
                    }
                });

                alertDialogBuilder.setNegativeButton("取消", null);

                AlertDialog alertDialog = alertDialogBuilder.create();
                alertDialog.show();
            }
        });

        this.ivBigImage = findViewById(R.id.ivBigImage);
        this.ivBigImage.setOnTouchListener(new View.OnTouchListener() {
            private static final float minDelta = 5;

            private float prePosX = 0f;
            // private float prePosY = 0f;
            private float curPosX = 0f;
            // private float curPosY = 0f;

            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        this.prePosX = event.getX();
                        break;
                    case MotionEvent.ACTION_MOVE:
                        this.curPosX = event.getX();
                        break;
                    case MotionEvent.ACTION_UP:
                        this.curPosX = event.getX();

                        float deltaX = this.curPosX - this.prePosX;
                        if (this.prePosX != 0 && deltaX > minDelta) {
                            // do right
                            imageIndex--;
                            if (imageIndex < 0) {
                                imageIndex = 0;
                            }
                            if (imageIndex >= 0 && imageIndex < imageList.size()) {
                                Glide.with(ExpressWarehouseCheckDetailActivity.this).load(imageList.get(imageIndex)).into(ExpressWarehouseCheckDetailActivity.this.ivBigImage);
                                setImageNumText();
                            }
                        }

                        if (this.prePosX != 0 && deltaX < -minDelta) {
                            // do left
                            imageIndex++;
                            if (imageIndex >= imageList.size()) {
                                imageIndex = imageList.size() - 1;
                            }

                            if (imageIndex >= 0 && imageIndex < imageList.size()) {
                                Glide.with(ExpressWarehouseCheckDetailActivity.this).load(imageList.get(imageIndex)).into(ExpressWarehouseCheckDetailActivity.this.ivBigImage);
                                setImageNumText();
                            }
                        }

                        this.prePosX = event.getX();
                        // this.prePosY = event.getY();
                        break;
                }
                return true;
            }
        });

        tvImageNum = findViewById(R.id.tvImageNum);
    }

    private void setImageNumText() {
        if (imageList == null || imageList.size() == 0) {
            tvImageNum.setText("未能找到关联图片");
            return;
        }

        tvImageNum.setText(String.format("第 %d 张， 共 %d 张，可左右滑动", imageIndex + 1, imageList.size()));
    }

    @Override
    public void onStart() {
        super.onStart();

        loadWarehouseCheckDetail();

        loadImageList();
    }

    public void loadWarehouseCheckDetail() {
        WarehouseCheckRequest warehouseCheckRequest = RetrofitUtil.createRequest(WarehouseCheckRequest.class);
        Call<WarehouseCheckDetailResponseInfo> warehouseCheckListCall = warehouseCheckRequest.detail(warehouseCheckId);
        // Dialog mask = MaskDialog.show(ExpressWarehouseCheckDetailActivity.this);
        warehouseCheckListCall.enqueue(new Callback<WarehouseCheckDetailResponseInfo>() {
            @Override
            public void onResponse(Call<WarehouseCheckDetailResponseInfo> call, Response<WarehouseCheckDetailResponseInfo> response) {
                // mask.dismiss();
                WarehouseCheckDetailResponseInfo respData = response.body();

                if (!RetrofitUtil.processResponse(response, ExpressWarehouseCheckDetailActivity.this)) {
                    return;
                }

                WarehouseCheckDetailInfo warehouseCheck = respData.getWarehouseCheck();

                tvTrackingNo.setText("快递单号：" + warehouseCheck.getTrackingNo());
                tvAoPinTrackingNo.setText("舜达入库号：" + warehouseCheck.getAoPinTrackingNo());
                tvStorageLocFullName.setText("仓位：" + warehouseCheck.getStorageLocFullName());
                tvClaimShelfStatus.setText("上架状态：" + warehouseCheck.getClaimShelfStatusName());
                tvChatCustomerCode.setText("聊天客户：" + warehouseCheck.getChatCustomerCode());
                tvChatUserLoginName.setText("聊天客服：" + warehouseCheck.getChatUserLoginName());
                tvStatus.setText("状态：" + warehouseCheck.getStatusName());
                tvCreateTime.setText("创建时间：" + warehouseCheck.getCreateTime());

                tvChatMessage.setText("聊天消息：" + warehouseCheck.getChatMessage());
                etReplyMessage.setText(warehouseCheck.getReplyMessage());

                ExpressWarehouseCheckDetailActivity.this.trackingNo = warehouseCheck.getTrackingNo();
                ExpressWarehouseCheckDetailActivity.this.aoPinTrackingNo = warehouseCheck.getAoPinTrackingNo();
            }

            @Override
            public void onFailure(Call<WarehouseCheckDetailResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressWarehouseCheckDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    public void loadImageList() {
        if (this.trackingNo == null || "".equals(this.trackingNo.trim())) {
            setImageNumText();
            return;
        }

        ExpressPhotoRequest expressPhotoRequest = RetrofitUtil.createRequest(ExpressPhotoRequest.class);
        Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> imageListCall = expressPhotoRequest.expressPhotoList(this.trackingNo);
        // Dialog mask = MaskDialog.show(activity);
        imageListCall.enqueue(new Callback<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>>() {
            @Override
            public void onResponse(Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> call, Response<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> response) {
                // mask.dismiss();

                ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData> respData = response.body();

                if (!RetrofitUtil.processResponse(response, ExpressWarehouseCheckDetailActivity.this)) {
                    return;
                }

                List<ExpressPhotoExpressPhotoListItem> imageItemList = respData.getData().getPhotoList();
                List<String> imageList = new ArrayList<>();
                for (ExpressPhotoExpressPhotoListItem imageItem : imageItemList) {
                    imageList.add(imageItem.getBigImageUrl());
                }

                ExpressWarehouseCheckDetailActivity.this.imageList = imageList;

                if (imageList.size() > 0) {
                    String imageUrl = imageList.get(imageIndex);
                    Glide.with(ExpressWarehouseCheckDetailActivity.this).load(imageUrl).into(ivBigImage);

                    setImageNumText();
                }
            }

            @Override
            public void onFailure(Call<ResponseDataInfo<ExpressPhotoExpressPhotoListResponseData>> call, Throwable t) {
                Toast.makeText(ExpressWarehouseCheckDetailActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
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