using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateService {
    private readonly ISession session;

    public LogisticsOrderCreateService(ISession session) {
        this.session = session;
    }

    public LogisticsOrderInfo CreateOrder(int customerId) {
        LogisticsOrderCreateSelectInfo createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            throw new ValidationException("订单创建数据不存在，请先在前面步骤录入创建数据！");
        }

        if (createSelect.Valid == false) {
            throw new ValidationException("订单创建数据在确认前，又在别的界面被修改过，数据已失效！");
        }

        // 检查收件人和发货人是否一样
        int addressReceiveId = createSelect.AddressReceiveId;
        CustomerReceiveAddressInfo addressReceive = session.Get<CustomerReceiveAddressInfo>(addressReceiveId);
        if (addressReceive == null) {
            throw new Exception("订单创建收货地址未选择！");
        }

        int addressSendId = createSelect.AddressSendId;
        CustomerSendAddressInfo addressSend = session.Get<CustomerSendAddressInfo>(addressSendId);
        if (addressSend == null) {
            throw new Exception("订单创建发货地址未选择！");
        }

        string receiverNameToCompare = StringUtil.GetPinyin(addressReceive.LastName + addressReceive.FirstName);
        receiverNameToCompare = receiverNameToCompare.Replace(" ", "").ToLower();

        string senderNameToCompare = StringUtil.GetPinyin(addressSend.Name);
        senderNameToCompare = senderNameToCompare.Replace(" ", "").ToLower();

        if (receiverNameToCompare == senderNameToCompare) {
            throw new ValidationException("因海关要求，收货人与发货人，姓名不可以相同，请在【我的地址】修改！");
        }        
        return null;
    }
}