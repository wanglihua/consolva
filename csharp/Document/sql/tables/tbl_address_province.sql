CREATE TABLE tbl_address_province
(
    id          INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    code        VARCHAR(40) NOT NULL COMMENT '编码',
    name        VARCHAR(50) NOT NULL COMMENT '名称',
    short_name  VARCHAR(20) NOT NULL COMMENT '简称',
    lng         VARCHAR(20) NOT NULL COMMENT '纬度',
    lat         VARCHAR(20) NOT NULL COMMENT '经度',
    sort        INT         NOT NULL COMMENT '排序',
    create_time DATETIME    NOT NULL COMMENT '创建时间',
    update_time DATETIME    NOT NULL COMMENT '更新时间',

    INDEX ix_address_province_code (code)
) COMMENT '省';

/*

TRUNCATE TABLE tbl_address_province;

INSERT INTO tbl_address_province
    (id, code, name, short_name, lng, lat, sort, create_time, update_time)
SELECT id,
       code,
       name,
       short_name,
       lng,
       lat,
       sort,
       create_time,
       update_time
FROM iepoch.tbl_address_province;

 */