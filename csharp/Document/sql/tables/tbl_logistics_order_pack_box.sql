# DROP TABLE IF EXISTS tbl_logistics_order_pack_box;

CREATE TABLE tbl_logistics_order_pack_box
(
    id            INT AUTO_INCREMENT COMMENT 'Id' PRIMARY KEY,

    order_id      INT      NOT NULL COMMENT '订单',
    order_pack_id INT      NOT NULL COMMENT '订单打包',
    box_num       INT      NOT NULL COMMENT '订单打包箱序',                              # 在整个订单中的序号
    ship_time     DATETIME NULL COMMENT '发货时间',
    deliver_time  DATETIME NULL COMMENT '交付时间',
    status_id     INT      NOT NULL COMMENT '状态#type:LogisticsOrderPackBoxStatus', # 已打包，已发货。已交付 等等

    create_time   DATETIME NOT NULL COMMENT '创建时间',
    update_time   DATETIME NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_logistics_order_pack_box_oi_bn (order_id, box_num),

    FOREIGN KEY fk_logistics_order_pack_box_order_id (order_id) REFERENCES tbl_logistics_order (id),
    FOREIGN KEY fk_logistics_order_pack_box_order_pack_id (order_pack_id) REFERENCES tbl_logistics_order_pack (id)
) COMMENT '订单打包箱号';
