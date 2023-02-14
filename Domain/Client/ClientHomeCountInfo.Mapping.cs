using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Client;

public class ClientHomeCountInfoMapping : ClassMapping<ClientHomeCountInfo> {
    public ClientHomeCountInfoMapping() {
        Id(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.Generator(Generators.Assigned);
            }
        );

        Property(
            x => x.LastViewMessageId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("last_view_message_id");
                map.NotNullable(true);
            }
        );
    }
}


/*

SELECT p_customer_id                   AS customer_id,
       v_express_claim_count           AS express_claim_count,
       v_express_expect_count          AS express_expect_count,
       v_express_expect_enter_count    AS express_expect_enter_count,
       v_express_checkin_count         AS express_checkin_count,
       v_express_return_count          AS express_return_count,
       v_order_pay_count               AS order_pay_count,
       v_order_submitted_count         AS order_submitted_count,
       v_order_waiting_transport_count AS order_waiting_transport_count,
       v_order_transporting_count      AS order_transporting_count,
       v_order_delivered_count         AS order_delivered_count,
       v_new_message_count             AS new_message_count,
       v_coupon_batch_id IS NOT NULL   AS has_coupon_batch_pick;
           
 */