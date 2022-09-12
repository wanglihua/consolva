CREATE TABLE tbl_user_login_log
(
    id         INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    user_id    INT          NOT NULL COMMENT '登录用户',
    ip_address VARCHAR(200) NOT NULL COMMENT 'IP地址',
    user_agent VARCHAR(300) NOT NULL COMMENT 'User Agent',
    login_time DATETIME     NOT NULL COMMENT '登录时间',

    INDEX ix_user_login_log_user_id (user_id)
) COMMENT '用户登录日志';
