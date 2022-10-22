CREATE TABLE tbl_customer_character
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    content     VARCHAR(300) NOT NULL COMMENT '内容',

    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间'
) COMMENT '客户性质';
