DROP TABLE tbl_logistics_order_first_mode;

CREATE TABLE tbl_logistics_order_first_mode
(
    id            INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    order_id      INT                     NOT NULL COMMENT '物流订单',
    order_code    VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号',
    goods_type_id INT                     NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',
    first_type_id INT                     NOT NULL COMMENT '物流方式类型#type:LogisticsModeType',
    first_mode_id INT                     NOT NULL COMMENT '物流方式',
    channel_id    INT                     NOT NULL COMMENT '渠道类型#type:LogisticsModeChannel',

    create_time   DATETIME                NOT NULL COMMENT '创建时间',
    update_time   DATETIME                NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_logistics_order_first_mode_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_first_mode_first_mode_id FOREIGN KEY (first_mode_id) REFERENCES tbl_logistics_mode (id)
) COMMENT '物流订单头程';


# 这里可能后续会继续添加各种字段，用以替代原来国际单号的那个表
# 还是不用替换，利用 channel 的这个 type 进行链接