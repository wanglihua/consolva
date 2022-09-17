CREATE TABLE tbl_site_request_log
(
    id            INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    session_id    VARCHAR(100)   NOT NULL COMMENT 'session id',
    ip_address    VARCHAR(200)   NOT NULL COMMENT 'ip',
    user_id       INT            NULL COMMENT 'user id',
    customer_id   INT            NULL COMMENT 'customer id',
    url           VARCHAR(500)   NOT NULL COMMENT 'url',
    form_params   VARCHAR(520)   NULL COMMENT 'form params',
    total_seconds DECIMAL(10, 6) NOT NULL COMMENT 'total seconds',
    create_time   DATETIME       NOT NULL COMMENT '创建时间'

    # CONSTRAINT fk_site_request_key_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),
    # CONSTRAINT fk_site_request_key_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id),

    # INDEX ix_site_request_log_session_id (session_id),
    # INDEX ix_site_request_log_create_time (create_time)
) COMMENT '网站请求日志';


