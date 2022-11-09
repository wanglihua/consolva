using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Core;
using Domain.Customer;
using Domain.Pass;

namespace Domain.LogisticsOrder
{
    public class LogisticsOrderRefundApplyInfoMapping : ModelMapping<LogisticsOrderRefundApplyInfo>
    {
        public LogisticsOrderRefundApplyInfoMapping()
        {
            Table("tbl_logistics_order_refund_apply");

            ManyToOne(
                x => x.Order,
                map => {
                    map.Class(typeof(LogisticsOrderInfo));
                    map.Column("order_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            ManyToOne(
                x => x.Customer,
                map => {
                    map.Class(typeof(CustomerInfo));
                    map.Column("customer_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            Property(
                x => x.Type,
                map => {
                    map.Type<LogisticsOrderRefundTypeHibernateType>();
                    map.Column("type_id");
                    map.NotNullable(true);
                }
            );

            ManyToOne(
                x => x.Reason,
                map => {
                    map.Class(typeof(RefundReasonInfo));
                    map.Column("reason_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            Property(
                x => x.ReasonContent,
                map => {
                    map.Type(NHibernateUtil.String);
                    map.Column("reason");
                    map.Length(300);
                    map.NotNullable(true);
                }
            );

            Property(
                x => x.Amount,
                map => {
                    map.Type(NHibernateUtil.Decimal);
                    map.Column("amount");
                    map.NotNullable(true);
                }
            );

            Property(
                x => x.PayType,
                map => {
                    map.Type<LogisticsOrderRefundPayTypeHibernateType>();
                    map.Column("pay_type_id");
                    map.NotNullable(true);
                }
            );

            Property(
                x => x.Status,
                map => {
                    map.Type<LogisticsOrderRefundApplyStatusHibernateType>();
                    map.Column("status_id");
                    map.NotNullable(true);
                }
            );

            ManyToOne(
                x => x.ApplyUser,
                map => {
                    map.Class(typeof(UserInfo));
                    map.Column("apply_user_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            Property(
                x => x.ApplyTime,
                map => {
                    map.Type(NHibernateUtil.DateTime);
                    map.Column("apply_time");
                }
            );

            Property(
                x => x.ApplyMemo,
                map => {
                    map.Type(NHibernateUtil.String);
                    map.Column("apply_memo");
                    map.Length(300);
                }
            );

            ManyToOne(
                x => x.ApproveUser,
                map => {
                    map.Class(typeof(UserInfo));
                    map.Column("approve_user_id");
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            Property(
                x => x.ApproveTime,
                map => {
                    map.Type(NHibernateUtil.DateTime);
                    map.Column("approve_time");
                }
            );

            Property(
                x => x.ApproveMemo,
                map => {
                    map.Type(NHibernateUtil.String);
                    map.Column("approve_memo");
                    map.Length(300);
                }
            );
        }
    }
}