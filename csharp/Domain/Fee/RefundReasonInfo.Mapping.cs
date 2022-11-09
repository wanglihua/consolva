using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Core;

namespace Domain.LogisticsOrder
{
    public class RefundReasonInfoMapping : ModelMapping<RefundReasonInfo>
    {
        public RefundReasonInfoMapping()
        {
            Table("tbl_refund_reason");

            Property(
                x => x.Content,
                map => {
                    map.Type(NHibernateUtil.String);
                    map.Column("content");
                    map.Length(300);
                    map.NotNullable(true);
                }
            );

            Property(
                x => x.Status,
                map => {
                    map.Type<RefundReasonStatusHibernateType>();
                    map.Column("status_id");
                    map.NotNullable(true);
                }
            );
        }
    }
}