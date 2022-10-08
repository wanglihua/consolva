CREATE TABLE tbl_address_city
(
    id          INT         NOT NULL PRIMARY KEY COMMENT '编码',
    province_id INT         NOT NULL COMMENT '省编码',
    name        VARCHAR(50) NOT NULL COMMENT '名称',
    short_name  VARCHAR(20) NOT NULL COMMENT '简称',
    lng         VARCHAR(20) NOT NULL COMMENT '纬度',
    lat         VARCHAR(20) NOT NULL COMMENT '经度',
    sort        INT         NOT NULL COMMENT '排序',

    CONSTRAINT fk_address_city_province_id FOREIGN KEY (province_id) REFERENCES tbl_address_province (id)
) COMMENT '城市';

/*

TRUNCATE TABLE tbl_address_city;

INSERT INTO tbl_address_city(id, province_id, name, short_name, lng, lat, sort)
SELECT code,
       province_code,
       name,
       short_name,
       lng,
       lat,
       sort
FROM iepoch.tbl_address_city;

 */