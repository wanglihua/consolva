using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerReceiveAddressService {
    private readonly ISession session;

    public CustomerReceiveAddressService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取完整地址
    ///
    /// 参考 https://www.zhihu.com/question/20562267
    /// </summary>
    /// <param name="receiveAddress"></param>
    /// <returns></returns>
    public string GetFullAddress(CustomerReceiveAddressInfo receiveAddress) {
        string unitLocStr = string.Empty;
        if (string.IsNullOrWhiteSpace(receiveAddress.UnitLoc) == false) {
            unitLocStr = $"{receiveAddress.UnitLoc}, ";
        }

        if (receiveAddress.Country == Country.Australia) {
            return $"{unitLocStr}{receiveAddress.Address}, {receiveAddress.City} {receiveAddress.State} {receiveAddress.Zipcode}";
        }

        if (receiveAddress.Country == Country.America) {
            return $"{unitLocStr}{receiveAddress.Address}, {receiveAddress.City}, {receiveAddress.State} {receiveAddress.Zipcode}";
        }

        return $"{unitLocStr}{receiveAddress.Address}, {receiveAddress.City} {receiveAddress.State} {receiveAddress.Zipcode}";
    }

    /// <summary>
    /// 获取完整地址
    ///
    /// 参考 https://www.zhihu.com/question/20562267
    /// </summary>
    /// <param name="receiveAddress"></param>
    /// <returns></returns>
    public string GetFullAddress(LogisticsOrderReceiveAddressInfo receiveAddress) {
        string unitLocStr = string.Empty;
        if (string.IsNullOrWhiteSpace(receiveAddress.UnitLoc) == false) {
            unitLocStr = $"{receiveAddress.UnitLoc}, ";
        }

        if (receiveAddress.Country == Country.Australia) {
            return $"{unitLocStr}{receiveAddress.Address}, {receiveAddress.City} {receiveAddress.State} {receiveAddress.Zipcode}";
        }

        if (receiveAddress.Country == Country.America) {
            return $"{unitLocStr}{receiveAddress.Address}, {receiveAddress.City}, {receiveAddress.State} {receiveAddress.Zipcode}";
        }

        return $"{unitLocStr}{receiveAddress.Address}, {receiveAddress.City} {receiveAddress.State} {receiveAddress.Zipcode}";
    }    
}