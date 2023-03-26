package com.huadu.shunda.ui.experssimageshow;


import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.huadu.shunda.R;
import com.huadu.shunda.data.http.expressimageshow.ExpressImageShowImageItemInfo;

import java.util.ArrayList;
import java.util.List;


public class ExpressImageListAdapter extends BaseAdapter {
    private Context context;
    private List<ExpressImageShowImageItemInfo> imageItemList;

    public ExpressImageListAdapter(Context context, List<ExpressImageShowImageItemInfo> imageItemList) {
        this.context = context;

        if (imageItemList != null) {
            this.imageItemList = imageItemList;
        } else {
            this.imageItemList = new ArrayList<>();
        }
    }

    @Override
    public int getCount() {
        return imageItemList.size();
    }

    @Override
    public Object getItem(int i) {
        return imageItemList.get(i);
    }

    @Override
    public long getItemId(int i) {
        return i;
    }

    @Override
    public View getView(int i, View view, ViewGroup viewGroup) {
        LinearLayout linearLayout;
        if (view == null) {//判断view是否可以重载
            LayoutInflater inflater = LayoutInflater.from(context);
            linearLayout = (LinearLayout) inflater.inflate(R.layout.item_express_image_list, null);
            ImageView imageView = linearLayout.findViewById(R.id.imageView);
            Glide.with(context)
                    .load(imageItemList.get(i).getSmallImageUrl())
                    // .placeholder(R.drawable.bitmap)//加载未完成时显示占位图
                    .diskCacheStrategy(DiskCacheStrategy.NONE)
                    .into(imageView);//显示的位置
        } else {//可以重载则直接使用原来的view
            linearLayout = (LinearLayout) view;
            ImageView imageView = linearLayout.findViewById(R.id.imageView);

            Glide.with(context)
                    .load(imageItemList.get(i).getSmallImageUrl())
                    // .placeholder(R.drawable.bitmap)//加载未完成时显示占位图
                    .diskCacheStrategy(DiskCacheStrategy.NONE)
                    .into(imageView);
        }

        return linearLayout;
    }
}
