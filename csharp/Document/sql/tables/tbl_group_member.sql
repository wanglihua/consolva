CREATE TABLE tbl_group_member (
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    group_id    INT          NOT NULL COMMENT '群组',
    user_id     INT          NOT NULL COMMENT '用户',
    create_time DATETIME     NOT NULL COMMENT '创建时间',

    FOREIGN KEY fk_group_member_group_id (group_id) REFERENCES tbl_group (id),
    FOREIGN KEY fk_group_member_user_id (user_id) REFERENCES tbl_user (id)
) COMMENT '群组成员';
