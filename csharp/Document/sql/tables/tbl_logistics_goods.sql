CREATE TABLE tbl_logistics_goods
(
    id                         INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    name                       VARCHAR(200)            NOT NULL COMMENT '扩展名',
    cate_id                    INT          DEFAULT 0  NOT NULL COMMENT '类别#type:LogisticsGoodsCate',
    type_id                    INT          DEFAULT 0  NOT NULL COMMENT '物品类型#type:LogisticsGoodsType',
    brand_name                 VARCHAR(200) DEFAULT '' NOT NULL COMMENT '品牌名称',
    hs_goods_id                INT                     NULL COMMENT '海关品名',
    add_user_id                INT                     NOT NULL COMMENT '添加人员',
    update_user_id             INT                     NOT NULL COMMENT '最后修改人',
    create_express_tracking_no VARCHAR(30)  DEFAULT '' NOT NULL COMMENT '创建单号',
    status_id                  INT                     NOT NULL COMMENT '状态#type:LogisticsGoodsStatus',
    create_time                DATETIME                NOT NULL COMMENT '创建时间',
    update_time                DATETIME                NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_goods_type_id (type_id),
    UNIQUE INDEX ix_logistics_goods_name (name),

    CONSTRAINT fk_logistics_goods_hs_goods_id FOREIGN KEY (hs_goods_id) REFERENCES tbl_hs_goods (id),
    CONSTRAINT fk_logistics_goods_add_user_id FOREIGN KEY (add_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_logistics_goods_update_user_id FOREIGN KEY (update_user_id) REFERENCES tbl_user (id)
) COMMENT '物流物品';
