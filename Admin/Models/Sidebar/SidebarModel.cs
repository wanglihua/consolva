using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core.Util;
using shunda.Domain.Logistics;
using shunda.Domain.Storage;

namespace shunda.Admin.Models.Sidebar;

public class SidebarModel {
    /// <summary>
    /// Sidebar 是不是被折叠的
    /// </summary>
    [BindNever]
    public bool IsSidebarMin { get; set; } = true;

    /// <summary>
    /// 是否是管理员
    /// </summary>
    [BindNever]
    public bool IsAdmin { get; set; }

    [BindNever]
    public Dictionary<int, bool> PassResIdMap { get; set; } = new();

    /// <summary>
    /// 是否有对某资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResId"></param>
    /// <returns></returns>
    public bool HasPassRes(int passResId) {
        return PassUtil.HasPassRes(PassResIdMap, IsAdmin, passResId);
    }

    /// <summary>
    /// 是否有对其中某个资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResIds"></param>
    /// <returns></returns>
    public bool HasOneOfPassRes(params int[] passResIds) {
        return PassUtil.HasOneOfPassRes(PassResIdMap, IsAdmin, passResIds);
    }

    public bool HasOneOfPassRes(IList<int> passResIdList) {
        return PassUtil.HasOneOfPassRes(PassResIdMap, IsAdmin, passResIdList);
    }

    [BindNever]
    public IList<LogisticsGoodsType> LogisticsGoodsTypeList { get; set; } = LogisticsGoodsType.GetInnerList();

    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

#if DEBUG
    public bool Debug { get; } = true;
#else
    public bool Debug { get; } = false;
#endif
}