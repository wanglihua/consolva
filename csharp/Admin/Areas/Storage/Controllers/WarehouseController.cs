using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Storage.Models.Warehouse;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Storage.Controllers;

[AdminAuth]
// [AdminAuth(PassRes.Storage.Warehouse)]
[StorageArea]
public class WarehouseController : Controller
{
    private ISession session;

    public WarehouseController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/warehouse")]
    public IActionResult Index()
    {
        WarehouseIndexModel model = new WarehouseIndexModel();

        /*
         todo 这个后面补上
        IList<ShelfDuplicateInfo> shelfDuplicateList = session.Query<ShelfDuplicateInfo>()
            .OrderBy(e => e.WarehouseName).ThenBy(e => e.ShelfName)
            .ToList();
        model.ShelfDuplicateList = shelfDuplicateList;

        IList<StorageLocDuplicateInfo> storageDuplicateList = session.Query<StorageLocDuplicateInfo>()
            .OrderBy(e => e.WarehouseName).ThenBy(e => e.ShelfName).ThenBy(e => e.StorageLocRowNum).ThenBy(e => e.StorageLocColNum)
            .ToList();
        model.StorageDuplicateList = storageDuplicateList;
        */

        return View("WarehouseIndex", model);
    }

    [HttpPost]
    [Route("/admin/warehouse/list")]
    public IActionResult List(WarehouseListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<WarehouseInfo> warehouseList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<WarehouseInfo>();

        WarehouseListModel model = new WarehouseListModel(listPager);
        model.WarehouseList = warehouseList;

        return View("WarehouseList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(WarehouseListQueryParam queryParam)
    {
        string codeQuery = queryParam.CodeQuery ?? string.Empty;
        string nameQuery = queryParam.NameQuery ?? string.Empty;

        ICriteria criteria = session.CreateCriteria<WarehouseInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(codeQuery) == false)
        {
            codeQuery = codeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<WarehouseInfo>(e => e.Code).IsInsensitiveLike(codeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(nameQuery) == false)
        {
            nameQuery = nameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<WarehouseInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/warehouse/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        WarehouseWithAddressDescriptionInfo warehouse;

        if (id == null || id.Value == 0)
        {
            warehouse = new WarehouseWithAddressDescriptionInfo();
        }
        else
        {
            warehouse = session.Get<WarehouseWithAddressDescriptionInfo>(id.Value);
            if (warehouse == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        IList<WarehouseStatus> warehouseStatusList = WarehouseStatus.GetList();

        WarehouseDetailModel model = new WarehouseDetailModel();
        model.Warehouse = warehouse;
        model.WarehouseStatusList = warehouseStatusList;

        return View("WarehouseDetail", model);
    }

    [HttpPost]
    [Route("/admin/warehouse/save")]
    [Transactional]
    public IActionResult Save(WarehouseDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        WarehouseWithAddressDescriptionInfo warehouseFromUi = model.Warehouse;

        int warehouseId = warehouseFromUi.Id;

        bool isCreate = warehouseId == 0;

        WarehouseWithAddressDescriptionInfo warehouse;

        if (isCreate)
        {
            warehouse = new WarehouseWithAddressDescriptionInfo();
        }
        else
        {
            warehouse = session.Get<WarehouseWithAddressDescriptionInfo>(warehouseId);
        }

        warehouse.Code = warehouseFromUi.Code;
        warehouse.Name = warehouseFromUi.Name;
        warehouse.Description = warehouseFromUi.Description;
        warehouse.Address = warehouseFromUi.Address;
        warehouse.AddressDescription = warehouseFromUi.AddressDescription;
        warehouse.OuterStatus = warehouseFromUi.OuterStatus;
        warehouse.InnerStatus = warehouseFromUi.InnerStatus;
        warehouse.OrderNum = warehouseFromUi.OrderNum;

        if (isCreate)
        {
            session.Save(warehouse);
        }
        else
        {
            session.Update(warehouse);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", warehouse.Id }
        });
    }

    [HttpPost]
    [Route("/admin/warehouse/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            WarehouseInfo warehouse = session.Load<WarehouseInfo>(id);
            session.Delete(warehouse);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}