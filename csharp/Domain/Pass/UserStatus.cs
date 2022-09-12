using System.Collections.Generic;
using shunda.Core.CodeType;

namespace shunda.Domain.Pass
{
    /// <summary>
    /// 用户状态
    /// </summary>
    public class UserStatus : CodeTypeBase
    {
        /// <summary>
        /// 正常状态, 100
        /// </summary>
        public static readonly UserStatus Normal = new UserStatus(100, "正常", "正常");

        /// <summary>
        /// 冻结状态, 200
        /// </summary>
        public static readonly UserStatus Frozen = new UserStatus(200, "冻结", "冻结");

        /// <summary>
        /// None
        /// </summary>
        private static readonly UserStatus None = new UserStatus(0, "", "");

        public static UserStatus GetById(int id)
        {
            IList<UserStatus> statusList = GetList();
            foreach (UserStatus status in statusList)
            {
                if (status.Id == id)
                {
                    return status;
                }
            }

            return None;
        }

        public static IList<UserStatus> GetList()
        {
            IList<UserStatus> statusList = new List<UserStatus>();
            statusList.Add(Normal);
            statusList.Add(Frozen);

            return statusList;
        }

        private UserStatus(int id, string name, string description) : base(id, name, description)
        {
        }
    }

    public class UserStatusHibernateType : CodeHibernateTypeBase<UserStatus>
    {
        protected override UserStatus GetCodeTypeById(int id)
        {
            return UserStatus.GetById(id);
        }
    }

    public class UserStatusModelBinder : CodeTypeModelBinderBase<UserStatus>
    {
        protected override UserStatus GetCodeTypeById(int id)
        {
            return UserStatus.GetById(id);
        }
    }
}