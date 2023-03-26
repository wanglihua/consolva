package com.huadu.iepoch.ui.pickup.homedeliverbatch;

import android.Manifest;
import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.net.Uri;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.ClipboardUtil;
import com.huadu.iepoch.core.MaskDialog;
import com.huadu.iepoch.core.RetrofitUtil;
import com.huadu.iepoch.core.SoundUtil;
import com.huadu.iepoch.data.http.ResponseInfo;
import com.huadu.iepoch.http.HomeDeliverBatchRequest;

import java.util.List;
import java.util.Map;

import permissions.dispatcher.NeedsPermission;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomeDeliverBatchEntryListAdapter extends SimpleAdapter {
    private Context context = null;
    private List<? extends Map<String, ?>> data = null;

    /**
     * Constructor
     *
     * @param context  The context where the View associated with this SimpleAdapter is running
     * @param data     A List of Maps. Each entry in the List corresponds to one row in the list. The
     *                 Maps contain the data for each row, and should include all the entries specified in
     *                 "from"
     * @param resource Resource identifier of a view layout that defines the views for this list
     *                 item. The layout file should include at least those named views defined in "to"
     * @param from     A list of column names that will be added to the Map associated with each
     *                 item.
     * @param to       The views that should display column in the "from" parameter. These should all be
     *                 TextViews. The first N views in this list are given the values of the first N columns
     */
    public HomeDeliverBatchEntryListAdapter(Context context, List<? extends Map<String, ?>> data, int resource, String[] from, int[] to) {
        super(context, data, resource, from, to);
        this.context = context;
        this.data = data;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        if (convertView == null) {
            convertView = LinearLayout.inflate(context, R.layout.item_home_deliver_batch_entry_list, null);
        }

        TextView tvOrderStatus = convertView.findViewById(R.id.tvOrderStatus);
        if (tvOrderStatus != null) {
            Object finishedObject = data.get(position).get("orderStatusFinished");
            if (finishedObject != null) {
                boolean finished = (boolean) finishedObject;
                if (finished) {
                    tvOrderStatus.setTextColor(0xfffe5000); // 橘红色
                } else {
                    tvOrderStatus.setTextColor(Color.GRAY);
                }
            }
        }

        TextView tvMessageStatus = convertView.findViewById(R.id.tvMessageStatus);
        if (tvMessageStatus != null) {
            Object sentObject = data.get(position).get("messageStatusSent");
            if (sentObject != null) {
                boolean sent = (boolean) sentObject;
                if (sent) {
                    tvMessageStatus.setTextColor(0xff00bece); // 蓝绿色
                } else {
                    tvMessageStatus.setTextColor(Color.GRAY);
                }
            }
        }

        View lytButton = convertView.findViewById(R.id.lytButton);
        if (lytButton != null) {
            Object showButtonObject = data.get(position).get("showButton");
            if (showButtonObject != null) {
                boolean showButton = (boolean) showButtonObject;
                if (showButton) {
                    lytButton.setVisibility(View.VISIBLE);

                    ImageButton btnUploadFail = convertView.findViewById(R.id.btnUploadFail);
                    if (btnUploadFail != null) {
                        btnUploadFail.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListAdapter.this.context);
                                alertDialogBuilder.setTitle("设为派送失败");
                                alertDialogBuilder.setMessage("至少上传一张照片！");
                                alertDialogBuilder.setPositiveButton("相册选择", new AlertDialog.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialogInterface, int i) {
                                        Object orderCodeObject = data.get(position).get("orderCode");
                                        if (orderCodeObject != null) {
                                            String orderCode = (String) orderCodeObject;
                                            ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)).showSelectPhoto(orderCode, HomeDeliverBatchEntryListActivity.REQ_SELECT_PHOTO_FAIL);
                                        }
                                    }
                                });

                                alertDialogBuilder.setNegativeButton("即时拍照", new AlertDialog.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialogInterface, int i) {
                                        Object orderCodeObject = data.get(position).get("orderCode");
                                        if (orderCodeObject != null) {
                                            String orderCode = (String) orderCodeObject;
                                            ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)).showCapturePhoto(orderCode, HomeDeliverBatchEntryListActivity.REQ_CAPTURE_PHOTO_FAIL);
                                        }
                                    }
                                });

                                AlertDialog alertDialog = alertDialogBuilder.create();
                                alertDialog.show();
                            }
                        });
                    }

                    ImageButton btnCopy = convertView.findViewById(R.id.btnCopy);
                    if (btnCopy != null) {
                        btnCopy.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                Object addressObject = data.get(position).get("address");
                                if (addressObject != null) {
                                    String address = (String) addressObject;

                                    Activity activity = ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context));
                                    ClipboardUtil.copyToClipboard(activity, address);

                                    Toast.makeText(activity, "地址成功复制到剪贴板！", Toast.LENGTH_LONG).show();
                                }
                            }
                        });
                    }

                    ImageButton btnNavigation = convertView.findViewById(R.id.btnNavigation);
                    if (btnNavigation != null) {
                        btnNavigation.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                String googleMapPkg = "com.google.android.apps.maps";
                                if (isGoogleMapsInstalled() == false) {
                                    Toast.makeText(context, "google map 未安装！", Toast.LENGTH_LONG).show();
                                    return;
                                }

                                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListAdapter.this.context);
                                alertDialogBuilder.setTitle("导航");
                                alertDialogBuilder.setMessage("确认要打开 google map 吗？");
                                alertDialogBuilder.setPositiveButton("打开", new AlertDialog.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialogInterface, int i) {
                                        Object addressObject = data.get(position).get("address");
                                        if (addressObject != null) {
                                            String address = (String) addressObject;

                                            // Uri  gmmIntentUri = Uri.parse("geo:0,0?q=1600 Amphitheatre Parkway, Mountain+View, California");
                                            Uri gmmIntentUri = Uri.parse("geo:0,0?q=" + address);
                                            Intent mapIntent = new Intent(Intent.ACTION_VIEW, gmmIntentUri);
                                            mapIntent.setPackage(googleMapPkg);
                                            ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)).startActivity(mapIntent);
                                        }
                                    }
                                });

                                alertDialogBuilder.setNegativeButton("取消", null);

                                AlertDialog alertDialog = alertDialogBuilder.create();
                                alertDialog.show();
                            }
                        });
                    }

                    TextView tvCompany = convertView.findViewById(R.id.tvCompany);
                    if (tvCompany != null) {
                        Object companyObject = data.get(position).get("company");
                        Object companyLabelObject = data.get(position).get("companyLabel");
                        if (companyObject == null) {
                            tvCompany.setVisibility(View.GONE);
                        } else {
                            String company = (String) companyObject;
                            if ("".equals(company)) {
                                tvCompany.setVisibility(View.GONE);
                            } else {
                                tvCompany.setVisibility(View.VISIBLE);
                                tvCompany.setText((String) companyLabelObject);
                            }
                        }
                    }

                    ImageButton btnSend = convertView.findViewById(R.id.btnSend);
                    if (btnSend != null) {
                        btnSend.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                Object entryIdObject = data.get(position).get("id");
                                if (entryIdObject != null) {
                                    int entryId = (Integer) entryIdObject;

                                    AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListAdapter.this.context);
                                    alertDialogBuilder.setTitle("确认");
                                    alertDialogBuilder.setMessage("确认30分钟内将送达本订单包裹吗？");
                                    alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                                        @Override
                                        public void onClick(DialogInterface dialogInterface, int i) {

                                            HomeDeliverBatchRequest homeDeliverBatchRequest = RetrofitUtil.createRequest(HomeDeliverBatchRequest.class);
                                            Call<ResponseInfo> sendFourthMessageCall = homeDeliverBatchRequest.sendFourthMessage(entryId);
                                            Dialog mask = MaskDialog.show(HomeDeliverBatchEntryListAdapter.this.context);
                                            sendFourthMessageCall.enqueue(new Callback<ResponseInfo>() {
                                                @Override
                                                public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                                                    mask.dismiss();

                                                    ResponseInfo respData = response.body();

                                                    if (!RetrofitUtil.checkResponse(respData, HomeDeliverBatchEntryListAdapter.this.context)) {
                                                        return;
                                                    }

                                                    SoundUtil.playSuccess(HomeDeliverBatchEntryListAdapter.this.context);

                                                    Toast.makeText(HomeDeliverBatchEntryListAdapter.this.context, respData.getMessage(), Toast.LENGTH_LONG).show();

                                                    ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)).loadEntryList();
                                                }

                                                @Override
                                                public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                                    Toast.makeText(HomeDeliverBatchEntryListAdapter.this.context, t.getMessage(), Toast.LENGTH_SHORT).show();
                                                }
                                            });

                                        }
                                    });

                                    alertDialogBuilder.setNegativeButton("取消", null);

                                    AlertDialog alertDialog = alertDialogBuilder.create();
                                    alertDialog.show();
                                }
                            }
                        });
                    }

                    ImageButton btnPhone = convertView.findViewById(R.id.btnPhone);
                    if (btnPhone != null) {
                        btnPhone.setOnClickListener(new View.OnClickListener() {
                            @Override
                            @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.CALL_PHONE})
                            public void onClick(View v) {
                                if (ContextCompat.checkSelfPermission(HomeDeliverBatchEntryListAdapter.this.context, Manifest.permission.CALL_PHONE) != PackageManager.PERMISSION_GRANTED) {
                                    ActivityCompat.requestPermissions(((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)), new String[]{Manifest.permission.CALL_PHONE}, 0);
                                    return;
                                }

                                String phone = "";
                                Object phoneObject = data.get(position).get("customerPhone");
                                if (phoneObject != null) {
                                    phone = phoneObject.toString();
                                }

                                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListAdapter.this.context);
                                alertDialogBuilder.setTitle("确认");
                                alertDialogBuilder.setMessage("确认要给 " + phone + " 打电话吗？");
                                String finalPhone = phone;
                                alertDialogBuilder.setPositiveButton("确定", new AlertDialog.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialogInterface, int i) {
                                        Intent intent = new Intent();
                                        intent.setAction(Intent.ACTION_CALL);
                                        intent.setData(Uri.parse("tel:" + finalPhone));
                                        HomeDeliverBatchEntryListAdapter.this.context.startActivity(intent);
                                    }
                                });

                                alertDialogBuilder.setNegativeButton("取消", null);

                                AlertDialog alertDialog = alertDialogBuilder.create();
                                alertDialog.show();


                            }
                        });
                    }

                    ImageButton btnUpload = convertView.findViewById(R.id.btnUpload);
                    if (btnUpload != null) {
                        btnUpload.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(HomeDeliverBatchEntryListAdapter.this.context);
                                alertDialogBuilder.setTitle("上传方式");
                                alertDialogBuilder.setMessage("【即时拍照】还是【相册选择】上传！");
                                alertDialogBuilder.setPositiveButton("相册选择", new AlertDialog.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialogInterface, int i) {
                                        Object orderCodeObject = data.get(position).get("orderCode");
                                        if (orderCodeObject != null) {
                                            String orderCode = (String) orderCodeObject;
                                            ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)).showSelectPhoto(orderCode, HomeDeliverBatchEntryListActivity.REQ_SELECT_PHOTO);
                                        }
                                    }
                                });

                                alertDialogBuilder.setNegativeButton("即时拍照", new AlertDialog.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialogInterface, int i) {
                                        Object orderCodeObject = data.get(position).get("orderCode");
                                        if (orderCodeObject != null) {
                                            String orderCode = (String) orderCodeObject;
                                            ((HomeDeliverBatchEntryListActivity) (HomeDeliverBatchEntryListAdapter.this.context)).showCapturePhoto(orderCode, HomeDeliverBatchEntryListActivity.REQ_CAPTURE_PHOTO);
                                        }
                                    }
                                });

                                AlertDialog alertDialog = alertDialogBuilder.create();
                                alertDialog.show();
                            }
                        });
                    }
                } else {
                    lytButton.setVisibility(View.GONE);
                }
            }
        }

        return super.getView(position, convertView, parent);
    }

    private boolean isGoogleMapsInstalled() {
        try {
            ApplicationInfo info = context.getPackageManager().getApplicationInfo("com.google.android.apps.maps", 0);
            return true;
        } catch (PackageManager.NameNotFoundException e) {
            return false;
        }
    }
}
