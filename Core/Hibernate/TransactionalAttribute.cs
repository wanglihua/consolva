namespace shunda.Core.Hibernate;

/// <summary>
/// 用来标记某个Action是不是启用事务
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class TransactionalAttribute : Attribute
{
}