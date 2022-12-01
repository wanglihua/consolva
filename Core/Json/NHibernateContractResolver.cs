using System.Reflection;
using Newtonsoft.Json.Serialization;
using NHibernate.Proxy;

namespace shunda.Core.Json;

/// <summary>
/// 解决 NHibernate 序列化成 Json 的时候的 Proxy 的处理问题
/// 这个类基于 CamelCasePropertyNamesContractResolver （会改成 camel case，这也是默认的方式）， 也可以基于 DefaultContractResolver（不改变大小写）
/// </summary>
public class NHibernateContractResolver : CamelCasePropertyNamesContractResolver
{
    protected override JsonContract CreateContract(Type objectType)
    {
        if (typeof(INHibernateProxy).IsAssignableFrom(objectType))
        {
            return base.CreateContract(objectType.BaseType!);
        }
        else
        {
            return base.CreateContract(objectType);
        }
    }

    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        if (typeof(INHibernateProxy).IsAssignableFrom(objectType))
        {
            return base.GetSerializableMembers(objectType.BaseType!);
        }
        else
        {
            return base.GetSerializableMembers(objectType);
        }
    }
}