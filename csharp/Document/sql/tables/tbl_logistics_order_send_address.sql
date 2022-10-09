CREATE TABLE tbl_logistics_order_send_address
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id    INT          DEFAULT 0  NOT NULL COMMENT '订单id',
    country_id  INT          DEFAULT 0  NOT NULL COMMENT '国家#type:Country',
    first_name  VARCHAR(60)  DEFAULT '' NOT NULL COMMENT '发货人名',
    last_name   VARCHAR(60)  DEFAULT '' NOT NULL COMMENT '发货人姓',
    mobile      VARCHAR(60)  DEFAULT '' NOT NULL COMMENT '手机',
    province_id INT                     NOT NULL COMMENT '省代码',
    city_id     INT                     NOT NULL COMMENT '城市代码',
    area_id     INT                     NOT NULL COMMENT '区域代码',
    street_id   INT                     NOT NULL COMMENT '街道代码',
    address     VARCHAR(200) DEFAULT '' NOT NULL COMMENT '地址',
    zipcode     VARCHAR(10)  DEFAULT '' NOT NULL COMMENT '邮政编码',
    tfn         VARCHAR(100) DEFAULT '' NULL COMMENT '税号',
    company     VARCHAR(100) DEFAULT '' NULL COMMENT '公司',
    create_time DATETIME                NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_logistics_order_send_address_customer_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_send_address_province_id FOREIGN KEY (province_id) REFERENCES tbl_address_province (id),
    CONSTRAINT fk_logistics_order_send_address_city_id FOREIGN KEY (city_id) REFERENCES tbl_address_city (id),
    CONSTRAINT fk_logistics_order_send_address_area_id FOREIGN KEY (area_id) REFERENCES tbl_address_area (id),
    CONSTRAINT fk_logistics_order_send_address_street_id FOREIGN KEY (street_id) REFERENCES tbl_address_street (id)
) COMMENT '订单发货地址';
