using System.Text;

namespace shunda.Core.Util;

public static class StringUtil {
    public static bool IsNumeric(string str) {
        if (string.IsNullOrEmpty(str)) {
            return true;
        }

        if (int.TryParse(str, out int intVal) == false) {
            return false;
        }

        return true;
    }

    public static string LimitMaxLength(string str, int maxLength) {
        if (string.IsNullOrWhiteSpace(str)) {
            return String.Empty;
        }

        if (str.Length > maxLength) {
            str = str.Substring(0, maxLength);
        }

        return str;
    }

    private static readonly int chineseCharCodeBegin = Convert.ToInt32("4e00", 16);
    private static readonly int chineseCharCodeEnd = Convert.ToInt32("9fff", 16);

    /// <summary>
    /// 获取字符串长度，一个汉字算 2 个长度，一个出字母和数字算 1 个长度
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetStringLengthInByte(string str) {
        if (str == null) {
            return 0;
        }

        int length = 0;
        for (int i = 0; i < str.Length; i++) {
            char c = str[i];

            if (Char.IsDigit(c)) {
                length += 1;
            } else if (Char.IsWhiteSpace(c)) {
                length += 1;
            } else if (Char.ConvertToUtf32(str, i) >= chineseCharCodeBegin && Char.ConvertToUtf32(str, i) <= chineseCharCodeEnd) {
                length += 2;
            } else {
                length += 1;
            }

            // 中文字也会被判定为 Char.IsLetter 
        }

        return length;
    }

    /// <summary>
    /// 字符串是不是都是中文字符
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool AllChinese(string str) {
        if (string.IsNullOrWhiteSpace(str)) {
            return true; // 暂且返回 true
        }

        for (int i = 0; i < str.Length; i++) {
            if (Char.ConvertToUtf32(str, i) < chineseCharCodeBegin || Char.ConvertToUtf32(str, i) > chineseCharCodeEnd) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 是不是都是英文字母或者字符
    /// 来之网络
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static bool AllEnglish(string content) {
        if (string.IsNullOrWhiteSpace(content)) {
            return true;
        }

        var bytes = Encoding.UTF8.GetBytes(content);
        bool result = bytes.Length == content.Length;
        return result;
    }

    /// <summary>
    /// 字符串中是否有数字
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool HasDigit(string str) {
        if (str == null) {
            return false;
        }

        for (int i = 0; i < str.Length; i++) {
            char c = str[i];

            if (Char.IsDigit(c)) {
                return true;
            }
        }

        return false;
    }
}