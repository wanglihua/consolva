CREATE TABLE tbl_express_log
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id     INT                     NOT NULL COMMENT '类型#type:ExpressLogType',
    express_id  INT                     NOT NULL COMMENT '快递单',
    order_id    INT                     NULL COMMENT '物流订单', # 记录日志时快递单有关联的订单，就记录这个订单
    description VARCHAR(300) DEFAULT '' NOT NULL COMMENT '描述',
    customer_id INT                     NULL COMMENT '操作会员',
    user_id     INT                     NULL COMMENT '操作用户',
    create_time DATETIME                NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_express_log_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_log_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_log_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_express_log_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id)
) COMMENT '快递单日志';
