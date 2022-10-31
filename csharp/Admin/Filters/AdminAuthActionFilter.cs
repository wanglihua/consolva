using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service;

namespace shunda.Admin.Filters;

public class AdminAuthActionFilter : IActionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly LoginUserService loginUserService;

    public AdminAuthActionFilter(LoginUserService loginUserService)
    {
        this.loginUserService = loginUserService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("AdminAuthActionFilter OnActionExecuting");

        ControllerActionDescriptor? actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor == null)
        {
            return;
        }

        #region 登录判断

        // 如果有 NoLoginRequiredAttribute 标记，就不用进行登录检查
        object[] controllerNoLoginRequiredAttributes = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(NoLoginRequiredAttribute), true);
        if ((controllerNoLoginRequiredAttributes?.Length ?? 0) != 0)
        {
            return;
        }

        object[] actionNoLoginRequiredAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(NoLoginRequiredAttribute), true);
        if ((actionNoLoginRequiredAttributes?.Length ?? 0) != 0)
        {
            return;
        }

        // 如果 没有 NoLoginRequiredAttribute 标记， 并且用户没登录，就提示未登录错误
        if (this.loginUserService.IsLogined == false)
        {
            ShowLogin(context);
            return;
        }

        #endregion

        #region 权限判断

        // Controller 类上设置的 possResId 中有一个是已授权的，就是 Controller 级别已授权。就是这里的各个 passResId 是 "或 or" 的关系
        // Action 方法上设置的 passResId 中的有一个是已授权的，就是 Action 级别已授权。就是这里的各个 passResId 是 "或 or" 的关系
        // Controller 级别（如果有）已授权 和 Action 级别（如果有）已授权，同时满足，这个Action才能正常访问。就是 Controller级别 和 Action级别 是 "且 and" 的关系

        object[] controllerAdminAuthAttributes = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AdminAuthAttribute), true);
        object[] actionAdminAuthAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AdminAuthAttribute), true);

        bool isAdmin = this.loginUserService.IsAdmin;

        // 如果当前是管理员登录，接下来的权限判断也就用不着了
        if (isAdmin)
        {
            return;
        }

        Dictionary<int, bool> passResIdMap = this.loginUserService.GetPassResIdMap();

        // 下面的两个循环，表名Controller 和 Action 上的PassRes是并且的关系

        foreach (object adminAuthAttributeObj in controllerAdminAuthAttributes)
        {
            if (adminAuthAttributeObj is AdminAuthAttribute adminAuthAttribute)
            {
                // 判断是否是管理员登录
                // if ((adminAuthAttribute.AdminOnly && isAdmin == false))
                if (adminAuthAttribute.AdminOnly) // isAdmin false的是不会走到这一步的，前面已经有判断
                {
                    ShowLogin(context);
                    return;
                }

                // 判断是否是有标记的权限
                // adminAuthAttribute.PassResIdList 中只要有一个授权了，就是可以访问标注的 Controller 或 Action 了
                if (adminAuthAttribute.PassResIdList.Count != 0 && adminAuthAttribute.PassResIdList.Where(e => passResIdMap.ContainsKey(e)).Count() == 0)
                {
                    ShowLogin(context);
                    return;
                }
            }
        }

        foreach (object adminAuthAttributeObj in actionAdminAuthAttributes)
        {
            if (adminAuthAttributeObj is AdminAuthAttribute adminAuthAttribute)
            {
                // 判断是否是管理员登录
                // if ((adminAuthAttribute.AdminOnly && isAdmin == false))
                if (adminAuthAttribute.AdminOnly) // isAdmin false的是不会走到这一步的，前面已经有判断
                {
                    ShowLogin(context);
                    return;
                }

                // 判断是否是有标记的权限
                // adminAuthAttribute.PassResIdList 中只要有一个授权了，就是可以访问标注的 Controller 或 Action 了
                if (adminAuthAttribute.PassResIdList.Count != 0 && adminAuthAttribute.PassResIdList.Where(e => passResIdMap.ContainsKey(e)).Count() == 0)
                {
                    ShowLogin(context);
                    return;
                }
            }
        }

        #endregion
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("AdminAuthActionFilter OnActionExecuted");
    }

    private void ShowLogin(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        if (WebUtil.ShouldShowJson(request))
        {
            context.Result = new JsonResult(new {
                success = false,
                message = "用户未登录，或登录会话过期，请重新登录！",
            });
        }
        else
        {
            context.Result = new RedirectResult(loginUserService.GetUserLoginUrl());
        }
    }
}