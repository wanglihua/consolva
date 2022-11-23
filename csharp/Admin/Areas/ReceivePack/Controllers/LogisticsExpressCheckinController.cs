using System.Text;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NLog;
using shunda.Admin.Services;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Customer;
using shunda.Service.Express;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

/// <summary>
/// 快递入库
/// </summary>
[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.ExpressCheckin)]
public class LogisticsExpressCheckinController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private ExpressCompanyService expressCompanyService;
    private ExpressTrackingNoCorrectService expressTrackingNoCorrectService;
    private ExpressTrackingNoService expressTrackingNoService;
    private ExpressCheckinService expressCheckinService;
    private LoginUserService loginUserService;
    private CustomerService customerService;
    private LogisticsGoodsTypeService logisticsGoodsTypeService;
    private AoPinTrackingNoService aoPinTrackingNoService;
    private StorageLocService storageLocService;
    private ExpressCheckinConfigService expressCheckinConfigService;
    private CustomerMessageService customerMessageService;
    private ExpressLogService expressLogService;
    private LogisticsExpressService logisticsExpressService;
    private LogisitcsClaimGoodsVolumeWeightService logisitcsClaimGoodsVolumeWeightService;
    private LogisticsGoodsImageService logisticsGoodsImageService;
    private SizeWeightHoldExpressService sizeWeightHoldExpressService;

    public LogisticsExpressCheckinController(
        ISession session,
        ExpressCompanyService expressCompanyService,
        ExpressTrackingNoCorrectService expressTrackingNoCorrectService,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressCheckinService expressCheckinService,
        LoginUserService loginUserService,
        CustomerService customerService,
        LogisticsGoodsTypeService logisticsGoodsTypeService,
        AoPinTrackingNoService aoPinTrackingNoService,
        StorageLocService storageLocService,
        ExpressCheckinConfigService expressCheckinConfigService,
        CustomerMessageService customerMessageService,
        ExpressLogService expressLogService,
        LogisticsExpressService logisticsExpressService,
        LogisitcsClaimGoodsVolumeWeightService logisitcsClaimGoodsVolumeWeightService,
        LogisticsGoodsImageService logisticsGoodsImageService,
        SizeWeightHoldExpressService sizeWeightHoldExpressService)
    {
        this.session = session;
        this.expressCompanyService = expressCompanyService;
        this.expressTrackingNoCorrectService = expressTrackingNoCorrectService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressCheckinService = expressCheckinService;
        this.loginUserService = loginUserService;
        this.customerService = customerService;
        this.logisticsGoodsTypeService = logisticsGoodsTypeService;
        this.aoPinTrackingNoService = aoPinTrackingNoService;
        this.storageLocService = storageLocService;
        this.expressCheckinConfigService = expressCheckinConfigService;
        this.customerMessageService = customerMessageService;
        this.expressLogService = expressLogService;
        this.logisticsExpressService = logisticsExpressService;
        this.logisitcsClaimGoodsVolumeWeightService = logisitcsClaimGoodsVolumeWeightService;
        this.logisticsGoodsImageService = logisticsGoodsImageService;
        this.sizeWeightHoldExpressService = sizeWeightHoldExpressService;
    }

    [HttpGet]
    [Route("/admin/logistics/express/checkin/{warehouseId:int}")]
    [NoHibernateAutoTransaction]
    public IActionResult Index(int warehouseId)
    {
        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        WarehouseInfo warehouseInfo = session.Get<WarehouseInfo>(warehouseId);
        if (warehouseInfo == null)
        {
            throw new ValidationException("指定的仓库不存在！");
        }

        LogisticsExpressCheckinIndexModel model = new LogisticsExpressCheckinIndexModel();
        model.Warehouse = warehouseInfo;
        model.CheckinLoginName = loginUser?.LoginName ?? string.Empty;
        model.CheckinUserName = loginUser?.UserName ?? string.Empty;

        var checkinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        model.CheckinPrintType = expressCheckinConfigService.GetVersion3PrintType(checkinConfig);
        model.CheckinPrintCount = expressCheckinConfigService.GetVersion3PrintCount(checkinConfig);

        return View("LogisticsExpressCheckinIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/express/checkin/express")]
    [NoHibernateAutoTransaction]
    public IActionResult Express(int warehouseId, string expressTrackingNo)
    {
        int loginUserId = loginUserService.GetLoginUserId();

        string oldExpressTrackingNo = expressTrackingNo;
        expressTrackingNo = expressTrackingNoCorrectService.RemoveUselessSuffix(expressTrackingNo);

        if (string.IsNullOrWhiteSpace(expressTrackingNo) == false)
        {
            expressTrackingNo = expressTrackingNo.Trim().ToUpper();
        }

        // 查看已经存在的, 还在仓位上的 tracking no 并把信息显示在这里

        LogisticsClaimGoodsInfo claimExist = session.Query<LogisticsClaimGoodsInfo>()
            // .Where(e => e.TrackingNo == expressTrackingNo && e.WarehouseProcessed == false)
            .Where(e => e.TrackingNo == expressTrackingNo) // WarehouseProcessed 在保存的时候进行控制好了
            .Take(1)
            .SingleOrDefault();

        LogisticsExpressCheckinExpressModel model = new LogisticsExpressCheckinExpressModel();

        model.WarehouseId = warehouseId;
        model.Warehouse = session.Get<WarehouseInfo>(warehouseId);
        model.ExpressTrackingNo = expressTrackingNo;
        model.OldExpressTrackingNo = oldExpressTrackingNo;

        int? claimExistShelfId = null;
        StorageLocInfo storageLocExist = null;
        if (claimExist != null)
        {
            model.ClaimId = claimExist.Id;
            model.BillCustomerCode = claimExist.BillCustomerCode;
            model.ExpressCompany = claimExist.ExpressCompany;
            model.CheckinPackageCount = claimExist.ExpressPackageCount;
            model.WoodFrame = claimExist.WoodFrame ?? false;
            model.BarkClean = claimExist.BarkClean ?? false;
            model.Description = claimExist.Description;

            int storageLocIdExist = claimExist.StorageLoc?.Id ?? 0;
            storageLocExist = session.Query<StorageLocInfo>()
                .Fetch(e => e.Shelf)
                .Fetch(e => e.Spec)
                .Where(e => e.Id == storageLocIdExist)
                .Take(1)
                .SingleOrDefault();

            claimExistShelfId = storageLocExist?.Shelf?.Id;

            model.StorageLocName = storageLocExist?.GetFullName();
        }

        LogisticsExpressCheckinUserShelfInfo userSelect = expressCheckinService.GetUserShelfSelect(loginUserId);

        if ((claimExistShelfId ?? 0) != 0)
        {
            model.ShelfId = claimExistShelfId;
        }
        else
        {
            model.ShelfId = userSelect?.Shelf?.Id ?? 0;
        }

        model.ShelfList = expressCheckinService.GetAvaibleShelfList(warehouseId, loginUserId, claimExistShelfId);

        if (model.ShelfId != null)
        {
            int shelfId = model.ShelfId ?? 0;
            model.StorageLocSpecList = GetShelfStorageLocSpecList(shelfId);
        }

        if (storageLocExist != null)
        {
            model.StorageLocSpecId = storageLocExist.Spec?.Id ?? 0;
        }
        else
        {
            model.StorageLocSpecId = userSelect?.StorageLocSpec?.Id ?? 0;
        }

        model.RemarkList = session.Query<LogisticsExpressRemarkInfo>().OrderBy(e => e.Id).ToList();

        // 物品列表
        if (claimExist != null)
        {
            int claimExistId = claimExist.Id;
            IList<LogisticsClaimGoodsEntryInfo> goodsEntryList = session.Query<LogisticsClaimGoodsEntryInfo>()
                .Where(e => e.ClaimId == claimExistId)
                .OrderBy(e => e.Id)
                .ToList();

            IList<int> goodsIdList = goodsEntryList.Select(e => e.GoodsId).ToList();
            IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
                .Where(e => goodsIdList.Contains(e.Id))
                .ToList();

            IList<LogisticsExpressCheckinExpressGoodsModel> goodsModelList = new List<LogisticsExpressCheckinExpressGoodsModel>();

            foreach (LogisticsClaimGoodsEntryInfo goodsEntry in goodsEntryList)
            {
                LogisticsExpressCheckinExpressGoodsModel goodsModel = new LogisticsExpressCheckinExpressGoodsModel();
                goodsModel.EntryId = goodsEntry.Id;
                goodsModel.UiIndex = goodsEntry.Id.ToString();
                goodsModel.GoodsId = goodsEntry.GoodsId;
                goodsModel.GoodsName = logisticsGoodsList.Where(e => e.Id == goodsEntry.GoodsId).Take(1).SingleOrDefault()?.Name;
                goodsModel.Quantity = goodsEntry.Quantity;
                goodsModel.Unit = goodsEntry.Unit;
                goodsModel.UnitList = GoodsUnit.GetList();

                goodsModelList.Add(goodsModel);
            }

            model.GoodsList = goodsModelList;
        }

        // 尺寸重量数据 begin

        LogisticsExpressInfo logisticsExpress = session.Query<LogisticsExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        model.ChildSizeWeightList = new List<LogisticsExpressCheckinSizeWeightChildItem>();

        if (logisticsExpress != null)
        {
            model.Length = logisticsExpress.Length;
            model.Width = logisticsExpress.Width;
            model.Height = logisticsExpress.Height;
            model.Weight = logisticsExpress.Weight;
            model.ExpressPackageCount = logisticsExpress.PackageCount ?? 1;

            int logisticsExpressId = logisticsExpress.Id;
            IList<LogisticsExpressChildInfo> expressChildList = session.Query<LogisticsExpressChildInfo>()
                .Where(e => e.Parent.Id == logisticsExpressId)
                .OrderBy(e => e.Id)
                .ToList();

            IList<LogisticsExpressCheckinSizeWeightChildItem> childItemList = new List<LogisticsExpressCheckinSizeWeightChildItem>();
            foreach (LogisticsExpressChildInfo expressChild in expressChildList)
            {
                LogisticsExpressCheckinSizeWeightChildItem childItem = new LogisticsExpressCheckinSizeWeightChildItem();

                childItem.ExpressChildId = expressChild.Id;
                childItem.ParentTrackingNo = expressTrackingNo;
                childItem.TrackingNo = expressChild.TrackingNo.Replace(expressTrackingNo + "-", "");
                childItem.Length = expressChild.Length;
                childItem.Width = expressChild.Width;
                childItem.Height = expressChild.Height;
                childItem.Weight = expressChild.Weight;
                childItem.PackageCount = expressChild.PackageCount ?? 1;

                childItemList.Add(childItem);
            }

            model.ChildSizeWeightList = childItemList;
        }

        // 尺寸重量数据 end

        // 是不是已经 2.0 创建订单了
        LogisticsOrderGoodsExpressInfo orderExpress = session
            .Query<LogisticsOrderGoodsExpressInfo>()
            .Fetch(e => e.Order)
            .Where(e => e.ExpressTrackingNo == expressTrackingNo
                        && e.Order.OrderStatus != LogisticsOrderStatus.Draft
                        && e.Order.OrderStatus != LogisticsOrderStatus.OrderCanceled
                        && e.Order.OrderStatus != LogisticsOrderStatus.OrderNormalCompleted) // 就判断这几个状态够了的
            .Take(1)
            .SingleOrDefault();

        if (orderExpress != null)
        {
            // throw new ValidationException("快递单号已经存在于现有订单中，请确认快递单号是否正确，或从2.0界面入库！");
            model.Message = "快递单号已经存在于现有订单中，请确认快递单号是否正确，或从2.0界面入库！";
        }

        // 是不是已经预报的快递单号
        var expressExpect = session.Query<LogisticsExpressExpectInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();
        model.ExpressExpect = expressExpect;

        var expressCheckinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        model.AutoRecordCheckinVideo = expressCheckinConfig.AutoRecordCheckinVideo;

        return View("LogisticsExpressCheckinExpress", model);
    }

    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/storage/loc/spec/list")]
    [NoHibernateAutoTransaction]
    public IActionResult StorageLocSpecList(int shelfId)
    {
        LogisticsExpressCheckinExpressModel model = new LogisticsExpressCheckinExpressModel();

        model.StorageLocSpecList = GetShelfStorageLocSpecList(shelfId);

        return View("LogisticsExpressCheckinStorageLocSpecList", model);
    }

    [NonAction]
    private IList<StorageLocSpecInfo> GetShelfStorageLocSpecList(int shelfId)
    {
        // 获取货架仓位规格列表
        IList<StorageLocSpecInfo> specList = session.Query<StorageLocInfo>()
            .Fetch(e => e.Spec)
            .Where(e => e.Shelf.Id == shelfId)
            .Select(e => e.Spec)
            .Distinct()
            .ToList();

        specList = specList.OrderBy(e => e.Length * e.Width * e.Height).ToList();

        return specList;
    }


    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/add/goods")]
    [NoHibernateAutoTransaction]
    public IActionResult AddGoods()
    {
        LogisticsExpressCheckinExpressGoodsModel model = new LogisticsExpressCheckinExpressGoodsModel();
        model.UiIndex = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        model.UnitList = GoodsUnit.GetList();

        return View("LogisticsExpressCheckinExpressGoods", model);
    }

    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/check/size/weight/data")]
    [AdminAuthNoCheck]
    [NoHibernateAutoTransaction]
    public IActionResult CheckSizeWeightData(string expressTrackingNo)
    {
        IList<LogisticsExpressInfo> logisticsExpressList = session.Query<LogisticsExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();

        IList<LogisticsExpressChildInfo> logisticsExpressChildList = session.Query<LogisticsExpressChildInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();

        bool hasSizeWeightData = logisticsExpressList.Count != 0 || logisticsExpressChildList.Count != 0;

        bool hasShortLengthData = false;
        bool hasLightWeightData = false;
        if (hasSizeWeightData) // 如果尺寸重量信息都没有，太轻的尺寸重量信息自然就没有了
        {
            foreach (LogisticsExpressInfo e in logisticsExpressList)
            {
                if (e.Length < 1 || e.Width < 1 || e.Height < 1)
                {
                    hasShortLengthData = true;
                }

                if (e.Weight < 0.1m)
                {
                    hasLightWeightData = true;
                }
            }

            foreach (LogisticsExpressChildInfo c in logisticsExpressChildList)
            {
                if (c.Length < 1 || c.Width < 1 || c.Height < 1)
                {
                    hasShortLengthData = true;
                }

                if (c.Weight < 0.1m)
                {
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

    [HttpPost]
    [Route("/admin/logistics/express/checkin/save")]
    [NoHibernateAutoTransaction]
    public IActionResult CheckinSave(LogisticsExpressCheckinExpressModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int warehouseId = model.WarehouseId ?? 0;
        WarehouseInfo warehouse = session.Query<WarehouseInfo>()
            .Where(e => e.Id == warehouseId && e.Type == WarehouseType.Cn)
            .Take(1)
            .SingleOrDefault();

        if (warehouse == null)
        {
            throw new ValidationException("指定的仓库不存在或不是中国区仓库！");
        }

        if (model.GoodsList == null || model.GoodsList.Count == 0)
        {
            throw new ValidationException("物品列表不能为空！");
        }

        if (model.ShelfId == null || model.ShelfId == 0)
        {
            throw new ValidationException("请指定快递单入库货架！");
        }

        IList<string> errorMessageList = new List<string>();

        IList<LogisticsExpressCheckinExpressGoodsModel> goodsModelList = model.GoodsList;
        int goodsListCount = goodsModelList.Count;
        for (int index = 0; index < goodsListCount; index++)
        {
            LogisticsExpressCheckinExpressGoodsModel goodsModel = goodsModelList[index];

            if (goodsModel.GoodsId == 0)
            {
                errorMessageList.Add($"第 {index + 1} 个物品，扩展名不能为空！");
            }

            if (goodsModel.Quantity <= 0)
            {
                // errorMessageList.Add($"第 {index + 1} 个物品，数量不能为空！");
                errorMessageList.Add($"第 {index + 1} 个物品，数量应该大于0！");
            }

            if (string.IsNullOrWhiteSpace(goodsModel.Unit))
            {
                errorMessageList.Add($"第 {index + 1} 个物品，单位不能为空！");
            }
        }

        if (errorMessageList.Count != 0)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(errorMessageList));
        }

        int loginUserId = loginUserService.GetLoginUserId();

        // 快递单号大写与去掉前后于中间的空格，订单创建时同样也要做这样的事情
        string expressTrackingNo = model.ExpressTrackingNo.Trim().Replace(" ", "").ToUpper(); // 去空格再大写

        // 快递单号验证
        if (expressTrackingNoService.IsValidateTrackingNo(expressTrackingNo) == false)
        {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        int billCustomerId = customerService.GetCustomerIdFromCustomerCode(model.BillCustomerCode);

        //  这个检查先不用了
        /*
        if (billCustomerId == 0)
        {
            throw new ValidationException("不能从面单客户ID中解析确切的客户ID，请检查！");
        }
        */

        if (billCustomerId != 0)
        {
            CustomerInfo billCustomer = session.Get<CustomerInfo>(billCustomerId);
            if (billCustomer == null)
            {
                throw new ValidationException("从面单客户ID中解析的客户ID不存在，请检查！");
            }
        }

        // 获取并检查尺寸重量信息是否已经存在
        IList<LogisticsExpressItem> expressItemList = logisticsExpressService.GetLogisticsExpressItemList(expressTrackingNo);
        if (expressItemList == null || expressItemList.Count == 0)
        {
            throw new ValidationException("快递单尺寸重量信息未录入保存！");
        }

        // 检查快递单号图片，如果不符合会抛出异常
        expressCheckinService.CheckPhotoWhenCheckin(expressTrackingNo);

        int shelfId = model.ShelfId ?? 0;
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException("指定的货架不存在！");
        }

        int storageLocSpecId = model.StorageLocSpecId ?? 0;
        StorageLocSpecInfo storageLocSpec = session.Get<StorageLocSpecInfo>(storageLocSpecId);
        if (storageLocSpec == null)
        {
            throw new ValidationException("指定的仓位规格不存在！");
        }

        if (shelf.Type != ShelfType.Claim)
        {
            throw new ValidationException($"指定的货架 {shelf.Name} 不是申领件货架！");
        }

        LogisticsExpressExpectInfo expressExpect = null;

        int claimId = model.ClaimId;
        bool isCreate = claimId == 0;

        lock (ExpressCheckinLockObject) // lock 时，数据库事务应该放在 lock 区域之内
        {
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();

                LogisticsClaimGoodsInfo claimGoods = null;

                if (isCreate == false)
                {
                    claimGoods = session.Get<LogisticsClaimGoodsInfo>(claimId);
                    if (claimGoods.TrackingNo != model.ExpressTrackingNo)
                    {
                        LogisticsClaimGoodsInfo claimGoodsOther = session.Query<LogisticsClaimGoodsInfo>()
                            .Where(e => e.TrackingNo == expressTrackingNo)
                            .Take(1)
                            .FirstOrDefault();
                        if (claimGoodsOther != null)
                        {
                            throw new ValidationException("该快递单号存在其他入库记录！");
                        }
                    }
                }
                else
                {
                    claimGoods = session.Query<LogisticsClaimGoodsInfo>()
                        .Fetch(e => e.StorageLoc)
                        .Where(e => e.TrackingNo == expressTrackingNo)
                        .Take(1)
                        .FirstOrDefault();

                    if (claimGoods != null && claimGoods.Id != claimId)
                    {
                        throw new ValidationException("该快递单号存在其他入库记录！");
                    }
                }

                /*
                if (claimGoods != null && (claimGoods.Picked == true || LogisticsClaimGoodsStatus.IsCheckedIn(claimGoods.Status)))
                {
                    throw new ValidationException("该快递单号已被会员认领，不能修改更新记录信息！");
                }
                */

                if (claimGoods != null && (claimGoods.WarehouseProcessed == true || LogisticsClaimGoodsStatus.IsOrderCreate(claimGoods.Status)))
                {
                    throw new ValidationException("该快递单号已提交订单，不能修改更新记录信息！");
                }

                if (claimGoods != null && (claimGoods.Status == LogisticsClaimGoodsStatus.WaitReturn))
                {
                    throw new ValidationException("该快递单号当前申请退运中，不能修改更新记录信息！");
                }

                /*
                if (claimGoods != null && (claimGoods.WarehouseProcessed == true || claimGoods.Status == LogisticsClaimGoodsStatus.Order))
                {
                    throw new ValidationException("该快递单号已存在入库记录，并且已经下架或打包，不能修改更新记录信息！");
                }
                */

                // 检查有无物品照片
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsItem in goodsModelList)
                {
                    if (logisticsGoodsImageService.HasGoodsImage(goodsItem.GoodsId) == false)
                    {
                        LogisticsGoodsInfo logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsItem.GoodsId);

                        throw new ValidationException($"物品 #Id:{goodsItem.GoodsId} #{logisticsGoods?.Name} 还没有任何关联照片，请设置物品创建单号，有单号相关订单照片后，再使用该物品名称！");
                    }
                }

                // 已丢弃的快递，应该可以在这里更新入库信息的

                // 物品类型
                IList<int> goodsIdList = new List<int>();
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsItem in goodsModelList)
                {
                    goodsIdList.Add(goodsItem.GoodsId);
                }

                LogisticsGoodsType goodsType = logisticsGoodsTypeService.GetGoodsTypeFromLogisticsGoodsIdList(goodsIdList);

                // 如果已经在 快递单号预报 里面的，就设置 快递单号预报为 录货值 状态
                // bool hasOrder = false;
                // int customerId = 0;

                expressExpect = session.Query<LogisticsExpressExpectInfo>()
                    .Where(e => e.TrackingNo == expressTrackingNo)
                    .Take(1)
                    .SingleOrDefault();

                if (expressExpect != null && expressExpect.Status == LogisticsExpressExpectStatus.Create)
                {
                    expressExpect.Status = LogisticsExpressExpectStatus.Claim;
                    session.Save(expressExpect);
                    session.Flush();

                    // customerId = expressExpect.Customer.Id;
                }

                // 是不是已经 2.0 创建订单了
                LogisticsOrderGoodsExpressInfo orderExpress = session
                    .Query<LogisticsOrderGoodsExpressInfo>()
                    .Fetch(e => e.Order)
                    .Where(e => e.ExpressTrackingNo == expressTrackingNo
                                && e.Order.OrderStatus != LogisticsOrderStatus.Draft
                                && e.Order.OrderStatus != LogisticsOrderStatus.OrderCanceled
                                && e.Order.OrderStatus != LogisticsOrderStatus.OrderNormalCompleted) // 就判断这几个状态够了的
                    .Take(1)
                    .SingleOrDefault();

                if (orderExpress != null)
                {
                    throw new ValidationException("快递单号已经存在于现有订单中，请确认快递单号是否正确，或从2.0界面入库！");

                    /*
                    hasOrder = true;
                    customerId = orderExpress.Order?.Customer?.Id ?? 0;
                    */
                }

                if (claimGoods == null)
                {
                    claimGoods = new LogisticsClaimGoodsInfo();
                    claimGoods.TrackingNo = expressTrackingNo;
                    claimGoods.AoPinTrackingNo = string.Empty; // 后面会回填这个澳拼跟踪号
                    claimGoods.Picked = false;
                    claimGoods.Status = LogisticsClaimGoodsStatus.WaitPick; // 初始化为 WaitPick     
                    claimGoods.ShelfStatus = ExpressShelfStatus.ToOnShelf; // 初始化为 待上架
                    claimGoods.WarehouseProcessed = false;
                    claimGoods.AddTime = DateTime.Now;
                }

                IList<string> editDescList = new List<string>();

                // 仓库
                if (isCreate == false && (claimGoods.Warehouse?.Id ?? 0) != warehouseId)
                {
                    editDescList.Add($"仓库:{claimGoods.Warehouse?.Name}=>{warehouse?.Name}");
                }

                claimGoods.Warehouse = warehouse;

                // 快递公司
                if (isCreate == false && claimGoods.ExpressCompany != model.ExpressCompany)
                {
                    editDescList.Add($"快递公司:{claimGoods.ExpressCompany}=>{model.ExpressCompany}");
                }

                claimGoods.ExpressCompany = model.ExpressCompany;

                // 物品类型
                if (isCreate == false && claimGoods.GoodsType != goodsType)
                {
                    editDescList.Add($"物品类型:{claimGoods.GoodsType?.Name}=>{goodsType?.Name}");
                }

                claimGoods.GoodsType = goodsType;

                // 添加用户
                if (isCreate == false && claimGoods.AddUserId != loginUserId)
                {
                    UserInfo oldAddUser = session.Get<UserInfo>(claimGoods.AddUserId);
                    UserInfo newAddUser = session.Get<UserInfo>(loginUserId);
                    editDescList.Add($"添加用户:{oldAddUser?.UserName}=>{newAddUser?.UserName}");
                }

                claimGoods.AddUserId = loginUserId;

                /*
                if (hasOrder)
                {
                    claimGoods.Status = LogisticsClaimGoodsStatus.Order;
                    claimGoods.PickTime = DateTime.Now;
                    claimGoods.CustomerId = customerId;
                    claimGoods.Order = orderExpress.Order;
                }
                */

                // 包裹数量
                int prePackageCount = claimGoods.ExpressPackageCount; // 如果是新建，不是修改保存的话，这个值就是0
                int packageCount = expressItemList.Sum(e => e.Quantity);

                if (packageCount <= 0)
                {
                    packageCount = 1;
                }

                if (isCreate == false && prePackageCount != packageCount)
                {
                    editDescList.Add($"包裹数量:{prePackageCount}=>{packageCount}");
                }

                claimGoods.ExpressPackageCount = packageCount;

                // 打木架
                if (claimGoods.WoodFrame == false && model.WoodFrame == true)
                {
                    claimGoods.WoodFrameAddMode = ExpressWoodFrameAddMode.Checkin;
                }

                if (isCreate == false && claimGoods.WoodFrame != model.WoodFrame)
                {
                    string woodFrameBeforeDesc = claimGoods.WoodFrame == true ? "是" : "否";
                    string woodFrameAfterDesc = model.WoodFrame == true ? "是" : "否";
                    editDescList.Add($"打木架:{woodFrameBeforeDesc}=>{woodFrameAfterDesc}");
                }

                claimGoods.WoodFrame = model.WoodFrame;

                // 清理树皮
                if (claimGoods.BarkClean == false && model.BarkClean == true)
                {
                    claimGoods.BarkCleanAddMode = ExpressBarkCleanAddMode.Checkin;
                }

                if (isCreate == false && claimGoods.BarkClean != model.BarkClean)
                {
                    string barkCleanBeforeDesc = claimGoods.BarkClean == true ? "是" : "否";
                    string barkCleanAfterDesc = model.BarkClean == true ? "是" : "否";
                    editDescList.Add($"清理树皮杂物:{barkCleanBeforeDesc}=>{barkCleanAfterDesc}");
                }

                claimGoods.BarkClean = model.BarkClean;

                // 备注
                if (isCreate == false && claimGoods.Description != model.Description)
                {
                    editDescList.Add($"备注:{claimGoods.Description}=>{model.Description}");
                }

                claimGoods.Description = model.Description;

                // 面单客户

                if (isCreate == false && claimGoods.BillCustomerCode != model.BillCustomerCode)
                {
                    editDescList.Add($"面单客户:{claimGoods.BillCustomerCode}=>{model.BillCustomerCode}");
                }

                claimGoods.BillCustomerCode = model.BillCustomerCode;
                claimGoods.BillCustomerId = billCustomerId;

                // 如果是新入库的，或者是 原来的但修改了货架, 或者是修改了仓位规格
                if (isCreate == true || ( /* isCreate == false && */ (claimGoods.StorageLoc?.Shelf?.Id ?? 0) != model.ShelfId || (claimGoods.StorageLoc?.Spec?.Id ?? 0) != model.StorageLocSpecId))
                {
                    StorageLocInfo recommandStorage = expressCheckinService.GetRecommandStorage(shelf, storageLocSpec); // 从这里取到的仓位ID，已经保证是仓位上物品不超过限制了
                    if (recommandStorage == null)
                    {
                        throw new ValidationException("未能获取推荐仓位，请尝试重新选择货架或仓位规格！");
                    }

                    // 检查仓位和货架是否可用
                    storageLocService.CheckStorageLoc((recommandStorage?.Id ?? 0), ShelfType.Claim);

                    claimGoods.StorageLoc = recommandStorage;
                    claimGoods.CheckinStorageLoc = recommandStorage;
                }

                if (expressExpect != null
                    && string.IsNullOrWhiteSpace(expressExpect.Description) == false // 预报里面的描述非空
                    && string.IsNullOrWhiteSpace(claimGoods.CustomerRemark) == true) // 申领件里面的客户备注为空
                {
                    claimGoods.CustomerRemark = expressExpect.Description;
                }

                session.SaveOrUpdate(claimGoods);
                session.Flush();

                claimId = claimGoods.Id;
                if (claimId == 0)
                {
                    throw new Exception("快递单号没能保存成功！");
                }

                // 如果是新增（新入库），或者入库信息修改时，总箱数有变动
                if (prePackageCount != packageCount)
                {
                    // 生成澳拼跟踪号，返回主单号
                    var mainTrackingNo = aoPinTrackingNoService.Generate(claimId);

                    claimGoods.AoPinTrackingNo = mainTrackingNo;
                    session.Update(claimGoods);
                    session.Flush();
                }

                // 删除被移除的物品
                IList<LogisticsClaimGoodsEntryInfo> goodsEntryList = new List<LogisticsClaimGoodsEntryInfo>();
                if (isCreate == false)
                {
                    goodsEntryList = session.Query<LogisticsClaimGoodsEntryInfo>()
                        .Where(e => e.ClaimId == claimId)
                        .ToList();

                    // 移除界面上没有，但数据库中有的物品
                    IList<int> entryIdListFromUi = goodsModelList.Select(e => e.EntryId).ToList();
                    IList<LogisticsClaimGoodsEntryInfo> goodsEntryToDelete = goodsEntryList
                        .Where(e => entryIdListFromUi.Contains(e.Id) == false)
                        .ToList();

                    foreach (LogisticsClaimGoodsEntryInfo ce in goodsEntryToDelete)
                    {
                        if (isCreate == false)
                        {
                            LogisticsGoodsInfo goodsRemove = session.Get<LogisticsGoodsInfo>(ce.GoodsId);
                            if (goodsRemove != null)
                            {
                                editDescList.Add($"移除物品:{goodsRemove.Name}*{ce.Quantity}{ce.Unit}");
                            }
                        }

                        session.Delete(ce);
                    }

                    session.Flush();
                }

                // 后续处理就不需要从数据库获取这个列表
                IList<LogisticsClaimGoodsEntryInfo> claimEntryList = new List<LogisticsClaimGoodsEntryInfo>();

                // 保存物品
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsItem in goodsModelList)
                {
                    LogisticsClaimGoodsEntryInfo claimGoodsEntry = null;
                    if (goodsItem.EntryId != 0)
                    {
                        claimGoodsEntry = goodsEntryList.Where(e => e.Id == goodsItem.EntryId).Take(1).SingleOrDefault();
                    }

                    if (claimGoodsEntry == null)
                    {
                        claimGoodsEntry = new LogisticsClaimGoodsEntryInfo();
                    }

                    claimGoodsEntry.ClaimId = claimId;

                    if (isCreate == false &&
                        (claimGoodsEntry.GoodsId != goodsItem.GoodsId
                         || claimGoodsEntry.Quantity != goodsItem.Quantity
                         || claimGoodsEntry.Unit != goodsItem.Unit))
                    {
                        LogisticsGoodsInfo goodsEditAfter = session.Get<LogisticsGoodsInfo>(goodsItem.GoodsId);
                        if (claimGoodsEntry.Id != 0) // 这个时候是修改(isCreate == false)，编辑物品
                        {
                            LogisticsGoodsInfo goodsEditBefore = session.Get<LogisticsGoodsInfo>(claimGoodsEntry.GoodsId);

                            editDescList.Add($"修改物品: {goodsEditBefore?.Name}{claimGoodsEntry?.Quantity}{claimGoodsEntry?.Unit}=>{goodsEditAfter?.Name}{goodsItem.Quantity}{goodsItem.Unit}");
                        }
                        else // 这个时候是修改(isCreate == false)，添加物品
                        {
                            editDescList.Add($"添加物品: {goodsEditAfter?.Name}{goodsItem.Quantity}{goodsItem.Unit}");
                        }
                    }

                    claimGoodsEntry.GoodsId = goodsItem.GoodsId;
                    claimGoodsEntry.Quantity = goodsItem.Quantity;
                    claimGoodsEntry.Unit = goodsItem.Unit;

                    session.SaveOrUpdate(claimGoodsEntry);

                    claimEntryList.Add(claimGoodsEntry);
                }

                session.Flush();

                /*
                // 3.0 快递单入库的时候，如果快递单已经创建订单（可能是2.0的界面创建的），就把入库的认领件设置成已认领的，并把认领件的仓位设置到订单快递单号仓位(这时一般还没仓位)上
                if (orderExpress != null)
                {
                    StorageLocInfo storageLoc = claimGoods.StorageLoc;

                    int orderExpressId = orderExpress.Id;
                    session.Query<LogisticsOrderGoodsInfo>()
                        .Where(e => e.Express.Id == orderExpressId)
                        .UpdateBuilder()
                        .Set(e => e.StorageLoc, storageLoc)
                        .Set(e => e.UpdateTime, DateTime.Now)
                        .Update();
                }
                */

                StorageLocInfo storageLoc = claimGoods.StorageLoc;
                if (isCreate)
                {
                    string logDesc = storageLoc?.GetFullName() ?? string.Empty;
                    if (expressExpect != null)
                    {
                        logDesc += $"，面单客户按预报客户：{expressExpect.Customer?.Id ?? 0}";
                    }

                    expressLogService.UserClaimLog(claimGoods.Id, ExpressLogType.Checkin, logDesc, loginUserId);
                }
                else
                {
                    string logDesc = (storageLoc?.GetFullName() ?? string.Empty) + " " + string.Join(";", editDescList);
                    if (expressExpect != null)
                    {
                        logDesc += $"，面单客户按预报客户：{expressExpect.Customer?.Id ?? 0}";
                    }

                    expressLogService.UserClaimLog(claimGoods.Id, ExpressLogType.CheckinEdit, logDesc, loginUserId);
                }

                logisitcsClaimGoodsVolumeWeightService.SumSingle(claimGoods.Id);

                // 超大超重hold
                sizeWeightHoldExpressService.TryAddHold(claimGoods.Id, expressItemList, claimEntryList);

                expressCheckinService.SaveUserShelfSelect(loginUserId, model.ShelfId ?? 0, model.StorageLocSpecId ?? 0);

                transaction.Commit();
            }
            catch (Exception exp)
            {
                transaction?.Rollback();

                if (exp is ValidationException == false)
                {
                    log.Error(exp);
                }

                throw;
            }
        }

        if (isCreate == true) // 只在首次入库的时候提醒，修改信息的时候不提醒
        {
            // 发送消息提醒，虽然不在同一个事务内，但也是在前面都成功的情况下才发送的
            if (expressExpect != null) // 优先按预报的发送消息
            {
                if (expressExpect.Customer != null) // 这个 Customer 应该不可空才对
                {
                    customerMessageService.SendMessageYuBaoHuoZhi(expressExpect.Customer, expressExpect.TrackingNo);
                }
            }
            else if (billCustomerId != 0) // 没预报但有面单会员ID
            {
                CustomerInfo billCustomer = session.Get<CustomerInfo>(billCustomerId);
                if (billCustomer != null)
                {
                    customerMessageService.SendMessageWoDeShenLingJian(billCustomer);
                }
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "快递单号保存入库成功！" },
        });
    }

    #region 搜索物品列表

    [HttpPost]
    [Route("/admin/logistics/express/checkin/search/goods/list/data")]
    [NoHibernateAutoTransaction]
    public IActionResult SearchGoodsListData(int warehouseId, string param)
    {
        IList<LogisticsGoodsInfo> goodsList = new List<LogisticsGoodsInfo>();
        int totalCount = 0;

        if (string.IsNullOrWhiteSpace(param) == false)
        {
            goodsList = GetSearchGoodsCriteria(warehouseId, param).List<LogisticsGoodsInfo>();
            totalCount = GetSearchGoodsCriteria(warehouseId, param).SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        IList<IDictionary<string, object>> dataList = new List<IDictionary<string, object>>();

        foreach (LogisticsGoodsInfo goodsInfo in goodsList)
        {
            string goodsAdminName = goodsInfo?.GetGoodsAdminName();

            dataList.Add(new Dictionary<string, object> {
                { "id", goodsInfo.Id },
                { "text", goodsAdminName },
                { "type", goodsInfo.Type }
                // 可以放入物品其他属性
            });
        }

        return Json(new Dictionary<string, object> {
            { "items", dataList },
            { "totalCount", totalCount }
        });
    }

    [NonAction]
    private ICriteria GetSearchGoodsCriteria(int warehouseId, string param)
    {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);

        IList<LogisticsGoodsCate> goodsCateList = null;
        if (warehouse != null)
        {
            if (warehouse.Cate == WarehouseCate.Sea)
            {
                goodsCateList = new List<LogisticsGoodsCate>() {
                    LogisticsGoodsCate.Sea,
                    LogisticsGoodsCate.SeaAir,
                    LogisticsGoodsCate.SeaExpress,
                    LogisticsGoodsCate.SeaAirExpress,
                };
            }

            if (warehouse.Cate == WarehouseCate.Air)
            {
                goodsCateList = new List<LogisticsGoodsCate>() {
                    LogisticsGoodsCate.Air,
                    LogisticsGoodsCate.SeaAir,
                    LogisticsGoodsCate.AirExpress,
                    LogisticsGoodsCate.SeaAirExpress,
                };
            }
        }

        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();
        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

        if (goodsCateList != null)
        {
            criteria = criteria.Add(Restrictions.In(nameof(LogisticsGoodsInfo.Cate), goodsCateList.ToArray()));
        }

        criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status != LogisticsGoodsStatus.Disabled));

        // criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.Confirmed)); 这个物品搜索时，可以搜还没确认的物品，海关报关会用确认过的物品
        criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(param, MatchMode.Anywhere));

        return criteria;
    }

    #endregion

    [HttpPost]
    [Route("/admin/logistics/express/checkin/get/express/com/name")]
    [NoHibernateAutoTransaction]
    public IActionResult GetExpressComName(string expressTrackingNo)
    {
        string comName = expressCompanyService.GetExpressCom(expressTrackingNo);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "content", comName }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/express/checkin/claim/get/print/data/by/id")]
    [AdminAuthNoCheck]
    [NoHibernateAutoTransaction]
    public IActionResult GetPrintDataById(int claimId)
    {
        LogisticsClaimGoodsInfo claimGoods = session.Get<LogisticsClaimGoodsInfo>(claimId);
        if (claimGoods == null)
        {
            throw new ValidationException($"指定的快递单号#{claimId}不存在");
        }

        // string expressTrackingNo = claimGoods.TrackingNo;
        string customerCode = claimGoods.BillCustomerCode;
        string orderCode = string.Empty;

        int storageLocId = claimGoods.StorageLoc?.Id ?? 0;
        StorageLocInfo storageLocInfo = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Fetch(e => e.Spec)
            .Where(e => e.Id == storageLocId)
            .Take(1)
            .SingleOrDefault();
        string storageLoc = storageLocInfo?.GetFullName() ?? string.Empty;

        string description = claimGoods.Description ?? string.Empty;

        string checkinPackageCount = claimGoods.ExpressPackageCount.ToString();

        int checkinUserId = claimGoods.AddUserId;
        UserInfo checkinUser = session.Get<UserInfo>(checkinUserId);
        string loginName = checkinUser?.UserName ?? string.Empty;

        string checkinTime = claimGoods.AddTime.ToString("MM-dd HH:mm");
        string goodsTypeShortName = claimGoods.GoodsType?.CheckinLabelShortName;

        var aoPinSubTrackingNoList = aoPinTrackingNoService.ListSubTrackingNo(claimGoods.Id);

        // 如果还没有澳拼单号的，就生成。有些历史入库的快递单号的，还没有澳拼入库单号。这个逻辑主要用在历史入库快递单，补打标签的时候
        if (aoPinSubTrackingNoList == null || aoPinSubTrackingNoList.Count == 0)
        {
            string mainTrackingNo = aoPinTrackingNoService.Generate(claimGoods.Id);
            claimGoods.AoPinTrackingNo = mainTrackingNo;
            session.Update(claimGoods);
            session.Flush();

            aoPinSubTrackingNoList = aoPinTrackingNoService.ListSubTrackingNo(claimGoods.Id);
        }

        aoPinSubTrackingNoList = aoPinSubTrackingNoList.OrderBy(e => e.AoPinTrackingNo).ToList(); // 澳拼入库子单号排序一下

        var aoPinSubTrackingNoItemList = new List<IDictionary<string, object>>();
        foreach (var subTrackingNo in aoPinSubTrackingNoList)
        {
            IDictionary<string, object> subTrackingNoItem = new Dictionary<string, object>();
            subTrackingNoItem["aoPinTrackingNo"] = subTrackingNo.AoPinTrackingNo;

            subTrackingNoItem["trackingNo"] = subTrackingNo.LogisticsExpress?.TrackingNo ?? string.Empty;
            if (subTrackingNo.ChildExpress != null)
            {
                subTrackingNoItem["trackingNo"] = subTrackingNo.ChildExpress?.TrackingNo ?? string.Empty;
            }

            aoPinSubTrackingNoItemList.Add(subTrackingNoItem);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取数据成功！" },
            { "expressTrackingNo", claimGoods.TrackingNo },
            { "customerCode", customerCode },
            { "warehouseNote", description },
            { "description", description },
            { "checkinPackageCount", checkinPackageCount },
            { "orderCode", orderCode },
            { "storageLoc", storageLoc },
            { "loginName", loginName },
            { "checkinTime", checkinTime },
            { "goodsTypeName", goodsTypeShortName },
            { "aoPinSubTNList", aoPinSubTrackingNoItemList }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/express/checkin/claim/get/print/data")]
    [AdminAuthNoCheck]
    [NoHibernateAutoTransaction]
    public IActionResult GetPrintData(string expressTrackingNo)
    {
        LogisticsClaimGoodsInfo claimGoods = session.Query<LogisticsClaimGoodsInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (claimGoods == null)
        {
            throw new ValidationException($"指定的快递单号{expressTrackingNo}不存在");
        }

        return GetPrintDataById(claimGoods.Id);
    }

    [HttpPost]
    [Route("/admin/logistics/express/checkin/claim/get/print/data/manual")]
    [AdminAuthNoCheck]
    [NoHibernateAutoTransaction]
    public IActionResult GetPrintDataManual(string expressTrackingNo)
    {
        LogisticsClaimGoodsInfo claimGoods = session.Query<LogisticsClaimGoodsInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (claimGoods == null)
        {
            throw new ValidationException($"指定的快递单号{expressTrackingNo}不存在");
        }


        var printData = GetPrintDataById(claimGoods.Id);

        int loginUserId = loginUserService.GetLoginUserId();
        expressLogService.UserClaimLog(claimGoods.Id, ExpressLogType.ManualPrintLabel, loginUserId);

        return printData;
    }

    #region size weight

    [HttpPost]
    [Route("/admin/logistics/express/checkin/save/express/size/weight")]
    public IActionResult SaveSizeWeight(LogisticsExpressCheckinSizeWeightModel model)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (model.Length != null && model.Length.Value <= 0)
        {
            throw new ValidationException("长度应该大于0！");
        }

        if (model.Width != null && model.Width.Value <= 0)
        {
            throw new ValidationException("宽度应该大于0！");
        }

        if (model.Height != null && model.Height.Value <= 0)
        {
            throw new ValidationException("高度应该大于0！");
        }

        if (model.PackageCount != null && model.PackageCount.Value <= 0)
        {
            throw new ValidationException("数量应该大于0！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        string expressTrackingNo = model.ExpressTrackingNo;

        // 检查快递单号状态
        LogisticsOrderGoodsExpressInfo orderExpress = session.Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.ExpressTrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();
        if (orderExpress != null && orderExpress.Status == LogisticsOrderGoodsExpressStatus.Packed)
        {
            throw new ValidationException("已打包快递单号不能，保存尺寸重量信息");
        }

        bool shouldReGenerateAoPinTrackingNo = false;

        LogisticsExpressInfo logisticsExpress = session.Query<LogisticsExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        bool logisticsExpressCreate = false;
        if (logisticsExpress == null)
        {
            logisticsExpressCreate = true;

            logisticsExpress = new LogisticsExpressInfo();
            logisticsExpress.TrackingNo = expressTrackingNo;
            logisticsExpress.Status = LogisticsExpressStatus.Sign;
            logisticsExpress.SignTime = DateTime.Now;
        }

        if (logisticsExpressCreate == true)
        {
            shouldReGenerateAoPinTrackingNo = true; // 一般这种情况下也不需要重新生成的，申领件肯定还没入库
        }

        StringBuilder expressEditLogDescBuiler = new StringBuilder();

        // 长
        if (logisticsExpressCreate == false && logisticsExpress.Length != model.Length)
        {
            expressEditLogDescBuiler.Append($"长:{logisticsExpress.Length}=>{model.Length}");
        }

        logisticsExpress.Length = model.Length;

        // 宽
        if (logisticsExpressCreate == false && logisticsExpress.Width != model.Width)
        {
            expressEditLogDescBuiler.Append($"宽:{logisticsExpress.Width}=>{model.Width}");
        }

        logisticsExpress.Width = model.Width;

        // 高
        if (logisticsExpressCreate == false && logisticsExpress.Height != model.Height)
        {
            expressEditLogDescBuiler.Append($"高:{logisticsExpress.Height}=>{model.Height}");
        }

        logisticsExpress.Height = model.Height;

        // 重量
        if (logisticsExpressCreate == false && logisticsExpress.Weight != model.Weight)
        {
            expressEditLogDescBuiler.Append($"重量:{logisticsExpress.Weight}=>{model.Weight}");
        }

        logisticsExpress.Weight = model.Weight;

        // 箱数
        if (logisticsExpressCreate == false && logisticsExpress.PackageCount != model.PackageCount)
        {
            expressEditLogDescBuiler.Append($"箱数:{logisticsExpress.PackageCount}=>{model.PackageCount}");

            shouldReGenerateAoPinTrackingNo = true;
        }

        logisticsExpress.PackageCount = model.PackageCount;

        session.SaveOrUpdate(logisticsExpress);
        session.Flush();


        if (logisticsExpressCreate == true)
        {
            string addLogDesc = $"长:{logisticsExpress.Length}, 宽:{logisticsExpress.Width}, 高:{logisticsExpress.Height}, 重量:{logisticsExpress.Weight}, 箱数:{logisticsExpress.PackageCount}";
            expressLogService.UserLog(expressTrackingNo, ExpressLogType.SizeWeightAdd, addLogDesc, loginUserId);
        }
        else
        {
            expressLogService.UserLog(expressTrackingNo, ExpressLogType.SizeWeightEdit, expressEditLogDescBuiler.ToString(), loginUserId);
        }


        int logisticsExpressId = logisticsExpress.Id;

        // 保存子单号尺寸
        {
            IList<LogisticsExpressCheckinSizeWeightChildItem> childSizeWeightList = model.ChildSizeWeightList;
            if (childSizeWeightList == null)
            {
                childSizeWeightList = new List<LogisticsExpressCheckinSizeWeightChildItem>();
            }

            // 检查尺寸合法性
            foreach (var childSizeWeight in childSizeWeightList)
            {
                if (childSizeWeight.Length != null && childSizeWeight.Length.Value <= 0)
                {
                    throw new ValidationException($"长度值{childSizeWeight.Length.Value}应该大于0！");
                }

                if (childSizeWeight.Width != null && childSizeWeight.Width.Value <= 0)
                {
                    throw new ValidationException($"宽度值{childSizeWeight.Width}.Value}}应该大于0！");
                }

                if (childSizeWeight.Height != null && childSizeWeight.Height.Value <= 0)
                {
                    throw new ValidationException($"高度值{childSizeWeight.Height.Value}应该大于0！");
                }

                if (childSizeWeight.PackageCount != null && childSizeWeight.PackageCount.Value <= 0)
                {
                    throw new ValidationException($"数量值{childSizeWeight.PackageCount.Value}应该大于0！");
                }
            }

            IList<int> childSizeWeightIdList = childSizeWeightList.Select(e => e.ExpressChildId).ToList();

            IList<LogisticsExpressChildInfo> childSizeWeightListInDb = session.Query<LogisticsExpressChildInfo>()
                .Where(e => e.Parent.Id == logisticsExpressId)
                .ToList();

            // 子单号尺寸，数据库里面有界面上没有的删除
            foreach (var chidSizeWeightInDb in childSizeWeightListInDb)
            {
                if (childSizeWeightIdList.Contains(chidSizeWeightInDb.Id) == false)
                {
                    // 先删除可能已经生成的澳拼入库子单号，否则接下来的删除会有外键引用错误
                    int chidSizeWeightInDbId = chidSizeWeightInDb.Id;
                    session.Query<AoPinTrackingNoExpressInfo>()
                        .Where(e => e.ChildExpress != null && e.ChildExpress.Id == chidSizeWeightInDbId)
                        .Delete();

                    // 再删除子单号尺寸
                    session.Delete(chidSizeWeightInDb);

                    string deleteLogDesc = $"长:{chidSizeWeightInDb.Length}, 宽:{chidSizeWeightInDb.Width}, 高:{chidSizeWeightInDb.Height}, 重量:{chidSizeWeightInDb.Weight}, 箱数:{chidSizeWeightInDb.PackageCount}";
                    expressLogService.UserLog(expressTrackingNo, ExpressLogType.SizeWeightDelete, deleteLogDesc, loginUserId);

                    shouldReGenerateAoPinTrackingNo = true;
                }
            }

            session.Flush();

            // 子单号尺寸，界面上有的，新增或更新
            foreach (LogisticsExpressCheckinSizeWeightChildItem childItem in childSizeWeightList)
            {
                if (childItem.ExpressChildId == 0)
                {
                    LogisticsExpressChildInfo expressChild = new LogisticsExpressChildInfo();
                    // expressChild.Id = childItem.ExpressChildId;
                    expressChild.Parent = logisticsExpress;

                    string subTrackingNo = $"{expressTrackingNo}-{childItem.TrackingNo}";
                    expressChild.TrackingNo = subTrackingNo;
                    expressChild.Length = childItem.Length;
                    expressChild.Width = childItem.Width;
                    expressChild.Height = childItem.Height;
                    expressChild.Weight = childItem.Weight;
                    expressChild.PackageCount = childItem.PackageCount;

                    session.Save(expressChild);

                    string addLogDesc = $"子单号:{subTrackingNo}, 长:{expressChild.Length}, 宽:{expressChild.Width}, 高:{expressChild.Height}, 重量:{expressChild.Weight}, 箱数:{expressChild.PackageCount}";
                    expressLogService.UserLog(expressTrackingNo, ExpressLogType.SizeWeightAdd, addLogDesc, loginUserId);

                    shouldReGenerateAoPinTrackingNo = true;
                }
                else
                {
                    LogisticsExpressChildInfo expressChild = session.Get<LogisticsExpressChildInfo>(childItem.ExpressChildId);
                    expressChild.Parent = logisticsExpress;

                    StringBuilder childEditLogDescBuiler = new StringBuilder();

                    string subTrackingNo = $"{expressTrackingNo}-{childItem.TrackingNo}";
                    if (expressChild.TrackingNo != subTrackingNo)
                    {
                        expressChild.TrackingNo = subTrackingNo;

                        childEditLogDescBuiler.Append($"子单号:{expressChild.TrackingNo}=>{subTrackingNo}");
                    }

                    if (expressChild.Length != childItem.Length)
                    {
                        expressChild.Length = childItem.Length;

                        childEditLogDescBuiler.Append($"长:{expressChild.Length}=>{childItem.Length}");
                    }

                    if (expressChild.Width != childItem.Width)
                    {
                        expressChild.Width = childItem.Width;

                        childEditLogDescBuiler.Append($"宽:{expressChild.Width}=>{childItem.Width}");
                    }

                    if (expressChild.Height != childItem.Height)
                    {
                        expressChild.Height = childItem.Height;

                        childEditLogDescBuiler.Append($"高:{expressChild.Height}=>{childItem.Height}");
                    }

                    if (expressChild.Weight != childItem.Weight)
                    {
                        expressChild.Weight = childItem.Weight;

                        childEditLogDescBuiler.Append($"重量:{expressChild.Weight}=>{childItem.Weight}");
                    }

                    if (expressChild.PackageCount != childItem.PackageCount)
                    {
                        expressChild.PackageCount = childItem.PackageCount;

                        childEditLogDescBuiler.Append($"箱数:{expressChild.PackageCount}=>{childItem.PackageCount}");

                        shouldReGenerateAoPinTrackingNo = true;
                    }

                    expressLogService.UserLog(expressTrackingNo, ExpressLogType.SizeWeightEdit, childEditLogDescBuiler.ToString(), loginUserId);

                    session.Update(expressChild);
                }
            }

            // 如果是已入库的快递单，就要更新包裹数量；并且可能生成新的澳拼入库子单号
            var claimGoods = session.Query<LogisticsClaimGoodsInfo>()
                .Where(e => e.TrackingNo == expressTrackingNo)
                .Take(1)
                .SingleOrDefault();

            if (claimGoods != null)
            {
                // 如果子单号有删除，并且是已入库的快递单的，就重新生成澳拼子单号
                if (shouldReGenerateAoPinTrackingNo)
                {
                    // 这里是重新生成子单号
                    var mainTrackingNo = aoPinTrackingNoService.Generate(claimGoods.Id);
                    claimGoods.AoPinTrackingNo = mainTrackingNo; // 设置主单号
                }

                int packageCount = (logisticsExpress.PackageCount ?? 1) + childSizeWeightList.Sum(e => (e.PackageCount ?? 1));
                if (packageCount <= 0)
                {
                    packageCount = 1;
                }

                claimGoods.ExpressPackageCount = packageCount;

                session.Update(claimGoods);

                session.Flush();

                logisitcsClaimGoodsVolumeWeightService.SumSingle(claimGoods.Id);
            }

            session.Flush();
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "尺寸重量数据保存成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/express/checkin/new/express/size/weight/row")]
    public IActionResult NewExpressSizeWeightRow(string expressTrackingNo)
    {
        LogisticsExpressCheckinSizeWeightChildItem model = new LogisticsExpressCheckinSizeWeightChildItem();
        model.ParentTrackingNo = expressTrackingNo;
        model.Length = null;
        model.Width = null;
        model.Height = null;
        model.Weight = null;
        model.PackageCount = 1;
        return View("LogisticsExpressCheckinSizeWeightRow", model);
    }

    #endregion
}