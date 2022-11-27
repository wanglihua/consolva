# DROP TABLE IF EXISTS tbl_customer_message_cycle_timing_send_record;

CREATE TABLE tbl_customer_message_cycle_timing_send_record
(
    id                INT      NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id           INT      NOT NULL COMMENT '消息类型#type:CustomerMessageType',
    setting_status_id INT      NOT NULL COMMENT '启用状态#type:CustomerMessageSettingStatus',
    sms_send          BOOL     NOT NULL COMMENT '是否SMS发送',
    wechat_send       BOOL     NOT NULL COMMENT '是否微信发送',
    create_time       DATETIME NOT NULL COMMENT '创建时间',

    INDEX ix_customer_message_cycle_timing_send_record_type_id (type_id),
    INDEX ix_customer_message_cycle_timing_send_record_create_time (create_time)
) COMMENT '定时任务推送记录';
