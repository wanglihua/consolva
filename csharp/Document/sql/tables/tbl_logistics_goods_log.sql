CREATE TABLE tbl_logistics_goods_log
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id     INT          DEFAULT 0  NOT NULL COMMENT '操作类型#type:LogisticsGoodsLogType',
    goods_id    INT                     NOT NULL COMMENT '物品',
    user_id     INT                     NOT NULL COMMENT '操作人',
    description VARCHAR(800) DEFAULT '' NOT NULL COMMENT '操作备注',

    create_time DATETIME                NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_logistics_goods_log_goods_id FOREIGN KEY (goods_id) REFERENCES tbl_logistics_goods (id),
    CONSTRAINT fk_logistics_goods_log_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),
    INDEX ix_logistics_goods_log_type_id (type_id)
) COMMENT '物品操作日志';