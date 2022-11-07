CREATE TABLE tbl_logistics_order_fee
(
    id                    INT                         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id           INT                         NOT NULL COMMENT ' 客户',
    order_id              INT                         NOT NULL COMMENT '订单',
    type_id               INT            DEFAULT 0    NOT NULL COMMENT '费用类型#type:LogisticsOrderFeeType',
    fee                   DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '金额',
    description           VARCHAR(255)   DEFAULT ''   NOT NULL COMMENT '描述',
    manual_reason_id      INT                         NULL COMMENT '增减核销费用原因id',
    manual_reason_content VARCHAR(300)   DEFAULT ''   NOT NULL COMMENT '增减核销费用原因content',
    manual_user_id        INT                         NULL COMMENT '手动增减费用操作人',
    create_time           DATETIME                    NOT NULL COMMENT '创建时间',
    update_time           DATETIME                    NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_logistics_order_fee_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_logistics_order_fee_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_fee_manual_reason_id FOREIGN KEY (manual_reason_id) REFERENCES tbl_manual_fee_reason (id),
    CONSTRAINT fk_logistics_order_fee_manual_user_id FOREIGN KEY (manual_user_id) REFERENCES tbl_user (id),
    
    INDEX ix_logistics_order_fee_type_id (type_id)
) COMMENT '订单费用';




