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
        {
            // 帮我赚钱提醒

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.BangWoZhuanQianTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.BangWoZhuanQianTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = true;
                setting.SmsSend = false;

                setting.WeiXinMessageTemplate1 = "嗨，老板，今天澳拼帮您赚钱了吗？动动手指，将【帮我赚钱】里您的专属海报转发微信朋友圈，让澳拼马不停蹄帮您赚钱哦！";
                setting.SmsMessageTemplate = "嗨，老板，今天澳拼帮您赚钱了吗？将【帮我赚钱】里您的专属海报转发朋友圈就可以赚钱哦！";

                session.Save(setting);
                session.Flush();
            }

            setting.Placeholders = "";
            session.Update(setting);
            session.Flush();
        }

        {
            // 未注册会员推送

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.WeiZhuCeHuiYuanTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.WeiZhuCeHuiYuanTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = true;
                setting.SmsSend = false;

                setting.WeiXinMessageTemplate1 = "亲，澳拼集运发红包了。现在注册会员，即送￥100红包哦。该红包即可以在澳拼消费，也可以提现。进入公众号【费率|下单】里面的【在线下单】即可注册会员呢，别错过机会哦！";
                setting.SmsMessageTemplate = "亲，澳拼集运发红包了。现在注册会员，即送￥100红包哦！";

                session.Save(setting);
                session.Flush();
            }

            setting.Placeholders = "";
            session.Update(setting);
            session.Flush();
        }

        {
            // 已注册未下单推送

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.YiZhuCeXiaDanTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.YiZhuCeXiaDanTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = true;
                setting.SmsSend = false;

                setting.WeiXinMessageTemplate1 = "嗨，老板，澳拼很久没有见您下单了。下个订单，赏口饭吃呗。现在下单，不仅费用低，还有优惠活动哦！";
                setting.SmsMessageTemplate = "嗨，老板，澳拼很久没有见您下单了。下个订单，赏口饭吃呗。现在下单，不仅费用低，还有优惠活动哦！";

                session.Save(setting);
                session.Flush();
            }

            setting.Placeholders = "";
            session.Update(setting);
            session.Flush();
        }

        {
            // 会员未关注公众号推送

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.HuiYuanWeiGuanZhuGongZhongHaoTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.HuiYuanWeiGuanZhuGongZhongHaoTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = false;
                setting.SmsSend = true;

                setting.WeiXinMessageTemplate1 = "亲，发现您未关注澳拼公众号哦。关注公众号，登录系统免密码，且会收到更丰富的实时通知哦！澳拼公众号：iepoch-au";
                setting.SmsMessageTemplate = "亲，发现您未关注澳拼公众号哦。关注公众号，登录系统免密码，且会收到更丰富的实时通知哦！澳拼公众号：iepoch-au";

                session.Save(setting);
                session.Flush();
            }

            setting.Placeholders = "";
            session.Update(setting);
            session.Flush();
        }

        {
            // 取消关注公众号推送

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.QuXiaoGuanZhuGongZhongHaoTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.QuXiaoGuanZhuGongZhongHaoTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = false;
                setting.SmsSend = true;

                setting.WeiXinMessageTemplate1 = "亲，您取消关注澳拼公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！澳拼公众号：iepoch-au";
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

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.DingShiTiJiaoDaBaoTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.DingShiTiJiaoDaBaoTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = true;
                setting.SmsSend = false;

                setting.WeiXinMessageTemplate1 = "亲，您的{0}订单目前已是全部入库状态，如您不再需要添加新的快递，请及时点击【提交打包】按钮哦。早打包，早收到宝贝呢！";
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

            var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>().Where(e => e.Type == CustomerMessageType.GongGongShenLingJianChuLiTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageCycleTimingSendSettingInfo();
                setting.Type = CustomerMessageType.GongGongShenLingJianChuLiTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WeiXinSend = true;
                setting.SmsSend = false;

                setting.WeiXinMessageTemplate1 = "亲，客服发现您有【提交打包】前状态的订单，不知公共申领区有没有您的包裹。方便的话，去看一下吧，万一有呢！";
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
