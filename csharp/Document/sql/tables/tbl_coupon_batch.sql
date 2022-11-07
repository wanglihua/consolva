# DROP TABLE IF EXISTS tbl_coupon_batch;

CREATE TABLE tbl_coupon_batch
(
    id                   INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    name                 VARCHAR(200) NOT NULL COMMENT '名称',
    type_id              INT          NOT NULL COMMENT '类型#type:CouponType',
    expire               DATETIME     NOT NULL COMMENT '有效期',
    total_quantity       INT          NOT NULL COMMENT '总数量',
    pick_quantity        INT          NOT NULL DEFAULT 0 COMMENT '已领用数量',
    use_quantity         INT          NOT NULL DEFAULT 0 COMMENT '已使用数量',
    rule                 VARCHAR(20)  NULL COMMENT '规则',   # 设置完规则后自动填入，类似规则描述的简短版
    rule_desc            VARCHAR(500) NULL COMMENT '规则描述', # 设置完规则后自动填入
    max_qty_per_customer INT          NOT NULL DEFAULT 1 COMMENT '每客户可领取数量',
    status_id            INT          NOT NULL COMMENT '状态#type:CouponBatchStatus',

    create_time          DATETIME     NOT NULL COMMENT '创建时间',
    update_time          DATETIME     NOT NULL COMMENT '更新时间',

    INDEX ix_coupon_batch_expire (expire)
) COMMENT '优惠券批次';
