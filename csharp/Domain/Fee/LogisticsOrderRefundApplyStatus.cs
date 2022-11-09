using System.Collections.Generic;
using Core.CodeType;

namespace Domain.LogisticsOrder
{
    /// <summary>
    /// 退赔款申请状态
    /// </summary>
    public class LogisticsOrderRefundApplyStatus : CodeTypeBase
    {
        /// <summary>
        /// 未提交
        /// </summary>
        public static readonly LogisticsOrderRefundApplyStatus Saved = new LogisticsOrderRefundApplyStatus(100, "未提交", "未提交");

        /// <summary>
        /// 已回退
        /// </summary>
        public static readonly LogisticsOrderRefundApplyStatus Backed = new LogisticsOrderRefundApplyStatus(200, "已回退", "已回退");

        /// <summary>
        /// 已提交
        /// </summary>
        public static readonly LogisticsOrderRefundApplyStatus Submitted = new LogisticsOrderRefundApplyStatus(300, "已提交", "已提交");

        /// <summary>
        /// 已审核
        /// </summary>
        public static readonly LogisticsOrderRefundApplyStatus Approved = new LogisticsOrderRefundApplyStatus(400, "已审核", "已审核");


        /// <summary>
        /// None
        /// </summary>
        public static readonly LogisticsOrderRefundApplyStatus None = new LogisticsOrderRefundApplyStatus(0, "", "");

        public static LogisticsOrderRefundApplyStatus GetById(int id)
        {
            IList<LogisticsOrderRefundApplyStatus> statusList = GetList();
            foreach (LogisticsOrderRefundApplyStatus status in statusList)
            {
                if (status.Id == id)
                {
                    return status;
                }
            }

            return None;
        }

        public static IList<LogisticsOrderRefundApplyStatus> GetList()
        {
            IList<LogisticsOrderRefundApplyStatus> statusList = new List<LogisticsOrderRefundApplyStatus>();
            statusList.Add(Saved);
            statusList.Add(Backed);
            statusList.Add(Submitted);
            statusList.Add(Approved);

            return statusList;
        }

        public static IList<LogisticsOrderRefundApplyStatus> GetApproveViewList()
        {
            IList<LogisticsOrderRefundApplyStatus> statusList = new List<LogisticsOrderRefundApplyStatus>();
            statusList.Add(Submitted);
            statusList.Add(Approved);

            return statusList;
        }

        private LogisticsOrderRefundApplyStatus(int id, string name, string description) : base(id, name, description)
        {
        }
    }

    public class LogisticsOrderRefundApplyStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderRefundApplyStatus>
    {
        protected override LogisticsOrderRefundApplyStatus GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundApplyStatus.GetById(id);
        }
    }

    public class LogisticsOrderRefundApplyStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderRefundApplyStatus>
    {
        protected override LogisticsOrderRefundApplyStatus GetCodeTypeById(int id)
        {
            return LogisticsOrderRefundApplyStatus.GetById(id);
        }
    }
}