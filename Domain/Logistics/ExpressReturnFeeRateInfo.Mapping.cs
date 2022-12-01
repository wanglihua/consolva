using NHibernate;
using shunda.Core;

namespace shunda.Domain.Logistics;


    public class ExpressReturnFeeRateInfoMapping : EntityMapping<ExpressReturnFeeRateInfo>
    {
        public ExpressReturnFeeRateInfoMapping()
        {
            Table("tbl_express_return_fee_rate");

            Property(
                x => x.FeePerExpress,
                map => {
                    map.Type(NHibernateUtil.Decimal);
                    map.Column("fee_per_express");
                    map.NotNullable(true);
                }
            );

        }
    }