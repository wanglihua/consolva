using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using shunda.Service;

namespace shunda.Admin.Filters;

public class LoginTimeoutResetActionFilter : IActionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private LoginUserService loginUserService;

    public LoginTimeoutResetActionFilter(LoginUserService loginUserService)
    {
        this.loginUserService = loginUserService;
    }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("LoginTimeoutResetActionFilter.OnActionExecuting");

        loginUserService.ResetLoginTimeout();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("LoginTimeoutResetActionFilter.OnActionExecuted");
    }
}
