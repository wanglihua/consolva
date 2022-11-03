CREATE TABLE tbl_manual_charge_reason
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    content     VARCHAR(300) NOT NULL COMMENT '内容',
    status_id   INT          NOT NULL COMMENT '状态#type:ManualChargeReasonStatus',

    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间',

    INDEX ix_manual_charge_reason_status_id (status_id)
) COMMENT '手动充值原因';
