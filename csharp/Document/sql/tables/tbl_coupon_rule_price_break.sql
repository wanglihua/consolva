DROP TABLE IF EXISTS tbl_coupon_rule_price_break;

CREATE TABLE tbl_coupon_rule_price_break
(
    id          INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    batch_id    INT            NOT NULL COMMENT '批次',
    amount      DECIMAL(10, 2) NOT NULL COMMENT '消费金额',
    discount    DECIMAL(10, 2) NOT NULL COMMENT '折减金额',

    create_time DATETIME       NOT NULL COMMENT '创建时间',
    update_time DATETIME       NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_coupon_rule_price_break_batch_id FOREIGN KEY (batch_id) REFERENCES tbl_coupon_batch (id)
) COMMENT '优惠券满减规则';