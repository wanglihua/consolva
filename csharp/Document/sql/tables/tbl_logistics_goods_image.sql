CREATE TABLE tbl_logistics_goods_image
(
    id           INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    goods_id     INT          NOT NULL COMMENT '物流物品',
    store_path   VARCHAR(255) NOT NULL COMMENT '存储路径',
    store_name   VARCHAR(50)  NOT NULL COMMENT '存储名称',                           # 服务器上的文件名，设计为guid
    store_loc_id INT          NOT NULL COMMENT '存储位置#type:ExpressPhotoStoreLoc', # 存储在阿里云ECS还是OSS
    create_time  DATETIME     NOT NULL COMMENT '创建时间',

    CONSTRAINT fk_logistics_goods_image_goods_id FOREIGN KEY (goods_id) REFERENCES tbl_logistics_goods (id)
) COMMENT '物流物品图片';
