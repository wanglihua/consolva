using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.Logistics.Models.HsGoods;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.HsGoods)]
public class HsGoodsController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly HsGoodsExcelService hsGoodsExcelService;

    public HsGoodsController(ISession session,
        LoginUserService loginUserService,
        HsGoodsExcelService hsGoodsExcelService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.hsGoodsExcelService = hsGoodsExcelService;
    }

    [HttpGet]
    [Route("/admin/hs/goods")]
    public IActionResult Index() {
        HsGoodsIndexModel model = new HsGoodsIndexModel();
        return View("HsGoodsIndex", model);
    }

    [HttpPost]
    [Route("/admin/hs/goods/list")]
    public IActionResult List(HsGoodsListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<HsGoodsInfo> hsGoodsList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(EntityNoUpdate.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<HsGoodsInfo>();

        foreach (HsGoodsInfo hsGoodsInfo in hsGoodsList) {
            // hsGoodsInfo
        }

        HsGoodsListModel model = new HsGoodsListModel(listPager);
        model.HsGoodsList = hsGoodsList;

        return View("HsGoodsList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(HsGoodsListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<HsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false) {
            string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<HsGoodsInfo>(e => e.CnHsName).IsInsensitiveLike(cnHsNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/hs/goods/detail/{id:int?}")]
    public IActionResult Detail(int? id) {
        session.DefaultReadOnly = true;

        HsGoodsInfo? hsGoods;

        if (id == null || id.Value == 0) {
            hsGoods = new HsGoodsInfo();
        } else {
            hsGoods = session.Get<HsGoodsInfo>(id.Value);
            if (hsGoods == null) {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        HsGoodsDetailModel model = new HsGoodsDetailModel();

        model.HsGoodsId = hsGoods.Id;
        model.CnHsCode = hsGoods.CnHsCode;
        model.CnHsName = hsGoods.CnHsName;
        model.EnHsCode = hsGoods.EnHsCode;
        model.EnHsName = hsGoods.EnHsName;
        model.Elem01 = hsGoods.Elem01;
        model.Elem02 = hsGoods.Elem02;
        model.Elem03 = hsGoods.Elem03;
        model.Elem04 = hsGoods.Elem04;
        model.Elem05 = hsGoods.Elem05;
        model.Elem06 = hsGoods.Elem06;
        model.Elem07 = hsGoods.Elem07;
        model.Elem08 = hsGoods.Elem08;

        return View("HsGoodsDetail", model);
    }

    [HttpPost]
    [Route("/admin/hs/goods/save")]
    [Transactional]
    public IActionResult Save(HsGoodsSaveModel model) {
        ValidationUtil.Validate<HsGoodsSaveModelValidator, HsGoodsSaveModel>(model);

        UserInfo loginUser = loginUserService.GetLoginUser();

        int hsGoodsId = model.HsGoodsId;

        bool isCreate = hsGoodsId == 0;

        HsGoodsInfo? hsGoods;

        if (isCreate) {
            hsGoods = new HsGoodsInfo();
            hsGoods.AddUser = loginUser;
        } else {
            hsGoods = session.Get<HsGoodsInfo>(hsGoodsId);
        }

        hsGoods.CnHsCode = model.CnHsCode;
        hsGoods.CnHsName = model.CnHsName;
        hsGoods.EnHsCode = model.EnHsCode;
        hsGoods.EnHsName = model.EnHsName;
        hsGoods.Elem01 = model.Elem01 ?? string.Empty; // 这里的 ?? 必须要加上
        hsGoods.Elem02 = model.Elem02 ?? string.Empty;
        hsGoods.Elem03 = model.Elem03 ?? string.Empty;
        hsGoods.Elem04 = model.Elem04 ?? string.Empty;
        hsGoods.Elem05 = model.Elem05 ?? string.Empty;
        hsGoods.Elem06 = model.Elem06 ?? string.Empty;
        hsGoods.Elem07 = model.Elem07 ?? string.Empty;
        hsGoods.Elem08 = model.Elem08 ?? string.Empty;

        if (isCreate) {
            session.Save(hsGoods);
        } else {
            session.Update(hsGoods);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "isCreate", isCreate },
            { "id", hsGoods.Id }
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (int id in idList) {
            HsGoodsInfo? hsGoods = session.Get<HsGoodsInfo>(id);

            int count = session.Query<LogisticsGoodsInfo>().Where(e => e.HsGoods!.Id == id).Count();
            if (count != 0) {
                throw new ValidationException($"海关品名 {hsGoods.CnHsName} 已在扩展名中使用中，不能删除！");
            }

            session.Delete(hsGoods);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/detail/data/{id:int}")]
    public IActionResult DetailData(int id) {
        HsGoodsInfo? hsGoodsInfo = session.Get<HsGoodsInfo>(id);
        if (hsGoodsInfo == null) {
            throw new ValidationException("指定的海关物品不存在！");
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取成功！" },
            { "hsGoods", hsGoodsInfo },
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/import")]
    public IActionResult Import(IFormFile excelFile) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        var loginUser = loginUserService.GetLoginUser();

        hsGoodsExcelService.Import(excelFile, loginUser);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/export")]
    public IActionResult Export(HsGoodsListQueryParam queryParam) {
        var query = session
            .Query<HsGoodsInfo>();

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false) {
            query = query.Where(e => e.CnHsName.Contains(queryParam.CnHsNameQuery));
        }

        IList<HsGoodsInfo> hsGoodsList = query
            .OrderBy(e => e.CnHsName)
            .ToList();

        string fileName = "海关品名" + ".xlsx";
        var excel = hsGoodsExcelService.Export(hsGoodsList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}