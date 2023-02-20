using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Storage.Models.StorageLocSpec;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Storage.Controllers;

[StorageArea]
[AdminAuth(PassRes.Storage.StorageLocSpec)]
public class StorageLocSpecController : Controller
{
    private readonly ISession session;

    public StorageLocSpecController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/storage/loc/spec")]
    public IActionResult Index()
    {
        var model = new StorageLocSpecIndexModel();
        model.TypeList = ShelfType.GetList();

        return View("StorageLocSpecIndex", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/list")]
    public IActionResult List(StorageLocSpecListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var storageLocSpecList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<StorageLocSpecInfo>();

        var model = new StorageLocSpecListModel(listPager);
        model.StorageLocSpecList = storageLocSpecList;

        return View("StorageLocSpecList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(StorageLocSpecListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<StorageLocSpecInfo>();

        criteria = criteria.SetReadOnly(true);

        var shelfTypeQuery = queryParam.TypeQuery ?? ShelfType.None;
        if (shelfTypeQuery != ShelfType.None) criteria = criteria.Add(Restrictions.Where<StorageLocSpecInfo>(e => e.ShelfType == shelfTypeQuery));

        var codeQuery = queryParam.CodeQuery ?? string.Empty;
        if (string.IsNullOrWhiteSpace(codeQuery) == false)
        {
            codeQuery = codeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<StorageLocSpecInfo>(e => e.Code).IsInsensitiveLike(codeQuery, MatchMode.Anywhere));
        }

        var nameQuery = queryParam.NameQuery ?? string.Empty;
        if (string.IsNullOrWhiteSpace(nameQuery) == false)
        {
            nameQuery = nameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<StorageLocSpecInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        StorageLocSpecInfo? storageLocSpec;

        if (id == null || id.Value == 0)
        {
            storageLocSpec = new StorageLocSpecInfo();
        }
        else
        {
            storageLocSpec = session.Get<StorageLocSpecInfo>(id.Value);
            if (storageLocSpec == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new StorageLocSpecDetailModel();
        model.StorageLocSpec = storageLocSpec;
        model.TypeList = ShelfType.GetList();
        model.ColorList = WebColor.GetColorList();

        return View("StorageLocSpecDetail", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/save")]
    [Transactional]
    public IActionResult Save(StorageLocSpecSaveModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var storageLocSpecFromUi = model.StorageLocSpec;
        var storageLocSpecId = storageLocSpecFromUi.Id;

        var isCreate = storageLocSpecId == 0;

        StorageLocSpecInfo? storageLocSpec;

        if (isCreate)
        {
            storageLocSpec = new StorageLocSpecInfo();

            var storageLocSpecType = storageLocSpecFromUi.ShelfType;
            var code = storageLocSpecFromUi.Code;

            var count = session
                .Query<StorageLocSpecInfo>()
                .Where(e => e.ShelfType == storageLocSpecType
                            && e.Code == code)
                .Count();

            if (count != 0) throw new ValidationException($"{storageLocSpecType.Name}类型中已有编号为{code}的库位规格！");
        }
        else
        {
            storageLocSpec = session.Get<StorageLocSpecInfo>(storageLocSpecId);

            var storageLocSpecType = storageLocSpecFromUi.ShelfType;
            var code = storageLocSpecFromUi.Code;

            var count = session
                .Query<StorageLocSpecInfo>()
                .Where(e => e.Id != storageLocSpecId
                            && e.ShelfType == storageLocSpecType
                            && e.Code == code)
                .Count();

            if (count != 0) throw new ValidationException($"{storageLocSpecType.Name}类型中已有编号为{code}的库位规格！");
        }

        storageLocSpec.ShelfType = storageLocSpecFromUi.ShelfType;
        storageLocSpec.Code = storageLocSpecFromUi.Code;

        storageLocSpec.Name = storageLocSpecFromUi.Name;
        if (storageLocSpec.Name == null) storageLocSpec.Name = string.Empty;

        storageLocSpec.Width = storageLocSpecFromUi.Width;
        storageLocSpec.Height = storageLocSpecFromUi.Height;
        storageLocSpec.Length = storageLocSpecFromUi.Length;
        storageLocSpec.Color = storageLocSpecFromUi.Color;
        storageLocSpec.OrderNum = storageLocSpecFromUi.OrderNum;

        if (isCreate)
            session.Save(storageLocSpec);
        else
            session.Update(storageLocSpec);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var specId in idList)
        {
            var storageLocSpec = session.Get<StorageLocSpecInfo>(specId);

            var count = session.Query<StorageLocInfo>().Where(e => e.Spec.Id == specId).Count();
            if (count != 0) throw new ValidationException($"仓位规格{storageLocSpec.Name}有仓位正在使用中，删除失败！");
        }

        foreach (var id in idList)
        {
            var storageLocSpec = session.Load<StorageLocSpecInfo>(id);
            session.Delete(storageLocSpec);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}