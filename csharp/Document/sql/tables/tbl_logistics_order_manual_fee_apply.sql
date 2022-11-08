# DROP TABLE IF EXISTS tbl_logistics_order_manual_fee_apply;

CREATE TABLE tbl_logistics_order_manual_fee_apply
(
    id                    INT                         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id           INT                         NOT NULL COMMENT ' 客户',
    order_id              INT                         NOT NULL COMMENT '订单',
    type_id               INT            DEFAULT 0    NOT NULL COMMENT '费用类型#type:LogisticsOrderFeeType', # 用这个字段来区分是增加还是减少
    fee                   DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '金额',
    description           VARCHAR(255)   DEFAULT ''   NOT NULL COMMENT '描述',
    manual_reason_id      INT                         NOT NULL COMMENT '增减核销费用原因id',
    manual_reason_content VARCHAR(300)                NOT NULL COMMENT '增减核销费用原因content',
    apply_user_id         INT                         NOT NULL COMMENT '申请用户',
    approve_user_id       INT                         NULL COMMENT '审核用户',
    status_id             INT                         NOT NULL COMMENT '审核状态#type:LogisticsOrderManualFeeApplyStatus',
    create_time           DATETIME                    NOT NULL COMMENT '创建时间',
    update_time           DATETIME                    NOT NULL COMMENT '更新时间',

    FOREIGN KEY fk_logistics_order_manual_fee_apply_customer_id (customer_id) REFERENCES tbl_customer (id),
    FOREIGN KEY fk_logistics_order_manual_fee_apply_order_id (order_id) REFERENCES tbl_logistics_order (id),
    FOREIGN KEY fk_logistics_order_manual_fee_apply_manual_reason_id (manual_reason_id) REFERENCES tbl_manual_fee_reason (id),
    FOREIGN KEY fk_logistics_order_manual_fee_apply_apply_user_id (apply_user_id) REFERENCES tbl_user (id),
    FOREIGN KEY fk_logistics_order_manual_fee_apply_manual_approve_user_id (approve_user_id) REFERENCES tbl_user (id),
    INDEX ix_logistics_order_manual_fee_apply_status_id (status_id)

) COMMENT '订单手工费用申请';


DROP VIEW IF EXISTS view_logistics_order_manual_fee_apply_count;

CREATE VIEW view_logistics_order_manual_fee_apply_count AS
SELECT fa.order_id  AS order_id,
       COUNT(fa.id) AS apply_count
FROM tbl_logistics_order_manual_fee_apply fa
GROUP BY fa.order_id;