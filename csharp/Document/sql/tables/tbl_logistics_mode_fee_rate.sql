CREATE TABLE tbl_logistics_mode_fee_rate
(
    id            INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    mode_id       INT      NOT NULL COMMENT '物流方式',
    goods_type_id INT      NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',
    fee_rate_id   INT      NOT NULL COMMENT '通用费率',
    create_time   DATETIME NOT NULL COMMENT '创建时间',
    update_time   DATETIME NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_logistics_mode_fee_rate_mode_id_goods_type_id (mode_id, goods_type_id),
    CONSTRAINT fk_logistics_mode_fee_rate_model_id FOREIGN KEY (mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT fk_logistics_mode_fee_rate_fee_rate_id FOREIGN KEY (fee_rate_id) REFERENCES tbl_logistics_fee_rate (id)
) COMMENT '物流方式通用费率'; # 物流方式和通用费率的关联表