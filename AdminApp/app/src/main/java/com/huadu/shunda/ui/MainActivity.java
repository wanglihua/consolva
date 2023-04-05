package com.huadu.shunda.ui;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.huadu.shunda.R;
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.login.LoginResponseData;
import com.huadu.shunda.ui.expressdump.ExpressDumpMainActivity;
import com.huadu.shunda.ui.expressshelf.ExpressOffShelfWarehouseListActivity;
import com.huadu.shunda.ui.expressshelf.ExpressOnShelfActivity;
import com.huadu.shunda.ui.expressshelf.ExpressPackingActivity;
import com.huadu.shunda.ui.expressshelf.ExpressReturnOffShelfWarehouseListActivity;
import com.huadu.shunda.ui.expressshelf.ExpressWarehouseCheckShelfListActivity;
import com.huadu.shunda.ui.photo.PhotoHomeActivity;
import com.huadu.shunda.ui.query.CustomerPackBoxStockPlaceActivity;
import com.huadu.shunda.ui.query.ExpressPackageQueryActivity;
import com.huadu.shunda.ui.query.PackBoxDetailActivity;
import com.huadu.shunda.ui.query.PackBoxShipActivity;
import com.huadu.shunda.update.UpdateManager;

import java.util.ArrayList;
import java.util.List;

// import permissions.dispatcher.RuntimePermissions;

// @RuntimePermissions
public class MainActivity extends BaseActivity {

    public static MainActivity mainActivity = null;

    private TextView tvExpressOnShelf = null;
    private TextView tvExpressWarehouseCheck = null;
    private TextView tvExpressOffShelf = null;
    private TextView tvExpressReturnOffShelf = null;
    private TextView tvExpressDump = null;
    private TextView tvExpressPacking = null;
    private TextView tvOrderPayDetail = null;
    private TextView tvPackBoxDetail = null;
    private TextView tvCustomerPackBoxStockPlace = null;
    private TextView tvExpressPackageQuery = null;
    private TextView tvOrderPhoto = null;
    private TextView tvSoftUpdate = null;

    private Button btnTest = null;
    private Button btnPdaScanTest = null;

    private UpdateManager updateManager = null; // 后面记得初始化

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        try {
            RetrofitUtil.versionName = getPackageManager().getPackageInfo(getPackageName(), 0).versionName;
            // RetrofitUtil.deviceData = Build.MANUFACTURER + "##" + Build.PRODUCT + "##" + Build.BRAND + "##" + Build.MODEL + "##" + Build.DEVICE + "##" + Build.FINGERPRINT;
            RetrofitUtil.deviceData = Build.VERSION.SDK_INT + "##" + Build.VERSION.RELEASE + "##" + Build.FINGERPRINT;
        } catch (Exception ignored) {
        }

        // 避免从桌面启动程序后，会重新实例化入口类的activity
        if (!this.isTaskRoot()) { // 当前类不是该Task的根部，那么之前启动
            Intent intent = getIntent();
            if (intent != null) {
                String action = intent.getAction();
                if (intent.hasCategory(Intent.CATEGORY_LAUNCHER) && Intent.ACTION_MAIN.equals(action)) { // 当前类是从桌面启动的
                    super.onCreate(savedInstanceState);
                    finish(); // finish掉该类，直接打开该Task中现存的Activity
                    return;
                }
            }
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mainActivity = this;

        updateManager = new UpdateManager(MainActivity.this);

        tvExpressOnShelf = findViewById(R.id.tvExpressOnShelf);
        tvExpressOnShelf.setOnClickListener(new TextViewExpressOnShelfClickListener());

        tvExpressWarehouseCheck = findViewById(R.id.tvExpressWarehouseCheck);
        tvExpressWarehouseCheck.setOnClickListener(new TextViewWarehouseCheckClickListener());

        tvExpressOffShelf = findViewById(R.id.tvExpressOffShelf);
        tvExpressOffShelf.setOnClickListener(new TextViewExpressOffShelfClickListener());

        tvExpressReturnOffShelf = findViewById(R.id.tvExpressReturnOffShelf);
        tvExpressReturnOffShelf.setOnClickListener(new TextViewExpressReturnOffShelfClickListener());

        tvExpressDump = findViewById(R.id.tvExpressDump);
        tvExpressDump.setOnClickListener(new TextViewExpressDumpClickListener());

        tvExpressPacking = findViewById(R.id.tvExpressPacking);
        tvExpressPacking.setOnClickListener(new TextViewExpressPackingClickListener());

        tvPackBoxDetail = findViewById(R.id.tvPackBoxDetail);
        tvPackBoxDetail.setOnClickListener(new TextViewPackBoxDetailClickListener());

        tvCustomerPackBoxStockPlace = findViewById(R.id.tvCustomerPackBoxStockPlace);
        tvCustomerPackBoxStockPlace.setOnClickListener(new TextViewCustomerPackBoxStockPlaceClickListener());

        tvOrderPayDetail = findViewById(R.id.tvOrderPayDetail);
        tvOrderPayDetail.setOnClickListener(new TextViewOrderPayDetailClickListener());

        tvExpressPackageQuery = findViewById(R.id.tvExpressPackageQuery);
        tvExpressPackageQuery.setOnClickListener(new TextViewExpressPackageQueryClickListener());

        tvOrderPhoto = findViewById(R.id.tvOrderPhoto);
        tvOrderPhoto.setOnClickListener(new TextViewOrderPhotoClickListener());

        tvSoftUpdate = findViewById(R.id.tvSoftUpdate);
        tvSoftUpdate.setOnClickListener(new TextViewSoftUpdateClickListener());

        btnTest = findViewById(R.id.btnTest);
        btnTest.setOnClickListener(new ButtonTestClickListener());

        btnPdaScanTest = findViewById(R.id.btnPdaScanTest);
        btnPdaScanTest.setOnClickListener(new ButtonPdaScanTestClickListener());

        // 预加载声音文件
        SoundUtil.init(this);
    }

