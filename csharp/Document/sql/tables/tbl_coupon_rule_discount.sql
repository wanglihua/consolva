# DROP TABLE IF EXISTS tbl_coupon_rule_discount;

CREATE TABLE tbl_coupon_rule_discount
(
    id          INT           NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    batch_id    INT           NOT NULL COMMENT '批次',
    rate        DECIMAL(6, 3) NOT NULL COMMENT '折率',

    create_time DATETIME      NOT NULL COMMENT '创建时间',
    update_time DATETIME      NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_coupon_rule_discount_batch_id FOREIGN KEY (batch_id) REFERENCES tbl_coupon_batch (id)
) COMMENT '优惠券折扣规则';