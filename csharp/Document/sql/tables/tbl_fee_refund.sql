# DROP TABLE IF EXISTS tbl_fee_refund;

CREATE TABLE tbl_fee_refund
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id    INT                     NOT NULL COMMENT '物流订单',
    order_code  VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号',
    customer_id INT                     NOT NULL COMMENT '会员',
    type_id     INT                     NOT NULL COMMENT '类型#type:FeeRefundType',   # 退款，赔款
    reason_id   INT                     NOT NULL COMMENT '退赔原因',
    reason      VARCHAR(300)            NOT NULL COMMENT '退赔款原因',
    amount      DECIMAL(10, 2)          NOT NULL COMMENT '退赔金额',
    pay_type_id INT                     NOT NULL COMMENT '退赔支付方式#type:FeeRefundPayType',
    status_id   INT                     NOT NULL COMMENT '状态#type:FeeRefundStatus', # 未支付，已支付 等等
    apply_id    INT                     NOT NULL COMMENT '对应申请',
    create_time DATETIME                NOT NULL COMMENT '创建时间',
    update_time DATETIME                NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_fee_refund_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_fee_refund_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_fee_refund_reason_id FOREIGN KEY (reason_id) REFERENCES tbl_fee_refund_reason (id),
    CONSTRAINT fk_fee_refund_apply_id FOREIGN KEY (apply_id) REFERENCES tbl_fee_refund_apply (id),
    INDEX ix_fee_refund_status_id (status_id)
) COMMENT '订单退赔款';
