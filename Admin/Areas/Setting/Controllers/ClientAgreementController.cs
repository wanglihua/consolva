using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Setting.Models.ClientAgreement;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Client;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

[SettingArea]
[AdminAuth(PassRes.Setting.ClientAgreement)]
public class ClientAgreementController : Controller {
    private ISession session;
    private LoginUserService loginUserService;

    public ClientAgreementController(ISession session, LoginUserService loginUserService) {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/client/agreement")]
    public IActionResult Index() {
        ClientAgreementIndexModel model = new ClientAgreementIndexModel();
        return View("ClientAgreementIndex", model);
    }

    [HttpPost]
    [Route("/admin/client/agreement/list")]
    public IActionResult List(ClientAgreementListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ClientAgreementInfo> clientAgreementList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ClientAgreementInfo>();

        foreach (ClientAgreementInfo clientAgreementInfo in clientAgreementList) {
            // clientAgreementInfo
        }

        ClientAgreementListModel model = new ClientAgreementListModel(listPager);
        model.ClientAgreementList = clientAgreementList;

        return View("ClientAgreementList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ClientAgreementListQueryParam queryParam) {
        string? titleQuery = queryParam.TitleQuery;
        string? contentQuery = queryParam.ContentQuery;
        string? countryQuery = queryParam.CountryQuery;

        ICriteria criteria = session.CreateCriteria<ClientAgreementInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(titleQuery) == false) {
            titleQuery = titleQuery.Trim();
            criteria = criteria.Add(Restrictions.On<ClientAgreementInfo>(e => e.Title).IsInsensitiveLike(titleQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(contentQuery) == false) {
            contentQuery = contentQuery.Trim();
            criteria = criteria.Add(Restrictions.On<ClientAgreementInfo>(e => e.Content).IsInsensitiveLike(contentQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(countryQuery) == false) {
            countryQuery = countryQuery.Trim();
            criteria = criteria.Add(Restrictions.On<ClientAgreementInfo>(e => e.Country).IsInsensitiveLike(countryQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/client/agreement/detail/{id:int?}")]
    public IActionResult Detail(int? id) {
        session.DefaultReadOnly = true;

        ClientAgreementInfo clientAgreement;

        if (id == null || id.Value == 0) {
            clientAgreement = new ClientAgreementInfo();
            clientAgreement.Creator = session.Get<UserInfo>(loginUserService.GetLoginUserId());
        } else {
            clientAgreement = session.Get<ClientAgreementInfo>(id.Value);
            if (clientAgreement == null) {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        ClientAgreementDetailModel model = new ClientAgreementDetailModel {
            Id = clientAgreement.Id,
            Title = clientAgreement.Title,
            Content = clientAgreement.Content,
            Country = clientAgreement.Country,
            Status = clientAgreement.Status,
            OrderNum = clientAgreement.OrderNum,
        };

        model.CountryList = Country.GetNoChinaList();
        model.StatusList = ClientAgreementStatus.GetList();

        return View("ClientAgreementDetail", model);
    }

    [HttpPost]
    [Route("/admin/client/agreement/save")]
    [Transactional]
    public IActionResult Save(ClientAgreementDetailModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int clientAgreementId = model.Id;

        bool isCreate = clientAgreementId == 0;

        ClientAgreementInfo clientAgreement;

        if (isCreate) {
            clientAgreement = new ClientAgreementInfo();
            clientAgreement.Status = ClientAgreementStatus.Disabled; // 加在这里是为了，后面发布时间设置的判断
        } else {
            clientAgreement = session.Get<ClientAgreementInfo>(clientAgreementId);
        }

        clientAgreement.Title = model.Title;
        clientAgreement.Content = model.Content;
        clientAgreement.Country = model.Country;
        clientAgreement.OrderNum = model.OrderNum;

        if (clientAgreement.Status == ClientAgreementStatus.Disabled && model.Status == ClientAgreementStatus.Enabled) {
            clientAgreement.PublishTime = DateTime.Now;
        }

        clientAgreement.Status = model.Status; // 放在上面发布时间判断的后面
        clientAgreement.Creator = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        if (isCreate) {
            session.Save(clientAgreement);
        } else {
            session.Update(clientAgreement);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", clientAgreement.Id }
        });
    }

    [HttpPost]
    [Route("/admin/client/agreement/delete")]
    public IActionResult Delete(int[] idList) {
        foreach (int id in idList) {
            ClientAgreementInfo clientAgreement = session.Load<ClientAgreementInfo>(id);
            session.Delete(clientAgreement);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}