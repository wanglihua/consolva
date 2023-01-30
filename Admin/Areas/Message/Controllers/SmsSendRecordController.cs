using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.Message.Models.SmsSendRecord;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Message;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Message.Controllers;

[MessageArea]
[AdminAuth(PassRes.CustomerMessage.MessageView)]
public class SmsSendRecordController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SmsSendRecordController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/sms/send/record")]
    public IActionResult Index() {
        SmsSendRecordIndexModel model = new SmsSendRecordIndexModel();
        return View("SmsSendRecordIndex", model);
    }

    [HttpPost]
    [Route("/admin/sms/send/record/list")]
    public IActionResult List(SmsSendRecordListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.Count(Projections.Property(nameof(SmsSendRecordInfo.Id))))
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<SmsSendRecordInfo> smsSendRecordList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<SmsSendRecordInfo>();

        foreach (SmsSendRecordInfo smsSendRecord in smsSendRecordList) {
        }

        SmsSendRecordListModel model = new SmsSendRecordListModel(listPager);
        model.SmsSendRecordList = smsSendRecordList;

        return View("SmsSendRecordList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(SmsSendRecordListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<SmsSendRecordInfo>();
        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.MobileQuery) == false) {
            string mobileQuery = queryParam.MobileQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<SmsSendRecordInfo>(e => e.Mobile == mobileQuery));
        }

        return criteria;
    }
}