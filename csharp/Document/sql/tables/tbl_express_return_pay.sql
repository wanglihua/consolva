# DROP TABLE IF EXISTS tbl_express_return_pay;

CREATE TABLE tbl_express_return_pay
(
    id                INT                    NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    express_id        INT                    NOT NULL COMMENT '快递单',
    customer_id       INT                    NOT NULL COMMENT '客户',
    pay_type_id       INT                    NOT NULL COMMENT '支付类型#type:PayType',
    out_trade_no      VARCHAR(50) DEFAULT '' NOT NULL COMMENT '支付订单编号',
    paypal_payment_id VARCHAR(50) DEFAULT '' NOT NULL COMMENT 'PayPal Payment Id',
    amount            DECIMAL(10, 2)         NOT NULL COMMENT '支付金额',
    status_id         INT                    NOT NULL COMMENT '状态#type:PayStatus',

    create_time       DATETIME               NOT NULL COMMENT '创建时间',
    update_time       DATETIME               NOT NULL COMMENT '更新时间',

    INDEX ix_express_return_pay_pay_out_trade_no (out_trade_no),
    INDEX ix_express_return_pay_payment_id (paypal_payment_id),

    CONSTRAINT fk_express_return_pay_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_return_pay_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '快递单退运支付';
