package com.huadu.shunda.ui.query;

import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.huadu.shunda.R;
import com.huadu.shunda.core.MaskDialog;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.http.goodsexpress.GoodsExpressQueryResponseInfo;
import com.huadu.shunda.http.GoodsExpressRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ExpressPackageQueryActivity extends BaseActivity {

    private static final int REQ_QR_CODE_EXPRESS_TRACKING_NO = 1;

    private TextView tvExpressTrackingNo = null;
    private TextView tvIssueGoods = null;
    private TextView tvIssueGoodsDesc1 = null;
    private TextView tvIssueGoodsDesc2 = null;
    private TextView tvClaimGoods = null;
    private TextView tvClaimGoodsDesc1 = null;
    private TextView tvClaimGoodsDesc2 = null;
    private TextView tvOrderExpress = null;
    private TextView tvOrderExpressDesc = null;

    private Button btnScan = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_express_package_query);

        tvExpressTrackingNo = findViewById(R.id.tvExpressTrackingNo);
        tvIssueGoods = findViewById(R.id.tvIssueGoods);
        tvIssueGoodsDesc1 = findViewById(R.id.tvIssueGoodsDesc1);
        tvIssueGoodsDesc2 = findViewById(R.id.tvIssueGoodsDesc2);
        tvClaimGoods = findViewById(R.id.tvClaimGoods);
        tvClaimGoodsDesc1 = findViewById(R.id.tvClaimGoodsDesc1);
        tvClaimGoodsDesc2 = findViewById(R.id.tvClaimGoodsDesc2);
        tvOrderExpress = findViewById(R.id.tvOrderExpress);
        tvOrderExpressDesc = findViewById(R.id.tvOrderExpressDesc);

        btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_EXPRESS_TRACKING_NO);
            }
        });
    }

    private void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描快递单号二维码");
            startActivityForResult(intent, requestCode);
        } else {
            if (PermissionUtil.checkCameraStoragePermissions(ExpressPackageQueryActivity.this, requestCode) == false) {
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "扫描快递单号二维码");
            startActivityForResult(intent, requestCode);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK) {
            Bundle bundle = data.getExtras();
            final String expressTrackingNo = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);

            if (requestCode == REQ_QR_CODE_EXPRESS_TRACKING_NO) {

                GoodsExpressRequest goodsExpressRequest = RetrofitUtil.createRequest(GoodsExpressRequest.class);
                Call<GoodsExpressQueryResponseInfo> queryCall = goodsExpressRequest.query(expressTrackingNo);
                Dialog mask = MaskDialog.show(ExpressPackageQueryActivity.this);
                queryCall.enqueue(new Callback<GoodsExpressQueryResponseInfo>() {
                    @Override
                    public void onResponse(Call<GoodsExpressQueryResponseInfo> call, Response<GoodsExpressQueryResponseInfo> response) {
                        mask.dismiss();

                        GoodsExpressQueryResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, ExpressPackageQueryActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(ExpressPackageQueryActivity.this);

                        tvExpressTrackingNo.setText("快递单号：" + expressTrackingNo);

                        if (respData.isIssueGoods()) {
                            tvIssueGoods.setVisibility(View.VISIBLE);
                            tvIssueGoodsDesc1.setVisibility(View.VISIBLE);
                            tvIssueGoodsDesc2.setVisibility(View.VISIBLE);
                            tvIssueGoodsDesc1.setText(respData.getIssueGoodsDesc1());
                            tvIssueGoodsDesc2.setText(respData.getIssueGoodsDesc2());
                        } else {
                            tvIssueGoods.setVisibility(View.GONE);
                            tvIssueGoodsDesc1.setVisibility(View.GONE);
                            tvIssueGoodsDesc2.setVisibility(View.GONE);
                            tvIssueGoodsDesc1.setText("");
                            tvIssueGoodsDesc2.setText("");
                        }

                        if (respData.isClaimGoods()) {
                            tvClaimGoods.setVisibility(View.VISIBLE);
                            tvClaimGoodsDesc1.setVisibility(View.VISIBLE);
                            tvClaimGoodsDesc2.setVisibility(View.VISIBLE);
                            tvClaimGoodsDesc1.setText(respData.getClaimGoodsDesc1());
                            tvClaimGoodsDesc2.setText(respData.getClaimGoodsDesc2());
                        } else {
                            tvClaimGoods.setVisibility(View.GONE);
                            tvClaimGoodsDesc1.setVisibility(View.GONE);
                            tvClaimGoodsDesc2.setVisibility(View.GONE);
                            tvClaimGoodsDesc1.setText("");
                            tvClaimGoodsDesc2.setText("");
                        }

                        if (respData.isOrderExpress()) {
                            tvOrderExpress.setVisibility(View.VISIBLE);
                            tvOrderExpressDesc.setVisibility(View.VISIBLE);
                            tvOrderExpressDesc.setText(respData.getOrderExpressDesc());
                        } else {
                            tvOrderExpress.setVisibility(View.GONE);
                            tvOrderExpressDesc.setVisibility(View.GONE);
                            tvOrderExpressDesc.setText("");
                        }
                    }

                    @Override
                    public void onFailure(Call<GoodsExpressQueryResponseInfo> call, Throwable t) {
                        Toast.makeText(ExpressPackageQueryActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            // 获得授权
            startQrCode(requestCode);
        } else {
            // 被禁止授权
            Toast.makeText(this, "请至权限中心打开本应用的相机访问权限", Toast.LENGTH_LONG).show();
        }
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
