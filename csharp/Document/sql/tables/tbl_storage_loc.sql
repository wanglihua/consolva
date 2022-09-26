CREATE TABLE tbl_storage_loc
(
    id                  INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    name                VARCHAR(255) NOT NULL COMMENT '名称',
    row_num             INT          NOT NULL COMMENT '层',
    col_num             INT          NOT NULL COMMENT '列',
    spec_id             INT          NOT NULL COMMENT '规格',
    shelf_id            INT          NOT NULL COMMENT '货架',
    status_id           INT          NOT NULL COMMENT '状态',
    order_num           INT          NOT NULL COMMENT '排序',
    off_shelf_order_num INT          NOT NULL COMMENT '下架排序',

    create_time         DATETIME     NOT NULL COMMENT '创建时间',
    update_time         DATETIME     NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_storage_loc_spec_id FOREIGN KEY (spec_id) REFERENCES tbl_storage_loc_spec (id),
    CONSTRAINT fk_storage_loc_shelf_id FOREIGN KEY (shelf_id) REFERENCES tbl_shelf (id),
    # INDEX ix_storage_loc_row_num (row_num),
    # INDEX ix_storage_loc_col_num (col_num),
    INDEX ix_storage_loc_order_num (order_num),
    INDEX ix_storage_loc_off_shelf_order_num (off_shelf_order_num)
) COMMENT '库位';
