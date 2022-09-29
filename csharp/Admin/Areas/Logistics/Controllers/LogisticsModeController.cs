using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsModeList)]
public class LogisticsModeController : Controller
{
    private ISession session;
    // private LogisticsModeService logisticsModeService = null; todo 后面补上

    public LogisticsModeController(ISession session /*, LogisticsModeService logisticsModeService*/)
    {
        this.session = session;
        // this.logisticsModeService = logisticsModeService;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/{typeId:int}")]
    public IActionResult Index(int typeId)
    {
        LogisticsModeIndexModel model = new LogisticsModeIndexModel();
        model.TypeId = typeId;
        model.Type = LogisticsModeType.GetById(typeId);

        model.LogisticsModeTypeList = LogisticsModeType.GetListForLogisticsModePageDropdown();
        model.StatusList = LogisticsModeStatus.GetList();

        return View("LogisticsModeIndex", model);
    }

    [HttpPost]
    [NoHibernateAutoTransaction]
    [Route("/admin/logistics/mode/list")]
    public IActionResult List(LogisticsModeListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsModeInfo> logisticsModeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Model.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsModeInfo>();

        IList<int> logisticsModeIdList = logisticsModeList.Select(e => e.Id).ToList();
        IList<LogisticsModeFirstSecondLinkInfo> firstSecondLinkList = session.Query<LogisticsModeFirstSecondLinkInfo>()
            .Where(e => logisticsModeIdList.Contains(e.FirstLogisticsMode.Id))
            .ToList();
        foreach (var logisticsMode in logisticsModeList)
        {
            logisticsMode.LinkedSecondTypesCache = string.Join(", ", firstSecondLinkList
                .Where(e => e.FirstLogisticsMode.Id == logisticsMode.Id)
                .OrderBy(e => e.SecondLogisticsType.Id)
                .Select(e => e.SecondLogisticsType)
                .Select(e => e.Name));
        }

        LogisticsModeListModel model = new LogisticsModeListModel(listPager);
        model.LogisticsModeList = logisticsModeList;
        model.ModeType = LogisticsModeType.GetById(queryParam.TypeIdQuery);

        return View("LogisticsModeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsModeListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsModeInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsModeInfo.Warehouse), nameof(LogisticsModeInfo.Warehouse));
        criteria = criteria.CreateAlias(nameof(LogisticsModeInfo.CommonFeeRate), nameof(LogisticsModeInfo.CommonFeeRate), JoinType.LeftOuterJoin);
        criteria = criteria.CreateAlias(nameof(LogisticsModeInfo.SpecialFeeRate), nameof(LogisticsModeInfo.SpecialFeeRate), JoinType.LeftOuterJoin);

        criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Cate == LogisticsModeCate.Customer));

        int typeIdQuery = queryParam.TypeIdQuery;
        if (typeIdQuery != 0)
        {
            var typeQuery = LogisticsModeType.GetById(typeIdQuery);
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Type == typeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CodeQuery) == false)
        {
            string codeQuery = queryParam.CodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsModeInfo>(e => e.Code).IsLike(codeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsModeInfo>(e => e.Name).IsLike(nameQuery, MatchMode.Anywhere));
        }

