CREATE TABLE tbl_logistics_insurance
(
    id                INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    name              VARCHAR(50)    NOT NULL COMMENT '名称',
    content           TEXT           NULL COMMENT '保险条款',
    type_id           INT            NOT NULL COMMENT '费用类型#type:LogisticsInsuranceType',
    calc_base_type_id INT            NOT NULL COMMENT '计算基数#type:FeeCalcBaseType',
    fee_rate          DECIMAL(10, 4) NOT NULL COMMENT '费率/金额',
    order_num         INT            NULL COMMENT '排序',
    status_id         INT            NOT NULL COMMENT '状态#type:LogisticsInsuranceStatus',
    create_time       DATETIME       NOT NULL COMMENT '创建时间',
    update_time       DATETIME       NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_insurance_type_id (type_id),
    INDEX ix_logistics_insurance_order_num (order_num)
) COMMENT '保险';
