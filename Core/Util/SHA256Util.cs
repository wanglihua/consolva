using System.Security.Cryptography;
using System.Text;

namespace shunda.Core.Util;

public class SHA256Util {
    public static string Encrypt(string clearText) {
        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(clearText);
        bytes = sha256.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes) {
            sb.Append(b.ToString("x2").ToLower());
        }

        return sb.ToString();
    }

    public static readonly int SHA256Length = SHA256Util.Encrypt("hello").Length;
}