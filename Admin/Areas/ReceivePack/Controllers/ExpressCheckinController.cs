using System.Text;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NLog;
using shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Message;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Customer;
using shunda.Service.Express;
using shunda.Service.Logistics;
using shunda.Service.Message.CustomerMessage;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

/// <summary>
/// 快递入库
/// </summary>
[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.ExpressCheckin)]
public class ExpressCheckinController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly ExpressCompanyService expressCompanyService;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressCheckinService expressCheckinService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;
    private readonly LogisticsGoodsTypeService logisticsGoodsTypeService;
    private readonly CheckinTrackingNoService checkinTrackingNoService;
    private readonly StorageLocService storageLocService;
    private readonly ExpressCheckinConfigService expressCheckinConfigService;
    private readonly CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService;
    private readonly CustomerMessageExpressCheckinService customerMessageExpressCheckinService;
    private readonly ExpressPhotoMessageService expressPhotoMessageService;
    private readonly ExpressLogService expressLogService;
    private readonly ExpressSizeWeightService expressSizeWeightService;
    private readonly ExpressPhotoService expressPhotoService;
    private readonly ExpressVideoService expressVideoService;
    private readonly StorageLocExpressService storageLocExpressService;

    public ExpressCheckinController(
        ISession session,
        ExpressCompanyService expressCompanyService,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressCheckinService expressCheckinService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService,
        LogisticsGoodsTypeService logisticsGoodsTypeService,
        CheckinTrackingNoService checkinTrackingNoService,
        StorageLocService storageLocService,
        ExpressCheckinConfigService expressCheckinConfigService,
        CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService,
        CustomerMessageExpressCheckinService customerMessageExpressCheckinService,
        ExpressPhotoMessageService expressPhotoMessageService,
        ExpressLogService expressLogService,
        ExpressSizeWeightService expressSizeWeightService,
        ExpressPhotoService expressPhotoService,
        ExpressVideoService expressVideoService,
        StorageLocExpressService storageLocExpressService
    ) {
        this.session = session;
        this.expressCompanyService = expressCompanyService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressCheckinService = expressCheckinService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
        this.logisticsGoodsTypeService = logisticsGoodsTypeService;
        this.checkinTrackingNoService = checkinTrackingNoService;
        this.storageLocService = storageLocService;
        this.expressCheckinConfigService = expressCheckinConfigService;
        this.customerMessageExpressExpectEnterService = customerMessageExpressExpectEnterService;
        this.customerMessageExpressCheckinService = customerMessageExpressCheckinService;
        this.expressPhotoMessageService = expressPhotoMessageService;
        this.expressLogService = expressLogService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.expressPhotoService = expressPhotoService;
        this.expressVideoService = expressVideoService;
        this.storageLocExpressService = storageLocExpressService;
    }

    [HttpGet]
    [Route("/admin/express/checkin/{warehouseId:int}")]
    public IActionResult Index(int warehouseId) {
        UserInfo loginUser = loginUserService.GetLoginUser();

        WarehouseInfo warehouseInfo = session.Get<WarehouseInfo>(warehouseId);
        if (warehouseInfo == null) {
            throw new ValidationException("指定的仓库不存在！");
        }

        LogisticsExpressCheckinIndexModel model = new LogisticsExpressCheckinIndexModel();
        model.Warehouse = warehouseInfo;
        model.CheckinLoginName = loginUser.LoginName;
        model.CheckinUserName = loginUser.UserName;

        var checkinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        model.CheckinPrintType = expressCheckinConfigService.GetLabelType(checkinConfig);
        model.CheckinPrintCount = expressCheckinConfigService.GetPrintCount(checkinConfig);

        return View("ExpressCheckinIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/checkin/express")]
    public IActionResult Express(int warehouseId, string trackingNo) {
        int loginUserId = loginUserService.GetLoginUserId();

        trackingNo = expressTrackingNoService.RemoveUselessSuffix(trackingNo);

        if (string.IsNullOrWhiteSpace(trackingNo) == false) {
            trackingNo = trackingNo.Trim().ToUpper();
        }

        // 查看已经存在的, 还在仓位上的 tracking no 并把信息显示在这里

        ExpressInfo? expressExist = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        LogisticsExpressCheckinExpressDetailModel model = new LogisticsExpressCheckinExpressDetailModel {
            WarehouseId = warehouseId,
            Warehouse = session.Get<WarehouseInfo>(warehouseId),
            TrackingNo = trackingNo
        };

        int? expressExistShelfId = null;
        StorageLocInfo? storageLocExist = null;
        if (expressExist != null) {
            int expressExistId = expressExist.Id;

            model.ExpressId = expressExist.Id;
            // model.GoodsType = expressExist.GoodsType;
            model.BillCustomerCode = expressExist.BillCustomerCode;
            model.ExpressCompany = expressExist.ExpressCompany;
            model.PackageCount = expressExist.PackageCount;

            // 仓库备注
            var expressWarehouseRemarkList = session.Query<ExpressRemarkInfo>()
                .Where(e => e.Express.Id == expressExistId && e.Type == ExpressRemarkType.Warehouse)
                .ToList();
            model.WarehouseRemark = string.Join(";", expressWarehouseRemarkList.Select(e => e.Content));

            int storageLocIdExist = expressExist.StorageLoc?.Id ?? 0;
            storageLocExist = session.Query<StorageLocInfo>()
                .Fetch(e => e.Shelf)
                .Fetch(e => e.Spec)
                .Where(e => e.Id == storageLocIdExist)
                .Take(1)
                .SingleOrDefault();

            expressExistShelfId = storageLocExist?.Shelf.Id;

            model.StorageLocName = storageLocExist?.GetFullName() ?? string.Empty;
        }

        ExpressCheckinUserShelfSelectInfo userSelect = expressCheckinService.GetUserShelfSelect(loginUserId);

        if ((expressExistShelfId ?? 0) != 0) {
            model.ShelfId = expressExistShelfId;
        } else {
            model.ShelfId = userSelect.Shelf.Id;
        }

        model.ShelfList = expressCheckinService.GetAvaibleShelfList(warehouseId, loginUserId, expressExistShelfId);

        if (model.ShelfId != null) {
            int shelfId = model.ShelfId ?? 0;
            model.StorageLocSpecList = GetShelfStorageLocSpecList(shelfId);
        }

        if (storageLocExist != null) {
            model.StorageLocSpecId = storageLocExist.Spec?.Id ?? 0;
        } else {
            model.StorageLocSpecId = userSelect?.StorageLocSpec?.Id ?? 0;
        }

        model.GoodsTypeList = LogisticsGoodsType.GetInnerList();

        model.RemarkPresetList = session.Query<ExpressRemarkPresetSelectInfo>().OrderBy(e => e.Id).ToList();

        // 物品列表
        if (expressExist != null) {
            int expressExistId = expressExist.Id;
            IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
                .Fetch(e => e.Goods)
                .Where(e => e.Express.Id == expressExistId)
                .OrderBy(e => e.Id)
                .ToList();

            IList<LogisticsExpressCheckinExpressGoodsModel> goodsModelList = new List<LogisticsExpressCheckinExpressGoodsModel>();

            foreach (ExpressGoodsInfo expressGoods in expressGoodsList) {
                LogisticsExpressCheckinExpressGoodsModel goodsModel = new LogisticsExpressCheckinExpressGoodsModel {
                    ExpressGoodsId = expressGoods.Id,
                    UiIndex = expressGoods.Id.ToString(),
                    GoodsId = expressGoods.Goods.Id,
                    GoodsName = expressGoods.Goods.Name,
                    GoodsType = expressGoods.GoodsType,
                    Quantity = expressGoods.Quantity,
                    Unit = expressGoods.Unit,
                    UnitList = GoodsUnit.GetList()
                };

                goodsModelList.Add(goodsModel);
            }

            model.GoodsList = goodsModelList;
        }

        // 尺寸重量数据 begin

        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        ExpressSizeWeightInfo? expressSizeWeight = expressSizeWeightList
            .Where(e => string.IsNullOrWhiteSpace(e.TrackingNoSub))
            .Take(1)
            .SingleOrDefault();

        model.ChildSizeWeightList = new List<LogisticsExpressCheckinSizeWeightChildItem>();

        if (expressSizeWeight != null) {
            model.Length = expressSizeWeight.Length;
            model.Width = expressSizeWeight.Width;
            model.Height = expressSizeWeight.Height;
            model.Weight = expressSizeWeight.Weight;
            model.ExpressPackageCount = expressSizeWeight.PackageCount;

            IList<ExpressSizeWeightInfo> expressChildList = expressSizeWeightList
                .Where(e => string.IsNullOrWhiteSpace(e.TrackingNoSub) == false)
                .OrderBy(e => e.Id)
                .ToList();

            IList<LogisticsExpressCheckinSizeWeightChildItem> childItemList = new List<LogisticsExpressCheckinSizeWeightChildItem>();
            foreach (ExpressSizeWeightInfo expressChild in expressChildList) {
                LogisticsExpressCheckinSizeWeightChildItem childItem = new LogisticsExpressCheckinSizeWeightChildItem {
                    ExpressChildId = expressChild.Id,
                    ParentTrackingNo = trackingNo,
                    TrackingNo = expressChild.TrackingNoSub.Replace(trackingNo + "-", ""),
                    Length = expressChild.Length,
                    Width = expressChild.Width,
                    Height = expressChild.Height,
                    Weight = expressChild.Weight,
                    PackageCount = expressChild.PackageCount
                };

                childItemList.Add(childItem);
            }

            model.ChildSizeWeightList = childItemList;
        }

        // 尺寸重量数据 end

        // 是不是已经预报的快递单号
        var expressExpect = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();
        model.ExpressExpect = expressExpect;

        var expressCheckinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        model.AutoRecordCheckinVideo = expressCheckinConfig.AutoRecordCheckinVideo;

        model.ExpressStatus = expressExist?.Status ?? ExpressStatus.None;

        return View("ExpressCheckinExpress", model);
    }

    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/storage/loc/spec/list")]
    public IActionResult StorageLocSpecList(int shelfId) {
        LogisticsExpressCheckinExpressDetailModel model = new LogisticsExpressCheckinExpressDetailModel();

        model.StorageLocSpecList = GetShelfStorageLocSpecList(shelfId);

        return View("ExpressCheckinStorageLocSpecList", model);
    }

    [NonAction]
    private IList<StorageLocSpecInfo> GetShelfStorageLocSpecList(int shelfId) {
        // 获取货架仓位规格列表
        IList<StorageLocSpecInfo> specList = session.Query<StorageLocInfo>()
            .Fetch(e => e.Spec)
            .Where(e => e.Shelf.Id == shelfId)
            .Select(e => e.Spec)
            .Distinct()
            .ToList();

        specList = specList.OrderBy(e => e.OrderNum).ThenBy(e => e.Length * e.Width * e.Height).ToList();

        return specList;
    }


    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/add/goods")]
    public IActionResult AddGoods() {
        LogisticsExpressCheckinExpressGoodsModel model = new LogisticsExpressCheckinExpressGoodsModel();
        model.UiIndex = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        model.UnitList = GoodsUnit.GetList();

        return View("ExpressCheckinExpressGoods", model);
    }

    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/check/size/weight/data")]
    [NoLoginRequired]
    public IActionResult CheckSizeWeightData(string expressTrackingNo) {
        IList<ExpressSizeWeightInfo> logisticsExpressList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();

        bool hasSizeWeightData = logisticsExpressList.Count != 0;

        bool hasShortLengthData = false;
        bool hasLightWeightData = false;
        if (hasSizeWeightData) // 如果尺寸重量信息都没有，太轻的尺寸重量信息自然就没有了
        {
            foreach (ExpressSizeWeightInfo e in logisticsExpressList) {
                if (e.Length < 1 || e.Width < 1 || e.Height < 1) {
                    hasShortLengthData = true;
                }

                if (e.Weight < 0.1m) {
                    hasLightWeightData = true;
                }
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "操作成功！" },
            { "hasData", hasSizeWeightData },
            { "tooShort", hasShortLengthData },
            { "tooLight", hasLightWeightData },
        });
    }

    private static readonly Object ExpressCheckinLockObject = new Object();

    /// <summary>
    /// 重新入库时快递单号修改信息时，快递单号是不能被修改的，否则许多逻辑也都要改
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    [Route("/admin/express/checkin/save")]
    // 内部有事务
    public IActionResult CheckinSave(LogisticsExpressCheckinExpressSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int warehouseId = model.WarehouseId;
        var warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException("指定的仓库不存在！");
        }

        if (model.GoodsList == null || model.GoodsList.Count == 0) {
            throw new ValidationException("物品列表不能为空！");
        }

        if (model.ShelfId == null || model.ShelfId == 0) {
            throw new ValidationException("请指定快递单入库货架！");
        }

        IList<string> errorMessageList = new List<string>();

        IList<LogisticsExpressCheckinExpressGoodsModel> goodsModelList = model.GoodsList;
        int goodsListCount = goodsModelList.Count;
        for (int index = 0; index < goodsListCount; index++) {
            LogisticsExpressCheckinExpressGoodsModel goodsModel = goodsModelList[index];

            if (goodsModel.GoodsId <= 0) {
                errorMessageList.Add($"第 {index + 1} 个物品，扩展名不能为空！");
            } else {
                var logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsModel.GoodsId);
                if (logisticsGoods == null) {
                    errorMessageList.Add($"第 {index + 1} 个物品，指定的扩展名不存在！");
                }
            }

            if (goodsModel.Quantity <= 0) {
                // errorMessageList.Add($"第 {index + 1} 个物品，数量不能为空！");
                errorMessageList.Add($"第 {index + 1} 个物品，数量应该大于0！");
            }

            if (string.IsNullOrWhiteSpace(goodsModel.Unit)) {
                errorMessageList.Add($"第 {index + 1} 个物品，单位不能为空！");
            }
        }

        if (errorMessageList.Count != 0) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(errorMessageList));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        // 快递单号大写与去掉前后于中间的空格，订单创建时同样也要做这样的事情
        string expressTrackingNo = model.TrackingNo.Trim().Replace(" ", "").ToUpper(); // 去空格再大写

        // 快递单号验证
        if (expressTrackingNoService.IsValidateTrackingNo(expressTrackingNo) == false) {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        int billCustomerId = customerCodeService.GetCustomerIdFromCustomerCode(model.BillCustomerCode);

        //  这个检查先不用了
        /*
        if (billCustomerId == 0)
        {
            throw new ValidationException("不能从面单客户ID中解析确切的客户ID，请检查！");
        }
        */

        /*
         这个现在不用了，如果客户不存在，前面customerCodeService.GetCustomerIdFromCustomerCode()返回的就是0
        if (billCustomerId != 0) {
            CustomerInfo billCustomer = session.Get<CustomerInfo>(billCustomerId);
            if (billCustomer == null) {
                throw new ValidationException("从面单客户ID中解析的客户ID不存在，请检查！");
            }
        }
        */

        // 获取并检查尺寸重量信息是否已经存在
        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();
        if (expressSizeWeightList == null || expressSizeWeightList.Count == 0) {
            throw new ValidationException("快递单尺寸重量信息未录入保存！");
        }

        // 检查快递单号图片，如果不符合会抛出异常
        expressCheckinService.CheckPhotoWhenCheckin(expressTrackingNo);

        int shelfId = model.ShelfId ?? 0;
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            throw new ValidationException("指定的货架不存在！");
        }

        int storageLocSpecId = model.StorageLocSpecId ?? 0;
        StorageLocSpecInfo storageLocSpec = session.Get<StorageLocSpecInfo>(storageLocSpecId);
        if (storageLocSpec == null) {
            throw new ValidationException("指定的仓位规格不存在！");
        }

        if (shelf.Type != ShelfType.Normal) {
            throw new ValidationException($"指定的货架 {shelf.Name} 不是正常区快递单货架！");
        }

        ExpressExpectInfo? expressExpect = null;

        int expressIdFromUi = model.ExpressId;
        bool isCreate = expressIdFromUi == 0;

        ExpressInfo? express = null;

        bool aoPinImportExpressStorageLocChanged = false;

        lock (ExpressCheckinLockObject) // lock 时，数据库事务应该放在 lock 区域之内
        {
            Transactional.Wrap(session, () => {
                if (isCreate == false) {
                    express = session.Get<ExpressInfo>(expressIdFromUi);
                    if (express == null) {
                        throw new ValidationException($"指定的快递单号#{expressIdFromUi}不存在！");
                    }

                    if (express.TrackingNo != model.TrackingNo) {
                        ExpressInfo? expressOther = session.Query<ExpressInfo>()
                            .Where(e => e.TrackingNo == expressTrackingNo)
                            .Take(1)
                            .FirstOrDefault();
                        if (expressOther != null) {
                            throw new ValidationException("该快递单号存在其他入库记录！"); // 一般发生在入库保存后，马上修改快递单号的情形
                        }
                    }
                } else {
                    express = session.Query<ExpressInfo>()
                        .Where(e => e.TrackingNo == expressTrackingNo)
                        .Take(1)
                        .FirstOrDefault();

                    if (express != null && express.Id != expressIdFromUi) {
                        throw new ValidationException("该快递单号存在其他入库记录！");
                    }
                }

                /*
                if (express != null && (express.Picked == true || ExpressStatus.IsCheckedIn(express.Status)))
                {
                    throw new ValidationException("该快递单号已被客户认领，不能修改更新记录信息！");
                }
                */

                if (express != null && ExpressStatus.IsOrderCreate(express.Status)) {
                    throw new ValidationException("该快递单号已提交订单，不能修改更新记录信息！");
                }

                if (express != null && (express.Status == ExpressStatus.WaitReturn)) {
                    throw new ValidationException("该快递单号当前申请退运中，不能修改更新记录信息！");
                }

                /*
                if (express != null && (express.WarehouseProcessed == true || express.Status == ExpressStatus.Order))
                {
                    throw new ValidationException("该快递单号已存在入库记录，并且已经下架或打包，不能修改更新记录信息！");
                }
                */

                // 检查有无物品照片
                /* todo 这个检查暂时不需要，以后再打开这个功能
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsItem in goodsModelList) {
                    if (logisticsGoodsImageService.HasGoodsImage(goodsItem.GoodsId) == false) {
                        LogisticsGoodsInfo logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsItem.GoodsId);

                        throw new ValidationException($"物品 #Id:{goodsItem.GoodsId} #{logisticsGoods?.Name} 还没有任何关联照片，请设置物品创建单号，有单号相关订单照片后，再使用该物品名称！");
                    }
                }
                */

                // 已丢弃的快递，应该可以在这里更新入库信息的

                // 如果已经在 快递单号预报 里面的，就设置 快递单号预报为 录货值 状态
                expressExpect = session.Query<ExpressExpectInfo>()
                    .Where(e => e.TrackingNo == expressTrackingNo)
                    .Take(1)
                    .SingleOrDefault();

                if (expressExpect != null && expressExpect.Status == ExpressExpectStatus.Create) {
                    expressExpect.Status = ExpressExpectStatus.Checkin;
                    session.Save(expressExpect);
                    session.Flush();
                }

                DateTime now = DateTime.Now;

                if (express == null) {
                    express = new ExpressInfo {
                        TrackingNo = expressTrackingNo,
                        CheckinTrackingNo = string.Empty, // 后面会回填这个澳拼跟踪号
                        Status = ExpressStatus.WaitPick, // 初始化为 WaitPick     
                        ShelfStatus = ExpressShelfStatus.ToOnShelf, // 初始化为 待上架
                        AddTime = now,
                    };
                }

                if (expressExpect != null) {
                    express.GoodsValue = expressExpect.GoodsValue;
                    express.ClaimTime = now;
                    express.Status = ExpressStatus.Checkin;
                    express.Customer = expressExpect.Customer;
                } else if (billCustomerId > 0) {
                    // 这里是能解析得到 billCustomerId 就自动认领了
                    express.ClaimTime = now;
                    express.Status = ExpressStatus.Checkin;
                    express.Customer = session.Load<CustomerInfo>(billCustomerId); // customerCodeService.GetCustomerIdFromCustomerCode() 中已经检查客户的存在了，这里用 session.Load()也可以的
                } else if (express.Status == ExpressStatus.Checkin) {
                    express.ClaimTime = null;
                    express.Status = ExpressStatus.WaitPick;
                    express.Customer = null;
                }

                IList<string> editDescList = new List<string>();

                // 仓库
                if (isCreate == false && express.Warehouse.Id != warehouseId) {
                    editDescList.Add($"仓库:{express.Warehouse.Name}=>{warehouse.Name}");
                }

                express.Warehouse = warehouse;

                // 快递公司
                if (isCreate == false && express.ExpressCompany != model.ExpressCompany) {
                    editDescList.Add($"快递公司:{express.ExpressCompany}=>{model.ExpressCompany}");
                }

                express.ExpressCompany = model.ExpressCompany;

                // 物品类型
                LogisticsGoodsType goodsTypeBefore = express.GoodsType;

                IList<LogisticsGoodsType> goodsTypeList = new List<LogisticsGoodsType>();
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsItem in goodsModelList) {
                    goodsTypeList.Add(goodsItem.GoodsType);
                }

                LogisticsGoodsType goodsType = logisticsGoodsTypeService.GetGoodsTypeFromGoodsTypeList(goodsTypeList);

                express.GoodsType = goodsType;

                if (isCreate == false && goodsTypeBefore != goodsType) {
                    editDescList.Add($"物品类型:{express.GoodsType.Name}=>{goodsType.Name}");
                }

                // 添加用户
                if (isCreate == false && express.AddUser.Id != loginUserId) {
                    UserInfo oldAddUser = express.AddUser;
                    editDescList.Add($"添加用户:{oldAddUser.UserName}=>{loginUser.UserName}");
                }

                express.AddUser = loginUser;

                // 包裹数量
                int prePackageCount = express.PackageCount; // 如果是新建，不是修改保存的话，这个值就是0
                int packageCount = expressSizeWeightList.Sum(e => e.PackageCount);

                if (packageCount <= 0) {
                    packageCount = 1;
                }

                if (isCreate == false && prePackageCount != packageCount) {
                    editDescList.Add($"包裹数量:{prePackageCount}=>{packageCount}");
                }

                express.PackageCount = packageCount;

                // 入库备注 日志 放在新的日志保存到数据库之前，后面 express 保存完后，保存到 express remark 表

                string warehouseRemark = model.WarehouseRemark?.Trim() ?? string.Empty;
                ExpressRemarkInfo? warehouseRemarkInDb = null;
                string oldWarehouseRemark = string.Empty;
                if (isCreate == false) {
                    warehouseRemarkInDb = session.Query<ExpressRemarkInfo>()
                        .Where(e => e.Express!.Id == expressIdFromUi && e.Type == ExpressRemarkType.Warehouse)
                        .Take(1)
                        .SingleOrDefault();
                    oldWarehouseRemark = warehouseRemarkInDb?.Content ?? string.Empty;
                    if (warehouseRemark != oldWarehouseRemark) {
                        editDescList.Add($"入库备注:{oldWarehouseRemark}=>{warehouseRemark}");
                    }
                }

                // 面单客户

                // 这个日志记录要放在 express.BillCustomerCode = model.BillCustomerCode; 之前
                if (isCreate == false && express.BillCustomerCode != model.BillCustomerCode) {
                    editDescList.Add($"面单客户:{express.BillCustomerCode}=>{model.BillCustomerCode}");
                }

                express.BillCustomerCode = model.BillCustomerCode;
                express.BillCustomerId = billCustomerId;

                bool isStorageLocChanged = isCreate == false && (express.StorageLoc.Shelf.Id != model.ShelfId || express.StorageLoc.Spec.Id != model.StorageLocSpecId);

                // 如果是新入库的，或者是 原来的但修改了货架, 或者是修改了仓位规格
                if (isCreate == true || isStorageLocChanged) {
                    StorageLocInfo recommandStorage = expressCheckinService.GetRecommandStorage(shelf, storageLocSpec); // 从这里取到的仓位ID，已经保证是仓位上物品不超过限制了
                    // 检查仓位和货架是否可用
                    storageLocService.CheckStorageLoc((recommandStorage.Id), ShelfType.Normal);

                    if (isStorageLocChanged) {
                        editDescList.Add($"仓位：{express.StorageLoc.GetFullName()} => {recommandStorage.GetFullName()}");
                    }

                    // 457 是澳拼导入的临时仓位
                    if (isStorageLocChanged && express.StorageLoc.Id == 457) {
                        int aoPinImportExpressIdBegin = 1496;
                        int aoPinImportExpressIdEnd = 2787;
                        if (express.Id >= aoPinImportExpressIdBegin && express.Id <= aoPinImportExpressIdEnd) {
                            aoPinImportExpressStorageLocChanged = true;
                        }
                    }

                    express.StorageLoc = recommandStorage;
                    express.Shelf = recommandStorage.Shelf;
                }

                session.SaveOrUpdate(express);

                session.Flush();

                // 放在上面的 session.Flush() 后面
                storageLocExpressService.UpdateSingle(express.StorageLoc.Id);

                // 入库跟踪号，这时生成，生成的号不会浪费

                if (isCreate) {
                    express.CheckinTrackingNo = checkinTrackingNoService.GetNextMainTrackinigNo();

                    session.Update(express);
                    session.Flush();
                }

                // 保存客户备注
                if (isCreate) // 新增是才需要这个客户备注迁移
                {
                    if (expressExpect != null
                        && string.IsNullOrWhiteSpace(expressExpect.Description) == false) // 预报里面的描述非空
                    {
                        session.Save(new ExpressRemarkInfo() {
                            Express = express,
                            Type = ExpressRemarkType.Customer,
                            // AddUser = loginUser,
                            AddUser = null,
                            Content = expressExpect.Description.Trim(),
                        });
                        session.Flush();
                    }
                }

                // 保存入库备注
                if (string.IsNullOrWhiteSpace(warehouseRemark) == false) {
                    // 入库备注非空，就新增或更新入库备注
                    if (warehouseRemarkInDb == null) {
                        warehouseRemarkInDb = new ExpressRemarkInfo() {
                            Express = express,
                            Type = ExpressRemarkType.Warehouse,
                            AddUser = loginUser,
                        };
                    }

                    warehouseRemarkInDb.Content = warehouseRemark;

                    if (isCreate == false && warehouseRemark != oldWarehouseRemark) {
                        // 编辑时如果入库备注跟原来的不同，就更新添加用户
                        warehouseRemarkInDb.AddUser = loginUser;
                    }

                    session.SaveOrUpdate(warehouseRemarkInDb);
                    session.Flush();
                } else if (warehouseRemarkInDb != null) {
                    // 如果入库备注为空，并且原来的得数据存在，就删除原来的数据，不要留一个空内容的备注记录
                    session.Delete(warehouseRemarkInDb);
                    session.Flush();
                }


                // 删除被移除的物品
                IList<ExpressGoodsInfo> expressGoodsListInDb = new List<ExpressGoodsInfo>();
                if (isCreate == false) {
                    expressGoodsListInDb = session.Query<ExpressGoodsInfo>()
                        .Fetch(e => e.Goods)
                        .Where(e => e.Express!.Id == expressIdFromUi)
                        .ToList();

                    // 移除界面上没有，但数据库中有的物品
                    IList<int> entryIdListFromUi = goodsModelList.Select(e => e.ExpressGoodsId).ToList();
                    IList<ExpressGoodsInfo> expressGoodsToDelete = expressGoodsListInDb
                        .Where(e => entryIdListFromUi.Contains(e.Id) == false)
                        .ToList();

                    foreach (ExpressGoodsInfo ce in expressGoodsToDelete) {
                        if (isCreate == false) {
                            string goodsNameToRemove = ce.Goods.Name;
                            editDescList.Add($"移除物品:{goodsNameToRemove}*{ce.Quantity}{ce.Unit}");
                        }

                        session.Delete(ce);
                    }

                    session.Flush();
                }

                // 保存物品
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsModel in goodsModelList) {
                    ExpressGoodsInfo? expressGoods = null;
                    if (goodsModel.ExpressGoodsId != 0) {
                        expressGoods = expressGoodsListInDb.Where(e => e.Id == goodsModel.ExpressGoodsId).Take(1).SingleOrDefault();
                    }

                    if (expressGoods == null) {
                        expressGoods = new ExpressGoodsInfo();
                    }

                    expressGoods.Express = express;

                    if (isCreate == false &&
                        (expressGoods.Goods.Id != goodsModel.GoodsId
                         || expressGoods.Quantity != goodsModel.Quantity
                         || expressGoods.Unit != goodsModel.Unit)) {
                        LogisticsGoodsInfo goodsEditAfter = session.Get<LogisticsGoodsInfo>(goodsModel.GoodsId);
                        if (expressGoods.Id != 0) // 这个时候是修改(isCreate == false)，编辑物品
                        {
                            LogisticsGoodsInfo goodsEditBefore = expressGoods.Goods;

                            editDescList.Add($"修改物品: {goodsEditBefore?.Name}{expressGoods.Quantity}{expressGoods.Unit}=>{goodsEditAfter?.Name}{goodsModel.Quantity}{goodsModel.Unit}");
                        } else // 这个时候是修改(isCreate == false)，添加物品
                        {
                            editDescList.Add($"添加物品: {goodsEditAfter?.Name}{goodsModel.Quantity}{goodsModel.Unit}");
                        }
                    }

                    expressGoods.Goods = session.Get<LogisticsGoodsInfo>(goodsModel.GoodsId); // 前面验证是否存在时，做了数据加载，这里一般就是从缓存中取得了
                    expressGoods.GoodsType = goodsModel.GoodsType;
                    expressGoods.Quantity = goodsModel.Quantity;
                    expressGoods.Unit = goodsModel.Unit ?? string.Empty;

                    session.SaveOrUpdate(expressGoods);
                }

                session.Flush();

                StorageLocInfo storageLoc = express.StorageLoc;
                if (isCreate) {
                    string logDesc = storageLoc.GetFullName();
                    if (expressExpect != null) {
                        string expectCustomerCode = customerCodeService.GetCustomerCode(expressExpect.Customer.Id);
                        logDesc += $"，面单客户按预报客户：{expectCustomerCode}, 预报时间 {expressExpect.CreateTime:yyyy-MM-dd HH:mm}";
                    }

                    expressLogService.UserLog(express.Id, ExpressLogType.Checkin, logDesc, loginUserId);
                } else {
                    string logDesc = storageLoc.GetFullName() + " " + string.Join(";", editDescList);
                    if (expressExpect != null) {
                        string expectCustomerCode = customerCodeService.GetCustomerCode(expressExpect.Customer.Id);
                        logDesc += $"，面单客户按预报客户：{expectCustomerCode}, 预报时间 {expressExpect.CreateTime:yyyy-MM-dd HH:mm}";
                    }

                    expressLogService.UserLog(express.Id, ExpressLogType.CheckinEdit, logDesc, loginUserId);
                }

                // 后面 已经调用了，这里就不用了 expressSizeWeightService.BindAndSum(express.Id);

                expressCheckinService.SaveUserShelfSelect(loginUserId, model.ShelfId ?? 0, model.StorageLocSpecId ?? 0);
            }); // 事务结束
        }

        if (isCreate == true) // 只在首次入库的时候提醒，修改信息的时候不提醒
        {
            Transactional.Wrap(session, () => {
                // 发送消息提醒，虽然不在同一个事务内，但也是在前面都成功的情况下才发送的
                if (expressExpect != null) {
                    // 优先按预报的发送消息
                    customerMessageExpressExpectEnterService.SendMessage(expressExpect.Customer, express!);
                } else if (billCustomerId != 0) {
                    // 没预报但有面单客户ID
                    CustomerInfo billCustomer = session.Get<CustomerInfo>(billCustomerId);
                    if (billCustomer != null) {
                        customerMessageExpressCheckinService.SendMessage(billCustomer, express!);
                    }
                }
            });
        }

        // 澳拼导入快递仓位有修改的发送通知消息
        if (isCreate == false && aoPinImportExpressStorageLocChanged == true) {
            Transactional.Wrap(session, () => {
                if (express != null && express.Customer != null) {
                    customerMessageExpressCheckinService.SendMessage(express.Customer, express!);
                }
            });
        }

        Transactional.Wrap(session, () => {
            int expressId = express?.Id ?? 0;
            expressSizeWeightService.BindExpressByTrackingNo(expressId);
            expressPhotoService.BindExpressByTrackingNo(expressId);
            expressVideoService.BindExpressByTrackingNo(expressId);
        });

        // 消息发送, 放在上面的图片快递绑定后面
        // 没用发送过的才需要发送，从发送时间表里面查询
        // 两个类型的消息，签收的和入库的
        // expressPhotoMessageService

        Transactional.Wrap(session, () => {
            int expressId = express?.Id ?? 0;

            var expressPhotoSign = session.Query<ExpressPhotoInfo>()
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Sign)
                .Take(1)
                .SingleOrDefault();
            if (expressPhotoSign != null) {
                // 这个是看有没发送过，不是看一个小时内有没发送
                var expressSendTimeSign = session.Query<ExpressPhotoMessageSendTimeInfo>()
                    .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Sign)
                    .Take(1)
                    .SingleOrDefault();

                if (expressSendTimeSign == null) {
                    expressPhotoMessageService.SendAddPhotoMessage(expressPhotoSign);
                }
            }

            var expressPhotoCheckin = session.Query<ExpressPhotoInfo>()
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Checkin)
                .Take(1)
                .SingleOrDefault();
            if (expressPhotoCheckin == null) {
                var expressSendTimeCheckin = session.Query<ExpressPhotoMessageSendTimeInfo>()
                    .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Checkin)
                    .Take(1)
                    .SingleOrDefault();

                if (expressSendTimeCheckin == null) {
                    expressPhotoMessageService.SendAddPhotoMessage(expressPhotoCheckin);
                }
            }
        });

        return Json(new JsonResultData {
            Success = true,
            Message = "快递单号保存入库成功！",
            Data = new {
                claim = billCustomerId == 0,
                billCustomerCode = model.BillCustomerCode,
            }
        });
    }

    #region 搜索物品列表

    [HttpPost]
    [Route("/admin/express/checkin/search/goods/list/data")]
    public IActionResult SearchGoodsListData(int warehouseId, string param) {
        IList<LogisticsGoodsInfo> goodsList = new List<LogisticsGoodsInfo>();
        int totalCount = 0;

        if (string.IsNullOrWhiteSpace(param) == false) {
            goodsList = GetSearchGoodsCriteria(warehouseId, param).List<LogisticsGoodsInfo>();
            totalCount = GetSearchGoodsCriteria(warehouseId, param).SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        IList<IDictionary<string, object>> dataList = new List<IDictionary<string, object>>();

        foreach (LogisticsGoodsInfo goods in goodsList) {
            string goodsAdminName = goods.Name;

            dataList.Add(new Dictionary<string, object> {
                { "id", goods.Id },
                { "text", goodsAdminName },
                { "type", goods.Type }
                // 可以放入物品其他属性
            });
        }

        return Json(new Dictionary<string, object> {
            { "items", dataList },
            { "totalCount", totalCount }
        });
    }

    [NonAction]
    private ICriteria GetSearchGoodsCriteria(int warehouseId, string param) {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);

        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();
        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

        criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status != LogisticsGoodsStatus.Disabled));

        // criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.Confirmed)); 这个物品搜索时，可以搜还没确认的物品，海关报关会用确认过的物品
        criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(param, MatchMode.Anywhere));

        return criteria;
    }

    #endregion

    [HttpPost]
    [Route("/admin/express/checkin/get/express/company/name")]
    public IActionResult GetExpressComName(string expressTrackingNo) {
        string comName = expressCompanyService.GetExpressCompanyName(expressTrackingNo);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "content", comName }
        });
    }

    [HttpPost]
    [Route("/admin/express/checkin/express/get/print/data/by/id")]
    [NoLoginRequired]
    public IActionResult GetPrintDataById(int expressId) {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在");
        }

        string customerCode = express.BillCustomerCode;
        string orderCode = string.Empty;

        int storageLocId = express.StorageLoc.Id;
        StorageLocInfo? storageLoc = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Fetch(e => e.Spec)
            .Where(e => e.Id == storageLocId)
            .Take(1)
            .SingleOrDefault();
        string storageLocName = storageLoc?.GetFullName() ?? string.Empty;
        string checkinPackageCount = express.PackageCount.ToString();
        string userName = express.AddUser.UserName;
        string checkinTime = express.AddTime.ToString("MM-dd HH:mm");
        string goodsTypeShortName = express.GoodsType.CheckinLabelShortName;

        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express!.Id == expressId) // 一个快递不会有很多备注，全部取出来算了
            .ToList();

        ExpressRemarkInfo? expressRemarkWarehouse = expressRemarkList
            .Where(e => e.Type == ExpressRemarkType.Warehouse)
            .Take(1)
            .SingleOrDefault();

        ExpressRemarkInfo? expressRemarkCustomer = expressRemarkList
            .Where(e => e.Type == ExpressRemarkType.Customer)
            .Take(1)
            .SingleOrDefault();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取数据成功！" },
            // { "expressTrackingNo", express.TrackingNo + CheckinLabelConfig.ExpressTrackingNoShundaSuffix }, // 加上后缀让打包时不能扫描
            { "expressTrackingNo", express.TrackingNo }, // 不需要后缀
            { "checkinTrackingNo", express.CheckinTrackingNo },
            { "customerCode", customerCode },
            { "warehouseNote", expressRemarkWarehouse?.Content ?? string.Empty },
            { "customerNote", expressRemarkCustomer?.Content ?? string.Empty },
            { "checkinPackageCount", checkinPackageCount },
            { "orderCode", orderCode },
            { "storageLoc", storageLocName },
            { "userName", userName },
            { "checkinTime", checkinTime },
            { "goodsTypeName", goodsTypeShortName },
        });
    }

    [HttpPost]
    [Route("/admin/express/checkin/express/get/print/data")]
    [NoLoginRequired]
    public IActionResult GetPrintData(string expressTrackingNo) {
        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null) {
            throw new ValidationException($"指定的快递单号{expressTrackingNo}不存在");
        }

        return GetPrintDataById(express.Id);
    }

    [HttpPost]
    [Route("/admin/express/checkin/express/get/print/data/manual")]
    [NoLoginRequired]
    [Transactional]
    public IActionResult GetPrintDataManual(string expressTrackingNo) {
        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null) {
            throw new ValidationException($"指定的快递单号{expressTrackingNo}不存在");
        }


        var printData = GetPrintDataById(express.Id);

        int loginUserId = loginUserService.GetLoginUserId();
        expressLogService.UserLog(express.Id, ExpressLogType.ManualPrintLabel, loginUserId);

        return printData;
    }

    #region size weight

    [HttpPost]
    [Route("/admin/express/checkin/save/express/size/weight")]
    [Transactional]
    public IActionResult SaveSizeWeight(LogisticsExpressCheckinSizeWeightModel model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (model.Length != null && model.Length.Value <= 0) {
            throw new ValidationException("长度应该大于0！");
        }

        if (model.Width != null && model.Width.Value <= 0) {
            throw new ValidationException("宽度应该大于0！");
        }

        if (model.Height != null && model.Height.Value <= 0) {
            throw new ValidationException("高度应该大于0！");
        }

        if (model.PackageCount != null && model.PackageCount.Value <= 0) {
            throw new ValidationException("数量应该大于0！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        string trackingNo = model.ExpressTrackingNo;

        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        // 检查快递单号状态
        if (express != null && ExpressStatus.IsPacked(express.Status)) {
            throw new ValidationException("已打包快递单号不能，保存尺寸重量信息");
        }

        // 先全部取出来
        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        ExpressSizeWeightInfo? expressSizeWeightMain = expressSizeWeightList
            .Where(e => string.IsNullOrWhiteSpace(e.TrackingNoSub))
            .Take(1)
            .SingleOrDefault();

        bool logisticsExpressCreate = false;
        if (expressSizeWeightMain == null) {
            logisticsExpressCreate = true;

            expressSizeWeightMain = new ExpressSizeWeightInfo();
            expressSizeWeightMain.Express = express; // 有可能为 null
            expressSizeWeightMain.TrackingNo = trackingNo;
            expressSizeWeightMain.TrackingNoSub = string.Empty;
        }

        StringBuilder expressEditLogDescBuiler = new StringBuilder();

        // 长
        if (logisticsExpressCreate == false && expressSizeWeightMain.Length != model.Length) {
            expressEditLogDescBuiler.Append($"长:{expressSizeWeightMain.Length}=>{model.Length}");
        }

        expressSizeWeightMain.Length = model.Length ?? 0;

        // 宽
        if (logisticsExpressCreate == false && expressSizeWeightMain.Width != model.Width) {
            expressEditLogDescBuiler.Append($"宽:{expressSizeWeightMain.Width}=>{model.Width}");
        }

        expressSizeWeightMain.Width = model.Width ?? 0;

        // 高
        if (logisticsExpressCreate == false && expressSizeWeightMain.Height != model.Height) {
            expressEditLogDescBuiler.Append($"高:{expressSizeWeightMain.Height}=>{model.Height}");
        }

        expressSizeWeightMain.Height = model.Height ?? 0;

        // 重量
        if (logisticsExpressCreate == false && expressSizeWeightMain.Weight != model.Weight) {
            expressEditLogDescBuiler.Append($"重量:{expressSizeWeightMain.Weight}=>{model.Weight}");
        }

        expressSizeWeightMain.Weight = model.Weight ?? 0;

        // 箱数
        if (logisticsExpressCreate == false && expressSizeWeightMain.PackageCount != model.PackageCount) {
            expressEditLogDescBuiler.Append($"箱数:{expressSizeWeightMain.PackageCount}=>{model.PackageCount}");
        }

        expressSizeWeightMain.PackageCount = model.PackageCount ?? 1;

        session.SaveOrUpdate(expressSizeWeightMain);
        session.Flush();


        if (logisticsExpressCreate == true) {
            string addLogDesc = $"长:{expressSizeWeightMain.Length}, 宽:{expressSizeWeightMain.Width}, 高:{expressSizeWeightMain.Height}, 重量:{expressSizeWeightMain.Weight}, 箱数:{expressSizeWeightMain.PackageCount}";
            expressLogService.UserLog(trackingNo, ExpressLogType.SizeWeightAdd, addLogDesc, loginUserId);
        } else {
            expressLogService.UserLog(trackingNo, ExpressLogType.SizeWeightEdit, expressEditLogDescBuiler.ToString(), loginUserId);
        }

        // 保存子单号尺寸
        {
            IList<LogisticsExpressCheckinSizeWeightChildItem> childSizeWeightList = model.ChildSizeWeightList;
            if (childSizeWeightList == null) {
                childSizeWeightList = new List<LogisticsExpressCheckinSizeWeightChildItem>();
            }

            // 检查尺寸合法性
            foreach (var childSizeWeight in childSizeWeightList) {
                if (childSizeWeight.Length != null && childSizeWeight.Length.Value <= 0) {
                    throw new ValidationException($"长度值{childSizeWeight.Length.Value}应该大于0！");
                }

                if (childSizeWeight.Width != null && childSizeWeight.Width.Value <= 0) {
                    throw new ValidationException($"宽度值{childSizeWeight.Width}.Value}}应该大于0！");
                }

                if (childSizeWeight.Height != null && childSizeWeight.Height.Value <= 0) {
                    throw new ValidationException($"高度值{childSizeWeight.Height.Value}应该大于0！");
                }

                if (childSizeWeight.PackageCount != null && childSizeWeight.PackageCount.Value <= 0) {
                    throw new ValidationException($"数量值{childSizeWeight.PackageCount.Value}应该大于0！");
                }
            }

            IList<int> childSizeWeightIdList = childSizeWeightList.Select(e => e.ExpressChildId).ToList();

            IList<ExpressSizeWeightInfo> childSizeWeightListInDb = expressSizeWeightList
                .Where(e => string.IsNullOrWhiteSpace(e.TrackingNoSub) == false)
                .ToList();

            // 子单号尺寸，数据库里面有界面上没有的删除
            foreach (var chidSizeWeightInDb in childSizeWeightListInDb) {
                if (childSizeWeightIdList.Contains(chidSizeWeightInDb.Id) == false) {
                    // 再删除子单号尺寸
                    session.Delete(chidSizeWeightInDb);

                    string deleteLogDesc = $"长:{chidSizeWeightInDb.Length}, 宽:{chidSizeWeightInDb.Width}, 高:{chidSizeWeightInDb.Height}, 重量:{chidSizeWeightInDb.Weight}, 箱数:{chidSizeWeightInDb.PackageCount}";
                    expressLogService.UserLog(trackingNo, ExpressLogType.SizeWeightDelete, deleteLogDesc, loginUserId);
                }
            }

            session.Flush();

            // 子单号尺寸，界面上有的，新增或更新
            foreach (LogisticsExpressCheckinSizeWeightChildItem childItem in childSizeWeightList) {
                if (childItem.ExpressChildId == 0) {
                    ExpressSizeWeightInfo expressChild = new ExpressSizeWeightInfo();
                    expressChild.TrackingNo = trackingNo;

                    string subTrackingNo = $"{trackingNo}-{childItem.TrackingNo}";
                    expressChild.Express = express; // 有可能为 null
                    expressChild.TrackingNo = subTrackingNo;
                    expressChild.Length = childItem.Length ?? 0;
                    expressChild.Width = childItem.Width ?? 0;
                    expressChild.Height = childItem.Height ?? 0;
                    expressChild.Weight = childItem.Weight ?? 0;
                    expressChild.PackageCount = childItem.PackageCount ?? 1;

                    session.Save(expressChild);

                    string addLogDesc = $"子单号:{subTrackingNo}, 长:{expressChild.Length}, 宽:{expressChild.Width}, 高:{expressChild.Height}, 重量:{expressChild.Weight}, 箱数:{expressChild.PackageCount}";
                    expressLogService.UserLog(trackingNo, ExpressLogType.SizeWeightAdd, addLogDesc, loginUserId);
                } else {
                    ExpressSizeWeightInfo? expressChild = expressSizeWeightList.Where(e => e.Id == childItem.ExpressChildId).Take(1).SingleOrDefault();
                    if (expressChild == null) {
                        throw new ValidationException($"指定的尺寸重量信息#{childItem.ExpressChildId}不存在！");
                    }

                    expressChild.TrackingNo = trackingNo;

                    StringBuilder childEditLogDescBuiler = new StringBuilder();

                    string subTrackingNo = $"{trackingNo}-{childItem.TrackingNo}";
                    if (expressChild.TrackingNoSub != subTrackingNo) {
                        expressChild.TrackingNoSub = subTrackingNo;

                        childEditLogDescBuiler.Append($"子单号:{expressChild.TrackingNo}=>{subTrackingNo}");
                    }

                    if (expressChild.Length != childItem.Length) {
                        expressChild.Length = childItem.Length ?? 0;

                        childEditLogDescBuiler.Append($"长:{expressChild.Length}=>{childItem.Length}");
                    }

                    if (expressChild.Width != childItem.Width) {
                        expressChild.Width = childItem.Width ?? 0;

                        childEditLogDescBuiler.Append($"宽:{expressChild.Width}=>{childItem.Width}");
                    }

                    if (expressChild.Height != childItem.Height) {
                        expressChild.Height = childItem.Height ?? 0;

                        childEditLogDescBuiler.Append($"高:{expressChild.Height}=>{childItem.Height}");
                    }

                    if (expressChild.Weight != childItem.Weight) {
                        expressChild.Weight = childItem.Weight ?? 0;

                        childEditLogDescBuiler.Append($"重量:{expressChild.Weight}=>{childItem.Weight}");
                    }

                    if (expressChild.PackageCount != childItem.PackageCount) {
                        expressChild.PackageCount = childItem.PackageCount ?? 1;

                        childEditLogDescBuiler.Append($"箱数:{expressChild.PackageCount}=>{childItem.PackageCount}");
                    }

                    expressLogService.UserLog(trackingNo, ExpressLogType.SizeWeightEdit, childEditLogDescBuiler.ToString(), loginUserId);

                    session.Update(expressChild);
                }
            }

            // 如果是已入库的快递单，就要更新包裹数量尺寸重量等
            if (express != null) {
                // expressSizeWeightService.SumDataAndUpdateExpress(express.Id);
                int expressId = express.Id;
                expressSizeWeightService.BindExpressByTrackingNo(expressId);
            }

            session.Flush();
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "尺寸重量数据保存成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/express/checkin/new/express/size/weight/row")]
    public IActionResult NewExpressSizeWeightRow(string expressTrackingNo) {
        LogisticsExpressCheckinSizeWeightChildItem model = new LogisticsExpressCheckinSizeWeightChildItem();
        model.ParentTrackingNo = expressTrackingNo;
        model.Length = null;
        model.Width = null;
        model.Height = null;
        model.Weight = null;
        model.PackageCount = 1;
        return View("ExpressCheckinSizeWeightRow", model);
    }

    #endregion
}