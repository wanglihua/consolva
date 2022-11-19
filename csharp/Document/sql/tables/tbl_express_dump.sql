CREATE TABLE tbl_express_dump
(
    id                  INT         NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    type_id             INT         NOT NULL COMMENT '类型#type:ExpressDumpType',
    express_id          INT         NOT NULL COMMENT '申领件',
    tracking_no         VARCHAR(30) NOT NULL COMMENT '快递单号',
    dump_user_id        INT         NOT NULL COMMENT '堆存用户',
    dump_time           DATETIME    NOT NULL COMMENT '堆存时间',
    shelf_id            INT         NOT NULL COMMENT '堆存货架',
    storage_loc_id      INT         NOT NULL COMMENT '堆存库位',
    stock_check_user_id INT         NULL COMMENT '盘点用户',
    stock_check_time    DATETIME    NULL COMMENT '盘点时间',

    create_time         DATETIME    NOT NULL COMMENT '创建时间',
    update_time         DATETIME    NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_express_dump_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_dump_dump_user_id FOREIGN KEY (dump_user_id) REFERENCES tbl_user (id),
    CONSTRAINT fk_express_dump_shelf_id FOREIGN KEY (shelf_id) REFERENCES tbl_shelf (id),
    CONSTRAINT fk_express_dump_storage_loc_id FOREIGN KEY (storage_loc_id) REFERENCES tbl_storage_loc (id),
    CONSTRAINT fk_express_dump_stock_check_user_id FOREIGN KEY (stock_check_user_id) REFERENCES tbl_user (id)
) COMMENT '快递堆存';


