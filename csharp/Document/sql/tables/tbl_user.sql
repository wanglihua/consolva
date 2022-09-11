CREATE TABLE tbl_user
(
    id              INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    login_name      VARCHAR(100) NOT NULL COMMENT '登录名',
    user_name       VARCHAR(100) NOT NULL COMMENT '用户名',
    password        VARCHAR(100) NOT NULL COMMENT '密码',
    last_login_time DATETIME     NULL COMMENT '登录时间',
    status_id       INT          NOT NULL DEFAULT 100 COMMENT '状态',
    
    create_time     DATETIME     NOT NULL COMMENT '创建时间',
    update_time     DATETIME     NOT NULL COMMENT '更新时间',
    version         INT          NOT NULL DEFAULT 1 COMMENT '数据版本',

    UNIQUE INDEX ix_user_login_name (login_name)
) COMMENT '用户';


INSERT INTO tbl_user (id, login_name, user_name, password, last_login_time, create_time, update_time, version)
VALUES (1, 'admin', '管理员', 'ee93178f56674047999ee057267560ce', '2022-01-01 00:00:00', '2022-01-01 00:00:00', '2022-01-01 00:00:00', 1);
