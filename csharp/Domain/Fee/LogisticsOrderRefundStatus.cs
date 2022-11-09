using System.Collections.Generic;
using Core.CodeType;

namespace Domain.LogisticsOrder
{
    /// <summary>
    /// 退赔款状态
    /// </summary>
    public class LogisticsOrderRefundStatus : CodeTypeBase
    {
        /// <summary>
        /// 已启用
        /// </summary>
        public static readonly LogisticsOrderRefundStatus NotPaid = new LogisticsOrderRefundStatus(100, "未支付", "未支付");

        /// <summary>
        /// 已停用
        /// </summary>
        public static readonly LogisticsOrderRefundStatus Paid = new LogisticsOrderRefundStatus(200, "已支付", "已支付");


        /// <summary>
        /// None
        /// </summary>
        public static readonly LogisticsOrderRefundStatus None = new LogisticsOrderRefundStatus(0, "", "");

        public static LogisticsOrderRefundStatus GetById(int id)
        {
            IList<LogisticsOrderRefundStatus> statusList = GetList();
            foreach (LogisticsOrderRefundStatus status in statusList)
            {
                if (status.Id == id)
                {
                    return status;
                }
            }

            return None;
        }

        public static IList<LogisticsOrderRefundStatus> GetList()
        {
            IList<LogisticsOrderRefundStatus> statusList = new List<LogisticsOrderRefundStatus>();
            statusList.Add(NotPaid);
            statusList.Add(Paid);

            return statusList;
        }

        private LogisticsOrderRefundStatus(int id, string name, string description) : base(id, name, description)
        {
        }
    }

    public class LogisticsOrderRefundStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderRefundStatus>
    {
        protected override LogisticsOrderRefundStatus GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundStatus.GetById(id);
        }
    }

    public class LogisticsOrderRefundStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderRefundStatus>
    {
        protected override LogisticsOrderRefundStatus GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundStatus.GetById(id);
        }
    }
}