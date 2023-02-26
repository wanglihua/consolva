using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using shunda.Client.Services;
using shunda.Core.Web;

namespace shunda.Client.Filters;

public class ClientAuthActionFilter : IActionFilter {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly LoginCustomerService loginCustomerService;

    public ClientAuthActionFilter(LoginCustomerService loginCustomerService) {
        this.loginCustomerService = loginCustomerService;
    }

    public void OnActionExecuting(ActionExecutingContext context) {
        // log.Trace("AdminAuthActionFilter OnActionExecuting");

        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor == null) return;

        #region 登录判断

        // 如果有 NoLoginRequiredAttribute 标记，就不用进行登录检查
        var controllerNoLoginRequiredAttributes = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(NoLoginRequiredAttribute), true);
        if ((controllerNoLoginRequiredAttributes?.Length ?? 0) != 0) return;

        var actionNoLoginRequiredAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(NoLoginRequiredAttribute), true);
        if ((actionNoLoginRequiredAttributes?.Length ?? 0) != 0) return;

        // 如果 没有 NoLoginRequiredAttribute 标记， 并且用户没登录，就提示未登录错误
        if (loginCustomerService.IsLogined == false) {
            ShowLogin(context);
            return;
        }

        #endregion
    }

    public void OnActionExecuted(ActionExecutedContext context) {
        // log.Trace("AdminAuthActionFilter OnActionExecuted");
    }

    private void ShowLogin(ActionExecutingContext context) {
        context.Result = new JsonResult(new {
            success = false,
            message = "用户未登录，或登录会话过期，请重新登录！"
        });
    }
}