CREATE TABLE tbl_logistics_order
(
    id                     INT                         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    code                   VARCHAR(100)   DEFAULT ''   NOT NULL COMMENT '订单编号',
    customer_id            INT            DEFAULT 0    NOT NULL COMMENT '客户',
    customer_code          VARCHAR(30)                 NOT NULL COMMENT '客户ID',
    fee_method_id          INT            DEFAULT 0    NOT NULL COMMENT '订单收费方式#type:LogisticsOrderFeeMethod',
    create_method_id       INT            DEFAULT 0    NOT NULL COMMENT '创建方式#type:LogisticsOrderCreateMethod',
    warehouse_id           INT                         NOT NULL COMMENT '收货仓库',
    logistics_mode_type_id INT                         NOT NULL COMMENT '物流方式类型#type:LogisticsModeType',
    logistics_mode_id      INT                         NOT NULL COMMENT '物流方式',
    goods_type_id          INT            DEFAULT 0    NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',
    order_status_id        INT            DEFAULT 0    NOT NULL COMMENT '订单状态', # todo
    pay_status_id          INT            DEFAULT 0    NOT NULL COMMENT '支付状态', # todo
    pay_type_id            INT                         NULL COMMENT '支付方式',     # todo
    goods_price            DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '商品总价',
    user_account           DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '使用余额',
    order_amount           DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '应付款金额',
    paid_amount            DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '已支付金额',
    insurance_id           INT                         NOT NULL COMMENT '保险id',
    gst_id                 INT                         NOT NULL COMMENT '纳税申报',

    customer_note          VARCHAR(250)   DEFAULT ''   NOT NULL COMMENT '客户备注',
    service_note           VARCHAR(255)   DEFAULT ''   NOT NULL COMMENT '客服备注',
    warehouse_note         VARCHAR(255)   DEFAULT ''   NOT NULL COMMENT '仓库备注',

    repack_type_id         INT                         NULL COMMENT '打包类型#type:LogisticsRepackType',

    storage_loc_id         INT                         NULL COMMENT '仓位',
    storage_loc_count      INT                         NULL COMMENT '占用仓位数',
    pickup_code            VARCHAR(12)    DEFAULT ''   NOT NULL COMMENT '取件码',

    total_actual_weight    DECIMAL(10, 3)              NULL COMMENT '实际重量',
    total_volume_weight    DECIMAL(10, 3)              NULL COMMENT '体积重',
    total_weight           DECIMAL(10, 3)              NULL COMMENT '总重量',
    total_package_count    INT                         NULL COMMENT '总箱数',
    total_volume           DECIMAL(14, 6)              NULL COMMENT '总体积',

    pay_time               DATETIME                    NULL COMMENT '支付时间',
    submit_pack_time       DATETIME                    NULL COMMENT '提交打包时间',
    transfer_time          DATETIME                    NULL COMMENT '转运时间',
    delivered_time         DATETIME                    NULL COMMENT '签收时间',

    create_time            DATETIME                    NOT NULL COMMENT '创建时间',
    update_time            DATETIME                    NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_logistics_order_code (code),
    INDEX ix_logistics_order_order_status_id (order_status_id),
    INDEX ix_logistics_order_pickup_code (pickup_code),
    INDEX ix_logistics_order_logistics_mode_type_id (logistics_mode_type_id),
    INDEX ix_logistics_order_create_time (create_time),

    CONSTRAINT fk_logistics_order_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_logistics_order_warehouse_id FOREIGN KEY (warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT fk_logistics_order_logistics_mode_id FOREIGN KEY (logistics_mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT fk_logistics_order_insurance_id FOREIGN KEY (insurance_id) REFERENCES tbl_logistics_insurance (id),
    CONSTRAINT fk_logistics_order_gst_id FOREIGN KEY (gst_id) REFERENCES tbl_logistics_gst (id),
    CONSTRAINT fk_logistics_order_storage_loc_id FOREIGN KEY (storage_loc_id) REFERENCES tbl_storage_loc (id)
) COMMENT '物流订单';


# 一个订单多个物流方式的怎么处理
# 可以把物流方式（logistics_mode_id）存储到【订单打包】里面，订单里面存储的是用户选择的那个，会优先使用这个选择的