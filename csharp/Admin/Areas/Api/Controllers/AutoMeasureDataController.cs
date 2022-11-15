using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Api.Models.AutoMeasureData;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Order;
using shunda.Service.AutoMeasure;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Api.Controllers;

[NoLoginRequired]
[ApiArea]
public class AutoMeasureDataController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private ExpressTrackingNoService expressTrackingNoService;
    private ExpressLogService expressLogService;

    public AutoMeasureDataController(ISession session, ExpressTrackingNoService expressTrackingNoService, ExpressLogService expressLogService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    [HttpPost]
    [Route("/admin/api/auto/measure/upload")]
    public IActionResult Upload()
    {
        ITransaction? transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            // Request.EnableRewind(); // 3.0 中方法名称有改变 using Microsoft.AspNetCore.Http.Internal; 
            StreamReader reader = new StreamReader(Request.Body);
            string requestBodyText = reader.ReadToEnd();
            // Request.Body.Position = 0;

            // 记录请求，方便接下来调试
            // log.Warn("requestBodyText");
            // log.Warn(requestBodyText);

            AutoMeasureDataUploadRequestModel? requestModel = JsonConvert.DeserializeObject<AutoMeasureDataUploadRequestModel>(requestBodyText);

            // 检查数据完整
            if (requestModel?.Data == null)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS001.Code);
            }

            #region sign check

            // 检查 app_key，sign 等

            string requestAppKey = requestModel.AppKey;

            AutoMeasureMachineInfo? autoMeasureMachine = session
                .Query<AutoMeasureMachineInfo>()
                .Where(e => e.AppKey == requestAppKey)
                .Take(1)
                .SingleOrDefault();
            string secret = autoMeasureMachine?.Secret ?? string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("app_key");
            sb.Append(requestModel.AppKey);
            sb.Append("method");
            sb.Append("weight");
            sb.Append("timestamp");
            sb.Append(requestModel.TimeStamp);

            IDictionary<string, string> dictData = new Dictionary<string, string>();
            // dictData["boxType"] = "express"; 签名不需要
            dictData["weight"] = requestModel.Data.Weight;
            dictData["trackingNumber"] = requestModel.Data.TrackingNumber;
            // dictData["length"] = requestModel.Data.Length; 签名不需要
            // dictData["width"] = requestModel.Data.Width; 签名不需要           
            // dictData["height"] = requestModel.Data.Height; 签名不需要
            // dictData["volume"] = requestModel.Data.Volume; 签名不需要
            dictData["barcode"] = "";
            string dataStr = JsonConvert.SerializeObject(dictData);
            sb.Append(dataStr);
            sb.Append(secret);

            string clearText = sb.ToString();
            string signCalculate = Md5Util.Encrypt(clearText).ToUpper();

            if (signCalculate.ToUpper() != requestModel.Sign.ToUpper())
            {
                log.Error("签名加密数据错误，requestBodyText：" + requestBodyText);
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS100.Code); // 参数签名错误，非法调用！
            }

            #endregion

            // 检查快递单号，箱号

            string trackingNumber = requestModel.Data?.TrackingNumber ?? string.Empty;

            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS002.Code); // 快递单号格式不合法！
            }

            trackingNumber = trackingNumber.ToUpper();

            // 这一类的子单号，还是当做母单号看待
            if (trackingNumber.StartsWith("JD") && trackingNumber.EndsWith("-1-1-"))
            {
                trackingNumber = trackingNumber.Replace("-1-1-", "");
            }

            // 检查快递单号
            if (expressTrackingNoService.IsValidateTrackingNo(requestModel.Data?.TrackingNumber ?? string.Empty) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS002.Code); // 快递单号格式不合法！
            }

            // 检查 长宽高，重量 数据

            if (decimal.TryParse(requestModel.Data?.Length, out decimal length) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (length <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (decimal.TryParse(requestModel.Data?.Width, out decimal width) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (width <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (decimal.TryParse(requestModel.Data?.Height, out decimal height) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (height <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (decimal.TryParse(requestModel.Data?.Weight, out decimal weight) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS005.Code); // 重量数据错误！
            }

            if (weight <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS005.Code); // 重量数据错误！
            }

            // 获取母单号
            string trackingNoMain = trackingNumber.Trim();

            int indexOfMinus = trackingNumber.IndexOf("-");
            if (indexOfMinus != -1)
            {
                trackingNoMain = trackingNumber.Substring(0, indexOfMinus);
            }

            // 可能的关联快递单号
            ExpressInfo? express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNoMain)
                .Take(1)
                .SingleOrDefault();
            LogisticsOrderInfo? order = express?.Order; // 可能为空
            string orderCode = order?.Code ?? string.Empty;

            // 是简单的母单号
            if (indexOfMinus == -1)
            {
                ExpressSizeWeightInfo? expressSizeWeight = session.Query<ExpressSizeWeightInfo>()
                    .Where(e => e.TrackingNo == trackingNoMain
                                && e.TrackingNoSub == string.Empty) // 取主单号
                    .Take(1)
                    .SingleOrDefault();

                bool create = false;
                if (expressSizeWeight == null)
                {
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
                if (create == false && expressSizeWeight.Length != length)
                {
                    editLogDescBuilder.Append($"长:{expressSizeWeight.Length}=>{length}");
                }

                expressSizeWeight.Length = length;

                // width
                if (create == false && expressSizeWeight.Width != width)
                {
                    editLogDescBuilder.Append($"宽:{expressSizeWeight.Width}=>{width}");
                }

                expressSizeWeight.Width = width;

                // height
                if (create == false && expressSizeWeight.Height != height)
                {
                    editLogDescBuilder.Append($"高:{expressSizeWeight.Height}=>{height}");
                }

                expressSizeWeight.Height = height;

                // weight
                if (create == false && expressSizeWeight.Weight != weight)
                {
                    editLogDescBuilder.Append($"重量:{expressSizeWeight.Weight}=>{weight}");
                }

                expressSizeWeight.Weight = weight;

                // 机器区分
                expressSizeWeight.Machine = autoMeasureMachine;

                expressSizeWeight.Express = express;
                expressSizeWeight.Order = order;
                expressSizeWeight.OrderCode = orderCode;

                session.SaveOrUpdate(expressSizeWeight);
                session.Flush();

                if (create == true)
                {
                    string addLogDesc = $"长:{expressSizeWeight.Length}, 宽:{expressSizeWeight.Width}, 高:{expressSizeWeight.Height}, 重量:{expressSizeWeight.Weight}, 箱数:{expressSizeWeight.PackageCount}";
                    expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightAdd, addLogDesc);
                }
                else
                {
                    expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightEdit, editLogDescBuilder.ToString());
                }
            }
            else // 是子单号
            {
                string trackingNoSub = trackingNumber.Trim();

                // 检查母单号的合法性
                if (expressTrackingNoService.IsValidateTrackingNo(trackingNoMain) == false)
                {
                    throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS002.Code); // 快递单号格式不合法！
                }

                ExpressSizeWeightInfo? expressSizeWeight = session.Query<ExpressSizeWeightInfo>()
                    .Where(e => e.TrackingNo == trackingNoMain // trackingNoMain 母单号
                                && e.TrackingNoSub == string.Empty
                    )
                    .Take(1)
                    .SingleOrDefault();

                // 如果这时母单号还不存在，就新增一个
                if (expressSizeWeight == null)
                {
                    expressSizeWeight = new ExpressSizeWeightInfo();

                    expressSizeWeight.TrackingNo = trackingNoMain; // trackingNoMain 母单号
                    expressSizeWeight.TrackingNoSub = string.Empty;
                    expressSizeWeight.Length = 0;
                    expressSizeWeight.Width = 0;
                    expressSizeWeight.Height = 0;
                    expressSizeWeight.Weight = 0;
                    expressSizeWeight.PackageCount = 0; // 这里取零，后面主单号数据上传时再更正

                    // 机器区分
                    expressSizeWeight.Machine = autoMeasureMachine;

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
                if (expressSizeWeightChild == null)
                {
                    create = true;

                    expressSizeWeightChild = new ExpressSizeWeightInfo();
                    expressSizeWeightChild.TrackingNo = trackingNoMain;
                    expressSizeWeightChild.TrackingNoSub = trackingNoSub;
                    expressSizeWeightChild.PackageCount = 1; // 可能这个箱数被后台修改过，所有只在创建时设置
                }

                StringBuilder editLogDescBuilder = new StringBuilder();
                editLogDescBuilder.Append($"子单号:{trackingNoSub}");

                // length
                if (expressSizeWeightChild.Length != length)
                {
                    editLogDescBuilder.Append($"长:{expressSizeWeightChild.Length}=>{length}");
                }

                expressSizeWeightChild.Length = length;

                // width
                if (expressSizeWeightChild.Width != width)
                {
                    editLogDescBuilder.Append($"宽:{expressSizeWeightChild.Width}=>{width}");
                }

                expressSizeWeightChild.Width = width;

                // height
                if (expressSizeWeightChild.Height != height)
                {
                    editLogDescBuilder.Append($"高:{expressSizeWeightChild.Height}=>{height}");
                }

                expressSizeWeightChild.Height = height;

                // weight
                if (expressSizeWeightChild.Weight != weight)
                {
                    editLogDescBuilder.Append($"重量:{expressSizeWeightChild.Weight}=>{weight}");
                }

                expressSizeWeightChild.Weight = weight;

                // 机器区分
                expressSizeWeightChild.Machine = autoMeasureMachine;

                expressSizeWeightChild.Express = express;
                expressSizeWeightChild.Order = order;
                expressSizeWeightChild.OrderCode = orderCode;                

                session.SaveOrUpdate(expressSizeWeightChild);
                session.Flush();

                if (create == true)
                {
                    string addLogDesc = $"子单号:{trackingNumber}, 长:{expressSizeWeightChild.Length}, 宽:{expressSizeWeightChild.Width}, 高:{expressSizeWeightChild.Height}, 重量:{expressSizeWeightChild.Weight}, 箱数:{expressSizeWeightChild.PackageCount}";
                    expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightAdd, addLogDesc); // 记录日志用主单号
                }
                else
                {
                    expressLogService.Log(trackingNoMain, ExpressLogType.SizeWeightAdd, editLogDescBuilder.ToString()); // 记录日志用主单号
                }
            }

            transaction.Commit();

            return Json(AutoMeasureDataUploadResponseModel.Success);
        }
        catch (AutoMeasureErrorException exp)
        {
            return Json(AutoMeasureDataUploadResponseModel.GetByCode(exp.Message));
        }
        catch (Exception exp)
        {
            transaction?.Rollback();

            log.Error(exp);

            return Json(AutoMeasureDataUploadResponseModel.ErrorS001); // 系统异常！
        }
    }
}