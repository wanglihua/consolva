CREATE TABLE tbl_customer_super_vip_discount
(
    id          INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    customer_id INT            NOT NULL COMMENT '会员',
    discount    DECIMAL(10, 2) NOT NULL COMMENT '折扣',
    description VARCHAR(500)   NULL COMMENT '备注',
    user_id     INT            NOT NULL COMMENT '操作用户',
    create_time DATETIME       NOT NULL COMMENT '创建时间',
    update_time DATETIME       NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_customer_super_vip_discount_customer_id (customer_id),
    CONSTRAINT fk_customer_super_vip_discount_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_customer_super_vip_discount_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id)
) COMMENT '超级VIP折扣';
