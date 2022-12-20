using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderCreateSelectInfoMapping : ClassMapping<LogisticsOrderCreateSelectInfo> {
    public LogisticsOrderCreateSelectInfoMapping() {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_logistics_order_create_select");

        Id(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.Generator(Generators.Assigned);
            }
        );

        Property(
            x => x.GoodsType,
            map => {
                map.Type<LogisticsGoodsTypeHibernateType>();
                map.Column("goods_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Country,
            map => {
                map.Type<CountryHibernateType>();
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FirstType,
            map => {
                map.Type<LogisticsModeTypeHibernateType>();
                map.Column("first_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FirstModeId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("first_mode_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AddressReceiveId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("address_receive_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AddressSendId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("address_send_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.InsuranceId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("insurance_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.GstId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("gst_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CouponId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("coupon_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_fee");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Valid,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("valid");
                map.NotNullable(true);
            }
        );
    }
}