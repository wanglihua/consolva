package com.huadu.shunda.core;

import android.content.Context;
import android.media.AudioAttributes;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Build;

import com.huadu.shunda.R;

public class SoundUtil {

    private static SoundPool successSoundPool = null;
    private static SoundPool errorSoundPool = null;

    public static void init(Context context) {
        initLoadSuccess(context);
        initLoadError(context);
    }

    private static void initLoadSuccess(Context context) {
        if (successSoundPool == null) {

            if (Build.VERSION.SDK_INT >= 21) {
                SoundPool.Builder builder = new SoundPool.Builder();
                builder.setMaxStreams(10);                                                          //传入最多播放音频数量,

                AudioAttributes.Builder attrBuilder = new AudioAttributes.Builder();
                attrBuilder.setLegacyStreamType(AudioManager.STREAM_SYSTEM);                        //设置音频流的合适的属性
                builder.setAudioAttributes(attrBuilder.build());                                   //加载一个AudioAttributes

                successSoundPool = builder.build();
            } else {
                successSoundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
            }

            successSoundPool.load(context, R.raw.success, 1);
        }
    }

    private static void initLoadError(Context context) {
        if (errorSoundPool == null) {
            if (Build.VERSION.SDK_INT >= 21) {
                SoundPool.Builder builder = new SoundPool.Builder();
                builder.setMaxStreams(10);                                                          //传入最多播放音频数量,

                AudioAttributes.Builder attrBuilder = new AudioAttributes.Builder();
                attrBuilder.setLegacyStreamType(AudioManager.STREAM_SYSTEM);                        //设置音频流的合适的属性
                builder.setAudioAttributes(attrBuilder.build());                                   //加载一个AudioAttributes

                errorSoundPool = builder.build();
            } else {
                errorSoundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
            }

            errorSoundPool.load(context, R.raw.error, 1);
        }
    }

    public static void playSuccess(Context context) {
        initLoadSuccess(context);
        successSoundPool.play(1, 1, 1, 0, 0, 1);
    }

    public static void playError(Context context) {
        initLoadError(context);
        errorSoundPool.play(1, 1, 1, 0, 0, 1);
    }
}
