# DROP TABLE IF EXISTS tbl_customer_message_client_customize;

CREATE TABLE tbl_customer_message_client_customize
(
    id          INT      NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id INT      NOT NULL COMMENT '客户',
    type_id     INT      NOT NULL COMMENT '消息类型',
    send        BOOL     NOT NULL COMMENT '消息是否发送',
    create_time DATETIME NOT NULL COMMENT '创建时间',
    update_time DATETIME NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_customer_message_client_customize_customer_type (customer_id, type_id)

) COMMENT '客户消息定制';