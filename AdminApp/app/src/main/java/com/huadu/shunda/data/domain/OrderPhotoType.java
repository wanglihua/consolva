package com.huadu.shunda.data.domain;

import java.util.Arrays;
import java.util.List;

public class OrderPhotoType extends CodeType {

    /**
     * 签收照片
     */
    public static final OrderPhotoType SignIn = new OrderPhotoType(100, "签收照片", "签收照片");

    /**
     * 入库照片
     */
    public static final OrderPhotoType CheckIn = new OrderPhotoType(200, "入库照片", "入库照片");

    /**
     * 问题件照片
     */
    public static final OrderPhotoType Issue = new OrderPhotoType(300, "问题件照片", "问题件照片");

    /**
     * 发货照片
     */
    public static final OrderPhotoType Ship = new OrderPhotoType(350, "发货照片", "发货照片");

    /**
     * 中国损毁照片
     */
    public static final OrderPhotoType CnDamage = new OrderPhotoType(400, "中国损毁照片", "中国损毁照片");

    /**
     * 退运照片
     */
    public static final OrderPhotoType Return = new OrderPhotoType(500, "退运照片", "退运照片");

    /**
     * 退运照片
     */
    public static final OrderPhotoType AuDamage = new OrderPhotoType(600, "澳洲损毁照片", "澳洲损毁照片");

    /**
     * 客服核对照片
     */
    public static final OrderPhotoType CustomerServiceCheck = new OrderPhotoType(700, "客服核对照片", "客服核对照片");

    /**
     * 客服售后照片
     */
    public static final OrderPhotoType CustomerServicePostSale = new OrderPhotoType(800, "客服售后照片", "客服售后照片");

    /**
     * 交付照片
     */
    public static final OrderPhotoType Deliver = new OrderPhotoType(900, "交付照片", "交付照片");

    /**
     * 派送失败照片
     */
    public static final OrderPhotoType DeliverFail = new OrderPhotoType(1000, "派送失败照片", "派送失败照片");

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
        return Arrays.asList(SignIn, CheckIn, Issue, Ship, CnDamage, Return, AuDamage, CustomerServiceCheck, CustomerServicePostSale, Deliver, DeliverFail);
    }
}
