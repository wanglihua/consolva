using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Logistics.Models.LogisticsFeeRate;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsFeeRate)]
public class LogisticsFeeRateController : Controller
{
    private readonly LogisticsFeeRateService logisticsFeeRateService;
    private readonly ISession session;

    public LogisticsFeeRateController(ISession session, LogisticsFeeRateService logisticsFeeRateService)
    {
        this.session = session;
        this.logisticsFeeRateService = logisticsFeeRateService;
    }

    [HttpGet]
    [Route("/admin/logistics/fee/rate")]
    public IActionResult Index()
    {
        var model = new LogisticsFeeRateIndexModel();
        model.StatusList = LogisticsFeeRateStatus.GetList();

        return View("LogisticsFeeRateIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/fee/rate/list")]
    public IActionResult List(LogisticsFeeRateListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsFeeRateList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsFeeRateInfo>();

        foreach (var logisticsFeeRateInfo in logisticsFeeRateList)
        {
            // logisticsFeeRateInfo
        }

        var model = new LogisticsFeeRateListModel(listPager);
        model.LogisticsFeeRateList = logisticsFeeRateList;

        return View("LogisticsFeeRateList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsFeeRateListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<LogisticsFeeRateInfo>();

        criteria = criteria.SetReadOnly(true);

        var nameQuery = queryParam.NameQuery ?? string.Empty;
        if (string.IsNullOrWhiteSpace(nameQuery) == false)
        {
            nameQuery = nameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsFeeRateInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        var statusQuery = queryParam.StatusQuery ?? LogisticsFeeRateStatus.None;
        if (statusQuery != null && statusQuery != LogisticsFeeRateStatus.None) criteria = criteria.Add(Restrictions.Where<LogisticsFeeRateInfo>(e => e.Status == statusQuery));

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/fee/rate/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        LogisticsFeeRateInfo logisticsFeeRate;

        if (id == null || id.Value == 0)
        {
            logisticsFeeRate = new LogisticsFeeRateInfo();
            logisticsFeeRate.PublishRate = 1.0m;
            logisticsFeeRate.VolumeDenominator = logisticsFeeRateService.GetVolumeDenominator(); // 取体积重分母默认值

            logisticsFeeRate.CustomerWidthDiscount = 1.0m;
            logisticsFeeRate.CustomerLengthDiscount = 1.0m;
            logisticsFeeRate.CustomerHeightDiscount = 1.0m;
            logisticsFeeRate.CustomerWeightDiscount = 1.0m;

            logisticsFeeRate.ExportWidthDiscount = 1.0m;
            logisticsFeeRate.ExportLengthDiscount = 1.0m;
            logisticsFeeRate.ExportHeightDiscount = 1.0m;
            logisticsFeeRate.ExportWeightDiscount = 1.0m;
        }
        else
        {
            logisticsFeeRate = session.Get<LogisticsFeeRateInfo>(id.Value);
            if (logisticsFeeRate == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new LogisticsFeeRateDetailModel();
        model.LogisticsFeeRate = logisticsFeeRate;
        model.VolumeWeightResultTypeList = VolumeWeightResultType.GetList();
        model.StatusList = LogisticsFeeRateStatus.GetList();

        return View("LogisticsFeeRateDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/fee/rate/save")]
    [Transactional]
    public IActionResult Save(LogisticsFeeRateDetailModel model)
    {
        var logisticsFeeRateFromUi = model.LogisticsFeeRate;

        // 在基本的验证通过的情况下加上额外的验证
        if (ModelState.IsValid)
        {
            if (logisticsFeeRateFromUi.CareVolumeWeight
                && (logisticsFeeRateFromUi.VolumeDenominator.HasValue == false || logisticsFeeRateFromUi.VolumeDenominator.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.VolumeDenominator), "考虑体积重，但体积重分母 未设置 或 为0！");

            if (logisticsFeeRateFromUi.CareVolumeActualWeightRatio
                && (logisticsFeeRateFromUi.MinVolumeActualWeightRatio.HasValue == false || logisticsFeeRateFromUi.MinVolumeActualWeightRatio.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.MinVolumeActualWeightRatio), "考虑体积重实际重倍率，但体积重实际重最小倍率 未设置 或 为0！");

            if (logisticsFeeRateFromUi.VolumeWeightCareMaxSideLength
                && (logisticsFeeRateFromUi.VolumeWeightMaxSideLengthBegin.HasValue == false || logisticsFeeRateFromUi.VolumeWeightMaxSideLengthBegin.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.VolumeWeightMaxSideLengthBegin), "体积重考虑最长边，但体积重最长边起始 未设置 或 为0！");

            if (logisticsFeeRateFromUi.CareGoodsVolumePrice
                && (logisticsFeeRateFromUi.GoodsVolumePrice == null || logisticsFeeRateFromUi.GoodsVolumePrice.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.GoodsVolumePrice), "考虑体积货值，但体积货值未设定！");

            if (logisticsFeeRateFromUi.HasLengthStart
                && logisticsFeeRateFromUi.LengthStart <= 0)
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.LengthStart), "有长度起步，但起步长度 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasLengthLimit
                && (logisticsFeeRateFromUi.LengthLimit.HasValue == false || logisticsFeeRateFromUi.LengthLimit.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.LengthLimit), "有长度限制，但限制长度 未配置 或 为0！");

            if (logisticsFeeRateFromUi.CanOverLength)
            {
                if (logisticsFeeRateFromUi.OverLengthStep.HasValue == false || logisticsFeeRateFromUi.OverLengthStep.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.OverLengthStep), "可以超长，但超长阶梯 未配置 或 为0！");

                if (logisticsFeeRateFromUi.OverLengthStepFee.HasValue == false || logisticsFeeRateFromUi.OverLengthStepFee.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.OverLengthStepFee), "可以超长，但超长阶梯收费 未配置 或 为0！");
            }

            if (logisticsFeeRateFromUi.HasOverLengthCapAmount
                && (logisticsFeeRateFromUi.OverLengthCapAmount.HasValue == false || logisticsFeeRateFromUi.OverLengthCapAmount.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.OverLengthCapAmount), "长度有费用封顶，但超封顶金额 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasSingleWeightStart
                && logisticsFeeRateFromUi.SingleWeightStart <= 0)
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.SingleWeightStart), "单箱重量有起步重量，但起步重量 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasSingleWeightLimit
                && (logisticsFeeRateFromUi.SingleWeightLimit.HasValue == false || logisticsFeeRateFromUi.SingleWeightLimit.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.SingleWeightLimit), "单箱重量有限制，但限制重量 未配置 或 为0！");

