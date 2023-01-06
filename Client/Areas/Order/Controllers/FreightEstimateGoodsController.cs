using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using shunda.Service.LogisticsOrder;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class FreightEstimateGoodsController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;

    public FreightEstimateGoodsController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
    }
}