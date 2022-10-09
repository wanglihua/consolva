CREATE TABLE tbl_logistics_order_receive_address
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id    INT          DEFAULT 0  NOT NULL COMMENT '客户id',
    first_name  VARCHAR(60)  DEFAULT '' NOT NULL COMMENT '收货人名',
    last_name   VARCHAR(60)  DEFAULT '' NOT NULL COMMENT '收货人姓',
    mobile      VARCHAR(60)  DEFAULT '' NOT NULL COMMENT '手机',
    state       VARCHAR(50)  DEFAULT '' NOT NULL COMMENT '省份',
    city        VARCHAR(50)  DEFAULT '' NOT NULL COMMENT '城市',
    address     VARCHAR(200) DEFAULT '' NOT NULL COMMENT '地址',
    unit_loc    VARCHAR(20)  DEFAULT '' NOT NULL COMMENT 'Unit号',
    zipcode     VARCHAR(10)  DEFAULT '' NOT NULL COMMENT '邮政编码',
    tfn         VARCHAR(100) DEFAULT '' NOT NULL COMMENT 'ABN/ACN',
    company     VARCHAR(100) DEFAULT '' NOT NULL COMMENT '公司',
    create_time DATETIME                NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_logistics_order_receive_address_customer_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id)
) COMMENT '订单收货地址';
