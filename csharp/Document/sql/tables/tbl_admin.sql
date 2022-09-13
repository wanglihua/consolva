CREATE TABLE tbl_admin
(
    id          INT          NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    user_id     INT          NOT NULL COMMENT '用户id',
    login_name  VARCHAR(100) NOT NULL COMMENT '登录名',
    user_name   VARCHAR(100) NOT NULL COMMENT '用户名',
    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间',

    FOREIGN KEY fk_admin_user_id (user_id) REFERENCES tbl_user (id)
) COMMENT '管理员';

INSERT INTO tbl_admin (id, user_id, login_name, user_name, create_time, update_time)
VALUES (1, 1, 'admin', '管理员', NOW(), NOW());