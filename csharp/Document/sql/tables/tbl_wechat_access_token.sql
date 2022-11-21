# DROP TABLE IF EXISTS tbl_wechat_access_token;

CREATE TABLE tbl_wechat_access_token
(
    id           INT          NOT NULL NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    access_token VARCHAR(600) NOT NULL COMMENT 'access token', # access_token的存储至少要保留512个字符空间？
    expire       DATETIME     NOT NULL COMMENT '过期时间',

    create_time  DATETIME     NOT NULL COMMENT '创建时间'
) COMMENT '微信AccessToken';
