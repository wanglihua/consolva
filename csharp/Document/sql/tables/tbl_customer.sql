CREATE TABLE tbl_customer
(
    id                 INT                          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    country_id         INT            DEFAULT 0     NOT NULL COMMENT '客户国家#type:Country',
    open_id            VARCHAR(100)   DEFAULT ''    NOT NULL COMMENT 'OpenId',
    mobile             VARCHAR(20)    DEFAULT ''    NOT NULL COMMENT '手机号码',
    mobile_validated   BOOL           DEFAULT FALSE NOT NULL COMMENT '手机验证',
    salt               VARCHAR(20)    DEFAULT ''    NOT NULL COMMENT '密码salt',
    password           VARCHAR(32)    DEFAULT ''    NOT NULL COMMENT '密码',
    pay_password       VARCHAR(32)    DEFAULT ''    NOT NULL COMMENT '支付密码',
    reg_time           DATETIME       DEFAULT NOW() NOT NULL COMMENT '注册时间',
    last_login         DATETIME       DEFAULT NOW() NOT NULL COMMENT '最后登录时间',
    last_ip            VARCHAR(200)   DEFAULT ''    NOT NULL COMMENT '最后登录ip',
    account            DECIMAL(10, 2) DEFAULT 0     NOT NULL COMMENT '用户金额',
    total_score        DECIMAL(10, 2) DEFAULT 0     NOT NULL COMMENT '总积分',
    valid_score        DECIMAL(10, 2) DEFAULT 0     NOT NULL COMMENT '有效积分',
    total_consume      DECIMAL(10, 2) DEFAULT 0     NOT NULL COMMENT '累计消费',
    level_id           INT            DEFAULT 0     NOT NULL COMMENT '客户级别#type:CustomerLevel',
    vip_rank_id        INT            DEFAULT 0     NOT NULL COMMENT 'VIP等级#type:CustomerVipRank',
    customer_character VARCHAR(30)    DEFAULT ''    NOT NULL COMMENT '客户性质',
    create_method_id   INT            DEFAULT 0     NOT NULL COMMENT '创建方式#type:CustomerCreateMethod',
    status_id          INT            DEFAULT 0     NOT NULL COMMENT '状态',
    create_time        DATETIME                     NOT NULL COMMENT '创建时间',
    update_time        DATETIME                     NOT NULL COMMENT '更新时间',

    INDEX ix_customer_mobile (mobile),
    INDEX ix_customer_open_id (open_id)
    # INDEX ix_customer_customer_character (customer_character), 这个很少查询 
    # INDEX ix_customer_create_time (create_time) 很少按这个查询 
) COMMENT '客户';


# todo 还需要加上一个收货代码 可以是数字前缀 234 