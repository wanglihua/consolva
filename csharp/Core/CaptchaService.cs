using System.Drawing;
using System.Drawing.Imaging;
using CSRedis;
using shunda.Core.Redis;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Core.Web;
using NLog;

namespace shunda.Core;

[Service(ServiceType.Scoped)]
public class CaptchaService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private const int defaultCodeLength = 4;

    private WebSessionService webSessionService;

    public CaptchaService(WebSessionService webSessionService)
    {
        this.webSessionService = webSessionService;
    }

    public MemoryStream Create(int codeLength = defaultCodeLength)
    {
        string code = RandNum(codeLength);

        // 把验证码放入 Session 中
        PutCodeToSession(code);

        log.Trace("captcha: " + code);

        //验证码颜色集合
        Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

        //验证码字体集合
        string[] fonts = { "Courier" };
        try
        {
            //定义图像的大小，生成图像的实例
            Bitmap image = new Bitmap(code.Length * 18, 32);
            Graphics graphics = Graphics.FromImage(image); //从Img对象生成新的Graphics对象
            graphics.Clear(Color.White); //背景设为白色

            Random random = new Random();

            //画图片的干扰线
            // for (int i = 0; i < 10; i++)
            for (int i = 0; i < 5; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                graphics.DrawLine(new Pen(colors[random.Next(colors.Length)]), x1, y1, x2, y2);
            }

            //在随机位置画背景点
            // for (int i = 0; i < 100; i++)
            for (int i = 0; i < 50; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                graphics.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }

            for (int i = 0; i < code.Length; i++)
            {
                int colorIndex = random.Next(colors.Length); //随机颜色索引值
                // int fontIndex = random.Next(fonts.Length); //随机字体索引值
                Font font = new Font(fonts[0], 18, FontStyle.Bold); //字体
                Brush brush = new SolidBrush(colors[colorIndex]); //颜色
                int ii = 4;

                //控制验证码不在同一高度
                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }

                graphics.DrawString(code.Substring(i, 1), font, brush, 3 + (i * 12), ii); //绘制一个验证字符
            }

            MemoryStream memoryStream = new MemoryStream(); //生成内存流对象
            image.Save(memoryStream, ImageFormat.Png); //将此图像以Png图像文件的格式保存到流中

            //回收资源
            graphics.Dispose();
            image.Dispose();

            return memoryStream;
        }
        catch (Exception exp)
        {
            log.Error(exp);
            return new MemoryStream();
        }
    }

    // string captchaChars = "3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y";
    private static readonly string captchaChars = "a,b,c,d,e,f,g,h,j,k,m,n,p,r,s,t,u,v,w,x,y";
    private static readonly string[] captchaCharArray = captchaChars.Split(new Char[] { ',' });

    private string RandNum(int codeLength = defaultCodeLength)
    {
        string code = ""; //产生的随机数
        int temp = -1; //记录上次随机数值，尽量避避免生产几个一样的随机数
        Random rand = new Random();
        //采用一个简单的算法以保证生成随机数的不同
        for (int i = 1; i < codeLength + 1; i++)
        {
            if (temp != -1)
            {
                rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks)); //初始化随机类
            }

            int t = rand.Next(captchaCharArray.Length - 1); //获取随机数，这里其实不用 -1 的
            if (temp != -1 && temp == t)
            {
                return RandNum(codeLength); //如果获取的随机数重复，则递归调用
            }

            temp = t; //把本次产生的随机数记录起来
            code += captchaCharArray[t]; //随机数的位数加一
        }

        return code;
    }

    /// <summary>
    /// 检查输入的图形验证码是否正确
    /// 可以在检查失败之后，调用 MakeCurrentExpire() 把当前图形验证码设置为失效（需要再方法外，调用者完成）
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public bool CheckInputCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return false;
        }

        code = code.ToUpper(); // 比较时，忽略大小写

        // string codeInSession = string.Empty;
        // if (memoryCache.TryGetValue(GetCaptchaSessionKey(), out codeInSession) == false)
        // {
        //     return false;
        // }

        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string codeInSession = redisClient.Get(GetCaptchaSessionKey());
        // 从redis中取得captcha后，应该马上把redis中的失效掉，
        // 不管原来输入的是否正确。输正确的也要过期掉。
        // 在这里调用这个过期后，别的地方实际已经不用再调用这个过期了。
        MakeCurrentExpire();

        if (string.IsNullOrWhiteSpace(codeInSession))
        {
            return false;
        }

        if (Md5Util.Encrypt(code) != codeInSession)
        {
            return false;
        }

        return true;
    }

    public void MakeCurrentExpire()
    {
        string code = RandNum();
        PutCodeToSession(code);
    }

    private void PutCodeToSession(String code)
    {
        code = code.ToUpper(); // 放入Session时，忽略大小写

        // memoryCache.Set(GetCaptchaSessionKey(), Md5Util.Encrypt(code), DateTimeOffset.Now.AddMinutes(30));

        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        redisClient.Set(GetCaptchaSessionKey(), Md5Util.Encrypt(code), TimeSpan.FromMinutes(30));
    }

    private string GetCaptchaSessionKey()
    {
        string keyPrefix = RedisClientUtil.RedisKeyPrefix;
        string webSessionId = webSessionService.SessionId;
        return $"{keyPrefix}:captcha:{webSessionId}";
    }
}