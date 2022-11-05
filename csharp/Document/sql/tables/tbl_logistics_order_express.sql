CREATE TABLE tbl_logistics_order_express
(
    id          INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id    INT      NOT NULL COMMENT '订单',
    express_id  INT      NOT NULL COMMENT '快递单号',

    create_time DATETIME NOT NULL COMMENT '创建时间',
    update_time DATETIME NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_logistics_order_express_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_express_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id)
) COMMENT '订单快递单号';

# 订单快递单号，这个表实际上也就作废订单会用到，正常订单用不到的
# 订单物品表系统中也不用再创建了
