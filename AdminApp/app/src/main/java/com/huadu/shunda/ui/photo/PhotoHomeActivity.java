package com.huadu.shunda.ui.photo;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;

import com.huadu.shunda.R;
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.data.domain.OrderPhotoType;
import com.huadu.shunda.ui.BaseActivity;

public class PhotoHomeActivity extends BaseActivity {

    private TextView tvPhotoSignIn = null;
    private TextView tvPhotoCheckIn = null;
    private TextView tvPhotoIssue = null;
    private TextView tvPhotoCnDamage = null;
    private TextView tvPhotoReturn = null;
    private TextView tvPhotoShip = null;
    private TextView tvPhotoAuDamage = null;
    private TextView tvPhotoDeliver = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_photo_home);

        tvPhotoSignIn = findViewById(R.id.tvPhotoSignIn);
        tvPhotoSignIn.setOnClickListener(new TextViewPhotoSignInClickListener());

        tvPhotoCheckIn = findViewById(R.id.tvPhotoCheckIn);
        tvPhotoCheckIn.setOnClickListener(new TextViewPhotoCheckInClickListener());

        tvPhotoIssue = findViewById(R.id.tvPhotoIssue);
        tvPhotoIssue.setOnClickListener(new TextViewPhotoIssueClickListener());

        tvPhotoCnDamage = findViewById(R.id.tvPhotoCnDamage);
        tvPhotoCnDamage.setOnClickListener(new TextViewPhotoCnDamageClickListener());

        tvPhotoReturn = findViewById(R.id.tvPhotoReturn);
        tvPhotoReturn.setOnClickListener(new TextViewPhotoReturnClickListener());

        tvPhotoShip = findViewById(R.id.tvPhotoShip);
        tvPhotoShip.setOnClickListener(new TextViewPhotoShipClickListener());

        tvPhotoAuDamage = findViewById(R.id.tvPhotoAuDamage);
        tvPhotoAuDamage.setOnClickListener(new TextViewPhotoAuDamageClickListener());

        tvPhotoDeliver = findViewById(R.id.tvPhotoDeliver);
        tvPhotoDeliver.setOnClickListener(new TextViewPhotoDeliverClickListener());

        checkGrantExternalRW();
    }

    @Override
    protected void onStart() {
        super.onStart();

        tvPhotoSignIn.setVisibility(View.GONE);
        tvPhotoCheckIn.setVisibility(View.GONE);
        tvPhotoIssue.setVisibility(View.GONE);
        tvPhotoCnDamage.setVisibility(View.GONE);
        tvPhotoReturn.setVisibility(View.GONE);
        tvPhotoShip.setVisibility(View.GONE);
        tvPhotoAuDamage.setVisibility(View.GONE);
        tvPhotoDeliver.setVisibility(View.GONE);

        if (LoginCache.getLoginResponse().getCanOrderPhotoSignIn()) {
            tvPhotoSignIn.setVisibility(View.VISIBLE);
        } else {
            tvPhotoSignIn.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoCheckIn()) {
            tvPhotoCheckIn.setVisibility(View.VISIBLE);
        } else {
            tvPhotoCheckIn.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoIssue()) {
            tvPhotoIssue.setVisibility(View.VISIBLE);
        } else {
            tvPhotoIssue.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoCnDamage()) {
            tvPhotoCnDamage.setVisibility(View.VISIBLE);
        } else {
            tvPhotoCnDamage.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoReturn()) {
            tvPhotoReturn.setVisibility(View.VISIBLE);
        } else {
            tvPhotoReturn.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoShip()) {
            tvPhotoShip.setVisibility(View.VISIBLE);
        } else {
            tvPhotoShip.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoAuDamage()) {
            tvPhotoAuDamage.setVisibility(View.VISIBLE);
        } else {
            tvPhotoAuDamage.setVisibility(View.GONE);
        }

        if (LoginCache.getLoginResponse().getCanOrderPhotoDeliver()) {
            tvPhotoDeliver.setVisibility(View.VISIBLE);
        } else {
            tvPhotoDeliver.setVisibility(View.GONE);
        }
    }

    private class TextViewPhotoSignInClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoExpressActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.SignIn.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoCheckInClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoExpressActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.CheckIn.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoIssueClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoExpressActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.Issue.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoCnDamageClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoExpressActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.CnDamage.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoReturnClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoExpressActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.Return.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoShipClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoOrderActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.Ship.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoAuDamageClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoOrderActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.AuDamage.getId());
            startActivity(intent);
        }
    }

    private class TextViewPhotoDeliverClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(PhotoHomeActivity.this, PhotoDeliverActivity.class);
            intent.putExtra("photoTypeId", OrderPhotoType.Deliver.getId());
            startActivity(intent);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            finish();
        }
        return true;
    }

    private void checkGrantExternalRW() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M
                && (this.checkSelfPermission(Manifest.permission.READ_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED
                || this.checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
        ) {
            this.requestPermissions(new String[]{
                    Manifest.permission.READ_EXTERNAL_STORAGE,
                    Manifest.permission.WRITE_EXTERNAL_STORAGE
            }, 1);
        }
    }
}
