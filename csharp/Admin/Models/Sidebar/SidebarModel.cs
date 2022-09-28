using shunda.Core.Util;

namespace shunda.Admin.Models.Sidebar;

public class SidebarModel
{
    /// <summary>
    /// Sidebar 是不是被折叠的
    /// </summary>
    public bool IsSidebarMin { get; set; } = true;

    /// <summary>
    /// 是否是管理员
    /// </summary>
    public bool IsAdmin { get; set; }

    public Dictionary<int, bool> PassResIdMap { get; set; } = new Dictionary<int, bool>();

    /// <summary>
    /// 是否有对某资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResId"></param>
    /// <returns></returns>
    public bool HasPassRes(int passResId)
    {
        return PassUtil.HasPassRes(PassResIdMap, IsAdmin, passResId);
    }

    /// <summary>
    /// 是否有对其中某个资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResIds"></param>
    /// <returns></returns>
    public bool HasOneOfPassRes(params int[] passResIds)
    {
        return PassUtil.HasOneOfPassRes(PassResIdMap, IsAdmin, passResIds);
    }

    public bool HasOneOfPassRes(IList<int> passResIdList)
    {
        return PassUtil.HasOneOfPassRes(PassResIdMap, IsAdmin, passResIdList);
    }

#if DEBUG
    public bool Debug { get; } = true;
#else
    public bool Debug { get; } = false;
#endif
}