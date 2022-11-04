CREATE TABLE tbl_exchange_rate
(
    id                 INT                         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    from_currency_code VARCHAR(10)   DEFAULT ''    NOT NULL COMMENT '原货币代码',
    from_currency_name VARCHAR(10)   DEFAULT ''    NOT NULL COMMENT '原货币名称',
    to_currency_code   VARCHAR(10)   DEFAULT ''    NOT NULL COMMENT '折算货币代码',
    to_currency_name   VARCHAR(10)   DEFAULT ''    NOT NULL COMMENT '折算货币名称',
    rate               DECIMAL(8, 4) DEFAULT 1     NOT NULL COMMENT '汇率',
    rate_fix           DECIMAL(8, 4) DEFAULT 0     NOT NULL COMMENT '汇率修正',
    refresh_mark       INT           DEFAULT FALSE NOT NULL COMMENT '更新标记',

    create_time        DATETIME                    NOT NULL COMMENT '创建时间',
    update_time        DATETIME                    NOT NULL COMMENT '更新时间',

    INDEX ix_exchange_from_currency_code (from_currency_code),
    INDEX ix_exchange_to_currency_code (to_currency_code),
    INDEX ix_exchange_rate_create_time (create_time),
    INDEX ix_exchange_rate_update_time (update_time)
) COMMENT '汇率';