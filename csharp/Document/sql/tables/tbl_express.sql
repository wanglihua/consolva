CREATE TABLE tbl_express
(
    id          INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    tracking_no VARCHAR(30) NOT NULL COMMENT '快递单号',

    create_time DATETIME    NOT NULL COMMENT '创建时间',
    update_time DATETIME    NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_express_tracking_no (tracking_no)
) COMMENT '商品快递';