    private void checkPermission() {
        List<String> permissionList = new ArrayList<>();


        if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.READ_EXTERNAL_STORAGE);
        }

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);
        }

        /*
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.FOREGROUND_SERVICE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.FOREGROUND_SERVICE);
        }
         */

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.CAMERA);
        }

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.INTERNET) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.INTERNET);
        }

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.VIBRATE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.VIBRATE);
        }

        /*
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.FLASHLIGHT) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.FLASHLIGHT);
        }
         */

        /*
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.GET_ACCOUNTS) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.GET_ACCOUNTS);
        }
         */

        /*
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_PROFILE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.READ_PROFILE);
        }
         */

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_NETWORK_STATE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.ACCESS_NETWORK_STATE);
        }

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_WIFI_STATE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.ACCESS_WIFI_STATE);
        }

        /*
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.RECORD_AUDIO);
        }
         */

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.REQUEST_INSTALL_PACKAGES) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.REQUEST_INSTALL_PACKAGES);
        }

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CALL_PHONE) != PackageManager.PERMISSION_GRANTED) {
            permissionList.add(Manifest.permission.CALL_PHONE);
        }

        if (permissionList.size() != 0) {
            ActivityCompat.requestPermissions(this, permissionList.toArray(new String[]{}), 0);
        }
    }

    @Override
    public void onStart() {
        super.onStart();

        /*
            检查所有权限
         */

        checkPermission();

        /*
        tvExpressOnShelf.setVisibility(View.GONE);
        tvExpressWarehouseCheck.setVisibility(View.GONE);
        tvExpressOffShelf.setVisibility(View.GONE);
        tvExpressReturnOffShelf.setVisibility(View.GONE);
        tvExpressDump.setVisibility(View.GONE);
        tvExpressPacking.setVisibility(View.GONE);
        tvPackBoxDetail.setVisibility(View.GONE);
        tvCustomerPackBoxStockPlace.setVisibility(View.GONE);
        tvOrderPayDetail.setVisibility(View.GONE);
        tvExpressPackageQuery.setVisibility(View.GONE);
        tvOrderPhoto.setVisibility(View.GONE);
         */

        tvOrderPhoto.setVisibility(View.VISIBLE);

        LoginResponseData respData = LoginCache.getLoginResponse();

        if (respData == null) {
            /*
            // Toast.makeText(MainActivity.this, "当前用户未登录或未能获取权限信息！", Toast.LENGTH_SHORT).show();
            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(MainActivity.this);
            alertDialogBuilder.setTitle("错误提示");
            alertDialogBuilder.setMessage("当前用户未登录或未能获取权限信息！");
            alertDialogBuilder.setPositiveButton("确定", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();
             */

            Intent intent = new Intent(MainActivity.this, LoginActivity.class);
            startActivity(intent);

            return;
        }

        this.setTitle("舜达仓管 - " + respData.getUserName());

        /*

        if (respData.getCanExpressOnShelf()) {
            tvExpressOnShelf.setVisibility(View.VISIBLE);
        } else {
            tvExpressOnShelf.setVisibility(View.GONE);
        }

        if (respData.getCanExpressWarehouseCheck()) {
            tvExpressWarehouseCheck.setVisibility(View.VISIBLE);
        } else {
            tvExpressWarehouseCheck.setVisibility(View.GONE);
        }

        if (respData.getCanExpressOffShelf()) {
            tvExpressOffShelf.setVisibility(View.VISIBLE);
        } else {
            tvExpressOffShelf.setVisibility(View.GONE);
        }

        if (respData.getCanExpressReturnOffShelf()) {
            tvExpressReturnOffShelf.setVisibility(View.VISIBLE);
        } else {
            tvExpressReturnOffShelf.setVisibility(View.GONE);
        }

        if (respData.getCanExpressDump()) {
            tvExpressDump.setVisibility(View.VISIBLE);
        } else {
            tvExpressDump.setVisibility(View.GONE);
        }

        if (respData.getCanExpressPacking()) {
            tvExpressPacking.setVisibility(View.VISIBLE);
        } else {
            tvExpressPacking.setVisibility(View.GONE);
        }

        if (respData.getCanPackBoxDetail()) {
            tvPackBoxDetail.setVisibility(View.VISIBLE);
        } else {
            tvPackBoxDetail.setVisibility(View.GONE);
        }

        if (respData.getCanCustomerPackBoxStockPlace()) {
            tvCustomerPackBoxStockPlace.setVisibility(View.VISIBLE);
        } else {
            tvCustomerPackBoxStockPlace.setVisibility(View.GONE);
        }

        if (respData.getCanOrderPayDetail()) {
            tvOrderPayDetail.setVisibility(View.VISIBLE);
        } else {
            tvOrderPayDetail.setVisibility(View.GONE);
        }

        if (respData.getCanExpressPackageQuery()) {
            tvExpressPackageQuery.setVisibility(View.VISIBLE);
        } else {
            tvExpressPackageQuery.setVisibility(View.GONE);
        }

        if (respData.getCanOrderPhoto()) {
            tvOrderPhoto.setVisibility(View.VISIBLE);
        } else {
            tvOrderPhoto.setVisibility(View.GONE);
        }

         */

        UpdateManager.CheckNewVersionResult checkNewVersionResult = updateManager.checkNewVersion();
        if (checkNewVersionResult == UpdateManager.CheckNewVersionResult.ToUpdate) {
            tvSoftUpdate.setVisibility(View.VISIBLE);
        } else {
            tvSoftUpdate.setVisibility(View.GONE);
        }
    }

    private class TextViewExpressOnShelfClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressOnShelfActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewWarehouseCheckClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressWarehouseCheckShelfListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewExpressOffShelfClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressOffShelfWarehouseListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewExpressReturnOffShelfClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressReturnOffShelfWarehouseListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewExpressDumpClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressDumpMainActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewExpressPackingClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressPackingActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewPackBoxDetailClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxDetailActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewCustomerPackBoxStockPlaceClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, CustomerPackBoxStockPlaceActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewOrderPayDetailClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxShipActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewExpressPackageQueryClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, ExpressPackageQueryActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewOrderPhotoClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PhotoHomeActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewSoftUpdateClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            /*
            String versionName = UpdateManager.getAppVersionName(MainActivity.this);
            Toast.makeText(MainActivity.this, versionName, Toast.LENGTH_LONG).show();
             */

            // 检查软件更新
            updateManager.checkUpdate();
        }
    }

    private class ButtonTestClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, TestActivity.class);
            startActivity(intent);
        }
    }

    private class ButtonPdaScanTestClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_main, menu);

        // 菜单 退出登录
        MenuItem menuItemLogout = menu.findItem(R.id.menu_item_logout);
        menuItemLogout.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem menuItem) {
                gotoLogin();
                return true;
            }
        });

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            gotoLogin();
        }

        return true;
    }

    private void gotoLogin() {

        // 清除掉登录缓存
        LoginCache.clear();

        Intent intent = new Intent(MainActivity.this, LoginActivity.class);
        startActivity(intent);
    }

    @Override
    public void onBackPressed() {
        // moveTaskToBack(true);
        finish();
    }

    @Override
    public void finish() {
        super.finish();
        // moveTaskToBack(true);
    }
}
