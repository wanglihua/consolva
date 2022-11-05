CREATE TABLE tbl_express_photo
(
    id                  INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id         INT                     NULL COMMENT '客户',
    order_id            INT                     NULL COMMENT '物流订单',
    order_code          VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号', # 这个字段不加索引，只是列表显示显示，不做查询，查询通过tbl_logistics_order表code字段，有索引
    express_id          INT                     NULL COMMENT '快递单号',
    express_tracking_no VARCHAR(30)             NULL COMMENT '快递单号',
    express_com_name    VARCHAR(100)            NULL COMMENT '快递公司',
    type_id             INT                     NOT NULL COMMENT '类型#type:ExpressPhotoType',
    store_path          VARCHAR(255)            NOT NULL COMMENT '存储路径',
    store_name          VARCHAR(50)             NOT NULL COMMENT '存储名称', # 服务器上的文件名，设计为guid
    user_id             INT                     NULL COMMENT '上传用户',
    store_oss           BOOL                    NULL COMMENT '存储在阿里云OSS',
    status_id           INT                     NOT NULL COMMENT '状态#type:ExpressPhotoStatus',

    create_time         DATETIME                NOT NULL COMMENT '创建时间',
    update_time         DATETIME                NOT NULL COMMENT '更新时间',

    INDEX ix_express_photo_express_tracking_no (express_tracking_no),
    CONSTRAINT fk_express_photo_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_photo_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    INDEX ix_express_photo_create_time (create_time)
) COMMENT '快递单图片';
