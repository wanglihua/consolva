using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Message;
using shunda.Admin.Areas.Message.Models.CustomerMessageClientUrl;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Message;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Message.Controllers;

/// <summary>
/// 客户消息客户端URL 控制器
/// </summary>
[MessageArea]
[AdminAuth(PassRes.CustomerMessage.MessageSetting)]
public class CustomerMessageClientUrlController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CustomerMessageClientUrlService customerMessageClientUrlService;

    public CustomerMessageClientUrlController(ISession session, CustomerMessageClientUrlService customerMessageClientUrlService)
    {
        this.session = session;
        this.customerMessageClientUrlService = customerMessageClientUrlService;
    }

    [HttpGet]
    [Route("/admin/message/customer/message/client/url")]
    public IActionResult Index()
    {
        CustomerMessageClientUrlIndexModel model = new CustomerMessageClientUrlIndexModel();

        model.MessageTypeList = CustomerMessageType.GetList();
        model.TerminalTypeList = TerminalType.GetList();

        return View("CustomerMessageClientUrlIndex", model);
    }

    [HttpPost]
    [Route("/admin/message/customer/message/client/url/list")]
    public IActionResult List(CustomerMessageClientUrlListQueryParam queryParam)
    {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<CustomerMessageClientUrlInfo>(e => customerMessageClientUrlAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending)
        {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        }
        else
        {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => customerMessageClientUrlAlias!.Id).Desc;

        IList<CustomerMessageClientUrlInfo> customerMessageClientUrlList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<CustomerMessageClientUrlInfo>();

        /*
        foreach (CustomerMessageClientUrlInfo customerMessageClientUrl in customerMessageClientUrlList)
        {
        }
        */

        CustomerMessageClientUrlListModel model = new CustomerMessageClientUrlListModel(listPager);
        model.CustomerMessageClientUrlList = customerMessageClientUrlList;

        model.CustomerMessageClientUrlAlias = nameof(customerMessageClientUrlAlias);

        return View("CustomerMessageClientUrlList", model);
    }

    private CustomerMessageClientUrlInfo? customerMessageClientUrlAlias = null;

    [NonAction]
    private IQueryOver<CustomerMessageClientUrlInfo, CustomerMessageClientUrlInfo> GetQueryOver(CustomerMessageClientUrlListQueryParam queryParam)
    {
        var query = session.QueryOver<CustomerMessageClientUrlInfo>(() => customerMessageClientUrlAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/message/customer/message/client/url/detail")]
    public IActionResult Detail(IdParam param)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        var id = param.Id;

        CustomerMessageClientUrlInfo? customerMessageClientUrl;

        if (id <= 0)
        {
            customerMessageClientUrl = new CustomerMessageClientUrlInfo();
        }
        else
        {
            customerMessageClientUrl = session.Get<CustomerMessageClientUrlInfo>(id);
            if (customerMessageClientUrl == null)
            {
                throw new ValidationException($"指定的 客户消息客户端URL#{id} 不存在！");
            }
        }

        CustomerMessageClientUrlDetailModel model = new CustomerMessageClientUrlDetailModel();
        model.CustomerMessageClientUrlId = customerMessageClientUrl.Id;

        model.MessageType = customerMessageClientUrl.MessageType;
        model.TerminalType = customerMessageClientUrl.TerminalType;
        model.ClientUrl = customerMessageClientUrl.ClientUrl;

        model.TerminalTypeList = TerminalType.GetList();

        var messageTypeListAll = CustomerMessageType.GetList();
        var messageTypeListExist = session.Query<CustomerMessageClientUrlInfo>().Select(e => e.MessageType).ToList();
        model.MessageTypeList = messageTypeListAll.Where(e => messageTypeListExist.Contains(e) == false).ToList();

        return View("CustomerMessageClientUrlDetail", model);
    }

    [HttpPost]
    [Route("/admin/message/customer/message/client/url/save")]
    [Transactional]
    public IActionResult Save(CustomerMessageClientUrlSaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int customerMessageClientUrlId = model.CustomerMessageClientUrlId;

        bool isCreate = customerMessageClientUrlId <= 0;

        CustomerMessageClientUrlInfo customerMessageClientUrl;

        if (isCreate)
        {
            customerMessageClientUrl = new CustomerMessageClientUrlInfo();
        }
        else
        {
            customerMessageClientUrl = session.Get<CustomerMessageClientUrlInfo>(customerMessageClientUrlId);
            if (customerMessageClientUrl == null)
            {
                throw new ValidationException($"指定的 客户消息客户端URL#{customerMessageClientUrlId} 不存在！");
            }
        }

        customerMessageClientUrl.MessageType = model.MessageType;
        customerMessageClientUrl.TerminalType = model.TerminalType;
        customerMessageClientUrl.ClientUrl = model.ClientUrl;

        if (isCreate)
        {
            session.Save(customerMessageClientUrl);
        }
        else
        {
            session.Update(customerMessageClientUrl);
        }

        session.Flush();

        customerMessageClientUrlService.MakeCacheExpire();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/message/customer/message/client/url/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param)
    {
        param.Validate("请选择要删除的客户消息客户端URL！");

        var idList = param.IdList;

        foreach (int id in idList)
        {
            CustomerMessageClientUrlInfo customerMessageClientUrl = session.Load<CustomerMessageClientUrlInfo>(id);
            session.Delete(customerMessageClientUrl);
        }

        session.Flush();

        customerMessageClientUrlService.MakeCacheExpire();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}