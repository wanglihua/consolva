using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

// todo 判断是不是当前登录人的订单

[OrderArea]
public class OrderDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public OrderDetailController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }
}