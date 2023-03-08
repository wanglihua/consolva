using NHibernate;
using shunda.Core.Service;
using shunda.Service.Message.CustomerMessage;

namespace shunda.Service.Message;

[Service(ServiceType.Scoped)]
public class CustomerMessageCycleTimingSendSettingService {
    private readonly ISession session;

    public CustomerMessageCycleTimingSendSettingService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 初始化设置
    /// </summary>
    public void InitSettings() {
        CustomerMessageHelpMeMakeMoneyService.InitSetting(session);
        CustomerMessageNewClaimService.InitSetting(session);
        CustomerMessageNoOrderService.InitSetting(session);

        session.Flush();
    }
}