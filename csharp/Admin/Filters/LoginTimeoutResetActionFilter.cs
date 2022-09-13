using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace shunda.Admin.Filters;

/*

public class LoginTimeoutResetActionFilter : IActionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private LoginCustomerService loginCustomerService;
    private LoginUserService loginUserService;

    public LoginTimeoutResetActionFilter(
        LoginCustomerService loginCustomerService,
        LoginUserService loginUserService)
    {
        this.loginCustomerService = loginCustomerService;
        this.loginUserService = loginUserService;
    }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("LoginTimeoutResetActionFilter.OnActionExecuting");

        loginCustomerService.ResetLoginTimeout();
        loginUserService.ResetLoginTimeout();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("LoginTimeoutResetActionFilter.OnActionExecuted");
    }
}

*/