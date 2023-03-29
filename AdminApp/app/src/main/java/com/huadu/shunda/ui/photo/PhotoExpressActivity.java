package com.huadu.shunda.ui.photo;

import android.Manifest;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageManager;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Switch;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.alibaba.sdk.android.oss.ClientException;
import com.alibaba.sdk.android.oss.OSS;
import com.alibaba.sdk.android.oss.OSSClient;
import com.alibaba.sdk.android.oss.ServiceException;
import com.alibaba.sdk.android.oss.callback.OSSCompletedCallback;
import com.alibaba.sdk.android.oss.callback.OSSProgressCallback;
import com.alibaba.sdk.android.oss.common.auth.OSSStsTokenCredentialProvider;
import com.alibaba.sdk.android.oss.internal.OSSAsyncTask;
import com.alibaba.sdk.android.oss.model.ObjectMetadata;
import com.alibaba.sdk.android.oss.model.PutObjectRequest;
import com.alibaba.sdk.android.oss.model.PutObjectResult;
import com.google.zxing.Result;
import com.huadu.shunda.R;
import com.huadu.shunda.core.Config;
import com.huadu.shunda.core.PdaUtil;
import com.huadu.shunda.core.PermissionUtil;
import com.huadu.shunda.core.PhotoUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.core.SoundUtil;
import com.huadu.shunda.data.domain.OrderPhotoType;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.http.ExpressPhotoRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.pda.ScanActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.activity.CaptureActivity;
import com.zhihu.matisse.Matisse;
import com.zhihu.matisse.MimeType;
import com.zhihu.matisse.engine.impl.GlideEngine;

import java.io.File;
import java.util.List;

import me.dm7.barcodescanner.zxing.ZXingScannerView;
import permissions.dispatcher.NeedsPermission;
import permissions.dispatcher.RuntimePermissions;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

@RuntimePermissions
public class PhotoExpressActivity extends BaseActivity {

    private static final int REQ_QR_CODE_EXPRESS_TRACKING_NO = 100;
    private static final int REQ_CAPTURE_PHOTO = 200;
    private static final int REQ_SELECT_PHOTO = 300;

    private OrderPhotoType photoType;

    private Uri imageCaptureUri;

    private EditText etExpressTrackingNo = null;

    private Button btnScanBarCode;
    private Button btnScanQrCode;
    private Button btnCapturePhoto;
    private Button btnSelectPhoto;
    private Switch swAutoStartCapture;
    private Switch swAutoUploadScan;

    private int autoStartCaptureCount = 0;
    private final int autoStartCaptureCountLimit = 2;

