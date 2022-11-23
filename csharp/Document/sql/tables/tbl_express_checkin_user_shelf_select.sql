CREATE TABLE tbl_express_checkin_user_shelf_select
(
    id                  INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',

    user_id             INT      NOT NULL COMMENT '用户',
    shelf_id            INT      NOT NULL COMMENT '货架',
    storage_loc_spec_id INT      NOT NULL COMMENT '仓位规格',

    create_time         DATETIME NOT NULL COMMENT '创建时间',
    update_time         DATETIME NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_express_checkin_user_shelf_select_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_express_checkin_user_shelf_select_shelf_id FOREIGN KEY (shelf_id) REFERENCES tbl_shelf (id),
    CONSTRAINT fk_express_checkin_user_shelf_select_storage_loc_spec_id FOREIGN KEY (storage_loc_spec_id) REFERENCES tbl_storage_loc_spec (id)

) COMMENT '快递单号入库用户货架选择记忆';
