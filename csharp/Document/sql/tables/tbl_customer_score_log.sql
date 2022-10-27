CREATE TABLE tbl_customer_score_log
(
    id                          INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id                 INT            NOT NULL COMMENT '客户id', # 积分所有者客户
    score_type_id               INT            NOT NULL COMMENT '积分变动类型#type:CustomerScoreType',
    score                       DECIMAL(10, 2) NOT NULL COMMENT '变动积分',
    amount                      DECIMAL(10, 2) NOT NULL COMMENT '金额',
    is_valid_score              BOOL           NOT NULL COMMENT '是否有效积分',
    description                 VARCHAR(300)   NOT NULL COMMENT '变动描述',
    logistics_order_pay_id      INT            NULL COMMENT '订单支付',
    logistics_order_id          INT            NULL COMMENT '集运订单Id',
    logistics_order_code        VARCHAR(100)   NULL COMMENT '集运订单编号',
    logistics_order_customer_id INT            NULL COMMENT '集运订单客户',

    create_time                 DATETIME       NOT NULL COMMENT '创建时间',
    update_time                 DATETIME       NOT NULL COMMENT '更新时间',

    INDEX ix_customer_score_log_score_type_id (score_type_id),
    CONSTRAINT fk_customer_score_log_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_customer_score_log_logistics_order_pay_id FOREIGN KEY (logistics_order_pay_id) REFERENCES tbl_logistics_order_pay (id),
    CONSTRAINT fk_customer_score_log_logistics_order_id FOREIGN KEY (logistics_order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_customer_score_log_logistics_order_customer_id FOREIGN KEY (logistics_order_customer_id) REFERENCES tbl_customer (id)
) COMMENT '客户积分日志';
