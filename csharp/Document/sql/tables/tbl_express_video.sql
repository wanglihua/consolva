CREATE TABLE tbl_express_video
(
    id                      INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    customer_id             INT          NULL COMMENT '客户',
    order_id                INT          NULL COMMENT '物流订单',
    express_id              INT          NULL COMMENT '快递单号',
    express_tracking_no     VARCHAR(30)  DEFAULT '' COMMENT '快递单号',
    express_com_name        VARCHAR(100) DEFAULT NULL COMMENT '快递公司',
    type_id                 INT          NOT NULL COMMENT '类型#type:ExpressVideoType',
    store_oss               TINYINT(1)   DEFAULT NULL COMMENT '存储在阿里云OSS',
    store_path              VARCHAR(255) NOT NULL COMMENT '存储路径',
    store_name              VARCHAR(50)  NOT NULL COMMENT '存储名称',
    user_id                 INT           NULL COMMENT '上传用户',
    status_id               INT          NOT NULL COMMENT '状态#type:ExpressVideoStatus',
    create_time             DATETIME     NOT NULL COMMENT '创建时间',
    update_time             DATETIME     NOT NULL COMMENT '更新时间',

    KEY fk_express_video_order_id (order_id),
    KEY fk_express_video_express_id (express_id),
    KEY ix_express_video_type_id (type_id),
    KEY ix_express_video_express_tracking_no (express_tracking_no),
    KEY ix_express_video_status_id (status_id),
    KEY ix_express_video_create_time (create_time)
) COMMENT '快递单视频';

