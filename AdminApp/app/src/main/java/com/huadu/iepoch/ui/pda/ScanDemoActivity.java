package com.huadu.iepoch.ui.pda;

import android.annotation.SuppressLint;
import android.app.ScansManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioButton;

import com.huadu.iepoch.R;
import com.huadu.iepoch.ui.BaseActivity;

public class ScanDemoActivity extends BaseActivity implements View.OnClickListener {

    private ScansManager mScansManager = null;

    private Button controlBtn = null;
    private Button scanBtn = null;
    private Button stopbtn = null;

    private RadioButton mAsBroacastRadioButton = null;
    private RadioButton mAsInputRadioButton = null;

    private RadioButton mAddNone = null;
    private RadioButton mAddEnter = null;


    public final static int BROADCAST_INPUT_MODE = 0;//广播模式
    public final static int INPUTTEXT_INPUT_MODE = 1;//输入框模式

    public final static int NON_EXTRA = 0;//无
    public final static int ENTER_EXTRA = 1;//添加回车

    private int inputmode = INPUTTEXT_INPUT_MODE;
    private int addExtra = NON_EXTRA;

    EditText et = null;
    private MyReceiver broadcastReceiver = null;

    private void initBroadcast() {
        if (broadcastReceiver == null) {
            IntentFilter filter = new IntentFilter();
            filter.addAction("android.scanservice.action.UPLOAD_BARCODE_DATA");
            filter.addAction("com.safeuem.doublebird.appReadyToInstall");
            broadcastReceiver = new MyReceiver();
            getApplicationContext().registerReceiver(broadcastReceiver, filter);
        }
    }

    private void reinitBroadcast() {
        //getApplicationContext().unregisterReceiver(broadcastReceiver);
    }

    private boolean scan = false;

    @SuppressLint("WrongConstant")
    private void initdata() {
        mScansManager = mScansManager = (ScansManager) getSystemService("scans");
        if (mScansManager != null) {
            inputmode = mScansManager.getInputMode();
            addExtra = mScansManager.getExtras();
        }
    }

    public final static boolean isNumeric(String s) {
        if (s != null && !"".equals(s.trim()))
            return s.matches("^[0-9]*$");
        else
            return false;
    }

    private void initView() {

        scanBtn = (Button) findViewById(R.id.scanbtn);
        scanBtn.setOnClickListener(this);
        //scanBtn.setText("开始扫描");

        stopbtn = (Button) findViewById(R.id.stopbtn);
        stopbtn.setOnClickListener(this);


        mAsBroacastRadioButton = (RadioButton) findViewById(R.id.asBroacastRadioButton);
        mAsBroacastRadioButton.setOnClickListener(this);

        mAsInputRadioButton = (RadioButton) findViewById(R.id.asInputRadioButton);
        mAsInputRadioButton.setOnClickListener(this);

        mAsBroacastRadioButton.setChecked(false);
        mAsInputRadioButton.setChecked(false);

        if (inputmode == BROADCAST_INPUT_MODE) {
            mAsBroacastRadioButton.setChecked(true);
        } else if (inputmode == INPUTTEXT_INPUT_MODE) {
            mAsInputRadioButton.setChecked(true);
        }

        mAddNone = (RadioButton) findViewById(R.id.addnone);
        mAddNone.setOnClickListener(this);

        mAddEnter = (RadioButton) findViewById(R.id.addEnter);
        mAddEnter.setOnClickListener(this);
        mAddNone.setChecked(false);
        mAddEnter.setChecked(false);

        if (addExtra == NON_EXTRA) {
            mAddNone.setChecked(true);
        } else if (addExtra == ENTER_EXTRA) {
            mAddEnter.setChecked(true);
        }
        et = (EditText) findViewById(R.id.content_et);

    }

    public void showSoftInputFromWindow(EditText editText) {
        editText.setFocusable(true);
        editText.setFocusableInTouchMode(true);
        editText.requestFocus();

    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_scan_demo);
        initdata();
        initView();
        initBroadcast();
        showSoftInputFromWindow(et);
    }

    @Override
    protected void onResume() {
        super.onResume();

    }

    @Override
    protected void onPause() {
        super.onPause();
        mScansManager.stopScan();
    }

    @Override
    public void onClick(View v) {
        int id = v.getId();
        switch (id) {
            case R.id.scanbtn:
                /*
                if(scan){
                    mScansManager.stopScan();
                    scanBtn.setText("开始扫描");

                }else{
                    mScansManager.stopScan();
                    mScansManager.startScan();

                    scanBtn.setText("停止扫描");

                }

               */
                //mScansManager.stopScan();
                mScansManager.startScan();

                break;
            case R.id.stopbtn:
                mScansManager.stopScan();

                break;
            case R.id.asBroacastRadioButton:
                mAsInputRadioButton.setChecked(false);
                mAsBroacastRadioButton.setChecked(true);
                mScansManager.setInputMode(BROADCAST_INPUT_MODE);
                break;
            case R.id.asInputRadioButton:
                mAsInputRadioButton.setChecked(true);
                mAsBroacastRadioButton.setChecked(false);
                mScansManager.setInputMode(INPUTTEXT_INPUT_MODE);
                break;
            case R.id.addnone:
                mAddEnter.setChecked(false);
                mAddNone.setChecked(true);
                mScansManager.setExtras(NON_EXTRA);
                break;
            case R.id.addEnter:
                mAddEnter.setChecked(true);
                mAddNone.setChecked(false);
                mScansManager.setExtras(ENTER_EXTRA);
                break;

        }
    }


    class MyReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {
            if (intent.getAction().equals("com.safeuem.doublebird.appReadyToInstall")) {
                et.append("com.safeuem.doublebird.appReadyToInstall");
            } else {
                et.append("Broadcast:" + intent.getStringExtra("barcode") + "\n");
            }
        }
    }

    @Override
    public void onBackPressed() {
        // 这里处理逻辑代码，大家注意：该方法仅适用于2.0或更新版的sdk
        return;
    }

    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            finish();
        }
        return true;
    }
}
