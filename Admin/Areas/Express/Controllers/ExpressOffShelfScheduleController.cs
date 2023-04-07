using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Admin.Areas.Express.Models.ExpressOffShelfSchedule;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressOffShelfSchedule)]
public class ExpressOffShelfScheduleController : Controller {
    private readonly ISession session;
    private readonly ExpressListService expressListService;
    private readonly ExpressShelfScheduleService expressShelfScheduleService;

    public ExpressOffShelfScheduleController(
        ISession session,
        ExpressListService expressListService,
        ExpressShelfScheduleService expressShelfScheduleService
    ) {
        this.session = session;
        this.expressListService = expressListService;
        this.expressShelfScheduleService = expressShelfScheduleService;
    }

    [HttpGet]
    [Route("/admin/express/off/shelf/schedule/{warehouseId:int}")]
    public IActionResult Index(int warehouseId) {
        var warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException($"指定的仓库#{warehouseId}不存在！");
        }

        ExpressOffShelfScheduleIndexModel model = new ExpressOffShelfScheduleIndexModel();

        model.OffShelfUserList = session.Query<WarehouseWorkerInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Warehouse.Id == warehouseId && e.Type == WarehouseWorkerType.OffShelf)
            .OrderBy(e => e.User.LoginName)
            .Select(e => e.User)
            .ToList();

        IList<ShelfInfo> shelfList = expressListService.GetShelfList(ExpressListMenuCate.ToOffShelfSchedule, warehouseId);

        model.Warehouse = warehouse;
        model.ShelfList = shelfList;

        model.FirstModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Cate == LogisticsModeCate.Customer
                        && e.InnerStatus != LogisticsModeStatus.Disabled)
            .OrderBy(e => e.Name)
            .ThenBy(e => e.Id)
            .ToList();

        return View("ExpressOffShelfScheduleIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/off/shelf/scheduling/list")]
    public IActionResult SchedulingList(ExpressListQueryParam queryParam) {
        int totalCount = expressListService.Count(ExpressListMenuCate.ToOffShelfSchedule, queryParam);
        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressInfo> schedulingList = expressListService.List(ExpressListMenuCate.ToOffShelfSchedule, queryParam, listPager);

        ExpressOffShelfSchedulingListModel model = new ExpressOffShelfSchedulingListModel(listPager);
        model.SchedulingList = schedulingList;

        // IList<ShelfInfo> shelfList = expressListService.GetShelfList(ExpressListMenuCate.ToOffShelfSchedule, queryParam);
        // model.ShelfList = shelfList;

        return View("ExpressOffShelfSchedulingList", model);
    }

    [HttpPost]
    [Route("/admin/express/off/shelf/scheduled/list")]
    public IActionResult ScheduledList(ExpressListQueryParam queryParam) {
        int totalCount = expressShelfScheduleService.Count(ExpressShelfScheduleType.OffShelf, queryParam);
        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressShelfScheduleInfo> scheduledList = expressShelfScheduleService.List(ExpressShelfScheduleType.OffShelf, queryParam, listPager);

        ExpressOffShelfScheduledListModel model = new ExpressOffShelfScheduledListModel(listPager);
        model.ScheduledList = scheduledList;

        return View("ExpressOffShelfScheduledList", model);
    }

    [HttpPost]
    [Route("/admin/express/off/shelf/scheduling")]
    [Transactional]
    public IActionResult Scheduling(IList<int> idList) {
        if (idList == null || idList.Count == 0) {
            throw new ValidationException("请选择要移除排班的数据！");
        }

        var scheduleList = session.Query<ExpressShelfScheduleInfo>()
            .Where(e => e.ScheduleType == ExpressShelfScheduleType.OffShelf && idList.Contains(e.Id))
            .ToList();

        foreach (var schedule in scheduleList) {
            session.Delete(schedule);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "移除排班成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/off/shelf/scheduling/by/query")]
    [Transactional]
    public IActionResult SchedulingByQuery(ExpressListQueryParam queryParam) {
        IList<ExpressShelfScheduleInfo> scheduledList = expressShelfScheduleService.List(ExpressShelfScheduleType.OffShelf, queryParam);

        if (scheduledList.Count == 0) {
            throw new ValidationException("指定查询条件的排班数据不存在！");
        }

        foreach (ExpressShelfScheduleInfo schedule in scheduledList) {
            session.Delete(schedule);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "移除排班成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/off/shelf/scheduled")]
    [Transactional]
    public IActionResult Scheduled(IList<int> idList, int userId) {
        if (idList == null || idList.Count == 0) {
            throw new ValidationException("请选择要排班的数据！");
        }

        UserInfo user = session.Get<UserInfo>(userId);

        if (user == null) {
            throw new ValidationException("指定的用户不存在！");
        }

        IList<ExpressInfo> schedulingList = session.Query<ExpressInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (var e in schedulingList) {
            var schedule = new ExpressShelfScheduleInfo {
                Express = e,
                User = user,
                ScheduleType = ExpressShelfScheduleType.OffShelf
            };

            session.Save(schedule);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "排班成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/off/shelf/scheduled/by/query")]
    [Transactional]
    public IActionResult ScheduledByQuery(ExpressListQueryParam queryParam, int userId) {
        IList<ExpressInfo> schedulingList = expressListService.List(ExpressListMenuCate.ToOffShelfSchedule, queryParam);

        if (schedulingList.Count == 0) {
            throw new ValidationException("指定查询条件的待排班数据不存在！");
        }

        var user = session.Get<UserInfo>(userId);
        if (user == null) {
            throw new ValidationException($"指定的用户#{userId}不存在！");
        }

        foreach (var e in schedulingList) {
            var schedule = new ExpressShelfScheduleInfo {
                Express = e,
                User = user,
                ScheduleType = ExpressShelfScheduleType.OffShelf
            };

            session.Save(schedule);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "排班成功！",
        });
    }
}
