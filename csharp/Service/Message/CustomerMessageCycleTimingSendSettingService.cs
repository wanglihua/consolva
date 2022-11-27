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
        
        {
            // 定时提交打包提醒

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.TimingSubmitPack).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.TimingSubmitPack;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，您的{0}订单目前已是全部入库状态，如您不再需要添加新的快递，请及时点击【提交打包】按钮哦。早打包，早收到宝贝呢！";
                setting.SmsMessageTemplate = "亲，您的{0}订单目前已是全部入库状态，如您不再需要添加新的快递，请及时点击【提交打包】按钮哦。";

                session.Save(setting);
                session.Flush();
            }

            setting.Placeholders = "";
            session.Update(setting);
            session.Flush();
        }
        
        session.Flush();
    }
}
