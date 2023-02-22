using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoods;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsListByType)]
public class LogisticsGoodsController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsGoodsImageService logisticsGoodsImageService;
    private readonly LogisticsGoodsLogService logisticsGoodsLogService;
    private readonly LogisticsGoodsExcelService logisticsGoodsExcelService;

    public LogisticsGoodsController(ISession session,
        LoginUserService loginUserService,
        LogisticsGoodsImageService logisticsGoodsImageService,
        LogisticsGoodsLogService logisticsGoodsLogService,
        LogisticsGoodsExcelService logisticsGoodsExcelService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsGoodsImageService = logisticsGoodsImageService;
        this.logisticsGoodsLogService = logisticsGoodsLogService;
        this.logisticsGoodsExcelService = logisticsGoodsExcelService;
    }

    [HttpGet]
    [Route("/admin/logistics/goods/{typeId:int}")]
    public IActionResult Index(int typeId) {
        LogisticsGoodsType logisticsGoodsType = LogisticsGoodsType.GetById(typeId);
        if (logisticsGoodsType == null || logisticsGoodsType == LogisticsGoodsType.None) {
            throw new ValidationException("指定的物品类型不存在！");
        }

        LogisticsGoodsIndexModel model = new LogisticsGoodsIndexModel();
        model.Type = logisticsGoodsType;

        return View("LogisticsGoodsIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/list")]
    public IActionResult List(LogisticsGoodsListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGoodsInfo>();

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
        }

        LogisticsGoodsListModel model = new LogisticsGoodsListModel(listPager);
        model.LogisticsGoodsList = logisticsGoodsList;

        return View("LogisticsGoodsList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsGoodsListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

        LogisticsGoodsType logisticsGoodsType = queryParam.TypeQuery ?? LogisticsGoodsType.None;
        criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false) {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CnHsCodeQuery) == false) {
            string cnHsCodeQuery = queryParam.CnHsCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods!.CnHsCode).IsInsensitiveLike(cnHsCodeQuery, MatchMode.Exact)); // 相当于 Eq
        }

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false) {
            string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods!.CnHsName).IsInsensitiveLike(cnHsNameQuery, MatchMode.Exact));
        }

        if (string.IsNullOrWhiteSpace(queryParam.EnHsCodeQuery) == false) {
            string enHsCodeQuery = queryParam.EnHsCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods!.EnHsCode).IsInsensitiveLike(enHsCodeQuery, MatchMode.Exact));
        }

        if (string.IsNullOrWhiteSpace(queryParam.EnHsNameQuery) == false) {
            string enHsNameQuery = queryParam.EnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods!.EnHsName).IsInsensitiveLike(enHsNameQuery, MatchMode.Exact));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/goods/detail/{id:int?}")]
    public IActionResult Detail(int? id, int? statusId) {
        session.DefaultReadOnly = true;

        var loginUser = loginUserService.GetLoginUser();

        LogisticsGoodsInfo? logisticsGoods = null;

        bool isCreate = false;

        if (id == null || id.Value == 0) {
            logisticsGoods = new LogisticsGoodsInfo();


            if (loginUser != null) {
                logisticsGoods.AddUser = loginUser;
            }

            isCreate = true;
        } else {
            logisticsGoods = session.Get<LogisticsGoodsInfo>(id.Value);
            if (logisticsGoods == null) {
                throw new ValidationException("指定的数据项不存在！");
            }

            isCreate = false;
        }

        LogisticsGoodsDetailModel model = new LogisticsGoodsDetailModel();
        model.StatusId = statusId;

        SetupDetailModel(model, logisticsGoods);

        IList<ExpressPhotoInfo> createExpressPhotoList = logisticsGoodsImageService.GetGoodsCreateExpressImageList(new List<string>() { logisticsGoods.CreateExpressTrackingNo });
        logisticsGoods.CreateExpressPhotoListCache = createExpressPhotoList;

        IList<ExpressPhotoInfo> goodsImageItemList = logisticsGoodsImageService.GetGoodsRecentOrderImageList(logisticsGoods.Id);

        IList<int> orderIdList = goodsImageItemList
            .Where(e => e.Order != null)
            .Select(e => e.Order!.Id).Distinct()
            .OrderByDescending(e => e)
            .ToList();

        if (orderIdList.Count > 0) {
            int orderId = orderIdList[0];
            logisticsGoods.FirstOrderExpressPhotoListCache = goodsImageItemList.Where(e => e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
        }

        if (orderIdList.Count > 1) {
            int orderId = orderIdList[1];
            logisticsGoods.SecondOrderExpressPhotoListCache = goodsImageItemList.Where(e => e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
        }

        if (orderIdList.Count > 2) {
            int orderId = orderIdList[2];
            logisticsGoods.ThirdOrderExpressPhotoListCache = goodsImageItemList.Where(e => e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
        }

        model.IsCreate = isCreate;
        model.IsViewOnly = false;

        int goodsId = id ?? 0;
        model.LogList = session.Query<LogisticsGoodsLogInfo>()
            .Fetch(e => e.Goods)
            .Fetch(e => e.User)
            .Where(e => e.Goods.Id == goodsId)
            .ToList();

        return View("LogisticsGoodsDetail", model);
    }

    [HttpGet]
    [Route("/admin/logistics/goods/view/{id:int}")]
    public IActionResult View(int id) {
        session.DefaultReadOnly = true;

        LogisticsGoodsInfo? logisticsGoods = session.Get<LogisticsGoodsInfo>(id);
        if (logisticsGoods == null) {
            throw new ValidationException("指定的数据项不存在！");
        }

        LogisticsGoodsDetailModel model = new LogisticsGoodsDetailModel();

        SetupDetailModel(model, logisticsGoods);

        /*

        model.ImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => e.Goods.Id == id)
            .OrderBy(e => e.Id)
            .ToList();

        foreach (var image in model.ImageList)
        {
            image.ImageFullUrlCache = logisticsGoodsImageService.GetFullImageUrl(image.SubPath, image.FileName);
        }
        
         */

        model.IsCreate = false;
        model.IsViewOnly = true;

        int goodsId = id;
        model.LogList = session.Query<LogisticsGoodsLogInfo>()
            .Fetch(e => e.Goods)
            .Fetch(e => e.User)
            .Where(e => e.Goods.Id == goodsId)
            .ToList();

        return View("LogisticsGoodsDetail", model);
    }

    [NonAction]
    private void SetupDetailModel(LogisticsGoodsDetailModel model, LogisticsGoodsInfo logisticsGoods) {
        model.TypeList = LogisticsGoodsType.GetInnerList();
        model.StatusList = LogisticsGoodsStatus.GetList();

        model.LogisticsGoodsId = logisticsGoods.Id;
        model.Name = logisticsGoods.Name;
        model.Type = logisticsGoods.Type;
        model.BrandName = logisticsGoods.BrandName;
        model.CreateExpressTrackingNo = logisticsGoods.CreateExpressTrackingNo;
        model.Status = logisticsGoods.Status;

        model.AddUser = logisticsGoods.AddUser;

        HsGoodsInfo? hsGoods = logisticsGoods.HsGoods;

        if (hsGoods != null) {
            model.HsGoodsId = hsGoods.Id;

            model.CnHsCode = hsGoods.CnHsCode;
            model.CnHsName = hsGoods.CnHsName;
            model.EnHsCode = hsGoods.EnHsCode;
            model.EnHsName = hsGoods.EnHsName;
            model.Elem01 = hsGoods.Elem01;
            model.Elem02 = hsGoods.Elem02;
            model.Elem03 = hsGoods.Elem03;
            model.Elem04 = hsGoods.Elem04;
            model.Elem05 = hsGoods.Elem05;
            model.Elem06 = hsGoods.Elem06;
            model.Elem07 = hsGoods.Elem07;
            model.Elem08 = hsGoods.Elem08;
        }
    }

    [HttpPost]
    [Route("/admin/logistics/goods/save")]
    [Transactional]
    public IActionResult Save(LogisticsGoodsSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        bool isCreate = model.LogisticsGoodsId == 0;

        LogisticsGoodsInfo logisticsGoods = SaveInternal(model);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "isCreate", isCreate },
            { "id", logisticsGoods.Id }
        });
    }

    [NonAction]
    private LogisticsGoodsInfo SaveInternal(LogisticsGoodsSaveModel model) {
        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        int logisticsGoodsId = model.LogisticsGoodsId;

        bool isCreate = logisticsGoodsId == 0;

        LogisticsGoodsInfo? logisticsGoods = null;

        if (isCreate) {
            logisticsGoods = new LogisticsGoodsInfo();
            logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;
            logisticsGoods.AddUser = loginUser;
        } else {
            logisticsGoods = session.Get<LogisticsGoodsInfo>(logisticsGoodsId);
        }

        logisticsGoods.Cate = LogisticsGoodsCate.None;
        logisticsGoods.Name = model.Name.Trim();
        logisticsGoods.Type = model.Type;
        logisticsGoods.BrandName = model.BrandName ?? string.Empty;
        logisticsGoods.CreateExpressTrackingNo = model.CreateExpressTrackingNo;

        logisticsGoods.UpdateUser = loginUser;

        if (isCreate) {
            session.Save(logisticsGoods);
            session.Flush();

            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Create, logisticsGoods.Id, loginUserId);
        } else {
            logisticsGoods.UpdateUser = loginUser;
            session.Update(logisticsGoods);
            session.Flush();

            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Modify, logisticsGoods.Id, loginUserId);
        }

        session.Flush();

        int hsGoodsId = model.HsGoodsId;

        if (model.HsGoodsAutoSelect == false && string.IsNullOrWhiteSpace(model.CnHsCode) == false) {
            string cnHsName = model.CnHsName ?? string.Empty;
            int count = session.Query<HsGoodsInfo>().Where(e => e.CnHsName == cnHsName).Count();
            if (count != 0) {
                throw new ValidationException($"{cnHsName}已经存在，请直接选择！"); // 用异常可以回滚前面的保存，事务会撤回
            }

            HsGoodsInfo hsGoodsInfo = new HsGoodsInfo();

            hsGoodsInfo.CnHsCode = model.CnHsCode ?? string.Empty;
            hsGoodsInfo.CnHsName = model.CnHsName ?? string.Empty;
            hsGoodsInfo.EnHsCode = model.EnHsCode ?? string.Empty;
            hsGoodsInfo.EnHsName = model.EnHsName ?? string.Empty;
            hsGoodsInfo.Elem01 = model.Elem01 ?? string.Empty;
            hsGoodsInfo.Elem02 = model.Elem02 ?? string.Empty;
            hsGoodsInfo.Elem03 = model.Elem03 ?? string.Empty;
            hsGoodsInfo.Elem04 = model.Elem04 ?? string.Empty;
            hsGoodsInfo.Elem05 = model.Elem05 ?? string.Empty;
            hsGoodsInfo.Elem06 = model.Elem06 ?? string.Empty;
            hsGoodsInfo.Elem07 = model.Elem07 ?? string.Empty;
            hsGoodsInfo.Elem08 = model.Elem08 ?? string.Empty;

            hsGoodsInfo.AddUser = loginUser;

            session.Save(hsGoodsInfo);
            session.Flush();

            hsGoodsId = hsGoodsInfo.Id;
        }

        if (hsGoodsId != 0) {
            logisticsGoods.HsGoods = session.Load<HsGoodsInfo>(hsGoodsId);
        } else {
            logisticsGoods.HsGoods = null;
        }

        session.Update(logisticsGoods);
        session.Flush();

        // 保存图片， 其实这个现在一般也不用了
        if (model.NewImageList != null && model.NewImageList.Count != 0) {
            foreach (var image in model.NewImageList) {
                LogisticsGoodsImageInfo goodsImage = new LogisticsGoodsImageInfo();
                goodsImage.Goods = logisticsGoods;
                goodsImage.StorePath = image.StorePath;
                goodsImage.StoreName = image.StoreName;

                session.Save(goodsImage);
            }
        }

        session.Flush();

        // 当前没照片状态的，才需要这样的判断
        if (logisticsGoods.Status == LogisticsGoodsStatus.PhotoRequired) {
            bool hasPhoto = logisticsGoodsImageService.HasGoodsImage(logisticsGoods.Id);
            if (hasPhoto) {
                logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;

                session.Update(logisticsGoods);
                session.Flush();
            }
        }

        return logisticsGoods;
    }

    [HttpPost]
    [Route("/admin/logistics/goods/save/confirm")]
    [Transactional]
    public IActionResult SaveConfirm(LogisticsGoodsDetailModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        bool isCreate = model.LogisticsGoodsId == 0;

        LogisticsGoodsInfo logisticsGoods = SaveInternal(model);

        /* todo 暂时去掉这个检查，以后再加上
        if (logisticsGoods.Status == LogisticsGoodsStatus.PhotoRequired) {
            throw new ValidationException("物品还没有任何关联照片，请设置物品创建单号，有单号相关订单照片后，再确认该物品名称！");
        }
        */

        logisticsGoods.Status = LogisticsGoodsStatus.Confirmed;

        logisticsGoods.UpdateUser = loginUser;
        session.Update(logisticsGoods);
        session.Flush();

        logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Confirm, logisticsGoods.Id, loginUserId);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存确认成功！" },
            { "isCreate", isCreate },
            { "id", logisticsGoods.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("未指定要删除的物品！");
        }

        foreach (int id in idList) {
            LogisticsGoodsInfo logisticsGoods = session.Load<LogisticsGoodsInfo>(id);
            session.Delete(logisticsGoods);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/enable")]
    [Transactional]
    public IActionResult Enable(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("未指定要启用的物品！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => idList.Contains(e.Id)).ToList();
        foreach (LogisticsGoodsInfo goods in goodsList) {
            if (goods.Status != LogisticsGoodsStatus.Disabled) {
                throw new ValidationException($"物品【{goods.Name}】当前状态不是已禁用状态！");
            }
        }

        foreach (LogisticsGoodsInfo goods in goodsList) {
            goods.UpdateUser = loginUser;
            goods.Status = LogisticsGoodsStatus.Confirmed;
            session.Update(goods);
        }

        session.Flush();

        foreach (LogisticsGoodsInfo goods in goodsList) {
            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Enable, goods.Id, loginUserId);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "启用成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/disable")]
    [Transactional]
    public IActionResult Disable(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("未指定要禁用的物品！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => idList.Contains(e.Id)).ToList();
        foreach (LogisticsGoodsInfo goods in goodsList) {
            if (goods.Status != LogisticsGoodsStatus.Confirmed) {
                throw new ValidationException($"物品【{goods.Name}】当前状态不是已确认状态！");
            }
        }

        foreach (LogisticsGoodsInfo goods in goodsList) {
            goods.UpdateUser = loginUser;
            goods.Status = LogisticsGoodsStatus.Disabled;
            session.Update(goods);
        }

        session.Flush();

        foreach (LogisticsGoodsInfo goods in goodsList) {
            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Disable, goods.Id, loginUserId);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "禁用成功！" },
        });
    }

    #region Image

    [HttpPost]
    [Route("/admin/logistics/goods/image/upload")]
    public IActionResult ImageUpload(IFormFile imageFile) {
        if (imageFile == null) {
            throw new ValidationException("文件未能上传成功！");
        }

        string fileExt = logisticsGoodsImageService.GetFileExt(imageFile.FileName);
        if (fileExt.ToLower() != ".png".ToLower() && fileExt.ToLower() != ".jpg".ToLower() && fileExt.ToLower() != ".jpeg".ToLower()) {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", "只支持 png 和 jpg,jpeg 格式的图片文件！" },
            });
        }

        var result = logisticsGoodsImageService.SaveImageFile(imageFile);

        string imageFileUrl = result.Item1;
        string subPath = result.Item2;
        string fileName = result.Item3;

        LogisticsGoodsImageModel model = new LogisticsGoodsImageModel();
        model.ImageFileUrl = imageFileUrl;
        model.StorePath = subPath;
        model.StoreName = fileName;

        return View("LogisticsGoodsImage", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/image/delete")]
    public IActionResult ImageDelete(int imageId) {
        var logisticsGoodsImage = session.Get<LogisticsGoodsImageInfo>(imageId);
        if (logisticsGoodsImage == null) {
            throw new ValidationException("指定的客服仓库交互图片不存在！");
        }

        var goodsId = logisticsGoodsImage.Goods.Id;
        var logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsId);
        if (logisticsGoods == null) {
            throw new ValidationException("图片对应的客服仓库交互不存在！");
        }

        session.Delete(logisticsGoodsImage);

        logisticsGoodsImageService.DeleteFile(logisticsGoodsImage);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "图片删除成功！" },
        });
    }

    #endregion

    [HttpPost]
    [Route("/admin/logistics/goods/export")]
    public IActionResult Export(LogisticsGoodsListQueryParam queryParam) {
        IList<LogisticsGoodsInfo> logisticsGoodsList = GetListCriteria(queryParam)
            .List<LogisticsGoodsInfo>();

        var type = queryParam.TypeQuery;

        string fileName = type?.Name + "扩展名" + ".xlsx";
        string sheetName = type?.Name + "扩展名";
        var excel = logisticsGoodsExcelService.Export(logisticsGoodsList, sheetName);

        return File(excel, ContentType.Xlsx, fileName);
    }
}