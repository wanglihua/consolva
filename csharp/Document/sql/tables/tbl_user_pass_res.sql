CREATE TABLE tbl_user_pass_res (
    id          INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    user_id     INT      NOT NULL COMMENT '用户',
    pass_res_id INT      NOT NULL COMMENT '资源',
    create_time DATETIME NOT NULL COMMENT '创建时间',

    FOREIGN KEY fk_user_pass_res_user_id (user_id) REFERENCES tbl_user (id),
    FOREIGN KEY fk_user_pass_res_pass_res_id (pass_res_id) REFERENCES tbl_pass_res (id)
) COMMENT '用户资源授权';