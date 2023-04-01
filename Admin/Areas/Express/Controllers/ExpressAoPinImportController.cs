using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
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
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly CheckinTrackingNoService checkinTrackingNoService;
    private readonly CustomerAoPinIdExcelService customerAoPinIdExcelService;
    private readonly ExpressCompanyService expressCompanyService;
    private readonly ExpressLogService expressLogService;

    public ExpressAoPinImportController(
        ISession session,
        LoginUserService loginUserService,
        CheckinTrackingNoService checkinTrackingNoService,
        CustomerAoPinIdExcelService customerAoPinIdExcelService,
        ExpressCompanyService expressCompanyService,
        ExpressLogService expressLogService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.checkinTrackingNoService = checkinTrackingNoService;
        this.customerAoPinIdExcelService = customerAoPinIdExcelService;
        this.expressCompanyService = expressCompanyService;
        this.expressLogService = expressLogService;
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

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

#if DEBUG
        int storageLocId = 5;
        int shelfId = 3;
#else
        int storageLocId = 457;
        int shelfId = 6;
#endif

        int warehouseId = 1;
        var warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException($"仓库#{warehouseId}不存在！");
        }

        var shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            throw new ValidationException($"指定的货架#{shelfId}不存在！");
        }

        var storageLoc = session.Get<StorageLocInfo>(storageLocId);
        if (storageLoc == null) {
            throw new ValidationException($"指定的仓位#{storageLocId}不存在！");
        }

        var aoPinCustomerList = session.Query<CustomerInfo>().Where(e => e.AoPinId != 0).ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var ei in expressImportList) {
            foreach (var gi in ei.GoodsList) {
                var goodsList = session.Query<LogisticsGoodsInfo>().Where(e => e.Name == gi.GoodsName).ToList();
                if (goodsList.Count == 0) {
                    throw new ValidationException($"指定的物品名称#{gi.GoodsName}不存在！");
                }

                LogisticsGoodsType goodsType = GetGoodsType(gi.GoodsTypeId);
                var goods = goodsList.Where(e => e.Type == goodsType).Take(1).SingleOrDefault();
                if (goods == null) {
                    goods = goodsList.Take(1).SingleOrDefault();
                }

                if (goods == null) {
                    sb.Append(gi.GoodsName);

                    string goodsTypeName = "普通类";
                    if (gi.GoodsTypeId == 380) {
                        goodsTypeName = "特货";
                    } else {
                        goodsTypeName = "禁止类";
                    }

                    sb.Append(" " + goodsTypeName);
                    sb.Append(Environment.NewLine);
                }
            }
        }

        string goodsNamesNotExist = sb.ToString();
        if (string.IsNullOrWhiteSpace(goodsNamesNotExist) == false) {
            log.Error("指定的物品名称不存在！");
            log.Error(goodsNamesNotExist);
            throw new ValidationException($"指定的物品名称#{goodsNamesNotExist}不存在！");
        }

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
                // throw new ValidationException($"澳拼客户ID#{aoPinCustomerId}在系统中没有对应客户数据！");
            }

            int aoPinBillCustomerId = 0;
            if (string.IsNullOrWhiteSpace(ei.BillCustomerCode) == false) {
                aoPinBillCustomerId = customerAoPinIdExcelService.GetNumberFromStr(ei.BillCustomerCode);
            }

            var billCustomer = aoPinCustomerList.Where(e => e.AoPinId == aoPinBillCustomerId).Take(1).SingleOrDefault();
            // if (billCustomer == null) {
            //     throw new ValidationException($"澳拼客户ID#{aoPinBillCustomerId}在系统中没有对应客户数据！");
            // }

            int billCustomerId = 0;
            if (billCustomer != null) {
                billCustomerId = billCustomer?.Id ?? 0;
            }

            var express = new ExpressInfo {
                TrackingNo = ei.TrackingNo,
                CheckinTrackingNo = checkinTrackingNoService.GetNextMainTrackinigNo(),
                Status = expressStatus,
                ShelfStatus = ExpressShelfStatus.ToOnShelf,
                AddTime = ei.AddTime,
                ClaimTime = ei.ClaimTime,
                Customer = customer,
                BillCustomerCode = "澳拼" + ei.BillCustomerCode,
                BillCustomerId = billCustomerId,
                Warehouse = warehouse,
                ExpressCompany = expressCompanyService.GetExpressCompanyName(ei.TrackingNo),
                GoodsType = GetGoodsType(ei.GoodsTypeId),
                AddUser = loginUser,
                PackageCount = ei.PackageCount,
                Shelf = shelf,
                StorageLoc = storageLoc,
            };

            session.Save(express);
            session.Flush();

            foreach (var gi in ei.GoodsList) {
                var goodsList = session.Query<LogisticsGoodsInfo>().Where(e => e.Name == gi.GoodsName).ToList();
                if (goodsList.Count == 0) {
                    throw new ValidationException($"指定的物品名称#{gi.GoodsName}不存在！");
                }

                LogisticsGoodsType goodsType = GetGoodsType(gi.GoodsTypeId);
                var goods = goodsList.Where(e => e.Type == goodsType).Take(1).SingleOrDefault();
                if (goods == null) {
                    goods = goodsList.Take(1).SingleOrDefault();
                }

                if (goods == null) {
                    throw new ValidationException($"指定的物品名称#{gi.GoodsName}不存在！");
                }

                var expressGoods = new ExpressGoodsInfo {
                    Express = express,
                    Goods = goods,
                    GoodsType = goodsType,
                    Quantity = gi.Quantity,
                    Unit = gi.Unit,
                };

                session.Save(expressGoods);
            }

            session.Flush();

            if (ei.SizeWeightList != null) {
                foreach (var sw in ei.SizeWeightList) {
                    var expressSizeWeight = new ExpressSizeWeightInfo();

                    expressSizeWeight.TrackingNo = ei.TrackingNo; // trackingNoMain 母单号
                    expressSizeWeight.TrackingNoSub = string.Empty;
                    expressSizeWeight.Length = sw.Length;
                    expressSizeWeight.Width = sw.Width;
                    expressSizeWeight.Height = sw.Height;
                    expressSizeWeight.Weight = sw.Weight;
                    expressSizeWeight.PackageCount = sw.PackageCount;

                    // 机器区分
                    expressSizeWeight.Machine = null;

                    expressSizeWeight.Express = express;
                    expressSizeWeight.Order = null;
                    expressSizeWeight.OrderCode = string.Empty;

                    session.Save(expressSizeWeight);
                }

                session.Flush();
            }

            expressLogService.UserLog(express.Id, ExpressLogType.AoPinImport, loginUserId);
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