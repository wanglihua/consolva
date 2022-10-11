using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;

namespace shunda.Domain.Order;

public class LogisticsOrderSendAddressInfoMapping : EntityMapping<LogisticsOrderSendAddressInfo>
{
    public LogisticsOrderSendAddressInfoMapping()
    {
        Table("tbl_logistics_order_send_address");

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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
            x => x.FirstName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("first_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("last_name");
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

        ManyToOne(
            x => x.Province,
            map => {
                map.Class(typeof(AddressProvinceInfo));
                map.Column("province_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.City,
            map => {
                map.Class(typeof(AddressCityInfo));
                map.Column("city_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Area,
            map => {
                map.Class(typeof(AddressAreaInfo));
                map.Column("area_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Street,
            map => {
                map.Class(typeof(AddressStreetInfo));
                map.Column("street_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Address,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("address");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Zipcode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("zipcode");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Tfn,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tfn");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Company,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("company");
                map.NotNullable(false);
            }
        );

    }
}