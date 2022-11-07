using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerCharacter;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerCharacter)]
public class CustomerCharacterController : Controller
{
    private readonly ISession session;

    public CustomerCharacterController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/character")]
    public IActionResult Index()
    {
        var model = new CustomerCharacterIndexModel();
        return View("CustomerCharacterIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/character/list")]
    public IActionResult List(CustomerCharacterListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerCharacterList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerCharacterInfo>();

        var model = new CustomerCharacterListModel(listPager);
        model.CustomerCharacterList = customerCharacterList;

        return View("CustomerCharacterList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerCharacterListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<CustomerCharacterInfo>();

        criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/character/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        CustomerCharacterInfo customerCharacter;

        if (id == null || id.Value == 0)
        {
            customerCharacter = new CustomerCharacterInfo();
        }
        else
        {
            customerCharacter = session.Get<CustomerCharacterInfo>(id.Value);
            if (customerCharacter == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new CustomerCharacterDetailModel();
        model.CustomerCharacter = customerCharacter;

        return View("CustomerCharacterDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/character/save")]
    [Transactional]
    public IActionResult Save(CustomerCharacterDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var customerCharacterFromUi = model.CustomerCharacter;
        var customerCharacterId = customerCharacterFromUi.Id;

        var isCreate = customerCharacterId == 0;

        CustomerCharacterInfo customerCharacter;

        if (isCreate)
            customerCharacter = new CustomerCharacterInfo();
        else
            customerCharacter = session.Get<CustomerCharacterInfo>(customerCharacterId);

        customerCharacter.Content = customerCharacterFromUi.Content;

        if (isCreate)
            session.Save(customerCharacter);
        else
            session.Update(customerCharacter);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/character/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var customerCharacter = session.Load<CustomerCharacterInfo>(id);
            session.Delete(customerCharacter);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}