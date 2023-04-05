using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using shunda.Admin.Areas.Express.Models.ExpressList;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Customer;
using shunda.Service.Express;
using shunda.Service.Logistics;
using shunda.Service.Pass;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

/// <summary>
/// 快递详情
/// </summary>
[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressListController : Controller {
    private readonly ISession session;
    private readonly ExpressListService expressListService;
    private readonly LoginUserService loginUserService;
    private readonly ExpressExcelService expressExcelService;
    private readonly ExpressService expressService;
    private readonly WarehouseService warehouseService;
    private readonly PassResService passResService;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public ExpressListController(ISession session,
        ExpressListService expressListService,
        LoginUserService loginUserService,
        ExpressExcelService expressExcelService,
        ExpressService expressService,
        WarehouseService warehouseService,
        PassResService passResService,
        LogisticsModeSettingService logisticsModeSettingService
    ) {
        this.session = session;
        this.expressListService = expressListService;
        this.loginUserService = loginUserService;
        this.expressExcelService = expressExcelService;
        this.expressService = expressService;
        this.warehouseService = warehouseService;
        this.passResService = passResService;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    [HttpGet]
    [Route("/admin/express")]
    public IActionResult Index() {
        ExpressIndexModel model = new ExpressIndexModel();

        model.WarehouseList = warehouseService.GetWarehouseListWithCacheForAdmin();

        model.CheckinUserList = session.Query<WarehouseWorkerInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Type == WarehouseWorkerType.Checkin)
            .OrderBy(e => e.Id)
            .Select(e => e.User)
            .ToList();

        int loginUserId = loginUserService.GetLoginUserId();
        model.CanExpressForceDelete = passResService.UserHasPassRes(loginUserId, PassRes.Express.ExpressForceDelete);
        model.CanExpressThrowAway = passResService.UserHasPassRes(loginUserId, PassRes.Express.ExpressThrowAway);

        return View("ExpressIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/list")]
    public IActionResult List(ExpressListQueryParam queryParam) {
        int totalCount = expressListService.Count(ExpressListMenuCate.All, queryParam);
        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.All, queryParam, listPager);

        ExpressListModel model = new ExpressListModel(listPager);
        model.ExpressList = expressList;
        model.VolumeWeightDenominator = logisticsModeSettingService.GetSetting().VolumeWeightDenominator;

        return View("ExpressList", model);
    }

    [HttpPost]
    [Route("/admin/express/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("请选择要删除的快递单！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        var expressList = session.Query<ExpressInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (int id in idList) {
            var express = expressList.Where(e => e.Id == id).Take(1).SingleOrDefault();
            if (express == null) {
                throw new ValidationException($"指定的快递单#{id}不存在！");
            }

            expressService.Delete(id, loginUserId);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/force/delete")]
    [AdminAuth(PassRes.Express.ExpressForceDelete)]
    [Transactional]
    public IActionResult ForceDelete(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("请选择要删除的快递单！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        var expressList = session.Query<ExpressInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (int id in idList) {
            var express = expressList.Where(e => e.Id == id).Take(1).SingleOrDefault();
            if (express == null) {
                throw new ValidationException($"指定的快递单#{id}不存在！");
            }

            expressService.ForceDelete(id, loginUserId);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/throw/away")]
    [AdminAuth(PassRes.Express.ExpressThrowAway)]
    [Transactional]
    public IActionResult ThrowAway(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("请选择要设置已丢弃的快递单！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        var expressList = session.Query<ExpressInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (int id in idList) {
            var express = expressList.Where(e => e.Id == id).Take(1).SingleOrDefault();
            if (express == null) {
                throw new ValidationException($"指定的快递单#{id}不存在！");
            }

            expressService.ThrowAway(id, loginUserId);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "设置已丢弃成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/export")]
    public IActionResult Export(ExpressListQueryParam queryParam) {
        int totalCount = expressListService.Count(ExpressListMenuCate.All, queryParam);
        if (totalCount > 10000) // 暂时不能导出超1万个吧
        {
            throw new ValidationException("导出的快递单号太多了，请缩小查询条件！");
        }

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.All, queryParam);

        string fileName = $"快递单导出.xlsx";
        var excel = expressExcelService.Export(expressList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}