CREATE TABLE tbl_group
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    code        VARCHAR(50)  NOT NULL COMMENT '编码',
    name        VARCHAR(50)  NOT NULL COMMENT '名称',
    description VARCHAR(200) NULL COMMENT '描述',
    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_group_code (code),
    UNIQUE INDEX ix_group_name (name)
) COMMENT '群组';
