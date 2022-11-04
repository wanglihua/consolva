# DROP TABLE IF EXISTS tbl_logistics_order_code_customer;

CREATE TABLE tbl_logistics_order_code_customer
(
    customer_id INT      NOT NULL PRIMARY KEY COMMENT '客户Id',
    seq         INT      NOT NULL DEFAULT 1 COMMENT '序列号',
    create_time DATETIME NOT NULL COMMENT '创建时间',
    update_time DATETIME NOT NULL COMMENT '更新时间'
) COMMENT '物流订单编码按客户';