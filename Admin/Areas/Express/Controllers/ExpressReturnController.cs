using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.Express.Models.ExpressReturn;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressReturn)]
public class ExpressReturnController : Controller {
    private readonly ISession session;
    private readonly WarehouseService warehouseService;

    public ExpressReturnController(ISession session, WarehouseService warehouseService) {
        this.session = session;
        this.warehouseService = warehouseService;
    }

    [HttpGet]
    [Route("/admin/express/return")]
    public IActionResult Index() {
        ExpressReturnIndexModel model = new ExpressReturnIndexModel {
            WarehouseList = warehouseService.GetWarehouseListForAdmin(),
            ReturnStatusList = ExpressStatus.GetReturnList(),
        };

        return View("ExpressReturnIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/express/return/list")]
    public IActionResult List(ExpressReturnListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<ExpressReturnInfo>(e => expressReturnAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => expressReturnAlias!.Id).Desc;

        IList<ExpressReturnInfo> expressReturnList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<ExpressReturnInfo>();

        /*
        foreach (ExpressReturnInfo expressReturn in expressReturnList) {
        }
        */

        ExpressReturnListModel model = new ExpressReturnListModel(listPager);
        model.ExpressReturnList = expressReturnList;

        model.ExpressReturnAlias = nameof(expressReturnAlias);
        model.ExpressAlias = nameof(expressAlias);
        model.AddUserAlias = nameof(addUserAlias);
        model.WarehouseAlias = nameof(warehouseAlias);
        model.CustomerAlias = nameof(customerAlias);
        model.ReturnUserAlias = nameof(returnUserAlias);

        return View("ExpressReturnList", model);
    }

    private ExpressReturnInfo? expressReturnAlias = null;
    private ExpressInfo? expressAlias = null;
    private UserInfo? addUserAlias = null;
    private WarehouseInfo? warehouseAlias = null;
    private CustomerInfo? customerAlias = null;
    private CustomerInfo? returnUserAlias = null;

    [NonAction]
    private IQueryOver<ExpressReturnInfo, ExpressReturnInfo> GetQueryOver(ExpressReturnListQueryParam queryParam) {
        var query = session.QueryOver<ExpressReturnInfo>(() => expressReturnAlias!)
            .JoinAlias(() => expressReturnAlias!.Express, () => expressAlias)
            .JoinAlias(() => expressReturnAlias!.AddUser, () => addUserAlias)
            .JoinAlias(() => expressReturnAlias!.Warehouse, () => warehouseAlias)
            .JoinAlias(() => expressReturnAlias!.Customer, () => customerAlias, JoinType.LeftOuterJoin)
            .JoinAlias(() => expressReturnAlias!.ReturnUser, () => returnUserAlias, JoinType.LeftOuterJoin);

        return query;
    }

    [HttpPost]
    [Route("/admin/express/express/return/detail")]
    public IActionResult Detail(IdParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        var id = param.Id;

        ExpressReturnInfo? expressReturn;

        if (id <= 0) {
            expressReturn = new ExpressReturnInfo();
        } else {
            expressReturn = session.Get<ExpressReturnInfo>(id);
            if (expressReturn == null) {
                throw new ValidationException($"指定的 快递退运#{id} 不存在！");
            }
        }

        ExpressReturnDetailModel model = new ExpressReturnDetailModel();
        model.ExpressReturnId = expressReturn.Id;

        model.ExpressId = expressReturn.Express?.Id ?? 0;
        model.TrackingNo = expressReturn.TrackingNo;
        model.CheckinTrackingNo = expressReturn.CheckinTrackingNo;
        model.ExpressCompany = expressReturn.ExpressCompany;
        model.GoodsType = expressReturn.GoodsType;
        model.AddTime = expressReturn.AddTime;
        model.AddUserId = expressReturn.AddUser?.Id ?? 0;
        model.WarehouseId = expressReturn.Warehouse?.Id ?? 0;
        model.CustomerId = expressReturn.Customer?.Id ?? 0;
        model.Status = expressReturn.Status;
        model.ReturnTrackingNo = expressReturn.ReturnTrackingNo;
        model.ReturnExpressCompany = expressReturn.ReturnExpressCompany;
        model.ReturnApplyTime = expressReturn.ReturnApplyTime;
        model.ReturnPickupCode = expressReturn.ReturnPickupCode;
        model.ReturnUserId = expressReturn.ReturnUser?.Id ?? 0;
        model.ReturnTime = expressReturn.ReturnTime;

        model.GoodsTypeList = LogisticsGoodsType.GetInnerList();
        model.StatusList = ExpressStatus.GetList();

        return View("ExpressReturnDetail", model);
    }

    [HttpPost]
    [Route("/admin/express/express/return/save")]
    [Transactional]
    public IActionResult Save(ExpressReturnSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int expressReturnId = model.ExpressReturnId;

        bool isCreate = expressReturnId <= 0;

        ExpressReturnInfo expressReturn;

        if (isCreate) {
            expressReturn = new ExpressReturnInfo();
        } else {
            expressReturn = session.Get<ExpressReturnInfo>(expressReturnId);
            if (expressReturn == null) {
                throw new ValidationException($"指定的 快递退运#{expressReturnId} 不存在！");
            }
        }

        ExpressInfo express = session.Get<ExpressInfo>(model.ExpressId);
        UserInfo addUser = session.Get<UserInfo>(model.AddUserId);
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(model.WarehouseId);
        CustomerInfo customer = session.Get<CustomerInfo>(model.CustomerId);
        CustomerInfo returnUser = session.Get<CustomerInfo>(model.ReturnUserId);
        expressReturn.Express = express;
        expressReturn.TrackingNo = model.TrackingNo;
        expressReturn.CheckinTrackingNo = model.CheckinTrackingNo;
        expressReturn.ExpressCompany = model.ExpressCompany;
        expressReturn.GoodsType = model.GoodsType;
        expressReturn.AddTime = model.AddTime;
        expressReturn.AddUser = addUser;
        expressReturn.Warehouse = warehouse;
        expressReturn.Customer = customer;
        expressReturn.Status = model.Status;
        expressReturn.ReturnTrackingNo = model.ReturnTrackingNo;
        expressReturn.ReturnExpressCompany = model.ReturnExpressCompany;
        expressReturn.ReturnApplyTime = model.ReturnApplyTime;
        expressReturn.ReturnPickupCode = model.ReturnPickupCode?? string.Empty;
        expressReturn.ReturnUser = returnUser;
        expressReturn.ReturnTime = model.ReturnTime;

        if (isCreate) {
            session.Save(expressReturn);
        } else {
            session.Update(expressReturn);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}