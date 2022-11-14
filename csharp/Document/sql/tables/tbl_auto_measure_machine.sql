CREATE TABLE tbl_auto_measure_machine
(
    id           INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    name         VARCHAR(100) NOT NULL COMMENT '名称',
    description  VARCHAR(300) NOT NULL COMMENT '描述',
    warehouse_id INT          NOT NULL COMMENT '仓库',
    app_key      VARCHAR(50)  NOT NULL COMMENT 'app key',
    secret       VARCHAR(50)  NOT NULL COMMENT 'secret',

    create_time  DATETIME     NOT NULL COMMENT '创建时间',
    update_time  DATETIME     NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_auto_measure_machine_warehouse_id FOREIGN KEY (warehouse_id) REFERENCES tbl_warehouse (id),
    INDEX ix_auto_measure_machine_app_key (app_key) # 没几台设备，这个索引加不加都一样
) COMMENT '自动秤设备';
