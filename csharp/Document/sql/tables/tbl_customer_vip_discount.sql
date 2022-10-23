CREATE TABLE tbl_customer_vip_discount
(
    id          INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    rank_id     VARCHAR(50)    NOT NULL COMMENT '会员VIP等级#type:CustomerVipRank',
    score       DECIMAL(10, 2) NOT NULL COMMENT '历史总积分',
    discount    DECIMAL(10, 2) NOT NULL COMMENT '折扣',
    user_id     INT            NOT NULL COMMENT '操作用户',

    create_time DATETIME       NOT NULL COMMENT '创建时间',
    update_time DATETIME       NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_customer_vip_discount_rank_id (rank_id),
    INDEX ix_customer_vip_discount_score (score),
    CONSTRAINT fk_customer_vip_discount_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id)
) COMMENT 'VIP折扣设置';
