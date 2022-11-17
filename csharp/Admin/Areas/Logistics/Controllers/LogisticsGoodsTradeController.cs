using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Core.Util;
using Domain;
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
using Web.Areas.Admin.Models.HsGoods;
using Web.Areas.Admin.Models.LogisticsGoodsTrade;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.Goods.GoodsTrade)]
    public class LogisticsGoodsTradeController : Controller
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private ISession session = null;
        private LoginUserService loginUserService = null;
        private LogisticsGoodsTypeService logisticsGoodsTypeService = null;
        private LogisticsGoodsImageService logisticsGoodsImageService = null;
        private LogisticsGoodsLogService logisticsGoodsLogService = null;

        public LogisticsGoodsTradeController(
            ISession session,
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
        [Route("/admin/logistics/goods/trade/{statusId:int}")]
        [NoHibernateAutoTransaction]
        public IActionResult Index(int statusId)
        {
            LogisticsGoodsTradeIndexModel model = new LogisticsGoodsTradeIndexModel();

            model.StatusId = statusId;
            model.GoodsTypeList = LogisticsGoodsType.GetInnerList();
            model.GoodsCateList = LogisticsGoodsCate.GetList();

            return View("LogisticsGoodsTradeIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/goods/trade/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(TradeGoodsSearchListQueryParam queryParam)
        {
            int totalCount = GetTradeGoodsSearchListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<LogisticsGoodsInfo> logisticsGoodsList = GetTradeGoodsSearchListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<LogisticsGoodsInfo>();

            IList<int> userIdList = logisticsGoodsList.Select(e => e.AddUserId).ToList();
            IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                // logisticsGoodsInfo
                UserInfo userInfo = userList.Where(e => e.Id == logisticsGoods.AddUserId).Take(1).SingleOrDefault();
                if (userInfo != null)
                {
                    logisticsGoods.AddUserUserNameCache = $"{userInfo.UserName}({userInfo.LoginName})";
                }

                IList<LogisticsOrderPhotoInfo> createExpressImageList = logisticsGoodsImageService.GetGoodsCreateExpressImageList(new List<string>() { logisticsGoods.CreateExpressTrackingNo });
                logisticsGoods.CreateExpressImageListCache = createExpressImageList;

                IList<LogisticsGoodsImageItem> goodsImageItemList = logisticsGoodsImageService.GetGoodsImageList(logisticsGoods.Id);

                IList<int> orderIdList = goodsImageItemList.Select(e => e.OrderId).Distinct().OrderByDescending(e => e).ToList();

                if (orderIdList.Count > 0)
                {
                    int orderId = orderIdList[0];
                    logisticsGoods.FirstOrderImageListCache = goodsImageItemList.Where(e => e.OrderId == orderId).OrderBy(e => e.Id).ToList();
                }

                if (orderIdList.Count > 1)
                {
                    int orderId = orderIdList[1];
                    logisticsGoods.SecondOrderImageListCache = goodsImageItemList.Where(e => e.OrderId == orderId).OrderBy(e => e.Id).ToList();
                }

                if (orderIdList.Count > 2)
                {
                    int orderId = orderIdList[2];
                    logisticsGoods.ThirdOrderImageListCache = goodsImageItemList.Where(e => e.OrderId == orderId).OrderBy(e => e.Id).ToList();
                }
            }

            LogisticsGoodsTradeListModel model = new LogisticsGoodsTradeListModel(listPager);
            model.LogisticsGoodsList = logisticsGoodsList;

            return View("LogisticsGoodsTradeList", model);
        }

        [NonAction]
        public ICriteria GetTradeGoodsSearchListCriteria(TradeGoodsSearchListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

            criteria = criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);
            criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.UpdateUser), nameof(LogisticsGoodsInfo.UpdateUser), JoinType.LeftOuterJoin);

            LogisticsGoodsStatus statusQuery = queryParam.StatusQuery;

            if (statusQuery == LogisticsGoodsStatus.NotConfirmed)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.NoPhoto || e.Status == LogisticsGoodsStatus.NotConfirmed));
            }

            if (statusQuery == LogisticsGoodsStatus.Confirmed)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.Confirmed));
            }

            LogisticsGoodsCate cateQuery = queryParam.CateQuery;
            if (cateQuery != null && cateQuery != LogisticsGoodsCate.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Cate == cateQuery));
            }

            LogisticsGoodsType logisticsGoodsType = queryParam.TypeQuery;
            if (logisticsGoodsType != null && logisticsGoodsType != LogisticsGoodsType.None)
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

            if (string.IsNullOrWhiteSpace(queryParam.CnHsCodeQuery) == false)
            {
                string cnHsCodeQuery = queryParam.CnHsCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.CnHsCode)}", cnHsCodeQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false)
            {
                string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.CnHsName)}", cnHsNameQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.AuHsCodeQuery) == false)
            {
                string auHsCodeQuery = queryParam.AuHsCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.AuHsCode)}", auHsCodeQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.AuHsNameQuery) == false)
            {
                string auHsNameQuery = queryParam.AuHsNameQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.AuHsName)}", auHsNameQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.UpdateUserQuery) == false)
            {
                string updateUserQuery = queryParam.UpdateUserQuery.Trim();
                UserInfo user = session.Query<UserInfo>()
                    .Where(e => e.LoginName == updateUserQuery || e.UserName == updateUserQuery)
                    .Take(1)
                    .SingleOrDefault();

                int userId = user?.Id ?? 0;
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.UpdateUser)}.{nameof(UserInfo.Id)}", userId));
            }

            if (queryParam.BeginUpdateDateQuery != null)
            {
                DateTime beginUpdateDataQuery = queryParam.BeginUpdateDateQuery.Value.Date;
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.UpdateTime >= beginUpdateDataQuery));
            }

            if (queryParam.EndUpdateDateQuery != null)
            {
                DateTime endUpdateDataQuery = queryParam.EndUpdateDateQuery.Value.Date.AddDays(1);
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.UpdateTime < endUpdateDataQuery));
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/logistics/goods/trade/delete")]
        public IActionResult Delete(int[] idList)
        {
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
        [Route("/admin/logistics/goods/trade/show/select/hs/code")]
        [NoHibernateAutoTransaction]
        public IActionResult ShowSelectHsCode()
        {
            LogisticsGoodsTradeSelectHsCodeModel model = new LogisticsGoodsTradeSelectHsCodeModel();

            return View("LogisticsGoodsTradeSelectHsCode", model);
        }

        [HttpPost]
        [Route("/admin/logistics/goods/trade/show/select/hs/code/list")]
        [NoHibernateAutoTransaction]
        public IActionResult ShowSelectHsCodeList(HsGoodsListQueryParam queryParam)
        {
            int totalCount = GetShowSelectHsCodeListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<HsGoodsInfo> hsGoodsList = GetShowSelectHsCodeListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<HsGoodsInfo>();

            foreach (HsGoodsInfo hsGoodsInfo in hsGoodsList)
            {
                // hsGoodsInfo
            }

            HsGoodsListModel model = new HsGoodsListModel(listPager);
            model.HsGoodsList = hsGoodsList;

            return View("LogisticsGoodsTradeSelectHsCodeList", model);
        }

        [NonAction]
        private ICriteria GetShowSelectHsCodeListCriteria(HsGoodsListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<HsGoodsInfo>();

            criteria = criteria.SetReadOnly(true);

            string cnHsNameQuery = queryParam.CnHsNameQuery;
            if (string.IsNullOrWhiteSpace(cnHsNameQuery) == false)
            {
                cnHsNameQuery = cnHsNameQuery.Trim();
                criteria = criteria.Add(Restrictions.On<HsGoodsInfo>(e => e.CnHsName).IsInsensitiveLike(cnHsNameQuery, MatchMode.Anywhere));
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/logistics/goods/trade/set/no/photo/status")]
        public IActionResult SetNoPhotoStatus(int[] idList)
        {
            if (idList == null || idList.Length == 0)
            {
                throw new ValidationException("请至少选择一条数据！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (int id in idList)
            {
                LogisticsGoodsInfo logisticsGoods = logisticsGoodsList
                    .Where(e => e.Id == id)
                    .Take(1)
                    .SingleOrDefault();

                if (logisticsGoods == null)
                {
                    throw new ValidationException($"指定的物品#{id}不存在！");
                }
            }

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                logisticsGoods.UpdateUser = loginUser;
                logisticsGoods.Status = LogisticsGoodsStatus.NoPhoto;
                session.Update(logisticsGoods);
            }

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.SetNoPhoto, logisticsGoods.Id, loginUserId);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "手工设置需拍照状态成功！" }
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/trade/import")]
        [NoHibernateAutoTransaction]
        public IActionResult Import(IFormFile excelFile)
        {
            if (excelFile == null)
            {
                throw new ValidationException("导入失败，文件未上传！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            IList<HsGoodsInfo> hsGoodsList = session
                .Query<HsGoodsInfo>()
                .ToList();

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

                cellIndex = 0;

                // 扩展名
                string name = row.GetCell(cellIndex++)?.StringCellValue;
                if (string.IsNullOrWhiteSpace(name))
                {
                    break;
                }

                // 物品类型，有这一列，但不起作用
                string goodsTypeStr = row.GetCell(cellIndex++)?.StringCellValue;

                /*
                if (string.IsNullOrWhiteSpace(goodsTypeStr))
                {
                    throw new ValidationException($"第{rowIndex}行数据 物品类型 不能为空，导入失败！");
                }

                LogisticsGoodsType goodsType = LogisticsGoodsType.GetByName(goodsTypeStr.Trim());
                if (goodsType == LogisticsGoodsType.None)
                {
                    throw new ValidationException($"第{rowIndex}行数据 物品类型 数据有误，导入失败！");
                }

                logisticsGoodsInDb.Type = goodsType;
                */


                // 是否品牌
                bool hasBrand = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否品牌 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否品牌 数据有误，导入失败！");


                // 品牌
                string brandName = row.GetCell(cellIndex++)?.StringCellValue;
                if (hasBrand && string.IsNullOrWhiteSpace(brandName))
                {
                    throw new ValidationException($"第{rowIndex}行数据 有品牌商品品牌名 不能为空，导入失败！");
                }

                // 是否含电池或其他限制
                bool hasBattery = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否含电池或其他限制 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否含电池或其他限制 数据有误，导入失败！");

                // 是否含草木竹
                bool hasBamboo = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否含草木竹 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否含草木竹 数据有误，导入失败！");

                // 是否含实木包装
                bool hasWoodPackage = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否含实木包装 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否含实木包装 数据有误，导入失败！");

                // 是否特货类
                bool isSpecial = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否特货类 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否特货类 数据有误，导入失败！");

                // 是否禁止类
                bool isForbidden = GetBoolValue(
                    row.GetCell(cellIndex++)?.StringCellValue,
                    $"第{rowIndex}行数据 是否禁止类 不能为空，导入失败！",
                    $"第{rowIndex}行数据 是否禁止类 数据有误，导入失败！");


                LogisticsGoodsInfo logisticsGoodsInDb = session
                    .Query<LogisticsGoodsInfo>()
                    .Where(e => e.Name == name
                                && e.HasBrand == hasBrand
                                && e.BrandName == brandName
                                && e.HasBattery == hasBattery
                                && e.HasBamboo == hasBamboo
                                && e.HasWoodPackage == hasWoodPackage
                                && e.IsSpecial == isSpecial
                                && e.IsForbidden == isForbidden)
                    .Take(1)
                    .SingleOrDefault();

                if (logisticsGoodsInDb == null)
                {
                    throw new ValidationException($"第{rowIndex}行数据指定的 扩展名+特征属性 不存在，导入失败！");
                }

                logisticsGoodsInDb.Name = name?.Trim();
                logisticsGoodsInDb.HasBrand = hasBrand;
                logisticsGoodsInDb.BrandName = brandName;
                logisticsGoodsInDb.HasBattery = hasBattery;
                logisticsGoodsInDb.HasBamboo = hasBamboo;
                logisticsGoodsInDb.HasWoodPackage = hasWoodPackage;
                logisticsGoodsInDb.IsSpecial = isSpecial;
                logisticsGoodsInDb.IsForbidden = isForbidden;

                // 中国海关申报名
                string cnHsName = row.GetCell(cellIndex++)?.StringCellValue;

                if (string.IsNullOrWhiteSpace(cnHsName))
                {
                    throw new ValidationException($"第{rowIndex}行数据 中国海关申报名 不能为空，导入失败！");
                }

                cnHsName = cnHsName.Trim();

                HsGoodsInfo hsGoods = hsGoodsList
                    .Where(e => e.CnHsName == cnHsName)
                    .Take(1)
                    .SingleOrDefault();

                if (hsGoods == null)
                {
                    throw new ValidationException($"第{rowIndex}行数据指定的 中国海关申报名 不存在，导入失败！");
                }

                logisticsGoodsInDb.HsGoods = hsGoods;

                logisticsGoodsList.Add(logisticsGoodsInDb);
            }

            ITransaction transaction = null;
            try
            {
                transaction = session.BeginTransaction();

                foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
                {
                    // 这里也可以不判断类型的，应该仓库那边导入的时候已经判断过一次了
                    LogisticsGoodsType logisticsGoodsType = logisticsGoodsTypeService.GetGoodsType(
                        logisticsGoods.HasBrand,
                        logisticsGoods.HasBattery,
                        logisticsGoods.HasBamboo,
                        logisticsGoods.HasWoodPackage,
                        logisticsGoods.IsSpecial,
                        logisticsGoods.IsForbidden);

                    logisticsGoods.Type = logisticsGoodsType;

                    logisticsGoods.UpdateUser = loginUser;
                    logisticsGoods.Status = LogisticsGoodsStatus.Confirmed;
                    session.Update(logisticsGoods);
                }

                session.Flush();

                foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
                {
                    logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Confirm, logisticsGoods.Id, loginUserId);
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
        [Route("/admin/logistics/goods/trade/export")]
        [NoHibernateAutoTransaction]
        public IActionResult Export(TradeGoodsSearchListQueryParam queryParam)
        {
            string fileName = "确认扩展名" + ".xlsx";
            string sheetName = "确认扩展名";

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);

            IList<LogisticsGoodsInfo> logisticsGoodsList = GetTradeGoodsSearchListCriteria(queryParam)
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
            cell.SetCellValue("扩展名");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("物品类型");

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

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("中国海关申报名");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("中国海关编码");

            foreach (LogisticsGoodsInfo goods in logisticsGoodsList)
            {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                // 扩展名
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.Name);

                // 物品类型
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.Type.Name);

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

                // 中国海关申报名
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.HsGoods?.CnHsName);

                // 中国海关编码
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(goods.HsGoods?.CnHsCode);
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