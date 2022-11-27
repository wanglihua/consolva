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

        {
            // 取消关注公众号推送

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.CancelFollow).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.CancelFollow;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = false;
                setting.SmsSend = true;

                setting.WechatMessageTemplate1 = "亲，您取消关注澳拼公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！澳拼公众号：iepoch-au";
                setting.SmsMessageTemplate = "亲，您取消关注澳拼公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！澳拼公众号：iepoch-au";

                session.Save(setting);
                session.Flush();
            }

            setting.Placeholders = "";
            session.Update(setting);
            session.Flush();
        }

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

        {
            // 公共申领件处理提醒

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.NewClaim).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.NewClaim;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，客服发现您有【提交打包】前状态的订单，不知公共申领区有没有您的包裹。方便的话，去看一下吧，万一有呢！";
                setting.SmsMessageTemplate = "亲，您有【提交打包】前状态的订单，不知公共申领区有没有您的包裹。方便的话，去看一下吧，万一有呢！";

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
