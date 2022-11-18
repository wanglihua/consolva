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
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressPhoto)]
public class ExpressPhotoController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    private readonly CustomerCodeService customerCodeService;
    private readonly ExpressLogService expressLogService;
    private readonly ExpressPhotoFileService expressPhotoFileService;
    private readonly ExpressPhotoService expressPhotoService;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly LoginUserService loginUserService;

    private readonly ISession session;

    public ExpressPhotoController(ISession session,
        ExpressPhotoFileService expressPhotoFileService,
        ExpressPhotoService expressPhotoService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressLogService expressLogService)
    {
        this.session = session;
        this.expressPhotoFileService = expressPhotoFileService;
        this.expressPhotoService = expressPhotoService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    [HttpGet]
    [Route("/admin/express/photo")]
    public IActionResult Index()
    {
        var model = new ExpressPhotoIndexModel();
        model.PhotoTypeList = ExpressPhotoType.GetList();

        var isAdmin = loginUserService.IsAdmin;
        var passResIdMap = loginUserService.GetPassResIdMap();
        model.CanDelete = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Express.ExpressPhotoDelete);

        return View("ExpressPhotoIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/photo/list")]
    public IActionResult List(ExpressPhotoListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            queryParam.OrderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserNameQuery) == false)
        {
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

        foreach (var expressPhoto in expressPhotoList)
        {
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
    private ICriteria GetListCriteria(ExpressPhotoListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<ExpressPhotoInfo>();

        criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(ExpressPhotoInfo.Order), nameof(ExpressPhotoInfo.Order), JoinType.LeftOuterJoin);
        // criteria = criteria.CreateAlias(nameof(ExpressPhotoInfo.Express), nameof(ExpressPhotoInfo.Express), JoinType.LeftOuterJoin);
        // criteria = criteria.CreateAlias(nameof(ExpressPhotoInfo.User), nameof(ExpressPhotoInfo.User), JoinType.LeftOuterJoin);

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != ExpressPhotoType.None)
        {
            var typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.Type == typeQuery));
        }


        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            var customerCodeQuery = queryParam.CustomerCodeQuery.Trim();
            var customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.Customer!.Id == customerIdQuery));
        }

        if (queryParam.OrderId != 0)
        {
            int orderId = queryParam.OrderId;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressPhotoInfo.Order)}.{nameof(ExpressPhotoInfo.Order.Id)}", orderId));
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false)
        {
            var trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq(nameof(ExpressPhotoInfo.TrackingNo), trackingNoQuery));
        }

        if (queryParam.BeginDateQuery != null)
        {
            var beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.CreateTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null)
        {
            var endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressPhotoInfo>(e => e.CreateTime < endDateQueryValue));
        }

        if (queryParam.UserId != 0)
        {
            int userId = queryParam.UserId;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressPhotoInfo.User)}.{nameof(ExpressPhotoInfo.User.Id)}", userId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/express/photo/detail")]
    public IActionResult Detail()
    {
        session.DefaultReadOnly = true;

        var model = new ExpressPhotoDetailModel();
        model.PhotoTypeList = ExpressPhotoType.GetList();

        return View("ExpressPhotoDetail", model);
    }

    [HttpPost]
    [Route("/admin/express/photo/upload")]
    public IActionResult Upload(IFormFile imageFile)
    {
        if (imageFile == null) throw new ValidationException("文件未能上传成功！");

        var fileExt = expressPhotoFileService.GetFileExt(imageFile.FileName);
        if (fileExt.ToLower() != ".png".ToLower() && fileExt.ToLower() != ".jpg".ToLower() && fileExt.ToLower() != ".jpeg".ToLower())
            return Json(new Dictionary<string, object> {
                { "success", false },
                { "message", "只支持 png 和 jpg,jpeg 格式的图片文件！" }
            });

        var result = expressPhotoFileService.SaveImageFile(imageFile);

        var imageFileUrl = result.Item1;
        var storePath = result.Item2;
        var storeName = result.Item3;

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "上传成功！" },
            { "imageFileUrl", imageFileUrl },
            { "storePath", storePath },
            { "storeName", storeName }
        });
    }

    /// <summary>
    ///     这里只能创建不能编辑，要么删除了重新创建
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    [Route("/admin/express/photo/save")]
    [Transactional]
    public IActionResult Save(ExpressPhotoDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId)!;


        if (string.IsNullOrWhiteSpace(model.TrackingNo) == false && expressTrackingNoService.IsValidateTrackingNo(model.TrackingNo) == false) throw new ValidationException("快递单号格式不符合规范！");

        LogisticsOrderInfo? order = null;

        var orderCodeInput = model.OrderCode ?? string.Empty;

        if (string.IsNullOrEmpty(orderCodeInput) == false)
        {
            order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeInput)
                .Take(1)
                .SingleOrDefault();

            if (order == null) throw new ValidationException("指定的订单编号不存在！");
        }

        ExpressInfo? express = null;
        var trackingNo = model.TrackingNo;
        if (string.IsNullOrWhiteSpace(trackingNo) == false)
        {
            express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();

            if (express != null && order != null && (express.Order?.Id ?? 0) != order.Id)
            {
                throw new ValidationException("指定的快递单号和订单编号不匹配，可能快递单号属于别的订单！");
            }

            // 这一语句就保证如果订单编号没有录入，但快递单号对应的快递单有订单关联时，就设置成快递单关联订单
            order = express?.Order; // 也可能是 null。
        }

        CustomerInfo? customer = null;
        if (express != null)
        {
            customer = express.Customer;
        }
        else if (order != null) // 适用订单图片的情形
        {
            customer = order.Customer;
        }
        
        string orderCode = order?.Code ?? string.Empty;

        var expressPhoto = new ExpressPhotoInfo();

        expressPhoto.Status = ExpressPhotoStatus.Normal;
        expressPhoto.User = loginUser;

        expressPhoto.TrackingNo = trackingNo;
        expressPhoto.ExpressCompany = string.Empty;
        expressPhoto.Express = express;
        expressPhoto.Customer = customer;
        expressPhoto.Order = order;
        expressPhoto.OrderCode = orderCode;
        expressPhoto.Type = model.PhotoType;
        expressPhoto.StoreLoc = ExpressPhotoStoreLoc.Ecs; // 这里暂时是 Ecs 的
        expressPhoto.StorePath = model.StorePath;
        expressPhoto.StoreName = model.StoreName;

        session.Save(expressPhoto);
        session.Flush();

        if (express == null)
        {
            expressLogService.UserLog(expressPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型：{expressPhoto.Type?.Name}", loginUserId);
        }
        else
        {
            expressLogService.UserLog(express.Id, ExpressLogType.PhotoUpload, $"类型：{expressPhoto.Type?.Name}", loginUserId);
        }

        // 这里已经把这个逻辑给做了，就不用调用这些绑定method了
        // if (express != null)
        // {
        //     expressPhotoService.BindPhotoToExpress(expressPhoto.Id, express);
        // }
        // else if (logisticsOrder != null)
        // {
        //     expressPhotoService.BindPhotoToOrder(expressPhoto.Id, logisticsOrder);
        // }

        expressPhotoFileService.AddWatermark(expressPhoto.StorePath, expressPhoto.StoreName, expressPhoto.Type ?? ExpressPhotoType.None);
        expressPhotoService.SendAddOrderPhotoMessage(expressPhoto);

        // 如果有同单号的快递单号图片或视频，更新他们的关联订单和快递单
        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressPhotoInfo ep in expressPhotoList)
        {
            ep.Express = express;
            ep.Customer = customer;
            ep.Order = order;
            ep.OrderCode = orderCode;

            session.Update(ep);
        }

        session.Flush();

        IList<ExpressVideoInfo> expressVideoList = session.Query<ExpressVideoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressVideoInfo ev in expressVideoList)
        {
            ev.Express = express;
            ev.Customer = customer;
            ev.Order = order;
            ev.OrderCode = orderCode;

            session.Update(ev);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/express/photo/delete")]
    [AdminAuth(PassRes.Express.ExpressPhotoDelete)]
    public IActionResult Delete(int[] idList)
    {
        IList<ExpressPhotoInfo> photoList = new List<ExpressPhotoInfo>();

        foreach (var id in idList)
        {
            var expressPhoto = session.Load<ExpressPhotoInfo>(id);
            session.Delete(expressPhoto);

            photoList.Add(expressPhoto);
        }

        session.Flush();

        var client = new OssClient(AliYunOssConfig.EndpointChina, AliYunOssConfig.AccessKeyId, AliYunOssConfig.AccessKeySecret);

        foreach (var expressPhoto in photoList)
            if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss)
                // 如果是OSS图片删除，就要做OSS图片删除
                try
                {
                    var objectName = Path.Combine(expressPhoto.StorePath, expressPhoto.StoreName).Replace("\\", "/");
                    client.DeleteObject(AliYunOssConfig.BucketNameChina, objectName);
                }
                catch (Exception exp)
                {
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