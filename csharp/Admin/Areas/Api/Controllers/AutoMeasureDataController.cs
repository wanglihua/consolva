using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Api.Models.AutoMeasureData;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Service.AutoMeasure;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Api.Controllers;

[NoLoginRequired]
[ApiArea]
public class AutoMeasureDataController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session = null;
    private ExpressTrackingNoService expressTrackingNoService = null;
    private ExpressLogService expressLogService = null;

    public AutoMeasureDataController(ISession session, ExpressTrackingNoService expressTrackingNoService, ExpressLogService expressLogService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    [HttpPost]
    [Route("/auto/api/measure/upload")]
    public IActionResult Upload()
    {
        ITransaction transaction = null;
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

            AutoMeasureDataUploadRequestModel requestModel = JsonConvert.DeserializeObject<AutoMeasureDataUploadRequestModel>(requestBodyText);

            // 检查数据完整
            if (requestModel.Data == null)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS001.Code);
            }

            #region sign check

            // 检查 app_key，sign 等

            string requestAppKey = requestModel.AppKey;

            AutoMeasureMachineInfo autoMeasureMachine = session
                .Query<AutoMeasureMachineInfo>()
                .Where(e => e.AppKey == requestAppKey)
                .Take(1)
                .SingleOrDefault();

            string secret = AutoMeasureMachineConfig.Secret;
            if (autoMeasureMachine != null)
            {
                secret = autoMeasureMachine.Secret;
            }

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

            string trackingNumber = requestModel.Data?.TrackingNumber;

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
            if (expressTrackingNoService.IsValidateTrackingNo(requestModel.Data?.TrackingNumber) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS002.Code); // 快递单号格式不合法！
            }

            // 检查 长宽高，重量 数据

            if (decimal.TryParse(requestModel.Data.Length, out decimal length) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (length <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (decimal.TryParse(requestModel.Data.Width, out decimal width) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (width <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (decimal.TryParse(requestModel.Data.Height, out decimal height) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (height <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS004.Code); // 长宽高数据错误！
            }

            if (decimal.TryParse(requestModel.Data.Weight, out decimal weight) == false)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS005.Code); // 重量数据错误！
            }

            if (weight <= 0)
            {
                throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS005.Code); // 重量数据错误！
            }

            int indexOfMinus = trackingNumber.IndexOf("-");

            // 是简单的母单号
            if (indexOfMinus == -1)
            {
                LogisticsExpressInfo logisticsExpress = session.Query<LogisticsExpressInfo>()
                    .Where(e => e.TrackingNo == trackingNumber)
                    .Take(1)
                    .SingleOrDefault();

                bool create = false;
                if (logisticsExpress == null)
                {
                    create = true;

                    logisticsExpress = new LogisticsExpressInfo();

                    // 只在新增的时候保存这些个字段
                    logisticsExpress.Status = LogisticsExpressStatus.Sign;
                    logisticsExpress.SignTime = DateTime.Now;
                    logisticsExpress.GoodsType = LogisticsGoodsType.None;
                    logisticsExpress.TrackingNo = trackingNumber;
                    logisticsExpress.PackageCount = 1; // 可能这个箱数被后台修改过，所有只在创建时设置
                }

                if (logisticsExpress.PackageCount <= 0) // 可能这个母单号是，子单号尺寸上传时自动创建的。那时这个数量设置的是0，如果是这种情况，这里要纠正过来。
                {
                    logisticsExpress.PackageCount = 1;
                }

                StringBuilder editLogDescBuilder = new StringBuilder();

                // length
                if (create == false && logisticsExpress.Length != length)
                {
                    editLogDescBuilder.Append($"长:{logisticsExpress.Length}=>{length}");
                }

                logisticsExpress.Length = length;

                // width
                if (create == false && logisticsExpress.Width != width)
                {
                    editLogDescBuilder.Append($"宽:{logisticsExpress.Width}=>{width}");
                }

                logisticsExpress.Width = width;

                // height
                if (create == false && logisticsExpress.Height != height)
                {
                    editLogDescBuilder.Append($"高:{logisticsExpress.Height}=>{height}");
                }

                logisticsExpress.Height = height;

                // weight
                if (create == false && logisticsExpress.Weight != weight)
                {
                    editLogDescBuilder.Append($"重量:{logisticsExpress.Weight}=>{weight}");
                }

                logisticsExpress.Weight = weight;

                // 机器区分
                logisticsExpress.AppKey = requestAppKey;
                logisticsExpress.Machine = autoMeasureMachine;
                logisticsExpress.SignWarehouse = autoMeasureMachine?.Warehouse;

                session.SaveOrUpdate(logisticsExpress);
                session.Flush();

                if (create == true)
                {
                    string addLogDesc = $"长:{logisticsExpress.Length}, 宽:{logisticsExpress.Width}, 高:{logisticsExpress.Height}, 重量:{logisticsExpress.Weight}, 箱数:{logisticsExpress.PackageCount}";
                    expressLogService.Log(trackingNumber, ExpressLogType.SizeWeightAdd, addLogDesc);
                }
                else
                {
                    expressLogService.Log(trackingNumber, ExpressLogType.SizeWeightEdit, editLogDescBuilder.ToString());
                }
            }
            else // 是子单号
            {
                // 获取母单号
                string trackingNumberParent = trackingNumber.Substring(0, indexOfMinus);

                // 检查母单号的合法性
                if (expressTrackingNoService.IsValidateTrackingNo(trackingNumberParent) == false)
                {
                    throw new AutoMeasureErrorException(AutoMeasureDataUploadResponseModel.ErrorS002.Code); // 快递单号格式不合法！
                }

                LogisticsExpressInfo logisticsExpress = session.Query<LogisticsExpressInfo>()
                    .Where(e => e.TrackingNo == trackingNumberParent) // trackingNumberParent 母单号
                    .Take(1)
                    .SingleOrDefault();

                // 如果这时母单号还不存在，就新增一个
                if (logisticsExpress == null)
                {
                    logisticsExpress = new LogisticsExpressInfo();

                    logisticsExpress.Status = LogisticsExpressStatus.Sign;
                    logisticsExpress.SignTime = DateTime.Now;
                    logisticsExpress.GoodsType = LogisticsGoodsType.None;
                    logisticsExpress.TrackingNo = trackingNumberParent; // trackingNumberParent 母单号
                    logisticsExpress.Length = null;
                    logisticsExpress.Width = null;
                    logisticsExpress.Height = null;
                    logisticsExpress.Weight = null;
                    logisticsExpress.PackageCount = 0;

                    session.Save(logisticsExpress);
                    session.Flush();
                }

                LogisticsExpressChildInfo logisticsExpressChild = session.Query<LogisticsExpressChildInfo>()
                    .Where(e => e.TrackingNo == trackingNumber) // trackingNumber 子单号
                    .Take(1)
                    .SingleOrDefault();

                bool create = false;
                if (logisticsExpressChild == null)
                {
                    create = true;

                    logisticsExpressChild = new LogisticsExpressChildInfo();
                    logisticsExpressChild.PackageCount = 1; // 可能这个箱数被后台修改过，所有只在创建时设置
                }

                StringBuilder editLogDescBuilder = new StringBuilder();
                editLogDescBuilder.Append($"子单号:{trackingNumber}");

                logisticsExpressChild.Parent = logisticsExpress;
                logisticsExpressChild.TrackingNo = trackingNumber; // trackingNumber 子单号

                // length
                if (logisticsExpressChild.Length != length)
                {
                    editLogDescBuilder.Append($"长:{logisticsExpressChild.Length}=>{length}");
                }

                logisticsExpressChild.Length = length;

                // width
                if (logisticsExpressChild.Width != width)
                {
                    editLogDescBuilder.Append($"宽:{logisticsExpressChild.Width}=>{width}");
                }

                logisticsExpressChild.Width = width;

                // height
                if (logisticsExpressChild.Height != height)
                {
                    editLogDescBuilder.Append($"高:{logisticsExpressChild.Height}=>{height}");
                }

                logisticsExpressChild.Height = height;

                // weight
                if (logisticsExpressChild.Weight != weight)
                {
                    editLogDescBuilder.Append($"重量:{logisticsExpressChild.Weight}=>{weight}");
                }

                logisticsExpressChild.Weight = weight;

                session.SaveOrUpdate(logisticsExpressChild);
                session.Flush();

                if (create == true)
                {
                    string addLogDesc = $"子单号:{trackingNumber}, 长:{logisticsExpressChild.Length}, 宽:{logisticsExpressChild.Width}, 高:{logisticsExpressChild.Height}, 重量:{logisticsExpressChild.Weight}, 箱数:{logisticsExpressChild.PackageCount}";
                    expressLogService.Log(trackingNumberParent, ExpressLogType.SizeWeightAdd, addLogDesc);
                }
                else
                {
                    expressLogService.Log(trackingNumberParent, ExpressLogType.SizeWeightAdd, editLogDescBuilder.ToString());
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