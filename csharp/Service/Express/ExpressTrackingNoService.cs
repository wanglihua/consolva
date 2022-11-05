using System.Text;
using shunda.Core.Config;
using shunda.Core.Service;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressTrackingNoService
{
    public string GetHiddenTrackingNo(string trackingNo)
    {
        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            return string.Empty;
        }

        if (trackingNo.Length <= 3)
        {
            return trackingNo;
        }

        // 长度小于等于9时隐藏 4 个字符，大于 9 时隐藏 6 个字符
        int hiddenCharCount = 4;
        if (trackingNo.Length > 9)
        {
            hiddenCharCount = 6;
        }

        int startHideIndex = trackingNo.Length - hiddenCharCount - 1;

        StringBuilder sb = new StringBuilder();
        for (int index = 0; index < trackingNo.Length; index++)
        {
            if (startHideIndex <= index && index < startHideIndex + hiddenCharCount)
            {
                sb.Append('*');
            }
            else
            {
                sb.Append(trackingNo[index]);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 检查快递单号是否匹配
    /// </summary>
    /// <param name="trackingNoFull">完整的正确的快递单号</param>
    /// <param name="trackingNoInput">完整输入的快递单号，或者隐藏部分的快递单号</param>
    /// <returns></returns>
    public bool IsTrackingNoMatch(string trackingNoFull, string trackingNoInput)
    {
        if (string.IsNullOrWhiteSpace(trackingNoFull))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(trackingNoInput))
        {
            return false;
        }

        trackingNoFull = trackingNoFull.Trim();
        trackingNoInput = trackingNoInput.Trim();

        if (trackingNoFull.ToUpper() == trackingNoInput.ToUpper())
        {
            return true;
        }

        string hiddenPart = "";
        int inputTrackingNoLength = trackingNoInput.Length;
        for (int index = 0; index < inputTrackingNoLength; index++)
        {
            hiddenPart += "*";
        }

        string trackingNoHidden = GetHiddenTrackingNo(trackingNoFull);

        string trackingNoInputFull = trackingNoHidden.Replace(hiddenPart, trackingNoInput);

        if (trackingNoFull.ToUpper() == trackingNoInputFull.ToUpper())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是不是合法的快递单号
    /// </summary>
    /// <param name="trackingNo"></param>
    /// <returns></returns>
    public bool IsValidateTrackingNo(string trackingNo)
    {
        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            return false;
        }

        trackingNo = trackingNo.Trim();

        if (trackingNo.Length < ExpressTrackingNoConfig.MinTrackingNoLength) // 假定快递单号的长度不能小于6
        {
            return false;
        }

        // 快递单号必须是字母和数字，还可以是 - 和 / 
        foreach (char trackingNoChar in trackingNo)
        {
            if (char.IsLetterOrDigit(trackingNoChar) == false && trackingNoChar != '-' && trackingNoChar != '/')
            {
                return false;
            }
        }

        return true;
    }
}