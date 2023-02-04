using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailFeeController : Controller {
    private readonly ISession session;

    public LogisticsOrderDetailFeeController(ISession session) {
        this.session = session;
    }
    
    
}