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

    // string checkinCodeChars = "3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y";
    private static readonly string CHECKIN_CODE_CHARS = "a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y";
    private static readonly string[] CHECKIN_CODE_CHAR_ARRAY = CHECKIN_CODE_CHARS.Split(new Char[] { ',' });
    private static readonly int CHECKIN_CODE_CHAR_ARRAY_LENGTH = CHECKIN_CODE_CHAR_ARRAY.Length;

    private static readonly IDictionary<string, int> DICT_CHECKIN_CODE_INDEX = CHECKIN_CODE_CHAR_ARRAY
        .Select((e, index) => new { Code = e, Index = index })
        .ToDictionary(e => e.Code, e => e.Index);

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

            // 去掉sd 和 sdqq 开头的
            randCheckCodeList = randCheckCodeList
                .Where(e => e.StartsWith("sd") == false && e.StartsWith("sdqq") == false) // 假设生成的都是小写的
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
        int indexSum = 0;
        Random rand = new Random();
        for (int i = 0; i < CODE_LENGTH - 1; i++) {
            int codeIndex = rand.Next(CHECKIN_CODE_CHAR_ARRAY.Length);
            // 保证生成的 checkin code 各个字母都不相同
            while (codeIndexMap.ContainsKey(codeIndex)) {
                codeIndex = rand.Next(CHECKIN_CODE_CHAR_ARRAY.Length);
            }

            code += CHECKIN_CODE_CHAR_ARRAY[codeIndex]; //随机数的位数加一
            codeIndexMap.Add(codeIndex, true);
            indexSum += codeIndex;
        }

        // 校验位
        indexSum %= CHECKIN_CODE_CHAR_ARRAY_LENGTH;
        code += CHECKIN_CODE_CHAR_ARRAY[indexSum];

        return code;
    }

    /// <summary>
    /// 用校验位验证入库码的合法性
    /// </summary>
    /// <returns></returns>
    public bool CheckCheckinCodeValid(string checkinCode) {
        if (string.IsNullOrWhiteSpace(checkinCode)) {
            return false;
        }

        checkinCode = checkinCode.ToLower();

        if (checkinCode.Length != CODE_LENGTH) {
            return false;
        }

        int codeIndexSum = 0;
        for (int i = 0; i < CODE_LENGTH - 1; i++) {
            string codeSingle = checkinCode[i].ToString();
            if (DICT_CHECKIN_CODE_INDEX.ContainsKey(codeSingle)) {
                codeIndexSum += DICT_CHECKIN_CODE_INDEX[codeSingle];
            }
        }

        codeIndexSum %= CODE_LENGTH;
        if (CHECKIN_CODE_CHAR_ARRAY[codeIndexSum] != checkinCode[CODE_LENGTH - 1].ToString()) {
            return false;
        }

        return true;
    }
}