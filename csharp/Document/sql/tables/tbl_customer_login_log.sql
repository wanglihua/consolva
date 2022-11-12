CREATE TABLE tbl_customer_login_log
(
    id          INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    stage_id    INT          DEFAULT 0  NOT NULL COMMENT '登录阶段#type:CustomerLoginStage',
    customer_id INT                     NOT NULL COMMENT '登录客户',
    open_id     VARCHAR(100) DEFAULT '' NOT NULL COMMENT 'open id',
    session_id  VARCHAR(50)  DEFAULT '' NOT NULL COMMENT 'session id',
    ip_address  VARCHAR(200) DEFAULT '' NOT NULL COMMENT 'IP地址',
    user_agent  VARCHAR(300) DEFAULT '' NOT NULL COMMENT 'User Agent',
    login_time  DATETIME                NOT NULL COMMENT '登录时间',

    CONSTRAINT fk_customer_login_log_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '客户登录日志';
