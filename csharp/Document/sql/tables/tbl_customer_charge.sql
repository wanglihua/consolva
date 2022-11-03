CREATE TABLE tbl_customer_charge
(
    id                   INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    charge_type_id       INT            NOT NULL COMMENT '充值方式#type:CustomerChargeType',
    customer_id          INT            NOT NULL DEFAULT 0 COMMENT '客户id',
    amount               DECIMAL(10, 2) NOT NULL DEFAULT 0.00 COMMENT '充值金额',
    award                DECIMAL(10, 2) NOT NULL DEFAULT 0.00 COMMENT '奖励金额',
    out_trade_no         VARCHAR(50)    NULL COMMENT '支付订单编号',
    paypal_payment_id    VARCHAR(50)    NULL COMMENT 'PayPal payment id',
    manual_charge_reason VARCHAR(300)   NULL COMMENT '手动充值原因',
    user_id              INT            NULL COMMENT '操作用户',
    status_id            INT            NOT NULL COMMENT '充值状态#type:CustomerChargeStatus',
    create_time          DATETIME       NOT NULL COMMENT '创建时间',
    update_time          DATETIME       NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_customer_charge_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_customer_charge_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),
    INDEX ix_customer_charge_out_trade_no (out_trade_no),
    INDEX ix_customer_charge_paypal_payment_id (paypal_payment_id)
) COMMENT '客户账户充值';
