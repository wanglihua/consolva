#nullable disable

using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Core.Web;

/// <summary>
/// 可以实现类似 @(Url.Action&lt;AdminController&gt;(c => c.Index())) 的 Url生成方式
/// </summary>
public static class ActionUrlExtensions {
    public static string Action<TController>(this IUrlHelper helper, Expression<Action<TController>> action) where TController : Controller {
        return Action<TController>(helper, action, string.Empty);
    }

    public static string Action<TController>(this IUrlHelper helper, Expression<Action<TController>> action, string areaName) where TController : Controller {
        var controllerName = GetControllerName(typeof(TController));
        var methodCallExpression = GetMethodCallExpression(action);
        var actionName = methodCallExpression.Method.Name;
        var routes = RouteValueExtractor.GetRouteValues(methodCallExpression, areaName);
        var link = helper.Action(action: actionName, controller: controllerName, values: routes);
        return link;
    }

    private static string GetControllerName(Type controllerType) {
        var controllerName = controllerType.Name;
        if (controllerType.Name.EndsWith("Controller")) {
            var totalLength = controllerType.Name.Length;
            var suffixLength = "Controller".Length;
            controllerName = controllerType.Name.Substring(0, totalLength - suffixLength);
        }

        return controllerName;
    }

    private static MethodCallExpression GetMethodCallExpression<TController>(Expression<Action<TController>> actionSelector) {
        var call = actionSelector.Body as MethodCallExpression;
        if (call == null) {
            throw new ArgumentException("You must call a method on " + typeof(TController).Name, "actionSelector");
        }

        return call;
    }
}
