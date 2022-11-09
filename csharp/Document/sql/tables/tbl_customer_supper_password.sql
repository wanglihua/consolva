DROP TABLE IF EXISTS tbl_customer_supper_password;

CREATE TABLE tbl_customer_supper_password
(
    id          INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id INT         NOT NULL DEFAULT 0 COMMENT '客户id',
    password    VARCHAR(40) NOT NULL COMMENT '密码', # 不加密方式
    create_time DATETIME    NOT NULL COMMENT '创建时间',
    update_time DATETIME    NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_customer_supper_password_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '客户超级密码';
