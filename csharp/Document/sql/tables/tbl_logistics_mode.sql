CREATE TABLE tbl_logistics_mode
(
    id                     INT            NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    type_id                INT            NOT NULL COMMENT '物流类型',
    code                   VARCHAR(100)   NOT NULL COMMENT '编码',
    name                   VARCHAR(200)   NOT NULL COMMENT '名称',
    description            VARCHAR(500)   NOT NULL COMMENT '描述',

    fee_rate_id            INT            NULL COMMENT '通用费率',
    inner_fee_rate_id      INT            NULL COMMENT '内部通用费率',
    customer_fee_rate_id   INT            NULL COMMENT '客户通用费率',
    common_fee_rate_id     INT            NULL COMMENT '普通类通用费率',
    restrict_fee_rate_id   INT            NULL COMMENT '限制类通用费率',
    bamboo_fee_rate_id     INT            NULL COMMENT '草木竹类通用费率',
    special_fee_rate_id    INT            NULL COMMENT '特货类通用费率',

    fee_cal_desc           VARCHAR(300)   NULL COMMENT '费用计算描述',

    warehouse_from_id      INT            NOT NULL COMMENT '发货仓库',
    warehouse_to_id        INT            NULL COMMENT '到货仓库',
    pickup_station_id      INT            NULL COMMENT '自提点',

    min_weight             DECIMAL(10, 3) NULL COMMENT '最小重量',
    max_weight             DECIMAL(10, 3) NULL COMMENT '最大重量',

    sync_tla               BOOL           NULL COMMENT '是否入选TLA',
    pack_print_type_id     INT            NULL COMMENT '打包标签',
    english_pack_print     BOOL           NULL COMMENT '英文标签打印',

    express_company_name   VARCHAR(100)   NULL COMMENT '快递公司名称',
    express_company_url    VARCHAR(300)   NULL COMMENT '快递公司网址',

    free_delivery_distance DECIMAL(10, 1) NULL     DEFAULT 30 COMMENT '免费送货距离', # 单位KM，一般用于送货上门免费送货距离，后来也用于澳洲快递

    status_id              INT            NOT NULL COMMENT '状态',
    inner_status_id        INT            NULL COMMENT '状态',

    create_time            DATETIME       NOT NULL COMMENT '创建时间',
    update_time            DATETIME       NOT NULL COMMENT '更新时间',

    INDEX ix_logistics_mode_type_id (type_id),
    INDEX ix_logistics_mode_status_id (status_id),
    FOREIGN KEY fk_logistics_mode_fee_rate_id (fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_inner_fee_rate_id (inner_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_customer_fee_rate_id (customer_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_common_fee_rate_id (common_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_restrict_fee_rate_id (restrict_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_bamboo_fee_rate_id (bamboo_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_special_fee_rate_id (special_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id),
    FOREIGN KEY fk_logistics_mode_warehouse_from_id (warehouse_from_id) REFERENCES tbl_warehouse (id),
    FOREIGN KEY fk_logistics_mode_warehouse_to_id (warehouse_to_id) REFERENCES tbl_warehouse (id)
) COMMENT '物流方式';


/*

ALTER TABLE tbl_logistics_mode
    MODIFY fee_rate_id INT NULL COMMENT '通用费率';

ALTER TABLE tbl_logistics_mode
    ADD common_fee_rate_id INT NULL COMMENT '普通类通用费率';

ALTER TABLE tbl_logistics_mode
    ADD restrict_fee_rate_id INT NULL COMMENT '限制类通用费率';

ALTER TABLE tbl_logistics_mode
    ADD bamboo_fee_rate_id INT NULL COMMENT '草木竹类通用费率';

ALTER TABLE tbl_logistics_mode
    ADD customer_fee_rate_id INT NULL COMMENT '客户通用费率';

ALTER TABLE tbl_logistics_mode
    ADD inner_fee_rate_id INT NULL COMMENT '内部通用费率';

*/

ALTER TABLE tbl_logistics_mode
    ADD FOREIGN KEY fk_logistics_mode_inner_fee_rate_id (inner_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id);

ALTER TABLE tbl_logistics_mode
    ADD FOREIGN KEY fk_logistics_mode_customer_fee_rate_id (customer_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id);

ALTER TABLE tbl_logistics_mode
    ADD FOREIGN KEY fk_logistics_mode_common_fee_rate_id (common_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id);

ALTER TABLE tbl_logistics_mode
    ADD FOREIGN KEY fk_logistics_mode_restrict_fee_rate_id (restrict_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id);

ALTER TABLE tbl_logistics_mode
    ADD FOREIGN KEY fk_logistics_mode_bamboo_fee_rate_id (bamboo_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id);

ALTER TABLE tbl_logistics_mode
    MODIFY customer_fee_rate_id INT NULL COMMENT '客户通用费率' AFTER fee_rate_id;

ALTER TABLE tbl_logistics_mode
    MODIFY inner_fee_rate_id INT NULL COMMENT '内部通用费率' AFTER customer_fee_rate_id;

ALTER TABLE tbl_logistics_mode
    MODIFY common_fee_rate_id INT NULL COMMENT '普通类通用费率' AFTER inner_fee_rate_id;

ALTER TABLE tbl_logistics_mode
    MODIFY restrict_fee_rate_id INT NULL COMMENT '限制类通用费率' AFTER common_fee_rate_id;

ALTER TABLE tbl_logistics_mode
    MODIFY bamboo_fee_rate_id INT NULL COMMENT '草木竹类通用费率' AFTER restrict_fee_rate_id;

ALTER TABLE tbl_logistics_mode
    ADD special_fee_rate_id INT NULL COMMENT '特货类通用费率' AFTER bamboo_fee_rate_id;

ALTER TABLE tbl_logistics_mode
    ADD FOREIGN KEY fk_logistics_mode_special_fee_rate_id (special_fee_rate_id) REFERENCES tbl_logistics_fee_rate (id);

ALTER TABLE tbl_logistics_mode
    ADD inner_status_id INT NULL COMMENT '状态' AFTER status_id;

ALTER TABLE tbl_logistics_mode
    ADD min_weight DECIMAL(10, 3) NULL COMMENT '最小重量' AFTER pickup_station_id;

ALTER TABLE tbl_logistics_mode
    ADD max_weight DECIMAL(10, 3) NULL COMMENT '最大重量' AFTER min_weight;

ALTER TABLE tbl_logistics_mode
    ADD sync_tla BOOL NULL COMMENT '是否入选TLA' AFTER max_weight;

ALTER TABLE tbl_logistics_mode
    ADD pack_print_type_id INT NULL COMMENT '打包标签' AFTER sync_tla;

ALTER TABLE tbl_logistics_mode
    ADD english_pack_print BOOL NULL COMMENT '英文标签打印' AFTER pack_print_type_id;

ALTER TABLE tbl_logistics_mode
    ADD express_company_name VARCHAR(100) NULL COMMENT '快递公司名称' AFTER english_pack_print;

ALTER TABLE tbl_logistics_mode
    ADD express_company_url VARCHAR(300) NULL COMMENT '快递公司网址' AFTER express_company_name;

ALTER TABLE tbl_logistics_mode
    ADD multi_express_one_pack BOOL NULL COMMENT '是否合箱' AFTER english_pack_print;

ALTER TABLE tbl_logistics_mode
    ADD cate_id INT NOT NULL DEFAULT 1000 COMMENT '类别' AFTER id;

ALTER TABLE tbl_logistics_mode
    ADD inner_mode_id INT NULL COMMENT '内部计费物流方式' AFTER express_company_url;

ALTER TABLE tbl_logistics_mode
    ADD CONSTRAINT fk_logistics_mode_inner_mode_id FOREIGN KEY fk_logistics_mode_inner_mode_id (inner_mode_id) REFERENCES tbl_logistics_mode (id);

ALTER TABLE tbl_logistics_mode
    ADD fee_cal_desc VARCHAR(300) NULL COMMENT '费用计算描述' AFTER special_fee_rate_id; # 主要用于空运已打包消息描述

ALTER TABLE tbl_logistics_mode
    ADD free_delivery_distance DECIMAL(10, 1) NULL DEFAULT 30 COMMENT '免费送货距离' AFTER express_company_url;

ALTER TABLE tbl_logistics_mode
    ADD pack_size_type_id INT NOT NULL DEFAULT 0 COMMENT '包裹大小件类型' AFTER type_id;