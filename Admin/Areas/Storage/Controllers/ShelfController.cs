using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Storage.Models.Shelf;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Storage.Controllers;

[StorageArea]
[AdminAuth(PassRes.Storage.Warehouse)]
public class ShelfController : Controller
{
    private readonly ISession session;
    private readonly StorageLocExpressService storageLocExpressService;

    public ShelfController(ISession session, StorageLocExpressService storageLocExpressService)
    {
        this.session = session;
        this.storageLocExpressService = storageLocExpressService;
    }

    [HttpGet]
    [Route("/admin/shelf/{warehouseId:int}")]
    public IActionResult Index(int warehouseId)
    {
        var warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) return Content("指定的仓库不存在！");

        var shelfTypeList = ShelfType.GetList();

        var model = new ShelfIndexModel();
        model.WarehouseId = warehouseId;
        model.Warehouse = warehouse;
        model.ShelfTypeList = shelfTypeList;

        /*
        IList<ShelfDuplicateInfo> shelfDuplicateList = session.Query<ShelfDuplicateInfo>()
            .OrderBy(e => e.WarehouseName).ThenBy(e => e.ShelfName)
            .ToList();
        model.ShelfDuplicateList = shelfDuplicateList;

        IList<StorageLocDuplicateInfo> storageDuplicateList = session.Query<StorageLocDuplicateInfo>()
            .OrderBy(e => e.WarehouseName).ThenBy(e => e.ShelfName).ThenBy(e => e.StorageLocRowNum).ThenBy(e => e.StorageLocColNum)
            .ToList();
        model.StorageDuplicateList = storageDuplicateList;
        */

        return View("ShelfIndex", model);
    }

    [HttpPost]
    [Route("/admin/shelf/list")]
    public IActionResult List(ShelfListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var shelfList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ShelfInfo>();

        foreach (var shelfInfo in shelfList)
        {
            // shelfInfo.StatusName = ShelfStatus.GetById(shelfInfo.StatusId).Name;
            // shelfInfo.TypeName = ShelfType.GetById(shelfInfo.TypeId).Name;
        }

        var model = new ShelfListModel(listPager);
        model.ShelfList = shelfList;

        return View("ShelfList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ShelfListQueryParam queryParam)
    {
        var warehouseId = queryParam.WarehouseId;
        var shelfTypeQuery = queryParam.ShelfTypeQuery ?? ShelfType.None;
        var codeQuery = queryParam.CodeQuery ?? string.Empty;
        var nameQuery = queryParam.NameQuery ?? string.Empty;

        var criteria = session.CreateCriteria<ShelfInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<ShelfInfo>(e => e.Warehouse.Id == warehouseId));

        if (shelfTypeQuery != null && shelfTypeQuery != ShelfType.None) criteria = criteria.Add(Restrictions.Where<ShelfInfo>(e => e.Type == shelfTypeQuery));

        if (string.IsNullOrWhiteSpace(codeQuery) == false)
        {
            codeQuery = codeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<ShelfInfo>(e => e.Code).IsInsensitiveLike(codeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(nameQuery) == false)
        {
            nameQuery = nameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<ShelfInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/shelf/detail")]
    public IActionResult Detail(ShelfDetailModel model)
    {
        var shelfId = model.Id;
        var warehouseId = model.WarehouseId;

        ShelfInfo? shelf;

        if (shelfId == 0)
        {
            shelf = new ShelfInfo();
            shelf.Warehouse = session.Get<WarehouseInfo>(warehouseId);
        }
        else
        {
            shelf = session.Get<ShelfInfo>(shelfId);
            if (shelf == null) throw new ValidationException("指定的数据项不存在！");
        }

        model.Id = shelf.Id;
        model.Code = shelf.Code;
        model.Name = shelf.Name;
        model.Type = shelf.Type;
        model.Cate = shelf.Cate;
        model.Status = shelf.Status;
        model.OrderNum = shelf.OrderNum;
        model.WarehouseId = shelf.Warehouse?.Id ?? 0;
        model.ShelfStatusList = ShelfStatus.GetList();
        model.ShelfTypeList = ShelfType.GetList();
        model.ShelfCateList = ShelfCate.GetList();

        return View("ShelfDetail", model);
    }

    [HttpPost]
    [Route("/admin/shelf/save")]
    [Transactional]
    public IActionResult Save(ShelfSaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        var shelfId = model.Id;

        var isCreate = shelfId == 0;

        ShelfInfo? shelf;

        if (isCreate)
            shelf = new ShelfInfo();
        else
            shelf = session.Get<ShelfInfo>(shelfId);

        shelf.Code = model.Code;
        shelf.Name = model.Name;
        shelf.Type = model.Type;
        shelf.Cate = model.Cate;
        shelf.Status = model.Status;
        shelf.OrderNum = model.OrderNum;
        shelf.Warehouse = session.Get<WarehouseInfo>(model.WarehouseId);

        if (isCreate)
        {
            session.Save(shelf);
        }
        else
        {
            session.Update(shelf);
        }

        session.Flush();

        storageLocExpressService.UpdateShelf(shelf.Id);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/shelf/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int shelfId in idList)
        {
            ShelfInfo? shelf = session.Get<ShelfInfo>(shelfId);

            int storageLocId = session.Query<StorageLocInfo>()
                .Where(e => e.Shelf.Id == shelfId)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
            if (storageLocId != 0)
            {
                throw new ValidationException($"货架{shelf.Name}下面有定义的仓位，请删除仓位后再删除货架！");
            }
        }

        foreach (var id in idList)
        {
            var shelf = session.Load<ShelfInfo>(id);
            session.Delete(shelf);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }

    /// <summary>
    /// 更新仓位快递单
    /// </summary>
    [HttpPost]
    [Route("/admin/shelf/update/storage/loc/express")]
    [Transactional]
    public IActionResult UpdateStorageLocExpress()
    {
        // 所有仓库所有货架的都更新了的
        storageLocExpressService.UpdateAll();

        return Json(new JsonResultData {
            Success = true,
            Message = "更新仓位快递单成功！",
        });
    }
}