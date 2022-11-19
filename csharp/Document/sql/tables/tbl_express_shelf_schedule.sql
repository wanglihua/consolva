# DROP TABLE tbl_express_shelf_schedule;

CREATE TABLE tbl_express_shelf_schedule
(
    id               INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    express_id       INT      NOT NULL COMMENT '快递单',
    user_id          INT      NOT NULL COMMENT '上架人',
    schedule_type_id INT      NOT NULL COMMENT '排班类型#type:ExpressShelfScheduleType',

    create_time      DATETIME NOT NULL COMMENT '创建时间',
    update_time      DATETIME NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_express_shelf_schedule_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_shelf_schedule_user_id FOREIGN KEY (user_id) REFERENCES tbl_user (id),

    UNIQUE INDEX ix_express_shelf_schedule_c_s (express_id, schedule_type_id)

) COMMENT '快递单上下架排班';

