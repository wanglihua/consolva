CREATE TABLE tbl_logistics_mode_goods_type
(
    id                INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    logistics_mode_id INT      NOT NULL COMMENT '物流方式',
    goods_type_id     INT      NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',

    create_time       DATETIME NOT NULL COMMENT '创建时间',
    update_time       DATETIME NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_logistics_mode_logistics_mode_id FOREIGN KEY (logistics_mode_id) REFERENCES tbl_logistics_mode (id)
) COMMENT '物流方式物品类型';