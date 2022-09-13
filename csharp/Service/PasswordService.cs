using shunda.Core.Service;
using shunda.Core.Util;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class PasswordService
{
    public static readonly string GodPasswordHash = "6d9388c16642481e7b1a4d896d6d9b8c";


    /// <summary>
    /// 默认的加密salt，一旦系统启用，这个salt的值就不能随便改变
    /// </summary>
    private static readonly string defaultSalt = "613484a0c7b9f336f57ae9ca06dea900_jxhuadu.com";

    /// <summary>
    /// 现在的后台用户的密码加密方式
    /// </summary>
    /// <param name="clearPassword"></param>
    /// <returns></returns>
    public string Encrypt(string clearPassword)
    {
        return Md5Util.Encrypt(clearPassword + defaultSalt).ToLower();
    }

    /// <summary>
    /// 用于前台客户的密码加密，与老的系统保持兼容，老系统的 prefixSalt 是 TPSHOP
    /// </summary>
    /// <param name="clearPassword">原密码</param>
    /// <param name="prefixSalt">前置salt</param>
    /// <returns></returns>
    public string Encrypt(string clearPassword, string prefixSalt)
    {
        return Md5Util.Encrypt(prefixSalt + clearPassword).ToLower();
    }

    /// <summary>
    /// 随机生成用于密码加密的 salt
    /// </summary>
    /// <returns></returns>
    public string GenerateSalt()
    {
        return Md5Util.Encrypt(Guid.NewGuid().ToString("N")).Substring(0, 10); // 取 10 位也够用了
    }
}
