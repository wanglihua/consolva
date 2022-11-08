# DROP TABLE IF EXISTS tbl_fee_write_off_apply;

CREATE TABLE tbl_fee_write_off_apply
(
    id                       INT                       NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id              INT                       NOT NULL COMMENT '客户',
    order_id                 INT                       NOT NULL COMMENT '订单',
    amount                   DECIMAL(10, 2) DEFAULT 0  NOT NULL COMMENT '金额',
    description              VARCHAR(255)   DEFAULT '' NULL COMMENT '描述',
    write_off_reason_id      INT                       NOT NULL COMMENT '核销原因Id',
    write_off_reason_content VARCHAR(300)              NOT NULL COMMENT '核销原因',
    apply_user_id            INT                       NOT NULL COMMENT '申请用户',
    approve_user_id          INT                       NULL COMMENT '审核用户',
    status_id                INT                       NOT NULL COMMENT '审核状态#type:FeeWriteOffApplyStatus',
    create_time              DATETIME                  NOT NULL COMMENT '创建时间',
    update_time              DATETIME                  NOT NULL COMMENT '更新时间',


    CONSTRAINT fk_fee_write_off_apply_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_fee_write_off_apply_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_fee_write_off_apply_write_off_reason_id FOREIGN KEY (write_off_reason_id) REFERENCES tbl_manual_fee_reason (id),
    INDEX ix_fee_write_off_apply_pay_status_id (status_id)
) COMMENT '订单费用核销申请';

DROP VIEW IF EXISTS view_fee_write_off_apply_count;

CREATE VIEW view_fee_write_off_apply_count AS
SELECT fa.order_id  AS order_id,
       COUNT(fa.id) AS apply_count
FROM tbl_fee_write_off_apply fa
GROUP BY fa.order_id;