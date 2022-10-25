CREATE TABLE tbl_customer_account_entry
(
    id             INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id    INT                     NOT NULL COMMENT '客户id',
    change_type_id INT                     NOT NULL COMMENT '余额变动类型#type:CustomerAccountChangeType',
    amount         DECIMAL(10, 2)          NOT NULL COMMENT '变动金额',
    account_before DECIMAL(10, 2)          NOT NULL COMMENT '变动前余额',
    account_after  DECIMAL(10, 2)          NOT NULL COMMENT '变动后余额',
    description    VARCHAR(300) DEFAULT '' NOT NULL COMMENT '变动描述',
    create_time    DATETIME                NOT NULL COMMENT '创建时间',
    update_time    DATETIME                NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_customer_account_entry_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),
    INDEX ix_customer_account_change_type_id (change_type_id),
    INDEX ix_customer_account_create_time (create_time),
    INDEX ix_customer_account_update_time (update_time)
) COMMENT '客户账户变动明细';
