CREATE TABLE tbl_express_goods
(
    id          INT                       NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    express_id  INT                       NOT NULL COMMENT '快递单',
    goods_id    INT                       NOT NULL COMMENT '物品扩展名',
    price       DECIMAL(10, 2) DEFAULT 0  NOT NULL COMMENT '价格',
    quantity    INT                       NOT NULL COMMENT '数量',
    unit        VARCHAR(8)     DEFAULT '' NOT NULL COMMENT '数量单位',

    create_time DATETIME                  NOT NULL COMMENT '创建时间',
    update_time DATETIME                  NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_express_goods_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_goods_goods_id FOREIGN KEY (goods_id) REFERENCES tbl_logistics_goods (id)
) COMMENT '快递单物品明细';
