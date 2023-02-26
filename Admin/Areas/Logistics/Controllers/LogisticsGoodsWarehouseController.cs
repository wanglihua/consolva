using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsWarehouse)]
public class LogisticsGoodsWarehouseController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsGoodsImageService logisticsGoodsImageService;
    private readonly LogisticsGoodsLogService logisticsGoodsLogService;
    private readonly LogisticsGoodsWarehouseExcelService logisticsGoodsWarehouseExcelService;

    public LogisticsGoodsWarehouseController(ISession session,
        LoginUserService loginUserService,
        LogisticsGoodsImageService logisticsGoodsImageService,
        LogisticsGoodsLogService logisticsGoodsLogService,
        LogisticsGoodsWarehouseExcelService logisticsGoodsWarehouseExcelService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsGoodsImageService = logisticsGoodsImageService;
        this.logisticsGoodsLogService = logisticsGoodsLogService;
        this.logisticsGoodsWarehouseExcelService = logisticsGoodsWarehouseExcelService;
    }

    [HttpGet]
    [Route("/admin/logistics/goods/warehouse/")]
    public IActionResult Index() {
        LogisticsGoodsWarehouseIndexModel model = new LogisticsGoodsWarehouseIndexModel();
        // model.GoodsTypeList = LogisticsGoodsType.GetInnerList();

        return View("LogisticsGoodsWarehouseIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/list")]
    public IActionResult List(LogisticsGoodsWarehouseListQueryParam queryParam) {
        int totalCount = GetWarehouseGoodsSearchListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetWarehouseGoodsSearchListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(EntityNoUpdate.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGoodsInfo>();

        // IList<int> userIdList = logisticsGoodsList.Select(e => e.AddUser.Id).ToList();
        // IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
        }

        LogisticsGoodsWarehouseListModel model = new LogisticsGoodsWarehouseListModel(listPager);
        model.LogisticsGoodsList = logisticsGoodsList;

        return View("LogisticsGoodsWarehouseList", model);
    }

    [NonAction]
    public ICriteria GetWarehouseGoodsSearchListCriteria(LogisticsGoodsWarehouseListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

        // 这里不需要状态过滤
        // int statusNewId = LogisticsGoodsStatus.New.Id;
        // criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.StatusId == statusNewId));

        if (queryParam.GoodsTypeQuery != null && queryParam.GoodsTypeQuery != LogisticsGoodsType.None) {
            LogisticsGoodsType logisticsGoodsType = queryParam.GoodsTypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));
        }

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false) {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/detail")]
    public IActionResult Detail(LogisticsGoodsWarehouseListQueryParam queryParam) {
        LogisticsGoodsWarehouseDetailModel model = new LogisticsGoodsWarehouseDetailModel();

        model.TypeList = LogisticsGoodsType.GetInnerList();

        if (queryParam.GoodsId != 0) {
            LogisticsGoodsInfo? logisticsGoods = session.Get<LogisticsGoodsInfo>(queryParam.GoodsId);
            if (logisticsGoods == null) {
                throw new ValidationException("指定的物品不存在！");
            }

            model.Name = logisticsGoods.Name;
            model.Type = logisticsGoods.Type;
            model.BrandName = logisticsGoods.BrandName;
            model.CreateExpressTrackingNo = logisticsGoods.CreateExpressTrackingNo;
        } else {
            model.Name = queryParam.NameQuery ?? string.Empty;
            model.Type = queryParam.GoodsTypeQuery ?? LogisticsGoodsType.None;

            model.CreateExpressTrackingNo = string.Empty;
        }

        /*
         
        model.ImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => e.Goods.Id == queryParam.GoodsId)
            .OrderBy(e => e.Id)
            .ToList();

        foreach (var image in model.ImageList)
        {
            image.ImageFullUrlCache = logisticsGoodsImageService.GetFullImageUrl(image.SubPath, image.FileName);
        }
        
        */

        return View("LogisticsGoodsWarehouseDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/add/save")]
    [Transactional]
    public IActionResult Save(LogisticsGoodsWarehouseSaveModel model) {
        if (!this.ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        bool isCreate = true;
        LogisticsGoodsInfo? logisticsGoods = null;

        if (model.GoodsId != 0) {
            logisticsGoods = session.Get<LogisticsGoodsInfo>(model.GoodsId);
            if (logisticsGoods == null) {
                throw new ValidationException("指定的物品不存在！");
            }

            if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed) {
                throw new ValidationException("物品当前状态已确认，不能修改！");
            }

            int goodsId = model.GoodsId;
            string goodsName = model.Name;
            int count = session
                .Query<LogisticsGoodsInfo>()
                .Where(e => e.Id != goodsId
                            && e.Name == goodsName
                            && e.BrandName == model.BrandName)
                .Count();

            if (count != 0) {
                throw new ValidationException("该扩展名已经存在，不能添加相同的扩展名！");
            }

            isCreate = false;
        } else {
            logisticsGoods = new LogisticsGoodsInfo();

            string goodsName = model.Name;
            int count = session.Query<LogisticsGoodsInfo>()
                .Where(e => e.Name == goodsName
                            && e.BrandName == model.BrandName)
                .Count();

            if (count != 0) {
                throw new ValidationException("该扩展名已经存在，不能添加相同的扩展名！");
            }

            isCreate = true;
        }

        logisticsGoods.Name = model.Name.Trim();
        logisticsGoods.Type = model.Type;

        logisticsGoods.BrandName = model.BrandName ?? string.Empty;
        logisticsGoods.CreateExpressTrackingNo = model.CreateExpressTrackingNo ?? string.Empty;

        logisticsGoods.AddUser = loginUser;
        logisticsGoods.UpdateUser = loginUser;

        if (isCreate) {
            logisticsGoods.Status = LogisticsGoodsStatus.PhotoRequired; // 后面再改这个状态

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

        // 保存图片，这个其实已经不用了
        if (model.NewImageList != null && model.NewImageList.Count != 0) {
            foreach (var image in model.NewImageList) {
                LogisticsGoodsImageInfo interactImage = new LogisticsGoodsImageInfo();
                interactImage.Goods = logisticsGoods;
                interactImage.StorePath = image.StorePath;
                interactImage.StoreName = image.StoreName;

                session.Save(interactImage);
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

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/delete")]
    public IActionResult Delete(int[] idList) {
        IList<int> goodsIdList = idList.ToList();

        foreach (int id in goodsIdList) {
            LogisticsGoodsInfo? logisticsGoods = session.Get<LogisticsGoodsInfo>(id);
            if (logisticsGoods != null) {
                if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed) {
                    throw new ValidationException($"扩展名 {logisticsGoods.Name} 是已确认状态，不能在仓库添加时删除！");
                }
            }
        }

        IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Id))
            .ToList();

        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Goods.Id))
            .ToList();

        IList<LogisticsGoodsImageInfo> logisticsGoodsImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => goodsIdList.Contains(e.Goods.Id))
            .ToList();

        foreach (int goodsId in goodsIdList) {
            LogisticsGoodsInfo? logisticsGoods = logisticsGoodsList
                .Where(e => e.Id == goodsId)
                .Take(1)
                .SingleOrDefault();
            if (logisticsGoods == null) {
                throw new ValidationException($"指定的物品不存在！#{goodsId}");
            }

            var orderGoods = expressGoodsList
                .Where(e => e.Goods.Id == goodsId)
                .Take(1)
                .SingleOrDefault();

            if (orderGoods != null) {
                throw new ValidationException($"物品 {logisticsGoods.Name}, 在订单物品中有被引用，不能删除！");
            }
        }

        // 数据库事务
        Transactional.Wrap(session, () => {
            foreach (var goodsImage in logisticsGoodsImageList) {
                session.Delete(goodsImage);
            }

            foreach (var logisticsGoods in logisticsGoodsList) {
                session.Delete(logisticsGoods);
            }

            session.Flush();
        });

        foreach (var goodsImage in logisticsGoodsImageList) {
            try {
                logisticsGoodsImageService.DeleteFile(goodsImage);
            }
            catch (Exception exp) {
                log.Error(exp);
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    #region Image

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/image/upload")]
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
        string storePath = result.Item2;
        string storeName = result.Item3;

        LogisticsGoodsWarehouseImageModel model = new LogisticsGoodsWarehouseImageModel();
        model.ImageFileUrl = imageFileUrl;
        model.StorePath = storePath;
        model.StoreName = storeName;

        return View("LogisticsGoodsWarehouseImage", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/image/delete")]
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
    [Route("/admin/logistics/goods/warehouse/import")]
    public IActionResult Import(IFormFile excelFile) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        var loginUser = loginUserService.GetLoginUser();
        logisticsGoodsWarehouseExcelService.Import(excelFile, loginUser);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/export")]
    public IActionResult Export(LogisticsGoodsWarehouseListQueryParam queryParam) {
        IList<LogisticsGoodsInfo> logisticsGoodsList = GetWarehouseGoodsSearchListCriteria(queryParam)
            .List<LogisticsGoodsInfo>();

        string fileName = "扩展名添加" + ".xlsx";
        var excel = logisticsGoodsWarehouseExcelService.Export(logisticsGoodsList);
        return File(excel, ContentType.Xlsx, fileName);
    }
}