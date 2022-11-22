# DROP TABLE tbl_warehouse_worker;

CREATE TABLE tbl_warehouse_worker
(
    id           INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id      INT          NOT NULL COMMENT '工作人员类型#type:WarehouseWorkerType',
    warehouse_id INT          NOT NULL COMMENT '仓库',
    user_id      INT          NOT NULL COMMENT '用户',
    description  VARCHAR(300) NOT NULL COMMENT '描述',

    create_time  DATETIME     NOT NULL COMMENT '创建时间',
    update_time  DATETIME     NOT NULL COMMENT '更新时间',

    INDEX ix_warehouse_worker_type_id (type_id),
    CONSTRAINT fk_warehouse_worker_warehouse_id FOREIGN KEY (warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT fk_warehouse_worker_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id)
) COMMENT '仓库工作人员';
