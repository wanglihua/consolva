package com.huadu.iepoch.ui;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.bumptech.glide.Glide;
import com.huadu.iepoch.R;
import com.huadu.iepoch.core.PdaUtil;
import com.huadu.iepoch.core.PermissionUtil;
import com.huadu.iepoch.ui.pda.ScanActivity;
import com.huadu.iepoch.ui.zxing.QrScanConstants;
import com.huadu.iepoch.ui.zxing.activity.CaptureActivity;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class TestActivity extends BaseActivity {

    private static final int REQ_QR_CODE = 100;

    private Button btnScan = null;
    private Button btnCameraScan = null;
    private Button btnPdaScan = null;

    private TextView tvText = null;

    private ImageView ivTest = null;

    private List<String> imageList = new ArrayList<String>();
    private int imageIndex = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_test);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new ButtonScanClickListener());

        btnCameraScan = findViewById(R.id.btnCameraScan);
        btnCameraScan.setOnClickListener(new ButtonCameraScanClickListener());

        btnPdaScan = findViewById(R.id.btnPdaScan);
        btnPdaScan.setOnClickListener(new ButtonPdaScanClickListener());

        tvText = findViewById(R.id.tvText);
        // tvText.setText("Hello World!");

        ivTest = findViewById(R.id.ivTest);

        ivTest.setOnTouchListener(new View.OnTouchListener() {
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
                        // this.prePosY = event.getY();
                        break;
                    case MotionEvent.ACTION_MOVE:
                        this.curPosX = event.getX();
                        // this.curPosY = event.getY();
                        break;
                    case MotionEvent.ACTION_UP:
                        this.curPosX = event.getX();
                        // this.curPosY = event.getY();

                        float deltaX = this.curPosX - this.prePosX;
                        if (this.prePosX != 0 && deltaX > minDelta) {
                            // do right
                            Log.e("delta", "right");

                            imageIndex--;
                            if (imageIndex < 0) {
                                imageIndex = 0;
                            }
                            if (imageIndex >= 0 && imageIndex < imageList.size()) {
                                Glide.with(TestActivity.this).load(imageList.get(imageIndex)).into(ivTest);
                            }
                        }

                        if (this.prePosX != 0 && deltaX < -minDelta) {
                            // do left
                            Log.e("delta", "left");

                            imageIndex++;
                            if (imageIndex >= imageList.size()) {
                                imageIndex = imageList.size() - 1;
                            }

                            if (imageIndex >= 0 && imageIndex < imageList.size()) {
                                Glide.with(TestActivity.this).load(imageList.get(imageIndex)).into(ivTest);
                            }
                        }

                        this.prePosX = event.getX();
                        // this.prePosY = event.getY();
                        break;
                }
                return true;
            }
        });

        new Date(2022, 1, 1);

        imageList.add("http://iepoch-version-3-china.oss-cn-hangzhou.aliyuncs.com/logistics/202202/20220224/3e66fce516524dd1aa492b807bed4dbd.jpg?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_5pys5b-r6YCS5bey562-5pS277yM562-5pS25Lq677ya5r6z5ou86ZuG6L-Q,color_FF0000,g_south,y_80");
        imageList.add("http://iepoch-version-3-china.oss-cn-hangzhou.aliyuncs.com/logistics/202202/20220224/b65c9f76107f4f66bfa36b586b92fe10.jpg?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_5pys5b-r6YCS5bey562-5pS277yM562-5pS25Lq677ya5r6z5ou86ZuG6L-Q,color_FF0000,g_south,y_80");
        imageList.add("http://iepoch-version-3-china.oss-cn-hangzhou.aliyuncs.com/logistics/202202/20220224/5cdc4b87b4814c7cb3f84c28db452a75.jpg?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_44CQ5r6z5ou86ZuG6L-Q44CR44CQ5YyX5Lqs5pe26Ze044CRMjAyMi0wMi0yNCAxNDoyNSDmi43mkYQ,color_FF0000,g_south,y_80");
        imageList.add("http://iepoch-version-3-china.oss-cn-hangzhou.aliyuncs.com/logistics/202202/20220224/a5004133fadb49e38d73de208bc4effa.jpg?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_44CQ5r6z5ou86ZuG6L-Q44CR44CQ5YyX5Lqs5pe26Ze044CRMjAyMi0wMi0yNCAxNDoyNSDmi43mkYQ,color_FF0000,g_south,y_80");
        imageList.add("http://iepoch-version-3-china.oss-cn-hangzhou.aliyuncs.com/logistics/202202/20220224/973b4c4973b7456ea1f8c1236e8016e3.jpg?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_44CQ5r6z5ou86ZuG6L-Q44CR44CQ5YyX5Lqs5pe26Ze044CRMjAyMi0wMi0yNCAxNDoyNiDmi43mkYQ,color_FF0000,g_south,y_80");
    }

    @Override
    public void onStart() {
        super.onStart();

        if (imageIndex >= 0 && imageIndex < imageList.size()) {
            Glide.with(this).load(imageList.get(imageIndex)).into(ivTest);
        }
    }

    private class ButtonScanClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            startQrCode(REQ_QR_CODE);

            Toast.makeText(TestActivity.this, "after startQrCode", Toast.LENGTH_LONG).show();
        }
    }

    private class ButtonCameraScanClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            // SoundUtil.playSuccess(TestActivity.this);

            if (PermissionUtil.checkCameraStoragePermissions(TestActivity.this, REQ_QR_CODE) == false) {
                return;
            }

            Intent intent = new Intent(TestActivity.this, CaptureActivity.class);
            intent.putExtra("labelText", "快递单号：\rYT5718107745848\n澳拼入库号：AP18572AH\n包裹数量：1\n客户ID：AP7278CN\n仓位：FC-1层-27号-顶层");
            startActivityForResult(intent, REQ_QR_CODE);
        }
    }

    private class ButtonPdaScanClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            // SoundUtil.playError(TestActivity.this);

            Intent intent = new Intent(TestActivity.this, ScanActivity.class);
            intent.putExtra("labelText", "快递单号：\rYT5718107745848\n澳拼入库号：AP18572AH\n包裹数量：1\n客户ID：AP7278CN\n仓位：FC-1层-27号-顶层");
            startActivityForResult(intent, REQ_QR_CODE);
        }
    }

    private void startQrCode(int requestCode) {

        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(TestActivity.this, ScanActivity.class);
            intent.putExtra("labelText", "扫描二维码测试");
            startActivityForResult(intent, REQ_QR_CODE);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(TestActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "扫描二维码测试");
            startActivityForResult(intent, requestCode);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Toast.makeText(this, "扫描成功", Toast.LENGTH_SHORT).show();

            Bundle bundle = data.getExtras();
            String scanResult = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            Toast.makeText(this, scanResult, Toast.LENGTH_LONG).show();

            startQrCode(REQ_QR_CODE);


            Toast.makeText(TestActivity.this, "after startQrCode 2", Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            // 获得授权
            startQrCode(requestCode);

        } else {
            // 被禁止授权
            Toast.makeText(this, "请至权限中心打开本应用的相机访问权限", Toast.LENGTH_LONG).show();
        }
    }
}
