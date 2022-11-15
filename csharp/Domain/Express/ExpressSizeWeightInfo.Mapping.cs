using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

public class ExpressSizeWeightInfoMapping : EntityMapping<ExpressSizeWeightInfo>
{
    public ExpressSizeWeightInfoMapping()
    {
        Table("tbl_express_size_weight");

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TrackingNoSub,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no_sub");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Length,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Width,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("width");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Height,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("height");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Weight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PackageCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("package_count");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Machine,
            map => {
                map.Class(typeof(AutoMeasureMachineInfo));
                map.Column("machine_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

    }
}