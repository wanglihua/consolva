CREATE TABLE tbl_pass_res
(
    id          INT          NOT NULL PRIMARY KEY COMMENT 'id',
    cate_id     INT          NOT NULL COMMENT '类别',
    name        VARCHAR(100) NOT NULL COMMENT '名称',
    description VARCHAR(300) NULL COMMENT '描述',

    FOREIGN KEY fk_pass_res_cate_id (cate_id) REFERENCES tbl_pass_res_cate (id)
) COMMENT '权限资源';