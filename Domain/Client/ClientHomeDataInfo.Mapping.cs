using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Client;

public class ClientHomeCountInfoMapping : ClassMapping<ClientHomeDataInfo> {
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
            x => x.ExpressClaimCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("express_claim_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExpressExpectCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("express_expect_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExpressExpectEnterCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("express_expect_enter_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExpressCheckinCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("express_checkin_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExpressReturnCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("express_return_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderPayCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_pay_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderSubmittedCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_submitted_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderWaitingTransportCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_waiting_transport_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderTransportingCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_transporting_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderDeliveredCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_delivered_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.NewMessageCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("new_message_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.HasCouponBatchPick,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("has_coupon_batch_pick");
                map.NotNullable(true);
            }
        );
    }
}