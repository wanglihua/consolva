using Aliyun.OSS;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Admin.Areas.Express.Models.ExpressVideo;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Express;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressPhoto)]
public class ExpressVideoController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly ExpressVideoService expressVideoService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;
    private readonly PassResService passResService;

    public ExpressVideoController(ISession session,
        ExpressVideoService expressVideoService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService,
        PassResService passResService
    ) {
        this.session = session;
        this.expressVideoService = expressVideoService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/express/video")]
    public IActionResult Index() {
        ExpressVideoIndexModel model = new ExpressVideoIndexModel();
        model.VideoTypeList = ExpressVideoType.GetList();

        int loginUserId = loginUserService.GetLoginUserId();
        model.CanExpressPhotoDelete = passResService.UserHasPassRes(loginUserId, PassRes.Express.ExpressPhotoDelete);

        return View("ExpressVideoIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/video/list")]
    public IActionResult List(ExpressVideoListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            queryParam.OrderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserNameQuery) == false) {
            string userNameQuery = queryParam.UserNameQuery.Trim();
            queryParam.UserId = session.Query<UserInfo>()
                .Where(e => e.LoginName == userNameQuery || e.UserName == userNameQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.Count<ExpressVideoInfo>(e => e.Id))
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<int> expressVideoIdList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .SetProjection(Projections.Property(nameof(ExpressVideoInfo.Id)))
            .List<int>();

        ICriteria listCriteria = session.CreateCriteria<ExpressVideoInfo>();
        // listCriteria = listCriteria.CreateAlias(nameof(ExpressVideoInfo.Customer), nameof(ExpressVideoInfo.Customer), JoinType.LeftOuterJoin);
        // listCriteria = listCriteria.CreateAlias(nameof(ExpressVideoInfo.Order), nameof(ExpressVideoInfo.Order), JoinType.LeftOuterJoin);
        // listCriteria = listCriteria.CreateAlias(nameof(ExpressVideoInfo.Express), nameof(ExpressVideoInfo.Express), JoinType.LeftOuterJoin);
        listCriteria = listCriteria.CreateAlias(nameof(ExpressVideoInfo.User), nameof(ExpressVideoInfo.User), JoinType.LeftOuterJoin);

        IList<ExpressVideoInfo> expressVideoList = listCriteria
            .Add(Restrictions.In($"{nameof(ExpressVideoInfo.Id)}", expressVideoIdList.ToArray()))
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .List<ExpressVideoInfo>();

        foreach (ExpressVideoInfo orderVideo in expressVideoList) {
            orderVideo.CustomerCodeCache = customerCodeService.GetCustomerCode(orderVideo.Customer?.Id ?? 0);
            orderVideo.VideoFullUrlCache = expressVideoService.GetVideoFullUrl(orderVideo, OssSiteLocType.China);
        }

        ExpressVideoListModel model = new ExpressVideoListModel(listPager);
        model.ExpressVideoList = expressVideoList;

        return View("ExpressVideoList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ExpressVideoListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<ExpressVideoInfo>();

        criteria.SetReadOnly(true);

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != ExpressVideoType.None) {
            ExpressVideoType typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressVideoInfo>(e => e.Type == typeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            string customerCodeQuery = queryParam.CustomerCodeQuery.Trim();
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<ExpressVideoInfo>(e => e.Customer!.Id == customerIdQuery));
        }

        if (queryParam.OrderId != null) {
            int orderId = queryParam.OrderId.Value;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressVideoInfo.Order)}.{nameof(ExpressVideoInfo.Order.Id)}", orderId));
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq(nameof(ExpressVideoInfo.TrackingNo), trackingNoQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressVideoInfo>(e => e.CreateTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressVideoInfo>(e => e.CreateTime < endDateQueryValue));
        }

        if (queryParam.UserId != 0) {
            int userId = queryParam.UserId;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressVideoInfo.User)}.{nameof(ExpressVideoInfo.User.Id)}", userId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/express/video/delete")]
    [AdminAuth(PassRes.Express.ExpressPhotoDelete)]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        IList<ExpressVideoInfo> videoList = new List<ExpressVideoInfo>();

        foreach (int id in idList) {
            ExpressVideoInfo expressVideo = session.Load<ExpressVideoInfo>(id);
            session.Delete(expressVideo);

            videoList.Add(expressVideo);
        }

        session.Flush();

        OssClient client = new OssClient(AliYunOssConfig.EndpointChina, AliYunOssConfig.AccessKeyId, AliYunOssConfig.AccessKeySecret);

        foreach (ExpressVideoInfo ev in videoList) {
            if (ev.StoreLoc == ExpressPhotoStoreLoc.Oss) {
                // 如果是OSS图片删除，就要做OSS图片删除
                try {
                    string objectName = Path.Combine(ev.StorePath, ev.StoreName).Replace("\\", "/");
                    client.DeleteObject(AliYunOssConfig.BucketNameChina, objectName);
                }
                catch (Exception exp) {
                    log.Error(exp);
                }
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}