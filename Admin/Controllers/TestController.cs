using System.Globalization;
using CSRedis;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Redis;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Customer;
using shunda.Service.Wechat.WechatApp;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

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

        var customer = session.Get<CustomerInfo>(1);
        if (customer != null) {
            customer.LastLogin = DateTime.Now;
            session.Update(customer);
        
            session.Flush();
        }

        session.Query<CustomerInfo>()
            .Where(e => e.Id == 1)
            .UpdateBuilder()
            .Set(e => e.LastLogin, DateTime.Now)
            .Update();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}