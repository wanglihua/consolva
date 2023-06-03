using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Admin.Areas.Customer.Models.CustomerChargeAwardSetting;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

/// <summary>
/// 客户充值优惠设置 控制器
/// </summary>
[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerChargeAwardSetting)]
public class CustomerChargeAwardSettingController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerChargeAwardSettingController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/customer/charge/award/setting")]
    public IActionResult Index() {
        CustomerChargeAwardSettingIndexModel model = new CustomerChargeAwardSettingIndexModel();
        return View("CustomerChargeAwardSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/charge/award/setting/list")]
    public IActionResult List(CustomerChargeAwardSettingListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<CustomerChargeAwardSettingInfo>(e => customerChargeAwardSettingAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.OrderByAlias(() => customerChargeAwardSettingAlias!.AmountBegin).Asc;

        IList<CustomerChargeAwardSettingInfo> customerChargeAwardSettingList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<CustomerChargeAwardSettingInfo>();

        CustomerChargeAwardSettingListModel model = new CustomerChargeAwardSettingListModel(listPager) {
            CustomerChargeAwardSettingList = customerChargeAwardSettingList,
            CustomerChargeAwardSettingAlias = nameof(customerChargeAwardSettingAlias)
        };

        return View("CustomerChargeAwardSettingList", model);
    }

    private CustomerChargeAwardSettingInfo? customerChargeAwardSettingAlias = null;

    [NonAction]
    private IQueryOver<CustomerChargeAwardSettingInfo, CustomerChargeAwardSettingInfo> GetQueryOver(CustomerChargeAwardSettingListQueryParam queryParam) {
        var query = session.QueryOver<CustomerChargeAwardSettingInfo>(() => customerChargeAwardSettingAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/customer/customer/charge/award/setting/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        CustomerChargeAwardSettingInfo? customerChargeAwardSetting;

        if (id <= 0) {
            customerChargeAwardSetting = new CustomerChargeAwardSettingInfo();
        } else {
            customerChargeAwardSetting = session.Get<CustomerChargeAwardSettingInfo>(id);
            if (customerChargeAwardSetting == null) {
                throw new ValidationException($"指定的 客户充值优惠设置#{id} 不存在！");
            }
        }

        CustomerChargeAwardSettingDetailModel model = new CustomerChargeAwardSettingDetailModel {
            CustomerChargeAwardSettingId = customerChargeAwardSetting.Id,
            AmountBegin = customerChargeAwardSetting.AmountBegin,
            AmountEnd = customerChargeAwardSetting.AmountEnd,
            Award = customerChargeAwardSetting.Award
        };

        return View("CustomerChargeAwardSettingDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/charge/award/setting/save")]
    [Transactional]
    public IActionResult Save(CustomerChargeAwardSettingSaveModel model) {
        ValidationUtil.Validate<CustomerChargeAwardSettingSaveModelValidator, CustomerChargeAwardSettingSaveModel>(model);

        int customerChargeAwardSettingId = model.CustomerChargeAwardSettingId;

        bool isCreate = customerChargeAwardSettingId <= 0;

        CustomerChargeAwardSettingInfo customerChargeAwardSetting;

        if (isCreate) {
            customerChargeAwardSetting = new CustomerChargeAwardSettingInfo();
        } else {
            customerChargeAwardSetting = session.Get<CustomerChargeAwardSettingInfo>(customerChargeAwardSettingId);
            if (customerChargeAwardSetting == null) {
                throw new ValidationException($"指定的 客户充值优惠设置#{customerChargeAwardSettingId} 不存在！");
            }
        }

        customerChargeAwardSetting.AmountBegin = model.AmountBegin;
        customerChargeAwardSetting.AmountEnd = model.AmountEnd;
        customerChargeAwardSetting.Award = model.Award;

        if (isCreate) {
            session.Save(customerChargeAwardSetting);
        } else {
            session.Update(customerChargeAwardSetting);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/customer/customer/charge/award/setting/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的客户充值优惠设置！");

        var idList = param.IdList;

        foreach (int id in idList) {
            CustomerChargeAwardSettingInfo customerChargeAwardSetting = session.Load<CustomerChargeAwardSettingInfo>(id);
            session.Delete(customerChargeAwardSetting);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}