package com.huadu.shunda.data.domain;

import java.util.Arrays;
import java.util.List;

public class OrderPhotoType extends CodeType {

    /**
     * 签收照片
     */
    public static final OrderPhotoType SignIn = new OrderPhotoType(1000, "签收照片", "签收照片");

    /**
     * 入库照片
     */
    public static final OrderPhotoType CheckIn = new OrderPhotoType(2000, "入库照片", "入库照片");
    /**
     * 问题件照片
     */
    public static final OrderPhotoType Pack = new OrderPhotoType(3000, "打包照片", "打包照片");

    /**
     * 问题件照片
     */
    public static final OrderPhotoType Issue = new OrderPhotoType(4000, "问题件照片", "问题件照片");

    /**
     * 发货照片
     */
    public static final OrderPhotoType Ship = new OrderPhotoType(5000, "发货照片", "发货照片");

    /**
     * 中国损毁照片
     */
    public static final OrderPhotoType CnDamage = new OrderPhotoType(6000, "中国损毁照片", "中国损毁照片");

    /**
     * 退运照片
     */
    public static final OrderPhotoType Return = new OrderPhotoType(7000, "退运照片", "退运照片");

    /**
     * 客服核对照片
     */
    public static final OrderPhotoType CustomerServiceCheck = new OrderPhotoType(9000, "客服核对照片", "客服核对照片");

    /**
     * 客服售后照片
     */
    public static final OrderPhotoType CustomerServicePostSale = new OrderPhotoType(10000, "客服售后照片", "客服售后照片");

    /**
     * 交付照片
     */
    public static final OrderPhotoType Deliver = new OrderPhotoType(88000, "交付照片", "交付照片");

    /**
     * 派送失败照片
     */
    public static final OrderPhotoType DeliverFail = new OrderPhotoType(89000, "派送失败照片", "派送失败照片");

    /**
     * None
     */
    public static final OrderPhotoType None = new OrderPhotoType(0, "", "");

    private OrderPhotoType(int id, String name, String description) {
        super(id, name, description);
    }

    public static OrderPhotoType getById(int id) {
        for (OrderPhotoType type : getList()) {
            if (type.getId() == id) {
                return type;
            }
        }

        return None;
    }

    public static List<OrderPhotoType> getList() {
        return Arrays.asList(SignIn, CheckIn, Pack, Issue, Ship, CnDamage, Return, CustomerServiceCheck, CustomerServicePostSale, Deliver, DeliverFail);
    }
}
