using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace shunda.Core.Web;

/// <summary>
/// 可以实现类似 @(Url.Action&lt;AdminController&gt;(c => c.Index())) 的 Url生成方式
/// </summary>
public static class ActionUrlExtensions
{
    public static string Action<TController>(this IUrlHelper helper, Expression<Action<TController>> action) where TController : Controller
    {
        var controllerName = GetControllerName(typeof(TController));
        var methodCallExpression = GetMethodCallExpression(action);
        var actionName = methodCallExpression.Method.Name;
        var routes = RouteValueExtractor.GetRouteValues(methodCallExpression);
        var link = helper.Action(action: actionName, controller: controllerName, values: routes);
        return link;
    }

    private static string GetControllerName(Type controllerType)
    {
        var controllerName = controllerType.Name;
        if (controllerType.Name.EndsWith("Controller"))
        {
            var totalLength = controllerType.Name.Length;
            var suffixLength = "Controller".Length;
            controllerName = controllerType.Name.Substring(0, totalLength - suffixLength);
        }

        return controllerName;
    }

    private static MethodCallExpression GetMethodCallExpression<TController>(Expression<Action<TController>> actionSelector)
    {
        var call = actionSelector.Body as MethodCallExpression;
        if (call == null)
        {
            throw new ArgumentException("You must call a method on " + typeof(TController).Name, "actionSelector");
        }

        return call;
    }
}

/// <summary>
/// 从方法调用表达式中获取 route values，主要配合 ActionUrlExtension 的使用
/// </summary>
internal class RouteValueExtractor
{
    public static object GetRouteValues(MethodCallExpression call)
    {
        var routes = new Dictionary<string, object>();
        var parameters = call.Method.GetParameters();
        var pairs = call.Arguments.Select((a, i) => new {
            Argument = a,
            ParamName = parameters[i].Name
        });

        foreach (var item in pairs)
        {
            string name = item.ParamName;
            object value = GetValue(item.Argument);
            if (value != null)
            {
                var valueType = value.GetType();
                if (valueType.IsValueType)
                {
                    routes.Add(name, value);
                }
                else
                {
                    throw new NotSupportedException("Unsupported parameter type {0}");
                }
            }
        }

        return DictionaryToObject(routes);
    }

    private static object GetValue(Expression expression)
    {
        if (expression.NodeType == ExpressionType.Constant)
        {
            return ((ConstantExpression)expression).Value;
        }

        throw new NotSupportedException("Unsupported parameter expression");
    }

    private static dynamic DictionaryToObject(IDictionary<string, object> dictionary)
    {
        var expandoObj = new ExpandoObject();
        var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)expandoObj;
        foreach (var keyValuePair in dictionary)
        {
            expandoObjCollection.Add(keyValuePair);
        }

        dynamic eoDynamic = expandoObj;
        return eoDynamic;
    }
}