DROP TABLE IF EXISTS tbl_customer_message;

CREATE TABLE tbl_customer_message
(
    id           INT          NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    send_type_id INT          NOT NULL COMMENT '消息发送类型',
    type_id      INT          NOT NULL COMMENT '消息类型',
    customer_id  INT          NOT NULL COMMENT '客户',
    content      VARCHAR(400) NOT NULL COMMENT '消息内容',
    status_id    INT          NOT NULL DEFAULT 100 COMMENT '发送状态',
    sms_send     BOOL         NOT NULL COMMENT '是否SMS发送',
    wei_xin_send BOOL         NOT NULL COMMENT '是否微信发送',
    send_time    DATETIME     NOT NULL COMMENT '发送时间',
    create_time  DATETIME     NOT NULL COMMENT '创建时间',
    update_time  DATETIME     NOT NULL COMMENT '更新时间',
    version      INT          NOT NULL DEFAULT 1 COMMENT '数据版本',

    INDEX ix_customer_message_type_id (type_id),
    INDEX ix_customer_message_create_time (create_time),

    CONSTRAINT fk_customer_message_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)

) COMMENT '客户消息';

ALTER TABLE tbl_customer_message
    ADD order_id INT NULL COMMENT '物流订单' AFTER customer_id;

ALTER TABLE tbl_customer_message
    ADD CONSTRAINT fk_customer_message_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id);
