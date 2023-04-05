using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressOffShelfSchedule)]
public class ExpressOffShelfScheduleController : Controller {

    private readonly ISession session;

    public ExpressOffShelfScheduleController(ISession session) {
        this.session = session;
    }
}

/*

using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Hibernate;
using Core.Util;
using Domain.Logistics;
using Domain.LogisticsOrder;
using Domain.Pass;
using Domain.Warehouse;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using Service.Express;
using Web.Areas.Admin.Models.ExpressOffShelfSchedule;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.ExpressManage.ExpressOffShelfSchedule)]
    public class ExpressOffShelfScheduleController : Controller
    {
        private ISession session = null;
        private ExpressClaimService expressClaimService = null;
        private ExpressShelfScheduleService expressShelfScheduleService = null;

        public ExpressOffShelfScheduleController(ISession session,
            ExpressClaimService expressClaimService,
            ExpressShelfScheduleService expressShelfScheduleService)
        {
            this.session = session;
            this.expressClaimService = expressClaimService;
            this.expressShelfScheduleService = expressShelfScheduleService;
        }

        [HttpGet]
        [Route("/admin/express/off/shelf/schedule/{warehouseId:int}")]
        [NotTransactional]
        public IActionResult Index(int warehouseId)
        {
            WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
            if (warehouse == null)
            {
                throw new ValidationException($"指定的仓库#{warehouseId}不存在！");
            }

            ExpressOffShelfScheduleIndexModel model = new ExpressOffShelfScheduleIndexModel();

            model.OffShelfUserList = session.Query<WarehouseWorkerInfo>()
                .Fetch(e => e.User)
                .Where(e => e.Warehouse.Id == warehouseId && e.Type == WarehouseWorkerType.OffShelf)
                .OrderBy(e => e.User.LoginName)
                .Select(e => e.User)
                .ToList();

            IList<ShelfInfo> shelfList = expressClaimService.GetShelfList(ExpressClaimCate.ToOffShelfSchedule, warehouseId);

            model.Warehouse = warehouse;
            model.ShelfList = shelfList;

            model.TargetWarehouseList = session.Query<WarehouseInfo>()
                .Where(e => e.Type == WarehouseType.Au
                            && e.InnerStatus == WarehouseStatus.Enabled)
                .OrderBy(e => e.OrderNum)
                .ThenBy(e => e.Id)
                .ToList();

            IList<LogisticsModeType> firstModeTypeList = LogisticsModeType.GetCnAuFirstTypeList();
            model.FirstModeList = session.Query<LogisticsModeInfo>()
                .Where(e => e.Cate == LogisticsModeCate.Customer
                            && firstModeTypeList.Contains(e.Type)
                            && e.InnerStatus != LogisticsModeStatus.Disabled)
                .OrderBy(e => e.Name)
                .ThenBy(e => e.Id)
                .ToList();

            return View("ExpressOffShelfScheduleIndex", model);
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduling/first/mode/select")]
        [NotTransactional]
        public IActionResult SchedulingFirstModeSelect(int targetWarehouseId)
        {
            ExpressOffShelfScheduleIndexModel model = new ExpressOffShelfScheduleIndexModel();

            var firstLogisticsModeTypeList = LogisticsModeType.GetCnAuFirstTypeList();

            var query = session.Query<LogisticsModeInfo>()
                .Where(e => e.Cate == LogisticsModeCate.Customer
                            && firstLogisticsModeTypeList.Contains(e.Type)
                            && e.InnerStatus != LogisticsModeStatus.Disabled);

            if (targetWarehouseId != 0)
            {
                query = query.Where(e => e.WarehouseTo.Id == targetWarehouseId);
            }

            model.FirstModeList = query
                .OrderBy(e => e.Name)
                .ToList();

            return View("ExpressOffShelfSchedulingFirstModeSelect", model);
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduled/first/mode/select")]
        [NotTransactional]
        public IActionResult ScheduledFirstModeSelect(int targetWarehouseId)
        {
            ExpressOffShelfScheduleIndexModel model = new ExpressOffShelfScheduleIndexModel();

            var firstLogisticsModeTypeList = LogisticsModeType.GetCnAuFirstTypeList();

            var query = session.Query<LogisticsModeInfo>()
                .Where(e => e.Cate == LogisticsModeCate.Customer
                            && firstLogisticsModeTypeList.Contains(e.Type)
                            && e.InnerStatus != LogisticsModeStatus.Disabled);

            if (targetWarehouseId != 0)
            {
                query = query.Where(e => e.WarehouseTo.Id == targetWarehouseId);
            }

            model.FirstModeList = query
                .OrderBy(e => e.Name)
                .ToList();

            return View("ExpressOffShelfScheduledFirstModeSelect", model);
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduling/list")]
        [NotTransactional]
        public IActionResult SchedulingList(ExpressClaimListQueryParam queryParam)
        {
            int totalCount = expressClaimService.Count(ExpressClaimCate.ToOffShelfSchedule, queryParam);
            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<LogisticsClaimGoodsInfo> schedulingList = expressClaimService.List(ExpressClaimCate.ToOffShelfSchedule, queryParam, listPager);

            ExpressOffShelfSchedulingListModel model = new ExpressOffShelfSchedulingListModel(listPager);
            model.SchedulingList = schedulingList;

            // IList<ShelfInfo> shelfList = expressClaimService.GetShelfList(ExpressClaimCate.ToOffShelfSchedule, queryParam);
            // model.ShelfList = shelfList;

            return View("ExpressOffShelfSchedulingList", model);
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduled/list")]
        [NotTransactional]
        public IActionResult ScheduledList(ExpressClaimListQueryParam queryParam)
        {
            int totalCount = expressShelfScheduleService.Count(ExpressShelfScheduleType.OffShelf, queryParam);
            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<ExpressShelfScheduleInfo> scheduledList = expressShelfScheduleService.List(ExpressShelfScheduleType.OffShelf, queryParam, listPager);

            ExpressOffShelfScheduledListModel model = new ExpressOffShelfScheduledListModel(listPager);
            model.ScheduledList = scheduledList;

            return View("ExpressOffShelfScheduledList", model);
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduling")]
        public IActionResult Scheduling(IList<int> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                throw new ValidationException("请选择要移除排班的数据！");
            }

            session.Query<ExpressShelfScheduleInfo>()
                .Where(e => e.ScheduleType == ExpressShelfScheduleType.OffShelf && idList.Contains(e.Id))
                .Delete();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "移除排班成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduling/by/query")]
        public IActionResult SchedulingByQuery(ExpressClaimListQueryParam queryParam)
        {
            IList<ExpressShelfScheduleInfo> scheduledList = expressShelfScheduleService.List(ExpressShelfScheduleType.OffShelf, queryParam);

            if (scheduledList.Count == 0)
            {
                throw new ValidationException("指定查询条件的排班数据不存在！");
            }

            foreach (ExpressShelfScheduleInfo schedule in scheduledList)
            {
                session.Delete(schedule);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "移除排班成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduled")]
        public IActionResult Scheduled(IList<int> idList, int userId)
        {
            if (idList == null || idList.Count == 0)
            {
                throw new ValidationException("请选择要排班的数据！");
            }

            UserInfo user = session.Get<UserInfo>(userId);

            if (user == null)
            {
                throw new ValidationException("指定的用户不存在！");
            }

            IList<LogisticsClaimGoodsInfo> schedulingList = session.Query<LogisticsClaimGoodsInfo>()
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (LogisticsClaimGoodsInfo cg in schedulingList)
            {
                ExpressShelfScheduleInfo schedule = new ExpressShelfScheduleInfo();
                schedule.Claim = cg;
                schedule.User = user;
                schedule.ScheduleType = ExpressShelfScheduleType.OffShelf;

                session.Save(schedule);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "排班成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/express/off/shelf/scheduled/by/query")]
        public IActionResult ScheduledByQuery(ExpressClaimListQueryParam queryParam, int userId)
        {
            IList<LogisticsClaimGoodsInfo> schedulingList = expressClaimService.List(ExpressClaimCate.ToOffShelfSchedule, queryParam);

            if (schedulingList.Count == 0)
            {
                throw new ValidationException("指定查询条件的待排班数据不存在！");
            }

            UserInfo user = session.Get<UserInfo>(userId);
            if (user == null)
            {
                throw new ValidationException("指定的用户不存在！");
            }

            foreach (LogisticsClaimGoodsInfo cg in schedulingList)
            {
                ExpressShelfScheduleInfo schedule = new ExpressShelfScheduleInfo();
                schedule.Claim = cg;
                schedule.User = user;
                schedule.ScheduleType = ExpressShelfScheduleType.OffShelf;

                session.Save(schedule);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "排班成功！"},
            });
        }
    }
}

*/
