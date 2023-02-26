using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Areas.Customer.Models.CustomerSupperPassword;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.SupperPassword)]
public class CustomerSupperPasswordController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    private readonly CustomerCodeService customerCodeService;
    private readonly CustomerSupperPasswordService customerSupperPasswordService;

    private readonly ISession session;

    public CustomerSupperPasswordController(
        ISession session,
        CustomerSupperPasswordService customerSupperPasswordService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerSupperPasswordService = customerSupperPasswordService;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/supper/password")]
    public IActionResult Index()
    {
        var model = new CustomerSupperPasswordIndexModel();
        model.PasswordExpireMinutes = customerSupperPasswordService.GetPasswordExpireMinutes();

        return View("CustomerSupperPasswordIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/supper/password/generate")]
    [Transactional]
    public IActionResult Generate(string customerCode)
    {
        if (string.IsNullOrWhiteSpace(customerCode)) throw new ValidationException("客户ID不能为空！");

        var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) throw new ValidationException($"指定的 客户ID#{customerCode} 不存在！");

        var supperPassword = customerSupperPasswordService.GenerateNewPassword(customerId);

        var customerDescription = $"客户ID：{customerCodeService.GetCustomerCode(customerId)}";

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "超级密码生成成功！" },
            { "password", supperPassword },
            { "customerDescription", customerDescription }
        });
    }
}