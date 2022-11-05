# DROP TABLE IF EXISTS tbl_express_remark;

CREATE TABLE tbl_express_remark
(
    id          INT          NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',
    express_id  INT          NULL COMMENT '快递单号',
    order_id    INT          NULL COMMENT '订单', # express_id order_id 不能同时为空
    type_id     INT          NOT NULL COMMENT '备注类型#type:ExpressRemarkType',
    remark      VARCHAR(300) NOT NULL COMMENT '备注',
    add_user_id INT          NULL COMMENT '添加用户',

    create_time DATETIME     NOT NULL COMMENT '创建时间',
    update_time DATETIME     NOT NULL COMMENT '更新时间',

    CONSTRAINT fk_express_remark_express_id FOREIGN KEY (express_id) REFERENCES tbl_express (id),
    CONSTRAINT fk_express_remark_order_id FOREIGN KEY (order_id) REFERENCES tbl_logistics_order (id),
    CONSTRAINT fk_express_remark_add_user_id FOREIGN KEY (add_user_id) REFERENCES tbl_user (id)
) COMMENT '快递单备注';


# 订单图片，尺寸重量等都会采用类似的设计，就是跟订单的混在一起
# 这样设计的一个缺点是作废订单会取不到快递单号相关这些信息，这时可以换种取法通过订单的快递单号来取