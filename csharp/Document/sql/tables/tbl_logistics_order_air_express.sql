CREATE TABLE tbl_logistics_order_air_express
(
    id                INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id          INT                     NULL COMMENT '订单',   # 导入的时候还没关联订单，这样方便，然后后面通过界面或者导入关联订单
    order_code        VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号',
    logistics_mode_id INT                     NULL COMMENT '物流方式', # 这个可以和【订单打包】里面存储的物流方式关联起来
    tracking_no       VARCHAR(30)             NOT NULL COMMENT '国际快递单号',
    shipping_weight   DECIMAL(10, 3)          NULL COMMENT '承运人计费重量Kg',
    estimate_fee      DECIMAL(10, 2)          NULL COMMENT '预估运费支出RMB',
    shipping_fee      DECIMAL(10, 2)          NULL COMMENT '运费支出RMB',
    channel           VARCHAR(50)             NULL COMMENT '发货渠道',
    memo              VARCHAR(300)            NULL COMMENT '空运备注',

    import_user_id    INT                     NULL COMMENT '导入人',
    import_time       DATETIME                NULL COMMENT '导入时间',
    delivered_time    DATETIME                NULL COMMENT '签收时间',
    status_id         INT          DEFAULT 0  NOT NULL COMMENT '状态#type:LogisticsOrderAirExpressStatus',

    create_time       DATETIME                NOT NULL COMMENT '创建时间',
    update_time       DATETIME                NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_order_air_express_tracking_no (tracking_no),
    CONSTRAINT fk_logistics_order_air_express_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_air_express_logistics_mode_id FOREIGN KEY (logistics_mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT fk_logistics_order_air_express_import_user_id FOREIGN KEY (import_user_id) REFERENCES tbl_user (id)

) COMMENT '国际单号';

# 一个订单多个物流方式的怎么处理
# 【订单打包箱号】里面可以加上一个字段  air_express_id，【订单打包】和空运快递单号通过物流方式（logistics_mode_id）关联起来后，就可以把这个air_express_id填到【订单打包箱号】
# 如果是 一个渠道发了两个单号，那怎么搞。这时，也许可以在订单详细【订单打包箱号】的信息展示里面，让【订单打包箱号】可以选择一个空运单号并保存。一个渠道一个单号的，就没这个问题

