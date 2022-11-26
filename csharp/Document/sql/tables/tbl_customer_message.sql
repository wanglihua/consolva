# DROP TABLE IF EXISTS tbl_customer_message;

CREATE TABLE tbl_customer_message
(
    id           INT                      NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    send_type_id INT          DEFAULT 0   NOT NULL COMMENT '消息发送类型#type:CustomerMessageSendType',
    type_id      INT          DEFAULT 0   NOT NULL COMMENT '消息类型#type:CustomerMessageType',
    customer_id  INT                      NOT NULL COMMENT '客户',
    order_id     INT                      NULL COMMENT '物流订单',
    content      VARCHAR(400) DEFAULT ''  NOT NULL COMMENT '消息内容',
    status_id    INT          DEFAULT 100 NOT NULL COMMENT '发送状态#type:CustomerMessageStatus',
    sms_send     BOOL                     NOT NULL COMMENT '是否SMS发送',
    wechat_send  BOOL                     NOT NULL COMMENT '是否微信发送',
    send_time    DATETIME                 NOT NULL COMMENT '发送时间',
    create_time  DATETIME                 NOT NULL COMMENT '创建时间',
    update_time  DATETIME                 NOT NULL COMMENT '更新时间',

    INDEX ix_customer_message_type_id (type_id),
    INDEX ix_customer_message_create_time (create_time),

    CONSTRAINT fk_customer_message_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_customer_message_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id)
) COMMENT '客户消息';