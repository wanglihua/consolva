using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Areas.Pass.Models.WarehouseWorker;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Domain.Storage;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[PassArea]
[AdminAuth(AdminOnly = true)]
public class WarehouseWorkerController : Controller {
    private readonly ISession session;
    private readonly WarehouseService warehouseService;

    public WarehouseWorkerController(ISession session, WarehouseService warehouseService) {
        this.session = session;
        this.warehouseService = warehouseService;
    }

    [HttpGet]
    [Route("/admin/warehouse/worker")]
    public IActionResult Index() {
        IList<WarehouseInfo> warehouseList = warehouseService.GetWarehouseList();

        WarehouseWorkerIndexModel model = new WarehouseWorkerIndexModel();
        model.WarehouseList = warehouseList;
        model.TypeList = WarehouseWorkerType.GetList();

        return View("WarehouseWorkerIndex", model);
    }

    [HttpPost]
    [Route("/admin/warehouse/worker/list")]
    public IActionResult List(WarehouseWorkerListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<WarehouseWorkerInfo> warehouseWorkerList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<WarehouseWorkerInfo>();

        WarehouseWorkerListModel model = new WarehouseWorkerListModel(listPager);
        model.WarehouseWorkerList = warehouseWorkerList;
        model.WarehouseAlias = warehouseAlias;
        model.UserAlias = userAlias;

        return View("WarehouseWorkerList", model);
    }

    private static readonly string warehouseAlias = "w";
    private static readonly string userAlias = "u";

