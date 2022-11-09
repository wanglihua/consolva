# DROP TABLE IF EXISTS tbl_logistics_mode;

CREATE TABLE tbl_logistics_mode
(
    id                   INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    cate_id              INT          NOT NULL DEFAULT 1000 COMMENT '类别#type:LogisticsModeCate',
    type_id              INT          NOT NULL COMMENT '物流类型#type:LogisticsModeType',
    pack_size_type_id    INT          NOT NULL COMMENT '包裹大小件类型#type:LogisticsModePackSizeType',
    code                 VARCHAR(100) NOT NULL COMMENT '编码',
    name                 VARCHAR(200) NOT NULL COMMENT '名称',
    description          VARCHAR(500) NOT NULL COMMENT '描述',
    fee_calc_desc        VARCHAR(300) NULL COMMENT '费用计算描述',

    warehouse_id         INT          NOT NULL COMMENT '发货仓库',

    pack_label_type_id   INT          NOT NULL COMMENT '打包标签#type:LogisticsPackLabelType',

    channel_id           INT          NOT NULL COMMENT '渠道类型#type:LogisticsModeChannel',
    express_company_name VARCHAR(100) NULL COMMENT '快递公司名称',
    express_company_url  VARCHAR(300) NULL COMMENT '快递公司网址',

    inner_mode_id        INT          NULL COMMENT '内部计费物流方式',

    status_id            INT          NOT NULL COMMENT '状态#type:LogisticsModeStatus',
    inner_status_id      INT          NOT NULL COMMENT '状态#type:LogisticsModeStatus',

    create_time          DATETIME     NOT NULL COMMENT '创建时间',
    update_time          DATETIME     NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_mode_type_id (type_id),
    CONSTRAINT fk_logistics_mode_warehouse_id FOREIGN KEY (warehouse_id) REFERENCES tbl_warehouse (id),
    CONSTRAINT fk_logistics_mode_inner_mode_id FOREIGN KEY (inner_mode_id) REFERENCES tbl_logistics_mode (id)
) COMMENT '物流方式';