package com.huadu.iepoch.ui;

import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.http.KeepLoginRequest;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class BaseActivity extends AppCompatActivity {

    @Override
    protected void onCreate(@Nullable Bundle bundle) {
        super.onCreate(bundle);
        setupHomeButton();
    }

    protected void setupHomeButton() {
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setHomeButtonEnabled(true);
            actionBar.setDisplayHomeAsUpEnabled(true);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();
        }

        return true;
    }

    @Override
    protected void onRestart() {
        super.onRestart();

        KeepLoginRequest loginRequest = RetrofitUtil.createRequest(KeepLoginRequest.class);
        Call<ResponseInfo> loginCall = loginRequest.check();
        loginCall.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                ResponseInfo respData = response.body();

                if (respData == null) {
                    SoundUtil.playError(BaseActivity.this);

                    // Toast.makeText(BaseActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                    AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(BaseActivity.this);
                    alertDialogBuilder.setTitle("错误提示");
                    alertDialogBuilder.setMessage("未能接收到服务器返回数据！");
                    alertDialogBuilder.setPositiveButton("确定", null);

                    AlertDialog alertDialog = alertDialogBuilder.create();
                    alertDialog.show();

                    return;
                }

                if (!respData.getSuccess()) {
                    Intent intent = new Intent(BaseActivity.this, LoginActivity.class);
                    startActivity(intent);

                    return;
                }

            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(BaseActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });

    }
}