        LogisticsModeStatus statusQuery = queryParam.StatusQuery;
        if (statusQuery != null && statusQuery != LogisticsModeStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Status == statusQuery));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/detail/{id:int}/{typeId:int?}")]
    [NoHibernateAutoTransaction]
    public IActionResult Detail(int id, int? typeId)
    {
        session.DefaultReadOnly = true;

        IList<LogisticsModeType> typeList = LogisticsModeType.GetListForLogisticsModePageDropdown();

        LogisticsModeDetailModel model = new LogisticsModeDetailModel();

        LogisticsModeInfo logisticsMode = null;
        if (id == 0)
        {
            logisticsMode = new LogisticsModeInfo();

            if (typeId.HasValue)
            {
                logisticsMode.Type = LogisticsModeType.GetById(typeId.Value);
            }
            else
            {
                logisticsMode.Type = LogisticsModeType.GetById(typeList.First().Id);
            }

            // 在界面上保证 RepackTypeIdList 不为 null
            model.RepackTypeIdList = new List<int>();
            model.SecondLogisticModeTypeIdList = LogisticsModeType.GetCnAuSecondTypeList().Select(e => e.Id).ToList();
            model.GoodsTypeIdList = LogisticsGoodsType.GetCustomerList().Select(e => e.Id).ToList();
        }
        else
        {
            logisticsMode = session.Get<LogisticsModeInfo>(id);
            if (logisticsMode == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            IList<LogisticsModeRepackTypeInfo> repackTypeList = session
                .Query<LogisticsModeRepackTypeInfo>()
                .Where(e => e.LogisticsModeId == id)
                .ToList();

            model.RepackTypeIdList = repackTypeList.Select(e => e.RepackTypeId).ToList();

            IList<LogisticsModeFirstSecondLinkInfo> logisticsModeFirstSecondLinkList = session
                .Query<LogisticsModeFirstSecondLinkInfo>()
                .Where(e => e.FirstLogisticsMode.Id == id)
                .ToList();
            model.SecondLogisticModeTypeIdList = logisticsModeFirstSecondLinkList.Select(e => e.SecondLogisticsType.Id).ToList();

            IList<LogisticsModeGoodsTypeInfo> logisticsModeGoodsTypeList = session
                .Query<LogisticsModeGoodsTypeInfo>()
                .Where(e => e.LogisticsMode.Id == id)
                .ToList();
            model.GoodsTypeIdList = logisticsModeGoodsTypeList.Select(e => e.GoodsType.Id).ToList();
        }

        model.Id = logisticsMode.Id;
        model.Type = logisticsMode.Type;
        model.PackSizeType = logisticsMode.PackSizeType;
        model.Code = logisticsMode.Code;
        model.Name = logisticsMode.Name;
        model.Description = logisticsMode.Description;
        model.CompanyName = logisticsMode.CompanyName;
        model.IconFileName = logisticsMode.IconFileName;

        model.MinWeight = logisticsMode.MinWeight;
        model.MaxWeight = logisticsMode.MaxWeight;

        model.SyncTla = logisticsMode.SyncTla;
        if (logisticsMode.PackPrintType == null)
        {
            model.PackPrintTypeId = null;
        }
        else
        {
            model.PackPrintTypeId = logisticsMode.PackPrintType?.Id ?? 0;
        }

        model.EnglishPackPrint = logisticsMode.EnglishPackPrint;
        model.MultiExpressOnePack = logisticsMode.MultiExpressOnePack;

        model.ExpressCompanyName = logisticsMode.ExpressCompanyName;
        model.ExpressCompanyUrl = logisticsMode.ExpressCompanyUrl;

        model.FreeDeliveryDistance = logisticsMode.FreeDeliveryDistance;

        if (logisticsMode.PickupStation != null)
        {
            model.PickupStationId = logisticsMode.PickupStation.Id;
        }

        model.Status = logisticsMode.Status;
        model.InnerStatus = logisticsMode.InnerStatus;

        if (logisticsMode.FeeRate != null)
        {
            model.FeeRateId = logisticsMode.FeeRate.Id;
        }

        if (logisticsMode.CommonFeeRate != null)
        {
            model.CommonFeeRateId = logisticsMode.CommonFeeRate.Id;
        }

        if (logisticsMode.RestrictFeeRate != null)
        {
            model.RestrictFeeRateId = logisticsMode.RestrictFeeRate.Id;
        }

        if (logisticsMode.BambooFeeRate != null)
        {
            model.BambooFeeRateId = logisticsMode.BambooFeeRate.Id;
        }

        if (logisticsMode.SpecialFeeRate != null)
        {
            model.SpecialFeeRateId = logisticsMode.SpecialFeeRate.Id;
        }

        if (logisticsMode.CustomerFeeRate != null)
        {
            model.CustomerFeeRateId = logisticsMode.CustomerFeeRate.Id;
        }

        if (logisticsMode.InnerFeeRate != null)
        {
            model.InnerFeeRateId = logisticsMode.InnerFeeRate.Id;
        }

        model.FeeCalDesc = logisticsMode.FeeCalDesc;

        if (logisticsMode.WarehouseFrom != null)
        {
            model.WarehouseFromId = logisticsMode.WarehouseFrom.Id;
        }

        if (logisticsMode.WarehouseTo != null)
        {
            model.WarehouseToId = logisticsMode.WarehouseTo.Id;
        }

        model.LogisticsModeTypeList = typeList;
        model.PackSizeTypeList = LogisticsModePackSizeType.GetList();

        var feeRateEnabledStatus = LogisticsFeeRateStatus.Enabled;
        model.LogisticsFeeRateList = session.Query<LogisticsFeeRateInfo>().Where(e => e.Status == feeRateEnabledStatus).OrderBy(e => e.Id).ToList();
        model.WarehouseList = session.Query<WarehouseInfo>().OrderBy(e => e.OrderNum).ToList();
        model.PickupStationList = session.Query<PickupStationInfo>().OrderBy(e => e.Name).ToList();
        model.StatusList = LogisticsModeStatus.GetList();
        model.RepackTypeList = LogisticsRepackType.GetList();
        model.SecondLogisticModeTypeList = LogisticsModeType.GetCnAuSecondTypeList();
        model.GoodsTypeList = LogisticsGoodsType.GetCustomerList();
        model.IsFirstLogisticsMode = LogisticsModeType.IsCnAuFirstType(logisticsMode.Type?.Id ?? 0);

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
    public IActionResult Save(LogisticsModeDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (model.Type == LogisticsModeType.CnAuExpress)
        {
            if (string.IsNullOrWhiteSpace(model.ExpressCompanyName))
            {
                throw new ValidationException("快递单公司名称不能为空！");
            }

            if (string.IsNullOrWhiteSpace(model.ExpressCompanyUrl))
            {
                throw new ValidationException("快递公司网址不能为空！");
            }
        }

        if (model.Type == LogisticsModeType.HomeDelivery)
        {
            if (model.FreeDeliveryDistance != null && model.FreeDeliveryDistance.Value < 0)
            {
                throw new ValidationException("送货上门减免距离不能小于 0 ！");
            }
        }

        if (model.Type == LogisticsModeType.AuExpress)
        {
            if (model.FreeDeliveryDistance != null && model.FreeDeliveryDistance.Value < 0)
            {
                throw new ValidationException("澳洲快递减免距离不能小于 0 ！");
            }
        }

        if (model.Type == LogisticsModeType.CnAuSea)
        {
            if (model.PackSizeType == null || model.PackSizeType == LogisticsModePackSizeType.None)
            {
                throw new ValidationException("海运类型物流包裹大小件设置不能为空！");
            }
        }

        int logisticsModeId = model.Id;

        bool isCreate = logisticsModeId == 0;

        LogisticsModeInfo logisticsMode = null;

        if (isCreate)
        {
            logisticsMode = new LogisticsModeInfo();
        }
        else
        {
            logisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);
        }

        logisticsMode.Type = model.Type;

        if (model.PackSizeType == null)
        {
            model.PackSizeType = LogisticsModePackSizeType.None;
        }

        logisticsMode.PackSizeType = model.PackSizeType;

        logisticsMode.Code = model.Code;
        logisticsMode.Name = model.Name;
        logisticsMode.Description = model.Description;
        logisticsMode.CompanyName = model.CompanyName;
        logisticsMode.IconFileName = model.IconFileName;

        logisticsMode.MinWeight = model.MinWeight;
        logisticsMode.MaxWeight = model.MaxWeight;

        logisticsMode.SyncTla = model.SyncTla;
        if (model.PackPrintTypeId == null)
        {
            logisticsMode.PackPrintType = null;
        }
        else
        {
            logisticsMode.PackPrintType = LogisticsOrderPackPrintType.GetById(model.PackPrintTypeId.Value);
        }

        logisticsMode.EnglishPackPrint = model.EnglishPackPrint;
        logisticsMode.MultiExpressOnePack = model.MultiExpressOnePack;

        if (logisticsMode.Type == LogisticsModeType.CnAuExpress)
        {
            logisticsMode.ExpressCompanyName = model.ExpressCompanyName;
            logisticsMode.ExpressCompanyUrl = model.ExpressCompanyUrl;
        }

        if (logisticsMode.Type == LogisticsModeType.HomeDelivery || model.Type == LogisticsModeType.AuExpress)
        {
            logisticsMode.FreeDeliveryDistance = model.FreeDeliveryDistance;
        }

        logisticsMode.Status = model.Status;
        logisticsMode.InnerStatus = model.InnerStatus;

        if (model.FeeRateId.HasValue && model.FeeRateId.Value != 0)
        {
            logisticsMode.FeeRate = session.Load<LogisticsFeeRateInfo>(model.FeeRateId.Value);
        }
        else
        {
            logisticsMode.FeeRate = null;
        }

        if (model.CommonFeeRateId.HasValue && model.CommonFeeRateId.Value != 0)
        {
            logisticsMode.CommonFeeRate = session.Load<LogisticsFeeRateInfo>(model.CommonFeeRateId.Value);
        }
        else
        {
            logisticsMode.CommonFeeRate = null;
        }

        if (model.RestrictFeeRateId.HasValue && model.RestrictFeeRateId.Value != 0)
        {
            logisticsMode.RestrictFeeRate = session.Load<LogisticsFeeRateInfo>(model.RestrictFeeRateId.Value);
        }
        else
        {
            logisticsMode.RestrictFeeRate = null;
        }

        if (model.BambooFeeRateId.HasValue && model.BambooFeeRateId.Value != 0)
        {
            logisticsMode.BambooFeeRate = session.Load<LogisticsFeeRateInfo>(model.BambooFeeRateId.Value);
        }
        else
        {
            logisticsMode.BambooFeeRate = null;
        }

        if (model.SpecialFeeRateId.HasValue && model.SpecialFeeRateId.Value != 0)
        {
            logisticsMode.SpecialFeeRate = session.Load<LogisticsFeeRateInfo>(model.SpecialFeeRateId.Value);
        }
        else
        {
            logisticsMode.SpecialFeeRate = null;
        }

        if (model.CustomerFeeRateId.HasValue && model.CustomerFeeRateId.Value != 0)
        {
            logisticsMode.CustomerFeeRate = session.Load<LogisticsFeeRateInfo>(model.CustomerFeeRateId.Value);
        }
        else
        {
            logisticsMode.CustomerFeeRate = null;
        }

        if (model.InnerFeeRateId.HasValue && model.InnerFeeRateId.Value != 0)
        {
            logisticsMode.InnerFeeRate = session.Load<LogisticsFeeRateInfo>(model.InnerFeeRateId.Value);
        }
        else
        {
            logisticsMode.InnerFeeRate = null;
        }

        logisticsMode.FeeCalDesc = model.FeeCalDesc;

        logisticsMode.WarehouseFrom = session.Load<WarehouseInfo>(model.WarehouseFromId);
        if (model.WarehouseToId != null && model.WarehouseToId.Value != 0)
        {
            logisticsMode.WarehouseTo = session.Load<WarehouseInfo>(model.WarehouseToId.Value);
        }

        if (model.Type == LogisticsModeType.TerminalPickup)
        {
            if (model.PickupStationId != null && model.PickupStationId.Value != 0)
            {
                logisticsMode.PickupStation = session.Load<PickupStationInfo>(model.PickupStationId.Value);
            }
        }

        CheckResult checkResult = logisticsModeService.ValidateLogisticsMode(logisticsMode);
        if (checkResult.Success == false)
        {
            ModelState.AddModelError("Warehouse", checkResult.Message);
        }

        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (model.Type != LogisticsModeType.TerminalPickup)
        {
            logisticsMode.WarehouseFrom = session.Load<WarehouseInfo>(model.WarehouseFromId);
        }
        else
        {
            PickupStationInfo pickupStationInfo = session.Get<PickupStationInfo>(model.PickupStationId);

            logisticsMode.PickupStation = pickupStationInfo;
            logisticsMode.WarehouseFrom = pickupStationInfo.Warehouse; // 这里如果以后自提点里面的仓库设置改了，也要更新到自提点对应的物流方式里面去
        }

        if (model.WarehouseToId != null)
        {
            logisticsMode.WarehouseTo = session.Load<WarehouseInfo>(model.WarehouseToId.Value);
        }
        else
        {
            logisticsMode.WarehouseTo = null;
        }

        logisticsMode.InnerModeId = model.InnerModeId;

        if (isCreate)
        {
            session.Save(logisticsMode);
        }
        else
        {
            session.Update(logisticsMode);
        }

        session.Flush();

        var logisticsModeIdInDb = logisticsMode.Id;

        #region 打包方式

        // 保存打包方式

        IList<int> repackTypeIdList = model.RepackTypeIdList;
        if (repackTypeIdList == null)
        {
            repackTypeIdList = new List<int>();
        }

        IList<int> repackTypeIdListInDb = session
            .Query<LogisticsModeRepackTypeInfo>()
            .Where(e => e.LogisticsModeId == logisticsModeIdInDb)
            .Select(e => e.RepackTypeId)
            .ToList();

        // 界面上有数据库里面没有的添加保存
        foreach (var repackTypeId in repackTypeIdList)
        {
            if (repackTypeIdListInDb.Contains(repackTypeId) == false)
            {
                LogisticsModeRepackTypeInfo logisticsModeRepackTypeInfo = new LogisticsModeRepackTypeInfo();
                logisticsModeRepackTypeInfo.LogisticsModeId = logisticsModeIdInDb;
                logisticsModeRepackTypeInfo.RepackTypeId = repackTypeId;

                session.Save(logisticsModeRepackTypeInfo);
            }
        }

        // 数据库里面有界面上没有的删除
        foreach (var repackTypeId in repackTypeIdListInDb)
        {
            if (repackTypeIdList.Contains(repackTypeId) == false)
            {
                session.Query<LogisticsModeRepackTypeInfo>().Where(e => e.LogisticsModeId == logisticsModeIdInDb && e.RepackTypeId == repackTypeId).Delete();
            }
        }

        session.Flush();

        #endregion

        #region 二程物流方式关联，物品类型关联

        bool isFirstLogisticsMode = LogisticsModeType.IsCnAuFirstType(logisticsMode.Type?.Id ?? 0);

        if (isFirstLogisticsMode)
        {
            // 保存 二程物流方式关联

            IList<int> secondLogisticModeTypeIdList = model.SecondLogisticModeTypeIdList;
            if (secondLogisticModeTypeIdList == null)
            {
                secondLogisticModeTypeIdList = new List<int>();
            }

            IList<int> secondLogisticModeTypeIdListInDb = session
                .Query<LogisticsModeFirstSecondLinkInfo>()
                .Where(e => e.FirstLogisticsMode.Id == logisticsModeIdInDb)
                .Select(e => e.SecondLogisticsType)
                .ToList()
                .Select(e => e.Id) // SecondLogisticsTypeId
                .ToList();

            // 二程物流方式关联 界面上有数据库里面没有的添加保存
            foreach (var secondLogisticsTypeId in secondLogisticModeTypeIdList)
            {
                if (secondLogisticModeTypeIdListInDb.Contains(secondLogisticsTypeId) == false)
                {
                    LogisticsModeFirstSecondLinkInfo logisticsModeRepackTypeInfo = new LogisticsModeFirstSecondLinkInfo();
                    logisticsModeRepackTypeInfo.FirstLogisticsMode = logisticsMode;
                    logisticsModeRepackTypeInfo.SecondLogisticsType = LogisticsModeType.GetById(secondLogisticsTypeId);

                    session.Save(logisticsModeRepackTypeInfo);
                }
            }

            // 二程物流方式关联 数据库里面有界面上没有的删除
            foreach (var secondLogisticsTypeId in secondLogisticModeTypeIdListInDb)
            {
                if (secondLogisticModeTypeIdList.Contains(secondLogisticsTypeId) == false)
                {
                    LogisticsModeType secondLogisticsType = LogisticsModeType.GetById(secondLogisticsTypeId);
                    session.Query<LogisticsModeFirstSecondLinkInfo>()
                        .Where(e => e.FirstLogisticsMode.Id == logisticsModeIdInDb && e.SecondLogisticsType == secondLogisticsType)
                        .Delete();
                }
            }

            session.Flush();

            IList<int> goodsTypeIdList = model.GoodsTypeIdList;
            if (goodsTypeIdList == null)
            {
                goodsTypeIdList = new List<int>();
            }

            IList<int> goodsTypeIdListInDb = session
                .Query<LogisticsModeGoodsTypeInfo>()
                .Where(e => e.LogisticsMode.Id == logisticsModeIdInDb)
                .Select(e => e.GoodsType)
                .ToList()
                .Select(e => e.Id)
                .ToList();

            // 物品类型关联 界面上有数据库里面没有的添加保存
            foreach (int goodsTypeId in goodsTypeIdList)
            {
                if (goodsTypeIdListInDb.Contains(goodsTypeId) == false)
                {
                    LogisticsModeGoodsTypeInfo logisticsModeGoodsType = new LogisticsModeGoodsTypeInfo();
                    logisticsModeGoodsType.LogisticsMode = logisticsMode;
                    logisticsModeGoodsType.GoodsType = LogisticsGoodsType.GetById(goodsTypeId);

                    session.Save(logisticsModeGoodsType);
                }
            }

            // 物品类型关联 数据库里面有界面上没有的删除
            foreach (int goodsTypeId in goodsTypeIdListInDb)
            {
                if (goodsTypeIdList.Contains(goodsTypeId) == false)
                {
                    LogisticsGoodsType goodsType = LogisticsGoodsType.GetById(goodsTypeId);
                    session.Query<LogisticsModeGoodsTypeInfo>()
                        .Where(e => e.LogisticsMode.Id == logisticsModeIdInDb && e.GoodsType == goodsType)
                        .Delete();
                }
            }

            session.Flush();
        }

        #endregion

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", logisticsMode.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/mode/delete")]
    public IActionResult Delete(int[] idList)
    {
        foreach (int modeId in idList)
        {
            LogisticsModeInfo logisticsMode = session.Load<LogisticsModeInfo>(modeId);


            // 先删除关联的费率数据
            if (logisticsMode.Type == LogisticsModeType.CnAuAir)
            {
                session.Query<LogisticsFeeRateAirInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            if (logisticsMode.Type == LogisticsModeType.CnAuExpress)
            {
                session.Query<LogisticsFeeRateExpressInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            if (logisticsMode.Type == LogisticsModeType.AuExpress)
            {
                session.Query<LogisticsFeeRateExpressAuInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            if (logisticsMode.Type == LogisticsModeType.HomeDelivery)
            {
                session.Query<LogisticsFeeRateHomeDeliveryInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            if (logisticsMode.Type == LogisticsModeType.TerminalPickup)
            {
                session.Query<LogisticsFeeRatePickupStationInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            if (logisticsMode.Type == LogisticsModeType.CnAuSea)
            {
                session.Query<LogisticsFeeRateSeaInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            if (logisticsMode.Type == LogisticsModeType.WarehousePickup)
            {
                session.Query<LogisticsFeeRateWarehouseInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
            }

            session.Delete(logisticsMode);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}