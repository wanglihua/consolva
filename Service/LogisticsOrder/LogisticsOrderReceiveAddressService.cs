using NHibernate;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderReceiveAddressService {
    private readonly ISession session;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public LogisticsOrderReceiveAddressService(ISession session,
        CustomerReceiveAddressService customerReceiveAddressService
    ) {
        this.session = session;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    /// <summary>
    /// 获取完整地址
    ///
    /// 参考 https://www.zhihu.com/question/20562267
    /// </summary>
    /// <param name="receiveAddress"></param>
    /// <returns></returns>
    public string GetFullAddress(LogisticsOrderReceiveAddressInfo receiveAddress) {
        return customerReceiveAddressService.GetFullAddress(receiveAddress);
    }
}