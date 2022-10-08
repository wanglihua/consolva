CREATE TABLE tbl_address_province
(
    id         INT         NOT NULL PRIMARY KEY COMMENT '编码',
    name       VARCHAR(50) NOT NULL COMMENT '名称',
    short_name VARCHAR(20) NOT NULL COMMENT '简称',
    lng        VARCHAR(20) NOT NULL COMMENT '纬度',
    lat        VARCHAR(20) NOT NULL COMMENT '经度',
    sort       INT         NOT NULL COMMENT '排序'
) COMMENT '省';

/*

TRUNCATE TABLE tbl_address_province;

INSERT INTO tbl_address_province
    (id, name, short_name, lng, lat, sort)
SELECT code,
       name,
       short_name,
       lng,
       lat,
       sort
FROM iepoch.tbl_address_province;

 */