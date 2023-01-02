using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Logistics.Models.HsGoods;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsTrade)]
public class LogisticsGoodsTradeController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsGoodsImageService logisticsGoodsImageService;
    private readonly LogisticsGoodsLogService logisticsGoodsLogService;
    private readonly LogisticsGoodsTradeExcelService logisticsGoodsTradeExcelService;

    public LogisticsGoodsTradeController(
        ISession session,
        LoginUserService loginUserService,
        LogisticsGoodsImageService logisticsGoodsImageService,
        LogisticsGoodsLogService logisticsGoodsLogService,
        LogisticsGoodsTradeExcelService logisticsGoodsTradeExcelService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsGoodsImageService = logisticsGoodsImageService;
        this.logisticsGoodsLogService = logisticsGoodsLogService;
        this.logisticsGoodsTradeExcelService = logisticsGoodsTradeExcelService;
    }

    [HttpGet]
    [Route("/admin/logistics/goods/trade/{statusId:int}")]
    public IActionResult Index(int statusId) {
        LogisticsGoodsTradeIndexModel model = new LogisticsGoodsTradeIndexModel();

        model.Status = LogisticsGoodsStatus.GetById(statusId);
        model.GoodsTypeList = LogisticsGoodsType.GetInnerList();

        return View("LogisticsGoodsTradeIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/list")]
    public IActionResult List(TradeGoodsSearchListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.UpdateUserQuery) == false) {
            string updateUserQuery = queryParam.UpdateUserQuery.Trim();
            queryParam.UpdateUserId = session.Query<UserInfo>()
                .Where(e => e.LoginName == updateUserQuery || e.UserName == updateUserQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        int totalCount = GetTradeGoodsSearchListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetTradeGoodsSearchListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGoodsInfo>();

        IList<string> createExpressTrackingNoList = logisticsGoodsList.Select(e => e.CreateExpressTrackingNo).ToList();
        IList<ExpressPhotoInfo> createExpressImageListAll = logisticsGoodsImageService.GetGoodsCreateExpressImageList(createExpressTrackingNoList);

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
            logisticsGoods.CreateExpressPhotoListCache = createExpressImageListAll.Where(e => e.TrackingNo == logisticsGoods.CreateExpressTrackingNo).OrderBy(e => e.Id).ToList();

            IList<ExpressPhotoInfo> goodsExpressPhotoList = logisticsGoodsImageService.GetGoodsRecentOrderImageList(logisticsGoods.Id);

            IList<int> orderIdList = goodsExpressPhotoList.Where(e => e.Order != null).Select(e => e.Order!.Id).Distinct().OrderByDescending(e => e).ToList();

            if (orderIdList.Count > 0) {
                int orderId = orderIdList[0];
                logisticsGoods.FirstOrderExpressPhotoListCache = goodsExpressPhotoList.Where(e => e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
            }

            if (orderIdList.Count > 1) {
                int orderId = orderIdList[1];
                logisticsGoods.SecondOrderExpressPhotoListCache = goodsExpressPhotoList.Where(e => e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
            }

            if (orderIdList.Count > 2) {
                int orderId = orderIdList[2];
                logisticsGoods.ThirdOrderExpressPhotoListCache = goodsExpressPhotoList.Where(e => e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
            }
        }

        LogisticsGoodsTradeListModel model = new LogisticsGoodsTradeListModel(listPager);
        model.LogisticsGoodsList = logisticsGoodsList;

        return View("LogisticsGoodsTradeList", model);
    }

    [NonAction]
    public ICriteria GetTradeGoodsSearchListCriteria(TradeGoodsSearchListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);
        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.UpdateUser), nameof(LogisticsGoodsInfo.UpdateUser), JoinType.LeftOuterJoin);

        if (queryParam.StatusQuery != null) {
            LogisticsGoodsStatus statusQuery = queryParam.StatusQuery;

            if (statusQuery == LogisticsGoodsStatus.NotConfirmed) {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.PhotoRequired || e.Status == LogisticsGoodsStatus.NotConfirmed));
            }

            if (statusQuery == LogisticsGoodsStatus.Confirmed) {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.Confirmed));
            }
        }

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != LogisticsGoodsType.None) {
            LogisticsGoodsType logisticsGoodsType = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));
        }


        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false) {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CnHsCodeQuery) == false) {
            string cnHsCodeQuery = queryParam.CnHsCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.CnHsCode)}", cnHsCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false) {
            string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.CnHsName)}", cnHsNameQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.EnHsCodeQuery) == false) {
            string enHsCodeQuery = queryParam.EnHsCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.EnHsCode)}", enHsCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.EnHsNameQuery) == false) {
            string enHsNameQuery = queryParam.EnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.EnHsName)}", enHsNameQuery));
        }

        if (queryParam.UpdateUserId != 0) {
            int updateUserId = queryParam.UpdateUserId;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.UpdateUser)}.{nameof(UserInfo.Id)}", updateUserId));
        }

        if (queryParam.BeginUpdateDateQuery != null) {
            DateTime beginUpdateDataQuery = queryParam.BeginUpdateDateQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.UpdateTime >= beginUpdateDataQuery));
        }

        if (queryParam.EndUpdateDateQuery != null) {
            DateTime endUpdateDataQuery = queryParam.EndUpdateDateQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.UpdateTime < endUpdateDataQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        IList<int> goodsIdList = idList.ToList();

        IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Id))
            .ToList();

        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Goods.Id))
            .ToList();

        IList<LogisticsGoodsImageInfo> logisticsGoodsImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => goodsIdList.Contains(e.Goods.Id))
            .ToList();

        foreach (int goodsId in goodsIdList) {
            LogisticsGoodsInfo? logisticsGoods = logisticsGoodsList.Where(e => e.Id == goodsId).Take(1).SingleOrDefault();
            if (logisticsGoods == null) {
                throw new ValidationException($"指定的物品不存在！#{goodsId}");
            }

            var orderGoods = expressGoodsList
                .Where(e => e.Goods.Id == goodsId)
                .Take(1)
                .SingleOrDefault();

            if (orderGoods != null) {
                throw new ValidationException($"物品 {logisticsGoods.Name}, 在快递单物品中有被引用，不能删除！");
            }
        }

        foreach (var goodsImage in logisticsGoodsImageList) {
            session.Delete(goodsImage);
        }

        foreach (var logisticsGoods in logisticsGoodsList) {
            session.Delete(logisticsGoods);
        }

        session.Flush();

        foreach (var goodsImage in logisticsGoodsImageList) {
            try {
                logisticsGoodsImageService.DeleteFile(goodsImage);
            }
            catch (Exception exp) {
                log.Error(exp);
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/show/select/hs/code")]
    public IActionResult ShowSelectHsCode() {
        LogisticsGoodsTradeSelectHsCodeModel model = new LogisticsGoodsTradeSelectHsCodeModel();

        return View("LogisticsGoodsTradeSelectHsCode", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/show/select/hs/code/list")]
    public IActionResult ShowSelectHsCodeList(HsGoodsListQueryParam queryParam) {
        int totalCount = GetShowSelectHsCodeListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<HsGoodsInfo> hsGoodsList = GetShowSelectHsCodeListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<HsGoodsInfo>();

        foreach (HsGoodsInfo hsGoods in hsGoodsList) {
            // hsGoods
        }

        HsGoodsListModel model = new HsGoodsListModel(listPager);
        model.HsGoodsList = hsGoodsList;

        return View("LogisticsGoodsTradeSelectHsCodeList", model);
    }

    [NonAction]
    private ICriteria GetShowSelectHsCodeListCriteria(HsGoodsListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<HsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);


        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false) {
            string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<HsGoodsInfo>(e => e.CnHsName).IsInsensitiveLike(cnHsNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/set/no/photo/status")]
    [Transactional]
    public IActionResult SetNoPhotoStatus(int[] idList) {
        IList<int> goodsIdList = idList.ToList();

        if (goodsIdList == null || goodsIdList.Count == 0) {
            throw new ValidationException("请至少选择一条数据！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Id))
            .ToList();

        foreach (int goodsId in goodsIdList) {
            LogisticsGoodsInfo? logisticsGoods = logisticsGoodsList
                .Where(e => e.Id == goodsId)
                .Take(1)
                .SingleOrDefault();

            if (logisticsGoods == null) {
                throw new ValidationException($"指定的物品#{goodsId}不存在！");
            }
        }

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
            logisticsGoods.UpdateUser = loginUser;
            logisticsGoods.Status = LogisticsGoodsStatus.PhotoRequired;
            session.Update(logisticsGoods);
        }

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.SetPhotoRequire, logisticsGoods.Id, loginUserId);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "手工设置需拍照状态成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/import")]
    public IActionResult Import(IFormFile excelFile) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        var loginUser = loginUserService.GetLoginUser();
        logisticsGoodsTradeExcelService.Import(excelFile, loginUser);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/export")]
    public IActionResult Export(TradeGoodsSearchListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.UpdateUserQuery) == false) {
            string updateUserQuery = queryParam.UpdateUserQuery.Trim();
            queryParam.UpdateUserId = session.Query<UserInfo>()
                .Where(e => e.LoginName == updateUserQuery || e.UserName == updateUserQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetTradeGoodsSearchListCriteria(queryParam)
            .List<LogisticsGoodsInfo>();

        string fileName = "确认扩展名" + ".xlsx";
        var excel = logisticsGoodsTradeExcelService.Export(logisticsGoodsList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}