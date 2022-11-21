CREATE TABLE tbl_logistics_order_pack
(
    id                  INT PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    order_id            INT                         NOT NULL COMMENT '订单',
    goods_type_id       INT                         NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',
    order_first_mode_id INT                         NOT NULL COMMENT '物流订单头程',
    first_type_id       INT                         NOT NULL COMMENT '物流方式类型#type:LogisticsModeType',
    first_mode_id       INT                         NOT NULL COMMENT '物流方式',
    length              DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '长/cm',
    width               DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '宽/cm',
    height              DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '高/cm',
    weight              DECIMAL(10, 2) DEFAULT 0.00 NOT NULL COMMENT '重量/kg',
    package_count       INT            DEFAULT 1    NOT NULL COMMENT '装箱数目',
    pack_user_id        INT            DEFAULT 0    NOT NULL COMMENT '打包人员',
    pack_time           DATETIME                    NOT NULL COMMENT '打包时间', # 一般就是 create_time

    create_time         DATETIME                    NOT NULL COMMENT '创建时间',
    update_time         DATETIME                    NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_logistics_order_pack_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_logistics_order_pack_order_first_mode_id FOREIGN KEY (order_first_mode_id) REFERENCES tbl_logistics_order_first_mode (id),
    CONSTRAINT fk_logistics_order_pack_first_mode_id FOREIGN KEY (first_mode_id) REFERENCES tbl_logistics_mode (id),
    CONSTRAINT fk_logistics_order_pack_user_id FOREIGN KEY (pack_user_id) REFERENCES tbl_user (id)
) COMMENT '物流订单打包';
