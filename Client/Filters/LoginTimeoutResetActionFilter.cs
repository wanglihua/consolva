using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using shunda.Client.Services;

namespace shunda.Client.Filters;

public class LoginTimeoutResetActionFilter : IActionFilter
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly LoginCustomerService loginCustomerService;
    
    public LoginTimeoutResetActionFilter(LoginCustomerService loginCustomerService)
    {
        this.loginCustomerService = loginCustomerService;
    }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        log.Trace("LoginTimeoutResetActionFilter.OnActionExecuting");

        loginCustomerService.ResetLoginTimeout();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        log.Trace("LoginTimeoutResetActionFilter.OnActionExecuted");
    }
}