using shunda.Core.Service;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class MobileService
{
    /// <summary>
    /// 是不是正确的电话号码格式
    /// </summary>
    /// <param name="mobile">带有国家代号的电话号码格式</param>
    /// <returns></returns>
    public bool IsCorrectMobile(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            return false;
        }
        
        // todo b 完成电话号码格式检查逻辑

        return true;
    }
}