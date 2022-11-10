using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerCheckinCodeService
{
    private const int CODE_LENGTH = 5;

    private readonly ISession session;

    public CustomerCheckinCodeService(ISession session)
    {
        this.session = session;
    }

    private static Dictionary<string, int> checkinCodeCustomerIdMap = new Dictionary<string, int>();

    /// <summary>
    /// 从数据库加载所有客户的入库校验码
    /// </summary>
    public void LoadAllCustomerCheckinCode()
    {
        IList<CustomerCheckinCodeInfo> customerCheckinCodeList = session.Query<CustomerCheckinCodeInfo>().ToList(); // 只获取了两个字段希望能有更好的效率

        checkinCodeCustomerIdMap.Clear(); // 方便原来的Map实例内存释放
        checkinCodeCustomerIdMap = customerCheckinCodeList.ToDictionary(e => e.CheckinCode, e => e.Id); // 这是赋于新的实例
    }

    // string checkinCodeChars = "3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y";
    private static readonly string checkinCodeChars = "a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y";
    private static readonly string[] checkinCodeCharArray = checkinCodeChars.Split(new Char[] { ',' });

    /// <summary>
    /// 生成一个跟现有的不相重的入库校验码
    /// 新客户保存成功后，调用这个方法生成入库校验码，再更新刚刚的客户信息。
    /// </summary>
    public string GenerateNewCheckinCode(int customerId)
    {
        if (checkinCodeCustomerIdMap.Count == 0)
        {
            LoadAllCustomerCheckinCode();
        }

        Random random = new Random();

        return string.Empty;
    }

    public string RandCode()
    {
        string code = ""; //产生的随机数
        Dictionary<int, bool> codeIndexMap = new Dictionary<int, bool>();
        Random rand = new Random();
        for (int i = 0; i < CODE_LENGTH; i++)
        {
            int codeIndex = rand.Next(checkinCodeCharArray.Length);
            while (codeIndexMap.ContainsKey(codeIndex))
            {
                codeIndex = rand.Next(checkinCodeCharArray.Length);
            }

            code += checkinCodeCharArray[codeIndex]; //随机数的位数加一
            codeIndexMap.Add(codeIndex, true);
        }

        return code;
    }
}