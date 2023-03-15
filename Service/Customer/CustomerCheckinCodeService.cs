using System.Text;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerCheckinCodeService {
    private const int CODE_LENGTH = 6; // 5位+1位校验位

    private readonly ISession session;

    public CustomerCheckinCodeService(ISession session) {
        this.session = session;
    }

    private static readonly IDictionary<string, string> CODE_CHINESE_MAP = new Dictionary<string, string>() {
        { "0", "零" },
        { "1", "壹" },
        { "2", "贰" },
        { "3", "叁" },
        { "4", "肆" },
        { "5", "伍" },
        { "6", "陆" },
        { "7", "柒" },
        { "8", "捌" },
        { "9", "玖" },
    };

    private static readonly IList<string> CHECKIN_CODE_CHAR_LIST = CODE_CHINESE_MAP.Keys.OrderBy(e => e).ToList();
    private static readonly int CHECKIN_CODE_CHAR_LIST_LENGTH = CODE_CHINESE_MAP.Keys.Count;

    /// <summary>
    /// 生成一个跟现有的不相重的入库校验码
    /// </summary>
    public string GenerateNewCheckinCode() {
        string newCheckinCode = string.Empty;
        // 最多做100次，如果100次还找不到未使用的入库校验码，就报错。一般情况下是肯定能找到的
        for (int i = 0; i < 100; i++) {
            IList<string> randCheckCodeList = new List<string>();
            // 一次生成10个随机数，然后一次拿这10个数去数据库查询是不是有没被使用的，这样一次数据库查询就成功的几率很高
            for (int j = 0; j < 10; j++) {
                randCheckCodeList.Add(RandCode());
            }

            // 去掉0开头的
            randCheckCodeList = randCheckCodeList
                .Where(e => e.StartsWith("0") == false)
                .ToList();

            if (randCheckCodeList.Count > 0) {
                // 查询生成的入库校验码当前正在使用的
                var existCheckinCodeList = session.Query<CustomerCheckinCodeInfo>()
                    .Where(e => randCheckCodeList.Contains(e.CheckinCode))
                    .Select(e => e.CheckinCode)
                    .ToList();

                // 查找生成的入库校验码中还没实际使用的一个
                string? notUsedCheckinCode = randCheckCodeList
                    .Where(e => existCheckinCodeList.Contains(e) == false)
                    .Take(1)
                    .SingleOrDefault();

                // 如果找出未使用的就退出循环
                if (string.IsNullOrWhiteSpace(notUsedCheckinCode) == false) {
                    newCheckinCode = notUsedCheckinCode;
                    break;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(newCheckinCode)) {
            throw new Exception("生成客户入库校验码失败！");
        }

        return newCheckinCode;
    }

    public string RandCode() {
        string code = ""; //产生的随机数
        Dictionary<int, bool> codeIndexMap = new Dictionary<int, bool>();
        Random rand = new Random();
        for (int i = 0; i < CODE_LENGTH; i++) {
            int codeIndex = rand.Next(CHECKIN_CODE_CHAR_LIST_LENGTH);
            // 保证生成的 checkin code 各个字母都不相同
            while (codeIndexMap.ContainsKey(codeIndex)) {
                codeIndex = rand.Next(CHECKIN_CODE_CHAR_LIST_LENGTH);
            }

            code += CHECKIN_CODE_CHAR_LIST[codeIndex]; //随机数的位数加一
            codeIndexMap.Add(codeIndex, true);
        }

        return code;
    }

    /// <summary>
    /// 将代码转成中文
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public string GetChineseCode(string code) {
        if (string.IsNullOrWhiteSpace(code)) {
            return string.Empty;
        }

        code = code.Trim();

        StringBuilder sb = new StringBuilder();
        foreach (char codeChar in code) {
            string codeCharStr = codeChar.ToString();
            if (CODE_CHINESE_MAP.TryGetValue(codeCharStr, out string? chineseCode)) {
                sb.Append(chineseCode);
            } else {
                sb.Append(codeCharStr);
            }
        }

        return sb.ToString();
    }
}