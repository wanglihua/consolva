CREATE TABLE tbl_express_expect
(
    id               INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    tracking_no      VARCHAR(30)  NOT NULL COMMENT '快递单号#query',
    description      VARCHAR(500) NOT NULL COMMENT '备注',
    customer_id      INT          NOT NULL COMMENT '客户#ref:tbl_customer#query',
    last_edit_time   DATETIME     NOT NULL COMMENT '客户最后编辑时间',
    status_id        INT          NOT NULL COMMENT '状态#type:ExpressExpectStatus#query',
    overdue_msg_sent DATETIME     NULL COMMENT '超期未入库消息发送时间', # 这个字段可用于标记超期未入库消息是否已发送，也可以控制发送频率

    create_time      DATETIME     NOT NULL COMMENT '创建时间',
    update_time      DATETIME     NOT NULL COMMENT '更新时间',    # 这个更新时间可能不是客户的最后更新时间

    CONSTRAINT fk_express_expect_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    INDEX ix_express_expect_tracking_no (tracking_no),
    INDEX ix_express_expect_last_edit_time (last_edit_time)
) COMMENT '快递单号预报';
