using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Customer.Models.CustomerSuperVipDiscount;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerSuperVipDiscount)]
public class CustomerSuperVipDiscountController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly CustomerCodeService customerCodeService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerSuperVipDiscountExcelService customerSuperVipDiscountExcelService;

    private readonly ISession session;

    public CustomerSuperVipDiscountController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        CustomerSuperVipDiscountExcelService customerSuperVipDiscountExcelService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.customerSuperVipDiscountExcelService = customerSuperVipDiscountExcelService;
    }

    [HttpGet]
    [Route("/admin/customer/super/vip/discount")]
    public IActionResult Index() {
        var model = new CustomerSuperVipDiscountIndexModel();
        return View("CustomerSuperVipDiscountIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/list")]
    public IActionResult List(CustomerSuperVipDiscountListQueryParam queryParam) {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerSuperVipDiscountList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerSuperVipDiscountInfo>();

        foreach (var customerSuperVipDiscount in customerSuperVipDiscountList) customerSuperVipDiscount.CustomerCodeCache = customerCodeService.GetCustomerCode(customerSuperVipDiscount.Customer?.Id ?? 0);

        var model = new CustomerSuperVipDiscountListModel(listPager);
        model.CustomerSuperVipDiscountList = customerSuperVipDiscountList;

        return View("CustomerSuperVipDiscountList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerSuperVipDiscountListQueryParam queryParam) {
        var criteria = session.CreateCriteria<CustomerSuperVipDiscountInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCode) == false) {
            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCode);
            criteria = criteria.Add(Restrictions.Where<CustomerSuperVipDiscountInfo>(e => e.Customer.Id == customerId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/detail/{id:int?}")]
    public IActionResult Detail(int? id) {
        session.DefaultReadOnly = true;

        CustomerSuperVipDiscountInfo? customerSuperVipDiscount;

        var model = new CustomerSuperVipDiscountDetailModel();

        if (id == null || id.Value == 0) {
            customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
        } else {
            customerSuperVipDiscount = session.Get<CustomerSuperVipDiscountInfo>(id.Value);
            if (customerSuperVipDiscount == null) throw new ValidationException("指定的数据项不存在！");

            customerSuperVipDiscount.CustomerCodeCache = customerCodeService.GetCustomerCode(customerSuperVipDiscount.Customer?.Id ?? 0);
        }

        model.CustomerSuperVipDiscount = customerSuperVipDiscount;

        return View("CustomerSuperVipDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/save")]
    public IActionResult Save(CustomerSuperVipDiscountDetailModel model) {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var customerSuperVipDiscountFromUi = model.CustomerSuperVipDiscount;
        var customerSuperVipDiscountId = customerSuperVipDiscountFromUi.Id;

        var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerSuperVipDiscountFromUi.CustomerCodeCache);
        if (customerId == 0) throw new ValidationException("指定的客户ID格式不对或客户ID未指定！");

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) throw new ValidationException("指定的客户不存在！");

        var isCreate = customerSuperVipDiscountId == 0;

        CustomerSuperVipDiscountInfo? customerSuperVipDiscount;

        if (isCreate) {
            var customerSuperVipDiscountExist = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == customerId)
                .Take(1)
                .SingleOrDefault();

            if (customerSuperVipDiscountExist != null) throw new ValidationException($"指定的客户#{customerId}超级VIP设置已存在！");

            customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
        } else {
            var customerSuperVipDiscountExist = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == customerId && e.Id != customerSuperVipDiscountId)
                .Take(1)
                .SingleOrDefault();

            if (customerSuperVipDiscountExist != null) throw new ValidationException($"指定的客户#{customerId}超级VIP设置已存在！");

            customerSuperVipDiscount = session.Get<CustomerSuperVipDiscountInfo>(customerSuperVipDiscountId);
        }

        customerSuperVipDiscount.Customer = customer;
        customerSuperVipDiscount.Description = model.CustomerSuperVipDiscount.Description;
        customerSuperVipDiscount.Discount = customerSuperVipDiscountFromUi.Discount;
        customerSuperVipDiscount.User = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        if (isCreate)
            session.Save(customerSuperVipDiscount);
        else
            session.Update(customerSuperVipDiscount);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (var id in idList) {
            var customerSuperVipDiscount = session.Load<CustomerSuperVipDiscountInfo>(id);
            session.Delete(customerSuperVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/export")]
    public IActionResult Export() {
        var fileName = "超级VIP折扣.xlsx";
        byte[] excel = customerSuperVipDiscountExcelService.Export();
        return File(excel, ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/import")]
    [Transactional]
    public IActionResult Import(IFormFile excelFile) {
        UserInfo loginUser = loginUserService.GetLoginUser();
        customerSuperVipDiscountExcelService.Import(excelFile, loginUser);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "导入成功！" }
        });
    }
}