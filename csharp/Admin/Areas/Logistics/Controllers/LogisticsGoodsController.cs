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
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Service;
using Service.Logistics;
using Web.Areas.Admin.Models.LogisticsGoods;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.Goods.GoodsListByType)]
    public class LogisticsGoodsController : Controller
    {
        private ISession session = null;
        private LoginUserService loginUserService = null;
        private LogisticsGoodsTypeService logisticsGoodsTypeService = null;
        private LogisticsGoodsImageService logisticsGoodsImageService = null;
        private LogisticsGoodsLogService logisticsGoodsLogService = null;

        public LogisticsGoodsController(ISession session,
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
        [Route("/admin/logistics/goods/{typeId:int}")]
        [NoHibernateAutoTransaction]
        public IActionResult Index(int typeId)
        {
            LogisticsGoodsType logisticsGoodsType = LogisticsGoodsType.GetById(typeId);
            if (logisticsGoodsType == null || logisticsGoodsType == LogisticsGoodsType.None)
            {
                throw new ValidationException("指定的物品类型不存在！");
            }

            LogisticsGoodsIndexModel model = new LogisticsGoodsIndexModel();
            model.Type = logisticsGoodsType;
            model.GoodsCateList = LogisticsGoodsCate.GetList();

            return View("LogisticsGoodsIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/goods/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(LogisticsGoodsListQueryParam queryParam)
        {
            int totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<LogisticsGoodsInfo> logisticsGoodsList = GetListCriteria(queryParam)
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

            LogisticsGoodsListModel model = new LogisticsGoodsListModel(listPager);
            model.LogisticsGoodsList = logisticsGoodsList;

            return View("LogisticsGoodsList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(LogisticsGoodsListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

            criteria = criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

            LogisticsGoodsType logisticsGoodsType = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));

            LogisticsGoodsCate cateQuery = queryParam.CateQuery;
            if (cateQuery != null && cateQuery != LogisticsGoodsCate.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Cate == cateQuery));
            }

            string nameQuery = queryParam.NameQuery;
            if (string.IsNullOrWhiteSpace(nameQuery) == false)
            {
                nameQuery = nameQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
            }

            string cnHsCodeQuery = queryParam.CnHsCodeQuery;
            if (string.IsNullOrWhiteSpace(cnHsCodeQuery) == false)
            {
                cnHsCodeQuery = cnHsCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods.CnHsCode).IsInsensitiveLike(cnHsCodeQuery, MatchMode.Anywhere));
            }

            string cnHsNameQuery = queryParam.CnHsNameQuery;
            if (string.IsNullOrWhiteSpace(cnHsNameQuery) == false)
            {
                cnHsNameQuery = cnHsNameQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods.CnHsName).IsInsensitiveLike(cnHsNameQuery, MatchMode.Anywhere));
            }

            string enHsCodeQuery = queryParam.EnHsCodeQuery;
            if (string.IsNullOrWhiteSpace(enHsCodeQuery) == false)
            {
                enHsCodeQuery = enHsCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods.EnHsCode).IsInsensitiveLike(enHsCodeQuery, MatchMode.Anywhere));
            }

            string enHsNameQuery = queryParam.EnHsNameQuery;
            if (string.IsNullOrWhiteSpace(enHsNameQuery) == false)
            {
                enHsNameQuery = enHsNameQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.HsGoods.EnHsName).IsInsensitiveLike(enHsNameQuery, MatchMode.Anywhere));
            }

            return criteria;
        }

        [HttpGet]
        [Route("/admin/logistics/goods/detail/{id:int?}")]
        [NoHibernateAutoTransaction]
        public IActionResult Detail(int? id, int? statusId)
        {
            session.DefaultReadOnly = true;

            LogisticsGoodsInfo logisticsGoods = null;

            bool isCreate = false;

            if (id == null || id.Value == 0)
            {
                logisticsGoods = new LogisticsGoodsInfo();

                int loginUserId = loginUserService.GetLoginUserId();
                UserInfo userInfo = session.Get<UserInfo>(loginUserId);
                if (userInfo != null)
                {
                    logisticsGoods.AddUserUserNameCache = $"{userInfo.UserName}({userInfo.LoginName})";
                }

                isCreate = true;
            }
            else
            {
                logisticsGoods = session.Get<LogisticsGoodsInfo>(id.Value);
                if (logisticsGoods == null)
                {
                    throw new ValidationException("指定的数据项不存在！");
                }

                UserInfo userInfo = session.Get<UserInfo>(logisticsGoods.AddUserId);
                if (userInfo != null)
                {
                    logisticsGoods.AddUserUserNameCache = $"{userInfo.UserName}({userInfo.LoginName})";
                }

                isCreate = false;
            }

            LogisticsGoodsDetailModel model = new LogisticsGoodsDetailModel();
            model.StatusId = statusId;

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

            IList<LogisticsOrderPhotoInfo> createExpressImageList = logisticsGoodsImageService.GetGoodsCreateExpressImageList(new List<string>() {logisticsGoods.CreateExpressTrackingNo});
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
        [NoHibernateAutoTransaction]
        public IActionResult View(int id)
        {
            session.DefaultReadOnly = true;

            LogisticsGoodsInfo logisticsGoods = null;

            logisticsGoods = session.Get<LogisticsGoodsInfo>(id);
            if (logisticsGoods == null)
            {
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
        private void SetupDetailModel(LogisticsGoodsDetailModel model, LogisticsGoodsInfo logisticsGoods)
        {
            model.CateList = LogisticsGoodsCate.GetList();
            model.TypeList = LogisticsGoodsType.GetInnerList();
            model.StatusList = LogisticsGoodsStatus.GetList();

            UserInfo userInfo = session.Get<UserInfo>(logisticsGoods.AddUserId);
            if (userInfo != null)
            {
                logisticsGoods.AddUserUserNameCache = $"{userInfo.UserName}({userInfo.LoginName})";
            }

            model.LogisticsGoods = logisticsGoods;

            HsGoodsInfo hsGoods = logisticsGoods.HsGoods;

            if (hsGoods != null)
            {
                model.HsGoodsId = hsGoods.Id;
                model.TypeId = hsGoods.TypeId;

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
        public IActionResult Save(LogisticsGoodsDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
            }

            if (model.LogisticsGoods.HasBrand && string.IsNullOrWhiteSpace(model.LogisticsGoods.BrandName))
            {
                throw new ValidationException("品牌扩展名请填写品牌名！");
            }

            bool isCreate = model.LogisticsGoods.Id == 0;

            LogisticsGoodsInfo logisticsGoods = SaveInternal(model);

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "保存成功！"},
                {"isCreate", isCreate},
                {"id", logisticsGoods.Id}
            });
        }

        [NonAction]
        private LogisticsGoodsInfo SaveInternal(LogisticsGoodsDetailModel model)
        {
            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            LogisticsGoodsInfo logisticsGoodsFromUi = model.LogisticsGoods;
            int logisticsGoodsId = logisticsGoodsFromUi.Id;

            bool isCreate = logisticsGoodsId == 0;

            LogisticsGoodsInfo logisticsGoods = null;

            if (isCreate)
            {
                logisticsGoods = new LogisticsGoodsInfo();
                logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;
                logisticsGoods.AddUserId = loginUserService.GetLoginUserId();
            }
            else
            {
                logisticsGoods = session.Get<LogisticsGoodsInfo>(logisticsGoodsId);
            }

            logisticsGoods.Cate = logisticsGoodsFromUi.Cate;
            logisticsGoods.Name = logisticsGoodsFromUi.Name?.Trim();
            logisticsGoods.Type = logisticsGoodsFromUi.Type;
            logisticsGoods.HasBrand = logisticsGoodsFromUi.HasBrand;

            if (logisticsGoodsFromUi.HasBrand)
            {
                logisticsGoods.BrandName = logisticsGoodsFromUi.BrandName;
            }
            else
            {
                logisticsGoods.BrandName = string.Empty;
            }

            logisticsGoods.HasBattery = logisticsGoodsFromUi.HasBattery;
            logisticsGoods.HasBamboo = logisticsGoodsFromUi.HasBamboo;
            logisticsGoods.HasWoodPackage = logisticsGoodsFromUi.HasWoodPackage;
            logisticsGoods.IsSpecial = logisticsGoodsFromUi.IsSpecial;
            logisticsGoods.IsForbidden = logisticsGoodsFromUi.IsForbidden;
            logisticsGoods.CreateExpressTrackingNo = logisticsGoodsFromUi.CreateExpressTrackingNo;

            if (isCreate)
            {
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

            int hsGoodsId = model.HsGoodsId;

            if (model.HsGoodsAutoSelect == false)
            {
                string cnHsName = model.CnHsName;
                int count = session.Query<HsGoodsInfo>().Where(e => e.CnHsName == cnHsName).Count();
                if (count != 0)
                {
                    throw new ValidationException($"{cnHsName}已经存在，请直接选择！"); // 用异常可以回滚前面的保存，事务会撤回
                }

                HsGoodsInfo hsGoodsInfo = new HsGoodsInfo();

                hsGoodsInfo.CnHsCode = model.CnHsCode;
                hsGoodsInfo.CnHsName = model.CnHsName;
                hsGoodsInfo.EnHsCode = model.EnHsCode;
                hsGoodsInfo.EnHsName = model.EnHsName;
                hsGoodsInfo.Elem01 = model.Elem01;
                hsGoodsInfo.Elem02 = model.Elem02;
                hsGoodsInfo.Elem03 = model.Elem03;
                hsGoodsInfo.Elem04 = model.Elem04;
                hsGoodsInfo.Elem05 = model.Elem05;
                hsGoodsInfo.Elem06 = model.Elem06;
                hsGoodsInfo.Elem07 = model.Elem07;
                hsGoodsInfo.Elem08 = model.Elem08;

                hsGoodsInfo.AddUserId = loginUserId;

                session.Save(hsGoodsInfo);
                session.Flush();

                hsGoodsId = hsGoodsInfo.Id;
            }

            logisticsGoods.HsGoods = session.Load<HsGoodsInfo>(hsGoodsId);

            session.Update(logisticsGoods);
            session.Flush();

            // 保存图片， 其实这个现在一般也不用了
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

            return logisticsGoods;
        }

        [HttpPost]
        [Route("/admin/logistics/goods/save/confirm")]
        public IActionResult SaveConfirm(LogisticsGoodsDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            bool isCreate = model.LogisticsGoods.Id == 0;

            LogisticsGoodsInfo logisticsGoods = SaveInternal(model);

            if (logisticsGoods.Status == LogisticsGoodsStatus.NoPhoto)
            {
                throw new ValidationException("物品还没有任何关联照片，请设置物品创建单号，有单号相关订单照片后，再确认该物品名称！");
            }

            logisticsGoods.Status = LogisticsGoodsStatus.Confirmed;

            logisticsGoods.UpdateUser = loginUser;
            session.Update(logisticsGoods);
            session.Flush();

            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Confirm, logisticsGoods.Id, loginUserId);

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "保存确认成功！"},
                {"isCreate", isCreate},
                {"id", logisticsGoods.Id}
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/delete")]
        public IActionResult Delete(int[] idList)
        {
            if (idList == null || idList.Length == 0)
            {
                throw new ValidationException("未指定要删除的物品！");
            }

            foreach (int id in idList)
            {
                LogisticsGoodsInfo logisticsGoods = session.Load<LogisticsGoodsInfo>(id);
                session.Delete(logisticsGoods);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "删除成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/enable")]
        public IActionResult Enable(int[] idList)
        {
            if (idList == null || idList.Length == 0)
            {
                throw new ValidationException("未指定要启用的物品！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => idList.Contains(e.Id)).ToList();
            foreach (LogisticsGoodsInfo goods in goodsList)
            {
                if (goods.Status != LogisticsGoodsStatus.Disabled)
                {
                    throw new ValidationException($"物品【{goods.Name}】当前状态不是已禁用状态！");
                }
            }

            foreach (LogisticsGoodsInfo goods in goodsList)
            {
                goods.UpdateUser = loginUser;
                goods.Status = LogisticsGoodsStatus.Confirmed;
                session.Update(goods);
            }

            session.Flush();

            foreach (LogisticsGoodsInfo goods in goodsList)
            {
                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Enable, goods.Id, loginUserId);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "启用成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/disable")]
        public IActionResult Disable(int[] idList)
        {
            if (idList == null || idList.Length == 0)
            {
                throw new ValidationException("未指定要禁用的物品！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo loginUser = session.Get<UserInfo>(loginUserId);

            IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => idList.Contains(e.Id)).ToList();
            foreach (LogisticsGoodsInfo goods in goodsList)
            {
                if (goods.Status != LogisticsGoodsStatus.Confirmed)
                {
                    throw new ValidationException($"物品【{goods.Name}】当前状态不是已确认状态！");
                }
            }

            foreach (LogisticsGoodsInfo goods in goodsList)
            {
                goods.UpdateUser = loginUser;
                goods.Status = LogisticsGoodsStatus.Disabled;
                session.Update(goods);
            }

            session.Flush();

            foreach (LogisticsGoodsInfo goods in goodsList)
            {
                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Disable, goods.Id, loginUserId);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "禁用成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/logistics/goods/get/goods/type")]
        [NoHibernateAutoTransaction]
        public IActionResult GetGoodsType(LogisticsGoodsDetailModel model)
        {
            LogisticsGoodsType type = GetGoodsTypeByDetailModel(model.LogisticsGoods);

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "获取物品类型成功！"},
                {"goodsType", type.Id},
            });
        }

        [NonAction]
        private LogisticsGoodsType GetGoodsTypeByDetailModel(LogisticsGoodsInfo logisticsGoods)
        {
            LogisticsGoodsType logisticsGoodsType = logisticsGoodsTypeService.GetGoodsType(
                logisticsGoods.HasBrand,
                logisticsGoods.HasBattery,
                logisticsGoods.HasBamboo,
                logisticsGoods.HasWoodPackage,
                logisticsGoods.IsSpecial,
                logisticsGoods.IsForbidden);

            return logisticsGoodsType;
        }

        #region Image

        [HttpPost]
        [Route("/admin/logistics/goods/image/upload")]
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
                    {"success", false},
                    {"message", "只支持 png 和 jpg,jpeg 格式的图片文件！"},
                });
            }

            var result = logisticsGoodsImageService.SaveImageFile(imageFile);

            string imageFileUrl = result.Item1;
            string subPath = result.Item2;
            string fileName = result.Item3;

            LogisticsGoodsImageModel model = new LogisticsGoodsImageModel();
            model.ImageFileUrl = imageFileUrl;
            model.SubPath = subPath;
            model.FileName = fileName;

            return View("LogisticsGoodsImage", model);
        }

        [HttpPost]
        [Route("/admin/logistics/goods/image/delete")]
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
                {"success", true},
                {"message", "图片删除成功！"},
            });
        }

        #endregion

        [HttpPost]
        [Route("/admin/logistics/goods/export")]
        [NoHibernateAutoTransaction]
        public IActionResult Export(LogisticsGoodsListQueryParam queryParam)
        {
            var type = queryParam.TypeQuery;

            string fileName = type?.Name + "扩展名" + ".xlsx";
            string sheetName = type?.Name + "扩展名";

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);

            IList<LogisticsGoodsInfo> logisticsGoodsList = GetListCriteria(queryParam)
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
    }
}