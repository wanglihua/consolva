using shunda.Core.Service;

namespace shunda.Service.Wechat;

/// <summary>
/// 微信session有关的逻辑
/// todo 这里只是做个空壳，后面补上
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatSessionService
{
    public WechatSessionService()
    {
    }

    public void SetOpenId(string openId)
    {
    }

    public string GetOpenId()
    {
        return "open-id-test-0001";
    }

    public void BindOpenIdToCustomerAndValidatePromotionNewThread(int customerId)
    {
    }
}