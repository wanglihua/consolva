using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Core.Util;
using Domain.Logistics;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Service;
using Service.Logistics;
using Web.Areas.Admin.Models.LogisticsGoodsWarehouse;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.Goods.GoodsWarehouse)]
    public class LogisticsGoodsWarehouseController : Controller
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private ISession session = null;
        private LoginUserService loginUserService = null;
        private LogisticsGoodsTypeService logisticsGoodsTypeService = null;
        private LogisticsGoodsImageService logisticsGoodsImageService = null;
        private LogisticsGoodsLogService logisticsGoodsLogService = null;

        public LogisticsGoodsWarehouseController(ISession session,
            LoginUserService loginUserService,
            LogisticsGoodsTypeService logisticsGoodsTypeService,
            LogisticsGoodsImageService logisticsGoodsImageService,
            LogisticsGoodsLogService logisticsGoodsLogService)
        {
            this.session = session;
            this.loginUserService = loginUserService;
            this.logisticsGoodsTypeService = logisticsGoodsTypeService;
            this.logisticsGoodsImageService = logisticsGoodsImageService;
            this.logisticsGoodsLogService = logisticsGoodsLogService;
        }

        [HttpGet]
        [Route("/admin/logistics/goods/warehouse/")]
        [NoHibernateAutoTransaction]
        public IActionResult Index()
        {
            LogisticsGoodsWarehouseIndexModel model = new LogisticsGoodsWarehouseIndexModel();
            // model.GoodsTypeList = LogisticsGoodsType.GetInnerList();
            model.GoodsCateList = LogisticsGoodsCate.GetList();

            return View("LogisticsGoodsWarehouseIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/goods/warehouse/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(LogisticsGoodsWarehouseListQueryParam queryParam)
        {
            int totalCount = GetWarehouseGoodsSearchListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<LogisticsGoodsInfo> logisticsGoodsList = GetWarehouseGoodsSearchListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<LogisticsGoodsInfo>();

            IList<int> userIdList = logisticsGoodsList.Select(e => e.AddUserId).ToList();
            IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();

            foreach (LogisticsGoodsInfo logisticsGoodsInfo in logisticsGoodsList)
            {
                // logisticsGoodsInfo
                UserInfo userInfo = userList.Where(e => e.Id == logisticsGoodsInfo.AddUserId).Take(1).SingleOrDefault();
                if (userInfo != null)
                {
                    logisticsGoodsInfo.AddUserUserNameCache = $"{userInfo.UserName}({userInfo.LoginName})";
                }
            }

            LogisticsGoodsWarehouseListModel model = new LogisticsGoodsWarehouseListModel(listPager);
            model.LogisticsGoodsList = logisticsGoodsList;

            return View("LogisticsGoodsWarehouseList", model);
        }

        [NonAction]
        public ICriteria GetWarehouseGoodsSearchListCriteria(LogisticsGoodsWarehouseListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

            criteria = criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

            // 这里不需要状态过滤
            // int statusNewId = LogisticsGoodsStatus.New.Id;
            // criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.StatusId == statusNewId));

            LogisticsGoodsCate logisticsGoodsCate = queryParam.CateQuery;
            if (logisticsGoodsCate != null && logisticsGoodsCate != LogisticsGoodsCate.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Cate == logisticsGoodsCate));
            }

            LogisticsGoodsType logisticsGoodsType = queryParam.TypeQuery;
            if (logisticsGoodsType != null)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));
            }

            string nameQuery = queryParam.NameQuery;
            if (string.IsNullOrWhiteSpace(nameQuery) == false)
            {
                nameQuery = nameQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
            }

            bool? hasBrandQuery = queryParam.HasBrandQuery;
            if (hasBrandQuery.HasValue)
            {
                bool hasBrandQueryValue = hasBrandQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.HasBrand == hasBrandQueryValue));
            }

            bool? hasBatteryQuery = queryParam.HasBatteryQuery;
            if (hasBatteryQuery.HasValue)
            {
                bool hasBatteryQueryValue = hasBatteryQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.HasBattery == hasBatteryQueryValue));
            }

            bool? hasBambooQuery = queryParam.HasBambooQuery;
            if (hasBambooQuery.HasValue)
            {
                bool hasBambooQueryValue = hasBambooQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.HasBamboo == hasBambooQueryValue));
            }

            bool? hasWoodPackageQuery = queryParam.HasWoodPackageQuery;
            if (hasWoodPackageQuery.HasValue)
            {
                bool hasWoodPackageQueryValue = hasWoodPackageQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.HasWoodPackage == hasWoodPackageQueryValue));
            }

            bool? isSpecialQuery = queryParam.IsSpecialQuery;
            if (isSpecialQuery.HasValue)
            {
                bool isSpecialQueryValue = isSpecialQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.IsSpecial == isSpecialQueryValue));
            }

            bool? isForbiddenQuery = queryParam.IsForbiddenQuery;
            if (isForbiddenQuery.HasValue)
            {
                bool isForbiddenQueryValue = isForbiddenQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.IsForbidden == isForbiddenQueryValue));
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/logistics/goods/warehouse/detail")]
        [NoHibernateAutoTransaction]
        public IActionResult Detail(LogisticsGoodsWarehouseListQueryParam queryParam)
        {
            LogisticsGoodsWarehouseDetailModel model = new LogisticsGoodsWarehouseDetailModel();

            model.CateList = LogisticsGoodsCate.GetList();
            model.TypeList = LogisticsGoodsType.GetInnerList();

            if (queryParam.GoodsId != 0)
            {
                LogisticsGoodsInfo logisticsGoodsInfo = session.Get<LogisticsGoodsInfo>(queryParam.GoodsId);
                if (logisticsGoodsInfo == null)
                {
                    throw new ValidationException("指定的物品不存在！");
                }

                model.Cate = logisticsGoodsInfo.Cate;
                model.Name = logisticsGoodsInfo.Name;
                model.Type = logisticsGoodsInfo.Type;
                model.HasBrand = logisticsGoodsInfo.HasBrand;
                model.BrandName = logisticsGoodsInfo.BrandName;
                model.HasBattery = logisticsGoodsInfo.HasBattery;
                model.HasBamboo = logisticsGoodsInfo.HasBamboo;
                model.HasWoodPackage = logisticsGoodsInfo.HasWoodPackage;
                model.IsSpecial = logisticsGoodsInfo.IsSpecial;
                model.IsForbidden = logisticsGoodsInfo.IsForbidden;
                model.CreateExpressTrackingNo = logisticsGoodsInfo.CreateExpressTrackingNo;
            }
            else
            {
                model.Cate = queryParam.CateQuery;
                model.Name = queryParam.NameQuery;
                model.Type = queryParam.TypeQuery;

                if (queryParam.HasBrandQuery.HasValue)
                {
                    model.HasBrand = queryParam.HasBrandQuery.Value;
                }
                else
                {
                    model.HasBrand = false;
                }

                if (queryParam.HasBatteryQuery.HasValue)
                {
                    model.HasBattery = queryParam.HasBatteryQuery.Value;
                }
                else
                {
                    model.HasBattery = false;
                }

                if (queryParam.HasBambooQuery.HasValue)
                {
                    model.HasBamboo = queryParam.HasBambooQuery.Value;
                }
                else
                {
                    model.HasBamboo = false;
                }

                if (queryParam.HasWoodPackageQuery.HasValue)
                {
                    model.HasWoodPackage = queryParam.HasWoodPackageQuery.Value;
                }
                else
                {
                    model.HasWoodPackage = false;
                }

                if (queryParam.IsSpecialQuery.HasValue)
                {
                    model.IsSpecial = queryParam.IsSpecialQuery.Value;
                }
                else
                {
                    model.IsSpecial = false;
                }

                if (queryParam.IsForbiddenQuery.HasValue)
                {
                    model.IsForbidden = queryParam.IsForbiddenQuery.Value;
                }
                else
                {
                    model.IsForbidden = false;
                }

                model.CreateExpressTrackingNo = string.Empty;

                // 如果是新增的话，就自动判断类型
                model.Type = GetGoodsTypeByDetailModel(model);
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
        public IActionResult Save(LogisticsGoodsWarehouseDetailModel model)
        {
            if (!this.ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            bool isCreate = true;
            LogisticsGoodsInfo logisticsGoods = new LogisticsGoodsInfo();

            if (model.HasBrand && string.IsNullOrWhiteSpace(model.BrandName))
            {
                throw new ValidationException("品牌扩展名请填写品牌名！");
            }

            if (model.GoodsId != 0)
            {
                logisticsGoods = session.Get<LogisticsGoodsInfo>(model.GoodsId);
                if (logisticsGoods == null)
                {
                    throw new ValidationException("指定的物品不存在！");
                }

                if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed)
                {
                    throw new ValidationException("物品当前状态已确认，不能修改！");
                }

                int goodsId = model.GoodsId;
                string goodsName = model.Name;
                int count = session
                    .Query<LogisticsGoodsInfo>()
                    .Where(e => e.Id != goodsId
                                && e.Name == goodsName
                                && e.HasBrand == model.HasBrand
                                && e.BrandName == model.BrandName
                                && e.HasBattery == model.HasBattery
                                && e.HasBamboo == model.HasBamboo
                                && e.HasWoodPackage == model.HasWoodPackage
                                && e.IsSpecial == model.IsSpecial
                                && e.IsForbidden == model.IsForbidden)
                    .Count();

                if (count != 0)
                {
                    throw new ValidationException("该扩展名已经存在，不能添加相同的扩展名！");
                }

                isCreate = false;
            }
            else
            {
                logisticsGoods = new LogisticsGoodsInfo();

                string goodsName = model.Name;
                int count = session.Query<LogisticsGoodsInfo>()
                    .Where(e => e.Name == goodsName
                                && e.HasBrand == model.HasBrand
                                && e.BrandName == model.BrandName
                                && e.HasBattery == model.HasBattery
                                && e.HasBamboo == model.HasBamboo
                                && e.HasWoodPackage == model.HasWoodPackage
                                && e.IsSpecial == model.IsSpecial
                                && e.IsForbidden == model.IsForbidden)
                    .Count();

                if (count != 0)
                {
                    throw new ValidationException("该扩展名已经存在，不能添加相同的扩展名！");
                }

                isCreate = true;
            }

            logisticsGoods.Cate = model.Cate;
            logisticsGoods.Name = model.Name?.Trim();
            logisticsGoods.Type = model.Type;
            logisticsGoods.HasBrand = model.HasBrand;

            if (model.HasBrand)
            {
                logisticsGoods.BrandName = model.BrandName;
            }
            else
            {
                logisticsGoods.BrandName = String.Empty;
            }

            logisticsGoods.HasBattery = model.HasBattery;
            logisticsGoods.HasBamboo = model.HasBamboo;
            logisticsGoods.HasWoodPackage = model.HasWoodPackage;
            logisticsGoods.IsSpecial = model.IsSpecial;
            logisticsGoods.IsForbidden = model.IsForbidden;
            logisticsGoods.CreateExpressTrackingNo = model.CreateExpressTrackingNo;

            UserInfo userInfo = session.Get<UserInfo>(loginUserService.GetLoginUserId());

            if (userInfo != null)
            {
                logisticsGoods.AddUserId = userInfo.Id;
            }

            if (isCreate)
            {
                logisticsGoods.Status = LogisticsGoodsStatus.NoPhoto; // 后面再改这个状态

                session.Save(logisticsGoods);
                session.Flush();

                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Create, logisticsGoods.Id, loginUserId);
            }
            else
            {
                logisticsGoods.UpdateUser = loginUser;
                session.Update(logisticsGoods);
                session.Flush();

                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Modify, logisticsGoods.Id, loginUserId);
            }

            session.Flush();

            // 保存图片，这个其实已经不用了
            if (model.NewImageList != null && model.NewImageList.Count != 0)
            {
                foreach (var image in model.NewImageList)
                {
                    LogisticsGoodsImageInfo interactImage = new LogisticsGoodsImageInfo();
                    interactImage.Goods = logisticsGoods;
                    interactImage.SubPath = image.SubPath;
                    interactImage.FileName = image.FileName;

                    session.Save(interactImage);
                }
            }

            session.Flush();

            // 当前没照片状态的，才需要这样的判断
            if (logisticsGoods.Status == LogisticsGoodsStatus.NoPhoto)
            {
                bool hasPhoto = logisticsGoodsImageService.HasGoodsImage(logisticsGoods.Id);
                if (hasPhoto)
                {
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
        public IActionResult Delete(int[] idList)
        {
            foreach (int id in idList)
            {
                LogisticsGoodsInfo logisticsGoods = session.Get<LogisticsGoodsInfo>(id);
                if (logisticsGoods != null)
                {
                    if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed)
                    {
                        throw new ValidationException($"扩展名 {logisticsGoods.Name} 是已确认状态，不能在仓库添加时删除！");
                    }
                }
            }

            IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
                .Where(e => idList.Contains(e.Id))
                .ToList();

            IList<LogisticsOrderGoodsInfo> logisticsOrderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
                .Where(e => idList.Contains(e.GoodsId) || idList.Contains(e.GoodsAdminId.Value))
                .ToList();

            IList<LogisticsGoodsImageInfo> logisticsGoodsImageList = session.Query<LogisticsGoodsImageInfo>()
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (int id in idList)
            {
                LogisticsGoodsInfo logisticsGoods = logisticsGoodsList.Where(e => e.Id == id).Take(1).SingleOrDefault();
                if (logisticsGoods == null)
                {
                    throw new ValidationException($"指定的物品不存在！#{id}");
                }

                var orderGoods = logisticsOrderGoodsList
                    .Where(e => e.GoodsId == id || e.GoodsAdminId == id)
                    .Take(1)
                    .SingleOrDefault();

                if (orderGoods != null)
                {
                    throw new ValidationException($"物品 {logisticsGoods.Name}, 在订单物品中有被引用，不能删除！");
                }
            }

            foreach (var goodsImage in logisticsGoodsImageList)
            {
                session.Delete(goodsImage);
            }

            foreach (var logisticsGoods in logisticsGoodsList)
            {
                session.Delete(logisticsGoods);
            }

            session.Flush();

            foreach (var goodsImage in logisticsGoodsImageList)
            {
                try
                {
                    logisticsGoodsImageService.DeleteFile(goodsImage);
                }
                catch (Exception exp)
                {
                    log.Error(exp);
                }
            }

            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "删除成功！" },
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/warehouse/get/goods/type")]
        [NoHibernateAutoTransaction]
        public IActionResult GetGoodsType(LogisticsGoodsWarehouseDetailModel model)
        {
            LogisticsGoodsType type = GetGoodsTypeByDetailModel(model);

            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "获取物品类型成功！" },
                { "goodsType", type.Id },
            });
        }

        [NonAction]
        private LogisticsGoodsType GetGoodsTypeByDetailModel(LogisticsGoodsWarehouseDetailModel model)
        {
            LogisticsGoodsType logisticsGoodsType = logisticsGoodsTypeService.GetGoodsType(
                model.HasBrand,
                model.HasBattery,
                model.HasBamboo,
                model.HasWoodPackage,
                model.IsSpecial,
                model.IsForbidden);

            return logisticsGoodsType;
        }

        #region Image

        [HttpPost]
        [Route("/admin/logistics/goods/warehouse/image/upload")]
        public IActionResult ImageUpload(IFormFile imageFile)
        {
            if (imageFile == null)
            {
                throw new ValidationException("文件未能上传成功！");
            }

            string fileExt = logisticsGoodsImageService.GetFileExt(imageFile.FileName);
            if (fileExt.ToLower() != ".png".ToLower() && fileExt.ToLower() != ".jpg".ToLower() && fileExt.ToLower() != ".jpeg".ToLower())
            {
                return Json(new Dictionary<string, object>() {
                    { "success", false },
                    { "message", "只支持 png 和 jpg,jpeg 格式的图片文件！" },
                });
            }

            var result = logisticsGoodsImageService.SaveImageFile(imageFile);

            string imageFileUrl = result.Item1;
            string subPath = result.Item2;
            string fileName = result.Item3;

            LogisticsGoodsWarehouseImageModel model = new LogisticsGoodsWarehouseImageModel();
            model.ImageFileUrl = imageFileUrl;
            model.SubPath = subPath;
            model.FileName = fileName;

            return View("LogisticsGoodsWarehouseImage", model);
        }

        [HttpPost]
        [Route("/admin/logistics/goods/warehouse/image/delete")]
        public IActionResult ImageDelete(int imageId)
        {
            var logisticsGoodsImage = session.Get<LogisticsGoodsImageInfo>(imageId);
            if (logisticsGoodsImage == null)
            {
                throw new ValidationException("指定的客服仓库交互图片不存在！");
            }

            var goodsId = logisticsGoodsImage.Goods?.Id ?? 0;
            var logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsId);
            if (logisticsGoods == null)
            {
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
        [NoHibernateAutoTransaction]
        public IActionResult Import(IFormFile excelFile)
        {
            if (excelFile == null)
            {
                throw new ValidationException("导入失败，文件未上传！");
            }

            IList<string> seaNameList = session
                .Query<LogisticsGoodsInfo>()
                .Where(e => e.Cate == LogisticsGoodsCate.Sea)
                .Select(e => e.Name)
                .Distinct()
                .ToList();

            IDictionary<string, bool> seaNameDict = new Dictionary<string, bool>();
            foreach (string seaName in seaNameList)
            {
                if (seaNameDict.ContainsKey(seaName) == false)
                {
                    seaNameDict.Add(seaName, true);
                }
            }

            IList<string> airNameList = session
                .Query<LogisticsGoodsInfo>()
                .Where(e => e.Cate == LogisticsGoodsCate.Air)
                .Select(e => e.Name)
                .Distinct()
                .ToList();

            IDictionary<string, bool> airNameDict = new Dictionary<string, bool>();
            foreach (string airName in airNameList)
            {
                if (airNameDict.ContainsKey(airName) == false)
                {
                    airNameDict.Add(airName, true);
                }
            }

            IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
            }

            IList<LogisticsGoodsInfo> logisticsGoodsList = new List<LogisticsGoodsInfo>();

            // 略过标题行，所以 rowIndex = 1 开始
            int lastRowNum = sheet.LastRowNum;
            for (int rowIndex = 1; rowIndex <= lastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);

                if (row == null)
                {
                    break;
                }

                // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
                int cellIndex = 0;
                int lastCellNum = row.LastCellNum;
                for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++)
                {
                    row.GetCell(cellIndex)?.SetCellType(CellType.String);
                }

                LogisticsGoodsInfo logisticsGoods = new LogisticsGoodsInfo();

                cellIndex = 0;

                string cateName = row.GetCell(cellIndex++)?.StringCellValue;
                if (string.IsNullOrWhiteSpace(cateName))
                {
                    break;
                }

                cateName = cateName.Trim();

                if (cateName == LogisticsGoodsCate.Air.Name)
                {
                    logisticsGoods.Cate = LogisticsGoodsCate.Air;
                }
                else if (cateName == LogisticsGoodsCate.Sea.Name)
                {
                    logisticsGoods.Cate = LogisticsGoodsCate.Sea;
                }
                else
                {
                    throw new ValidationException($"第{rowIndex}行数据 类别不对，应该是 {LogisticsGoodsCate.Sea.Name} 或者 {LogisticsGoodsCate.Air.Name} !");
                }

                // 扩展名
                string name = row.GetCell(cellIndex++)?.StringCellValue;
                if (string.IsNullOrWhiteSpace(name))
                {
                    break;
                }

                name = name.Trim();

                if (logisticsGoods.Cate == LogisticsGoodsCate.Sea)
                {
                    if (seaNameDict.ContainsKey(name))
                    {
                        throw new ValidationException($"第{rowIndex}行数据 {LogisticsGoodsCate.Sea.Name} 扩展名 {name} 已存在或重复，导入失败！");
                    }

                    seaNameDict.Add(name, true);
                }

                if (logisticsGoods.Cate == LogisticsGoodsCate.Air)
                {
                    if (airNameDict.ContainsKey(name))
                    {
                        throw new ValidationException($"第{rowIndex}行数据  {LogisticsGoodsCate.Air.Name}  扩展名 {name} 已存在或重复，导入失败！");
                    }

                    airNameDict.Add(name, true);
                }

                if (name.Length > 200)
                {
                    throw new ValidationException($"第{rowIndex}行数据  {cateName}  扩展名 {name} 太长，超过200长度，导入失败！");
                }

                logisticsGoods.Name = name.Trim();

                /*

                // 物品类型
                string goodsTypeStr = row.GetCell(cellIndex++)?.StringCellValue;
                if (string.IsNullOrWhiteSpace(goodsTypeStr))
                {
                    throw new ValidationException($"第{rowIndex}行数据 物品类型 不能为空，导入失败！");
                }

                LogisticsGoodsType goodsType = LogisticsGoodsType.GetByName(goodsTypeStr.Trim());
                if (goodsType == LogisticsGoodsType.None)
                {
                    throw new ValidationException($"第{rowIndex}行数据 物品类型 数据有误，导入失败！");
                }

                logisticsGoods.Type = goodsType;

                */

                // 是否品牌
                bool hasBrand = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否品牌 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否品牌 数据有误，导入失败！");

                logisticsGoods.HasBrand = hasBrand;

                // 品牌
                string brandName = row.GetCell(cellIndex++)?.StringCellValue;
                if (hasBrand && string.IsNullOrWhiteSpace(brandName))
                {
                    throw new ValidationException($"第{rowIndex}行数据 有品牌商品品牌名 不能为空，导入失败！");
                }

                logisticsGoods.BrandName = brandName;

                // 是否含电池或其他限制
                logisticsGoods.HasBattery = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否含电池或其他限制 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否含电池或其他限制 数据有误，导入失败！");

                // 是否含草木竹
                logisticsGoods.HasBamboo = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否含草木竹 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否含草木竹 数据有误，导入失败！");

                // 是否含实木包装
                logisticsGoods.HasWoodPackage = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否含实木包装 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否含实木包装 数据有误，导入失败！");

                // 是否特货类
                logisticsGoods.IsSpecial = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否特货类 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否特货类 数据有误，导入失败！");

                // 是否禁止类
                logisticsGoods.IsForbidden = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否禁止类 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否禁止类 数据有误，导入失败！");

                logisticsGoods.HsGoods = null;

                logisticsGoodsList.Add(logisticsGoods);
            }

            int loginUserId = loginUserService.GetLoginUserId();

            ITransaction transaction = null;
            try
            {
                transaction = session.BeginTransaction();

                foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
                {
                    LogisticsGoodsType logisticsGoodsType = logisticsGoodsTypeService.GetGoodsType(
                        logisticsGoods.HasBrand,
                        logisticsGoods.HasBattery,
                        logisticsGoods.HasBamboo,
                        logisticsGoods.HasWoodPackage,
                        logisticsGoods.IsSpecial,
                        logisticsGoods.IsForbidden);

                    logisticsGoods.Type = logisticsGoodsType;

                    logisticsGoods.AddUserId = loginUserId;
                    logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;

                    session.Save(logisticsGoods);
                }

                session.Flush();

                transaction.Commit();
            }
            catch (Exception exp)
            {
                transaction?.Rollback();

                log.Error(exp);

                throw;
            }

            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "导入成功！" },
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/warehouse/export")]
        [NoHibernateAutoTransaction]
        public IActionResult Export(LogisticsGoodsWarehouseListQueryParam queryParam)
        {
            string fileName = "扩展名添加" + ".xlsx";
            string sheetName = "扩展名添加";

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);

            IList<LogisticsGoodsInfo> logisticsGoodsList = GetWarehouseGoodsSearchListCriteria(queryParam)
                .List<LogisticsGoodsInfo>();

            int rowHeight = 22;
            ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
            ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
            ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

            // 扩展名    物品类型    是否品牌    品牌    是否含电池或其他限制    是否含草木竹    是否含实木包装    是否禁止类    中国海关申报名    中国海关编码
            int rowIndex = 0;

            IRow row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            int cellIndex = 0;

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            ICell cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("类别");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("扩展名");

            /*
            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("物品类型");
            */

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("是否品牌");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("品牌");

            sheet.SetColumnWidth(cellIndex, 18 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("是否含电池或其他限制");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("是否含草木竹");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("是否含实木包装");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("是否特货类");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("是否禁止类");

            /*
            sheet.SetColumnWidth(cellIndex, 10 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("状态");
            */

            foreach (LogisticsGoodsInfo goods in logisticsGoodsList)
            {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                // 类别
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.Cate?.Name);

                // 扩展名
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.Name);

                /*

                // 物品类型
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.Type.Name);

                */

                // 是否品牌
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(GetBoolStr(goods.HasBrand));

                // 品牌
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.BrandName);

                // 是否含电池或其他限制
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(GetBoolStr(goods.HasBattery));

                // 是否含草木竹
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(GetBoolStr(goods.HasBamboo));

                // 是否含实木包装
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(GetBoolStr(goods.HasWoodPackage));

                // 是否特货类
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(GetBoolStr(goods.IsSpecial));

                // 是否禁止类
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(GetBoolStr(goods.IsForbidden));

                /*
                // 状态
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(LogisticsGoodsStatus.GetById(goods.StatusId)?.Name);
                */
            }

            MemoryStream memoryStream = new MemoryStream();
            workbook.Write(memoryStream);
            memoryStream.Flush();

            return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
        }

        [NonAction]
        private string GetBoolStr(bool trueOrFalse)
        {
            if (trueOrFalse)
            {
                return "是";
            }
            else
            {
                return "否";
            }
        }

        [NonAction]
        private bool GetBoolValue(string trueOrFalseStr, string valueEmptyMessage, string valueErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(trueOrFalseStr))
            {
                throw new ValidationException(valueEmptyMessage);
            }
            else if (trueOrFalseStr == "是")
            {
                return true;
            }
            else if (trueOrFalseStr == "否")
            {
                return false;
            }
            else
            {
                throw new ValidationException(valueErrorMessage);
            }
        }
    }
}