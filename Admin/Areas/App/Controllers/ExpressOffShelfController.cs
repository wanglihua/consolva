using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Areas.App.Models.ExpressOffShelf;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[AdminAuth(PassRes.AdminApp.ExpressOffShelf)]
public class ExpressOffShelfController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly ExpressLogService expressLogService;

    public ExpressOffShelfController(
        ISession session,
        LoginUserService loginUserService,
        ExpressLogService expressLogService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.expressLogService = expressLogService;
    }

    [HttpPost]
    [Route("/admin/app/express/off/shelf/warehouse/list")]
    public IActionResult OffShelfWarehouseList() {
        IList<WarehouseInfo> warehouseList = session.Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.Name)
            .ToList();

        IList<ExpressOffShelfWarehouseListItem> warehouseItemList = new List<ExpressOffShelfWarehouseListItem>();

        foreach (WarehouseInfo warehouse in warehouseList) {
            warehouseItemList.Add(new ExpressOffShelfWarehouseListItem {
                Id = warehouse.Id,
                Code = warehouse.Code,
                Name = warehouse.Name,
            });
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressOffShelfWarehouseListModel {
                WarehouseList = warehouseItemList,
            }
        });
    }

    [HttpPost]
    [Route("/admin/app/express/off/shelf/shelf/list")]
    public IActionResult ShelfList(int warehouseId) {
        int loginUserId = loginUserService.GetLoginUserId();
        bool isAdmin = loginUserService.IsAdmin;

        // 子查询
        DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressShelfScheduleInfo>();
        scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Express), nameof(ExpressShelfScheduleInfo.Express));
        scheduleCriteria.Add(Restrictions.Or(
            Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Status)}", ExpressStatus.WaitPack),
            Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Status)}", ExpressStatus.PartOffShelf))
        );

        if (isAdmin == false) // 管理员的，显示所有已排班的；不是管理员的，只显示排班给当前登录人的。
        {
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.User)}.{nameof(ExpressShelfScheduleInfo.User.Id)}", loginUserId));
        }

        scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Id)}"));

        // 主查询
        ICriteria criteria = session.CreateCriteria<ExpressInfo>();
        criteria = criteria.CreateAlias(nameof(ExpressInfo.StorageLoc), nameof(ExpressInfo.StorageLoc));

        string shelfAlias = "shelf";
        criteria = criteria.CreateAlias($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Shelf)}", shelfAlias);

        criteria = criteria.Add(Restrictions.Or(
                Restrictions.Eq($"{nameof(ExpressInfo.Status)}", ExpressStatus.WaitPack),
                Restrictions.Eq($"{nameof(ExpressInfo.Status)}", ExpressStatus.PartOffShelf) // 要包含部分已下架的
            )
        );

        criteria = criteria.Add(Restrictions.Eq($"{shelfAlias}.{nameof(ShelfInfo.Warehouse)}.{nameof(WarehouseInfo.Id)}", warehouseId));

        criteria = criteria.Add(Subqueries.PropertyIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));

        criteria = criteria.SetProjection(
            Projections.ProjectionList()
                .Add(Projections.RowCount())
                .Add(Projections.GroupProperty($"shelf.{nameof(ShelfInfo.Id)}"), "shelf_id")
                .Add(Projections.GroupProperty($"shelf.{nameof(ShelfInfo.Name)}"), "shelf_name")
                .Add(Projections.GroupProperty($"shelf.{nameof(ShelfInfo.OrderNum)}"), "shelf_order_num")
        );

        criteria = criteria.AddOrder(Order.Asc($"shelf.{nameof(ShelfInfo.OrderNum)}"));
        criteria = criteria.AddOrder(Order.Asc($"shelf.{nameof(ShelfInfo.Name)}"));

        IList<object[]> shelfObjList = criteria.List<object[]>();

        var itemList = new List<ExpressOffShelfShelfListItem>();

        foreach (object[] shelfObj in shelfObjList) {
            itemList.Add(new ExpressOffShelfShelfListItem {
                ExpressCount = (int)(shelfObj[0] ?? 0),
                Id = (int)(shelfObj[1] ?? 0),
                Name = (string)(shelfObj[2] ?? ""),
            });
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "获取数据成功！",
            Data = new ExpressOffShelfShelfListModel {
                ShelfList = itemList,
            },
        });
    }

    [HttpPost]
    [Route("/admin/app/express/off/shelf/express/list")]
    public IActionResult ExpressList(int shelfId) {
        var shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            throw new ValidationException("指定的货架不存在！");
        }

        ICriteria criteria = GetOffShelfCriteria(shelfId);
        IList<ExpressInfo> expressList = criteria.List<ExpressInfo>();

        IList<int> customerIdList = expressList.Where(e => e.Customer != null).Select(e => e.Customer!.Id).Distinct().ToList();
        IList<CustomerInfo> customerList = session.Query<CustomerInfo>().Where(e => customerIdList.Contains(e.Id)).ToList();

        IList<int> userIdList = expressList.Select(e => e.AddUser.Id).Distinct().ToList();
        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => userIdList.Contains(e.Id)).ToList();

        IList<IDictionary<string, object>> itemList = new List<IDictionary<string, object>>();

        foreach (ExpressInfo express in expressList) {
            IDictionary<string, object> item = new Dictionary<string, object>();
            item["id"] = express.Id;
            item["trackingNo"] = express.TrackingNo;
            item["checkinTrackingNo"] = express.CheckinTrackingNo;
            item["packageCount"] = express.PackageCount;
            item["storageLocName"] = express.StorageLoc.GetFullName();

            var customer = customerList.Where(e => e.Id == (express.Customer?.Id ?? 0)).Take(1).SingleOrDefault();
            item["customerCode"] = customer?.CheckinCode ?? string.Empty;

            var addUser = userList.Where(e => e.Id == express.AddUser.Id).Take(1).SingleOrDefault();
            item["addUserName"] = addUser?.UserName ?? string.Empty;

            item["addTime"] = express.AddTime.ToString("yyyy-MM-dd HH:mm");

            // 是不是 下架时，还没上架
            item["notOnShelf"] = express.ShelfStatus == ExpressShelfStatus.ToOnShelf || express.ShelfStatus == ExpressShelfStatus.PartOnShelf;
            item["shelfStatus"] = express.ShelfStatus?.Name ?? string.Empty;

            itemList.Add(item);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new Dictionary<string, object>() {
                { "itemList", itemList },
                { "itemCount", itemList.Count }
            }
        });
    }

    [HttpPost]
    [Route("/admin/app/express/off/shelf/next/express")]
    public IActionResult NextExpress(int shelfId) {
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            throw new ValidationException("指定的货架不存在！");
        }

        ICriteria criteria = GetOffShelfCriteria(shelfId);
        ExpressInfo cg = criteria
            .SetMaxResults(1)
            .UniqueResult<ExpressInfo>();

        IDictionary<string, object> item = new Dictionary<string, object>();
        if (cg != null) {
            item = new Dictionary<string, object>();

            // 和List里面的字段一致

            item["id"] = cg.Id;
            item["trackingNo"] = cg.TrackingNo;
            item["checkinTrackingNo"] = cg.CheckinTrackingNo;
            item["packageCount"] = cg.PackageCount;
            item["storageLocName"] = cg.StorageLoc.GetFullName();

            CustomerInfo customer = session.Get<CustomerInfo>((cg.Customer?.Id ?? 0));
            item["customerCode"] = customer?.CheckinCode ?? string.Empty;
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = item,
        });
    }

    [NonAction]
    private ICriteria GetOffShelfCriteria(int shelfId) {
        int loginUserId = loginUserService.GetLoginUserId();
        bool isAdmin = loginUserService.IsAdmin;

        // 子查询
        DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressShelfScheduleInfo>();
        scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Express), nameof(ExpressShelfScheduleInfo.Express));
        scheduleCriteria.Add(Restrictions.Or(
            Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Status)}", ExpressStatus.WaitPack),
            Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Status)}", ExpressStatus.PartOffShelf))
        );

        if (isAdmin == false) // 管理员的，显示所有已排班的；不是管理员的，只显示排班给当前登录人的。
        {
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.User)}.{nameof(ExpressShelfScheduleInfo.User.Id)}", loginUserId));
        }

        scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Id)}"));

        // 主查询
        ICriteria criteria = session.CreateCriteria<ExpressInfo>();
        criteria = criteria.CreateAlias(nameof(ExpressInfo.StorageLoc), nameof(ExpressInfo.StorageLoc));

        string shelfAlias = "shelf";
        criteria = criteria.CreateAlias($"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Shelf)}", shelfAlias);

        criteria = criteria.Add(Restrictions.Eq($"{shelfAlias}.{nameof(ShelfInfo.Id)}", shelfId));
        criteria = criteria.Add(Restrictions.Or(
                Restrictions.Eq($"{nameof(ExpressInfo.Status)}", ExpressStatus.WaitPack),
                Restrictions.Eq($"{nameof(ExpressInfo.Status)}", ExpressStatus.PartOffShelf) // 要包含部分已下架的
            )
        );

        criteria = criteria.Add(Subqueries.PropertyIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));

        criteria = criteria.AddOrder(Order.Asc($"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.OffShelfOrderNum)}"));
        criteria = criteria.AddOrder(Order.Desc(nameof(ExpressInfo.AddTime)));

        return criteria;
    }

    /// <summary>
    /// 完成快递单号下架
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("/user/api/express/shelf/off/shelf/complete")]
    public IActionResult OffShelfComplete(ExpressOffShelfCompleteParam param) {
        ValidationUtil.Validate<ExpressOffShelfCompleteParamValidator, ExpressOffShelfCompleteParam>(param);

        int shelfId = param.ShelfId;
        string trackingNo = param.TrackingNo;

        UserInfo loginUser = loginUserService.GetLoginUser();
        int loginUserId = loginUserService.GetLoginUserId();

        var express = session.Query<ExpressInfo>()
            .Fetch(e => e.Order)
            .Where(e => e.TrackingNo == trackingNo || e.CheckinTrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null) {
            throw new ValidationException("快递单号或入库跟踪号不存在！");
        }

        int expressShelfId = express.StorageLoc.Shelf.Id;
        if (expressShelfId != shelfId) {
            throw new ValidationException("扫描的快递单号所在货架跟当前扫码快递单号不匹配，是不是选错货架了！");
        }

        // 2: 检查状态
        if (express.Status != ExpressStatus.WaitPack && express.Status != ExpressStatus.PartOffShelf) {
            throw new ValidationException("快递单当前不是待打包状态或部分下架状态，不能下架！");
        }

        IList<ExpressStatus> offShelfStatusList = ExpressStatus.GetOffShelfList();
        if (offShelfStatusList.Contains(express.Status)) {
            throw new ValidationException("快递单当前已下架或后续状态！");
        }

        if (express.Status == ExpressStatus.OffShelf) // 这个检查是多余的
        {
            throw new ValidationException("快递单当前已经下架！");
        }

        int claimGoodsId = express.Id;

        ICriteria criteria = GetOffShelfCriteria(shelfId);
        criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Id == claimGoodsId));

        express = criteria.SetMaxResults(1).UniqueResult<ExpressInfo>();
        if (express == null) {
            throw new ValidationException($"扫描的快递单号#{trackingNo}不在当前列表中！");
        }


        // 3：设置已下架
        express.Status = ExpressStatus.OffShelf;
        express.ShelfStatus = ExpressShelfStatus.OffShelf;

        express.OffShelfTime = DateTime.Now;
        express.OffShelfUser = loginUser;
        express.DumpStatus = ExpressDumpStatus.NotDump; // 下架的取消堆存标记

        session.Update(express);
        session.Flush();

        // 4: 下架日志
        expressLogService.UserLog(express.Id, ExpressLogType.OffShelf, loginUserId);

        return Json(new JsonResultData {
            Success = true,
            Message = "快递单下架成功！",
        });
    }
}