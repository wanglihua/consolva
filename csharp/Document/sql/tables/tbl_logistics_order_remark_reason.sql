CREATE TABLE tbl_logistics_order_remark_reason
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    reason      VARCHAR(100) NOT NULL COMMENT '原因',
    description VARCHAR(300) NOT NULL COMMENT '描述',

    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间'
) COMMENT '订单备注原因';

