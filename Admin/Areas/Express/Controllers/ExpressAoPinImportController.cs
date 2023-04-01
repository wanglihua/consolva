using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Admin.Areas.Express.Models.ExpressAoPinImport;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Customer;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressAoPinImportController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly CheckinTrackingNoService checkinTrackingNoService;
    private readonly CustomerAoPinIdExcelService customerAoPinIdExcelService;
    private readonly ExpressCompanyService expressCompanyService;

    public ExpressAoPinImportController(
        ISession session,
        LoginUserService loginUserService,
        CheckinTrackingNoService checkinTrackingNoService,
        CustomerAoPinIdExcelService customerAoPinIdExcelService,
        ExpressCompanyService expressCompanyService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.checkinTrackingNoService = checkinTrackingNoService;
        this.customerAoPinIdExcelService = customerAoPinIdExcelService;
        this.expressCompanyService = expressCompanyService;
    }

    [HttpGet]
    [Route("/admin/express/ao/pin/import")]
    public IActionResult Index() {
        return View("ExpressAoPinImportIndex");
    }

    [HttpPost]
    [Route("/admin/express/ao/pin/import/do")]
    [Transactional]
    public IActionResult Import(IFormFile formFile) {
        var streamReader = new StreamReader(formFile.OpenReadStream());
        string json = streamReader.ReadToEnd();
        var expressImportList = JsonConvert.DeserializeObject<List<ExpressAoPinImportExpress>>(json);

        if (expressImportList == null) {
            throw new ValidationException("未能解析接受数据！");
        }

        var loginUser = loginUserService.GetLoginUser();

// #if DEBUG
// #else
// #endif

        int warehouseId = 1;
        var warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException($"仓库#{warehouseId}不存在！");
        }

        var aoPinCustomerList = session.Query<CustomerInfo>().Where(e => e.AoPinId != 0).ToList();

        foreach (var ei in expressImportList) {
            string trackingNo = ei.TrackingNo;
            var expressInDb = session.Query<ExpressInfo>().Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();
            if (expressInDb != null) {
                continue;
            }

            ExpressStatus expressStatus = ExpressStatus.WaitPick;
            if (ei.CustomerId != 0) {
                expressStatus = ExpressStatus.Checkin;
            }

            int aoPinCustomerId = ei.CustomerId;
            var customer = aoPinCustomerList.Where(e => e.AoPinId == aoPinCustomerId).Take(1).SingleOrDefault();
            if (customer == null) {
                throw new ValidationException($"澳拼客户ID#{aoPinCustomerId}在系统中没有对应客户数据！");
            }

            int aoPinBillCustomerId = customerAoPinIdExcelService.GetNumberFromStr(ei.BillCustomerCode);
            var billCustomer = aoPinCustomerList.Where(e => e.AoPinId == aoPinBillCustomerId).Take(1).SingleOrDefault();
            if (billCustomer == null) {
                throw new ValidationException($"澳拼客户ID#{aoPinBillCustomerId}在系统中没有对应客户数据！");
            }

            var express = new ExpressInfo {
                TrackingNo = ei.TrackingNo,
                CheckinTrackingNo = checkinTrackingNoService.GetNextMainTrackinigNo(),
                Status = expressStatus,
                ShelfStatus = ExpressShelfStatus.ToOnShelf,
                AddTime = ei.AddTime,
                ClaimTime = ei.ClaimTime,
                BillCustomerCode = ei.BillCustomerCode,
                BillCustomerId = billCustomer.Id,
                Warehouse = warehouse,
                ExpressCompany = expressCompanyService.GetExpressCompanyName(ei.TrackingNo),
                GoodsType = GetGoodsType(ei.GoodsTypeId),
                AddUser = loginUser,
                PackageCount = ei.PackageCount,
                
            };
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    private LogisticsGoodsType GetGoodsType(int aoPinGoodsTypeId) {
        if (aoPinGoodsTypeId == 100) {
            return LogisticsGoodsType.Common;
        }

        if (aoPinGoodsTypeId == 380) {
            return LogisticsGoodsType.Brand;
        }

        if (aoPinGoodsTypeId == 400) {
            return LogisticsGoodsType.Forbidden;
        }

        return LogisticsGoodsType.None;
    }
}