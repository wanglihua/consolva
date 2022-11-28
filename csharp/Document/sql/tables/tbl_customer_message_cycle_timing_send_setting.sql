# DROP TABLE IF EXISTS tbl_customer_message_cycle_timing_send_setting;

CREATE TABLE tbl_customer_message_cycle_timing_send_setting
(
    id                       INT                        NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id                  INT          DEFAULT 0     NOT NULL COMMENT '消息类型#type:CustomerMessageType',
    wechat_message_template1 VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '微信消息模板1',
    wechat_message_template2 VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '微信消息模板2',
    wechat_message_template3 VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '微信消息模板3',
    sms_message_template     VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '短信消息模板',
    status_id                INT          DEFAULT 0     NOT NULL COMMENT '启用状态#type:CustomerMessageSettingStatus',
    sms_send                 BOOL                       NOT NULL COMMENT '是否SMS发送',
    wechat_send              BOOL                       NOT NULL COMMENT '是否微信发送',
    placeholders             VARCHAR(300) DEFAULT ''    NOT NULL COMMENT '占位符',
    interval_type_id         INT          DEFAULT 0     NOT NULL COMMENT '周期类型#type:CustomerMessageCycleTimingSendIntervalType', # 按周按天
    interval_weeks           INT          DEFAULT 1     NOT NULL COMMENT '发送周期（周）',
    interval_days            INT          DEFAULT 1     NOT NULL COMMENT '发送周期（天）',
    send_weekday             INT          DEFAULT 1     NOT NULL COMMENT '星期几发送',
    send_hour                INT          DEFAULT 19    NOT NULL COMMENT '发送时间',
    client_customizable      BOOL         DEFAULT FALSE NOT NULL COMMENT '客户端可定制',
    client_title             VARCHAR(100) DEFAULT ''    NOT NULL COMMENT '客户端标题',
    client_desc              VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '客户端描述',
    create_time              DATETIME                   NOT NULL COMMENT '创建时间',
    update_time              DATETIME                   NOT NULL COMMENT '更新时间',

    INDEX ix_customer_message_cycle_timing_send_setting_type_id (type_id)
) COMMENT '定时任务推送设置';
