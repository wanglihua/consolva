using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NLog;

#nullable disable

namespace shunda.Core.Web;

/// <summary>
/// @Html.Action()
///
/// 1、在Startup中的 ConfigureServices 方法添加：
///     services.AddSingleton&lt;IHttpContextAccessor, HttpContextAccessor&gt;();
///     services.AddSingleton&lt;IActionContextAccessor, ActionContextAccessor&gt;();
///    （备注：因为net core 默认不将IHttpContextAccessor，IActionContextAccessor 依赖注入，所以需要手动进行依赖注入）
/// 2、在页面中，我们就可以直接用 @Html.Action()方法 直接请求 控制器的方法了。
/// 3、小结：如今微软官方已经提供了 新的 TagHelper、View components 来替代之前的写法。这个教程只适用于对视图层不想做更变的小伙伴使用。如果 是新项目的，还是建议使用 View components!
///
/// </summary>
public static class HtmlHelperViewExtensions {
    private static Logger log = LogManager.GetCurrentClassLogger();

    public static IHtmlContent Action(this IHtmlHelper helper, string action, object parameters = null) {
        var controller = (string)helper.ViewContext.RouteData.Values["controller"];

        return Action(helper, action, controller, parameters);
    }

    public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, object parameters = null) {
        var area = (string)helper.ViewContext.RouteData.Values["area"];

        return Action(helper, action, controller, area, parameters);
    }

    public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, string area, object parameters = null) {
        if (action == null) {
            throw new ArgumentNullException("action");
        }

        if (controller == null) {
            throw new ArgumentNullException("controller");
        }


        var task = RenderActionAsync(helper, action, controller, area, parameters);

        return task.Result;
    }

    private static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action, string controller, string area, object parameters = null) {
        // fetching required services for invocation
        var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
        var actionContextAccessor = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();
        var httpContextAccessor = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
        // var actionSelector = serviceProvider.GetRequiredService<IActionSelector>();
        var actionSelector = serviceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();

        // IActionDescriptorCollectionProvider

        // creating new action invocation context
        var routeData = new RouteData();
        foreach (var router in helper.ViewContext.RouteData.Routers) {
            routeData.PushState(router, null, null);
        }

        routeData.PushState(null, new RouteValueDictionary(new { controller = controller, action = action, area = area }), null);
        routeData.PushState(null, new RouteValueDictionary(parameters ?? new { }), null);

        //get the actiondescriptor
        RouteContext routeContext = new RouteContext(helper.ViewContext.HttpContext) { RouteData = routeData };
        // var candidates = actionSelector.SelectCandidates(routeContext);
        // var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);
        var actionDescriptor = actionSelector.ActionDescriptors.Items.Where(i => i.RouteValues["Controller"] == controller && i.RouteValues["Action"] == action).First();

        var originalActionContext = actionContextAccessor.ActionContext;
        var originalHttpContext = httpContextAccessor.HttpContext;
        try {
            var newHttpContext = serviceProvider.GetRequiredService<IHttpContextFactory>().Create(helper.ViewContext.HttpContext.Features);
            if (newHttpContext.Items.ContainsKey(typeof(IUrlHelper))) {
                newHttpContext.Items.Remove(typeof(IUrlHelper));
            }

            newHttpContext.Response.Body = new MemoryStream();
            var actionContext = new ActionContext(newHttpContext, routeData, actionDescriptor);
            actionContextAccessor.ActionContext = actionContext;
            var invoker = serviceProvider.GetRequiredService<IActionInvokerFactory>().CreateInvoker(actionContext);
            await invoker.InvokeAsync();
            newHttpContext.Response.Body.Position = 0;
            using (var reader = new StreamReader(newHttpContext.Response.Body)) {
                return new HtmlString(reader.ReadToEnd());
            }
        }
        catch (Exception exp) {
            log.Error(exp);

            return new HtmlString(exp.Message);
        }
        finally {
            actionContextAccessor.ActionContext = originalActionContext;
            httpContextAccessor.HttpContext = originalHttpContext;
            if (helper.ViewContext.HttpContext.Items.ContainsKey(typeof(IUrlHelper))) {
                helper.ViewContext.HttpContext.Items.Remove(typeof(IUrlHelper));
            }
        }
    }
}