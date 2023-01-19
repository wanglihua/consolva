using Newtonsoft.Json;

namespace shunda.Admin.Areas.AutoApi.Models.AutoMeasureData;

public class AutoMeasureDataUploadResponseModel {
    /// <summary>
    /// Success
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel Success = new AutoMeasureDataUploadResponseModel() { Flag = "true", Code = "", Message = "", Data = true };

    /// <summary>
    /// 系统异常！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS001 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S001", Message = "系统异常！", Data = false };

    /// <summary>
    /// 快递单号格式不合法！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS002 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S002", Message = "快递单号格式不合法！", Data = false };

    /// <summary>
    /// 箱号不存在！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS003 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S003", Message = "箱号不存在！", Data = false };

    /// <summary>
    /// 长宽高数据错误！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS004 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S004", Message = "长宽高数据错误！", Data = false };

    /// <summary>
    /// 重量数据错误！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS005 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S005", Message = "重量数据错误！", Data = false };

    /// <summary>
    /// 其他错误！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS006 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S006", Message = "其他错误！", Data = false };

    /// <summary>
    /// 其他错误！
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel ErrorS100 = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "S100", Message = "参数签名错误，非法调用！", Data = false };

    /// <summary>
    /// 未知错误
    /// </summary>
    public static readonly AutoMeasureDataUploadResponseModel Unknow = new AutoMeasureDataUploadResponseModel() { Flag = "failure", Code = "unknow", Message = "未知错误", Data = false };

    private static readonly IReadOnlyList<AutoMeasureDataUploadResponseModel> codeTypeList = new List<AutoMeasureDataUploadResponseModel>() {
        Success,
        ErrorS001,
        ErrorS002,
        ErrorS003,
        ErrorS004,
        ErrorS005,
        ErrorS006,
        ErrorS100,
    };

    private static readonly Dictionary<string, AutoMeasureDataUploadResponseModel> codeTypeMap = codeTypeList.ToDictionary(e => e.Code);

    public static AutoMeasureDataUploadResponseModel GetByCode(string code) {
        if (codeTypeMap.TryGetValue(code, out AutoMeasureDataUploadResponseModel? codeType)) {
            return codeType;
        }

        return Unknow; // 未知错误
    }

    private AutoMeasureDataUploadResponseModel() {
    }

    [JsonProperty("flag")]
    public string Flag { get; set; } = string.Empty;

    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data")]
    public bool Data { get; set; }
}