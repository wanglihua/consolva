using System.Linq;
using Core;
using Domain.Message;
using NHibernate;

namespace shunda.Service.Message;

[Service(ServiceType.Scoped)]
public class CustomerMessageCycleTimingSendSettingService
{
    private ISession session = null;

    public CustomerMessageCycleTimingSendSettingService(ISession session)
    {
        this.session = session;
    }

    /// <summary>
    /// 初始化设置
    /// </summary>
    public void InitSettings()
    {
        CustomerMessageHelpMeMakeMoneyService.InitSetting(session);
        CustomerMessageNotRegisterService.InitSetting(session);
        CustomerMessageNoOrderService.InitSetting(session);
        CustomerMessageNoFollowService.InitSetting(session);
        CustomerMessageCancelFollowService.InitSetting(session);
        CustomerMessageTimingSumitPackService.InitSetting(session);

        session.Flush();
    }
}
