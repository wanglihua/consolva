package com.huadu.shunda.ui.unload;

import android.app.Dialog;
import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.Toast;

import com.huadu.shunda.R;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.stockplace.StockPlaceAddResponseInfo;
import com.huadu.shunda.http.StockPlaceRequest;
import com.huadu.shunda.ui.BaseActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockPlaceAddActivity extends BaseActivity {

    private int shipContainerId;

    private AutoCompleteTextView etStockPlaceName = null;
    private Button btnAdd = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_place_add);

        shipContainerId = getIntent().getIntExtra("shipContainerId", 0);

        etStockPlaceName = findViewById(R.id.etStockPlaceName);

        btnAdd = findViewById(R.id.btnAdd);
        btnAdd.setOnClickListener(new ButtonAddClickListener());

    }

    private class ButtonAddClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            String stockPlaceName = etStockPlaceName.getText().toString();

            if ("".equals(stockPlaceName)) {
                Toast.makeText(StockPlaceAddActivity.this, "托盘名称不能为空！", Toast.LENGTH_SHORT).show();
                return;
            }

            StockPlaceRequest stockPlaceRequest = RetrofitUtil.createRequest(StockPlaceRequest.class);
            Call<StockPlaceAddResponseInfo> addCall = stockPlaceRequest.add(shipContainerId, stockPlaceName);
            Dialog mask = MaskDialog.show(StockPlaceAddActivity.this);
            addCall.enqueue(new Callback<StockPlaceAddResponseInfo>() {
                @Override
                public void onResponse(Call<StockPlaceAddResponseInfo> call, Response<StockPlaceAddResponseInfo> response) {
                    mask.dismiss();

                    StockPlaceAddResponseInfo respData = response.body();

                    if (!RetrofitUtil.processResponse(response, StockPlaceAddActivity.this)) {
                        return;
                    }

                    SoundUtil.playSuccess(StockPlaceAddActivity.this);
                    
                    Intent intent = new Intent(StockPlaceAddActivity.this, StockPlaceDetailActivity.class);
                    intent.putExtra("stockPlaceId", (int) respData.getId());
                    intent.putExtra("shipContainerId", shipContainerId);
                    startActivity(intent);
                }

                @Override
                public void onFailure(Call<StockPlaceAddResponseInfo> call, Throwable t) {
                    Toast.makeText(StockPlaceAddActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();

            /*
            Intent intent = new Intent(this, StockPlaceListActivity.class);
            intent.putExtra("shipContainerId", shipContainerId);
            startActivity(intent);
             */
        }

        return true;
    }
}
