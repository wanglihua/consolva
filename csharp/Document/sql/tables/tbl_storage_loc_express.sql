CREATE TABLE tbl_storage_loc_express
(
    storage_loc_id INT NOT NULL COMMENT '仓位',
    shelf_id       INT NOT NULL COMMENT '货架',
    express_id     INT NOT NULL COMMENT '快递单',

    PRIMARY KEY pk_storage_loc_express (storage_loc_id, express_id),

    CONSTRAINT fk_storage_loc_express_express_storage_loc_id FOREIGN KEY (storage_loc_id) REFERENCES tbl_storage_loc (id),
    CONSTRAINT fk_storage_loc_express_express_shelf_id FOREIGN KEY (shelf_id) REFERENCES tbl_shelf (id),
    CONSTRAINT fk_storage_loc_express_express_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id)
);