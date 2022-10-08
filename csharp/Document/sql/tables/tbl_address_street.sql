CREATE TABLE tbl_address_street
(
    id          INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    area_code   VARCHAR(40) NOT NULL COMMENT '区域编码',
    code        VARCHAR(40) NOT NULL COMMENT '编码',
    name        VARCHAR(50) NOT NULL COMMENT '名称',
    short_name  VARCHAR(30) NOT NULL COMMENT '简称',
    lng         VARCHAR(20) NOT NULL COMMENT '纬度',
    lat         VARCHAR(20) NOT NULL COMMENT '经度',
    sort        INT         NOT NULL COMMENT '排序',
    create_time DATETIME    NOT NULL COMMENT '创建时间',
    update_time DATETIME    NOT NULL COMMENT '更新时间',

    INDEX ix_address_street_code (code),
    INDEX ix_address_street_area_code (area_code)
) COMMENT '街道';

TRUNCATE TABLE tbl_address_street;

INSERT INTO tbl_address_street(id, area_code, code, name, short_name, lng, lat, sort, create_time, update_time)
SELECT id,
       area_code,
       code,
       name,
       short_name,
       lng,
       lat,
       sort,
       create_time,
       update_time
FROM iepoch.tbl_address_street;