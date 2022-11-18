/*

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS tbl_express;

SET FOREIGN_KEY_CHECKS = 1;

 */

CREATE TABLE tbl_express
(
    id          INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    tracking_no VARCHAR(30) NOT NULL COMMENT '快递单号',
    order_id    INT         NULL COMMENT '物流订单',
    order_code VARCHAR(100) DEFAULT '' NOT NULL COMMENT '订单编号',
    customer_id INT         NULL COMMENT '客户',

    create_time DATETIME    NOT NULL COMMENT '创建时间',
    update_time DATETIME    NOT NULL COMMENT '更新时间',

    UNIQUE INDEX ix_express_tracking_no (tracking_no),
    CONSTRAINT fk_express_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_customer_id FOREIGN KEY (customer_id) REFERENCES tbl_customer (id)
) COMMENT '商品快递';


/*
 
 
CREATE TABLE tbl_logistics_claim_goods
(
    id                     INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    tracking_no            VARCHAR(30)  NOT NULL COMMENT '快递单号',
    old_tracking_no        VARCHAR(30)  NULL COMMENT '原快递单号',
    express_company        VARCHAR(100) NULL COMMENT '快递公司',
    description            VARCHAR(500) NULL COMMENT '申领件描述',
    picked                 BOOL         NOT NULL COMMENT '是否有被领取',
    pick_time              DATETIME     NULL COMMENT '领取时间',
    service_remark         VARCHAR(500) NULL COMMENT '客服备注',
    warehouse_processed    BOOL         NOT NULL COMMENT '是否仓库已处理',
    warehouse_process_time DATETIME     NULL COMMENT '仓库处理时间',
    warehouse_remark       VARCHAR(500) NULL COMMENT '仓库处理备注',
    user_id                INT          NOT NULL COMMENT '操作用户',
    add_time               DATETIME     NOT NULL COMMENT '添加时间',
    bill_customer_code     VARCHAR(50)  NULL COMMENT '面单客户',
    bill_customer_id       INT          NOT NULL COMMENT '面单客户Id',              # 这个值可以为零
    customer_id            INT          NULL COMMENT '认领客户',
    order_id               INT          NULL COMMENT '认领订单',
    storage_loc_id         INT          NOT NULL COMMENT '库位',
    express_package_count  INT          NOT NULL COMMENT '包裹数量',
    ignored                BOOL         NOT NULL DEFAULT FALSE COMMENT '忽略',
    goods_type_id          INT          NOT NULL DEFAULT 100 COMMENT '物品类型',
    status_id              INT          NOT NULL DEFAULT 100 COMMENT '状态',
    wood_frame             BOOL         NULL COMMENT '打木架',
    bark_clean             BOOL         NULL COMMENT '清理树皮杂物',
    return_tracking_no     VARCHAR(30)  NULL COMMENT '退运快递单号',
    return_express_company VARCHAR(100) NULL COMMENT '退运快递公司',
    return_time            DATETIME     NULL COMMENT '退运时间',
    dumped                 BOOL         NOT NULL DEFAULT FALSE COMMENT '是否已堆存', # 有了 dump_status_id 这个字段其实也不怎么需要了，但还保留着，让原来的逻辑继续生效
    dump_status_id         INT          NOT NULL DEFAULT 100 COMMENT '堆存状态',

    create_time            DATETIME     NOT NULL COMMENT '创建时间',
    update_time            DATETIME     NOT NULL COMMENT '更新时间',
    version                INT          NOT NULL DEFAULT 1 COMMENT '数据版本',

    INDEX ix_logistics_claim_goods_tracking_no (tracking_no),
    INDEX ix_logistics_claim_goods_picked (picked),
    INDEX ix_logistics_claim_goods_bill_customer_code (bill_customer_code),
    INDEX ix_logistics_claim_goods_bill_customer_id (bill_customer_id),
    INDEX ix_logistics_claim_goods_warehouse_processed (warehouse_processed),
    INDEX ix_logistics_claim_goods_goods_type_id (goods_type_id),
    INDEX ix_logistics_claim_goods_status_id (status_id),
    INDEX ix_logistics_claim_goods_wood_frame (wood_frame),
    INDEX ix_logistics_claim_goods_bark_clean (bark_clean),
    INDEX ix_logistics_claim_goods_return_tracking_no (return_tracking_no),
    FOREIGN KEY fk_logistics_claim_goods_user_id (user_id) REFERENCES tbl_user (id),
    FOREIGN KEY fk_logistics_claim_goods_customer_id (customer_id) REFERENCES tbl_customer (id),
    FOREIGN KEY fk_logistics_claim_goods_order_id (order_id) REFERENCES tbl_logistics_order (id)
) COMMENT '申领件';

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_goods_storage_loc_id (storage_loc_id) REFERENCES tbl_storage_loc (id);

SELECT *
FROM tbl_logistics_claim_goods cg
WHERE cg.storage_loc_id NOT IN (SELECT sl.id FROM tbl_storage_loc sl);

ALTER TABLE tbl_logistics_claim_goods
    ADD old_tracking_no VARCHAR(30) NULL COMMENT '原快递单号' AFTER tracking_no;

ALTER TABLE tbl_logistics_claim_goods
    MODIFY description VARCHAR(500) NULL COMMENT '描述';

ALTER TABLE tbl_logistics_claim_goods
    ADD express_package_count INT NOT NULL COMMENT '包裹数量' AFTER storage_loc_id;


ALTER TABLE tbl_logistics_claim_goods
    CHANGE customer_code bill_customer_code VARCHAR(50) NULL COMMENT '面单客户';

ALTER TABLE tbl_logistics_claim_goods
    ADD bill_customer_id INT NOT NULL COMMENT '面单客户Id' AFTER bill_customer_code;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_bill_customer_id (bill_customer_id);

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_bill_customer_code (bill_customer_code);

ALTER TABLE tbl_logistics_claim_goods
    ADD warehouse_id INT NULL COMMENT '仓库' AFTER order_id;

ALTER TABLE tbl_logistics_claim_goods
    MODIFY storage_loc_id INT NULL COMMENT '库位';

UPDATE tbl_logistics_claim_goods lcg
SET lcg.warehouse_id = (SELECT s.warehouse_id
                        FROM tbl_storage_loc sl
                                 INNER JOIN tbl_shelf s ON sl.shelf_id = s.id
                        WHERE sl.id = lcg.storage_loc_id)
WHERE 1 = 1;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_goods_warehouse_id (warehouse_id) REFERENCES tbl_warehouse (id);

UPDATE tbl_logistics_claim_goods
SET storage_loc_id = NULL
WHERE storage_loc_id = 0;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_goods_storage_loc_id (storage_loc_id) REFERENCES tbl_storage_loc (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD process_user_id INT NULL COMMENT '处理人' AFTER warehouse_processed;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_goods_process_user_id (process_user_id) REFERENCES tbl_user (id);

UPDATE tbl_logistics_claim_goods lcg
SET lcg.process_user_id = lcg.user_id
WHERE lcg.warehouse_processed = TRUE;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_add_time (add_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_pick_time (pick_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_warehouse_process_time (warehouse_process_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD ignored BOOL NOT NULL DEFAULT FALSE COMMENT '忽略' AFTER express_package_count;

ALTER TABLE tbl_logistics_claim_goods
    DROP FOREIGN KEY tbl_logistics_claim_goods_ibfk_3;

ALTER TABLE tbl_logistics_claim_goods
    ADD goods_type_id INT NOT NULL DEFAULT 100 COMMENT '物品类型' AFTER ignored;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_goods_type_id (goods_type_id);

ALTER TABLE tbl_logistics_claim_goods
    ADD status_id INT NOT NULL DEFAULT 100 COMMENT '状态' AFTER goods_type_id;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_status_id (status_id);

ALTER TABLE tbl_logistics_claim_goods
    ADD ao_pin_tracking_no VARCHAR(15) NULL COMMENT '澳拼跟踪号',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    MODIFY ao_pin_tracking_no VARCHAR(20) NULL COMMENT '澳拼跟踪号';

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_ao_pin_tracking_no (ao_pin_tracking_no);

ALTER TABLE tbl_logistics_claim_goods
    DROP INDEX ix_logistics_claim_goods_ao_pin_tracking_no;

ALTER TABLE tbl_logistics_claim_goods
    ADD UNIQUE INDEX ix_logistics_claim_goods_ao_pin_tracking_no (ao_pin_tracking_no);

# 这个语句其实可以不运行，默认就是100
UPDATE tbl_logistics_claim_goods cg
SET cg.status_id = 100
WHERE cg.picked = FALSE;

# 这个可以多次重复运行
UPDATE tbl_logistics_claim_goods cg
SET cg.status_id = 400
WHERE cg.picked = TRUE
  AND cg.status_id = 100 # 这个判断不能少，防止Order(400)后续状态会被错误的覆盖掉
  AND cg.order_id IS NOT NULL;

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame BOOL NULL COMMENT '打木架',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD bark_clean BOOL NULL COMMENT '清理树皮杂物',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame_customer BOOL NULL COMMENT '打木架会员选择',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD bark_clean_customer BOOL NULL COMMENT '清理树皮杂物会员选择',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame_user_id INT NULL COMMENT '打木架用户',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_wood_frame_user_id (wood_frame_user_id) REFERENCES tbl_user (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD bark_clean_user_id INT NULL COMMENT '清理树皮杂物用户',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_bark_clean_user_id (bark_clean_user_id) REFERENCES tbl_user (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame_time DATETIME NULL COMMENT '打木架时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_wft (wood_frame_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD bark_clean_time DATETIME NULL COMMENT '清理树皮杂物时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_bct (bark_clean_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD return_tracking_no VARCHAR(30) NULL COMMENT '退运快递单号',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD return_express_company VARCHAR(100) NULL COMMENT '退运快递公司',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD return_time DATETIME NULL COMMENT '退运时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_wood_frame (wood_frame);

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_bark_clean (bark_clean);

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_return_tracking_no (return_tracking_no);

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame_done BOOL NULL COMMENT '木架已打',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD bark_clean_done BOOL NULL COMMENT '树皮杂物已清理',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_wood_frame_done (wood_frame_done);

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_bark_clean_done (bark_clean_done);

ALTER TABLE tbl_logistics_claim_goods
    ADD shelf_status_id INT NULL COMMENT '快递单货架状态', # 已上架，已下架
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD on_shelf_time DATETIME NULL COMMENT '上架时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_on_shelf_time (on_shelf_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD on_shelf_user_id INT NULL COMMENT '上架用户',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_on_shelf_user_id (on_shelf_user_id) REFERENCES tbl_user (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD off_shelf_time DATETIME NULL COMMENT '下架时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_off_shelf_time (off_shelf_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD off_shelf_user_id INT NULL COMMENT '下架用户',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_off_shelf_user_id (off_shelf_user_id) REFERENCES tbl_user (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD return_apply_time DATETIME NULL COMMENT '退运申请时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD return_pickup_code VARCHAR(20) NULL COMMENT '退运取件码',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD dumped BOOL NOT NULL DEFAULT FALSE COMMENT '是否已堆存';

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logisitcs_claim_goods_dumped (dumped);

ALTER TABLE tbl_logistics_claim_goods
    ADD dump_status_id INT NOT NULL DEFAULT 100 COMMENT '堆存状态'; # AFTER dumped;

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_dump_status_id (dump_status_id);

ALTER TABLE tbl_logistics_claim_goods
    ADD total_weight DECIMAL(10, 3) NOT NULL DEFAULT 0 COMMENT '总重量'; # KG

ALTER TABLE tbl_logistics_claim_goods
    ADD total_volume DECIMAL(14, 6) NOT NULL DEFAULT 0 COMMENT '总体积'; # 立方米

ALTER TABLE tbl_logistics_claim_goods
    ADD packing_time DATETIME NULL COMMENT '入流动仓时间',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD packing_user_id INT NULL COMMENT '入流动仓用户',
    ALGORITHM = INSTANT;

ALTER TABLE tbl_logistics_claim_goods
    ADD FOREIGN KEY fk_logistics_claim_goods_packing_user_id (packing_user_id) REFERENCES tbl_user (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD archive_type_id INT NOT NULL DEFAULT 0 COMMENT '归档类型';

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_archive_id (archive_type_id);

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame_add_mode INT NOT NULL DEFAULT 100 COMMENT '打木架添加模式:手工还是入库';

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_wood_frame_add_mode (wood_frame_add_mode);

ALTER TABLE tbl_logistics_claim_goods
    ADD bark_clean_add_mode INT NOT NULL DEFAULT 100 COMMENT '清理树皮添加模式: 手工还是入库';

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_bark_clean_add_mode (bark_clean_add_mode);

ALTER TABLE tbl_logistics_claim_goods
    ADD checkin_warehouse_id INT NULL COMMENT '入库仓库';

ALTER TABLE tbl_logistics_claim_goods
    ADD CONSTRAINT fk_logistics_claim_goods_checkin_w_id FOREIGN KEY (checkin_warehouse_id) REFERENCES tbl_warehouse (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD checkin_storage_loc_id INT NULL COMMENT '入库仓位';

ALTER TABLE tbl_logistics_claim_goods
    ADD CONSTRAINT fk_logistics_claim_goods_checkin_loc_id FOREIGN KEY (checkin_storage_loc_id) REFERENCES tbl_storage_loc (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD dump_user_id INT NULL COMMENT '堆存用户';

ALTER TABLE tbl_logistics_claim_goods
    ADD CONSTRAINT fk_logistics_claim_goods_dump_user_id FOREIGN KEY (dump_user_id) REFERENCES tbl_user (id);

ALTER TABLE tbl_logistics_claim_goods
    ADD dump_time DATETIME NULL COMMENT '堆存时间';

ALTER TABLE tbl_logistics_claim_goods
    ADD INDEX ix_logistics_claim_goods_dump_time (dump_time);

ALTER TABLE tbl_logistics_claim_goods
    ADD customer_remark VARCHAR(500) NULL COMMENT '客户备注';

ALTER TABLE tbl_logistics_claim_goods
    ADD wood_frame_customer_remark VARCHAR(300) NULL COMMENT '打木架客户备注';

# 删除不必要的索引

DROP INDEX ix_logistics_claim_goods_picked ON tbl_logistics_claim_goods;
DROP INDEX ix_logistics_claim_goods_warehouse_processed ON tbl_logistics_claim_goods;
DROP INDEX ix_logistics_claim_goods_wood_frame ON tbl_logistics_claim_goods;
DROP INDEX ix_logistics_claim_goods_bark_clean ON tbl_logistics_claim_goods;
DROP INDEX ix_logistics_claim_goods_wood_frame_done ON tbl_logistics_claim_goods;
DROP INDEX ix_logistics_claim_goods_bark_clean_done ON tbl_logistics_claim_goods;
DROP INDEX ix_logisitcs_claim_goods_dumped ON tbl_logistics_claim_goods;

 
 */