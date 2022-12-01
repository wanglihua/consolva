using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using shunda.Admin.Models.Sidebar;
using shunda.Admin.Services;
using shunda.Domain.Storage;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent
{
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly SidebarStateService sidebarStateService;
    private readonly IMemoryCache memoryCache;

    public SidebarViewComponent(
        ISession session,
        LoginUserService loginUserService,
        SidebarStateService sidebarStateService,
        IMemoryCache memoryCache)
    {
        this.session = session;
        this.loginUserService = loginUserService;
        this.sidebarStateService = sidebarStateService;
        this.memoryCache = memoryCache;
    }

    public IViewComponentResult Invoke()
    {
        var isAdmin = loginUserService.IsAdmin;
        var loginUserId = loginUserService.GetLoginUserId();

        var model = new SidebarModel();
        model.IsAdmin = isAdmin;
        model.PassResIdMap = loginUserService.GetPassResIdMap();
        model.IsSidebarMin = sidebarStateService.GetMinState(loginUserId);

        string warehouseListCacheKey = "sidebar_warehouse_list_cache_key";
        if (memoryCache.TryGetValue(warehouseListCacheKey, out IList<WarehouseInfo>? warehouseList) == false)
        {
            warehouseList = session.Query<WarehouseInfo>().OrderBy(e => e.OrderNum).ToList();

            memoryCache.Set(warehouseListCacheKey, warehouseList, TimeSpan.FromMinutes(15)); // todo d 以后可以做成在仓库列表维护界面刷新这个缓存，并把缓存时间加长
        }

        model.WarehouseList = warehouseList!;

        return View("Sidebar", model);
    }
}