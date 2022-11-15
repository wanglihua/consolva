/*

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS tbl_express;

SET FOREIGN_KEY_CHECKS = 1;

 */

CREATE TABLE tbl_express
(
    id          INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    tracking_no VARCHAR(30) NOT NULL COMMENT '快递单号',
    order_id    INT         NULL COMMENT '物流订单',
    order_code VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号',
    customer_id INT         NULL COMMENT '客户',

    create_time DATETIME    NOT NULL COMMENT '创建时间',
    update_time DATETIME    NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_express_tracking_no (tracking_no),
    CONSTRAINT fk_express_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '商品快递';
