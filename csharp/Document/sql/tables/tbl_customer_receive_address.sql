CREATE TABLE tbl_customer_receive_address
(
    id          INT                        NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '表id',
    customer_id INT          DEFAULT 0     NOT NULL COMMENT '客户id',
    country_id  INT          DEFAULT 0     NOT NULL COMMENT '国家#type:Country',
    first_name  VARCHAR(60)  DEFAULT ''    NOT NULL COMMENT '收货人名',
    last_name   VARCHAR(60)  DEFAULT ''    NOT NULL COMMENT '收货人姓',
    mobile      VARCHAR(60)  DEFAULT ''    NOT NULL COMMENT '手机',
    state       VARCHAR(50)  DEFAULT ''    NOT NULL COMMENT '省份',
    city        VARCHAR(50)  DEFAULT ''    NOT NULL COMMENT '城市',
    address     VARCHAR(200) DEFAULT ''    NOT NULL COMMENT '地址',
    unit_loc    VARCHAR(20)  DEFAULT ''    NOT NULL COMMENT 'Unit号',
    zipcode     VARCHAR(10)  DEFAULT ''    NOT NULL COMMENT '邮政编码',
    tfn         VARCHAR(100) DEFAULT ''    NOT NULL COMMENT 'ABN/ACN',
    company     VARCHAR(100) DEFAULT ''    NOT NULL COMMENT '公司',
    is_default  BOOL         DEFAULT 0     NOT NULL COMMENT '默认收货地址',
    valid       BOOL         DEFAULT FALSE NOT NULL COMMENT '是否已验证',
    create_time DATETIME                   NOT NULL COMMENT '创建时间',
    update_time DATETIME                   NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_customer_receive_address_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '用户收货地址';
