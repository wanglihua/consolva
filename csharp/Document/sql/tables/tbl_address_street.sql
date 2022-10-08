CREATE TABLE tbl_address_street
(
    id         INT         NOT NULL PRIMARY KEY COMMENT '编码',
    area_id    INT         NOT NULL COMMENT '区域编码',
    name       VARCHAR(50) NOT NULL COMMENT '名称',
    short_name VARCHAR(30) NOT NULL COMMENT '简称',
    lng        VARCHAR(20) NOT NULL COMMENT '纬度',
    lat        VARCHAR(20) NOT NULL COMMENT '经度',
    sort       INT         NOT NULL COMMENT '排序',

    CONSTRAINT fk_address_street_area_id FOREIGN KEY (area_id) REFERENCES tbl_address_area (id)
) COMMENT '街道';


/*

TRUNCATE TABLE tbl_address_street;

INSERT INTO tbl_address_street(id, area_id, name, short_name, lng, lat, sort)
SELECT code,
       area_code,
       name,
       short_name,
       lng,
       lat,
       sort
FROM iepoch.tbl_address_street;

 */