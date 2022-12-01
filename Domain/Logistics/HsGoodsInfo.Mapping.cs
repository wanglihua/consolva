using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Logistics;

public class HsGoodsInfoMapping : EntityMapping<HsGoodsInfo>
{
    public HsGoodsInfoMapping()
    {
        Table("tbl_hs_goods");

        Property(
            x => x.GoodsType,
            map => {
                map.Type<LogisticsGoodsTypeHibernateType>();
                map.Column("goods_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CnHsCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("cn_hs_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CnHsName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("cn_hs_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.EnHsCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("en_hs_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.EnHsName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("en_hs_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem01,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem01");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem02,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem02");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem03,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem03");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem04,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem04");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem05,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem05");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem06,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem06");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem07,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem07");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Elem08,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("elem08");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.AddUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("add_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

    }
}