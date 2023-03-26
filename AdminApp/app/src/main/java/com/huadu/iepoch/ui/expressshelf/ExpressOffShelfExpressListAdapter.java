package com.huadu.iepoch.ui.expressshelf;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.huadu.iepoch.R;
import com.huadu.iepoch.data.http.expressshelf.ExpressShelfOffShelfExpressItemInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressOffShelfExpressListAdapter extends BaseAdapter {

    private Context context;
    private List<ExpressShelfOffShelfExpressItemInfo> expressItemList = null;

    public ExpressOffShelfExpressListAdapter(Context context, List<ExpressShelfOffShelfExpressItemInfo> expressItemList) {
        this.context = context;

        if (expressItemList != null) {
            this.expressItemList = expressItemList;
        } else {
            this.expressItemList = new ArrayList<>();
        }
    }

    @Override
    public int getCount() {
        return expressItemList.size();
    }

    @Override
    public Object getItem(int position) {
        return expressItemList.get(position);
    }

    @Override
    public long getItemId(int position) {
        return expressItemList.get(position).getId();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        LinearLayout linearLayout = null;
        if (convertView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            linearLayout = (LinearLayout) inflater.inflate(R.layout.item_express_off_shelf_express_list, null);
        } else {
            linearLayout = (LinearLayout) convertView;
        }

        ExpressShelfOffShelfExpressItemInfo item = expressItemList.get(position);

        TextView tvTrackingNo = linearLayout.findViewById(R.id.tvTrackingNo);
        TextView tvAoPinTrackingNo = linearLayout.findViewById(R.id.tvAoPinTrackingNo);
        TextView tvCustomerCode = linearLayout.findViewById(R.id.tvCustomerCode);
        TextView tvPackageCount = linearLayout.findViewById(R.id.tvPackageCount);
        // TextView tvAddUserNameLabel = linearLayout.findViewById(R.id.tvAddUserNameLabel);
        TextView tvAddUserName = linearLayout.findViewById(R.id.tvAddUserName);
        TextView tvAddTime = linearLayout.findViewById(R.id.tvAddTime);
        TextView tvStorageLocName = linearLayout.findViewById(R.id.tvStorageLocName);
        TextView tvShelfStatus = linearLayout.findViewById(R.id.tvShelfStatus);

        tvTrackingNo.setText(item.getTrackingNo());
        if (item.getNotOnShelf()) {
            tvTrackingNo.setTextColor(Color.RED);
        } else {
            tvTrackingNo.setTextColor(0xFF666666);
        }

        tvAoPinTrackingNo.setText("澳拼入库号：" + item.getAoPinTrackingNo());
        tvCustomerCode.setText("客户ID：" + item.getCustomerCode());
        tvPackageCount.setText("包裹数量：" + item.getPackageCount());
        tvStorageLocName.setText("仓位：" + item.getStorageLocName());

        String shelfStatus = item.getShelfStatus();
        if (shelfStatus == null) {
            shelfStatus = "";
        }
        tvShelfStatus.setText("上架状态：" + shelfStatus);

        tvAddUserName.setText(item.getAddUserName());
        tvAddTime.setText(item.getAddTime());

        return linearLayout;
    }
}
