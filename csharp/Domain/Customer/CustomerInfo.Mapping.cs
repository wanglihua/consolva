using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerInfoMapping : EntityMapping<CustomerInfo>
{
    public CustomerInfoMapping()
    {
        Table("tbl_customer");

        Property(
            x => x.Country,
            map => {
                map.Type<CountryHibernateType>();
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OpenId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("open_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Mobile,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("mobile");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MobileValidated,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("mobile_validated");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Salt,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("salt");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Password,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("password");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayPassword,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("pay_password");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.RegTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("reg_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastLogin,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("last_login");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastIp,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("last_ip");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Account,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("account");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalScore,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_score");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ValidScore,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("valid_score");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalConsume,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_consume");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Level,
            map => {
                map.Type<CustomerLevelHibernateType>();
                map.Column("level_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.VipRank,
            map => {
                map.Type<CustomerVipRankHibernateType>();
                map.Column("vip_rank_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerCharacter,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("customer_character");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CreateMethod,
            map => {
                map.Type<CustomerCreateMethodHibernateType>();
                map.Column("create_method_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StatusId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}