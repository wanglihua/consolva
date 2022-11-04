using NHibernate;
using shunda.Core;

namespace shunda.Domain;

public class ExchangeRateInfoMapping : EntityMapping<ExchangeRateInfo>
{
    public ExchangeRateInfoMapping()
    {
        Table("tbl_exchange_rate");

        Property(
            x => x.FromCurrencyCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("from_currency_code");
                map.Length(10);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FromCurrencyName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("from_currency_name");
                map.Length(10);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ToCurrencyCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("to_currency_code");
                map.Length(10);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ToCurrencyName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("to_currency_name");
                map.Length(10);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Rate,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("rate");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.RateFix,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("rate_fix");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.RefreshMark,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("refresh_mark");
                map.NotNullable(true);
            }
        );
    }
}