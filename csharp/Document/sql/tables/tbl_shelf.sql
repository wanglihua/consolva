CREATE TABLE tbl_shelf
(
    id           INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    code         VARCHAR(50)  NOT NULL COMMENT '编码',
    name         VARCHAR(255) NOT NULL COMMENT '货架名',
    type_id      INT          NOT NULL COMMENT '货架类型#type:ShelfType',
    cate_id      INT          NOT NULL COMMENT '货架类别#type:ShelfCate',
    status_id    INT          NOT NULL COMMENT '状态#type:ShelfStatus',
    order_num    INT          NOT NULL COMMENT '排序',
    warehouse_id INT          NOT NULL COMMENT '仓库',
    create_time  DATETIME     NOT NULL COMMENT '创建时间',
    update_time  DATETIME     NOT NULL COMMENT '更新时间',

    FOREIGN KEY fk_shelf_warehouse_id (warehouse_id) REFERENCES tbl_warehouse (id),
    INDEX ix_shelf_order_num (order_num)
) COMMENT = '货架';
