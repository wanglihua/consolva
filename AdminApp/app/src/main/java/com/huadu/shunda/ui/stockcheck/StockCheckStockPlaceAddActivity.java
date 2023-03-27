package com.huadu.shunda.ui.stockcheck;

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
import com.huadu.shunda.data.http.stockcheck.StockCheckStockPlaceAddResponseInfo;
import com.huadu.shunda.http.StockCheckRequest;
import com.huadu.shunda.ui.BaseActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StockCheckStockPlaceAddActivity extends BaseActivity {

    private int billId;

    private AutoCompleteTextView etStockPlaceName = null;
    private Button btnAdd = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stock_check_stock_place_add);

        billId = getIntent().getIntExtra("billId", 0);

        etStockPlaceName = findViewById(R.id.etStockPlaceName);

        btnAdd = findViewById(R.id.btnAdd);
        btnAdd.setOnClickListener(new ButtonAddClickListener());

    }

    private class ButtonAddClickListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            String stockPlaceName = etStockPlaceName.getText().toString();

            if ("".equals(stockPlaceName)) {
                Toast.makeText(StockCheckStockPlaceAddActivity.this, "托盘名称不能为空！", Toast.LENGTH_SHORT).show();
                return;
            }

            StockCheckRequest stockCheckRequest = RetrofitUtil.createRequest(StockCheckRequest.class);
            Call<StockCheckStockPlaceAddResponseInfo> addCall = stockCheckRequest.stockPlaceAdd(billId, stockPlaceName);
            Dialog mask = MaskDialog.show(StockCheckStockPlaceAddActivity.this);
            addCall.enqueue(new Callback<StockCheckStockPlaceAddResponseInfo>() {
                @Override
                public void onResponse(Call<StockCheckStockPlaceAddResponseInfo> call, Response<StockCheckStockPlaceAddResponseInfo> response) {
                    mask.dismiss();

                    StockCheckStockPlaceAddResponseInfo respData = response.body();

                    if (!RetrofitUtil.processResponse(response, StockCheckStockPlaceAddActivity.this)) {
                        return;
                    }

                    // finish();

                    SoundUtil.playSuccess(StockCheckStockPlaceAddActivity.this);

                    Intent intent = new Intent(StockCheckStockPlaceAddActivity.this, StockCheckStockPlaceDetailActivity.class);
                    intent.putExtra("stockPlaceId", (int) respData.getId());
                    startActivity(intent);
                }

                @Override
                public void onFailure(Call<StockCheckStockPlaceAddResponseInfo> call, Throwable t) {
                    Toast.makeText(StockCheckStockPlaceAddActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();
        }

        return true;
    }
}
