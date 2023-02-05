#nullable disable

using System.Dynamic;
using System.Linq.Expressions;

namespace shunda.Core.Web;

/// <summary>
/// 从方法调用表达式中获取 route values，主要配合 ActionUrlExtension 等的使用
/// </summary>
internal class RouteValueExtractor {
    public static object GetRouteValues(MethodCallExpression call, string areaName) {
        var routes = new Dictionary<string, object>();
        var parameters = call.Method.GetParameters();
        var pairs = call.Arguments.Select((a, i) => new {
            Argument = a,
            ParamName = parameters[i].Name
        });

        foreach (var item in pairs) {
            string name = item.ParamName;
            object value = GetValue(item.Argument);
            if (value != null) {
                var valueType = value.GetType();
                if (valueType.IsValueType) {
                    routes.Add(name, value);
                } else {
                    throw new NotSupportedException("Unsupported parameter type {0}");
                }
            }
        }

        routes["Area"] = areaName;

        return DictionaryToObject(routes);
    }

    private static object GetValue(Expression expression) {
        if (expression.NodeType == ExpressionType.Constant) {
            return ((ConstantExpression)expression).Value;
        }

        throw new NotSupportedException("Unsupported parameter expression");
    }

    private static dynamic DictionaryToObject(IDictionary<string, object> dictionary) {
        var expandoObj = new ExpandoObject();
        var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)expandoObj;
        foreach (var keyValuePair in dictionary) {
            expandoObjCollection.Add(keyValuePair);
        }

        dynamic eoDynamic = expandoObj;
        return eoDynamic;
    }
}