using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderActionInfoMapping : EntityNoUpdateMapping<LogisticsOrderActionInfo>
{
    public LogisticsOrderActionInfoMapping()
    {
        Table("tbl_logistics_order_action");

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
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsOrderActionTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderStatusBefore,
            map => {
                map.Type<LogisticsOrderStatusHibernateType>();
                map.Column("order_status_id_before");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderStatusAfter,
            map => {
                map.Type<LogisticsOrderStatusHibernateType>();
                map.Column("order_status_id_after");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayStatusBefore,
            map => {
                map.Type<PayStatusHibernateType>();
                map.Column("pay_status_id_before");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayStatusAfter,
            map => {
                map.Type<PayStatusHibernateType>();
                map.Column("pay_status_id_after");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ActionDesc,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("action_desc");
                map.NotNullable(true);
            }
        );

    }
}