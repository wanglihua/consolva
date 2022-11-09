using System.Collections.Generic;
using Core.CodeType;

namespace Domain.LogisticsOrder
{
    public class LogisticsOrderRefundType : CodeTypeBase
    {
        /// <summary>
        /// 退款
        /// </summary>
        public static readonly LogisticsOrderRefundType Refund = new LogisticsOrderRefundType(100, "退款", "客户支付了比订单当前费用更多的钱，订单处于超额支付状态，审核后会多一条订单费用抵充");

        /// <summary>
        /// 赔款
        /// </summary>
        public static readonly LogisticsOrderRefundType Indemnity = new LogisticsOrderRefundType(200, "赔款", "客户没有支付比订单费用更多的钱，但需要给与费用补偿，审核后不会多一条订单费用抵充");


        /// <summary>
        /// None
        /// </summary>
        public static readonly LogisticsOrderRefundType None = new LogisticsOrderRefundType(0, "None", "None");


        public static LogisticsOrderRefundType GetById(int typeId)
        {
            foreach (var typeInfo in GetList())
            {
                if (typeInfo.Id == typeId)
                {
                    return typeInfo;
                }
            }

            return None;
        }

        private static IList<LogisticsOrderRefundType> typeList = new List<LogisticsOrderRefundType>() {Refund, Indemnity};

        public static IList<LogisticsOrderRefundType> GetList()
        {
            return typeList;
        }

        private LogisticsOrderRefundType(int id, string name, string description) : base(id, name, description)
        {
        }
    }

    public class LogisticsOrderRefundTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderRefundType>
    {
        protected override LogisticsOrderRefundType GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundType.GetById(id);
        }
    }

    public class LogisticsOrderRefundTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderRefundType>
    {
        protected override LogisticsOrderRefundType GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundType.GetById(id);
        }
    }
}