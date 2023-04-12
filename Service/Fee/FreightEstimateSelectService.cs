using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class FreightEstimateSelectService {
    private readonly ISession session;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public FreightEstimateSelectService(ISession session, CustomerReceiveAddressService customerReceiveAddressService) {
        this.session = session;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    public void SaveCountry(int customerId, Country country) {
        var estimateSelect = session.Get<FreightEstimateSelectInfo>(customerId);
        if (estimateSelect == null) {
            estimateSelect = new FreightEstimateSelectInfo {
                CustomerId = customerId,
                Country = country,
            };
            session.Save(estimateSelect);
        } else {
            estimateSelect.Country = country;
            session.Update(estimateSelect);
        }

        session.Flush();
    }

    public void SaveFirstMode(int customerId, int firstModeId) {
        var firstMode = session.Get<LogisticsModeInfo>(firstModeId);
        if (firstMode == null) {
            throw new ValidationException($"指定的物流方式#{firstModeId}不存在！");
        }

        var firstType = firstMode.Type;

        var estimateSelect = session.Get<FreightEstimateSelectInfo>(customerId);
        if (estimateSelect == null) {
            estimateSelect = new FreightEstimateSelectInfo {
                CustomerId = customerId,
                FirstType = firstType,
                FirstModeId = firstModeId,
            };
            session.Save(estimateSelect);
        } else {
            if (estimateSelect.Country != firstMode.Country) {
                throw new ValidationException("物流方式对应国家和运费预估记录国家不一致！");
            }

            estimateSelect.FirstType = firstType;
            estimateSelect.FirstModeId = firstModeId;
            session.Update(estimateSelect);
        }

        session.Flush();
    }

    public void SaveReceiveAddress(int customerId, int receiveAddressId) {
        CustomerReceiveAddressInfo receiveAddress = session.Get<CustomerReceiveAddressInfo>(receiveAddressId);
        if (receiveAddress == null) {
            throw new ValidationException($"指定的收货地址#{receiveAddressId}不存在！");
        }

        // 判断是否有非英文字符 等
        customerReceiveAddressService.CheckAddress(receiveAddress);

        var estimateSelect = session.Get<FreightEstimateSelectInfo>(customerId);
        if (estimateSelect == null) {
            estimateSelect = new FreightEstimateSelectInfo {
                CustomerId = customerId,
                AddressReceiveId = receiveAddressId,
            };
            session.Save(estimateSelect);
        } else {
            estimateSelect.AddressReceiveId = receiveAddressId;
            session.Update(estimateSelect);
        }

        session.Flush();
    }
}