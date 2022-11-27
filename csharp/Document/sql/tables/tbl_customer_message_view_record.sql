# DROP TABLE IF EXISTS tbl_customer_message_view_record;

CREATE TABLE tbl_customer_message_view_record
(
    customer_id          INT      NOT NULL PRIMARY KEY COMMENT '客户',
    last_view_message_id INT      NOT NULL COMMENT '最后查看消息ID',
    last_view_time       DATETIME NOT NULL COMMENT '最后查看时间',

    CONSTRAINT fk_customer_message_view_record_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_customer_message_view_record_last_view_message_id FOREIGN KEY (last_view_message_id) REFERENCES tbl_customer_message (id)
) COMMENT '客户消息查看';