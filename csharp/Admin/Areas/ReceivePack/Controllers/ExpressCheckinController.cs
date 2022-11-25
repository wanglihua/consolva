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
using shunda.Core.Hibernate;
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
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

/// <summary>
/// 快递入库
/// </summary>
[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.ExpressCheckin)]
public class ExpressCheckinController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

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

    // private readonly CustomerMessageService customerMessageService;
    private ExpressLogService expressLogService;
    private ExpressSizeWeightService expressSizeWeightService;
    private LogisticsGoodsImageService logisticsGoodsImageService;

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
        // CustomerMessageService customerMessageService,
        ExpressLogService expressLogService,
        ExpressSizeWeightService expressSizeWeightService,
        LogisticsGoodsImageService logisticsGoodsImageService)
    {
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
        // this.customerMessageService = customerMessageService;
        this.expressLogService = expressLogService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.logisticsGoodsImageService = logisticsGoodsImageService;
    }

    [HttpGet]
    [Route("/admin/express/checkin/{warehouseId:int}")]
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
        model.CheckinPrintType = expressCheckinConfigService.GetLabelType(checkinConfig);
        model.CheckinPrintCount = expressCheckinConfigService.GetPrintCount(checkinConfig);

        return View("ExpressCheckinIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/checkin/express")]
    public IActionResult Express(int warehouseId, string expressTrackingNo)
    {
        int loginUserId = loginUserService.GetLoginUserId();

        expressTrackingNo = expressTrackingNoService.RemoveUselessSuffix(expressTrackingNo);

        if (string.IsNullOrWhiteSpace(expressTrackingNo) == false)
        {
            expressTrackingNo = expressTrackingNo.Trim().ToUpper();
        }

        // 查看已经存在的, 还在仓位上的 tracking no 并把信息显示在这里

        ExpressInfo? expressExist = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        LogisticsExpressCheckinExpressModel model = new LogisticsExpressCheckinExpressModel();

        model.WarehouseId = warehouseId;
        model.Warehouse = session.Get<WarehouseInfo>(warehouseId);
        model.ExpressTrackingNo = expressTrackingNo;

        int? expressExistShelfId = null;
        StorageLocInfo? storageLocExist = null;
        if (expressExist != null)
        {
            int expressExistId = expressExist.Id;

            model.ClaimId = expressExist.Id;
            model.BillCustomerCode = expressExist.BillCustomerCode;
            model.ExpressCompany = expressExist.ExpressCompany;
            model.PackageCount = expressExist.PackageCount;

            // 仓库备注

            ExpressRemarkInfo? expressRemark = session.Query<ExpressRemarkInfo>()
                .Where(e => e.Express!.Id == expressExistId)
                .Take(1)
                .SingleOrDefault();

            model.Description = expressRemark?.Remark ?? string.Empty;

            int storageLocIdExist = expressExist.StorageLoc?.Id ?? 0;
            storageLocExist = session.Query<StorageLocInfo>()
                .Fetch(e => e.Shelf)
                .Fetch(e => e.Spec)
                .Where(e => e.Id == storageLocIdExist)
                .Take(1)
                .SingleOrDefault();

            expressExistShelfId = storageLocExist?.Shelf?.Id;

            model.StorageLocName = storageLocExist?.GetFullName() ?? string.Empty;
        }

        ExpressCheckinUserShelfSelectInfo userSelect = expressCheckinService.GetUserShelfSelect(loginUserId);

        if ((expressExistShelfId ?? 0) != 0)
        {
            model.ShelfId = expressExistShelfId;
        }
        else
        {
            model.ShelfId = userSelect?.Shelf?.Id ?? 0;
        }

        model.ShelfList = expressCheckinService.GetAvaibleShelfList(warehouseId, loginUserId, expressExistShelfId);

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

        model.RemarkList = session.Query<ExpressRemarkPresetSelectInfo>().OrderBy(e => e.Id).ToList();

        // 物品列表
        if (expressExist != null)
        {
            int expressExistId = expressExist.Id;
            IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
                .Fetch(e => e.Goods)
                .Where(e => e.Express.Id == expressExistId)
                .OrderBy(e => e.Id)
                .ToList();

            IList<LogisticsExpressCheckinExpressGoodsModel> goodsModelList = new List<LogisticsExpressCheckinExpressGoodsModel>();

            foreach (ExpressGoodsInfo expressGoods in expressGoodsList)
            {
                LogisticsExpressCheckinExpressGoodsModel goodsModel = new LogisticsExpressCheckinExpressGoodsModel();
                goodsModel.EntryId = expressGoods.Id;
                goodsModel.UiIndex = expressGoods.Id.ToString();
                goodsModel.GoodsId = expressGoods.Goods.Id;
                goodsModel.GoodsName = expressGoods.Goods.Name;
                goodsModel.Quantity = expressGoods.Quantity;
                goodsModel.Unit = expressGoods.Unit;
                goodsModel.UnitList = GoodsUnit.GetList();

                goodsModelList.Add(goodsModel);
            }

            model.GoodsList = goodsModelList;
        }

        // 尺寸重量数据 begin

        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();

        ExpressSizeWeightInfo? expressSizeWeight = expressSizeWeightList
            .Where(e => string.IsNullOrWhiteSpace(e.TrackingNoSub))
            .Take(1)
            .SingleOrDefault();

        model.ChildSizeWeightList = new List<LogisticsExpressCheckinSizeWeightChildItem>();

        if (expressSizeWeight != null)
        {
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
            foreach (ExpressSizeWeightInfo expressChild in expressChildList)
            {
                LogisticsExpressCheckinSizeWeightChildItem childItem = new LogisticsExpressCheckinSizeWeightChildItem();

                childItem.ExpressChildId = expressChild.Id;
                childItem.ParentTrackingNo = expressTrackingNo;
                childItem.TrackingNo = expressChild.TrackingNoSub.Replace(expressTrackingNo + "-", "");
                childItem.Length = expressChild.Length;
                childItem.Width = expressChild.Width;
                childItem.Height = expressChild.Height;
                childItem.Weight = expressChild.Weight;
                childItem.PackageCount = expressChild.PackageCount;

                childItemList.Add(childItem);
            }

            model.ChildSizeWeightList = childItemList;
        }

        // 尺寸重量数据 end

        // 是不是已经预报的快递单号
        var expressExpect = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();
        model.ExpressExpect = expressExpect;

        var expressCheckinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        model.AutoRecordCheckinVideo = expressCheckinConfig.AutoRecordCheckinVideo;

        return View("ExpressCheckinExpress", model);
    }

    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/storage/loc/spec/list")]
    public IActionResult StorageLocSpecList(int shelfId)
    {
        LogisticsExpressCheckinExpressModel model = new LogisticsExpressCheckinExpressModel();

        model.StorageLocSpecList = GetShelfStorageLocSpecList(shelfId);

        return View("ExpressCheckinStorageLocSpecList", model);
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
    public IActionResult AddGoods()
    {
        LogisticsExpressCheckinExpressGoodsModel model = new LogisticsExpressCheckinExpressGoodsModel();
        model.UiIndex = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        model.UnitList = GoodsUnit.GetList();

        return View("ExpressCheckinExpressGoods", model);
    }

    [HttpPost]
    [Route("/admin/logisitcs/express/checkin/check/size/weight/data")]
    [NoLoginRequired]
    public IActionResult CheckSizeWeightData(string expressTrackingNo)
    {
        IList<ExpressSizeWeightInfo> logisticsExpressList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();

        bool hasSizeWeightData = logisticsExpressList.Count != 0;

        bool hasShortLengthData = false;
        bool hasLightWeightData = false;
        if (hasSizeWeightData) // 如果尺寸重量信息都没有，太轻的尺寸重量信息自然就没有了
        {
            foreach (ExpressSizeWeightInfo e in logisticsExpressList)
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
    [Route("/admin/express/checkin/save")]
    // 内部有事务
    public IActionResult CheckinSave(LogisticsExpressCheckinExpressModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int warehouseId = model.WarehouseId;
        WarehouseInfo? warehouse = session.Query<WarehouseInfo>()
            .Where(e => e.Id == warehouseId)
            .Take(1)
            .SingleOrDefault();

        if (warehouse == null)
        {
            throw new ValidationException("指定的仓库不存在！");
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
        UserInfo loginUser = session.Get<UserInfo>(loginUserId)!;

        // 快递单号大写与去掉前后于中间的空格，订单创建时同样也要做这样的事情
        string expressTrackingNo = model.ExpressTrackingNo.Trim().Replace(" ", "").ToUpper(); // 去空格再大写

        // 快递单号验证
        if (expressTrackingNoService.IsValidateTrackingNo(expressTrackingNo) == false)
        {
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

        if (billCustomerId != 0)
        {
            CustomerInfo billCustomer = session.Get<CustomerInfo>(billCustomerId);
            if (billCustomer == null)
            {
                throw new ValidationException("从面单客户ID中解析的客户ID不存在，请检查！");
            }
        }

        // 获取并检查尺寸重量信息是否已经存在
        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .ToList();
        if (expressSizeWeightList == null || expressSizeWeightList.Count == 0)
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

        if (shelf.Type != ShelfType.Normal)
        {
            throw new ValidationException($"指定的货架 {shelf.Name} 不是正常区快递单货架！");
        }

        ExpressExpectInfo? expressExpect = null;

        int expressId = model.ClaimId;
        bool isCreate = expressId == 0;

        lock (ExpressCheckinLockObject) // lock 时，数据库事务应该放在 lock 区域之内
        {
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();

                ExpressInfo? express = null;

                if (isCreate == false)
                {
                    express = session.Get<ExpressInfo>(expressId);
                    if (express.TrackingNo != model.ExpressTrackingNo)
                    {
                        ExpressInfo? expressOther = session.Query<ExpressInfo>()
                            .Where(e => e.TrackingNo == expressTrackingNo)
                            .Take(1)
                            .FirstOrDefault();
                        if (expressOther != null)
                        {
                            throw new ValidationException("该快递单号存在其他入库记录！"); // 一般发生在入库保存后，马上修改快递单号的情形
                        }
                    }
                }
                else
                {
                    express = session.Query<ExpressInfo>()
                        .Where(e => e.TrackingNo == expressTrackingNo)
                        .Take(1)
                        .FirstOrDefault();

                    if (express != null && express.Id != expressId)
                    {
                        throw new ValidationException("该快递单号存在其他入库记录！");
                    }
                }

                /*
                if (express != null && (express.Picked == true || ExpressStatus.IsCheckedIn(express.Status)))
                {
                    throw new ValidationException("该快递单号已被会员认领，不能修改更新记录信息！");
                }
                */

                if (express != null && ExpressStatus.IsOrderCreate(express.Status))
                {
                    throw new ValidationException("该快递单号已提交订单，不能修改更新记录信息！");
                }

                if (express != null && (express.Status == ExpressStatus.WaitReturn))
                {
                    throw new ValidationException("该快递单号当前申请退运中，不能修改更新记录信息！");
                }

                /*
                if (express != null && (express.WarehouseProcessed == true || express.Status == ExpressStatus.Order))
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

                IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => goodsIdList.Contains(e.Id)).ToList();
                IList<LogisticsGoodsType> goodsTypeList = goodsList.Select(e => e.Type).Distinct().ToList();

                LogisticsGoodsType goodsType = logisticsGoodsTypeService.GetGoodsTypeFromGoodsTypeList(goodsTypeList);

                // 如果已经在 快递单号预报 里面的，就设置 快递单号预报为 录货值 状态
                expressExpect = session.Query<ExpressExpectInfo>()
                    .Where(e => e.TrackingNo == expressTrackingNo)
                    .Take(1)
                    .SingleOrDefault();

                if (expressExpect != null && expressExpect.Status == ExpressExpectStatus.Create)
                {
                    expressExpect.Status = ExpressExpectStatus.Enter;
                    session.Save(expressExpect);
                    session.Flush();
                }

                if (express == null)
                {
                    express = new ExpressInfo();
                    express.TrackingNo = expressTrackingNo;
                    express.CheckinTrackingNo = string.Empty; // 后面会回填这个澳拼跟踪号
                    express.Status = ExpressStatus.WaitPick; // 初始化为 WaitPick     
                    express.ShelfStatus = ExpressShelfStatus.ToOnShelf; // 初始化为 待上架
                    express.AddTime = DateTime.Now;
                }

                IList<string> editDescList = new List<string>();

                // 仓库
                if (isCreate == false && (express.Warehouse?.Id ?? 0) != warehouseId)
                {
                    editDescList.Add($"仓库:{express.Warehouse?.Name}=>{warehouse.Name}");
                }

                express.Warehouse = warehouse;

                // 快递公司
                if (isCreate == false && express.ExpressCompany != model.ExpressCompany)
                {
                    editDescList.Add($"快递公司:{express.ExpressCompany}=>{model.ExpressCompany}");
                }

                express.ExpressCompany = model.ExpressCompany;

                // 物品类型
                if (isCreate == false && express.GoodsType != goodsType)
                {
                    editDescList.Add($"物品类型:{express.GoodsType.Name}=>{goodsType.Name}");
                }

                express.GoodsType = goodsType;

                // 添加用户
                if (isCreate == false && express.AddUser.Id != loginUserId)
                {
                    UserInfo oldAddUser = express.AddUser;
                    editDescList.Add($"添加用户:{oldAddUser.UserName}=>{loginUser.UserName}");
                }

                express.AddUser = loginUser;

                // 包裹数量
                int prePackageCount = express.PackageCount; // 如果是新建，不是修改保存的话，这个值就是0
                int packageCount = expressSizeWeightList.Sum(e => e.PackageCount);

                if (packageCount <= 0)
                {
                    packageCount = 1;
                }

                if (isCreate == false && prePackageCount != packageCount)
                {
                    editDescList.Add($"包裹数量:{prePackageCount}=>{packageCount}");
                }

                express.PackageCount = packageCount;

                // 入库备注 日志，后面 express 保存完后，保存到 express remark 表

                string checkinRemark = model.Description?.Trim() ?? string.Empty;
                ExpressRemarkInfo? checkinRemarkInDb = null;
                string oldCheckinRemark = string.Empty;
                if (isCreate == false)
                {
                    int expId = express.Id;
                    checkinRemarkInDb = session.Query<ExpressRemarkInfo>()
                        .Where(e => e.Express!.Id == expId && e.Type == ExpressRemarkType.Warehouse)
                        .Take(1)
                        .SingleOrDefault();
                    oldCheckinRemark = checkinRemarkInDb?.Remark ?? string.Empty;
                    if (checkinRemark != oldCheckinRemark)
                    {
                        editDescList.Add($"入库备注:{oldCheckinRemark}=>{checkinRemark}");
                    }
                }

                // 面单客户

                if (isCreate == false && express.BillCustomerCode != model.BillCustomerCode)
                {
                    editDescList.Add($"面单客户:{express.BillCustomerCode}=>{model.BillCustomerCode}");
                }

                express.BillCustomerCode = model.BillCustomerCode;
                express.BillCustomerId = billCustomerId;

                // 如果是新入库的，或者是 原来的但修改了货架, 或者是修改了仓位规格
                if (isCreate == true || ( /* isCreate == false && */ (express.StorageLoc?.Shelf?.Id ?? 0) != model.ShelfId || (express.StorageLoc?.Spec?.Id ?? 0) != model.StorageLocSpecId))
                {
                    StorageLocInfo recommandStorage = expressCheckinService.GetRecommandStorage(shelf, storageLocSpec); // 从这里取到的仓位ID，已经保证是仓位上物品不超过限制了
                    if (recommandStorage == null)
                    {
                        throw new ValidationException("未能获取推荐仓位，请尝试重新选择货架或仓位规格！");
                    }

                    // 检查仓位和货架是否可用
                    storageLocService.CheckStorageLoc((recommandStorage.Id), ShelfType.Normal);

                    express.StorageLoc = recommandStorage;
                }

                session.SaveOrUpdate(express);
                session.Flush();

                expressId = express.Id;
                if (expressId == 0)
                {
                    throw new Exception("快递单号没能保存成功！");
                }

                // 保存客户备注
                if (isCreate) // 新增是才需要这个客户备注迁移
                {
                    if (expressExpect != null
                        && string.IsNullOrWhiteSpace(expressExpect.Description) == false) // 预报里面的描述非空
                    {
                        // 这时肯定是新增

                        session.Save(new ExpressRemarkInfo() {
                            Express = express,
                            Type = ExpressRemarkType.Customer,
                            AddUser = loginUser,
                            Remark = expressExpect.Description.Trim(),
                        });
                        session.Flush();
                    }
                }

                // 保存入库备注
                if (checkinRemarkInDb == null)
                {
                    checkinRemarkInDb = new ExpressRemarkInfo() {
                        Express = express,
                        Type = ExpressRemarkType.Warehouse,
                        AddUser = loginUser,
                    };
                }

                checkinRemarkInDb.Remark = checkinRemark;

                if (isCreate == false && checkinRemark != oldCheckinRemark)
                {
                    // 编辑时如果入库备注跟原来的不同，就更新添加用户
                    checkinRemarkInDb.AddUser = loginUser;
                }

                session.SaveOrUpdate(checkinRemarkInDb);
                session.Flush();


                // 删除被移除的物品
                IList<ExpressGoodsInfo> expressGoodsList = new List<ExpressGoodsInfo>();
                if (isCreate == false)
                {
                    expressGoodsList = session.Query<ExpressGoodsInfo>()
                        .Where(e => e.Express!.Id == expressId)
                        .ToList();

                    // 移除界面上没有，但数据库中有的物品
                    IList<int> entryIdListFromUi = goodsModelList.Select(e => e.EntryId).ToList();
                    IList<ExpressGoodsInfo> expressGoodsToDelete = expressGoodsList
                        .Where(e => entryIdListFromUi.Contains(e.Id) == false)
                        .ToList();

                    foreach (ExpressGoodsInfo ce in expressGoodsToDelete)
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
                IList<ExpressGoodsInfo> expressEntryList = new List<ExpressGoodsInfo>();

                // 保存物品
                foreach (LogisticsExpressCheckinExpressGoodsModel goodsItem in goodsModelList)
                {
                    ExpressGoodsInfo expressEntry = null;
                    if (goodsItem.EntryId != 0)
                    {
                        expressEntry = expressGoodsList.Where(e => e.Id == goodsItem.EntryId).Take(1).SingleOrDefault();
                    }

                    if (expressEntry == null)
                    {
                        expressEntry = new ExpressGoodsInfo();
                    }

                    expressEntry.ClaimId = expressId;

                    if (isCreate == false &&
                        (expressEntry.GoodsId != goodsItem.GoodsId
                         || expressEntry.Quantity != goodsItem.Quantity
                         || expressEntry.Unit != goodsItem.Unit))
                    {
                        LogisticsGoodsInfo goodsEditAfter = session.Get<LogisticsGoodsInfo>(goodsItem.GoodsId);
                        if (expressEntry.Id != 0) // 这个时候是修改(isCreate == false)，编辑物品
                        {
                            LogisticsGoodsInfo goodsEditBefore = session.Get<LogisticsGoodsInfo>(expressEntry.GoodsId);

                            editDescList.Add($"修改物品: {goodsEditBefore?.Name}{expressEntry?.Quantity}{expressEntry?.Unit}=>{goodsEditAfter?.Name}{goodsItem.Quantity}{goodsItem.Unit}");
                        }
                        else // 这个时候是修改(isCreate == false)，添加物品
                        {
                            editDescList.Add($"添加物品: {goodsEditAfter?.Name}{goodsItem.Quantity}{goodsItem.Unit}");
                        }
                    }

                    expressEntry.GoodsId = goodsItem.GoodsId;
                    expressEntry.Quantity = goodsItem.Quantity;
                    expressEntry.Unit = goodsItem.Unit;

                    session.SaveOrUpdate(expressEntry);

                    expressEntryList.Add(expressEntry);
                }

                session.Flush();

                /*
                // 3.0 快递单入库的时候，如果快递单已经创建订单（可能是2.0的界面创建的），就把入库的认领件设置成已认领的，并把认领件的仓位设置到订单快递单号仓位(这时一般还没仓位)上
                if (orderExpress != null)
                {
                    StorageLocInfo storageLoc = express.StorageLoc;

                    int orderExpressId = orderExpress.Id;
                    session.Query<LogisticsOrderGoodsInfo>()
                        .Where(e => e.Express.Id == orderExpressId)
                        .UpdateBuilder()
                        .Set(e => e.StorageLoc, storageLoc)
                        .Set(e => e.UpdateTime, DateTime.Now)
                        .Update();
                }
                */

                StorageLocInfo storageLoc = express.StorageLoc;
                if (isCreate)
                {
                    string logDesc = storageLoc?.GetFullName() ?? string.Empty;
                    if (expressExpect != null)
                    {
                        logDesc += $"，面单客户按预报客户：{expressExpect.Customer?.Id ?? 0}";
                    }

                    expressLogService.UserClaimLog(express.Id, ExpressLogType.Checkin, logDesc, loginUserId);
                }
                else
                {
                    string logDesc = (storageLoc?.GetFullName() ?? string.Empty) + " " + string.Join(";", editDescList);
                    if (expressExpect != null)
                    {
                        logDesc += $"，面单客户按预报客户：{expressExpect.Customer?.Id ?? 0}";
                    }

                    expressLogService.UserClaimLog(express.Id, ExpressLogType.CheckinEdit, logDesc, loginUserId);
                }

                expressSizeWeightService.SumDataAndUpdateExpress(express.Id);

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
                    // todo 加上 customerMessageService.SendMessageYuBaoHuoZhi(expressExpect.Customer, expressExpect.TrackingNo);
                }
            }
            else if (billCustomerId != 0) // 没预报但有面单会员ID
            {
                CustomerInfo billCustomer = session.Get<CustomerInfo>(billCustomerId);
                if (billCustomer != null)
                {
                    // todo 加上 customerMessageService.SendMessageWoDeShenLingJian(billCustomer);
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
    [Route("/admin/express/checkin/search/goods/list/data")]
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

        foreach (LogisticsGoodsInfo goods in goodsList)
        {
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
    private ICriteria GetSearchGoodsCriteria(int warehouseId, string param)
    {
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
    public IActionResult GetExpressComName(string expressTrackingNo)
    {
        string comName = expressCompanyService.GetExpressCompanyName(expressTrackingNo);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "content", comName }
        });
    }

    [HttpPost]
    [Route("/admin/express/checkin/express/get/print/data/by/id")]
    [NoLoginRequired]
    public IActionResult GetPrintDataById(int expressId)
    {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
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
        string description = express.Description;
        string checkinPackageCount = express.PackageCount.ToString();
        string userName = express.AddUser.UserName;
        string checkinTime = express.AddTime.ToString("MM-dd HH:mm");
        string goodsTypeShortName = express.GoodsType.CheckinLabelShortName;

        /*
         这里不需要这些逻辑
        var aoPinSubTrackingNoList = checkinTrackingNoService.ListSubTrackingNo(express.Id);

        // 如果还没有澳拼单号的，就生成。有些历史入库的快递单号的，还没有澳拼入库单号。这个逻辑主要用在历史入库快递单，补打标签的时候
        if (aoPinSubTrackingNoList == null || aoPinSubTrackingNoList.Count == 0)
        {
            string mainTrackingNo = checkinTrackingNoService.Generate(express.Id);
            express.AoPinTrackingNo = mainTrackingNo;
            session.Update(express);
            session.Flush();

            aoPinSubTrackingNoList = checkinTrackingNoService.ListSubTrackingNo(express.Id);
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
        */

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取数据成功！" },
            { "expressTrackingNo", express.TrackingNo },
            { "checkinTrackingNo", express.CheckinTrackingNo },
            { "customerCode", customerCode },
            { "warehouseNote", description },
            { "description", description },
            { "checkinPackageCount", checkinPackageCount },
            { "orderCode", orderCode },
            { "storageLoc", storageLocName },
            { "loginName", userName }, // todo a 这里的 json名称可以改掉
            { "checkinTime", checkinTime },
            { "goodsTypeName", goodsTypeShortName },
        });
    }

    [HttpPost]
    [Route("/admin/express/checkin/express/get/print/data")]
    [NoLoginRequired]
    public IActionResult GetPrintData(string expressTrackingNo)
    {
        ExpressInfo express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null)
        {
            throw new ValidationException($"指定的快递单号{expressTrackingNo}不存在");
        }

        return GetPrintDataById(express.Id);
    }

    [HttpPost]
    [Route("/admin/express/checkin/express/get/print/data/manual")]
    [NoLoginRequired]
    public IActionResult GetPrintDataManual(string expressTrackingNo)
    {
        ExpressInfo express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null)
        {
            throw new ValidationException($"指定的快递单号{expressTrackingNo}不存在");
        }


        var printData = GetPrintDataById(express.Id);

        int loginUserId = loginUserService.GetLoginUserId();
        expressLogService.UserClaimLog(express.Id, ExpressLogType.ManualPrintLabel, loginUserId);

        return printData;
    }

    #region size weight

    [HttpPost]
    [Route("/admin/express/checkin/save/express/size/weight")]
    [Transactional]
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
            var express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == expressTrackingNo)
                .Take(1)
                .SingleOrDefault();

            if (express != null)
            {
                // 如果子单号有删除，并且是已入库的快递单的，就重新生成澳拼子单号
                if (shouldReGenerateAoPinTrackingNo)
                {
                    // 这里是重新生成子单号
                    var mainTrackingNo = checkinTrackingNoService.Generate(express.Id);
                    express.AoPinTrackingNo = mainTrackingNo; // 设置主单号
                }

                int packageCount = (logisticsExpress.PackageCount ?? 1) + childSizeWeightList.Sum(e => (e.PackageCount ?? 1));
                if (packageCount <= 0)
                {
                    packageCount = 1;
                }

                express.ExpressPackageCount = packageCount;

                session.Update(express);

                session.Flush();

                expressSizeWeightService.SumDataAndUpdateExpress(express.Id);
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
    public IActionResult NewExpressSizeWeightRow(string expressTrackingNo)
    {
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