using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Admin.Areas.Customer.Models.CustomerPromotion;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

/// <summary>
/// 客户推广 控制器
/// </summary>
[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerPromotion)]
public class CustomerPromotionController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerPromotionController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/customer/promotion")]
    public IActionResult Index() {
        CustomerPromotionIndexModel model = new CustomerPromotionIndexModel();


        return View("CustomerPromotionIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/list")]
    public IActionResult List(CustomerPromotionListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<CustomerPromotionInfo>(e => customerPromotionAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => customerPromotionAlias!.Id).Desc;
            
        IList<CustomerPromotionInfo> customerPromotionList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<CustomerPromotionInfo>();

        /*
        foreach (CustomerPromotionInfo customerPromotion in customerPromotionList) {
        }
        */

        CustomerPromotionListModel model = new CustomerPromotionListModel(listPager);
        model.CustomerPromotionList = customerPromotionList;
        
        model.CustomerPromotionAlias = nameof(customerPromotionAlias);
        model.ParentAlias = nameof(parentAlias);
        model.ChildAlias = nameof(childAlias);        

        return View("CustomerPromotionList", model);
    }

    private CustomerPromotionInfo? customerPromotionAlias = null;
    private CustomerInfo? parentAlias = null;
    private CustomerInfo? childAlias = null;

    [NonAction]
    private IQueryOver<CustomerPromotionInfo, CustomerPromotionInfo> GetQueryOver(CustomerPromotionListQueryParam queryParam) {
        var query = session.QueryOver<CustomerPromotionInfo>(() => customerPromotionAlias!)
            .JoinAlias(() => customerPromotionAlias!.Parent, () => parentAlias)
            .JoinAlias(() => customerPromotionAlias!.Child, () => childAlias, JoinType.LeftOuterJoin);

        return query;
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        CustomerPromotionInfo? customerPromotion;

        if (id <= 0) {
            customerPromotion = new CustomerPromotionInfo();
        } else {
            customerPromotion = session.Get<CustomerPromotionInfo>(id);
            if (customerPromotion == null) {
                throw new ValidationException($"指定的 客户推广#{id} 不存在！");
            }
        }

        CustomerPromotionDetailModel model = new CustomerPromotionDetailModel();
        model.CustomerPromotionId = customerPromotion.Id;

        model.ParentId = customerPromotion.Parent?.Id ?? 0;
        model.ChildId = customerPromotion.Child?.Id ?? 0;
        model.OpenId = customerPromotion.OpenId;
        model.ChildPayAmount = customerPromotion.ChildPayAmount;


        return View("CustomerPromotionDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/save")]
    [Transactional]
    public IActionResult Save(CustomerPromotionSaveModel model) {
        ValidationUtil.Validate<CustomerPromotionSaveModelValidator, CustomerPromotionSaveModel>(model);

        int customerPromotionId = model.CustomerPromotionId;

        bool isCreate = customerPromotionId <= 0;

        CustomerPromotionInfo customerPromotion;

        if (isCreate) {
            customerPromotion = new CustomerPromotionInfo();
        } else {
            customerPromotion = session.Get<CustomerPromotionInfo>(customerPromotionId);
            if (customerPromotion == null){
                throw new ValidationException($"指定的 客户推广#{customerPromotionId} 不存在！");
            }
        }

        CustomerInfo parent = session.Get<CustomerInfo>(model.ParentId);
        CustomerInfo child = session.Get<CustomerInfo>(model.ChildId);
        customerPromotion.Parent = parent;
        customerPromotion.Child = child;
        customerPromotion.OpenId = model.OpenId;
        customerPromotion.ChildPayAmount = model.ChildPayAmount;

        if (isCreate) {
            session.Save(customerPromotion);
        } else {
            session.Update(customerPromotion);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        ValidationUtil.Validate<IdListParamValidator, IdListParam>(param);

        var idList = param.IdList;

        foreach (int id in idList) {
            CustomerPromotionInfo customerPromotion = session.Load<CustomerPromotionInfo>(id);
            session.Delete(customerPromotion);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}