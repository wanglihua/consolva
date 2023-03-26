package com.huadu.iepoch.ui;

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

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.LoginCache;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.login.LoginResponseInfo;
import com.huadu.iepoch.ui.expressdump.ExpressDumpMainActivity;
import com.huadu.iepoch.ui.expressshelf.ExpressOffShelfWarehouseListActivity;
import com.huadu.iepoch.ui.expressshelf.ExpressOnShelfActivity;
import com.huadu.iepoch.ui.expressshelf.ExpressPackingActivity;
import com.huadu.iepoch.ui.expressshelf.ExpressReturnOffShelfWarehouseListActivity;
import com.huadu.iepoch.ui.expressshelf.ExpressWarehouseCheckShelfListActivity;
import com.huadu.iepoch.ui.handover.TerminalHandoverStationListActivity;
import com.huadu.iepoch.ui.labelprint.PackBoxLabelPrintActivity;
import com.huadu.iepoch.ui.labelprint.PackBoxLabelTlaPrintActivity;
import com.huadu.iepoch.ui.load.PackBoxLoadShipContainerInnerListActivity;
import com.huadu.iepoch.ui.photo.PhotoHomeActivity;
import com.huadu.iepoch.ui.pickup.booking.PickupBookingStationListActivity;
import com.huadu.iepoch.ui.pickup.booking.PickupBookingWarehouseListActivity;
import com.huadu.iepoch.ui.pickup.homedeliverbatch.HomeDeliverBatchWarehouseListActivity;
import com.huadu.iepoch.ui.pickup.homedeliverpickup.HomeDeliverPickupWarehouseListActivity;
import com.huadu.iepoch.ui.pickup.logisticshomedeliverpickup.LogisticsHomeDeliverPickupWarehouseListActivity;
import com.huadu.iepoch.ui.pickup.terminal.TerminalPickupStationListActivity;
import com.huadu.iepoch.ui.pickup.warehouse.WarehousePickupWarehouseListActivity;
import com.huadu.iepoch.ui.query.CustomerPackBoxStockPlaceActivity;
import com.huadu.iepoch.ui.query.ExpressPackageQueryActivity;
import com.huadu.iepoch.ui.query.PackBoxDetailActivity;
import com.huadu.iepoch.ui.query.PackBoxOrderPayDetailActivity;
import com.huadu.iepoch.ui.query.PackBoxStockPlaceActivity;
import com.huadu.iepoch.ui.stockcheck.StockCheckWarehouseListActivity;
import com.huadu.iepoch.ui.stockmove.StockMoveWarehouseListActivity;
import com.huadu.iepoch.ui.unload.PackBoxUnloadShipContainerInnerListActivity;
import com.huadu.iepoch.ui.unload.UnloadSortShipContainerListActivity;
import com.huadu.iepoch.update.UpdateManager;

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
    private TextView tvLoad = null;
    private TextView tvUnload = null;
    private TextView tvUnloadSort = null;
    private TextView tvPickupBookingWarehouse = null;
    private TextView tvPickupWarehouse = null;
    private TextView tvOrderPayDetail = null;
    private TextView tvPackBoxDetail = null;
    private TextView tvPackBoxStockPlace = null;
    private TextView tvCustomerPackBoxStockPlace = null;
    private TextView tvStockCheck = null;
    private TextView tvStockMove = null;
    private TextView tvExpressPackageQuery = null;
    private TextView tvTerminalHandover = null;
    private TextView tvTerminalPickupBooking = null;
    private TextView tvTerminalPickup = null;
    private TextView tvHomeDeliver = null;
    private TextView tvHomeDeliverBatch = null;
    private TextView tvLogisticsHomeDeliver = null;
    private TextView tvOrderPhoto = null;
    private TextView tvPackBoxLabelPrint = null;
    private TextView tvPackBoxLabelTlaPrint = null;
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

        tvLoad = findViewById(R.id.tvLoad);
        tvLoad.setOnClickListener(new TextViewLoadClickListener());

        tvUnload = findViewById(R.id.tvUnload);
        tvUnload.setOnClickListener(new TextViewUnloadClickListener());

        tvUnloadSort = findViewById(R.id.tvUnloadSort);
        tvUnloadSort.setOnClickListener(new TextViewUnloadSortClickListener());

        tvPickupBookingWarehouse = findViewById(R.id.tvPickupBookingWarehouse);
        tvPickupBookingWarehouse.setOnClickListener(new TextViewPickupBookingWarehouseClickListener());

        tvPickupWarehouse = findViewById(R.id.tvPickupWarehouse);
        tvPickupWarehouse.setOnClickListener(new TextViewPickupWarehouseClickListener());

        tvPackBoxDetail = findViewById(R.id.tvPackBoxDetail);
        tvPackBoxDetail.setOnClickListener(new TextViewPackBoxDetailClickListener());

        tvPackBoxStockPlace = findViewById(R.id.tvPackBoxStockPlace);
        tvPackBoxStockPlace.setOnClickListener(new TextViewPackBoxStockPlaceClickListener());

        tvCustomerPackBoxStockPlace = findViewById(R.id.tvCustomerPackBoxStockPlace);
        tvCustomerPackBoxStockPlace.setOnClickListener(new TextViewCustomerPackBoxStockPlaceClickListener());

        tvOrderPayDetail = findViewById(R.id.tvOrderPayDetail);
        tvOrderPayDetail.setOnClickListener(new TextViewOrderPayDetailClickListener());

        tvStockCheck = findViewById(R.id.tvStockCheck);
        tvStockCheck.setOnClickListener(new TextViewStockCheckClickListener());

        tvStockMove = findViewById(R.id.tvStockMove);
        tvStockMove.setOnClickListener(new TextViewStockMoveClickListener());

        tvExpressPackageQuery = findViewById(R.id.tvExpressPackageQuery);
        tvExpressPackageQuery.setOnClickListener(new TextViewExpressPackageQueryClickListener());

        tvTerminalHandover = findViewById(R.id.tvTerminalHandover);
        tvTerminalHandover.setOnClickListener(new TextViewTerminalHandoverClickListener());

        tvTerminalPickupBooking = findViewById(R.id.tvTerminalPickupBooking);
        tvTerminalPickupBooking.setOnClickListener(new TextViewTerminalPickupBookingClickListener());

        tvTerminalPickup = findViewById(R.id.tvTerminalPickup);
        tvTerminalPickup.setOnClickListener(new TextViewTerminalPickupClickListener());

        tvHomeDeliver = findViewById(R.id.tvHomeDeliver);
        tvHomeDeliver.setOnClickListener(new TextViewHomeDeliverClickListener());

        tvHomeDeliverBatch = findViewById(R.id.tvHomeDeliverBatch);
        tvHomeDeliverBatch.setOnClickListener(new TextViewHomeDeliverBatchClickListener());

        tvLogisticsHomeDeliver = findViewById(R.id.tvLogisticsHomeDeliver);
        tvLogisticsHomeDeliver.setOnClickListener(new TextViewLogisticsHomeDeliverClickListener());

        tvOrderPhoto = findViewById(R.id.tvOrderPhoto);
        tvOrderPhoto.setOnClickListener(new TextViewOrderPhotoClickListener());

        tvPackBoxLabelPrint = findViewById(R.id.tvPackBoxLabelPrint);
        tvPackBoxLabelPrint.setOnClickListener(new TextViewPackBoxLabelPrintClickListener());

        tvPackBoxLabelTlaPrint = findViewById(R.id.tvPackBoxLabelTlaPrint);
        tvPackBoxLabelTlaPrint.setOnClickListener(new TextViewPackBoxLabelTlaPrintClickListener());

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

        tvExpressOnShelf.setVisibility(View.GONE);
        tvExpressWarehouseCheck.setVisibility(View.GONE);
        tvExpressOffShelf.setVisibility(View.GONE);
        tvExpressReturnOffShelf.setVisibility(View.GONE);
        tvExpressDump.setVisibility(View.GONE);
        tvExpressPacking.setVisibility(View.GONE);
        tvLoad.setVisibility(View.GONE);
        tvUnload.setVisibility(View.GONE);
        tvUnloadSort.setVisibility(View.GONE);
        tvPickupBookingWarehouse.setVisibility(View.GONE);
        tvPickupWarehouse.setVisibility(View.GONE);
        tvPackBoxDetail.setVisibility(View.GONE);
        tvPackBoxStockPlace.setVisibility(View.GONE);
        tvCustomerPackBoxStockPlace.setVisibility(View.GONE);
        tvOrderPayDetail.setVisibility(View.GONE);
        tvStockCheck.setVisibility(View.GONE);
        tvStockMove.setVisibility(View.GONE);
        tvExpressPackageQuery.setVisibility(View.GONE);
        tvTerminalHandover.setVisibility(View.GONE);
        tvTerminalPickupBooking.setVisibility(View.GONE);
        tvTerminalPickup.setVisibility(View.GONE);
        tvHomeDeliver.setVisibility(View.GONE);
        tvHomeDeliverBatch.setVisibility(View.GONE);
        tvLogisticsHomeDeliver.setVisibility(View.GONE);
        tvOrderPhoto.setVisibility(View.GONE);
        tvPackBoxLabelPrint.setVisibility(View.GONE);
        tvPackBoxLabelTlaPrint.setVisibility(View.GONE);

        LoginResponseInfo respData = LoginCache.getLoginResponse();

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

        this.setTitle("澳拼仓管 - " + respData.getUserName());

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

        if (respData.getCanLoad()) {
            tvLoad.setVisibility(View.VISIBLE);
        } else {
            tvLoad.setVisibility(View.GONE);
        }

        if (respData.getCanUnload()) {
            tvUnload.setVisibility(View.VISIBLE);
        } else {
            tvUnload.setVisibility(View.GONE);
        }

        if (respData.getCanUnloadSort()) {
            tvUnloadSort.setVisibility(View.VISIBLE);
        } else {
            tvUnloadSort.setVisibility(View.GONE);
        }

        if (respData.getCanWarehousePickupBooking()) {
            tvPickupBookingWarehouse.setVisibility(View.VISIBLE);
        } else {
            tvPickupBookingWarehouse.setVisibility(View.GONE);
        }

        if (respData.getCanWarehousePickup()) {
            tvPickupWarehouse.setVisibility(View.VISIBLE);
        } else {
            tvPickupWarehouse.setVisibility(View.GONE);
        }

        if (respData.getCanPackBoxDetail()) {
            tvPackBoxDetail.setVisibility(View.VISIBLE);
        } else {
            tvPackBoxDetail.setVisibility(View.GONE);
        }

        if (respData.getCanPackBoxStockPlace()) {
            tvPackBoxStockPlace.setVisibility(View.VISIBLE);
        } else {
            tvPackBoxStockPlace.setVisibility(View.GONE);
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

        if (respData.getCanStockCheck()) {
            tvStockCheck.setVisibility(View.VISIBLE);
        } else {
            tvStockCheck.setVisibility(View.GONE);
        }

        if (respData.getCanStockMove()) {
            tvStockMove.setVisibility(View.VISIBLE);
        } else {
            tvStockMove.setVisibility(View.GONE);
        }

        if (respData.getCanExpressPackageQuery()) {
            tvExpressPackageQuery.setVisibility(View.VISIBLE);
        } else {
            tvExpressPackageQuery.setVisibility(View.GONE);
        }

        if (respData.getCanTerminalHandover()) {
            tvTerminalHandover.setVisibility(View.VISIBLE);
        } else {
            tvTerminalHandover.setVisibility(View.GONE);
        }

        if (respData.getCanTerminalPickupBooking()) {
            tvTerminalPickupBooking.setVisibility(View.VISIBLE);
        } else {
            tvTerminalPickupBooking.setVisibility(View.GONE);
        }

        if (respData.getCanTerminalPickup()) {
            tvTerminalPickup.setVisibility(View.VISIBLE);
        } else {
            tvTerminalPickup.setVisibility(View.GONE);
        }

        if (respData.getCanHomeDeliver()) {
            tvHomeDeliver.setVisibility(View.VISIBLE);
        } else {
            tvHomeDeliver.setVisibility(View.GONE);
        }

        if (respData.getCanHomeDeliverBatch()) {
            tvHomeDeliverBatch.setVisibility(View.VISIBLE);
        } else {
            tvHomeDeliverBatch.setVisibility(View.GONE);
        }

        if (respData.getCanLogisticsHomeDeliver()) {
            tvLogisticsHomeDeliver.setVisibility(View.VISIBLE);
        } else {
            tvLogisticsHomeDeliver.setVisibility(View.GONE);
        }

        if (respData.getCanOrderPhoto()) {
            tvOrderPhoto.setVisibility(View.VISIBLE);
        } else {
            tvOrderPhoto.setVisibility(View.GONE);
        }

        if (respData.getCanPackBoxLabelPrint()) {
            tvPackBoxLabelPrint.setVisibility(View.VISIBLE);
        } else {
            tvPackBoxLabelPrint.setVisibility(View.GONE);
        }

        if (respData.getCanPackBoxLabelTlaPrint()) {
            tvPackBoxLabelTlaPrint.setVisibility(View.VISIBLE);
        } else {
            tvPackBoxLabelTlaPrint.setVisibility(View.GONE);
        }

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

    private class TextViewLoadClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxLoadShipContainerInnerListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewUnloadClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxUnloadShipContainerInnerListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewUnloadSortClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, UnloadSortShipContainerListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewPickupBookingWarehouseClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PickupBookingWarehouseListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewPickupWarehouseClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, WarehousePickupWarehouseListActivity.class);
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

    private class TextViewPackBoxStockPlaceClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxStockPlaceActivity.class);
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
            Intent intent = new Intent(MainActivity.this, PackBoxOrderPayDetailActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewStockCheckClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, StockCheckWarehouseListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewStockMoveClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, StockMoveWarehouseListActivity.class);
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

    private class TextViewTerminalHandoverClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, TerminalHandoverStationListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewTerminalPickupBookingClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PickupBookingStationListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewTerminalPickupClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, TerminalPickupStationListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewHomeDeliverClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, HomeDeliverPickupWarehouseListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewHomeDeliverBatchClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, HomeDeliverBatchWarehouseListActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewLogisticsHomeDeliverClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, LogisticsHomeDeliverPickupWarehouseListActivity.class);
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

    private class TextViewPackBoxLabelPrintClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxLabelPrintActivity.class);
            startActivity(intent);
        }
    }

    private class TextViewPackBoxLabelTlaPrintClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, PackBoxLabelTlaPrintActivity.class);
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
