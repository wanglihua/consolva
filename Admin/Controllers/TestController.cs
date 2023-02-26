using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public TestController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/test")]
    [Transactional]
    public IActionResult Index() {
        var couponList = session.Query<CouponInfo>()
            .Fetch(e => e.Batch)
            .Where(e => e.Customer.Id == 1
                        && e.Status == CouponStatus.Pick)
            .Select(e => new {
                CouponId = e.Id,
                Code = e.Code,
                BatchId = e.Batch.Id,
                Expire = e.Batch.Expire,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}