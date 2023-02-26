using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.Setting.Models.ClientNotice;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

/// <summary>
/// 客户端公告 控制器
/// </summary>
[SettingArea]
[AdminAuth(PassRes.Setting.ClientNotice)]
public class ClientNoticeController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;

    public ClientNoticeController(
        ISession session,
        LoginUserService loginUserService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/setting/client/notice")]
    public IActionResult Index() {
        ClientNoticeIndexModel model = new ClientNoticeIndexModel();


        return View("ClientNoticeIndex", model);
    }

    [HttpPost]
    [Route("/admin/setting/client/notice/list")]
    public IActionResult List(ClientNoticeListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<ClientNoticeInfo>(e => clientNoticeAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => clientNoticeAlias!.Id).Desc;

        IList<ClientNoticeInfo> clientNoticeList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<ClientNoticeInfo>();

        /*
        foreach (ClientNoticeInfo clientNotice in clientNoticeList) {
        }
        */

        ClientNoticeListModel model = new ClientNoticeListModel(listPager);
        model.ClientNoticeList = clientNoticeList;

        model.ClientNoticeAlias = nameof(clientNoticeAlias);
        model.EditorUserAlias = nameof(editorUserAlias);

        return View("ClientNoticeList", model);
    }

    private ClientNoticeInfo? clientNoticeAlias = null;
    private UserInfo? editorUserAlias = null;

    [NonAction]
    private IQueryOver<ClientNoticeInfo, ClientNoticeInfo> GetQueryOver(ClientNoticeListQueryParam queryParam) {
        var query = session.QueryOver<ClientNoticeInfo>(() => clientNoticeAlias!)
            .JoinAlias(() => clientNoticeAlias!.EditorUser, () => editorUserAlias);

        return query;
    }

    [HttpPost]
    [Route("/admin/setting/client/notice/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        ClientNoticeInfo? clientNotice;

        if (id <= 0) {
            clientNotice = new ClientNoticeInfo();
        } else {
            clientNotice = session.Get<ClientNoticeInfo>(id);
            if (clientNotice == null) {
                throw new ValidationException($"指定的 客户端公告#{id} 不存在！");
            }
        }

        ClientNoticeDetailModel model = new ClientNoticeDetailModel();
        model.ClientNoticeId = clientNotice.Id;

        model.Title = clientNotice.Title;
        model.Url = clientNotice.Url;
        model.ListTop = clientNotice.ListTop;
        model.Important = clientNotice.Important;


        return View("ClientNoticeDetail", model);
    }

    [HttpPost]
    [Route("/admin/setting/client/notice/save")]
    [Transactional]
    public IActionResult Save(ClientNoticeSaveModel model) {
        ValidationUtil.Validate<ClientNoticeSaveModelValidator, ClientNoticeSaveModel>(model);

        int clientNoticeId = model.ClientNoticeId;

        bool isCreate = clientNoticeId <= 0;

        ClientNoticeInfo clientNotice;

        if (isCreate) {
            clientNotice = new ClientNoticeInfo();
        } else {
            clientNotice = session.Get<ClientNoticeInfo>(clientNoticeId);
            if (clientNotice == null) {
                throw new ValidationException($"指定的 客户端公告#{clientNoticeId} 不存在！");
            }
        }

        var editorUser = loginUserService.GetLoginUser();
        
        clientNotice.Title = model.Title;
        clientNotice.Url = model.Url;
        clientNotice.EditorUser = editorUser;
        clientNotice.ListTop = model.ListTop;
        clientNotice.Important = model.Important;

        // 每次更新时候都会更新这个时间，哪怕一点内容也没改，也就同时会更新 update time
        clientNotice.EditTime = DateTime.Now;

        if (isCreate) {
            session.Save(clientNotice);
        } else {
            session.Update(clientNotice);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/setting/client/notice/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的客户端公告！");

        var idList = param.IdList;

        foreach (int id in idList) {
            ClientNoticeInfo clientNotice = session.Load<ClientNoticeInfo>(id);
            session.Delete(clientNotice);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}