package com.huadu.iepoch.ui.expressshelf;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.huadu.iepoch.R;
import com.huadu.iepoch.data.http.warehousecheck.WarehouseCheckShelfListItemInfo;

import java.util.List;

public class ExpressWarehouseCheckShelfListAdapter extends BaseAdapter {

    private Context context = null;
    private List<WarehouseCheckShelfListItemInfo> shelfItemList = null;

    public ExpressWarehouseCheckShelfListAdapter(Context context, List<WarehouseCheckShelfListItemInfo> shelfItemList) {
        this.context = context;
        this.shelfItemList = shelfItemList;
    }

    @Override
    public int getCount() {
        return shelfItemList.size();
    }

    @Override
    public Object getItem(int position) {
        return shelfItemList.get(position);
    }

    @Override
    public long getItemId(int position) {
        return shelfItemList.get(position).getShelfId();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        LinearLayout linearLayout = null;
        if (convertView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            linearLayout = (LinearLayout) inflater.inflate(R.layout.item_express_warehouse_check_shelf_list, null);
        } else {
            linearLayout = (LinearLayout) convertView;
        }

        WarehouseCheckShelfListItemInfo item = shelfItemList.get(position);

        TextView tvShelfName = linearLayout.findViewById(R.id.tvShelfName);
        tvShelfName.setText(item.getShelfName() + "(" + item.getCheckCount() + ")");

        return linearLayout;
    }
}
