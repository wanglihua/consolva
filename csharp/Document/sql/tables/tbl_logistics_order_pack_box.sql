# DROP TABLE IF EXISTS tbl_logistics_order_pack_box;

CREATE TABLE tbl_logistics_order_pack_box
(
    id            INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',

    order_id      INT      NOT NULL COMMENT '订单',
    order_pack_id INT      NOT NULL COMMENT '订单打包',
    box_num       INT      NOT NULL COMMENT '订单打包箱序',                              # 在整个订单中的序号
    ship_time     DATETIME NULL COMMENT '发货时间',
    ship_user_id  INT      NULL COMMENT '发货人',
    deliver_time  DATETIME NULL COMMENT '交付时间',
    status_id     INT      NOT NULL COMMENT '状态#type:LogisticsOrderPackBoxStatus', # 已打包，已发货。已交付 等等

    create_time   DATETIME NOT NULL COMMENT '创建时间',
    update_time   DATETIME NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_logistics_order_pack_box_oi_bn (order_id, box_num),

    CONSTRAINT fk_logistics_order_pack_box_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_pack_box_order_pack_id FOREIGN KEY (order_pack_id) REFERENCES tbl_logistics_order_pack (id),
    CONSTRAINT fk_logistics_order_pack_box_ship_user_id FOREIGN KEY (ship_user_id) REFERENCES tbl_user (id)
) COMMENT '订单打包箱号';
