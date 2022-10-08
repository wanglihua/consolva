CREATE TABLE tbl_address_area
(
    id         INT         NOT NULL PRIMARY KEY COMMENT '编码',
    city_id    INT         NOT NULL COMMENT '城市编码',
    name       VARCHAR(50) NOT NULL COMMENT '名称',
    short_name VARCHAR(20) NOT NULL COMMENT '简称',
    lng        VARCHAR(20) NOT NULL COMMENT '纬度',
    lat        VARCHAR(20) NOT NULL COMMENT '经度',
    sort       INT         NOT NULL COMMENT '排序',

    CONSTRAINT fk_address_area_city_id FOREIGN KEY (city_id) REFERENCES tbl_address_city (id)
) COMMENT '地区';

/*

TRUNCATE TABLE tbl_address_area;

INSERT INTO tbl_address_area(id, city_id, name, short_name, lng, lat, sort)
SELECT code,
       city_code,
       name,
       short_name,
       lng,
       lat,
       sort
FROM iepoch.tbl_address_area;

 */