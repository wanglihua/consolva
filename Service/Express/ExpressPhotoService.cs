using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.Express;

/// <summary>
/// 这个类大部分处理数据业务处理相关的
/// 跟图片和文件有关的逻辑放在 ExpressPhotoFileService 中
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressPhotoService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    // private CustomerMessageService customerMessageService = null;
    private ExpressPhotoFileService expressPhotoFileService;

    public ExpressPhotoService(ISession session,
        // CustomerMessageService customerMessageService,
        ExpressPhotoFileService expressPhotoFileService) {
        this.session = session;
        // this.customerMessageService = customerMessageService;
        this.expressPhotoFileService = expressPhotoFileService;
    }

    public IList<ExpressPhotoInfo> GetExpressPhotoList(IList<string> trackingNoList) {
        if (trackingNoList == null || trackingNoList.Count == 0) {
            return new List<ExpressPhotoInfo>();
        }

        IList<ExpressPhotoInfo> orderPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .OrderBy(e => e.Type)
            .ToList();

        foreach (ExpressPhotoInfo orderPhoto in orderPhotoList) {
            orderPhoto.BigImageFullUrlCache = expressPhotoFileService.GetBigImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
            orderPhoto.SmallImageFullUrlCache = expressPhotoFileService.GetSmallImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
        }

        return orderPhotoList;
    }

    public IList<ExpressPhotoInfo> GetExpressPhotoList(IList<int> expressIdList) {
        if (expressIdList == null || expressIdList.Count == 0) {
            return new List<ExpressPhotoInfo>();
        }

        var expressList = session.Query<ExpressInfo>().Where(e => expressIdList.Contains(e.Id)).ToList();
        var trackingNoList = expressList.Select(e => e.TrackingNo).ToList();

        IList<ExpressPhotoInfo> orderPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .OrderBy(e => e.Type)
            .ToList();

        foreach (ExpressPhotoInfo orderPhoto in orderPhotoList) {
            orderPhoto.BigImageFullUrlCache = expressPhotoFileService.GetBigImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
            orderPhoto.SmallImageFullUrlCache = expressPhotoFileService.GetSmallImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
        }

        return orderPhotoList;
    }

    /// <summary>
    /// 绑定快递单号到图片，添加和更新快递单时调用
    /// </summary>
    /// <param name="expressId"></param>
    public void BindExpressByTrackingNo(int expressId) {
        ExpressInfo? express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        string trackingNo = express.TrackingNo.Trim();

        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        foreach (ExpressPhotoInfo expressPhoto in expressPhotoList) {
            expressPhoto.Express = express;
            expressPhoto.Customer = express.Customer; // 有可能为 null，也有可能不是
            expressPhoto.Order = express.Order; // 有可能为 null，也有可能不是
            expressPhoto.OrderCode = express.Order?.Code ?? string.Empty;

            session.Update(expressPhoto);
        }

        session.Flush();
    }
}