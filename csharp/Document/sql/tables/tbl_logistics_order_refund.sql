CREATE TABLE tbl_logistics_order_refund
(
    id          INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id    INT            NOT NULL COMMENT '物流订单',
    customer_id INT            NOT NULL COMMENT '会员',
    type_id     INT            NOT NULL COMMENT '类型', # 退款，赔款
    reason_id   INT            NOT NULL COMMENT '退赔原因',
    reason      VARCHAR(300)   NOT NULL COMMENT '退赔款原因',
    amount      DECIMAL(10, 2) NOT NULL COMMENT '退赔金额',
    pay_type_id INT            NOT NULL COMMENT '退赔支付方式',
    status_id   INT            NOT NULL COMMENT '状态', # 未支付，已支付 等等
    apply_id    INT            NOT NULL COMMENT '对应申请',
    create_time DATETIME       NOT NULL COMMENT '创建时间',
    update_time DATETIME       NOT NULL COMMENT '更新时间',
    version     INT            NOT NULL DEFAULT 1 COMMENT '数据版本',

    FOREIGN KEY fk_logistics_order_refund_order_id (order_id) REFERENCES tbl_logistics_order (id),
    FOREIGN KEY fk_logistics_order_refund_customer_id (customer_id) REFERENCES tbl_customer (id),
    FOREIGN KEY fk_logistics_order_refund_reason_id (reason_id) REFERENCES tbl_refund_reason (id),
    FOREIGN KEY fk_logistics_order_refund_apply_id (apply_id) REFERENCES tbl_logistics_order_refund_apply (id),
    INDEX ix_logistics_order_refund_type_id (type_id),
    INDEX ix_logistics_order_refund_pay_type_id (pay_type_id),
    INDEX ix_logistics_order_refund_status_id (status_id)
) COMMENT '订单退赔款';

ALTER TABLE tbl_logistics_order_refund
    ADD customer_id INT NOT NULL COMMENT '会员' AFTER order_id;

ALTER TABLE tbl_logistics_order_refund
    ADD reason_id INT NOT NULL COMMENT '退赔原因' AFTER type_id;

ALTER TABLE tbl_logistics_order_refund
    ADD FOREIGN KEY fk_logistics_order_refund_customer_id (customer_id) REFERENCES tbl_customer (id);

ALTER TABLE tbl_logistics_order_refund
    ADD FOREIGN KEY fk_logistics_order_refund_reason_id (reason_id) REFERENCES tbl_refund_reason (id);