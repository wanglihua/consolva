package com.huadu.shunda.ui.expressdump;

import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;

import com.huadu.shunda.R;
import com.huadu.shunda.ui.BaseActivity;

public class ExpressDumpMainActivity extends BaseActivity {

    private TextView tvNoPickupDump = null;
    private TextView tvNoProcessDump = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_dump_main);

        tvNoPickupDump = findViewById(R.id.tvNoPickupDump);
        tvNoPickupDump.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(ExpressDumpMainActivity.this, ExpressNoPickupDumpWarehouseListActivity.class);
                startActivity(intent);
            }
        });

        tvNoProcessDump = findViewById(R.id.tvNoProcessDump);
        tvNoProcessDump.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(ExpressDumpMainActivity.this, ExpressNoProcessDumpWarehouseListActivity.class);
                startActivity(intent);
            }
        });
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if (item.getItemId() == android.R.id.home) {
            finish();

            /*
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
             */
        }

        return true;
    }
}