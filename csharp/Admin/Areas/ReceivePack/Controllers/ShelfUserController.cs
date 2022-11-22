using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.ReceivePack.Models.ShelfUser;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.ExpressCheckinConfig)]
public class ShelfUserController : Controller
{
    private ISession session;

    public ShelfUserController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/shelf/user")]
    public IActionResult Index()
    {
        ShelfUserIndexModel model = new ShelfUserIndexModel();

        model.TypeList = ShelfUserType.GetList();

        return View("ShelfUserIndex", model);
    }

    [HttpPost]
    [Route("/admin/shelf/user/list")]
    public IActionResult List(ShelfUserListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ShelfUserInfo> shelfUserList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ShelfUserInfo>();

        foreach (ShelfUserInfo su in shelfUserList)
        {
            // su.Shelf.TypeName = ShelfType.GetById(su.Shelf.TypeId)?.Name;
        }

        ShelfUserListModel model = new ShelfUserListModel(listPager);
        model.ShelfUserList = shelfUserList;

        return View("ShelfUserList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ShelfUserListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ShelfUserInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ShelfUserInfo.User), nameof(ShelfUserInfo.User));
        criteria = criteria.CreateAlias(nameof(ShelfUserInfo.Warehouse), nameof(ShelfUserInfo.Warehouse));
        criteria = criteria.CreateAlias(nameof(ShelfUserInfo.Shelf), nameof(ShelfUserInfo.Shelf));

        if (string.IsNullOrWhiteSpace(queryParam.ShelfNameQuery) == false)
        {
            string shelfNameQuery = queryParam.ShelfNameQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(ShelfUserInfo.Shelf)}.{nameof(ShelfUserInfo.Shelf.Name)}", shelfNameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserQuery) == false)
        {
            string userQuery = queryParam.UserQuery.Trim();
            criteria = criteria.Add(
                Restrictions.Or(
                    Restrictions.Like($"{nameof(ShelfUserInfo.User)}.{nameof(ShelfUserInfo.User.LoginName)}", userQuery, MatchMode.Anywhere),
                    Restrictions.Like($"{nameof(ShelfUserInfo.User)}.{nameof(ShelfUserInfo.User.UserName)}", userQuery, MatchMode.Anywhere)
                )
            );
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/shelf/user/detail")]
    public IActionResult Detail(int id)
    {
        session.DefaultReadOnly = true;

        ShelfUserInfo shelfUser;

        ShelfUserDetailModel model = new ShelfUserDetailModel();
        if (id == 0)
        {
            shelfUser = new ShelfUserInfo();
            model.ShelfList = new List<ShelfInfo>();
        }
        else
        {
            shelfUser = session.Get<ShelfUserInfo>(id);
            if (shelfUser == null)
            {
                throw new ValidationException($"指定的 货架用户关联#{id} 不存在！");
            }

            int warehouseId = shelfUser.Warehouse?.Id ?? 0;
            model.WarehouseId = warehouseId;

            IList<ShelfInfo> shelfList = session.Query<ShelfInfo>()
                .Where(e => e.Warehouse.Id == warehouseId && e.Status == ShelfStatus.Enabled)
                .OrderBy(e => e.OrderNum).ThenBy(e => e.Name)
                .ToList();

            foreach (ShelfInfo shelf in shelfList)
            {
                // shelf.TypeName = ShelfType.GetById(shelf.TypeId)?.Name;
            }

            model.ShelfList = shelfList;
        }

        model.ShelfUserId = shelfUser.Id;

        model.LoginName = shelfUser.User.LoginName;

        model.ShelfId = shelfUser.Shelf?.Id ?? 0;
        model.Type = shelfUser.Type;

        model.WarehouseList = session.Query<WarehouseInfo>().Where(e => e.InnerStatus == WarehouseStatus.Enabled).ToList();

        model.TypeList = ShelfUserType.GetList();

        return View("ShelfUserDetail", model);
    }

    [HttpPost]
    [Route("/admin/shelf/user/shelf/list")]
    public IActionResult ShelfList(int warehouseId)
    {
        IList<ShelfInfo> shelfList = session.Query<ShelfInfo>()
            .Where(e => e.Warehouse.Id == warehouseId && e.Status == ShelfStatus.Enabled)
            .OrderBy(e => e.OrderNum).ThenBy(e => e.Name)
            .ToList();

        foreach (ShelfInfo shelf in shelfList)
        {
            // shelf.TypeName = ShelfType.GetById(shelf.TypeId)?.Name;
        }

        ShelfUserDetailModel model = new ShelfUserDetailModel();
        model.ShelfList = shelfList;

        return View("ShelfUserShelfList", model);
    }

    [HttpPost]
    [Route("/admin/shelf/user/save")]
    [Transactional]
    public IActionResult Save(ShelfUserDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int shelfUserId = model.ShelfUserId;

        bool isCreate = shelfUserId == 0;

        ShelfUserInfo shelfUser;

        if (isCreate)
        {
            shelfUser = new ShelfUserInfo();
        }
        else
        {
            shelfUser = session.Get<ShelfUserInfo>(shelfUserId);
            if (shelfUser == null)
            {
                throw new ValidationException($"指定的 货架用户关联#{shelfUserId} 不存在！");
            }
        }

        string loginName = model.LoginName?.Trim() ?? string.Empty;

        UserInfo? user = session.Query<UserInfo>().Where(e => e.LoginName == loginName).Take(1).SingleOrDefault();
        if (user == null)
        {
            throw new ValidationException("指定的用户不存在！");
        }

        WarehouseInfo warehouse = session.Get<WarehouseInfo>(model.WarehouseId);
        ShelfInfo shelf = session.Get<ShelfInfo>(model.ShelfId);

        shelfUser.Warehouse = warehouse;
        shelfUser.User = user;
        shelfUser.Shelf = shelf;
        shelfUser.Type = model.Type;

        if (isCreate)
        {
            session.Save(shelfUser);
        }
        else
        {
            session.Update(shelfUser);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/shelf/user/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要删除的数据项！");
        }

        foreach (int id in idList)
        {
            ShelfUserInfo shelfUser = session.Load<ShelfUserInfo>(id);
            session.Delete(shelfUser);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}