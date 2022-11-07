# DROP TABLE IF EXISTS tbl_coupon;

CREATE TABLE tbl_coupon
(
    id                     INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    batch_id               INT            NOT NULL COMMENT '批次',
    code                   VARCHAR(100)   NOT NULL COMMENT '编号', # 类似 01A0001 这样的编号，批次ID(最少2位)+类型编号+ID(最少4位)
    customer_id            INT            NOT NULL COMMENT '客户', # 领取后填充
    order_id               INT            NULL COMMENT '订单',     # 使用后填充，如果订单取消优惠券是不是也要返回
    actual_discount_amount DECIMAL(10, 2) NOT NULL DEFAULT 0 COMMENT '实际优惠金额',
    pick_time              DATETIME       NOT NULL COMMENT '领取时间',
    use_time               DATETIME       NULL COMMENT '使用时间',
    get_method_id          INT            NOT NULL DEFAULT 1000 COMMENT '获取方式',
    status_id              INT            NOT NULL COMMENT '状态#type:CouponStatus',

    create_time            DATETIME       NOT NULL COMMENT '创建时间',
    update_time            DATETIME       NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_coupon_code (code),
    CONSTRAINT fk_coupon_batch_id FOREIGN KEY (batch_id) REFERENCES tbl_coupon_batch (id),
    CONSTRAINT fk_coupon_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    CONSTRAINT fk_coupon_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id)
) COMMENT '优惠券';