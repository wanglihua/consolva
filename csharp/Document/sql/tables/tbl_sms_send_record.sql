CREATE TABLE tbl_sms_send_record
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    mobile      VARCHAR(20)  NOT NULL COMMENT '手机号',
    message     VARCHAR(400) NOT NULL COMMENT '消息内容',
    send_msg_id VARCHAR(30)  NOT NULL COMMENT '消息ID',
    send_code   VARCHAR(10)  NOT NULL COMMENT '返回代码',
    send_error  VARCHAR(300) NOT NULL COMMENT '返回错误',

    create_time DATETIME     NOT NULL COMMENT '创建时间',

    INDEX ix_sms_send_record_mobile (mobile)
) COMMENT '短信发送记录';