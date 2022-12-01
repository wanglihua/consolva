using System.Security.Cryptography;
using System.Text;

namespace shunda.Core.Util;

public class Md5Util
{
    public static string Encrypt(string clearText)
    {
        using var md5 = MD5.Create();

        byte[] bytes = Encoding.UTF8.GetBytes(clearText);
        bytes = md5.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2").ToLower());
        }

        return sb.ToString();
    }

    public static readonly int Md5Length = Md5Util.Encrypt("hello").Length;
}