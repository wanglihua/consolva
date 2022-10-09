CREATE TABLE tbl_logistics_order_air_express
(
    id             INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id       INT            NULL COMMENT '订单',        # 这个是外键
    tracking_no    VARCHAR(30)    NOT NULL COMMENT '国际快递单号',
    weight         DECIMAL(10, 3) NULL COMMENT '承运人计费重量Kg',
    estimate_fee   DECIMAL(10, 2) NULL COMMENT '预估运费支出RMB',
    fee            DECIMAL(10, 2) NULL COMMENT '运费支出RMB',
    channel        VARCHAR(50)    NULL COMMENT '发货渠道',
    memo           VARCHAR(300)   NULL COMMENT '空运备注',
    import_user_id INT            NULL COMMENT '导入人',
    import_time    DATETIME       NULL COMMENT '导入时间',

    create_time    DATETIME       NOT NULL COMMENT '创建时间',
    update_time    DATETIME       NOT NULL COMMENT '更新时间',
    version        INT            NOT NULL DEFAULT 1 COMMENT '数据版本',

    INDEX ix_logistics_order_air_express_tracking_no (tracking_no),
    FOREIGN KEY fk_logistics_order_air_express_import_user_id (import_user_id) REFERENCES tbl_user (id)

) COMMENT '空运订单国际快递单';

ALTER TABLE tbl_logistics_order_air_express
    ADD status_id INT NOT NULL DEFAULT 0 COMMENT '状态' AFTER import_time;

ALTER TABLE tbl_logistics_order_air_express
    ADD delivered_time DATETIME NULL COMMENT '签收时间' AFTER import_time;

ALTER TABLE tbl_logistics_order_air_express
    ADD estimate_fee DECIMAL(10, 2) NULL COMMENT '预估运费支出RMB' AFTER weight;
