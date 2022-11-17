CREATE TABLE tbl_hs_goods
(
    id            INT                     NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    goods_type_id INT          DEFAULT 0  NOT NULL COMMENT '物品类型#type:LogisticsGoodsType', # 暂时不用到，预先添加一下
    cn_hs_code    VARCHAR(100) DEFAULT '' NOT NULL COMMENT '中国海关编码',
    cn_hs_name    VARCHAR(200) DEFAULT '' NOT NULL COMMENT '中国海关申报名',
    en_hs_code    VARCHAR(100) DEFAULT '' NOT NULL COMMENT '英文海关编码',
    en_hs_name    VARCHAR(200) DEFAULT '' NOT NULL COMMENT '英文海关申报名',
    elem01        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素1',
    elem02        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素2',
    elem03        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素3',
    elem04        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素4',
    elem05        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素5',
    elem06        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素6',
    elem07        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素7',
    elem08        VARCHAR(300) DEFAULT '' NOT NULL COMMENT '要素8',
    add_user_id   INT                     NOT NULL COMMENT '添加人员',                         # 暂时不用到，预先添加一下
    create_time   DATETIME                NOT NULL COMMENT '创建时间',
    update_time   DATETIME                NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_hs_goods_cn_hs_name (cn_hs_name),
    CONSTRAINT fk_hs_goods_add_user_id FOREIGN KEY (add_user_id) REFERENCES tbl_user (id)
) COMMENT '海关品名';
