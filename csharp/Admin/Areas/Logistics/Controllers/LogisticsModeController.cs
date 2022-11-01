using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.Logistics.Models.LogisticsMode;
using shunda.Core;
using shunda.Core.Hibernate;
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

        model.LogisticsModeTypeList = LogisticsModeType.GetList();
        model.StatusList = LogisticsModeStatus.GetList();

        return View("LogisticsModeIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/mode/list")]
    public IActionResult List(LogisticsModeListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsModeInfo> logisticsModeList = GetListCriteria(queryParam)
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

        foreach (LogisticsModeInfo mode in logisticsModeList)
        {
            int modeId = mode.Id;
            mode.ModeFeeRateListCache = modeFeeRateList
                .Where(e => e.Mode.Id == modeId)
                .OrderBy(e =>e.GoodsType.Id) // 显示时按物品类型排序
                .ToList();
        }

        LogisticsModeListModel model = new LogisticsModeListModel(listPager);
        model.LogisticsModeList = logisticsModeList;
        model.ModeType = LogisticsModeType.GetById((queryParam.TypeIdQuery ?? 0));

        return View("LogisticsModeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsModeListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsModeInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsModeInfo.Warehouse), nameof(LogisticsModeInfo.Warehouse));

        criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Cate == LogisticsModeCate.Customer));


        if (queryParam.TypeIdQuery != null && queryParam.TypeIdQuery.Value != 0)
        {
            var typeQuery = LogisticsModeType.GetById(queryParam.TypeIdQuery.Value);
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


        if (queryParam.StatusQuery != null && queryParam.StatusQuery != LogisticsModeStatus.None)
        {
            LogisticsModeStatus statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Status == statusQuery));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/detail/{id:int}/{typeId:int?}")]
    public IActionResult Detail(int id, int? typeId)
    {
        session.DefaultReadOnly = true;

        IList<LogisticsModeType> typeList = LogisticsModeType.GetList();

        LogisticsModeDetailModel model = new LogisticsModeDetailModel();

        LogisticsModeInfo logisticsMode;
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
                .Where(e => e.LogisticsMode.Id == id)
                .ToList();

            model.RepackTypeIdList = repackTypeList.Select(e => e.RepackType.Id).ToList();

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

        model.ExpressCompanyName = logisticsMode.ExpressCompanyName;
        model.ExpressCompanyUrl = logisticsMode.ExpressCompanyUrl;

        model.Status = logisticsMode.Status;
        model.InnerStatus = logisticsMode.InnerStatus;

        // todo 这里要修改

        model.FeeCalDesc = logisticsMode.FeeCalDesc;

        if (logisticsMode.Warehouse != null)
        {
            model.WarehouseId = logisticsMode.Warehouse.Id;
        }

        model.LogisticsModeTypeList = typeList;
        model.PackSizeTypeList = LogisticsModePackSizeType.GetList();

        var feeRateEnabledStatus = LogisticsFeeRateStatus.Enabled;
        model.LogisticsFeeRateList = session.Query<LogisticsFeeRateInfo>().Where(e => e.Status == feeRateEnabledStatus).OrderBy(e => e.Id).ToList();
        model.WarehouseList = session.Query<WarehouseInfo>().OrderBy(e => e.OrderNum).ToList();
        model.StatusList = LogisticsModeStatus.GetList();
        model.RepackTypeList = LogisticsRepackType.GetList();
        model.GoodsTypeList = LogisticsGoodsType.GetCustomerList();

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

        int logisticsModeId = model.Id;

        bool isCreate = logisticsModeId == 0;

        LogisticsModeInfo logisticsMode;

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

        if (logisticsMode.Type == LogisticsModeType.CnAuExpress)
        {
            logisticsMode.ExpressCompanyName = model.ExpressCompanyName ?? string.Empty;
            logisticsMode.ExpressCompanyUrl = model.ExpressCompanyUrl ?? string.Empty;
        }

        logisticsMode.Status = model.Status;
        logisticsMode.InnerStatus = model.InnerStatus;

        // todo 这里要修改

        logisticsMode.FeeCalDesc = model.FeeCalDesc;

        if (model.WarehouseId == 0)
        {
            throw new ValidationException("收货仓库不能为空！");
        }

        var warehouse = session.Get<WarehouseInfo>(model.WarehouseId);
        if (warehouse == null)
        {
            throw new ValidationException($"指定的收货仓库#{model.WarehouseId}不存在！");
        }

        logisticsMode.Warehouse = warehouse;

        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
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

        var modeId = logisticsMode.Id;

        #region 打包方式

        // 保存打包方式

        IList<int> repackTypeIdList = model.RepackTypeIdList;
        if (repackTypeIdList == null)
        {
            repackTypeIdList = new List<int>();
        }

        IList<LogisticsRepackType> repackTypeListInDb = session
            .Query<LogisticsModeRepackTypeInfo>()
            .Where(e => e.LogisticsMode.Id == modeId)
            .Select(e => e.RepackType)
            .ToList();

        // 界面上有数据库里面没有的添加保存
        foreach (var repackTypeId in repackTypeIdList)
        {
            LogisticsRepackType repackType = LogisticsRepackType.GetById(repackTypeId);

            if (repackTypeListInDb.Contains(repackType) == false)
            {
                LogisticsModeRepackTypeInfo logisticsModeRepackTypeInfo = new LogisticsModeRepackTypeInfo();
                logisticsModeRepackTypeInfo.LogisticsMode = logisticsMode;
                logisticsModeRepackTypeInfo.RepackType = repackType;

                session.Save(logisticsModeRepackTypeInfo);
            }
        }

        session.Flush();

        // 数据库里面有界面上没有的删除
        foreach (var repackType in repackTypeListInDb)
        {
            if (repackTypeIdList.Contains(repackType.Id) == false)
            {
                session.Query<LogisticsModeRepackTypeInfo>()
                    .Where(e => e.LogisticsMode.Id == modeId
                                && e.RepackType == repackType)
                    .Delete();
            }
        }

        session.Flush();

        #endregion

        #region 物品类型

        IList<int> goodsTypeIdList = model.GoodsTypeIdList;
        if (goodsTypeIdList == null)
        {
            goodsTypeIdList = new List<int>();
        }

        IList<LogisticsGoodsType> goodsTypeListInDb = session
            .Query<LogisticsModeGoodsTypeInfo>()
            .Where(e => e.LogisticsMode.Id == modeId)
            .Select(e => e.GoodsType)
            .ToList();

        // 物品类型关联 界面上有数据库里面没有的添加保存
        foreach (int goodsTypeId in goodsTypeIdList)
        {
            LogisticsGoodsType goodsType = LogisticsGoodsType.GetById(goodsTypeId);

            if (goodsTypeListInDb.Contains(goodsType) == false)
            {
                LogisticsModeGoodsTypeInfo logisticsModeGoodsType = new LogisticsModeGoodsTypeInfo();
                logisticsModeGoodsType.LogisticsMode = logisticsMode;
                logisticsModeGoodsType.GoodsType = goodsType;

                session.Save(logisticsModeGoodsType);
            }
        }

        // 物品类型关联 数据库里面有界面上没有的删除
        foreach (LogisticsGoodsType goodsType in goodsTypeListInDb)
        {
            if (goodsTypeIdList.Contains(goodsType.Id) == false)
            {
                session.Query<LogisticsModeGoodsTypeInfo>()
                    .Where(e => e.LogisticsMode.Id == modeId && e.GoodsType == goodsType)
                    .Delete();
            }
        }

        session.Flush();

        #endregion

        // todo 检查 物品类型 和 通用费率 的配合

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", logisticsMode.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/mode/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int modeId in idList)
        {
            LogisticsModeInfo logisticsMode = session.Get<LogisticsModeInfo>(modeId);

            if (logisticsMode == null)
            {
                throw new ValidationException($"指定的物流方式#{modeId}不存在！");
            }

            // 先删除关联的费率数据
            if (logisticsMode.Type == LogisticsModeType.CnAuExpress)
            {
                session.Query<LogisticsFeeRateExpressInfo>().Where(e => e.LogisticsModeId == modeId).Delete();
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