            if (logisticsFeeRateFromUi.CanSingleOverWeight)
            {
                if (logisticsFeeRateFromUi.SingleOverWeightStep.HasValue == false || logisticsFeeRateFromUi.SingleOverWeightStep.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.SingleOverWeightStep), "允许单箱超重，但单箱超重阶梯 未设置 或 为0！");

                if (logisticsFeeRateFromUi.SingleOverWeightStepFee.HasValue == false || logisticsFeeRateFromUi.SingleOverWeightStepFee.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.SingleOverWeightStepFee), "允许单箱超重，但单箱超重阶梯收费 未设置 或 为0！");
            }

            if (logisticsFeeRateFromUi.HasAverageWeightStart
                && logisticsFeeRateFromUi.AverageWeightStart <= 0)
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.AverageWeightStart), "平均单箱重量有起步重量，但起步重量 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasAverageWeightLimit
                && (logisticsFeeRateFromUi.AverageWeightLimit.HasValue == false || logisticsFeeRateFromUi.AverageWeightLimit.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.AverageWeightLimit), "平均单箱重量有限制，但限制重量 未配置 或 为0！");

            if (logisticsFeeRateFromUi.CanAverageOverWeight)
            {
                if (logisticsFeeRateFromUi.AverageOverWeightStep.HasValue == false || logisticsFeeRateFromUi.AverageOverWeightStep.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.AverageOverWeightStep), "允许平均单箱超重，但平均单箱超重阶梯 未设置 或 为0！");

                if (logisticsFeeRateFromUi.AverageOverWeightStepFee.HasValue == false || logisticsFeeRateFromUi.AverageOverWeightStepFee.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.AverageOverWeightStepFee), "允许平均单箱超重，但平均单箱超重阶梯收费 未设置 或 为0！");
            }

            if (logisticsFeeRateFromUi.HasSingleOverWeightCapAmount
                && (logisticsFeeRateFromUi.SingleOverWeightCapAmount.HasValue == false || logisticsFeeRateFromUi.SingleOverWeightCapAmount.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.SingleOverWeightCapAmount), "单箱重量有费用封顶，但超封顶金额 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasWeightStart
                && logisticsFeeRateFromUi.WeightStart <= 0)
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.WeightStart), "重量有起步重量，但起步重量 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasWeightLimit
                && (logisticsFeeRateFromUi.WeightLimit.HasValue == false || logisticsFeeRateFromUi.WeightLimit.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.WeightLimit), "重量有限制，但限制重量 未配置 或 为0！");

            if (logisticsFeeRateFromUi.CanOverWeight)
            {
                if (logisticsFeeRateFromUi.OverWeightStep.HasValue == false || logisticsFeeRateFromUi.OverWeightStep.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.OverWeightStep), "允许超重，但超重阶梯 未设置 或 为0！");

                if (logisticsFeeRateFromUi.OverWeightStepFee.HasValue == false || logisticsFeeRateFromUi.OverWeightStepFee.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.OverWeightStepFee), "允许超重，但超重阶梯收费 未设置 或 为0！");
            }

            if (logisticsFeeRateFromUi.HasOverWeightCapAmount
                && (logisticsFeeRateFromUi.OverWeightCapAmount.HasValue == false || logisticsFeeRateFromUi.OverWeightCapAmount.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.OverWeightCapAmount), "重量有费用封顶，但封顶金额 未配置 或 为0！");

            if (logisticsFeeRateFromUi.HasFreePackageLimit)
            {
                if (logisticsFeeRateFromUi.FreePackageLimit.HasValue == false || logisticsFeeRateFromUi.FreePackageLimit.Value < 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.FreePackageLimit), "包裹免费数量有限制，但免费包裹数量 未设置 或 值小于0！");

                if (logisticsFeeRateFromUi.ExtraPackageFee.HasValue == false || logisticsFeeRateFromUi.ExtraPackageFee.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.ExtraPackageFee), "包裹免费数量有限制，但额外包裹收费 未设置 或 值不大于0！");
            }

            if (logisticsFeeRateFromUi.HasExtraPackageFeeCapAmount
                && (logisticsFeeRateFromUi.ExtraPackageFeeCapAmount.HasValue == false || logisticsFeeRateFromUi.ExtraPackageFeeCapAmount.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.ExtraPackageFeeCapAmount), "包裹数量有费用封顶，但封顶金额 未配置 或 值不大于0！");

            if (logisticsFeeRateFromUi.HasFreeStorageDayLimit)
            {
                if (logisticsFeeRateFromUi.FreeStorageDayLimit.HasValue == false || logisticsFeeRateFromUi.FreeStorageDayLimit.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.FreeStorageDayLimit), "仓储免费天数有限制，但免费仓储天数 未设置 或 为0！");

                if (logisticsFeeRateFromUi.ExtraStorageDayFee.HasValue == false || logisticsFeeRateFromUi.ExtraStorageDayFee.Value <= 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.ExtraStorageDayFee), "仓储免费天数有限制，但额外仓储收费 未设置 或 为0！");

                if (logisticsFeeRateFromUi.ExtraStorageDayMinFeePerDay.HasValue && logisticsFeeRateFromUi.ExtraStorageDayMinFeePerDay.Value < 0) ModelState.AddModelError(nameof(LogisticsFeeRateInfo.ExtraStorageDayMinFeePerDay), "仓储免费天数有限制，但最低仓储收费为负数，请设置不小于0的值！");
            }

            if (logisticsFeeRateFromUi.HasExtraStorageDayFeeCapAmount
                && (logisticsFeeRateFromUi.ExtraStorageDayFeeCapAmount.HasValue == false || logisticsFeeRateFromUi.ExtraStorageDayFeeCapAmount.Value <= 0))
                ModelState.AddModelError(nameof(LogisticsFeeRateInfo.ExtraStorageDayFeeCapAmount), "仓储有费用封顶，但封顶金额 未配置 或 为0！");
        }

        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));


        var logisticsFeeRateId = logisticsFeeRateFromUi.Id;

        var isCreate = logisticsFeeRateId == 0;

        LogisticsFeeRateInfo logisticsFeeRate;

        if (isCreate)
            logisticsFeeRate = new LogisticsFeeRateInfo();
        else
            logisticsFeeRate = session.Get<LogisticsFeeRateInfo>(logisticsFeeRateId);

        logisticsFeeRate.Name = logisticsFeeRateFromUi.Name;
        logisticsFeeRate.Status = logisticsFeeRateFromUi.Status;

        logisticsFeeRate.CareVolumeWeight = logisticsFeeRateFromUi.CareVolumeWeight;
        logisticsFeeRate.VolumeDenominator = logisticsFeeRateFromUi.VolumeDenominator;
        logisticsFeeRate.CareSideSum = logisticsFeeRateFromUi.CareSideSum;
        logisticsFeeRate.VolumeWeightResultType = logisticsFeeRateFromUi.VolumeWeightResultType;
        logisticsFeeRate.VolumeActualWeightMiddle = logisticsFeeRateFromUi.VolumeActualWeightMiddle;
        logisticsFeeRate.CareGoodsVolumePrice = logisticsFeeRateFromUi.CareGoodsVolumePrice;
        logisticsFeeRate.GoodsVolumePrice = logisticsFeeRateFromUi.GoodsVolumePrice;
        logisticsFeeRate.CareVolumeActualWeightRatio = logisticsFeeRateFromUi.CareVolumeActualWeightRatio;
        logisticsFeeRate.MinVolumeActualWeightRatio = logisticsFeeRateFromUi.MinVolumeActualWeightRatio;
        logisticsFeeRate.VolumeWeightCareMaxSideLength = logisticsFeeRateFromUi.VolumeWeightCareMaxSideLength;
        logisticsFeeRate.VolumeWeightMaxSideLengthBegin = logisticsFeeRateFromUi.VolumeWeightMaxSideLengthBegin;

        logisticsFeeRate.CustomerLengthDiscount = logisticsFeeRateFromUi.CustomerLengthDiscount;
        logisticsFeeRate.CustomerWidthDiscount = logisticsFeeRateFromUi.CustomerWidthDiscount;
        logisticsFeeRate.CustomerHeightDiscount = logisticsFeeRateFromUi.CustomerHeightDiscount;
        logisticsFeeRate.CustomerWeightDiscount = logisticsFeeRateFromUi.CustomerWeightDiscount;

        logisticsFeeRate.ExportLengthDiscount = logisticsFeeRateFromUi.ExportLengthDiscount;
        logisticsFeeRate.ExportWidthDiscount = logisticsFeeRateFromUi.ExportWidthDiscount;
        logisticsFeeRate.ExportHeightDiscount = logisticsFeeRateFromUi.ExportHeightDiscount;
        logisticsFeeRate.ExportWeightDiscount = logisticsFeeRateFromUi.ExportWeightDiscount;

        logisticsFeeRate.HasLengthStart = logisticsFeeRateFromUi.HasLengthStart;
        logisticsFeeRate.LengthStart = logisticsFeeRateFromUi.LengthStart;
        logisticsFeeRate.HasLengthLimit = logisticsFeeRateFromUi.HasLengthLimit;
        logisticsFeeRate.LengthLimit = logisticsFeeRateFromUi.LengthLimit;
        logisticsFeeRate.CanOverLength = logisticsFeeRateFromUi.CanOverLength;
        logisticsFeeRate.OverLengthStep = logisticsFeeRateFromUi.OverLengthStep;
        logisticsFeeRate.OverLengthStepFee = logisticsFeeRateFromUi.OverLengthStepFee;
        logisticsFeeRate.OverLengthFeeEveryPack = logisticsFeeRateFromUi.OverLengthFeeEveryPack;
        logisticsFeeRate.HasOverLengthCapAmount = logisticsFeeRateFromUi.HasOverLengthCapAmount;
        logisticsFeeRate.OverLengthCapAmount = logisticsFeeRateFromUi.OverLengthCapAmount;

        logisticsFeeRate.HasSingleWeightStart = logisticsFeeRateFromUi.HasSingleWeightStart;
        logisticsFeeRate.SingleWeightStart = logisticsFeeRateFromUi.SingleWeightStart;
        logisticsFeeRate.HasSingleWeightLimit = logisticsFeeRateFromUi.HasSingleWeightLimit;
        logisticsFeeRate.SingleWeightLimit = logisticsFeeRateFromUi.SingleWeightLimit;
        logisticsFeeRate.CanSingleOverWeight = logisticsFeeRateFromUi.CanSingleOverWeight;
        logisticsFeeRate.SingleOverWeightStep = logisticsFeeRateFromUi.SingleOverWeightStep;
        logisticsFeeRate.SingleOverWeightStepFee = logisticsFeeRateFromUi.SingleOverWeightStepFee;
        logisticsFeeRate.HasSingleOverWeightCapAmount = logisticsFeeRateFromUi.HasSingleOverWeightCapAmount;
        logisticsFeeRate.SingleOverWeightCapAmount = logisticsFeeRateFromUi.SingleOverWeightCapAmount;

        logisticsFeeRate.HasAverageWeightStart = logisticsFeeRateFromUi.HasAverageWeightStart;
        logisticsFeeRate.AverageWeightStart = logisticsFeeRateFromUi.AverageWeightStart;
        logisticsFeeRate.HasAverageWeightLimit = logisticsFeeRateFromUi.HasAverageWeightLimit;
        logisticsFeeRate.AverageWeightLimit = logisticsFeeRateFromUi.AverageWeightLimit;
        logisticsFeeRate.CanAverageOverWeight = logisticsFeeRateFromUi.CanAverageOverWeight;
        logisticsFeeRate.AverageOverWeightStep = logisticsFeeRateFromUi.AverageOverWeightStep;
        logisticsFeeRate.AverageOverWeightStepFee = logisticsFeeRateFromUi.AverageOverWeightStepFee;
        logisticsFeeRate.HasAverageOverWeightCapAmount = logisticsFeeRateFromUi.HasAverageOverWeightCapAmount;
        logisticsFeeRate.AverageOverWeightCapAmount = logisticsFeeRateFromUi.AverageOverWeightCapAmount;

        logisticsFeeRate.HasWeightStart = logisticsFeeRateFromUi.HasWeightStart;
        logisticsFeeRate.WeightStart = logisticsFeeRateFromUi.WeightStart;
        logisticsFeeRate.HasWeightLimit = logisticsFeeRateFromUi.HasWeightLimit;
        logisticsFeeRate.WeightLimit = logisticsFeeRateFromUi.WeightLimit;
        logisticsFeeRate.CanOverWeight = logisticsFeeRateFromUi.CanOverWeight;
        logisticsFeeRate.OverWeightStep = logisticsFeeRateFromUi.OverWeightStep;
        logisticsFeeRate.OverWeightStepFee = logisticsFeeRateFromUi.OverWeightStepFee;
        logisticsFeeRate.HasOverWeightCapAmount = logisticsFeeRateFromUi.HasOverWeightCapAmount;
        logisticsFeeRate.OverWeightCapAmount = logisticsFeeRateFromUi.OverWeightCapAmount;

        logisticsFeeRate.HasFreePackageLimit = logisticsFeeRateFromUi.HasFreePackageLimit;
        logisticsFeeRate.FreePackageLimit = logisticsFeeRateFromUi.FreePackageLimit;
        logisticsFeeRate.ExtraPackageFee = logisticsFeeRateFromUi.ExtraPackageFee;
        logisticsFeeRate.HasExtraPackageFeeCapAmount = logisticsFeeRateFromUi.HasExtraPackageFeeCapAmount;
        logisticsFeeRate.ExtraPackageFeeCapAmount = logisticsFeeRateFromUi.ExtraPackageFeeCapAmount;

        logisticsFeeRate.HasFreeStorageDayLimit = logisticsFeeRateFromUi.HasFreeStorageDayLimit;
        logisticsFeeRate.FreeStorageDayLimit = logisticsFeeRateFromUi.FreeStorageDayLimit;
        logisticsFeeRate.ExtraStorageDayFee = logisticsFeeRateFromUi.ExtraStorageDayFee;
        logisticsFeeRate.ExtraStorageDayMinFeePerDay = logisticsFeeRateFromUi.ExtraStorageDayMinFeePerDay;
        logisticsFeeRate.HasExtraStorageDayFeeCapAmount = logisticsFeeRateFromUi.HasExtraStorageDayFeeCapAmount;
        logisticsFeeRate.ExtraStorageDayFeeCapAmount = logisticsFeeRateFromUi.ExtraStorageDayFeeCapAmount;

        logisticsFeeRate.PublishRate = logisticsFeeRateFromUi.PublishRate;

        if (isCreate)
            session.Save(logisticsFeeRate);
        else
            session.Update(logisticsFeeRate);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "id", logisticsFeeRate.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/fee/rate/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var logisticsFeeRate = session.Get<LogisticsFeeRateInfo>(id);

            /* todo 启用这个检查
            IList<LogisticsModeInfo> modeList = session.Query<LogisticsModeInfo>().Where(e => e.FeeRate.Id 这里还要完善 == id).ToList();

            if (modeList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("通用费率 ");
                sb.Append(logisticsFeeRate.Name);
                sb.Append(" 正被 ");

                if (modeList.Count == 1)
                {
                    sb.Append(modeList[0].Name);
                }

                if (modeList.Count > 1)
                {
                    sb.Append(modeList[0].Name);
                    sb.Append("等");
                }

                sb.Append(" 物流方式引用中，不能删除！");

                throw new ValidationException(sb.ToString());
            }
            */

            session.Delete(logisticsFeeRate);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}