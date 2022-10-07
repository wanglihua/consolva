CREATE TABLE tbl_logistics_fee_rate_express
(
    id                    INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    logistics_mode_id     INT            NOT NULL COMMENT '物流方式id',
    start_weight          DECIMAL(10, 3) NOT NULL COMMENT '起步重量',
    start_price           DECIMAL(10, 2) NOT NULL COMMENT '起步价格',
    step_weight           DECIMAL(10, 3) NOT NULL COMMENT '阶梯重量',
    step_price            DECIMAL(10, 2) NOT NULL COMMENT '阶梯价格',
    start_free_min_weight DECIMAL(10, 2) NULL COMMENT '首重免费最小总重', # null 或者 <= 0 都表示不启用首重免费
    weight_break_start    DECIMAL(10, 3) NULL COMMENT '重量满减起始重量',
    weight_break_discount DECIMAL(10, 3) NULL COMMENT '重量满减优惠重量',

    create_time           DATETIME       NOT NULL COMMENT '创建时间',
    update_time           DATETIME       NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_logistics_fee_rate_express_logistics_mode_id FOREIGN KEY (logistics_mode_id) REFERENCES tbl_logistics_mode (id)
) COMMENT '快递具体费率';