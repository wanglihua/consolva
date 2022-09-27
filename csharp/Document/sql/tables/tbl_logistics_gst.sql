CREATE TABLE tbl_logistics_gst
(
    id                INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    name              VARCHAR(200)   NOT NULL COMMENT '名称',
    calc_base_type_id INT            NOT NULL COMMENT '计算基数#type:FeeCalcBaseType',
    start_amount      DECIMAL(10, 2) NOT NULL COMMENT '启征金额',
    fee_rate          DECIMAL(10, 4) NOT NULL COMMENT '费率',
    order_num         INT            NULL COMMENT '排序',
    status_id         INT            NOT NULL COMMENT '状态#type:LogisticsGstStatus',

    create_time       DATETIME       NOT NULL COMMENT '创建时间',
    update_time       DATETIME       NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_gst_order_num (order_num)
) COMMENT 'GST';
