using System.Text;
using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;

namespace shunda.Service.Customer;

/// <summary>
/// 超级密码逻辑
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerSupperPasswordService {
    private readonly ISession session;

    public CustomerSupperPasswordService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 超级密码的字符数
    /// </summary>
    private static int passwordLength = 8;

    /// <summary>
    /// 超级密码的候选字符
    /// </summary>
    private static string passwordChars = "3456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefhjkmnpqrstuvwxy";

    /// <summary>
    /// 超级密码过期时间，5分钟
    /// </summary>
    private static TimeSpan passwordExpireTime = new TimeSpan(0, 5, 0);

    /// <summary>
    /// 超级密码最小间隔时间，就是超级密码不能频繁生成
    /// </summary>
    private static TimeSpan passwordMinInterval = new TimeSpan(0, 0, 10);

    /// <summary>
    /// 生成新的超级密码
    /// </summary>
    /// <returns></returns>
    public string GenerateNewPassword(int customerId) {
        CustomerInfo? customerInfo = session.Get<CustomerInfo>(customerId);
        if (customerInfo == null) {
            throw new ValidationException("指定的客户不存在！");
        }

        DateTime maxPasswordTime = DateTime.Now.Add(-passwordMinInterval); // passwordMinInterval 前面有负号

        // 取最近的一个
        var recentSuperPassword = session.Query<CustomerSupperPasswordInfo>()
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (recentSuperPassword != null && recentSuperPassword.CreateTime > maxPasswordTime) {
            throw new ValidationException($"超级密码生成过于频繁，{passwordMinInterval.TotalSeconds}秒内只允许生成一个！");
        }

        string password = GenerateRandomPassword();

        CustomerSupperPasswordInfo supperPassword = new CustomerSupperPasswordInfo();
        supperPassword.CustomerId = customerId;
        supperPassword.Password = password;

        session.Save(supperPassword);
        session.Flush();

        return password;
    }

    /// <summary>
    /// 生成一个随机的新的超级密码
    /// </summary>
    /// <returns></returns>
    private string GenerateRandomPassword() {
        StringBuilder passwordStringBuilder = new StringBuilder();

        int passwordCharsTotalCount = passwordChars.Length;

        int lastRand = 0;
        for (int index = 0; index < passwordLength; index++) {
            Random random = new Random(DateTime.Now.Millisecond * lastRand + index);
            int rand = random.Next(0, passwordCharsTotalCount - 1);

            passwordStringBuilder.Append(passwordChars[rand]);

            lastRand = rand;
        }

        return passwordStringBuilder.ToString();
    }

    /// <summary>
    /// 检查超级密码是不是正确，是不是过期
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool CheckPasswordValid(int customerId, string password) {
        if (string.IsNullOrWhiteSpace(password)) {
            return false;
        }

        CustomerSupperPasswordInfo? superPassword = session
            .Query<CustomerSupperPasswordInfo>()
            .Where(e => e.CustomerId == customerId)
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (superPassword == null) {
            return false;
        }

        DateTime minPasswordTime = DateTime.Now.Add(-passwordExpireTime); // passwordExpireTime 前面有负号
        if (superPassword.CreateTime <= minPasswordTime) {
            return false;
        }

        if (superPassword.Password.Trim() != password.Trim()) {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取密码过期分钟数
    /// </summary>
    /// <returns></returns>
    public int GetPasswordExpireMinutes() {
        return (int)Math.Round(passwordExpireTime.TotalMinutes, MidpointRounding.AwayFromZero);
    }
}