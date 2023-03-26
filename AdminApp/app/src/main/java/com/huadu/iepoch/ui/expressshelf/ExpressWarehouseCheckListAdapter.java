package com.huadu.iepoch.ui.expressshelf;

import android.app.Activity;
import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.huadu.iepoch.R;
import com.huadu.iepoch.core.ClipboardUtil;
import com.huadu.iepoch.data.http.warehousecheck.WarehouseCheckListItemInfo;

import java.util.ArrayList;
import java.util.List;

public class ExpressWarehouseCheckListAdapter extends BaseAdapter {

    private Context context = null;
    private List<WarehouseCheckListItemInfo> warehouseCheckListItemList = null;

    public ExpressWarehouseCheckListAdapter(Context context, List<WarehouseCheckListItemInfo> warehouseCheckListItemList) {
        this.context = context;
        if (warehouseCheckListItemList != null) {
            this.warehouseCheckListItemList = warehouseCheckListItemList;
        } else {
            this.warehouseCheckListItemList = new ArrayList<>();
        }
    }

    @Override
    public int getCount() {
        return warehouseCheckListItemList.size();
    }

    @Override
    public Object getItem(int position) {
        return warehouseCheckListItemList.get(position);
    }

    @Override
    public long getItemId(int position) {
        return warehouseCheckListItemList.get(position).getId();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        LinearLayout linearLayout = null;
        if (convertView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            linearLayout = (LinearLayout) inflater.inflate(R.layout.item_express_warehouse_check_list, null);
        } else {
            linearLayout = (LinearLayout) convertView;
        }

        WarehouseCheckListItemInfo item = warehouseCheckListItemList.get(position);

        TextView tvTrackingNo = linearLayout.findViewById(R.id.tvTrackingNo);
        TextView tvAoPinTrackingNo = linearLayout.findViewById(R.id.tvAoPinTrackingNo);
        TextView tvStorageLocFullName = linearLayout.findViewById(R.id.tvStorageLocFullName);
        TextView tvClaimShelfStatus = linearLayout.findViewById(R.id.tvClaimShelfStatus);
        TextView tvChatCustomerCode = linearLayout.findViewById(R.id.tvChatCustomerCode);
        TextView tvChatUserLoginName = linearLayout.findViewById(R.id.tvChatUserLoginName);
        TextView tvStatus = linearLayout.findViewById(R.id.tvStatus);
        TextView tvCreateTime = linearLayout.findViewById(R.id.tvCreateTime);

        tvTrackingNo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Activity activity = ((ExpressWarehouseCheckListActivity) (ExpressWarehouseCheckListAdapter.this.context));
                ClipboardUtil.copyToClipboard(activity, item.getTrackingNo());

                Toast.makeText(activity, "快递单号成功复制到剪贴板！", Toast.LENGTH_LONG).show();
            }
        });

        tvAoPinTrackingNo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Activity activity = ((ExpressWarehouseCheckListActivity) (ExpressWarehouseCheckListAdapter.this.context));
                ClipboardUtil.copyToClipboard(activity, item.getAoPinTrackingNo());

                Toast.makeText(activity, "澳拼入库号成功复制到剪贴板！", Toast.LENGTH_LONG).show();
            }
        });

        tvTrackingNo.setText("快递单号：" + item.getTrackingNo());
        tvAoPinTrackingNo.setText("澳拼入库号：" + item.getAoPinTrackingNo());
        tvStorageLocFullName.setText("仓位：" + item.getStorageLocFullName());

        if (item.getToOnShelf()) {
            tvClaimShelfStatus.setTextColor(Color.RED);
        }

        tvClaimShelfStatus.setText("上架状态：" + item.getClaimShelfStatusName());

        tvChatCustomerCode.setText("聊天客户：" + item.getChatCustomerCode());
        tvChatUserLoginName.setText("聊天客服：" + item.getChatUserLoginName());
        tvStatus.setText("状态：" + item.getStatusName());
        tvCreateTime.setText("创建时间：" + item.getCreateTime());

        return linearLayout;
    }
}
