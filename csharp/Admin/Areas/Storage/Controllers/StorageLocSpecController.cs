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
    private ISession session;

    public StorageLocSpecController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/storage/loc/spec")]
    public IActionResult Index()
    {
        StorageLocSpecIndexModel model = new StorageLocSpecIndexModel();
        model.TypeList = ShelfType.GetList();

        return View("StorageLocSpecIndex", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/list")]
    public IActionResult List(StorageLocSpecListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<StorageLocSpecInfo> storageLocSpecList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<StorageLocSpecInfo>();

        StorageLocSpecListModel model = new StorageLocSpecListModel(listPager);
        model.StorageLocSpecList = storageLocSpecList;

        return View("StorageLocSpecList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(StorageLocSpecListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<StorageLocSpecInfo>();

        criteria = criteria.SetReadOnly(true);

        ShelfType shelfTypeQuery = queryParam.TypeQuery ?? ShelfType.None;
        if (shelfTypeQuery != ShelfType.None)
        {
            criteria = criteria.Add(Restrictions.Where<StorageLocSpecInfo>(e => e.ShelfType == shelfTypeQuery));
        }

        string codeQuery = queryParam.CodeQuery ?? string.Empty;
        if (string.IsNullOrWhiteSpace(codeQuery) == false)
        {
            codeQuery = codeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<StorageLocSpecInfo>(e => e.Code).IsInsensitiveLike(codeQuery, MatchMode.Anywhere));
        }

        string nameQuery = queryParam.NameQuery ?? string.Empty;
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
        StorageLocSpecInfo storageLocSpec;

        if (id == null || id.Value == 0)
        {
            storageLocSpec = new StorageLocSpecInfo();
        }
        else
        {
            storageLocSpec = session.Get<StorageLocSpecInfo>(id.Value);
            if (storageLocSpec == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        StorageLocSpecDetailModel model = new StorageLocSpecDetailModel();
        model.StorageLocSpec = storageLocSpec;
        model.TypeList = ShelfType.GetList();
        model.ColorList = WebColor.GetColorList();

        return View("StorageLocSpecDetail", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/save")]
    [Transactional]
    public IActionResult Save(StorageLocSpecDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        StorageLocSpecInfo storageLocSpecFromUi = model.StorageLocSpec;
        int storageLocSpecId = storageLocSpecFromUi.Id;

        bool isCreate = storageLocSpecId == 0;

        StorageLocSpecInfo storageLocSpec;

        if (isCreate)
        {
            storageLocSpec = new StorageLocSpecInfo();

            var storageLocSpecType = storageLocSpecFromUi.ShelfType;
            var code = storageLocSpecFromUi.Code;

            int count = session
                .Query<StorageLocSpecInfo>()
                .Where(e => e.ShelfType == storageLocSpecType
                            && e.Code == code)
                .Count();

            if (count != 0)
            {
                throw new ValidationException($"{storageLocSpecType.Name}类型中已有编号为{code}的库位规格！");
            }
        }
        else
        {
            storageLocSpec = session.Get<StorageLocSpecInfo>(storageLocSpecId);

            var storageLocSpecType = storageLocSpecFromUi.ShelfType;
            var code = storageLocSpecFromUi.Code;

            int count = session
                .Query<StorageLocSpecInfo>()
                .Where(e => e.Id != storageLocSpecId
                            && e.ShelfType == storageLocSpecType
                            && e.Code == code)
                .Count();

            if (count != 0)
            {
                throw new ValidationException($"{storageLocSpecType.Name}类型中已有编号为{code}的库位规格！");
            }
        }

        storageLocSpec.ShelfType = storageLocSpecFromUi.ShelfType;
        storageLocSpec.Code = storageLocSpecFromUi.Code;

        storageLocSpec.Name = storageLocSpecFromUi.Name;
        if (storageLocSpec.Name == null)
        {
            storageLocSpec.Name = string.Empty;
        }

        storageLocSpec.Width = storageLocSpecFromUi.Width;
        storageLocSpec.Height = storageLocSpecFromUi.Height;
        storageLocSpec.Length = storageLocSpecFromUi.Length;
        storageLocSpec.Color = storageLocSpecFromUi.Color;

        if (isCreate)
        {
            session.Save(storageLocSpec);
        }
        else
        {
            session.Update(storageLocSpec);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/spec/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int specId in idList)
        {
            StorageLocSpecInfo storageLocSpec = session.Get<StorageLocSpecInfo>(specId);

            int count = session.Query<StorageLocInfo>().Where(e => e.Spec.Id == specId).Count();
            if (count != 0)
            {
                throw new ValidationException($"仓位规格{storageLocSpec.Name}有仓位正在使用中，删除失败！");
            }
        }

        foreach (int id in idList)
        {
            StorageLocSpecInfo storageLocSpec = session.Load<StorageLocSpecInfo>(id);
            session.Delete(storageLocSpec);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}