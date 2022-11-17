CREATE TABLE tbl_logistics_goods
(
    id               INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    name             VARCHAR(200) NOT NULL COMMENT '扩展名',
    cate_id          INT          NOT NULL COMMENT '类别', # 暂时为可控类型
    type_id          INT          NOT NULL COMMENT '物品类型',
    has_brand        BOOL         NOT NULL COMMENT '是否品牌',
    brand_name       VARCHAR(200) NULL COMMENT '品牌名称',
    has_battery      BOOL         NOT NULL COMMENT '含电池或其他限制',
    has_bamboo       BOOL         NOT NULL COMMENT '是否含草木竹',
    has_wood_package BOOL         NOT NULL COMMENT '是否含实木包装',
    is_special       BOOL         NOT NULL DEFAULT FALSE COMMENT '是否特货类',
    is_forbidden     BOOL         NOT NULL COMMENT '是否禁止类',
    add_user_id      INT          NOT NULL COMMENT '添加人员',
    hs_goods_id      INT          NULL COMMENT '海关品名',
    status_id        INT          NOT NULL COMMENT '状态',
    create_time      DATETIME     NOT NULL COMMENT '创建时间',
    update_time      DATETIME     NOT NULL COMMENT '更新时间',
    version          INT          NOT NULL DEFAULT 1 COMMENT '数据版本',

    INDEX ix_logistics_goods_cate_id (cate_id),
    INDEX ix_logistics_goods_type_id (type_id),
    INDEX ix_logistics_goods_status_id (status_id),
    INDEX ix_logistics_goods_brand_name (brand_name),
    INDEX ix_logistics_goods_name (name),

    FOREIGN KEY fk_logistics_goods_add_user_id (add_user_id) REFERENCES tbl_user (id),
    FOREIGN KEY fk_logistics_goods_hs_goods_id (hs_goods_id) REFERENCES tbl_hs_goods (id)
) COMMENT '物流物品';


ALTER TABLE tbl_logistics_goods
    ADD cate_id INT NULL COMMENT '类别' AFTER name;

ALTER TABLE tbl_logistics_goods
    ADD INDEX ix_logistics_goods_cate_id (cate_id);


UPDATE tbl_logistics_goods
SET cate_id = 1000
WHERE 1 = 1;

ALTER TABLE tbl_logistics_goods
    MODIFY cate_id INT NOT NULL COMMENT '类别';

ALTER TABLE tbl_logistics_goods
    ADD is_special BOOL NOT NULL DEFAULT FALSE COMMENT '是否特货类' AFTER has_wood_package;

ALTER TABLE tbl_logistics_goods
    ADD create_express_tracking_no VARCHAR(30) NULL COMMENT '创建单号';

ALTER TABLE tbl_logistics_goods
    ADD INDEX ix_logistics_goods_update_time (update_time);

ALTER TABLE tbl_logistics_goods
    ADD update_user_id INT NULL DEFAULT NULL COMMENT '最后修改人';

ALTER TABLE tbl_logistics_goods
    ADD CONSTRAINT fk_logistics_goods_update_user_id FOREIGN KEY (update_user_id) REFERENCES tbl_user (id);

