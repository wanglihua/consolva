package com.huadu.iepoch.ui.experssimageshow;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.Toast;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.data.http.expressimageshow.ExpressImageShowImageItemInfo;
import com.huadu.iepoch.data.http.expressimageshow.ExpressImageShowImageListResponseInfo;
import com.huadu.iepoch.http.ExpressImageShowRequest;
import com.huadu.iepoch.ui.BaseActivity;

import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressImageListActivity extends BaseActivity {

    private static final int REFRESH_EXPRESS_IMAGE_LIST = 100100;

    private static List<String> imageList = new ArrayList<>();

    private String trackingNo;

    private ListView lvImageList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_image_list);

        Intent intent = getIntent();
        this.trackingNo = intent.getStringExtra("trackingNo");

        lvImageList = findViewById(R.id.lvImageList);
        lvImageList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                ExpressImageShowImageItemInfo imageItem = (ExpressImageShowImageItemInfo) parent.getItemAtPosition(position);

                Intent intentImageView = new Intent(ExpressImageListActivity.this, ExpressImageViewActivity.class);
                // intentImageView.putExtra("bigImageUrl", imageItem.getBigImageUrl());
                ExpressImageViewActivity.setImageList(imageList);
                intentImageView.putExtra("imageIndex", position);
                startActivity(intentImageView);
            }
        });

        new Thread(new Runnable() {
            @Override
            public void run() {
                loadImageList();
            }
        }).start();
    }

    private void loadImageList() {
        ExpressImageShowRequest expressImageShowRequest = RetrofitUtil.createRequest(ExpressImageShowRequest.class);
        Call<ExpressImageShowImageListResponseInfo> imageListCall = expressImageShowRequest.imageListCn(ExpressImageListActivity.this.trackingNo);
        // Dialog mask = MaskDialog.show(ExpressImageListActivity.this);
        imageListCall.enqueue(new Callback<ExpressImageShowImageListResponseInfo>() {
            @Override
            public void onResponse(Call<ExpressImageShowImageListResponseInfo> call, Response<ExpressImageShowImageListResponseInfo> response) {
                // mask.dismiss();

                ExpressImageShowImageListResponseInfo respData = response.body();

                if (!RetrofitUtil.checkResponse(respData, ExpressImageListActivity.this)) {
                    return;
                }

                Message msg = new Message();
                msg.what = REFRESH_EXPRESS_IMAGE_LIST;
                msg.obj = respData;

                messageHandler.sendMessage(msg);
            }

            @Override
            public void onFailure(Call<ExpressImageShowImageListResponseInfo> call, Throwable t) {
                Toast.makeText(ExpressImageListActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private MessageHandler messageHandler = new MessageHandler();

    @SuppressLint("HandlerLeak")
    private class MessageHandler extends Handler {
        @Override
        public void handleMessage(@NotNull Message msg) {
            super.handleMessage(msg);
            if (msg.what == REFRESH_EXPRESS_IMAGE_LIST) {
                ExpressImageShowImageListResponseInfo imageListResponse = (ExpressImageShowImageListResponseInfo) msg.obj;

                if (imageListResponse.getSuccess()) {

                    // 待下架快递列表：
                    // tvNotOffShelfListLabel.setText("待下架快递列表 (" + warehouseListResponse.getItemCount() + ")：");

                    List<ExpressImageShowImageItemInfo> imageItemList = imageListResponse.getImageList();
                    // 由于服务器端在这个地址上的错误处理，这里做个转换补偿，可以先不更新服务器也能行。以后这个转换可以去掉
                    for (ExpressImageShowImageItemInfo imageItem : imageItemList) {
                        imageItem.setBigImageUrl(imageItem.getBigImageUrl().replace('\\', '/'));
                        imageItem.setSmallImageUrl(imageItem.getSmallImageUrl().replace('\\', '/'));
                    }

                    // 填充静态变量
                    ExpressImageListActivity.imageList = new ArrayList<>();
                    for (ExpressImageShowImageItemInfo imageItem : imageItemList) { {
                        ExpressImageListActivity.imageList.add(imageItem.getBigImageUrl());
                    }}

                    ExpressImageListAdapter adapter = new ExpressImageListAdapter(ExpressImageListActivity.this, imageItemList);

                    lvImageList.setAdapter(adapter);
                } else {
                    Toast.makeText(ExpressImageListActivity.this, imageListResponse.getMessage(), Toast.LENGTH_SHORT).show();
                }
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
