CREATE TABLE tbl_logistics_order
(
    id                            INT(11)        NOT NULL AUTO_INCREMENT COMMENT '订单id',
    code                          VARCHAR(100)   NOT NULL DEFAULT '' COMMENT '订单编号',
    customer_code                 VARCHAR(30)    NOT NULL COMMENT '客户ID',
    goods_type_id                 INT(11)        NOT NULL DEFAULT '0' COMMENT '物品类型',
    customer_id                   INT(11)        NOT NULL DEFAULT '0' COMMENT '客户',
    order_status_id               INT(11)        NOT NULL DEFAULT '0' COMMENT '订单状态',
    pay_status_id                 INT(11)        NOT NULL DEFAULT '0' COMMENT '支付状态',
    consignee                     VARCHAR(60)    NOT NULL DEFAULT '' COMMENT '收货人',
    country                       VARCHAR(30)             DEFAULT '' COMMENT '国家',
    province                      VARCHAR(60)    NOT NULL DEFAULT '' COMMENT '省份',
    city                          VARCHAR(60)    NOT NULL DEFAULT '' COMMENT '城市',
    company                       VARCHAR(100)            DEFAULT '' COMMENT '公司',
    tfn                           VARCHAR(100)            DEFAULT '' COMMENT 'ABN/ACN',
    address                       VARCHAR(200)   NOT NULL DEFAULT '' COMMENT '地址',
    zipcode                       VARCHAR(60)    NOT NULL DEFAULT '' COMMENT '邮政编码',
    mobile                        VARCHAR(60)    NOT NULL DEFAULT '' COMMENT '手机',
    email                         VARCHAR(200)            DEFAULT '' COMMENT '邮箱地址',
    invoice_title                 VARCHAR(256)            DEFAULT '' COMMENT '发票抬头',
    pay_type_id                   INT(11)                 DEFAULT NULL COMMENT '支付方式',
    goods_price                   DECIMAL(10, 2) NOT NULL DEFAULT '0.00' COMMENT '商品总价',
    shipping_price                DECIMAL(10, 2) NOT NULL DEFAULT '0.00' COMMENT '邮费',
    user_money                    DECIMAL(10, 2) NOT NULL DEFAULT '0.00' COMMENT '使用余额',
    order_amount                  DECIMAL(10, 2) NOT NULL DEFAULT '0.00' COMMENT '应付款金额',
    paid_amount                   DECIMAL(10, 2) NOT NULL DEFAULT '0.00' COMMENT '已支付金额',
    insurance_id                  INT(11)        NOT NULL COMMENT '保险id',
    gst_id                        INT(11)        NOT NULL COMMENT '纳税申报',
    receiving_warehouse_id        INT(11)        NOT NULL COMMENT '国内收货仓库',
    target_warehouse_id           INT(11)                 DEFAULT NULL COMMENT '澳洲目的仓库',
    first_logistics_mode_type_id  INT(11)        NOT NULL COMMENT '跨国段物流方式类型',
    first_logistics_mode_id       INT(11)        NOT NULL COMMENT '跨国段物流方式',
    ship_container_code           VARCHAR(30)             DEFAULT NULL COMMENT '客户端船航，头程信息',
    second_logistics_mode_type_id INT(11)                 DEFAULT NULL COMMENT '澳洲段物流方式类型',
    second_logistics_mode_id      INT(11)                 DEFAULT NULL COMMENT '澳洲段物流方式',
    customer_note                 VARCHAR(250)            DEFAULT '' COMMENT '客户备注',
    service_note                  VARCHAR(255)            DEFAULT '' COMMENT '客服备注',
    warehouse_note                VARCHAR(255)            DEFAULT '' COMMENT '仓库备注',
    shipping_time                 DATETIME                DEFAULT NULL COMMENT '最后新发货时间',
    confirm_time                  DATETIME                DEFAULT NULL COMMENT '收货确认时间',
    pay_time                      DATETIME                DEFAULT NULL COMMENT '支付时间',
    repack_type_id                INT(11)                 DEFAULT NULL COMMENT '打包类型',
    submit_pack_time              DATETIME                DEFAULT NULL COMMENT '提交打包时间',
    storage_loc_id                INT(11)                 DEFAULT NULL COMMENT '仓位',
    pickup_code                   VARCHAR(12)             DEFAULT '' COMMENT '取件码',
    cate_id                       INT(11)                 DEFAULT NULL COMMENT '订单类别',
    air_express_id                INT(11)                 DEFAULT NULL COMMENT '国际快递单',
    air_express_tracking_no       VARCHAR(30)             DEFAULT NULL COMMENT '国际快递单号',
    air_express_weight            DECIMAL(10, 3)          DEFAULT NULL COMMENT '承运人计费冲凉Kg',
    air_express_fee               DECIMAL(10, 2)          DEFAULT NULL COMMENT '运费支出RMB',
    air_express_channel           VARCHAR(50)             DEFAULT NULL COMMENT '发货渠道',
    air_express_memo              VARCHAR(300)            DEFAULT NULL COMMENT '空运备注',
    air_express_import_user_id    INT(11)                 DEFAULT NULL COMMENT '导入人',
    air_express_import_time       DATETIME                DEFAULT NULL COMMENT '导入时间',
    total_actual_weight           DECIMAL(10, 3)          DEFAULT NULL COMMENT '实际重量',
    total_volume_weight           DECIMAL(10, 3)          DEFAULT NULL COMMENT '体积重',
    total_side_avg_volumn_weight  DECIMAL(10, 3)          DEFAULT NULL COMMENT '三边之和体积重',
    total_weight                  DECIMAL(10, 3)          DEFAULT NULL COMMENT '总重量',
    total_package_count           DECIMAL(10, 0)          DEFAULT NULL COMMENT '总箱数',
    total_volume                  DECIMAL(14, 6)          DEFAULT NULL COMMENT '总体积',
    max_side_length               DECIMAL(10, 2)          DEFAULT NULL COMMENT '最长边',
    max_volume_single_pack        DECIMAL(14, 6)          DEFAULT NULL COMMENT '单箱最大体积',
    max_weight_single_pack        DECIMAL(14, 6)          DEFAULT NULL COMMENT '单箱最大重量',
    cn_transfer_time              DATETIME                DEFAULT NULL COMMENT '转运时间',
    cn_depart_time                DATETIME                DEFAULT NULL COMMENT '离港时间',
    au_arrive_time                DATETIME                DEFAULT NULL COMMENT '到港时间',
    au_checkin_time               DATETIME                DEFAULT NULL COMMENT '到仓时间',
    au_deliver_ready_time         DATETIME                DEFAULT NULL COMMENT '可自提时间',
    create_time                   DATETIME       NOT NULL COMMENT '创建时间',
    update_time                   DATETIME       NOT NULL COMMENT '更新时间',
    version                       INT(11)        NOT NULL DEFAULT '1' COMMENT '数据版本',
    PRIMARY KEY (id),
    UNIQUE KEY ix_logistics_order_code (code),
    KEY ix_logistics_order_order_status_id (order_status_id),
    KEY ix_logistics_order_pay_status_id (pay_status_id),
    KEY ix_logistics_order_pickup_code (pickup_code),
    KEY ix_logistics_order_create_time (create_time),
    KEY ix_logistics_order_order_goods_type (goods_type_id),
    KEY ix_logistics_order_order_country (country),
    KEY ix_logistics_order_order_province (province),
    KEY ix_logistics_order_order_city (city),
    KEY ix_logistics_order_order_zipcode (zipcode),
    KEY ix_logistics_order_order_mobile (mobile),
    KEY ix_logistics_order_order_pay_type_id (pay_type_id),
    KEY ix_logistics_order_order_customer_id (customer_id),
    KEY ix_logistics_order_first_logistics_mode_type_id (first_logistics_mode_type_id),
    KEY ix_logistics_order_second_logistics_mode_type_id (second_logistics_mode_type_id),
    KEY fk_logistics_order_insurance_id (insurance_id),
    KEY fk_logistics_order_gst_id (gst_id),
    KEY fk_logistics_order_cn_receiving_warehouse_id (receiving_warehouse_id),
    KEY fk_logistics_order_au_target_warehouse_id (target_warehouse_id),
    KEY fk_logistics_order_first_logistics_mode_id (first_logistics_mode_id),
    KEY fk_logistics_order_second_logistics_mode_id (second_logistics_mode_id),
    KEY ix_logistics_order_split_status_id (split_status_id),
    KEY ix_logistics_order_storage_loc_id (storage_loc_id),
    KEY ix_logistics_order_submit_pack_time (submit_pack_time),
    KEY ix_logistics_order_combine_status_id (combine_status_id),
    KEY ix_logistics_order_update_time (update_time),
    KEY ix_logistics_order_air_express_tracking_no (air_express_tracking_no),
    KEY fk_logistics_order_air_express_import_user_id (air_express_import_user_id),
    KEY ix_logistics_order_cate_id (cate_id),
    KEY fk_logistics_order_air_express_air_express_id (air_express_id),
    CONSTRAINT tbl_logistics_order_ibfk_1 FOREIGN KEY (insurance_id) REFERENCES tbl_logistics_insurance (id),
    CONSTRAINT tbl_logistics_order_ibfk_2 FOREIGN KEY (gst_id) REFERENCES tbl_logistics_gst (id),
    CONSTRAINT tbl_logistics_order_ibfk_3 FOREIGN KEY (receiving_warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT tbl_logistics_order_ibfk_4 FOREIGN KEY (target_warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT tbl_logistics_order_ibfk_5 FOREIGN KEY (first_logistics_mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT tbl_logistics_order_ibfk_6 FOREIGN KEY (second_logistics_mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT tbl_logistics_order_ibfk_7 FOREIGN KEY (air_express_import_user_id) REFERENCES tbl_user (id),
    CONSTRAINT tbl_logistics_order_ibfk_8 FOREIGN KEY (air_express_id) REFERENCES tbl_logistics_order_air_express (id)
) ENGINE = InnoDB
  AUTO_INCREMENT = 77983
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='物流订单';


ALTER TABLE tbl_logistics_order
    ADD storage_loc_count INT NULL COMMENT '占用仓位数' AFTER storage_loc_id;


ALTER TABLE tbl_logistics_order
    ADD CONSTRAINT fk_logistics_order_storage_loc_id FOREIGN KEY (storage_loc_id) REFERENCES tbl_storage_loc (id);


ALTER TABLE tbl_logistics_order
    ADD last_name VARCHAR(60) DEFAULT '' NOT NULL COMMENT '收货人姓' AFTER consignee;


ALTER TABLE tbl_logistics_order
    ADD fee_method_id INT NULL COMMENT '订单收费方式';

ALTER TABLE tbl_logistics_order
    ADD unit_loc VARCHAR(50) NULL COMMENT 'Unit号';

ALTER TABLE tbl_logistics_order
    ADD create_method_id INT NOT NULL DEFAULT 1000 COMMENT '创建方式';

ALTER TABLE tbl_logistics_order
    MODIFY id INT NOT NULL COMMENT '订单id';