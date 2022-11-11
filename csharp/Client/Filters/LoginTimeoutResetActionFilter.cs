using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace shunda.Client.Filters;

public class LoginTimeoutResetActionFilter : IActionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    // todo 加上 loginCustomer的逻辑
    // private readonly LoginUserService loginUserService;
    //
    // public LoginTimeoutResetActionFilter(LoginUserService loginUserService)
    // {
    //     this.loginUserService = loginUserService;
    // }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("LoginTimeoutResetActionFilter.OnActionExecuting");

        // loginUserService.ResetLoginTimeout();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("LoginTimeoutResetActionFilter.OnActionExecuted");
    }
}