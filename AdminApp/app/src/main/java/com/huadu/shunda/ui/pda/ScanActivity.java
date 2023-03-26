package com.huadu.shunda.ui.pda;

import android.annotation.SuppressLint;
import android.app.ScansManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.view.MenuItem;

import com.huadu.shunda.R;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.rsk.scandemon6603.Scanner;

public class ScanActivity extends BaseActivity {

    private String labelText = "使用PDA模块扫描";

    private ViewfinderView viewfinderView;

    // 老的PDA
    private ScansManager pda1ScansManager = null;
    private Pda1ScansManagerReceiver pda1ScansManagerReceiver = null;

    // 新的PDA
    private Scanner pda2Scanner = null;
    private Pda2ScannerReceiver pda2ScannerReceiver = null;

    public final static int PDA1_BROADCAST_INPUT_MODE = 0;//广播模式
    public final static int PDA1_INPUTTEXT_INPUT_MODE = 1;//输入框模式

    public final static int PDA1_NON_EXTRA = 0;//无
    public final static int PDA1_ENTER_EXTRA = 1;//添加回车

    /*
        mode 0: 扫描结果以广播模式发送
        mode 1: 扫描结果以模拟按键模式发送
        mode 2: 扫描结果直接发送到焦点编辑框内
        mode 3: 扫描结果发送广播并且发送到编辑框内
        mode 4: 扫描结果发送粘贴板
     */
    public final static int PDA2_BROADCAST_INPUT_MODE = 0; // 广播模式

    @SuppressLint("WrongConstant")
    private void initScan() {
        pda1ScansManager = (ScansManager) getSystemService("scans");
        if (pda1ScansManager != null) {
            pda1ScansManager.setInputMode(PDA1_BROADCAST_INPUT_MODE);
            pda1ScansManager.setExtras(PDA1_NON_EXTRA);

            if (pda1ScansManagerReceiver == null) {
                IntentFilter filter = new IntentFilter();
                filter.addAction("android.scanservice.action.UPLOAD_BARCODE_DATA");
                pda1ScansManagerReceiver = new Pda1ScansManagerReceiver();
                getApplicationContext().registerReceiver(pda1ScansManagerReceiver, filter);
            }
        } else { // 使用新的终端接口

            pda2Scanner = Scanner.getInstance();
            pda2Scanner.setScanMode(PDA2_BROADCAST_INPUT_MODE); // 广播模式

            // 用这个广播模式
            if (pda2ScannerReceiver == null) {
                IntentFilter filter = new IntentFilter();
                filter.addAction("com.scanner.result");
                pda2ScannerReceiver = new Pda2ScannerReceiver();
                registerReceiver(pda2ScannerReceiver, filter);
            }

            pda2Scanner.setEndKey(0); // 无附加内容
            // pda2Scanner.open();
            // pda2Scanner.start();
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_scan);

        viewfinderView = findViewById(R.id.viewfinder_content);

        Intent intent = getIntent();

        labelText = intent.getStringExtra("labelText");
        if (labelText == null || "".equals(labelText)) {
            labelText = "使用PDA模块扫描";
        }

        viewfinderView.setLabelText(labelText);

        initScan();
    }

    @Override
    protected void onStart() {
        super.onStart();

        if (pda1ScansManager != null) {
            pda1ScansManager.startScan();
        } else if (pda2Scanner != null) {
            pda2Scanner.setEndKey(0); // 无附加内容
            if (!pda2Scanner.IsOpen()) {
                pda2Scanner.open();
            }
            pda2Scanner.start();
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        if (pda1ScansManager != null) {
            pda1ScansManager.stopScan();
        } else if (pda2Scanner != null) {
            pda2Scanner.stop();
            pda2Scanner.close();
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();

        if (pda1ScansManagerReceiver != null) {
            try {
                getApplicationContext().unregisterReceiver(pda1ScansManagerReceiver);
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        if (pda2ScannerReceiver != null) {
            try {
                unregisterReceiver(pda2ScannerReceiver);
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }

    class Pda1ScansManagerReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {
            // et.append("Broadcast:" + intent.getStringExtra("barcode") + "\n");

            Intent intentScan = ScanActivity.this.getIntent();
            intentScan.putExtra(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN, intent.getStringExtra("barcode"));
            ScanActivity.this.setResult(RESULT_OK, intentScan);

            if (pda1ScansManager != null) {
                pda1ScansManager.stopScan();
            }

            ScanActivity.this.finish();
        }
    }

    class Pda2ScannerReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {

            Intent intentScan = ScanActivity.this.getIntent();
            String data = intent.getStringExtra("data");
            if (data != null && data.startsWith("NXX:")) {
                data = data.substring("NXX:".length());
                data = data.trim();
            }
            intentScan.putExtra(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN, data);
            ScanActivity.this.setResult(RESULT_OK, intentScan);

            if (pda2Scanner != null) {
                pda2Scanner.stop();
                pda2Scanner.close();
                // pda2Scanner.close(); 不自动关闭，需要手工关闭
            }

            ScanActivity.this.finish();
        }
    }

    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            finish();
        }
        return true;
    }
}
