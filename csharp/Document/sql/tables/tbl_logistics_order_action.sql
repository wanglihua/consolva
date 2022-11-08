CREATE TABLE tbl_logistics_order_action
(
    id                     INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id               INT                     NOT NULL COMMENT '订单',
    user_id                INT                     NULL COMMENT '操作人',
    customer_id            INT                     NULL COMMENT '操作客户',
    type_id                INT          DEFAULT 0  NOT NULL COMMENT '操作类型#type:LogisticsOrderActionType',
    order_status_id_before INT          DEFAULT 0  NOT NULL COMMENT '订单状态前#type:LogisticsOrderStatus',
    order_status_id_after  INT          DEFAULT 0  NOT NULL COMMENT '订单状态后#type:LogisticsOrderStatus',
    pay_status_id_before   INT          DEFAULT 0  NOT NULL COMMENT '支付状态前#type:PayStatus',
    pay_status_id_after    INT          DEFAULT 0  NOT NULL COMMENT '支付状态后#type:PayStatus',
    action_desc            VARCHAR(800) DEFAULT '' NOT NULL COMMENT '操作备注',
    create_time            DATETIME                NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_logistics_order_action_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_action_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_logistics_order_action_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '订单操作日志';
