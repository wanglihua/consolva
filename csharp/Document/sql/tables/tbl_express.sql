/*

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS tbl_express;

SET FOREIGN_KEY_CHECKS = 1;

 */

CREATE TABLE tbl_express
(
    id                     INT                          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    tracking_no            VARCHAR(30)    DEFAULT ''    NOT NULL COMMENT '快递单号',
    checkin_tracking_no    VARCHAR(20)    DEFAULT ''    NOT NULL COMMENT '入库跟踪号',
    express_company        VARCHAR(100)   DEFAULT ''    NOT NULL COMMENT '快递公司',
    goods_type_id          INT            DEFAULT 0     NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',
    add_time               DATETIME                     NOT NULL COMMENT '入库时间',
    add_user_id            INT                          NOT NULL COMMENT '添加用户',
    bill_customer_code     VARCHAR(50)    DEFAULT ''    NOT NULL COMMENT '面单客户',
    bill_customer_id       INT            DEFAULT 0     NOT NULL COMMENT '面单客户Id', # 这个字段不加外键，加索引
    warehouse_id           INT                          NOT NULL COMMENT '仓库',
    shelf_id               INT                          NOT NULL COMMENT '货架',
    storage_loc_id         INT                          NOT NULL COMMENT '库位',
    package_count          INT                          NOT NULL COMMENT '包裹数量',
    claim_time             DATETIME                     NULL COMMENT '认领时间',
    customer_ignored       BOOL           DEFAULT FALSE NOT NULL COMMENT '客户忽略',
    order_id               INT                          NULL COMMENT '物流订单',
    order_code             VARCHAR(100)   DEFAULT ''    NOT NULL COMMENT '订单编号',
    customer_id            INT                          NULL COMMENT '客户',
    first_type_id          INT            DEFAULT 0     NOT NULL COMMENT '头程方式#type:LogisticsModeType',
    first_mode_id          INT                          NULL COMMENT '头程物流',
    submit_pack_time       DATETIME                     NULL COMMENT '提交打包时间',

    total_weight           DECIMAL(10, 3) DEFAULT 0     NOT NULL COMMENT '总重量',    # KG
    total_volume           DECIMAL(14, 6) DEFAULT 0     NOT NULL COMMENT '总体积',    # 立方米,

    on_shelf_time          DATETIME                     NULL COMMENT '上架时间',
    on_shelf_user_id       INT                          NULL COMMENT '上架用户',
    off_shelf_time         DATETIME                     NULL COMMENT '下架时间',
    off_shelf_user_id      INT                          NULL COMMENT '下架用户',
    packing_time           DATETIME                     NULL COMMENT '入流动仓时间',
    packing_user_id        INT                          NULL COMMENT '入流动仓用户',
    shelf_status_id        INT            DEFAULT 0     NOT NULL COMMENT '快递单货架状态#type:ExpressShelfStatus',

    return_tracking_no     VARCHAR(30)    DEFAULT ''    NOT NULL COMMENT '退运快递单号', # todo 必要的时候把这些移到另一个表里面
    return_express_company VARCHAR(100)   DEFAULT ''    NOT NULL COMMENT '退运快递公司',
    return_apply_time      DATETIME                     NULL COMMENT '退运申请时间',
    return_pickup_code     VARCHAR(20)                  NULL COMMENT '退运取件码',
    return_user_id         INT                          NULL COMMENT '退运操作人',
    return_time            DATETIME                     NULL COMMENT '退运时间',

    dump_user_id           INT                          NULL COMMENT '堆存用户',
    dump_time              DATETIME                     NULL COMMENT '堆存时间',
    dump_status_id         INT            DEFAULT 0     NOT NULL COMMENT '堆存状态#type:ExpressDumpStatus',

    archive_type_id        INT            DEFAULT 0     NOT NULL COMMENT '归档类型#type:ExpressArchiveType',
    status_id              INT            DEFAULT 0     NOT NULL COMMENT '状态#type:ExpressStatus',

    create_time            DATETIME                     NOT NULL COMMENT '创建时间',
    update_time            DATETIME                     NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_express_tracking_no (tracking_no),
    UNIQUE INDEX ix_express_checkin_tracking_no (checkin_tracking_no),
    INDEX ix_express_return_tracking_no (return_tracking_no),
    INDEX ix_express_bill_customer_id (bill_customer_id),
    INDEX ix_express_status_id (status_id), # 这个索引必须要加上的
    CONSTRAINT fk_express_add_user_id FOREIGN KEY (add_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_express_warehouse_id FOREIGN KEY (warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT fk_express_shelf_id FOREIGN KEY (shelf_id) REFERENCES tbl_shelf (id),
    CONSTRAINT fk_express_storage_loc_id FOREIGN KEY (storage_loc_id) REFERENCES tbl_storage_loc (id),
    CONSTRAINT fk_express_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_express_first_mode_id FOREIGN KEY (first_mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT fk_express_on_shelf_user_id FOREIGN KEY (on_shelf_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_express_off_shelf_user_id FOREIGN KEY (off_shelf_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_express_packing_user_id FOREIGN KEY (packing_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_express_dump_user_id FOREIGN KEY (dump_user_id) REFERENCES tbl_user (id)
) COMMENT '商品快递';
