CREATE TABLE tbl_logistics_order_pack_express
(
    id            INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id      INT      NOT NULL COMMENT '订单',
    order_pack_id INT      NOT NULL COMMENT '订单打包',
    express_id    INT      NOT NULL COMMENT '快递单',

    create_time   DATETIME NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_logistics_order_pack_express_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_pack_express_order_pack_id FOREIGN KEY (order_pack_id) REFERENCES tbl_logistics_order_pack (id),
    CONSTRAINT fk_logistics_order_pack_express_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id)
) COMMENT '订单打包快递单号';
