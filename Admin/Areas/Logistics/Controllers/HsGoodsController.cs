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
        model.HsGoods = hsGoods;

        return View("HsGoodsDetail", model);
    }

    [HttpPost]
    [Route("/admin/hs/goods/save")]
    [Transactional]
    public IActionResult Save(HsGoodsDetailModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        UserInfo loginUser = loginUserService.GetLoginUser();

        HsGoodsInfo hsGoodsFromUi = model.HsGoods;
        int hsGoodsIdFromUi = hsGoodsFromUi.Id;

        bool isCreate = hsGoodsIdFromUi == 0;

        HsGoodsInfo? hsGoods;

        if (isCreate) {
            hsGoods = new HsGoodsInfo();
            hsGoods.AddUser = loginUser;
        } else {
            hsGoods = session.Get<HsGoodsInfo>(hsGoodsIdFromUi);
        }

        hsGoods.CnHsCode = hsGoodsFromUi.CnHsCode;
        hsGoods.CnHsName = hsGoodsFromUi.CnHsName;
        hsGoods.EnHsCode = hsGoodsFromUi.EnHsCode;
        hsGoods.EnHsName = hsGoodsFromUi.EnHsName;
        hsGoods.Elem01 = hsGoodsFromUi.Elem01;
        hsGoods.Elem02 = hsGoodsFromUi.Elem02;
        hsGoods.Elem03 = hsGoodsFromUi.Elem03;
        hsGoods.Elem04 = hsGoodsFromUi.Elem04;
        hsGoods.Elem05 = hsGoodsFromUi.Elem05;
        hsGoods.Elem06 = hsGoodsFromUi.Elem06;
        hsGoods.Elem07 = hsGoodsFromUi.Elem07;
        hsGoods.Elem08 = hsGoodsFromUi.Elem08;

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