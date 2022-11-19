# DROP TABLE tbl_express_dump_schedule;

CREATE TABLE tbl_express_dump_schedule
(
    id           INT      NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',

    express_id   INT      NOT NULL COMMENT '快递单',
    user_id      INT      NOT NULL COMMENT '堆存人',
    dump_type_id INT      NOT NULL COMMENT '堆存类型#type:ExpressDumpType',

    create_time  DATETIME NOT NULL COMMENT '创建时间',
    update_time  DATETIME NOT NULL COMMENT '更新时间',

    FOREIGN KEY fk_express_dump_schedule_express_id (express_id) REFERENCES tbl_express (id),
    FOREIGN KEY fk_express_dump_schedule_user_id (user_id) REFERENCES tbl_user (id),

    UNIQUE INDEX ix_express_dump_schedule_c_s (express_id, dump_type_id)

) COMMENT '快递堆存排班';
