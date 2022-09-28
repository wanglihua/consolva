CREATE TABLE tbl_express_return_fee_rate
(
    id              INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    fee_per_express DECIMAL(10, 2) NOT NULL COMMENT '每快递操作费',
    create_time     DATETIME       NOT NULL COMMENT '创建时间',
    update_time     DATETIME       NOT NULL COMMENT '更新时间'
) COMMENT '退运操作费率';

