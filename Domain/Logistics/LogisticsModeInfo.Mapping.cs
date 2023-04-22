using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Domain.Logistics;

public class LogisticsModeInfoMapping : EntityMapping<LogisticsModeInfo> {
    public LogisticsModeInfoMapping() {
        Table("tbl_logistics_mode");

        Property(
            x => x.Country,
            map => {
                map.Type<CountryHibernateType>();
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Cate,
            map => {
                map.Type<LogisticsModeCateHibernateType>();
                map.Column("cate_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsModeTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PackSizeType,
            map => {
                map.Type<LogisticsModePackSizeTypeHibernateType>();
                map.Column("pack_size_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.Length(100);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(200);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(500);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FeeCalDesc,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("fee_calc_desc");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Promotion,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("promotion");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Warehouse,
            map => {
                map.Class(typeof(WarehouseInfo));
                map.Column("warehouse_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.PackLabelType,
            map => {
                map.Type<LogisticsPackLabelTypeHibernateType>();
                map.Column("pack_label_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Channel,
            map => {
                map.Type<LogisticsModeChannelHibernateType>();
                map.Column("channel_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExpressCompanyName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_company_name");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExpressCompanyUrl,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_company_url");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.FeeType,
            map => {
                map.Type<LogisticsModeFeeTypeHibernateType>();
                map.Column("fee_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.InnerModeId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("inner_mode_id");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsModeStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.InnerStatus,
            map => {
                map.Type<LogisticsModeStatusHibernateType>();
                map.Column("inner_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_num");
                map.NotNullable(true);
            }
        );
    }
}