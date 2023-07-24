using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

/// <summary>
/// 客户推广图片 控制器
/// </summary>
[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerPromotionImage)]
public class CustomerPromotionImageController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerPromotionImageController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/customer/promotion/image")]
    public IActionResult Index() {
        CustomerPromotionImageIndexModel model = new CustomerPromotionImageIndexModel();


        return View("CustomerPromotionImageIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/image/list")]
    public IActionResult List(CustomerPromotionImageListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<CustomerPromotionImageInfo>(e => customerPromotionImageAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => customerPromotionImageAlias!.Id).Desc;
            
        IList<CustomerPromotionImageInfo> customerPromotionImageList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<CustomerPromotionImageInfo>();

        /*
        foreach (CustomerPromotionImageInfo customerPromotionImage in customerPromotionImageList) {
        }
        */

        CustomerPromotionImageListModel model = new CustomerPromotionImageListModel(listPager);
        model.CustomerPromotionImageList = customerPromotionImageList;
        
        model.CustomerPromotionImageAlias = nameof(customerPromotionImageAlias);        

        return View("CustomerPromotionImageList", model);
    }

    private CustomerPromotionImageInfo? customerPromotionImageAlias = null;

    [NonAction]
    private IQueryOver<CustomerPromotionImageInfo, CustomerPromotionImageInfo> GetQueryOver(CustomerPromotionImageListQueryParam queryParam) {
        var query = session.QueryOver<CustomerPromotionImageInfo>(() => customerPromotionImageAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/image/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        CustomerPromotionImageInfo? customerPromotionImage;

        if (id <= 0) {
            customerPromotionImage = new CustomerPromotionImageInfo();
        } else {
            customerPromotionImage = session.Get<CustomerPromotionImageInfo>(id);
            if (customerPromotionImage == null) {
                throw new ValidationException($"指定的 客户推广图片#{id} 不存在！");
            }
        }

        CustomerPromotionImageDetailModel model = new CustomerPromotionImageDetailModel();
        model.CustomerPromotionImageId = customerPromotionImage.Id;

        model.ImageNum = customerPromotionImage.ImageNum;
        model.ImageFileName = customerPromotionImage.ImageFileName;


        return View("CustomerPromotionImageDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/image/save")]
    [Transactional]
    public IActionResult Save(CustomerPromotionImageSaveModel model) {
        ValidationUtil.Validate<CustomerPromotionImageSaveModelValidator, CustomerPromotionImageSaveModel>(model);

        int customerPromotionImageId = model.CustomerPromotionImageId;

        bool isCreate = customerPromotionImageId <= 0;

        CustomerPromotionImageInfo customerPromotionImage;

        if (isCreate) {
            customerPromotionImage = new CustomerPromotionImageInfo();
        } else {
            customerPromotionImage = session.Get<CustomerPromotionImageInfo>(customerPromotionImageId);
            if (customerPromotionImage == null){
                throw new ValidationException($"指定的 客户推广图片#{customerPromotionImageId} 不存在！");
            }
        }

        customerPromotionImage.ImageNum = model.ImageNum;
        customerPromotionImage.ImageFileName = model.ImageFileName;

        if (isCreate) {
            session.Save(customerPromotionImage);
        } else {
            session.Update(customerPromotionImage);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/image/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        ValidationUtil.Validate<IdListParamValidator, IdListParam>(param);

        var idList = param.IdList;

        foreach (int id in idList) {
            CustomerPromotionImageInfo customerPromotionImage = session.Load<CustomerPromotionImageInfo>(id);
            session.Delete(customerPromotionImage);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}