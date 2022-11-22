using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.ReceivePack.Models.ExpressRemarkPresetSelect;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

/// <summary>
/// 快递单备注预设 控制器
/// </summary>
[ReceivePackArea]
// [ExpressAuth(PassRes.XXX.ExpressRemarkPresetSelect)]
public class ExpressRemarkPresetSelectController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ExpressRemarkPresetSelectInfo? expressRemarkPresetSelectAlias = null;

    private readonly ISession session;

    public ExpressRemarkPresetSelectController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/express/remark/preset/select")]
    public IActionResult Index()
    {
        var model = new ExpressRemarkPresetSelectIndexModel();

        model.TypeList = ExpressRemarkType.GetList();

        return View("ExpressRemarkPresetSelectIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/remark/preset/select/list")]
    public IActionResult List(ExpressRemarkPresetSelectListQueryParam queryParam)
    {
        var totalCount = GetQueryOver(queryParam)
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending)
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        else
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;

        listQuery = listQuery.ThenByAlias(() => expressRemarkPresetSelectAlias!.Id).Desc;

        var expressRemarkPresetSelectList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<ExpressRemarkPresetSelectInfo>();

        /*
        foreach (ExpressRemarkPresetSelectInfo expressRemarkPresetSelect in expressRemarkPresetSelectList)
        {
        }
        */

        var model = new ExpressRemarkPresetSelectListModel(listPager);
        model.ExpressRemarkPresetSelectList = expressRemarkPresetSelectList;

        model.ExpressRemarkPresetSelectAlias = nameof(expressRemarkPresetSelectAlias);

        return View("ExpressRemarkPresetSelectList", model);
    }

    [NonAction]
    private IQueryOver<ExpressRemarkPresetSelectInfo, ExpressRemarkPresetSelectInfo> GetQueryOver(ExpressRemarkPresetSelectListQueryParam queryParam)
    {
        var query = session.QueryOver(() => expressRemarkPresetSelectAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/express/remark/preset/select/detail")]
    public IActionResult Detail(IdParam param)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var id = param.Id;

        ExpressRemarkPresetSelectInfo? expressRemarkPresetSelect;

        if (id <= 0)
        {
            expressRemarkPresetSelect = new ExpressRemarkPresetSelectInfo();
        }
        else
        {
            expressRemarkPresetSelect = session.Get<ExpressRemarkPresetSelectInfo>(id);
            if (expressRemarkPresetSelect == null) throw new ValidationException($"指定的 快递单备注预设#{id} 不存在！");
        }

        var model = new ExpressRemarkPresetSelectDetailModel();
        model.ExpressRemarkPresetSelectId = expressRemarkPresetSelect.Id;

        model.Type = expressRemarkPresetSelect.Type;
        model.Remark = expressRemarkPresetSelect.Remark;
        model.OrderNum = expressRemarkPresetSelect.OrderNum;

        model.TypeList = ExpressRemarkType.GetList();

        return View("ExpressRemarkPresetSelectDetail", model);
    }

    [HttpPost]
    [Route("/admin/express/remark/preset/select/save")]
    [Transactional]
    public IActionResult Save(ExpressRemarkPresetSelectSaveModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var expressRemarkPresetSelectId = model.ExpressRemarkPresetSelectId;

        var isCreate = expressRemarkPresetSelectId <= 0;

        ExpressRemarkPresetSelectInfo? expressRemarkPresetSelect;

        if (isCreate)
        {
            expressRemarkPresetSelect = new ExpressRemarkPresetSelectInfo();
        }
        else
        {
            expressRemarkPresetSelect = session.Get<ExpressRemarkPresetSelectInfo>(expressRemarkPresetSelectId);
            if (expressRemarkPresetSelect == null) throw new ValidationException($"指定的 快递单备注预设#{expressRemarkPresetSelectId} 不存在！");
        }

        expressRemarkPresetSelect.Type = model.Type;
        expressRemarkPresetSelect.Remark = model.Remark;
        expressRemarkPresetSelect.OrderNum = model.OrderNum;

        if (isCreate)
            session.Save(expressRemarkPresetSelect);
        else
            session.Update(expressRemarkPresetSelect);

        session.Flush();

        return Json(new {
            success = true,
            message = "保存成功！"
        });
    }

    [HttpPost]
    [Route("/admin/express/remark/preset/select/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param)
    {
        param.Validate("请选择要删除的快递单备注预设！");

        var idList = param.IdList;

        foreach (var id in idList)
        {
            var expressRemarkPresetSelect = session.Load<ExpressRemarkPresetSelectInfo>(id);
            session.Delete(expressRemarkPresetSelect);
        }

        session.Flush();

        return Json(new {
            success = true,
            message = "删除成功！"
        });
    }
}