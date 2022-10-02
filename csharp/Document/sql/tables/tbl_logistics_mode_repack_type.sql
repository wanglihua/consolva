CREATE TABLE tbl_logistics_mode_repack_type
(
    id                INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    logistics_mode_id INT      NOT NULL COMMENT '物流方式',
    repack_type_id    INT      NOT NULL COMMENT '打包方式#type:LogisticsRepackType',

    create_time       DATETIME NOT NULL COMMENT '创建时间',
    update_time       DATETIME NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_fee_mode_repack_type_id (repack_type_id),
    CONSTRAINT fk_logistics_mode_repack_type_logistics_mode_id FOREIGN KEY (logistics_mode_id) REFERENCES tbl_logistics_mode (id)
) COMMENT '物流方式打包方式';