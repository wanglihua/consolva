CREATE TABLE tbl_pass_res
(
    id          INT           NOT NULL PRIMARY KEY COMMENT 'id',
    cate_id     INT           NOT NULL COMMENT '类别',
    name        VARCHAR(100)  NOT NULL COMMENT '名称',
    description VARCHAR(300)  NULL COMMENT '描述',
    order_num   INT DEFAULT 0 NOT NULL COMMENT '序号',

    CONSTRAINT fk_pass_res_cate_id FOREIGN KEY (cate_id) REFERENCES tbl_pass_res_cate (id),
    INDEX ix_pass_res_order_num (order_num)
) COMMENT '权限资源';
