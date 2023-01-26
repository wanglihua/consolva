using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Areas.Logistics.Models.LogisticsMode;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsModeList)]
public class LogisticsModeController : Controller {
    private readonly ISession session;
    private readonly WarehouseService warehouseService;

    // private LogisticsModeService logisticsModeService = null; todo 后面补上

    public LogisticsModeController(ISession session, WarehouseService warehouseService /*, LogisticsModeService logisticsModeService*/) {
        this.session = session;
        this.warehouseService = warehouseService;
        // this.logisticsModeService = logisticsModeService;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/{countryId:int}")]
    public IActionResult Index(int countryId) {
        var model = new LogisticsModeIndexModel();
        model.Country = Country.GetById(countryId);

        model.CountryList = Country.GetNoChinaList();
        model.LogisticsModeTypeList = LogisticsModeType.GetList();
        model.StatusList = LogisticsModeStatus.GetList();

        return View("LogisticsModeIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/mode/list")]
    public IActionResult List(LogisticsModeListQueryParam queryParam) {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsModeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsModeInfo>();

        IList<int> modeIdList = logisticsModeList.Select(e => e.Id).ToList();
        IList<LogisticsModeFeeRateInfo> modeFeeRateList = session.Query<LogisticsModeFeeRateInfo>()
            .Fetch(e => e.FeeRate)
            .Where(e => modeIdList.Contains(e.Mode.Id))
            .ToList();

        foreach (var mode in logisticsModeList) {
            var modeId = mode.Id;
            mode.ModeFeeRateListCache = modeFeeRateList
                .Where(e => e.Mode.Id == modeId)
                .OrderBy(e => e.GoodsType.Id) // 显示时按物品类型排序
                .ToList();
        }

        var model = new LogisticsModeListModel(listPager);
        model.LogisticsModeList = logisticsModeList;
        model.Country = queryParam.Country;

        return View("LogisticsModeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsModeListQueryParam queryParam) {
        var criteria = session.CreateCriteria<LogisticsModeInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsModeInfo.Warehouse), nameof(LogisticsModeInfo.Warehouse));

        criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Cate == LogisticsModeCate.Customer));

        criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Country == queryParam.Country));

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != LogisticsModeType.None) {
            var typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Type == typeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CodeQuery) == false) {
            var codeQuery = queryParam.CodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsModeInfo>(e => e.Code).IsLike(codeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false) {
            var nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsModeInfo>(e => e.Name).IsLike(nameQuery, MatchMode.Anywhere));
        }


        if (queryParam.StatusQuery != null && queryParam.StatusQuery != LogisticsModeStatus.None) {
            var statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Status == statusQuery));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/detail/{id:int}/{countryId:int?}")]
    public IActionResult Detail(int id, int? countryId) {
        session.DefaultReadOnly = true;

        var typeList = LogisticsModeType.GetList();

        var model = new LogisticsModeDetailModel();

        LogisticsModeInfo? logisticsMode;
        if (id == 0) {
            logisticsMode = new LogisticsModeInfo();

            if (countryId.HasValue) {
                logisticsMode.Country = Country.GetById(countryId.Value);
            } else {
                logisticsMode.Country = Country.GetById(Country.GetNoChinaList().First().Id);
            }

            logisticsMode.Channel = LogisticsModeChannel.Other; // 新建时在界面上优先显示其他

            // 在界面上保证 RepackTypeIdList 不为 null
            model.RepackTypeIdList = new List<int>();
            model.GoodsTypeIdList = LogisticsGoodsType.GetCustomerList().Select(e => e.Id).ToList();

            model.ModeFeeRateList = new List<LogisticsModeFeeRateInfo>(); // 保证列表非空
        } else {
            logisticsMode = session.Get<LogisticsModeInfo>(id);
            if (logisticsMode == null) {
                throw new ValidationException("指定的数据项不存在！");
            }

            IList<LogisticsModeRepackTypeInfo> repackTypeList = session
                .Query<LogisticsModeRepackTypeInfo>()
                .Where(e => e.LogisticsMode.Id == id)
                .ToList();

            model.RepackTypeIdList = repackTypeList.Select(e => e.RepackType.Id).ToList();

            IList<LogisticsModeGoodsTypeInfo> logisticsModeGoodsTypeList = session
                .Query<LogisticsModeGoodsTypeInfo>()
                .Where(e => e.LogisticsMode.Id == id)
                .ToList();
            model.GoodsTypeIdList = logisticsModeGoodsTypeList.Select(e => e.GoodsType.Id).ToList();

            // 物流方式关联通用费率
            IList<LogisticsModeFeeRateInfo> modeFeeRateList = session.Query<LogisticsModeFeeRateInfo>()
                .Fetch(e => e.FeeRate)
                .Where(e => e.Mode.Id == id)
                .ToList();

            model.ModeFeeRateList = modeFeeRateList;
        }

        model.Id = logisticsMode.Id;
        model.Country = logisticsMode.Country;
        model.Type = logisticsMode.Type;
        model.PackSizeType = logisticsMode.PackSizeType;
        model.Code = logisticsMode.Code;
        model.Name = logisticsMode.Name;
        model.Description = logisticsMode.Description;

        model.Channel = logisticsMode.Channel;
        model.ExpressCompanyName = logisticsMode.ExpressCompanyName;
        model.ExpressCompanyUrl = logisticsMode.ExpressCompanyUrl;

        model.Status = logisticsMode.Status;
        model.InnerStatus = logisticsMode.InnerStatus;

        model.FeeCalDesc = logisticsMode.FeeCalDesc;

        if (logisticsMode.Warehouse != null) {
            model.WarehouseId = logisticsMode.Warehouse.Id;
        }

        model.CountryList = Country.GetNoChinaList();
        model.LogisticsModeTypeList = typeList;
        model.PackSizeTypeList = LogisticsModePackSizeType.GetList();

        var feeRateEnabledStatus = LogisticsFeeRateStatus.Enabled;
        model.LogisticsFeeRateList = session.Query<LogisticsFeeRateInfo>().Where(e => e.Status == feeRateEnabledStatus).OrderBy(e => e.Id).ToList();
        model.WarehouseList = warehouseService.GetWarehouseListForAdmin();
        model.StatusList = LogisticsModeStatus.GetList();
        model.RepackTypeList = LogisticsRepackType.GetList();
        model.GoodsTypeList = LogisticsGoodsType.GetCustomerList();
        model.ChannelList = LogisticsModeChannel.GetList();

        model.InnerModeId = logisticsMode.InnerModeId;
        var modeType = logisticsMode.Type;
        model.InnerModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Cate == LogisticsModeCate.Inner
                        && e.Type == modeType
                        && e.Status == LogisticsModeStatus.Enabled)
            .OrderBy(e => e.Name)
            .ToList();

        return View("LogisticsModeDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/mode/save")]
    [Transactional]
    public IActionResult Save(LogisticsModeDetailModel model) {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var repackTypeIdList = model.RepackTypeIdList;
        if (repackTypeIdList == null) repackTypeIdList = new List<int>();

        var goodsTypeIdList = model.GoodsTypeIdList;
        if (goodsTypeIdList == null) goodsTypeIdList = new List<int>();

        var goodsTypeListAll = LogisticsGoodsType.GetCustomerList();
        IList<int> feeRateIdList = new List<int>();
        IDictionary<int, int> goodsTypeFeeRateIdMap = new Dictionary<int, int>();
        foreach (var goodsType in goodsTypeListAll)
            if (Request.Form.TryGetValue(model.GetFeeRateFieldName(goodsType), out var feeRateIdStrs))
                if (feeRateIdStrs.Count > 0 && string.IsNullOrWhiteSpace(feeRateIdStrs[0]) == false)
                    if (int.TryParse(feeRateIdStrs[0], out var feeRateId)) {
                        feeRateIdList.Add(feeRateId);
                        goodsTypeFeeRateIdMap[goodsType.Id] = feeRateId;
                    }

        foreach (var goodsTypeId in goodsTypeIdList)
            if (goodsTypeFeeRateIdMap.TryGetValue(goodsTypeId, out var feeRateId) == false)
                throw new ValidationException($"物流方式选择了物品类型{LogisticsGoodsType.GetById(goodsTypeId).Name}，但没有设置对应的通用费率！");

        IList<LogisticsFeeRateInfo> feeRateListFromUi = session.Query<LogisticsFeeRateInfo>()
            .Where(e => feeRateIdList.Contains(e.Id))
            .ToList();
        foreach (var feeRateId in feeRateIdList) {
            var feeRate = feeRateListFromUi
                .Where(e => e.Id == feeRateId)
                .Take(1).SingleOrDefault();
            if (feeRate == null) // 一般情况下是不会发生的
                throw new ValidationException($"指定的通用费率#{feeRateId}不存在！");
        }

        // if (string.IsNullOrWhiteSpace(model.ExpressCompanyName))
        // {
        //     throw new ValidationException("快递单公司名称不能为空！");
        // }
        //
        // if (string.IsNullOrWhiteSpace(model.ExpressCompanyUrl))
        // {
        //     throw new ValidationException("快递公司网址不能为空！");
        // }

        var logisticsModeId = model.Id;

        var isCreate = logisticsModeId == 0;

        LogisticsModeInfo? logisticsMode;

        if (isCreate)
            logisticsMode = new LogisticsModeInfo();
        else
            logisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);

        logisticsMode.Country = model.Country;
        logisticsMode.Type = model.Type;

        if (model.PackSizeType == null) model.PackSizeType = LogisticsModePackSizeType.None;

        logisticsMode.PackSizeType = model.PackSizeType;

        logisticsMode.Code = model.Code;
        logisticsMode.Name = model.Name;
        logisticsMode.Description = model.Description;

        logisticsMode.Channel = model.Channel;
        logisticsMode.ExpressCompanyName = model.ExpressCompanyName ?? string.Empty;
        logisticsMode.ExpressCompanyUrl = model.ExpressCompanyUrl ?? string.Empty;

        logisticsMode.Status = model.Status;
        logisticsMode.InnerStatus = model.InnerStatus;

        logisticsMode.FeeCalDesc = model.FeeCalDesc;

        if (model.WarehouseId == 0) throw new ValidationException("收货仓库不能为空！");

        var warehouse = session.Get<WarehouseInfo>(model.WarehouseId);
        if (warehouse == null) throw new ValidationException($"指定的收货仓库#{model.WarehouseId}不存在！");

        logisticsMode.Warehouse = warehouse;

        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        logisticsMode.InnerModeId = model.InnerModeId;

        if (isCreate)
            session.Save(logisticsMode);
        else
            session.Update(logisticsMode);

        session.Flush();

        var modeId = logisticsMode.Id;

        #region 打包方式

        // 保存打包方式

        IList<LogisticsRepackType> repackTypeListInDb = session
            .Query<LogisticsModeRepackTypeInfo>()
            .Where(e => e.LogisticsMode.Id == modeId)
            .Select(e => e.RepackType)
            .ToList();

        // 界面上有数据库里面没有的添加保存
        foreach (var repackTypeId in repackTypeIdList) {
            var repackType = LogisticsRepackType.GetById(repackTypeId);

            if (repackTypeListInDb.Contains(repackType) == false) {
                var logisticsModeRepackTypeInfo = new LogisticsModeRepackTypeInfo();
                logisticsModeRepackTypeInfo.LogisticsMode = logisticsMode;
                logisticsModeRepackTypeInfo.RepackType = repackType;

                session.Save(logisticsModeRepackTypeInfo);
            }
        }

        session.Flush();

        // 数据库里面有界面上没有的删除
        foreach (var repackType in repackTypeListInDb) {
            if (repackTypeIdList.Contains(repackType.Id) == false) {
                var modeRepackList = session.Query<LogisticsModeRepackTypeInfo>()
                    .Where(e => e.LogisticsMode.Id == modeId
                                && e.RepackType == repackType)
                    .ToList();
                foreach (var modeRepack in modeRepackList) {
                    session.Delete(modeRepack);
                }

                session.Flush();
            }
        }

        session.Flush();

        #endregion

        #region 物品类型

        IList<LogisticsGoodsType> goodsTypeListInDb = session
            .Query<LogisticsModeGoodsTypeInfo>()
            .Where(e => e.LogisticsMode.Id == modeId)
            .Select(e => e.GoodsType)
            .ToList();

        // 物品类型关联 界面上有数据库里面没有的添加保存
        foreach (var goodsTypeId in goodsTypeIdList) {
            var goodsType = LogisticsGoodsType.GetById(goodsTypeId);

            if (goodsTypeListInDb.Contains(goodsType) == false) {
                var logisticsModeGoodsType = new LogisticsModeGoodsTypeInfo();
                logisticsModeGoodsType.LogisticsMode = logisticsMode;
                logisticsModeGoodsType.GoodsType = goodsType;

                session.Save(logisticsModeGoodsType);
            }
        }

        // 物品类型关联 数据库里面有界面上没有的删除
        foreach (var goodsType in goodsTypeListInDb) {
            if (goodsTypeIdList.Contains(goodsType.Id) == false) {
                var modeGoodsTypeList = session.Query<LogisticsModeGoodsTypeInfo>()
                    .Where(e => e.LogisticsMode.Id == modeId && e.GoodsType == goodsType)
                    .ToList();
                foreach (var modeGoodsType in modeGoodsTypeList) {
                    session.Delete(modeGoodsType);
                }

                session.Flush();
            }
        }

        session.Flush();

        #endregion

        // 保存通用费率
        IList<LogisticsModeFeeRateInfo> modeFeeRateListInDb = session.Query<LogisticsModeFeeRateInfo>()
            .Where(e => e.Mode.Id == modeId)
            .ToList();
        foreach (var goodsType in goodsTypeListAll) {
            var modeFeeRateInDb = modeFeeRateListInDb
                .Where(e => e.GoodsType == goodsType)
                .Take(1)
                .SingleOrDefault();

            if (goodsTypeFeeRateIdMap.TryGetValue(goodsType.Id, out var feeRateId)) {
                if (modeFeeRateInDb == null) // 数据库里面没有，但界面上有
                {
                    modeFeeRateInDb = new LogisticsModeFeeRateInfo();
                    modeFeeRateInDb.Mode = logisticsMode;
                    modeFeeRateInDb.GoodsType = goodsType;
                    modeFeeRateInDb.FeeRate = feeRateListFromUi.Where(e => e.Id == feeRateId).Take(1).Single(); // 前面验证过了，肯定存在

                    session.Save(modeFeeRateInDb);
                }
            } else {
                if (modeFeeRateInDb != null) // 数据里有，但界面上没有
                    session.Delete(modeFeeRateInDb);
            }
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", logisticsMode.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/mode/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (var modeId in idList) {
            var logisticsMode = session.Get<LogisticsModeInfo>(modeId);

            if (logisticsMode == null) throw new ValidationException($"指定的物流方式#{modeId}不存在！");

            // 先删除关联的费率数据
            if (logisticsMode.Type == LogisticsModeType.Express) {
                var feeRateExpressList = session.Query<LogisticsFeeRateExpressInfo>()
                    .Where(e => e.LogisticsMode.Id == modeId)
                    .ToList();
                foreach (var feeRateExpres in feeRateExpressList) {
                    session.Delete(feeRateExpres);
                }

                session.Flush();
            }

            session.Delete(logisticsMode);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}