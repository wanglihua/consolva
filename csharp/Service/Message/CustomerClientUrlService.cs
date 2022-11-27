using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Customer;

namespace shunda.Service.Message;

/// <summary>
/// todo a 有些不用的去掉，在使用的URL需要确认，是否是真的使用
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerClientUrlService
{
    public string GetAutoLoginUrl(string simpleClientUrl)
    {
        return SiteConfig.SiteRoot + "/wei/xin/login/2?url=" + SiteConfig.SiteRoot + simpleClientUrl;
    }

    public string GetCustomerPromotionUrl()
    {
        string url = $"/mobile/customer/promotion";
        return GetAutoLoginUrl(url);
    }

    public string GetWaitTransportOrderDetailUrl(CustomerInfo customer, int orderId)
    {
        string url = $"/mobile/order/detail/waiting-transport/{orderId}";
        return GetAutoLoginUrl(url);
    }

    public string GetTransportingOrderDetailUrl(CustomerInfo customer, int orderId)
    {
        string url = $"/mobile/order/detail/transporting/{orderId}";
        return GetAutoLoginUrl(url);
    }

    public string GetDeliveredOrderDetailUrl(CustomerInfo customer, int orderId)
    {
        string url = $"/mobile/order/detail/delivered/{orderId}";
        return GetAutoLoginUrl(url);
    }

    public string GetHomeDeliverBatchConfirmUrl(CustomerInfo customer, int entryId)
    {
        string url = $"/mobile/home/deliver/batch/confirm/{entryId}";
        return GetAutoLoginUrl(url);
    }

    public string GetCustomerScoreUrl(CustomerInfo customer)
    {
        string url = $"/mobile/score";
        return GetAutoLoginUrl(url);
    }

    public string GetCustomerAccountUrl(CustomerInfo customer)
    {
        string url = $"/mobile/account";
        return GetAutoLoginUrl(url);
    }

    public string GetExpressCheckinListUrl(CustomerInfo customer)
    {
        string url = $"/mobile/express/checkin/list";
        return GetAutoLoginUrl(url);
    }

    public string GetExpressCheckinDetailUrl(CustomerInfo customer, int claimId)
    {
        string url = $"/mobile/express/checkin/detail/{claimId}";
        return GetAutoLoginUrl(url);
    }

    public string GetExpressClaimListUrl(CustomerInfo customer)
    {
        string url = "/mobile/express/claim/list";
        return GetAutoLoginUrl(url);
    }

    public string GetExpressExpectListUrl(CustomerInfo customer)
    {
        string url = "/mobile/express/expect/list";
        return GetAutoLoginUrl(url);
    }

    public string GetChatUrl(CustomerInfo customer)
    {
        string url = "/mobile/chat";
        return GetAutoLoginUrl(url);
    }

    /*
    public string GetChatRateUrl(CustomerInfo customer, CustomerChatRateMessageInfo chatRateMessage)
    {
        string url = $"/mobile/chat/rate/{chatRateMessage.Code}";
        return GetAutoLoginUrl(url);
    }
    */
}