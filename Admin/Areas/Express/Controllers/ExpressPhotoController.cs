using Aliyun.OSS;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NLog;
using shunda.Admin.Areas.Express.Models.ExpressPhoto;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
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
public class ExpressPhotoController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    private readonly CustomerCodeService customerCodeService;
    private readonly ExpressLogService expressLogService;
    private readonly ExpressPhotoFileService expressPhotoFileService;
    private readonly ExpressPhotoService expressPhotoService;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly LoginUserService loginUserService;
    private readonly PassResService passResService;

    private readonly ISession session;

    public ExpressPhotoController(ISession session,
        ExpressPhotoFileService expressPhotoFileService,
        ExpressPhotoService expressPhotoService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressLogService expressLogService,
        PassResService passResService) {
        this.session = session;
        this.expressPhotoFileService = expressPhotoFileService;
        this.expressPhotoService = expressPhotoService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/express/photo")]
    public IActionResult Index() {
        var model = new ExpressPhotoIndexModel();
        model.PhotoTypeList = ExpressPhotoType.GetList();

        int loginUserId = loginUserService.GetLoginUserId();
        model.CanExpressPhotoDelete = passResService.UserHasPassRes(loginUserId, PassRes.Express.ExpressPhotoDelete);

        return View("ExpressPhotoIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/photo/list")]
    public IActionResult List(ExpressPhotoListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            queryParam.OrderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserNameQuery) == false) {
            var userNameQuery = queryParam.UserNameQuery.Trim();
            queryParam.UserId = session.Query<UserInfo>()
                .Where(e => e.LoginName == userNameQuery || e.UserName == userNameQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.Count<ExpressPhotoInfo>(e => e.Id))
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var expressPhotoIdList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .SetProjection(Projections.Property(nameof(ExpressPhotoInfo.Id)))
            .List<int>();

        var listCriteria = session.CreateCriteria<ExpressPhotoInfo>();
        listCriteria = listCriteria.CreateAlias(nameof(ExpressPhotoInfo.Customer), nameof(ExpressPhotoInfo.Customer), JoinType.LeftOuterJoin);
        listCriteria = listCriteria.CreateAlias(nameof(ExpressPhotoInfo.Order), nameof(ExpressPhotoInfo.Order), JoinType.LeftOuterJoin);
        // listCriteria = listCriteria.CreateAlias(nameof(ExpressPhotoInfo.Express), nameof(ExpressPhotoInfo.Express), JoinType.LeftOuterJoin);
        listCriteria = listCriteria.CreateAlias(nameof(ExpressPhotoInfo.User), nameof(ExpressPhotoInfo.User), JoinType.LeftOuterJoin);

        var expressPhotoList = listCriteria
            .Add(Restrictions.In($"{nameof(ExpressPhotoInfo.Id)}", expressPhotoIdList.ToArray()))
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .List<ExpressPhotoInfo>();

        foreach (var expressPhoto in expressPhotoList) {
            if (expressPhoto.Customer != null)
                expressPhoto.CustomerCodeCache = customerCodeService.GetCustomerCode(expressPhoto.Customer?.Id ?? 0);
            else
                expressPhoto.CustomerCodeCache = string.Empty;

            expressPhoto.BigImageFullUrlCache = expressPhotoFileService.GetBigImageFullUrlForSite(expressPhoto, OssSiteLocType.China);
            expressPhoto.SmallImageFullUrlCache = expressPhotoFileService.GetSmallImageFullUrlForSite(expressPhoto, OssSiteLocType.China);
        }

        var model = new ExpressPhotoListModel(listPager);
        model.ExpressPhotoList = expressPhotoList;

        return View("ExpressPhotoList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ExpressPhotoListQueryParam queryParam) {
        var criteria = session.CreateCriteria<ExpressPhotoInfo>();

        criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(ExpressPhotoInfo.Order), nameof(ExpressPhotoInfo.Order), JoinType.LeftOuterJoin);
        // criteria = criteria.CreateAlias(nameof(ExpressPhotoInfo.Express), nameof(ExpressPhotoInfo.Express), JoinType.LeftOuterJoin);
        // criteria = criteria.CreateAlias(nameof(ExpressPhotoInfo.User), nameof(ExpressPhotoInfo.User), JoinType.LeftOuterJoin);

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != ExpressPhotoType.None) {
            var typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.Type == typeQuery));
        }


        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            var customerCodeQuery = queryParam.CustomerCodeQuery.Trim();
            var customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.Customer!.Id == customerIdQuery));
        }

        if (queryParam.OrderId != null) {
            int orderId = queryParam.OrderId.Value;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressPhotoInfo.Order)}.{nameof(ExpressPhotoInfo.Order.Id)}", orderId));
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            var trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq(nameof(ExpressPhotoInfo.TrackingNo), trackingNoQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            var beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.CreateTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null) {
            var endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.CreateTime < endDateQueryValue));
        }

        if (queryParam.UserId != 0) {
            int userId = queryParam.UserId;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressPhotoInfo.User)}.{nameof(ExpressPhotoInfo.User.Id)}", userId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/express/photo/delete")]
    [AdminAuth(PassRes.Express.ExpressPhotoDelete)]
    public IActionResult Delete(int[] idList) {
        IList<ExpressPhotoInfo> photoList = new List<ExpressPhotoInfo>();

        foreach (var id in idList) {
            var expressPhoto = session.Load<ExpressPhotoInfo>(id);
            session.Delete(expressPhoto);

            photoList.Add(expressPhoto);
        }

        session.Flush();

        var client = new OssClient(AliYunOssConfig.EndpointChina, AliYunOssConfig.AccessKeyId, AliYunOssConfig.AccessKeySecret);

        foreach (var expressPhoto in photoList)
            if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss)
                // 如果是OSS图片删除，就要做OSS图片删除
                try {
                    var objectName = Path.Combine(expressPhoto.StorePath, expressPhoto.StoreName).Replace("\\", "/");
                    client.DeleteObject(AliYunOssConfig.BucketNameChina, objectName);
                }
                catch (Exception exp) {
                    log.Error(exp);
                }
            else
                // 如果是服务器图片就做服务器图片删除
                expressPhotoFileService.DeleteFile(expressPhoto);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}