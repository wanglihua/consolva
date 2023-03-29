package com.huadu.shunda.ui.zxing.activity;

import android.app.ProgressDialog;
import android.content.Intent;
import android.content.res.AssetFileDescriptor;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.os.Vibrator;
import android.provider.MediaStore;
import android.text.TextUtils;
import android.view.Menu;
import android.view.MenuItem;
import android.view.SurfaceHolder;
import android.view.SurfaceHolder.Callback;
import android.view.SurfaceView;
import android.widget.ImageButton;
import android.widget.Toast;

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
import com.google.zxing.BarcodeFormat;
import com.google.zxing.BinaryBitmap;
import com.google.zxing.ChecksumException;
import com.google.zxing.DecodeHintType;
import com.google.zxing.FormatException;
import com.google.zxing.NotFoundException;
import com.google.zxing.Result;
import com.google.zxing.common.HybridBinarizer;
import com.google.zxing.qrcode.QRCodeReader;
import com.huadu.shunda.R;
import com.huadu.shunda.core.Config;
import com.huadu.shunda.core.PhotoUtil;
import com.huadu.shunda.core.RetrofitUtil;
import com.huadu.shunda.data.domain.OrderPhotoType;
import com.huadu.shunda.data.http.ResponseInfo;
import com.huadu.shunda.http.ExpressPhotoRequest;
import com.huadu.shunda.ui.BaseActivity;
import com.huadu.shunda.ui.experssimageshow.ExpressImageViewActivity;
import com.huadu.shunda.ui.zxing.QrScanConstants;
import com.huadu.shunda.ui.zxing.camera.CameraManager;
import com.huadu.shunda.ui.zxing.decoding.CaptureActivityHandler;
import com.huadu.shunda.ui.zxing.decoding.InactivityTimer;
import com.huadu.shunda.ui.zxing.decoding.RGBLuminanceSource;
import com.huadu.shunda.ui.zxing.view.ViewfinderView;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Vector;

import retrofit2.Call;
import retrofit2.Response;

public class CaptureActivity extends BaseActivity implements Callback {

    private static List<String> imageList = new ArrayList<>();

    public static void setImageList(List<String> imageList) {
        CaptureActivity.imageList = imageList;
    }

    private static final int REQUEST_CODE_SCAN_GALLERY = 100;

    private boolean expressScanUpload = false;
    private boolean orderScanUpload = false;
    private int photoTypeId = OrderPhotoType.None.getId();
    private boolean hasImageList = false;

    private String labelText = "扫描二维码";

    private CaptureActivityHandler handler;
    private ViewfinderView viewfinderView;
    private SurfaceView scannerView;

