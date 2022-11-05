using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using shunda.Core;

namespace shunda.Domain;

public class SiteRequestLogInfoMapping : EntityNoCreateNoUpdateMapping<SiteRequestLogInfo>
{
    public SiteRequestLogInfoMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);

        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_site_request_log");

        Id(
            x => x.Id,
            map => {
                map.Column("id");
                map.Generator(Generators.Native);
            }
        );

        Property(
            x => x.SessionId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("session_id");
                map.Length(100);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UserId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("user_id");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.IpAddress,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("ip_address");
                map.Length(200);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Url,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("url");
                map.Length(500);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FormParams,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("form_params");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.TotalSeconds,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_seconds");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CreateTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("create_time");
                map.NotNullable(true);
            }
        );
    }
}