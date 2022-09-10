namespace shunda.Core;

/// <summary>
/// 这个异常一般是业务逻辑验证错误
/// 这个异常不会被记录到日志
/// 这个异常可能会造成事务回滚
/// </summary>
public class ValidationException : Exception
{
    public ValidationException()
    {
    }

    public ValidationException(string message) : base(message)
    {
    }
}