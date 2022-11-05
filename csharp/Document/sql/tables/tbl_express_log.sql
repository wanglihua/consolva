# DROP TABLE IF EXISTS tbl_express_log;

CREATE TABLE tbl_express_log
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id     INT                     NOT NULL COMMENT '类型#type:ExpressLogType',
    express_id  INT                     NULL COMMENT '快递单',      # 这个是可空的，尺寸上传，签收图片上传时，都还没快递单号数据，如果入库后把日志中的这个数据填上后面查询时，就方便好多
    tracking_no VARCHAR(30)             NOT NULL COMMENT '快递单号', # 这个字段非空
    order_id    INT                     NULL COMMENT '物流订单',     # 记录日志时快递单有关联的订单，就记录这个订单
    description VARCHAR(300) DEFAULT '' NOT NULL COMMENT '描述',
    customer_id INT                     NULL COMMENT '操作会员',
    user_id     INT                     NULL COMMENT '操作用户',
    create_time DATETIME                NOT NULL COMMENT '创建时间',

    INDEX ix_express_log_tracking_no (tracking_no),
    CONSTRAINT fk_express_log_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_log_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_log_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_express_log_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id)
) COMMENT '快递单日志';