    private ZXingScannerView scannerView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_photo_express);


        photoType = OrderPhotoType.getById(getIntent().getIntExtra("photoTypeId", OrderPhotoType.None.getId()));
        setTitle(photoType.getName());

        etExpressTrackingNo = findViewById(R.id.etExpressTrackingNo);
        etExpressTrackingNo.setOnFocusChangeListener(new View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View v, boolean hasFocus) {
                if (!hasFocus) {
                    showPhotoList();
                }
            }
        });

        btnScanBarCode = findViewById(R.id.btnScanBarCode);
        btnScanBarCode.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // startBarCode();
            }
        });

        btnScanQrCode = findViewById(R.id.btnScanQrCode);
        btnScanQrCode.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startQrCode(REQ_QR_CODE_EXPRESS_TRACKING_NO);
            }
        });

        btnCapturePhoto = findViewById(R.id.btnCapturePhoto);
        btnCapturePhoto.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (swAutoStartCapture.isChecked()) {
                    autoStartCaptureCount = 0;
                }
                showCapturePhoto();
            }
        });

        btnSelectPhoto = findViewById(R.id.btnSelectPhoto);
        btnSelectPhoto.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                showSelectPhoto();
            }
        });

        scannerView = new ZXingScannerView(this);
        // setContentView(scannerView);
        scannerView.setResultHandler(new ZXingScannerView.ResultHandler() {
            @Override
            public void handleResult(Result result) {
                etExpressTrackingNo.setText(result.getText());
            }
        });

        swAutoStartCapture = findViewById(R.id.swAutoStartCapture);
        swAutoUploadScan = findViewById(R.id.swAutoUploadScan);
    }

    @Deprecated
    @NeedsPermission(value = {Manifest.permission.CAMERA})
    protected void startBarCode() {
        if (PermissionUtil.checkCameraStoragePermissions(PhotoExpressActivity.this, REQ_QR_CODE_EXPRESS_TRACKING_NO) == false) {
            return;
        }

        scannerView.startCamera();
    }

    protected void startQrCode(int requestCode) {
        if (PdaUtil.hasPdaScanner(this)) {
            Intent intent = new Intent(this, ScanActivity.class);
            intent.putExtra("labelText", "扫描快递单号");
            intent.putExtra("expressScanUpload", swAutoUploadScan.isChecked());
            intent.putExtra("photoTypeId", photoType.getId());
            startActivityForResult(intent, requestCode);
        } else {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA}, requestCode);
                return;
            }

            Intent intent = new Intent(this, CaptureActivity.class);
            intent.putExtra("labelText", "扫描快递单号");
            intent.putExtra("expressScanUpload", swAutoUploadScan.isChecked());
            intent.putExtra("photoTypeId", photoType.getId());
            startActivityForResult(intent, requestCode);
        }
    }

    @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE})
    protected void showCapturePhoto() {
        if (checkExpressTrackingNoBlank() == false) {
            return;
        }

        Intent intentTakePhoto = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);

        if (swAutoUploadScan.isChecked()) {
            intentTakePhoto.putExtra("android.intent.extra.quickCapture", true);//启用快捷拍照
        }

        imageCaptureUri = Uri.fromFile(new File(Environment.getExternalStorageDirectory(), "temp_order_photo_" + String.valueOf(System.currentTimeMillis()) + ".jpg"));
        intentTakePhoto.putExtra(android.provider.MediaStore.EXTRA_OUTPUT, imageCaptureUri);
        intentTakePhoto.putExtra("return-data", true);

        startActivityForResult(intentTakePhoto, REQ_CAPTURE_PHOTO);
    }

    @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE})
    protected void onCapturePhoto(Intent data) {
        if (checkExpressTrackingNoBlank() == false) {
            return;
        }

        // begin 上传至ECS服务器

        /*

        String base64ImageData = PhotoUtil.compressImageFileToBase64(imageCaptureUri.getPath());

        LogisticsOrderPhotoRequest photoRequest = RetrofitUtil.createRequest(LogisticsOrderPhotoRequest.class);

        String expressTrackingNo = etExpressTrackingNo.getText().toString();
        Call<ResponseInfo> uploadBase64Call = photoRequest.uploadBase64("", expressTrackingNo, photoType.getId(), base64ImageData, LoginCache.getUserId());
        uploadBase64Call.enqueue(new Callback<ResponseInfo>() {
            @Override
            public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                if (imageCaptureUri != null && imageCaptureUri.getPath() != null) {
                    File tempOrderPhoto = new File(imageCaptureUri.getPath());
                    if (tempOrderPhoto.exists()) {
                        tempOrderPhoto.delete();
                    }
                }

                ResponseInfo respData = response.body();

                if (respData == null) {
                    Toast.makeText(PhotoExpressActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                    return;
                }

                if (!respData.getSuccess()) {
                    // Toast.makeText(PhotoExpressActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                    AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(PhotoExpressActivity.this);
                    alertDialogBuilder.setTitle("错误");
                    alertDialogBuilder.setMessage(respData.getMessage());
                    alertDialogBuilder.setPositiveButton("确定", null);

                    AlertDialog alertDialog = alertDialogBuilder.create();
                    alertDialog.show();
                } else {
                    Toast.makeText(PhotoExpressActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                    if (swAutoStartCapture.isChecked()) {
                        if (autoStartCaptureCount > 0) {
                            if (autoStartCaptureCount < autoStartCaptureCountLimit) {
                                autoStartCaptureCount++;
                                try {
                                    Thread.sleep(500);
                                } catch (Exception exp) {
                                    exp.printStackTrace();
                                }
                                showCapturePhoto();
                            }
                        }
                    }
                }
            }

            @Override
            public void onFailure(Call<ResponseInfo> call, Throwable t) {
                Toast.makeText(PhotoExpressActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });

        */

        // end 上传至ECS服务器

        // begin 上传至阿里云OSS

        byte[] imageBytes = PhotoUtil.compressImageFileToBytes(imageCaptureUri.getPath(), PhotoExpressActivity.this);

        // 构造上传请求。
        String photoKey = PhotoUtil.newPhotoStoreName(); // 获取新的文件名，一般都是以 .jpg 结尾

        ObjectMetadata objectMetadata = new ObjectMetadata();
        objectMetadata.setContentType("image/jpg");

        PutObjectRequest put = new PutObjectRequest(Config.getOssBucketName(), photoKey, imageBytes, objectMetadata);

        // 异步上传时可以设置进度回调。
        put.setProgressCallback(new OSSProgressCallback<PutObjectRequest>() {
            @Override
            public void onProgress(PutObjectRequest request, long currentSize, long totalSize) {
            }
        });

        OSS oss = new OSSClient(getApplicationContext(), Config.getOssEndPoint(), new OSSStsTokenCredentialProvider(Config.getOssAccessKeyId(), Config.getOssAccessKeySecret(), ""));

        OSSAsyncTask<PutObjectResult> task = oss.asyncPutObject(put, new OSSCompletedCallback<PutObjectRequest, PutObjectResult>() {
            @Override
            public void onSuccess(PutObjectRequest request, PutObjectResult result) {
                ExpressPhotoRequest photoRequest = RetrofitUtil.createRequest(ExpressPhotoRequest.class);

                String expressTrackingNo = etExpressTrackingNo.getText().toString();
                Call<ResponseInfo> uploadBase64Call = photoRequest.create("", expressTrackingNo, photoType.getId(), photoKey);
                // Dialog mask = MaskDialog.show(PhotoExpressActivity.this);
                uploadBase64Call.enqueue(new Callback<ResponseInfo>() {
                    @Override
                    public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                        // mask.dismiss();

                        if (imageCaptureUri != null && imageCaptureUri.getPath() != null) {
                            File tempOrderPhoto = new File(imageCaptureUri.getPath());
                            if (tempOrderPhoto.exists()) {
                                tempOrderPhoto.delete();
                            }
                        }

                        ResponseInfo respData = response.body();

                        if (!RetrofitUtil.processResponse(response, PhotoExpressActivity.this)) {
                            return;
                        }

                        SoundUtil.playSuccess(PhotoExpressActivity.this);
                        Toast.makeText(PhotoExpressActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                        if (swAutoStartCapture.isChecked()) {
                            if (autoStartCaptureCount > 0) {
                                if (autoStartCaptureCount < autoStartCaptureCountLimit) {
                                    autoStartCaptureCount++;
                                    try {
                                        Thread.sleep(500);
                                    } catch (Exception exp) {
                                        exp.printStackTrace();
                                    }
                                    showCapturePhoto();
                                }
                            }
                        }

                    }

                    @Override
                    public void onFailure(Call<ResponseInfo> call, Throwable t) {
                        Toast.makeText(PhotoExpressActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
            }

            @Override
            public void onFailure(PutObjectRequest request, ClientException clientExcepion, ServiceException serviceException) {
                // 请求异常。
                if (clientExcepion != null) {
                    // 本地异常，如网络异常等。
                    // clientExcepion.printStackTrace();

                    Toast.makeText(PhotoExpressActivity.this, "oss错误: " + clientExcepion.getMessage(), Toast.LENGTH_LONG).show();
                    return;

                }
                if (serviceException != null) {
                    // 服务异常。
                    Toast.makeText(PhotoExpressActivity.this, "oss错误: " + serviceException.getMessage(), Toast.LENGTH_LONG).show();
                    return;
                }

                Toast.makeText(PhotoExpressActivity.this, "oss上传错误", Toast.LENGTH_LONG).show();
            }
        });
        // task.cancel(); // 可以取消任务。
        // task.waitUntilFinished(); // 等待上传完成。

        // end 上传至阿里云OSS
    }

    @NeedsPermission(value = {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE})
    protected void showSelectPhoto() {
        if (checkExpressTrackingNoBlank() == false) {
            return;
        }

        Matisse.from(PhotoExpressActivity.this)
                //选择图片
                .choose(MimeType.ofImage())
                //是否只显示选择的类型的缩略图，就不会把所有图片视频都放在一起，而是需要什么展示什么
                .showSingleMediaType(true)
                //这两行要连用 是否在选择图片中展示照相 和适配安卓7.0 FileProvider
                //.capture(true)
                //.captureStrategy(new CaptureStrategy(false, "PhotoProvider"))
                //有序选择图片 123456...
                .countable(true)
                //最大选择数量为9
                .maxSelectable(9)
                //选择方向
                .restrictOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT)
                //界面中缩略图的质量
                .thumbnailScale(0.8f)
                //蓝色主题
                .theme(R.style.Matisse_Zhihu)
                //Glide加载方式
                .imageEngine(new GlideEngine())
                //请求码
                .forResult(REQ_SELECT_PHOTO);
    }

    @NeedsPermission(Manifest.permission.READ_EXTERNAL_STORAGE)
    protected void onSelectPhoto(Intent data) {
        if (checkExpressTrackingNoBlank() == false) {
            return;
        }

        List<Uri> selectedPhotoList = Matisse.obtainResult(data);

        for (Uri selectedPhoto : selectedPhotoList) {
            String[] proj = {MediaStore.Images.Media.DATA};
            Cursor actualimagecursor = this.managedQuery(selectedPhoto, proj, null, null, null);
            int actual_image_column_index = actualimagecursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
            actualimagecursor.moveToFirst();
            String img_path = actualimagecursor.getString(actual_image_column_index);

            /*

            // begin 上传至ECS服务器

            File imageFile = new File(img_path);
            RequestBody requestImageFile = RequestBody.create(MediaType.parse("multipart/form-data"), imageFile);
            MultipartBody.Part imageFilePart = MultipartBody.Part.createFormData("imageFile", imageFile.getName(), requestImageFile);

            LogisticsOrderPhotoRequest photoRequest = RetrofitUtil.createRequest(LogisticsOrderPhotoRequest.class);

            String expressTrackingNo = etExpressTrackingNo.getText().toString().trim();

            Call<ResponseInfo> uploadCall = photoRequest.upload("", expressTrackingNo, photoType.getId(), imageFilePart, LoginCache.getUserId());
            uploadCall.enqueue(new Callback<ResponseInfo>() {
                @Override
                public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                    ResponseInfo respData = response.body();

                    if (respData == null) {
                        Toast.makeText(PhotoExpressActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                        return;
                    }

                    if (!respData.getSuccess()) {
                        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(PhotoExpressActivity.this);
                        alertDialogBuilder.setTitle("错误");
                        alertDialogBuilder.setMessage(respData.getMessage());
                        alertDialogBuilder.setPositiveButton("确定", null);

                        AlertDialog alertDialog = alertDialogBuilder.create();
                        alertDialog.show();

                        return;
                    } else {
                        Toast.makeText(PhotoExpressActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                        return;
                    }
                }

                @Override
                public void onFailure(Call<ResponseInfo> call, Throwable t) {
                    Toast.makeText(PhotoExpressActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });

            // end 上传至ECS服务器

             */

            // begin 上传至阿里云OSS

            byte[] imageBytes = PhotoUtil.compressImageFileToBytes(img_path, PhotoExpressActivity.this);

            // 构造上传请求。
            String photoKey = PhotoUtil.newPhotoStoreName(); // 获取新的文件名，一般都是以 .jpg 结尾

            ObjectMetadata objectMetadata = new ObjectMetadata();
            objectMetadata.setContentType("image/jpg");

            PutObjectRequest put = new PutObjectRequest(Config.getOssBucketName(), photoKey, imageBytes, objectMetadata);

            // 异步上传时可以设置进度回调。
            put.setProgressCallback(new OSSProgressCallback<PutObjectRequest>() {
                @Override
                public void onProgress(PutObjectRequest request, long currentSize, long totalSize) {
                }
            });

            OSS oss = new OSSClient(getApplicationContext(), Config.getOssEndPoint(), new OSSStsTokenCredentialProvider(Config.getOssAccessKeyId(), Config.getOssAccessKeySecret(), ""));

            OSSAsyncTask<PutObjectResult> task = oss.asyncPutObject(put, new OSSCompletedCallback<PutObjectRequest, PutObjectResult>() {
                @Override
                public void onSuccess(PutObjectRequest request, PutObjectResult result) {
                    ExpressPhotoRequest photoRequest = RetrofitUtil.createRequest(ExpressPhotoRequest.class);

                    String expressTrackingNo = etExpressTrackingNo.getText().toString();
                    Call<ResponseInfo> uploadBase64Call = photoRequest.create("", expressTrackingNo, photoType.getId(), photoKey);
                    // Dialog mask = MaskDialog.show(PhotoExpressActivity.this);
                    uploadBase64Call.enqueue(new Callback<ResponseInfo>() {
                        @Override
                        public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                            // mask.dismiss();

                            if (imageCaptureUri != null && imageCaptureUri.getPath() != null) {
                                File tempOrderPhoto = new File(imageCaptureUri.getPath());
                                if (tempOrderPhoto.exists()) {
                                    tempOrderPhoto.delete();
                                }
                            }

                            ResponseInfo respData = response.body();

                            if (!RetrofitUtil.processResponse(response, PhotoExpressActivity.this)) {
                                return;
                            }

                            SoundUtil.playSuccess(PhotoExpressActivity.this);
                            Toast.makeText(PhotoExpressActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();

                            if (swAutoStartCapture.isChecked()) {
                                if (autoStartCaptureCount > 0) {
                                    if (autoStartCaptureCount < autoStartCaptureCountLimit) {
                                        autoStartCaptureCount++;
                                        try {
                                            Thread.sleep(500);
                                        } catch (Exception exp) {
                                            exp.printStackTrace();
                                        }
                                        showCapturePhoto();
                                    }
                                }
                            }

                        }

                        @Override
                        public void onFailure(Call<ResponseInfo> call, Throwable t) {
                            Toast.makeText(PhotoExpressActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                    });
                }

                @Override
                public void onFailure(PutObjectRequest request, ClientException clientExcepion, ServiceException serviceException) {
                    // 请求异常。
                    if (clientExcepion != null) {
                        // 本地异常，如网络异常等。
                        // clientExcepion.printStackTrace();

                        Toast.makeText(PhotoExpressActivity.this, "oss错误: " + clientExcepion.getMessage(), Toast.LENGTH_LONG).show();
                        return;
                    }
                    if (serviceException != null) {
                        // 服务异常。
                        Toast.makeText(PhotoExpressActivity.this, "oss错误: " + serviceException.getMessage(), Toast.LENGTH_LONG).show();
                        return;
                    }

                    Toast.makeText(PhotoExpressActivity.this, "oss上传错误", Toast.LENGTH_LONG).show();
                }
            });
            // task.cancel(); // 可以取消任务。
            // task.waitUntilFinished(); // 等待上传完成。

            // end 上传至阿里云OSS
        }
    }

    private void showPhotoList() {
        if (checkExpressTrackingNoBlank() == false) {
            return;
        }

        // todo 显示图片列表
    }

    private boolean checkExpressTrackingNoBlank() {
        String expressTrackingNo = etExpressTrackingNo.getText().toString().trim();
        if ("".equals(expressTrackingNo)) {
            SoundUtil.playError(PhotoExpressActivity.this);

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(PhotoExpressActivity.this);
            alertDialogBuilder.setTitle("错误");
            alertDialogBuilder.setMessage("快递单号不能为空！");
            alertDialogBuilder.setPositiveButton("确定", null);

            AlertDialog alertDialog = alertDialogBuilder.create();
            alertDialog.show();

            return false;
        }

        return true;
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        try {
            if (resultCode == RESULT_OK) {
                if (requestCode == REQ_QR_CODE_EXPRESS_TRACKING_NO) {
                    Bundle bundle = data.getExtras();
                    if (bundle == null) {
                        Toast.makeText(PhotoExpressActivity.this, "未能获取扫描的二维码数据！", Toast.LENGTH_SHORT).show();
                        return;
                    }

                    final String expressTrackingNo = bundle.getString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN);
                    etExpressTrackingNo.setText(expressTrackingNo);

                    if (swAutoStartCapture.isChecked()) {
                        // 自动开始拍第一张照片
                        autoStartCaptureCount = 1;

                        try {
                            Thread.sleep(500);
                        } catch (Exception exp) {
                            exp.printStackTrace();
                        }

                        showCapturePhoto();
                    }

                    // 显示图片列表
                    showPhotoList();
                } else if (requestCode == REQ_CAPTURE_PHOTO) {
                    onCapturePhoto(data);
                } else if (requestCode == REQ_SELECT_PHOTO) {
                    onSelectPhoto(data);
                }
            }
        } catch (Exception exp) {
            exp.printStackTrace();
        }
    }

    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            finish();
        }
        return true;
    }
}
