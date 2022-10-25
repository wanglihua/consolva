CREATE TABLE tbl_customer_score_rule
(
    id            INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    type_id       INT            NOT NULL COMMENT '类型#type:CustomerScoreType',
    amount        DECIMAL(10, 2) NOT NULL COMMENT '金额',
    score         DECIMAL(10, 2) NOT NULL COMMENT '积分',

    create_time   DATETIME       NOT NULL COMMENT '创建时间',
    update_time   DATETIME       NOT NULL COMMENT '更新时间',

    INDEX ix_customer_score_rule_type_id (type_id)
) COMMENT '会员积分规则';
