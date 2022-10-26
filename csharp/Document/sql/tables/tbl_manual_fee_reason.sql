CREATE TABLE tbl_manual_fee_reason
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    fee_type_id INT          NOT NULL COMMENT '费用类型#type:ManualFeeType',
    content     VARCHAR(300) NOT NULL COMMENT '内容',
    status_id   INT          NOT NULL COMMENT '状态#type:ManualFeeReasonStatus',

    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间',

    INDEX ix_manual_fee_reason_fee_type_id (fee_type_id),
    INDEX ix_manual_fee_reason_status_id (status_id)
) COMMENT '增减核销费用原因';