    [NonAction]
    private ICriteria GetListCriteria(WarehouseWorkerListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<WarehouseWorkerInfo>();

        criteria = criteria.CreateAlias(nameof(WarehouseWorkerInfo.Warehouse), warehouseAlias);
        criteria = criteria.CreateAlias(nameof(WarehouseWorkerInfo.User), userAlias);

        criteria = criteria.SetReadOnly(true);

        int warehouseIdQuery = queryParam.WarehouseIdQuery;
        if (warehouseIdQuery != 0) {
            criteria = criteria.Add(Restrictions.Where<WarehouseWorkerInfo>(e => e.Warehouse.Id == warehouseIdQuery));
        }

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != WarehouseWorkerType.None) {
            WarehouseWorkerType typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<WarehouseWorkerInfo>(e => e.Type == typeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserQuery) == false) {
            string userQuery = queryParam.UserQuery.Trim();
            criteria = criteria.Add(
                Restrictions.Or(
                    Restrictions.Like($"{userAlias}.{nameof(UserInfo.LoginName)}", userQuery, MatchMode.Anywhere),
                    Restrictions.Like($"{userAlias}.{nameof(UserInfo.UserName)}", userQuery, MatchMode.Anywhere)
                )
            );
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/warehouse/worker/detail")]
    public IActionResult Detail(WarehouseWorkerDetailParam param) {
        session.DefaultReadOnly = true;

        WarehouseWorkerInfo warehouseWorker;

        WarehouseWorkerDetailModel model = new WarehouseWorkerDetailModel();

        if (param.Id == null || param.Id.Value == 0) {
            warehouseWorker = new WarehouseWorkerInfo();
            model.WarehouseId = param.WarehouseId ?? 0;
            warehouseWorker.Type = param.Type ?? WarehouseWorkerType.None;
            model.UserQuery = param.UserQuery ?? string.Empty;
        } else {
            warehouseWorker = session.Get<WarehouseWorkerInfo>(param.Id.Value);
            if (warehouseWorker == null) {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        model.WarehouseList = warehouseService.GetWarehouseList();
        model.TypeList = WarehouseWorkerType.GetList();

        model.WarehouseWorker = warehouseWorker;

        if (warehouseWorker.Warehouse != null) {
            model.WarehouseId = warehouseWorker.Warehouse.Id;
        }

        model.GroupList = session.Query<GroupInfo>().OrderBy(e => e.Code).ToList();

        return View("WarehouseWorkerDetail", model);
    }

    [HttpPost]
    [Route("/admin/warehouse/worker/save")]
    public IActionResult Save(WarehouseWorkerSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (model.UserIdList == null || model.UserIdList.Count == 0) {
            throw new ValidationException("请选择要添加的用户！");
        }


        int warehouseId = model.WarehouseId;
        WarehouseWorkerType workerType = model.WorkerType;


        foreach (int userId in model.UserIdList) {
            var workerExist = session.Query<WarehouseWorkerInfo>()
                .Fetch(e => e.User)
                .Where(e => e.Warehouse.Id == warehouseId
                            && e.User.Id == userId
                            && e.Type == workerType)
                .Take(1)
                .SingleOrDefault();

            if (workerExist != null) {
                throw new ValidationException($"人员{workerExist.User.LoginName}({workerExist.User.UserName}) 已和指定的仓库和人员类型绑定！");
            }

            WarehouseWorkerInfo warehouseWorker = new WarehouseWorkerInfo();
            warehouseWorker.Warehouse = session.Get<WarehouseInfo>(warehouseId);
            warehouseWorker.User = session.Get<UserInfo>(userId);
            warehouseWorker.Type = workerType;

            session.Save(warehouseWorker);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/warehouse/worker/user/list")]
    public IActionResult UserList(WarehouseWorkerUserListQueryParam queryParam) {
        int totalCount = GetUserListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<UserInfo> userList = GetUserListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<UserInfo>();

        WarehouseWorkerUserListModel model = new WarehouseWorkerUserListModel(listPager);
        model.UserList = userList;

        return View("WarehouseWorkerUserList", model);
    }

    [NonAction]
    private ICriteria GetUserListCriteria(WarehouseWorkerUserListQueryParam queryParam) {
        ICriteria criteria = this.session.CreateCriteria<UserInfo>();

        criteria = criteria.SetReadOnly(true);

        string devLoginName = UserInfo.DevLoginName;
        criteria = criteria.Add(Restrictions.Where<UserInfo>(e => e.LoginName != devLoginName));
        criteria = criteria.Add(Restrictions.Where<UserInfo>(e => e.Status == UserStatus.Normal));

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false) {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(
                Restrictions.Or(
                    Restrictions.On<UserInfo>(e => e.LoginName).IsInsensitiveLike(nameQuery, MatchMode.Anywhere),
                    Restrictions.On<UserInfo>(e => e.UserName).IsInsensitiveLike(nameQuery, MatchMode.Anywhere)
                )
            );
        }

        if (queryParam.GroupIdQuery != 0) {
            int groupIdQuery = queryParam.GroupIdQuery;
            DetachedCriteria groupUserIdCriteria = DetachedCriteria.For<GroupMemberInfo>()
                .Add(Restrictions.Eq($"{nameof(GroupMemberInfo.Group)}.{nameof(GroupInfo.Id)}", groupIdQuery))
                .SetProjection(Projections.Property($"{nameof(GroupMemberInfo.User)}.{nameof(GroupMemberInfo.User.Id)}"));

            criteria = criteria.Add(Subqueries.PropertyIn(nameof(UserInfo.Id), groupUserIdCriteria));
        }

        if (queryParam.WarehouseId != 0 && (queryParam.Type != null && queryParam.Type != WarehouseWorkerType.None)) {
            int warehouseId = queryParam.WarehouseId;
            WarehouseWorkerType workerType = queryParam.Type;
            DetachedCriteria warehouseWorkerIdCriteria = DetachedCriteria.For<WarehouseWorkerInfo>()
                .Add(
                    Restrictions.And(
                        Restrictions.Eq($"{nameof(WarehouseWorkerInfo.Warehouse)}.{nameof(WarehouseWorkerInfo.Warehouse.Id)}", warehouseId),
                        Restrictions.Eq(nameof(WarehouseWorkerInfo.Type), workerType)
                    )
                )
                .SetProjection(Projections.Property($"{nameof(WarehouseWorkerInfo.User)}.{nameof(WarehouseWorkerInfo.User.Id)}"));

            criteria = criteria.Add(Subqueries.PropertyNotIn(nameof(UserInfo.Id), warehouseWorkerIdCriteria));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/warehouse/worker/delete")]
    public IActionResult Delete(int[] idList) {
        foreach (int id in idList) {
            WarehouseWorkerInfo warehouseWorker = session.Load<WarehouseWorkerInfo>(id);
            session.Delete(warehouseWorker);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}