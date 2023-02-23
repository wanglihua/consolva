using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Setting.Models.AutoMeasureMachine;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

[SettingArea]
[AdminAuth(PassRes.Setting.AutoMeasureMachine)]
public class AutoMeasureMachineController : Controller {
    private readonly ISession session;

    public AutoMeasureMachineController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/auto/measure/machine")]
    public IActionResult Index() {
        AutoMeasureMachineIndexModel model = new AutoMeasureMachineIndexModel();


        return View("AutoMeasureMachineIndex", model);
    }

    [HttpPost]
    [Route("/admin/auto/measure/machine/list")]
    public IActionResult List(AutoMeasureMachineListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<AutoMeasureMachineInfo> autoMeasureMachineList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<AutoMeasureMachineInfo>();

        foreach (AutoMeasureMachineInfo autoMeasureMachine in autoMeasureMachineList) {
        }

        AutoMeasureMachineListModel model = new AutoMeasureMachineListModel(listPager);
        model.AutoMeasureMachineList = autoMeasureMachineList;

        return View("AutoMeasureMachineList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(AutoMeasureMachineListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<AutoMeasureMachineInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(AutoMeasureMachineInfo.Warehouse), nameof(AutoMeasureMachineInfo.Warehouse));

        return criteria;
    }

    [HttpPost]
    [Route("/admin/auto/measure/machine/detail")]
    public IActionResult Detail(int id) {
        session.DefaultReadOnly = true;

        AutoMeasureMachineInfo? autoMeasureMachine = null;

        if (id == 0) {
            autoMeasureMachine = new AutoMeasureMachineInfo();
        } else {
            autoMeasureMachine = session.Get<AutoMeasureMachineInfo>(id);
            if (autoMeasureMachine == null) {
                throw new ValidationException($"指定的 自动秤设备#{id} 不存在！");
            }
        }

        AutoMeasureMachineDetailModel model = new AutoMeasureMachineDetailModel();

        model.Name = autoMeasureMachine.Name;
        model.Description = autoMeasureMachine.Description;
        model.WarehouseId = autoMeasureMachine.Warehouse?.Id ?? 0;
        model.AppKey = autoMeasureMachine.AppKey;
        model.Secret = autoMeasureMachine.Secret;

        model.WarehouseList = session.Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ToList();

        return View("AutoMeasureMachineDetail", model);
    }

    [HttpPost]
    [Route("/admin/auto/measure/machine/save")]
    [Transactional]
    public IActionResult Save(AutoMeasureMachineSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int autoMeasureMachineId = model.AutoMeasureMachineId;

        bool isCreate = autoMeasureMachineId == 0;

        AutoMeasureMachineInfo? autoMeasureMachine = null;

        if (isCreate) {
            autoMeasureMachine = new AutoMeasureMachineInfo();
        } else {
            autoMeasureMachine = session.Get<AutoMeasureMachineInfo>(autoMeasureMachineId);
            if (autoMeasureMachine == null) {
                throw new ValidationException($"指定的 自动秤设备#{autoMeasureMachineId} 不存在！");
            }
        }

        var warehouse = session.Get<WarehouseInfo>(model.WarehouseId);
        if (warehouse == null) {
            throw new ValidationException($"指定的仓库#{model.WarehouseId}不存在！");
        }

        autoMeasureMachine.Name = model.Name;
        autoMeasureMachine.Description = model.Description ?? string.Empty;
        autoMeasureMachine.Warehouse = warehouse;
        autoMeasureMachine.AppKey = model.AppKey;
        autoMeasureMachine.Secret = model.Secret;

        if (isCreate) {
            session.Save(autoMeasureMachine);
        } else {
            session.Update(autoMeasureMachine);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/auto/measure/machine/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (int id in idList) {
            AutoMeasureMachineInfo autoMeasureMachine = session.Load<AutoMeasureMachineInfo>(id);
            session.Delete(autoMeasureMachine);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}