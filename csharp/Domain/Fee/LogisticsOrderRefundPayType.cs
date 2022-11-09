using System.Collections.Generic;
using Core.CodeType;

namespace Domain.LogisticsOrder
{
    public class LogisticsOrderRefundPayType : CodeTypeBase
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        public static readonly LogisticsOrderRefundPayType AliPay = new LogisticsOrderRefundPayType(100, "支付宝", "支付宝");

        /// <summary>
        /// 微信
        /// </summary>
        public static readonly LogisticsOrderRefundPayType WxPay = new LogisticsOrderRefundPayType(200, "微信", "微信");

        /// <summary>
        /// 转账
        /// </summary>
        public static readonly LogisticsOrderRefundPayType Transfer = new LogisticsOrderRefundPayType(300, "转账", "转账");

        /// <summary>
        /// 余额充值
        /// </summary>
        public static readonly LogisticsOrderRefundPayType Charge = new LogisticsOrderRefundPayType(400, "余额充值", "余额充值");


        /// <summary>
        /// None
        /// </summary>
        public static readonly LogisticsOrderRefundPayType None = new LogisticsOrderRefundPayType(0, "None", "None");


        public static LogisticsOrderRefundPayType GetById(int typeId)
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

        private static IList<LogisticsOrderRefundPayType> typeList = new List<LogisticsOrderRefundPayType>() {AliPay, WxPay, Transfer, Charge};

        public static IList<LogisticsOrderRefundPayType> GetList()
        {
            return typeList;
        }

        private LogisticsOrderRefundPayType(int id, string name, string description) : base(id, name, description)
        {
        }
    }

    public class LogisticsOrderRefundPayTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderRefundPayType>
    {
        protected override LogisticsOrderRefundPayType GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundPayType.GetById(id);
        }
    }

    public class LogisticsOrderRefundPayTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderRefundPayType>
    {
        protected override LogisticsOrderRefundPayType GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundPayType.GetById(id);
        }
    }
}