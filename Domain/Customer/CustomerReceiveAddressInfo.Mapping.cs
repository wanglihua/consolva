using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Customer;

public class CustomerReceiveAddressInfoMapping : EntityMapping<CustomerReceiveAddressInfo>
{
    public CustomerReceiveAddressInfoMapping()
    {
        Table("tbl_customer_receive_address");

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
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

        Property(
            x => x.State,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("state");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.City,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("city");
                map.NotNullable(true);
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
            x => x.UnitLoc,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("unit_loc");
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
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Company,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("company");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.IsDefault,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("is_default");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<CustomerAddressStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}