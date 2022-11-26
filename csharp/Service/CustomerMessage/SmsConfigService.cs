using NHibernate;
using shunda.Core.Service;

namespace shunda.Service.CustomerMessage;

[Service(ServiceType.Scoped)]
public class SmsConfigService
{
    private ISession session = null;

    public SmsConfigService(ISession session)
    {
        this.session = session;
    }

    /// <summary>
    /// 获取短信发送配置信息
    /// </summary>
    /// <returns>如果配置信息还不存在就返回 Id 为 0 的配置信息</returns>
    public SmsConfigInfo GetSmsConfig()
    {
        IList<SmsConfigInfo> smsConfigList = session.Query<SmsConfigInfo>().OrderBy(e => e.Id).Take(1).ToList();

        if (smsConfigList == null || smsConfigList.Count == 0)
        {
            return new SmsConfigInfo();
        }

        return smsConfigList[0];
    }
}