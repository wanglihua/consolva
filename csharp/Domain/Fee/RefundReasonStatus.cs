using System.Collections.Generic;
using Core.CodeType;

namespace Domain.LogisticsOrder
{
    /// <summary>
    /// 退赔原因状态
    /// </summary>
    public class RefundReasonStatus : CodeTypeBase
    {
        /// <summary>
        /// 已启用
        /// </summary>
        public static readonly RefundReasonStatus Enabled = new RefundReasonStatus(100, "已启用", "已启用");

        /// <summary>
        /// 已停用
        /// </summary>
        public static readonly RefundReasonStatus Disabled = new RefundReasonStatus(200, "已停用", "已停用");


        /// <summary>
        /// None
        /// </summary>
        public static readonly RefundReasonStatus None = new RefundReasonStatus(0, "", "");

        public static RefundReasonStatus GetById(int id)
        {
            IList<RefundReasonStatus> statusList = GetList();
            foreach (RefundReasonStatus status in statusList)
            {
                if (status.Id == id)
                {
                    return status;
                }
            }

            return None;
        }

        public static IList<RefundReasonStatus> GetList()
        {
            IList<RefundReasonStatus> statusList = new List<RefundReasonStatus>();
            statusList.Add(Enabled);
            statusList.Add(Disabled);

            return statusList;
        }

        private RefundReasonStatus(int id, string name, string description) : base(id, name, description)
        {
        }
    }

    public class RefundReasonStatusHibernateType : CodeHibernateTypeBase<RefundReasonStatus>
    {
        protected override RefundReasonStatus GetCodeTypeById(int id)
        {
            return RefundReasonStatus.GetById(id);
        }
    }

    public class RefundReasonStatusModelBinder : CodeTypeModelBinderBase<RefundReasonStatus>
    {
        protected override RefundReasonStatus GetCodeTypeById(int id)
        {
            return RefundReasonStatus.GetById(id);
        }
    }
}