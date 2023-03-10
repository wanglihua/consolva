using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerVipDiscount)]
public class CustomerVipDiscountController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly LoginUserService loginUserService;
    private readonly CustomerVipDiscountExcelService customerVipDiscountExcelService;

    private readonly ISession session;

    public CustomerVipDiscountController(ISession session,
        LoginUserService loginUserService,
        CustomerVipDiscountExcelService customerVipDiscountExcelService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.customerVipDiscountExcelService = customerVipDiscountExcelService;
    }

    [HttpGet]
    [Route("/admin/customer/vip/discount")]
    public IActionResult Index() {
        var model = new CustomerVipDiscountIndexModel();
        return View("CustomerVipDiscountIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/list")]
    public IActionResult List(CustomerVipDiscountListQueryParam queryParam) {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerVipDiscountList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerVipDiscountInfo>();

        var model = new CustomerVipDiscountListModel(listPager);
        model.CustomerVipDiscountList = customerVipDiscountList;

        return View("CustomerVipDiscountList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerVipDiscountListQueryParam queryParam) {
        var criteria = session.CreateCriteria<CustomerVipDiscountInfo>();

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/detail/{id:int?}")]
    public IActionResult Detail(int? id) {
        session.DefaultReadOnly = true;

        CustomerVipDiscountInfo? customerVipDiscount;

        var isCreate = false;
        if (id == null || id.Value == 0) {
            isCreate = true;

            customerVipDiscount = new CustomerVipDiscountInfo();
        } else {
            isCreate = false;

            customerVipDiscount = session.Get<CustomerVipDiscountInfo>(id.Value);
            if (customerVipDiscount == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new CustomerVipDiscountDetailModel();
        model.CustomerVipDiscountId = customerVipDiscount.Id;
        model.Rank = customerVipDiscount.Rank;
        model.Score = customerVipDiscount.Score;
        model.Discount = customerVipDiscount.Discount;
        model.CanCoupon = customerVipDiscount.CanCoupon;

        model.IsCreate = isCreate;

        return View("CustomerVipDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/save")]
    [Transactional]
    public IActionResult Save(CustomerVipDiscountSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        var rank = model.Rank;

        if (rank == null || rank == CustomerVipRank.None) throw new ValidationException("指定的客户VIP级别不存在！");

        var loginUser = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        var customerVipDiscountId = model.CustomerVipDiscountId;

        var isCreate = customerVipDiscountId == 0;

        CustomerVipDiscountInfo? customerVipDiscount;

        if (isCreate) {
            var vipDiscount = session.Query<CustomerVipDiscountInfo>()
                .Where(e => e.Rank == rank)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscount != null) throw new ValidationException($"指定的客户VIP级别#{rank.Name}折扣设置已经存在！");

            customerVipDiscount = new CustomerVipDiscountInfo();
            customerVipDiscount.Rank = model.Rank; // 编辑时这个级别不能修改，所以只在新建时设置保存
        } else {
            var vipDiscount = session.Query<CustomerVipDiscountInfo>()
                .Where(e => e.Rank == rank && e.Id != customerVipDiscountId)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscount != null) throw new ValidationException($"指定的客户VIP级别#{rank.Name}折扣设置已经存在！");

            customerVipDiscount = session.Get<CustomerVipDiscountInfo>(customerVipDiscountId);
        }

        customerVipDiscount.Score = model.Score;
        customerVipDiscount.Discount = model.Discount;
        customerVipDiscount.CanCoupon = model.CanCoupon;
        customerVipDiscount.User = loginUser;

        if (isCreate)
            session.Save(customerVipDiscount);
        else
            session.Update(customerVipDiscount);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (var id in idList) {
            var customerVipDiscount = session.Load<CustomerVipDiscountInfo>(id);
            session.Delete(customerVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/export")]
    public IActionResult Export() {
        var fileName = "VIP折扣设置.xlsx";
        var excel = customerVipDiscountExcelService.Export();
        return File(excel, ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/import")]
    public IActionResult Import(IFormFile excelFile) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        UserInfo loginUser = loginUserService.GetLoginUser();
        customerVipDiscountExcelService.Import(excelFile, loginUser);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "导入成功！" }
        });
    }
}