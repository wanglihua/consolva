CREATE TABLE tbl_warehouse
(
    id                  INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    code                VARCHAR(100) NOT NULL COMMENT '编码',
    name                VARCHAR(200) NOT NULL COMMENT '名称',
    description         VARCHAR(500) NOT NULL COMMENT '描述',
    address             VARCHAR(500) NOT NULL COMMENT '地址',
    address_description TEXT         NOT NULL COMMENT '地址描述',
    outer_status_id     INT          NOT NULL COMMENT '对外状态',
    inner_status_id     INT          NOT NULL COMMENT '对内状态',
    order_num           INT          NOT NULL COMMENT '排序字段',
    create_time         DATETIME     NOT NULL COMMENT '创建时间',
    update_time         DATETIME     NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_warehouse_code (code),
    UNIQUE INDEX ix_warehouse_name (name),
    INDEX ix_warehouse_outer_status_id (outer_status_id),
    INDEX ix_warehouse_inner_status_id (inner_status_id),
    INDEX ix_warehouse_order_num (order_num)
) COMMENT '仓库';
