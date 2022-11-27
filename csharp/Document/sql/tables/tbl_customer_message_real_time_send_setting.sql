# DROP TABLE IF EXISTS tbl_customer_message_real_time_send_setting;

CREATE TABLE tbl_customer_message_real_time_send_setting
(
    id                       INT                        NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id                  INT                        NOT NULL COMMENT '消息类型#type:CustomerMessageType',
    wechat_message_template1 VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '微信消息模板1',
    wechat_message_template2 VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '微信消息模板2',
    wechat_message_template3 VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '微信消息模板3',
    sms_message_template     VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '短信消息模板',
    status_id                INT                        NOT NULL COMMENT '启用状态#type:CustomerMessageSettingStatus',
    sms_send                 BOOL                       NOT NULL COMMENT '是否SMS发送',
    wechat_send              BOOL                       NOT NULL COMMENT '是否微信发送',
    placeholders             VARCHAR(300) DEFAULT ''    NOT NULL COMMENT '占位符',
    client_customizable      BOOL         DEFAULT FALSE NOT NULL COMMENT '客户端可定制',
    client_title             VARCHAR(100) DEFAULT ''    NOT NULL COMMENT '客户端标题',
    client_desc              VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '客户端描述',

    create_time              DATETIME                   NOT NULL COMMENT '创建时间',
    update_time              DATETIME                   NOT NULL COMMENT '更新时间',

    INDEX ix_customer_message_real_time_send_setting_type_id (type_id)
) COMMENT '实时任务推送设置';