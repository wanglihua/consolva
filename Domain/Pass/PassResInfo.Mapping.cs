using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Pass;

public class PassResInfoMapping : EntityNoCreateNoUpdateMapping<PassResInfo> {
    public PassResInfoMapping() {
        Table("tbl_pass_res");

        Id(
            x => x.Id,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("id");
                map.Generator(Generators.Assigned);
            }
        );

        ManyToOne(
            x => x.Cate,
            map => {
                map.Class(typeof(PassResCateInfo));
                map.Column("cate_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(100);
                map.NotNullable(true);
                map.Unique(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(300);
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