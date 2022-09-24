using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Storage.Models.Shelf;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Storage.Controllers;

[StorageArea]
// [AdminAuth(PassRes.Storage.Warehouse)]
[AdminAuth]
public class ShelfController : Controller
{
    private ISession session = null;

    public ShelfController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/shelf/{warehouseId:int}")]
    public IActionResult Index(int warehouseId)
    {
        WarehouseInfo warehouseInfo = session.Get<WarehouseInfo>(warehouseId);
        if (warehouseInfo == null)
        {
            return Content("指定的仓库不存在！");
        }

        IList<ShelfType> shelfTypeList = ShelfType.GetList();

        ShelfIndexModel model = new ShelfIndexModel();
        model.WarehouseId = warehouseId;
        model.Warehouse = warehouseInfo;
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
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ShelfInfo> shelfList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ShelfInfo>();

        foreach (ShelfInfo shelfInfo in shelfList)
        {
            // shelfInfo.StatusName = ShelfStatus.GetById(shelfInfo.StatusId).Name;
            // shelfInfo.TypeName = ShelfType.GetById(shelfInfo.TypeId).Name;
        }

        ShelfListModel model = new ShelfListModel(listPager);
        model.ShelfList = shelfList;

        return View("ShelfList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ShelfListQueryParam queryParam)
    {
        int warehouseId = queryParam.WarehouseId;
        ShelfType shelfTypeQuery = queryParam.ShelfTypeQuery;
        string codeQuery = queryParam.CodeQuery;
        string nameQuery = queryParam.NameQuery;

        ICriteria criteria = session.CreateCriteria<ShelfInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<ShelfInfo>(e => e.Warehouse.Id == warehouseId));

        if (shelfTypeQuery != null && shelfTypeQuery != ShelfType.None)
        {
            criteria = criteria.Add(Restrictions.Where<ShelfInfo>(e => e.Type == shelfTypeQuery));
        }

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
        int shelfId = model.Id;
        int warehouseId = model.WarehouseId;

        session.DefaultReadOnly = true;

        ShelfInfo shelf = null;

        if (shelfId == 0)
        {
            shelf = new ShelfInfo();
            shelf.Warehouse = session.Get<WarehouseInfo>(warehouseId);
        }
        else
        {
            shelf = session.Get<ShelfInfo>(shelfId);
            if (shelf == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        model.Shelf = shelf;
        model.ShelfStatusList = ShelfStatus.GetList();
        model.ShelfTypeList = ShelfType.GetList();
        model.ShelfCateList = ShelfCate.GetList();
        model.WarehouseId = shelf.Warehouse?.Id ?? 0;

        return View("ShelfDetail", model);
    }

    [HttpPost]
    [Route("/admin/shelf/save")]
    [Transactional]
    public IActionResult Save(ShelfDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        ShelfInfo shelfFromUi = model.Shelf;
        int shelfId = shelfFromUi.Id;

        bool isCreate = shelfId == 0;

        ShelfInfo shelf = null;

        if (isCreate)
        {
            shelf = new ShelfInfo();
        }
        else
        {
            shelf = session.Get<ShelfInfo>(shelfId);
        }

        shelf.Code = shelfFromUi.Code;
        shelf.Name = shelfFromUi.Name;
        shelf.Type = shelfFromUi.Type;
        shelf.Cate = shelfFromUi.Cate;
        shelf.Status = shelfFromUi.Status;
        shelf.OrderNum = shelfFromUi.OrderNum;
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

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/shelf/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        /*
        foreach (int shelfId in idList)
        {
            ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);

            int storageLocCount = session.Query<StorageLocInfo>().Where(e => e.Shelf.Id == shelfId).Count();
            if (storageLocCount != 0)
            {
                throw new ValidationException($"货架{shelf.Name}下面有定义的仓位，请删除仓位后再删除货架！");
            }
        }
        */

        foreach (int id in idList)
        {
            ShelfInfo shelf = session.Load<ShelfInfo>(id);
            session.Delete(shelf);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}