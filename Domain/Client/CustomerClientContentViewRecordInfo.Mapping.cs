using shunda.Core;
using NHibernate;

namespace shunda.Domain.Client;

public class CustomerClientContentViewRecordInfoMapping : EntityMapping<CustomerClientContentViewRecordInfo> {
    public CustomerClientContentViewRecordInfoMapping() {
        Table("tbl_customer_client_content_view_record");

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<ClientContentTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.RelatedId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("related_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastReadTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("last_read_time");
                map.NotNullable(true);
            }
        );

    }
}