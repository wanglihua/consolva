DROP TABLE IF EXISTS tbl_fee_refund_apply;

CREATE TABLE tbl_fee_refund_apply
(
    id              INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id        INT                     NOT NULL COMMENT '物流订单',
    order_code      VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号',
    customer_id     INT                     NOT NULL COMMENT '会员',
    type_id         INT                     NOT NULL COMMENT '类型',                           # 退款，赔款
    reason_id       INT                     NOT NULL COMMENT '退赔原因#type:FeeRefundType',
    reason          VARCHAR(300) DEFAULT '' NOT NULL COMMENT '退赔款原因',
    amount          DECIMAL(10, 2)          NOT NULL COMMENT '退赔金额',
    pay_type_id     INT                     NOT NULL COMMENT '退赔支付方式#type:FeeRefundPayType',
    status_id       INT                     NOT NULL COMMENT '状态#type:FeeRefundApplyStatus', # 未提交，已回退，已提交，已审核 等等
    apply_user_id   INT                     NOT NULL COMMENT '申请人',
    apply_time      DATETIME                NULL COMMENT '申请时间',                             # 就是提交时间吧
    apply_memo      VARCHAR(300) DEFAULT '' NOT NULL COMMENT '申请备注',
    approve_user_id INT                     NULL COMMENT '审核人',
    approve_time    DATETIME                NULL COMMENT '审核时间',
    approve_memo    VARCHAR(300) DEFAULT '' NOT NULL COMMENT '审核备注',
    create_time     DATETIME                NOT NULL COMMENT '创建时间',
    update_time     DATETIME                NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_fee_refund_apply_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_fee_refund_apply_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_fee_refund_apply_reason_id FOREIGN KEY (reason_Id) REFERENCES tbl_fee_refund_reason (id),
    CONSTRAINT fk_fee_refund_apply_apply_user_id FOREIGN KEY (apply_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_fee_refund_apply_approve_user_id FOREIGN KEY (approve_user_id) REFERENCES tbl_user (id),
    INDEX ix_fee_refund_apply_type_id (type_id),
    INDEX ix_fee_refund_apply_pay_type_id (pay_type_id),
    INDEX ix_fee_refund_apply_status_id (status_id)
) COMMENT '订单退赔款申请';


DROP VIEW IF EXISTS view_fee_refund_apply_count;

CREATE VIEW view_fee_refund_apply_count AS
SELECT ra.order_id  AS order_id,
       COUNT(ra.id) AS apply_count
FROM tbl_fee_refund_apply ra
GROUP BY ra.order_id;