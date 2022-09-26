CREATE TABLE tbl_pass_res_cate
(
    id          INT           NOT NULL PRIMARY KEY COMMENT 'id',
    name        VARCHAR(100)  NOT NULL COMMENT '名称',
    description VARCHAR(300)  NULL COMMENT '描述',
    order_num   INT DEFAULT 0 NOT NULL COMMENT '序号',

    INDEX ix_pass_res_cate_name (name),
    INDEX ix_pass_res_cate_order_num (order_num)
) COMMENT '资源类别';
