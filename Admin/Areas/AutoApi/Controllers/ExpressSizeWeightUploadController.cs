using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using shunda.Admin.Areas.AutoApi.Models.AutoMeasureData;
using shunda.Admin.Areas.AutoApi.Models.ExpressSizeWeightUpload;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Service.AutoMeasure;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.AutoApi.Controllers;

[NoLoginRequired]
[AutoApiArea]
public class ExpressSizeWeightUploadController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressLogService expressLogService;

    public ExpressSizeWeightUploadController(ISession session, ExpressTrackingNoService expressTrackingNoService, ExpressLogService expressLogService) {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    [HttpPost]
    [Route("/admin/auto/api/express/size/weight/upload")]
    public IActionResult Upload() {
        log.Error("ExpressSizeWeightUploadController.Upload begin");
        try {
            Transactional.Wrap(session, () => {
                StreamReader streamReader = new StreamReader(Request.Body);
                string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

                log.Error("ExpressSizeWeightUploadController.Upload requestBody");
                log.Error(requestBody);

                var param = JsonConvert.DeserializeObject<ExpressSizeWeightUploadParam>(requestBody);

                if (param == null || param.Data == null) {
                    throw new ValidationException("尺寸重量上传数据解析异常！");
                }

                var data = param.Data;
                string trackingNo = data.TrackingNumber;

                if (string.IsNullOrWhiteSpace(trackingNo) == true) {
                    throw new ValidationException("上传尺寸重量数据快递单号不能为空！");
                }

                if (decimal.TryParse(data.Length, out decimal length) == false) {
                    throw new ValidationException("上传尺寸重量数据长度数据格式不正确！");
                }

                if (length < 0) {
                    throw new ValidationException("上传尺寸重量数据长度不能为负！");
                }

                if (decimal.TryParse(data.Width, out decimal width) == false) {
                    throw new ValidationException("上传尺寸重量数据宽度数据格式不正确！");
                }

                if (width < 0) {
                    throw new ValidationException("上传尺寸重量数据宽度不能为负！");
                }

                if (decimal.TryParse(data.Height, out decimal height) == false) {
                    throw new ValidationException("上传尺寸重量数据高度数据格式不正确！");
                }

                if (height < 0) {
                    throw new ValidationException("上传尺寸重量数据高度不能为负！");
                }

                if (decimal.TryParse(data.Weight, out decimal weight) == false) {
                    throw new ValidationException("上传尺寸重量数据重量数据格式不正确！");
                }

                if (weight < 0) {
                    throw new ValidationException("上传尺寸重量数据重量不能为负！");
                }

                string appKey = param.AppKey;

                AutoMeasureMachineInfo? autoMeasureMachine = session
                    .Query<AutoMeasureMachineInfo>()
                    .Where(e => e.AppKey == appKey)
                    .Take(1)
                    .SingleOrDefault();
                string secret = autoMeasureMachine?.Secret ?? string.Empty;
                string timestamp = param.TimeStamp;
                string dataQuery = $"height={height}&length={length}&tracking_number={trackingNo}&weight={weight}&width={width}";
                string signCal = Md5Util.Encrypt(Md5Util.Encrypt(appKey + dataQuery + timestamp) + Md5Util.Encrypt(secret + dataQuery + timestamp));

                if (signCal != param.Sign) {
                    log.Error($"上传尺寸重量数据签名错误, request body: {requestBody}");
                    log.Error($"上传尺寸重量数据签名错误, secret: {secret}");
                    throw new ValidationException("上传尺寸重量数据签名错误！");
                }

                // 获取母单号
                string trackingNoMain = trackingNo.Trim();

                int indexOfMinus = trackingNo.IndexOf("-", StringComparison.InvariantCulture);
                if (indexOfMinus != -1) {
                    trackingNoMain = trackingNo.Substring(0, indexOfMinus);
                }

                // 可能的关联快递单号
                ExpressInfo? express = session.Query<ExpressInfo>()
                    .Where(e => e.TrackingNo == trackingNoMain)
                    .Take(1)
                    .SingleOrDefault();
                LogisticsOrderInfo? order = express?.Order; // 可能为空
                string orderCode = order?.Code ?? string.Empty;

                // 是简单的母单号
                if (indexOfMinus == -1) {
                    ExpressSizeWeightInfo? expressSizeWeight = session.Query<ExpressSizeWeightInfo>()
                        .Where(e => e.TrackingNo == trackingNoMain
                                    && e.TrackingNoSub == string.Empty) // 取主单号
                        .Take(1)
                        .SingleOrDefault();

                    bool create = false;
                    if (expressSizeWeight == null) {
                        create = true;

                        expressSizeWeight = new ExpressSizeWeightInfo();

                        // 只在新增的时候保存这些个字段
                        expressSizeWeight.TrackingNo = trackingNoMain;
                        expressSizeWeight.TrackingNoSub = string.Empty;
                        expressSizeWeight.PackageCount = 1; // 可能这个箱数被后台修改过，所有只在创建时设置
                    }

                    if (expressSizeWeight.PackageCount <= 0) // 可能这个母单号是，子单号尺寸上传时自动创建的。那时这个数量设置的是0，如果是这种情况，这里要纠正过来。
                    {
                        expressSizeWeight.PackageCount = 1;
                    }

                    StringBuilder editLogDescBuilder = new StringBuilder();

                    // length
                    if (create == false && expressSizeWeight.Length != length) {
                        editLogDescBuilder.Append($"长:{expressSizeWeight.Length}=>{length}");
                    }

                    expressSizeWeight.Length = length;

                    // width
                    if (create == false && expressSizeWeight.Width != width) {
                        editLogDescBuilder.Append($"宽:{expressSizeWeight.Width}=>{width}");
                    }

                    expressSizeWeight.Width = width;

                    // height
                    if (create == false && expressSizeWeight.Height != height) {
                        editLogDescBuilder.Append($"高:{expressSizeWeight.Height}=>{height}");
                    }

                    expressSizeWeight.Height = height;

                    // weight
                    if (create == false && expressSizeWeight.Weight != weight) {
                        editLogDescBuilder.Append($"重量:{expressSizeWeight.Weight}=>{weight}");
                    }

                    expressSizeWeight.Weight = weight;

                    // 机器区分
                    expressSizeWeight.Machine = null;

                    expressSizeWeight.Express = express;
                    expressSizeWeight.Order = order;
                    expressSizeWeight.OrderCode = orderCode;

                    session.SaveOrUpdate(expressSizeWeight);
                    session.Flush();

                    if (create == true) {
                        string addLogDesc = $"长:{expressSizeWeight.Length}, 宽:{expressSizeWeight.Width}, 高:{expressSizeWeight.Height}, 重量:{expressSizeWeight.Weight}, 箱数:{expressSizeWeight.PackageCount}";
                        expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightAdd, addLogDesc);
                    } else {
                        expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightEdit, editLogDescBuilder.ToString());
                    }
                } else // 是子单号
                {
                    string trackingNoSub = trackingNo.Trim();

                    // 检查母单号的合法性
                    if (expressTrackingNoService.IsValidateTrackingNo(trackingNoMain) == false) {
                        throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS002.Code); // 快递单号格式不合法！
                    }

                    ExpressSizeWeightInfo? expressSizeWeight = session.Query<ExpressSizeWeightInfo>()
                        .Where(e => e.TrackingNo == trackingNoMain // trackingNoMain 母单号
                                    && e.TrackingNoSub == string.Empty
                        )
                        .Take(1)
                        .SingleOrDefault();

                    // 如果这时母单号还不存在，就新增一个
                    if (expressSizeWeight == null) {
                        expressSizeWeight = new ExpressSizeWeightInfo();

                        expressSizeWeight.TrackingNo = trackingNoMain; // trackingNoMain 母单号
                        expressSizeWeight.TrackingNoSub = string.Empty;
                        expressSizeWeight.Length = 0;
                        expressSizeWeight.Width = 0;
                        expressSizeWeight.Height = 0;
                        expressSizeWeight.Weight = 0;
                        expressSizeWeight.PackageCount = 0; // 这里取零，后面主单号数据上传时再更正

                        // 机器区分
                        expressSizeWeight.Machine = null;

                        expressSizeWeight.Express = express;
                        expressSizeWeight.Order = order;
                        expressSizeWeight.OrderCode = orderCode;

                        session.Save(expressSizeWeight);
                        session.Flush();
                    }

                    ExpressSizeWeightInfo? expressSizeWeightChild = session.Query<ExpressSizeWeightInfo>()
                        .Where(e => e.TrackingNo == trackingNoMain
                                    && e.TrackingNoSub == trackingNoSub)
                        .Take(1)
                        .SingleOrDefault();

                    bool create = false;
                    if (expressSizeWeightChild == null) {
                        create = true;

                        expressSizeWeightChild = new ExpressSizeWeightInfo();
                        expressSizeWeightChild.TrackingNo = trackingNoMain;
                        expressSizeWeightChild.TrackingNoSub = trackingNoSub;
                        expressSizeWeightChild.PackageCount = 1; // 可能这个箱数被后台修改过，所有只在创建时设置
                    }

                    StringBuilder editLogDescBuilder = new StringBuilder();
                    editLogDescBuilder.Append($"子单号:{trackingNoSub}");

                    // length
                    if (expressSizeWeightChild.Length != length) {
                        editLogDescBuilder.Append($"长:{expressSizeWeightChild.Length}=>{length}");
                    }

                    expressSizeWeightChild.Length = length;

                    // width
                    if (expressSizeWeightChild.Width != width) {
                        editLogDescBuilder.Append($"宽:{expressSizeWeightChild.Width}=>{width}");
                    }

                    expressSizeWeightChild.Width = width;

                    // height
                    if (expressSizeWeightChild.Height != height) {
                        editLogDescBuilder.Append($"高:{expressSizeWeightChild.Height}=>{height}");
                    }

                    expressSizeWeightChild.Height = height;

                    // weight
                    if (expressSizeWeightChild.Weight != weight) {
                        editLogDescBuilder.Append($"重量:{expressSizeWeightChild.Weight}=>{weight}");
                    }

                    expressSizeWeightChild.Weight = weight;

                    // 机器区分
                    expressSizeWeightChild.Machine = null;

                    expressSizeWeightChild.Express = express;
                    expressSizeWeightChild.Order = order;
                    expressSizeWeightChild.OrderCode = orderCode;

                    session.SaveOrUpdate(expressSizeWeightChild);
                    session.Flush();

                    if (create == true) {
                        string addLogDesc = $"子单号:{trackingNo}, 长:{expressSizeWeightChild.Length}, 宽:{expressSizeWeightChild.Width}, 高:{expressSizeWeightChild.Height}, 重量:{expressSizeWeightChild.Weight}, 箱数:{expressSizeWeightChild.PackageCount}";
                        expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightAdd, addLogDesc); // 记录日志用主单号
                    } else {
                        expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightAdd, editLogDescBuilder.ToString()); // 记录日志用主单号
                    }
                }
            });
        }
        catch (Exception exp) {
            log.Error(exp);

            return Json(new ExpressSizeWeightUploadResult {
                Flag = "failure",
                Code = "S01",
                Message = exp.Message,
                Data = false,
            });
        }

        log.Error("ExpressSizeWeightUploadController.Upload end");

        return Json(new ExpressSizeWeightUploadResult {
            Flag = "success",
            Code = string.Empty,
            Message = string.Empty,
            Data = true,
        });
    }
}