# DROP TABLE IF EXISTS tbl_express_remark_preset_select;

CREATE TABLE tbl_express_remark_preset_select
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    type_id     INT          NOT NULL COMMENT '备注类型#type:ExpressRemarkType',
    remark      VARCHAR(300) NOT NULL COMMENT '备注',
    order_num   INT          NOT NULL COMMENT '排序',

    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间'
) COMMENT '快递单备注预设';