# DROP TABLE IF EXISTS tbl_logistics_fee_rate;

CREATE TABLE tbl_logistics_fee_rate
(
    id                                   INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Id',

    name                                 VARCHAR(200)   NOT NULL COMMENT '名称',
    care_volume_weight                   BOOL           NOT NULL COMMENT '是否考虑体积重',
    volume_denominator                   INT            NULL COMMENT '体积重分母',
    care_side_sum                        BOOL           NOT NULL COMMENT '是否考虑三边之和',
    volume_weight_result_type_id         INT            NOT NULL COMMENT '体积重计算取向#type:VolumeWeightResultType',

    customer_length_discount             DECIMAL(10, 2) NOT NULL COMMENT '客户计算折扣 长',
    customer_width_discount              DECIMAL(10, 2) NOT NULL COMMENT '客户计算折扣 宽',
    customer_height_discount             DECIMAL(10, 2) NOT NULL COMMENT '客户计算折扣 高',
    customer_weight_discount             DECIMAL(10, 2) NOT NULL COMMENT '客户计算折扣 重',

    export_length_discount               DECIMAL(10, 2) NOT NULL COMMENT '导出计算折扣 长',
    export_width_discount                DECIMAL(10, 2) NOT NULL COMMENT '导出计算折扣 宽',
    export_height_discount               DECIMAL(10, 2) NOT NULL COMMENT '导出计算折扣 高',
    export_weight_discount               DECIMAL(10, 2) NOT NULL COMMENT '导出计算折扣 重',

    has_length_start                     BOOL           NOT NULL COMMENT '是否有起步长度',
    length_start                         INT            NULL COMMENT '起步长度',
    has_length_limit                     BOOL           NOT NULL COMMENT '是否有长度限制',
    length_limit                         INT            NULL COMMENT '长度限制',
    can_over_length                      BOOL           NOT NULL COMMENT '能否超长',
    over_length_step                     DECIMAL(10, 2) NULL COMMENT '超长阶梯',
    over_length_step_fee                 DECIMAL(10, 2) NULL COMMENT '超长阶梯收费',
    over_length_fee_every_pack           BOOL           NOT NULL COMMENT '是否按件计费',
    has_over_length_cap_amount           BOOL           NOT NULL COMMENT '是否有超长封顶金额',
    over_length_cap_amount               DECIMAL(10, 2) NULL COMMENT '超长封顶金额',

    has_single_weight_start              BOOL           NOT NULL COMMENT '是否有单箱起步重量',
    single_weight_start                  INT            NULL COMMENT '单箱起步重量',
    has_single_weight_limit              BOOL           NOT NULL COMMENT '是否有单箱重量限制',
    single_weight_limit                  INT            NULL COMMENT '单箱重量限制',
    can_single_over_weight               BOOL           NOT NULL COMMENT '单箱能否超重',
    single_over_weight_step              DECIMAL(10, 2) NULL COMMENT '单箱超重阶梯',
    single_over_weight_step_fee          DECIMAL(10, 2) NULL COMMENT '单箱超重阶梯收费',
    has_single_over_weight_cap_amount    BOOL           NOT NULL COMMENT '是否有单箱超重封顶金额',
    single_over_weight_cap_amount        DECIMAL(10, 2) NULL COMMENT '单箱超重封顶金额',

    has_average_weight_start             BOOL           NOT NULL COMMENT '是否有平均单箱起步重量',
    average_weight_start                 INT            NULL COMMENT '平均单箱起步重量',
    has_average_weight_limit             BOOL           NOT NULL COMMENT '是否有平均单箱重量限制',
    average_weight_limit                 INT            NULL COMMENT '平均单箱重量限制',
    can_average_over_weight              BOOL           NOT NULL COMMENT '平均单箱能否超重',
    average_over_weight_step             DECIMAL(10, 2) NULL COMMENT '平均单箱超重阶梯',
    average_over_weight_step_fee         DECIMAL(10, 2) NULL COMMENT '平均单箱超重阶梯收费',
    has_average_over_weight_cap_amount   BOOL           NOT NULL COMMENT '是否有平均单箱超重封顶金额',
    average_over_weight_cap_amount       DECIMAL(10, 2) NULL COMMENT '平均单箱超重封顶金额',

    has_weight_start                     BOOL           NOT NULL COMMENT '是否有起步重量',
    weight_start                         INT            NULL COMMENT '起步重量',
    has_weight_limit                     BOOL           NOT NULL COMMENT '是否有重量限制',
    weight_limit                         INT            NULL COMMENT '重量限制',
    can_over_weight                      BOOL           NOT NULL COMMENT '能否超重',
    over_weight_step                     DECIMAL(10, 2) NULL COMMENT '超重阶梯',
    over_weight_step_fee                 DECIMAL(10, 2) NULL COMMENT '超重阶梯收费',
    has_over_weight_cap_amount           BOOL           NOT NULL COMMENT '是否有超重封顶金额',
    over_weight_cap_amount               DECIMAL(10, 2) NULL COMMENT '超重封顶金额',
    volume_actual_weight_middle          BOOL           NOT NULL COMMENT '体积重实际重折中',

    care_volume_actual_weight_ratio      BOOL           NOT NULL COMMENT '是否考虑体积重实际重倍率',
    volume_weight_care_max_side_length   BOOL           NOT NULL COMMENT '体积重考虑最长边',
    volume_weight_max_side_length_begin  DECIMAL(10, 2) NULL COMMENT '体积重最长边起始',

    min_volume_actual_weight_ratio       DECIMAL(10, 2) NULL COMMENT '体积重实际重最小倍率',
    care_goods_volume_price              BOOL           NOT NULL COMMENT '是否启用实际货值与体积货值最大值计算',
    goods_volume_price                   DECIMAL(10, 2) NULL COMMENT '体积货值设定',

    has_free_package_limit               BOOL           NOT NULL COMMENT '是否有免费包裹数量限制',
    free_package_limit                   INT            NULL COMMENT '免费包裹数量限制',
    extra_package_fee                    DECIMAL(10, 2) NULL COMMENT '额外包裹收费',
    has_extra_package_fee_cap_amount     BOOL           NOT NULL COMMENT '是否有额外包裹收费封顶金额',
    extra_package_fee_cap_amount         DECIMAL(10, 2) NULL COMMENT '额外包裹收费封顶金额',

    has_free_storage_day_limit           BOOL           NOT NULL COMMENT '是否有免费仓储天数限制',
    free_storage_day_limit               INT            NULL COMMENT '免费仓储天数限制',
    extra_storage_day_fee                DECIMAL(10, 2) NULL COMMENT '额外仓储收费',
    extra_storage_day_min_fee_per_day    DECIMAL(10, 2) NULL COMMENT '最低仓储收费',
    has_extra_storage_day_fee_cap_amount BOOL           NOT NULL COMMENT '是否有额外仓储收费封顶金额',
    extra_storage_day_fee_cap_amount     DECIMAL(10, 2) NULL COMMENT '额外仓储收费封顶金额',

    publish_rate                         DECIMAL(10, 2) NOT NULL COMMENT '公布系数',

    status_id                            INT            NOT NULL COMMENT '状态#type:LogisticsFeeRateStatus',
    create_time                          DATETIME       NOT NULL COMMENT '创建时间',
    update_time                          DATETIME       NOT NULL COMMENT '更新时间'
) COMMENT '物流费率';
