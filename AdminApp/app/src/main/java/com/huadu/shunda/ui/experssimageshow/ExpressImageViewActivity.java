package com.huadu.shunda.ui.experssimageshow;

import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.huadu.shunda.R;
import com.huadu.shunda.ui.BaseActivity;

import java.util.ArrayList;
import java.util.List;

public class ExpressImageViewActivity extends BaseActivity {

    private static List<String> imageList = new ArrayList<>();

    public static void setImageList(List<String> imageList) {
        ExpressImageViewActivity.imageList = imageList;
    }

    private int imageIndex = 0;

    // private String bigImageUrl = "";

    private ImageView ivBigImage = null;

    private TextView tvImageNum = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_image_view);

        Intent intent = getIntent();

        // bigImageUrl = intent.getStringExtra("bigImageUrl");
        imageIndex = intent.getIntExtra("imageIndex", 0);

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
                                Glide.with(ExpressImageViewActivity.this).load(imageList.get(imageIndex)).into(ExpressImageViewActivity.this.ivBigImage);
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
                                Glide.with(ExpressImageViewActivity.this).load(imageList.get(imageIndex)).into(ExpressImageViewActivity.this.ivBigImage);
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

        this.tvImageNum = findViewById(R.id.tvImageNum);
        setImageNumText();

        String imageUrl = imageList.get(imageIndex);
        Glide.with(this).load(imageUrl).into(ivBigImage);
    }

    private void setImageNumText() {
        this.tvImageNum.setText(String.format("第 %d 张， 共 %d 张，可左右滑动", imageIndex + 1, imageList.size()));
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