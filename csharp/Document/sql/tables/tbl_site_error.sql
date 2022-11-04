# DROP TABLE IF EXISTS tbl_site_error;

CREATE TABLE tbl_site_error
(
    id             INT                        NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    error_type_id  INT                        NOT NULL COMMENT '错误类型#type:SiteErrorType',
    error_sub_type VARCHAR(100) DEFAULT ''    NOT NULL COMMENT '错误子类型',
    error_msg      VARCHAR(500) DEFAULT ''    NOT NULL COMMENT '错误信息',
    resolved       BOOL         DEFAULT FALSE NOT NULL COMMENT '是否已解决',

    create_time    DATETIME                   NOT NULL COMMENT '创建时间',
    update_time    DATETIME                   NOT NULL COMMENT '更新时间',

    INDEX ix_site_error_error_type_id (error_type_id),
    INDEX ix_site_error_error_sub_type (error_sub_type)
) COMMENT '网站错误记录';
