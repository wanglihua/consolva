using NHibernate;
using shunda.Core;

namespace shunda.Domain;

public class SiteErrorInfoMapping : EntityMapping<SiteErrorInfo>
{
    public SiteErrorInfoMapping()
    {
        Table("tbl_site_error");

        Property(
            x => x.Type,
            map => {
                map.Type<SiteErrorTypeHibernateType>();
                map.Column("error_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SubType,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("error_sub_type");
                map.Length(100);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ErrorMsg,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("error_msg");
                map.Length(500);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Resolved,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("resolved");
                map.NotNullable(true);
            }
        );
    }
}