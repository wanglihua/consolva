package com.huadu.shunda.ui;

import android.app.Dialog;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Toast;

import com.huadu.shunda.BuildConfig;
import com.huadu.shunda.R;
import com.huadu.shunda.core.Config;
import com.huadu.shunda.core.LoginCache;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SettingUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.login.LoginResponseInfo;
import com.huadu.shunda.http.LoginRequest;
import com.huadu.shunda.ui.pda.Pda2ScannerUtil;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends BaseActivity {

    private AutoCompleteTextView etLoginName = null;
    private EditText etPassword = null;
    private Button btnLogin = null;
    private Spinner spHost = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        etLoginName = findViewById(R.id.etLoginName);
        etPassword = findViewById(R.id.etPassword);
        spHost = findViewById(R.id.spHost);

        btnLogin = findViewById(R.id.btnLogin);
        btnLogin.setOnClickListener(new ButtonLoginClickListener());

        if (BuildConfig.DEBUG) {
            String savedLoginName = SettingUtil.getLoginName();
            String savedPassword = SettingUtil.getPassword();

            if (!"".equals(savedLoginName) && !"".equals(savedPassword)) {
                etLoginName.setText(SettingUtil.getLoginName());
                etPassword.setText(SettingUtil.getPassword());
            }
        }

        String host = SettingUtil.getHost();
        spHost.setAdapter(new ArrayAdapter<String>(this, android.R.layout.simple_spinner_dropdown_item, Config.getHostList()));

        List<String> hostList = Config.getHostList();

        int hostCount = hostList.size();
        for (int index = 0; index < hostCount; index++) {
            if (host.equals(hostList.get(index))) {
                spHost.setSelection(index);
            }
        }


        //添加Spinner事件监听
        spHost.setOnItemSelectedListener(new Spinner.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                String host = hostList.get(position);

                SettingUtil.saveHost(host);
                Config.setHost(host);

                //设置显示当前选择的项
                parent.setVisibility(View.VISIBLE);
            }

            @Override
            public void onNothingSelected(AdapterView<?> arg0) {
            }
        });

        // 先在应用启动的时候初始话好
        // 后面过程中多次调用，以便加快反应速度
        Pda2ScannerUtil.Connect(this);
    }

    @Override
    protected void setupHomeButton() {
    }

    private class ButtonLoginClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            String loginName = etLoginName.getText().toString();
            String password = etPassword.getText().toString();

            final String loginNameFinal = loginName;
            final String passwordFinal = password;

            if ("".equals(loginNameFinal) || "".equals(passwordFinal)) {
                Toast.makeText(LoginActivity.this, "用户名或者密码不能为空！", Toast.LENGTH_SHORT).show();
                return;
            }

            // 保存 Host 选择
            String host = spHost.getSelectedItem().toString();
            SettingUtil.saveHost(host);
            Config.setHost(host);

            LoginRequest loginRequest = RetrofitUtil.createRequest(LoginRequest.class);
            Call<LoginResponseInfo> loginCall = loginRequest.login(loginNameFinal, passwordFinal);
            Dialog mask = MaskDialog.show(LoginActivity.this);
            loginCall.enqueue(new Callback<LoginResponseInfo>() {
                @Override
                public void onResponse(Call<LoginResponseInfo> call, Response<LoginResponseInfo> response) {
                    mask.dismiss();

                    if (!RetrofitUtil.processResponse(response, LoginActivity.this)) {
                        return;
                    }

                    LoginResponseInfo respData = response.body();

                    LoginCache.setUserId(respData.getUserId());
                    LoginCache.setLoginName(loginNameFinal);
                    LoginCache.setPassword(passwordFinal);
                    LoginCache.setLoginResponse(respData);

                    if (BuildConfig.DEBUG) {
                        SettingUtil.saveLoginName(loginNameFinal);
                        SettingUtil.savePassword(passwordFinal);
                    }

                    SoundUtil.playSuccess(LoginActivity.this);

                    // Intent intent = new Intent(LoginActivity.this, MainActivity.class);
                    // startActivity(intent);
                    finish();
                }

                @Override
                public void onFailure(Call<LoginResponseInfo> call, Throwable t) {
                    Toast.makeText(LoginActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        }
    }

    @Override
    public void onBackPressed() {
        finish();

        if (MainActivity.mainActivity != null) {
            MainActivity.mainActivity.finish();
        }
    }
}
