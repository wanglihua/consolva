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
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_coupon_batch_pick");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UseAgreementHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("use_agreement_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.InsuranceClauseHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("insurance_clause_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FreightDescHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("freight_desc_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderProcessHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("order_process_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PromotionHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("promotion_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StorageInstructionHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("storage_instruction_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TransportLimitHasNew,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("transport_limit_has_new");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.HasFollowed,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_followed");
                map.NotNullable(true);
            }
        );
    }
}