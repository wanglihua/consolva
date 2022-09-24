CREATE TABLE tbl_storage_loc_spec
(
    id            INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    shelf_type_id INT          NOT NULL COMMENT '货架类型#type:ShelfType',
    code          VARCHAR(50)  NOT NULL COMMENT '编码',
    name          VARCHAR(255) NOT NULL COMMENT '名称',
    length        INT          NOT NULL COMMENT '长/cm',
    width         INT          NOT NULL COMMENT '宽/cm',
    height        INT          NOT NULL COMMENT '高/cm',
    color         VARCHAR(10)  NOT NULL COMMENT '颜色',

    create_time   DATETIME     NOT NULL COMMENT '创建时间',
    update_time   DATETIME     NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_storage_loc_spec_type_id_code (shelf_type_id, code)
) COMMENT '库位规格';
