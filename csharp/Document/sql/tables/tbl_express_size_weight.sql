# DROP TABLE IF EXISTS tbl_express_size_weight;

CREATE TABLE tbl_express_size_weight
(
    id              INT                       NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',

    express_id      INT                       NULL COMMENT '快递单',
    order_id        INT                       NULL COMMENT '订单',
    order_code      VARCHAR(100)   DEFAULT '' NOT NULL COMMENT '订单编号',

    tracking_no     VARCHAR(30)    DEFAULT '' NOT NULL COMMENT '快递单号',
    tracking_no_sub VARCHAR(30)    DEFAULT '' NOT NULL COMMENT '快递单子单号', # 不是子单号时，这个字段就空着
    description     VARCHAR(200)   DEFAULT '' NOT NULL COMMENT '快递单描述',

    length          DECIMAL(10, 1) DEFAULT 0  NOT NULL COMMENT '长度cm',
    width           DECIMAL(10, 1) DEFAULT 0  NOT NULL COMMENT '宽度cm',
    height          DECIMAL(10, 1) DEFAULT 0  NOT NULL COMMENT '高度cm',
    weight          DECIMAL(12, 3) DEFAULT 0  NOT NULL COMMENT '重量kg',
    package_count   INT            DEFAULT 1  NOT NULL COMMENT '入库包裹数',

    user_id         INT                       NULL COMMENT '添加用户',
    machine_id      INT                       NULL COMMENT '自动秤',

    create_time     DATETIME                  NOT NULL COMMENT '创建时间',
    update_time     DATETIME                  NOT NULL COMMENT '更新时间',

    INDEX ix_express_size_weight_tracking_no (tracking_no),
    INDEX ix_express_size_weight_tracking_no_sub (tracking_no_sub),
    INDEX ix_express_size_weight_create (create_time),

    CONSTRAINT fk_express_size_weight_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_size_weight_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_size_weight_machine_id FOREIGN KEY (machine_id) REFERENCES tbl_auto_measure_machine (id),
    CONSTRAINT fk_express_size_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id)
) COMMENT '快递单尺寸重量';

