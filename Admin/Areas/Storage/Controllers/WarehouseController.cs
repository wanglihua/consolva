using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Storage.Models.Warehouse;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Storage.Controllers;

[StorageArea]
[AdminAuth(PassRes.Storage.Warehouse)]
public class WarehouseController : Controller {
    private readonly ISession session;
    private readonly WarehouseService warehouseService;

    public WarehouseController(ISession session, WarehouseService warehouseService) {
        this.session = session;
        this.warehouseService = warehouseService;
    }

    [HttpGet]
    [Route("/admin/warehouse")]
    public IActionResult Index() {
        var model = new WarehouseIndexModel();

        /*
         todo b 这个后面补上
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
    public IActionResult List(WarehouseListQueryParam queryParam) {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var warehouseList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<WarehouseInfo>();

        var model = new WarehouseListModel(listPager);
        model.WarehouseList = warehouseList;

        return View("WarehouseList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(WarehouseListQueryParam queryParam) {
        var codeQuery = queryParam.CodeQuery ?? string.Empty;
        var nameQuery = queryParam.NameQuery ?? string.Empty;

        var criteria = session.CreateCriteria<WarehouseInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(codeQuery) == false) {
            codeQuery = codeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<WarehouseInfo>(e => e.Code).IsInsensitiveLike(codeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(nameQuery) == false) {
            nameQuery = nameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<WarehouseInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/warehouse/detail/{id:int?}")]
    public IActionResult Detail(int? id) {
        WarehouseInfo? warehouse;

        if (id == null || id.Value == 0) {
            warehouse = new WarehouseInfo();
        } else {
            warehouse = session.Get<WarehouseInfo>(id.Value);
            if (warehouse == null) throw new ValidationException("指定的数据项不存在！");
        }

        var warehouseStatusList = WarehouseStatus.GetList();

        var model = new WarehouseDetailModel();
        model.Warehouse = warehouse;
        model.WarehouseStatusList = warehouseStatusList;

        return View("WarehouseDetail", model);
    }

    [HttpPost]
    [Route("/admin/warehouse/save")]
    [Transactional]
    public IActionResult Save(WarehouseDetailModel model) {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var warehouseFromUi = model.Warehouse;

        var warehouseId = warehouseFromUi.Id;

        var isCreate = warehouseId == 0;

        WarehouseInfo? warehouse;

        if (isCreate)
            warehouse = new WarehouseInfo();
        else
            warehouse = session.Get<WarehouseInfo>(warehouseId);

        warehouse.Code = warehouseFromUi.Code;
        warehouse.Name = warehouseFromUi.Name;
        warehouse.Description = warehouseFromUi.Description;
        warehouse.Address = warehouseFromUi.Address;
        warehouse.Phone = warehouseFromUi.Phone;
        warehouse.Zipcode = warehouseFromUi.Zipcode;
        warehouse.OuterStatus = warehouseFromUi.OuterStatus;
        warehouse.InnerStatus = warehouseFromUi.InnerStatus;
        warehouse.OrderNum = warehouseFromUi.OrderNum;

        if (isCreate)
            session.Save(warehouse);
        else
            session.Update(warehouse);

        session.Flush();

        warehouseService.InvalidWarehouseListCache();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", warehouse.Id }
        });
    }

    [HttpPost]
    [Route("/admin/warehouse/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (var id in idList) {
            var warehouse = session.Load<WarehouseInfo>(id);
            session.Delete(warehouse);
        }

        session.Flush();

        warehouseService.InvalidWarehouseListCache();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}