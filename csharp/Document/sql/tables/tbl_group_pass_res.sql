CREATE TABLE tbl_group_pass_res (
    id          INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    group_id    INT      NOT NULL COMMENT '群组',
    pass_res_id INT      NOT NULL COMMENT '资源',
    create_time DATETIME NOT NULL COMMENT '创建时间',

    FOREIGN KEY fk_group_pass_res_group_id (group_id) REFERENCES tbl_group (id),
    FOREIGN KEY fk_group_pass_res_pass_res_id (pass_res_id) REFERENCES tbl_pass_res (id)
) COMMENT '群组资源授权';