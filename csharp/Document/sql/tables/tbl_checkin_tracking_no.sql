# DROP TABLE IF EXISTS tbl_checkin_tracking_no;

CREATE TABLE tbl_checkin_tracking_no
(
    id          INT           NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'id',
    year        INT           NOT NULL COMMENT '年份',
    month       INT           NOT NULL COMMENT '月份',
    seq         INT           NOT NULL COMMENT '序列号',

    create_time DATETIME      NOT NULL COMMENT '创建时间',
    update_time DATETIME      NOT NULL COMMENT '更新时间',

    INDEX ix_checkin_tracking_no_year_month (year, month)
) COMMENT '入库跟踪单号';

