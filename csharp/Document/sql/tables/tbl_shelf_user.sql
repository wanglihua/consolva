# DROP TABLE IF EXISTS tbl_shelf_user;

CREATE TABLE tbl_shelf_user
(
    id           INT      NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    user_id      INT      NOT NULL COMMENT '用户',
    warehouse_id INT      NOT NULL COMMENT '仓库',
    shelf_id     INT      NOT NULL COMMENT '货架',
    type_id      INT      NOT NULL COMMENT '类型#type:ShelfUserType',

    create_time  DATETIME NOT NULL COMMENT '创建时间',
    update_time  DATETIME NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_shelf_user_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_shelf_user_warehouse_id FOREIGN KEY (warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT fk_shelf_user_shelf_id FOREIGN KEY (shelf_id) REFERENCES tbl_shelf (id)
) COMMENT '货架用户关联';