    private ImageButton btnBack;
    private boolean hasSurface;
    private Vector<BarcodeFormat> decodeFormats;
    private String characterSet;
    private InactivityTimer inactivityTimer;
    private MediaPlayer mediaPlayer;
    private boolean playBeep;
    private static final float BEEP_VOLUME = 0.10f;
    private boolean vibrate;
    private ProgressDialog mProgress;
    private String photo_path;
    private Bitmap scanBitmap;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_scanner);
        CameraManager.init(getApplication());
        viewfinderView = findViewById(R.id.viewfinder_content);
        scannerView = (SurfaceView) findViewById(R.id.scanner_view);

        Intent intent = getIntent();

        expressScanUpload = intent.getBooleanExtra("expressScanUpload", false);
        orderScanUpload = intent.getBooleanExtra("orderScanUpload", false);
        photoTypeId = intent.getIntExtra("photoTypeId", OrderPhotoType.None.getId());
        hasImageList = intent.getBooleanExtra("hasImageList", false);

        labelText = intent.getStringExtra("labelText");
        if (labelText == null || "".equals(labelText)) {
            labelText = "扫描二维码";
        }

        viewfinderView.setLabelText(labelText);

        hasSurface = false;
        inactivityTimer = new InactivityTimer(this);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        if (hasImageList) {
            getMenuInflater().inflate(R.menu.menu_scanner, menu);
        }

        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        /*
        switch (item.getItemId()){
            case R.id.menu_item_scan_local:
                //打开手机中的相册
                Intent innerIntent = new Intent(Intent.ACTION_GET_CONTENT); //"android.intent.action.GET_CONTENT"
                innerIntent.setType("image/*");
                Intent wrapperIntent = Intent.createChooser(innerIntent, "选择二维码图片");
                this.startActivityForResult(wrapperIntent, REQUEST_CODE_SCAN_GALLERY);
                return true;
        }
        */

        if (item.getItemId() == R.id.menu_item_express_images) {
            if (hasImageList && CaptureActivity.imageList != null && CaptureActivity.imageList.size() > 0) {
                Intent intentImageView = new Intent(CaptureActivity.this, ExpressImageViewActivity.class);
                // intentImageView.putExtra("bigImageUrl", imageItem.getBigImageUrl());
                ExpressImageViewActivity.setImageList(CaptureActivity.imageList);
                intentImageView.putExtra("imageIndex", 0);
                startActivity(intentImageView);
            }
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onActivityResult(final int requestCode, int resultCode, Intent data) {
        if (resultCode == RESULT_OK) {
            switch (requestCode) {
                case REQUEST_CODE_SCAN_GALLERY:
                    //获取选中图片的路径
                    Cursor cursor = getContentResolver().query(data.getData(), null, null, null, null);
                    if (cursor.moveToFirst()) {
                        photo_path = cursor.getString(cursor.getColumnIndex(MediaStore.Images.Media.DATA));
                    }
                    cursor.close();

                    mProgress = new ProgressDialog(CaptureActivity.this);
                    mProgress.setMessage("正在扫描...");
                    mProgress.setCancelable(false);
                    mProgress.show();

                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            Result result = scanningImage(photo_path);
                            if (result != null) {
//                                Message m = handler.obtainMessage();
//                                m.what = R.id.decode_succeeded;
//                                m.obj = result.getText();
//                                handler.sendMessage(m);
                                Intent resultIntent = new Intent();
                                Bundle bundle = new Bundle();
                                bundle.putString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN, result.getText());
//                                Logger.d("saomiao",result.getText());
//                                bundle.putParcelable("bitmap",result.get);
                                resultIntent.putExtras(bundle);
                                CaptureActivity.this.setResult(RESULT_OK, resultIntent);

                            } else {
                                Message m = handler.obtainMessage();
                                m.what = R.id.decode_failed;
                                m.obj = "Scan failed!";
                                handler.sendMessage(m);
                            }
                        }
                    }).start();
                    break;
            }
        }
        super.onActivityResult(requestCode, resultCode, data);
    }

    /**
     * 扫描二维码图片的方法
     *
     * @param path
     * @return
     */
    public Result scanningImage(String path) {
        if (TextUtils.isEmpty(path)) {
            return null;
        }
        Hashtable<DecodeHintType, String> hints = new Hashtable<>();
        hints.put(DecodeHintType.CHARACTER_SET, "UTF8"); //设置二维码内容的编码

        BitmapFactory.Options options = new BitmapFactory.Options();
        options.inJustDecodeBounds = true; // 先获取原大小
        scanBitmap = BitmapFactory.decodeFile(path, options);
        options.inJustDecodeBounds = false; // 获取新的大小
        int sampleSize = (int) (options.outHeight / (float) 200);
        if (sampleSize <= 0)
            sampleSize = 1;
        options.inSampleSize = sampleSize;
        scanBitmap = BitmapFactory.decodeFile(path, options);
        RGBLuminanceSource source = new RGBLuminanceSource(scanBitmap);
        BinaryBitmap bitmap1 = new BinaryBitmap(new HybridBinarizer(source));
        QRCodeReader reader = new QRCodeReader();
        try {
            return reader.decode(bitmap1, hints);
        } catch (NotFoundException e) {
            // e.printStackTrace();
        } catch (ChecksumException e) {
            // e.printStackTrace();
        } catch (FormatException e) {
            // e.printStackTrace();
        }
        return null;
    }

    @Override
    protected void onResume() {
        super.onResume();
        SurfaceHolder surfaceHolder = scannerView.getHolder();
        if (hasSurface) {
            initCamera(surfaceHolder);
        } else {
            surfaceHolder.addCallback(this);
            surfaceHolder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);
        }
        decodeFormats = null;
        characterSet = null;

        playBeep = true;
        AudioManager audioService = (AudioManager) getSystemService(AUDIO_SERVICE);
        if (audioService.getRingerMode() != AudioManager.RINGER_MODE_NORMAL) {
            playBeep = false;
        }
        initBeepSound();
        vibrate = true;
    }

    @Override
    protected void onPause() {
        super.onPause();
        if (handler != null) {
            handler.quitSynchronously();
            handler = null;
        }
        CameraManager.get().closeDriver();
    }

    @Override
    protected void onDestroy() {
        inactivityTimer.shutdown();
        super.onDestroy();
    }

    /**
     * Handler scan result
     *
     * @param result
     * @param barcode
     */
    public void handleDecode(Result result, Bitmap barcode) {
        inactivityTimer.onActivity();
        playBeepSoundAndVibrate();
        String resultString = result.getText();
        //FIXME
        if (TextUtils.isEmpty(resultString)) {
            Toast.makeText(CaptureActivity.this, "Scan failed!", Toast.LENGTH_SHORT).show();
        } else {
            Intent resultIntent = new Intent();
            Bundle bundle = new Bundle();
            bundle.putString(QrScanConstants.INTENT_EXTRA_KEY_QR_SCAN, resultString);
            resultIntent.putExtras(bundle);
            this.setResult(RESULT_OK, resultIntent);

            // wanglihua begin

            if (expressScanUpload || orderScanUpload) {
                String base64 = PhotoUtil.bitmapToBase64(barcode, false);

                if (expressScanUpload) {

                    /*

                    // begin 上传至ECS服务器

                    LogisticsOrderPhotoRequest photoRequest = RetrofitUtil.createRequest(LogisticsOrderPhotoRequest.class);
                    String expressTrackingNo = resultString;
                    Call<ResponseInfo> uploadBase64Call = photoRequest.uploadBase64("", expressTrackingNo, photoTypeId, base64);
                    uploadBase64Call.enqueue(new retrofit2.Callback<ResponseInfo>() {
                        @Override
                        public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                            ResponseInfo respData = response.body();

                            if (respData == null) {
                                Toast.makeText(CaptureActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                return;
                            }

                            if (!respData.getSuccess()) {
                                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(CaptureActivity.this);
                                alertDialogBuilder.setTitle("错误");
                                alertDialogBuilder.setMessage(respData.getMessage());
                                alertDialogBuilder.setPositiveButton("确定", null);

                                AlertDialog alertDialog = alertDialogBuilder.create();
                                alertDialog.show();
                            } else {
                                Toast.makeText(CaptureActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        }

                        @Override
                        public void onFailure(Call<ResponseInfo> call, Throwable t) {
                            Toast.makeText(CaptureActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                    });

                    // end 上传至ECS服务器

                     */

                    // begin 上传至阿里云OSS

                    byte[] imageBytes = PhotoUtil.bitmapToBytes(barcode, false);

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

                            String expressTrackingNo = resultString;
                            Call<ResponseInfo> uploadBase64Call = photoRequest.create("", expressTrackingNo, photoTypeId, photoKey);
                            uploadBase64Call.enqueue(new retrofit2.Callback<ResponseInfo>() {
                                @Override
                                public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {

                                    ResponseInfo respData = response.body();

                                    if (respData == null) {
                                        Toast.makeText(CaptureActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                        return;
                                    }

                                    if (!respData.getSuccess()) {
                                        Toast.makeText(CaptureActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();

                                        /*
                                        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(CaptureActivity.this);
                                        alertDialogBuilder.setTitle("错误");
                                        alertDialogBuilder.setMessage(respData.getMessage());
                                        alertDialogBuilder.setPositiveButton("确定", null);

                                        AlertDialog alertDialog = alertDialogBuilder.create();
                                        alertDialog.show();
                                         */
                                    } else {
                                        Toast.makeText(CaptureActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                                    }
                                }

                                @Override
                                public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                    Toast.makeText(CaptureActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                                }
                            });
                        }

                        @Override
                        public void onFailure(PutObjectRequest request, ClientException clientExcepion, ServiceException serviceException) {
                            // 请求异常。
                            if (clientExcepion != null) {
                                // 本地异常，如网络异常等。
                                // clientExcepion.printStackTrace();

                                Toast.makeText(CaptureActivity.this, "oss错误: " + clientExcepion.getMessage(), Toast.LENGTH_LONG).show();
                                return;

                            }
                            if (serviceException != null) {
                                // 服务异常。
                                Toast.makeText(CaptureActivity.this, "oss错误: " + serviceException.getMessage(), Toast.LENGTH_LONG).show();
                                return;
                            }

                            Toast.makeText(CaptureActivity.this, "oss上传错误", Toast.LENGTH_LONG).show();
                        }
                    });
                    // task.cancel(); // 可以取消任务。
                    // task.waitUntilFinished(); // 等待上传完成。

                    // end 上传至阿里云OSS

                }

                if (orderScanUpload) {

                    /*

                    // begin 上传至ECS服务器

                    LogisticsOrderPhotoRequest photoRequest = RetrofitUtil.createRequest(LogisticsOrderPhotoRequest.class);

                    String orderCode = resultString;
                    Call<ResponseInfo> uploadBase64Call = photoRequest.uploadBase64(orderCode, "", photoTypeId, base64);
                    uploadBase64Call.enqueue(new retrofit2.Callback<ResponseInfo>() {
                        @Override
                        public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {
                            ResponseInfo respData = response.body();

                            if (respData == null) {
                                Toast.makeText(CaptureActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                return;
                            }

                            if (!respData.getSuccess()) {
                                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(CaptureActivity.this);
                                alertDialogBuilder.setTitle("错误");
                                alertDialogBuilder.setMessage(respData.getMessage());
                                alertDialogBuilder.setPositiveButton("确定", null);

                                AlertDialog alertDialog = alertDialogBuilder.create();
                                alertDialog.show();
                            } else {
                                Toast.makeText(CaptureActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        }

                        @Override
                        public void onFailure(Call<ResponseInfo> call, Throwable t) {
                            Toast.makeText(CaptureActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                    });

                    // end 上传至ECS服务器

                     */

                    // begin 上传至阿里云OSS

                    byte[] imageBytes = PhotoUtil.bitmapToBytes(barcode, false);

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

                            Call<ResponseInfo> uploadBase64Call = photoRequest.create(resultString, "", photoTypeId, photoKey);
                            uploadBase64Call.enqueue(new retrofit2.Callback<ResponseInfo>() {
                                @Override
                                public void onResponse(Call<ResponseInfo> call, Response<ResponseInfo> response) {

                                    ResponseInfo respData = response.body();

                                    if (respData == null) {
                                        Toast.makeText(CaptureActivity.this, "未能接收到服务器返回数据！", Toast.LENGTH_SHORT).show();
                                        return;
                                    }

                                    if (!respData.getSuccess()) {
                                        /*
                                        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(CaptureActivity.this);
                                        alertDialogBuilder.setTitle("错误");
                                        alertDialogBuilder.setMessage(respData.getMessage());
                                        alertDialogBuilder.setPositiveButton("确定", null);

                                        AlertDialog alertDialog = alertDialogBuilder.create();
                                        alertDialog.show();
                                         */

                                        Toast.makeText(CaptureActivity.this, respData.getMessage(), Toast.LENGTH_LONG).show();
                                    } else {
                                        Toast.makeText(CaptureActivity.this, respData.getMessage(), Toast.LENGTH_SHORT).show();
                                    }
                                }

                                @Override
                                public void onFailure(Call<ResponseInfo> call, Throwable t) {
                                    Toast.makeText(CaptureActivity.this, t.getMessage(), Toast.LENGTH_SHORT).show();
                                }
                            });
                        }

                        @Override
                        public void onFailure(PutObjectRequest request, ClientException clientExcepion, ServiceException serviceException) {
                            // 请求异常。
                            if (clientExcepion != null) {
                                // 本地异常，如网络异常等。
                                // clientExcepion.printStackTrace();

                                Toast.makeText(CaptureActivity.this, "oss错误: " + clientExcepion.getMessage(), Toast.LENGTH_LONG).show();
                                return;
                            }
                            if (serviceException != null) {
                                // 服务异常。
                                Toast.makeText(CaptureActivity.this, "oss错误: " + serviceException.getMessage(), Toast.LENGTH_LONG).show();
                                return;
                            }

                            Toast.makeText(CaptureActivity.this, "oss上传错误", Toast.LENGTH_LONG).show();
                        }
                    });
                    // task.cancel(); // 可以取消任务。
                    // task.waitUntilFinished(); // 等待上传完成。

                    // end 上传至阿里云OSS

                }
            }

            // wanglihua end
        }
        CaptureActivity.this.finish();
    }

    private void initCamera(SurfaceHolder surfaceHolder) {
        try {
            CameraManager.get().openDriver(surfaceHolder);
            CameraManager.get().setCameraDisplayOrientation(this);
        } catch (IOException ioe) {
            return;
        } catch (RuntimeException e) {
            return;
        }
        if (handler == null) {
            handler = new CaptureActivityHandler(this, decodeFormats,
                    characterSet);
        }
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {

    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        if (!hasSurface) {
            hasSurface = true;
            initCamera(holder);
        }
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        hasSurface = false;
    }

    public ViewfinderView getViewfinderView() {
        return viewfinderView;
    }

    public Handler getHandler() {
        return handler;
    }

    public void drawViewfinder() {
        viewfinderView.drawViewfinder();
    }

    private void initBeepSound() {
        if (playBeep && mediaPlayer == null) {
            // The volume on STREAM_SYSTEM is not adjustable, and users found it
            // too loud,
            // so we now play on the music stream.
            setVolumeControlStream(AudioManager.STREAM_MUSIC);
            mediaPlayer = new MediaPlayer();
            mediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);
            mediaPlayer.setOnCompletionListener(beepListener);

            AssetFileDescriptor file = getResources().openRawResourceFd(
                    R.raw.beep);
            try {
                mediaPlayer.setDataSource(file.getFileDescriptor(),
                        file.getStartOffset(), file.getLength());
                file.close();
                mediaPlayer.setVolume(BEEP_VOLUME, BEEP_VOLUME);
                mediaPlayer.prepare();
            } catch (IOException e) {
                mediaPlayer = null;
            }
        }
    }

    private static final long VIBRATE_DURATION = 200L;

    private void playBeepSoundAndVibrate() {
        if (playBeep && mediaPlayer != null) {
            mediaPlayer.start();
        }
        if (vibrate) {
            Vibrator vibrator = (Vibrator) getSystemService(VIBRATOR_SERVICE);
            vibrator.vibrate(VIBRATE_DURATION);
        }
    }

    /**
     * When the beep has finished playing, rewind to queue up another one.
     */
    private final OnCompletionListener beepListener = new OnCompletionListener() {
        public void onCompletion(MediaPlayer mediaPlayer) {
            mediaPlayer.seekTo(0);
        }
    };

}