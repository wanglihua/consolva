using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerCheckinCodeService {
    private const int CODE_LENGTH = 5;

    private readonly ISession session;

    public CustomerCheckinCodeService(ISession session) {
        this.session = session;
    }

    // string checkinCodeChars = "3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y";
    private static readonly string checkinCodeChars = "a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y";
    private static readonly string[] checkinCodeCharArray = checkinCodeChars.Split(new Char[] { ',' });

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
            int codeIndex = rand.Next(checkinCodeCharArray.Length);
            // 保证生成的 checkin code 各个字母都不相同
            while (codeIndexMap.ContainsKey(codeIndex)) {
                codeIndex = rand.Next(checkinCodeCharArray.Length);
            }

            code += checkinCodeCharArray[codeIndex]; //随机数的位数加一
            codeIndexMap.Add(codeIndex, true);
        }

        return code;
    }
}