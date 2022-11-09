using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Core;
using Domain.Customer;

namespace Domain.LogisticsOrder
{
    public class LogisticsOrderRefundInfoMapping : ModelMapping<LogisticsOrderRefundInfo>
    {
        public LogisticsOrderRefundInfoMapping()
        {
            Table("tbl_logistics_order_refund");

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
                    map.Class(typeof(FeeRefundReasonInfo));
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
                    map.Type<LogisticsOrderRefundStatusHibernateType>();
                    map.Column("status_id");
                    map.NotNullable(true);
                }
            );

            ManyToOne(
                x => x.Apply,
                map => {
                    map.Class(typeof(LogisticsOrderRefundApplyInfo));
                    map.Column("apply_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );
        }
    }
}