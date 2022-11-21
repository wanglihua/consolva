# DROP TABLE tbl_express_checkin_config;

CREATE TABLE tbl_express_checkin_config
(
    id                                INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    express_count_per_storage_loc     INT      NOT NULL COMMENT '每仓位快递数',
    signin_photo_min_qty_per_express  INT      NOT NULL DEFAULT 0 COMMENT '签收照片最小数量',
    checkin_photo_min_qty_per_express INT      NOT NULL DEFAULT 0 COMMENT '入库照片最小数量',
    auto_record_checkin_video         BOOL     NOT NULL DEFAULT FALSE COMMENT '是否自动录制视频',
    client_show_video                 BOOL     NOT NULL DEFAULT FALSE COMMENT '客户端是否显示视频',
    checkin_photo_client_show         BOOL     NOT NULL DEFAULT FALSE COMMENT '客户端是否显示入库照片',
    label_w10_h05_status_id           INT      NOT NULL COMMENT '标签10*5是否启用#type:ExpressCheckinLabelConfigStatus',
    label_w10_h05_print_count         INT      NOT NULL COMMENT '标签10*5打印数量',

    label_w05_h05_status_id           INT      NOT NULL COMMENT '标签5*5是否启用#type:ExpressCheckinLabelConfigStatus',
    label_w05_h05_print_count         INT      NOT NULL COMMENT '标签5*5打印数量',

    create_time                       DATETIME NOT NULL COMMENT '创建时间',
    update_time                       DATETIME NOT NULL COMMENT '更新时间'
) COMMENT '快递单入库配置';
