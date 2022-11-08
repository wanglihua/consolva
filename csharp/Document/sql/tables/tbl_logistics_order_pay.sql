# DROP TABLE IF EXISTS tbl_logistics_order_pay;

CREATE TABLE tbl_logistics_order_pay
(
    id                       INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id                 INT            NOT NULL COMMENT '订单',
    customer_id              INT            NOT NULL COMMENT '客户',
    pay_type_id              INT            NOT NULL COMMENT '支付类型',
    out_trade_no             VARCHAR(25)    NOT NULL COMMENT '支付订单编号',
    paypal_payment_id        VARCHAR(50)    NOT NULL COMMENT 'PayPal Payment Id',
    amount                   DECIMAL(10, 2) NOT NULL COMMENT '支付金额',
    user_id                  INT            NULL COMMENT '操作用户',
    status_id                INT            NOT NULL COMMENT '状态',
    write_off_reason_id      INT            NULL COMMENT '核销原因Id',
    write_off_reason_content VARCHAR(300)   NOT NULL COMMENT '核销原因',
    create_time              DATETIME       NOT NULL COMMENT '创建时间',
    update_time              DATETIME       NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_order_pay_pay_type_id (pay_type_id),
    INDEX ix_logistics_order_pay_pay_status_id (status_id),
    INDEX ix_logistics_order_pay_pay_out_trade_no (out_trade_no),
    INDEX ix_logistics_order_pay_payment_id (paypal_payment_id),

    CONSTRAINT fk_logistics_order_pay_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_pay_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_logistics_order_write_off_reason_id FOREIGN KEY (write_off_reason_id) REFERENCES tbl_manual_fee_reason (id)
) COMMENT '物流订单支付';